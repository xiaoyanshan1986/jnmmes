using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using ServiceCenter.Client.Mvc.Areas.RPT.Models;
using ServiceCenter.MES.Service.Client.RPT;
using ServiceCenter.MES.Service.Contract.RPT;
using ServiceCenter.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ServiceCenter.Client.Mvc.Areas.RPT.Controllers
{
    public class SemiProductionDataController : Controller
    {
        SortedList<string, string> sl = new SortedList<string, string>();
        Dictionary<string, string> dicColumn = new Dictionary<string, string>()
        {
            {"CUR_DAY","日期"},
            {"LINE_CODE","线别"},
            {"LOCATION_NAME","车间"},
            {"SUM_VALUE","合计"},
            {"RATE_VALUE","比率"}
        };
        // GET: QM/SemiProductionData
        #region ForLine 合格率日报 分线别
        public ActionResult Index()
        {
            return View(new SemiProductionViewModels());
        }
        public ActionResult IndexOfDefectPOS()
        {
            return View(new SemiProductionViewModels());
        }

        public ActionResult GetHighchartsResult(SemiProductionViewModels model)
        {
            DataTable dtData = new DataTable();
            DataTable dtNew = new DataTable();
            string strx = string.Empty, stry = string.Empty, strup = string.Empty, strlow = string.Empty;

            using (QMSemiProductionServiceClient client = new QMSemiProductionServiceClient())
            {
                MethodReturnResult<DataSet> rst = client.GetSemiProdQtyForLine(new QMSemiProductionGetParameter()
                {
                    LocationName = model.LocationName,
                    IsProdReport = model.IsProdReport,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate
                });
                if (rst.Code <= 0 && rst.Data != null && rst.Data.Tables.Count > 0)
                {
                    dtData = rst.Data.Tables[0];
                    List<SemiProductionViewModels> list_curday = new List<SemiProductionViewModels>();
                    List<SemiProductionViewModels> list_curday_line = new List<SemiProductionViewModels>();
                    List<SemiProductionViewModels> list_grade = new List<SemiProductionViewModels>();
                    //取出日期
                    var cur_day = from t in dtData.AsEnumerable()
                                  group t by new { t1 = t.Field<String>("CUR_DAY") } into m
                                  select new
                                  {
                                      CUR_DAY = m.First().Field<String>("CUR_DAY")
                                  };
                    foreach (var data in cur_day)
                    {
                        SemiProductionViewModels m = new SemiProductionViewModels();
                        m.CurDay = data.CUR_DAY.ToString();
                        list_curday.Add(m);
                    }
                    //取出日期和线别的组合
                    var cur_day_line = from t in dtData.AsEnumerable()
                                       group t by new { t1 = t.Field<String>("CUR_DAY"), t2 = t.Field<String>("LINE_CODE") } into m
                                       select new
                                       {
                                           CUR_DAY = m.First().Field<String>("CUR_DAY"),
                                           LINE_CODE = m.First().Field<String>("LINE_CODE")
                                       };
                    foreach (var data in cur_day_line)
                    {
                        SemiProductionViewModels m = new SemiProductionViewModels();
                        m.CurDay = data.CUR_DAY.ToString();
                        m.LineCode = data.LINE_CODE;
                        list_curday_line.Add(m);
                    }
                    //取出等级
                    var grade = from t in dtData.AsEnumerable()
                                group t by new { t1 = t.Field<String>("GRADE") } into m
                                select new
                                {
                                    GRADE = m.First().Field<String>("GRADE")
                                };
                    foreach (var data in grade)
                    {
                        SemiProductionViewModels m = new SemiProductionViewModels();
                        m.Grade = data.GRADE;
                        list_grade.Add(m);
                    }
                    int count = list_grade.Where(m => m.Grade.ToUpper() == model.Grade.ToUpper())
                                                         .Count();
                    if (count == 0)
                    {
                        return Json(sl, JsonRequestBehavior.AllowGet);
                    }
                    string[] columns = new string[] { "CUR_DAY", "LINE_CODE" };
                    //组织新表数据结构

                    for (int i = 0; i < columns.Length; i++)
                    {
                        DataColumn dc = dtNew.Columns.Add(columns[i]);
                        dc.Caption = dicColumn[dc.ColumnName];
                    }
                    //添加等级列
                    foreach (var data in list_grade)
                    {
                        DataColumn dcGrade = dtNew.Columns.Add(data.Grade);
                        dcGrade.DataType = typeof(double);
                    }

                    //添加比率列
                    DataColumn dcRate = dtNew.Columns.Add("RATE_VALUE");
                    dcRate.DataType = typeof(double);
                    dcRate.Caption = dicColumn[dcRate.ColumnName];

                    //添加合计列
                    DataColumn dcSum = dtNew.Columns.Add("SUM_VALUE");
                    dcSum.DataType = typeof(double);
                    dcSum.Caption = dicColumn[dcSum.ColumnName];
                    //填充数据
                    //添加日期，线别行。
                    foreach (var item in list_curday_line)
                    {
                        DataRow dr = dtNew.NewRow();
                        dr["CUR_DAY"] = item.CurDay;
                        dr["LINE_CODE"] = item.LineCode;
                        dtNew.Rows.Add(dr);
                    }
                    for (int i = 0; i < dtNew.Rows.Count; i++)
                    {
                        string curDay = Convert.ToString(dtNew.Rows[i]["CUR_DAY"]);
                        string lineCode = Convert.ToString(dtNew.Rows[i]["LINE_CODE"]);
                        double sumQty = 0;
                        for (int j = 0; j < list_grade.Count; j++)
                        {
                            string colName = list_grade[j].Grade;
                            double qty = 0;
                            var lnq = from row in dtData.AsEnumerable()
                                      where Convert.ToString(row["CUR_DAY"]) == curDay
                                            && Convert.ToString(row["LINE_CODE"]) == lineCode
                                            && Convert.ToString(row["GRADE"]) == colName
                                      select Convert.ToDouble(row["QUANTITY"]);
                            qty = lnq.Sum();

                            dtNew.Rows[i][colName] = qty;
                            sumQty += qty;
                        }
                        dtNew.Rows[i]["SUM_VALUE"] = sumQty;
                        //根据选择的等级进行计算比率列
                        dtNew.Rows[i]["RATE_VALUE"] = Math.Round(Convert.ToDouble(dtNew.Rows[i][model.Grade]) / sumQty, 4) * 100;
                    }
                    foreach (var data in list_curday)
                    {
                        DataRow dr = dtNew.NewRow();
                        dr["CUR_DAY"] = data.CurDay;
                        dr["LINE_CODE"] = "总良率";
                        dtNew.Rows.Add(dr);

                    }
                    for (int i = 0; i < dtNew.Rows.Count; i++)
                    {
                        if (Convert.ToString(dtNew.Rows[i]["LINE_CODE"]) == "总良率")
                        {
                            string curDay = Convert.ToString(dtNew.Rows[i]["CUR_DAY"]);

                            double sumQty = 0;
                            for (int j = 0; j < list_grade.Count; j++)
                            {
                                string colName = list_grade[j].Grade;
                                double qty = 0;
                                var lnq = from row in dtData.AsEnumerable()
                                          where Convert.ToString(row["CUR_DAY"]) == curDay
                                                && Convert.ToString(row["GRADE"]) == colName
                                          select Convert.ToDouble(row["QUANTITY"]);
                                qty = lnq.Sum();

                                dtNew.Rows[i][colName] = qty;
                                sumQty += qty;
                            }
                            dtNew.Rows[i]["SUM_VALUE"] = sumQty;
                            //根据选择的等级进行计算比率列
                            dtNew.Rows[i]["RATE_VALUE"] = Math.Round(Convert.ToDouble(dtNew.Rows[i][model.Grade]) / sumQty, 4) * 100;
                        }
                    }
                }
            }

            if (dtNew != null && dtNew.Rows.Count > 0)
            {
                sl = AssembleSpline(dtNew);
            }
            return Json(sl, JsonRequestBehavior.AllowGet);
        }
        public SortedList<string, string> AssembleSpline(DataTable dtDataForSpline)
        {
            string strup = string.Empty, strlow = string.Empty;
            List<string> list = new List<string>();
            System.Collections.Hashtable hsDrawLinesY = new System.Collections.Hashtable();
            StringBuilder strBuilderX = new StringBuilder();
            StringBuilder strBuildera = new StringBuilder();
            System.Collections.Hashtable hsDrawLinesX = new System.Collections.Hashtable();
            StringBuilder strBuilderY = new StringBuilder();


            var query = from t in dtDataForSpline.AsEnumerable()
                        group t by new { t1 = t.Field<String>("LINE_CODE") } into m
                        select new
                        {
                            LINE_CODE = m.First().Field<String>("LINE_CODE")
                        };
            foreach (var data in query)
            {
                list.Add(data.LINE_CODE.ToString());
            }

            foreach (string botaId in list)
            {
                strBuilderY = new StringBuilder();
                hsDrawLinesY.Add(botaId, strBuilderY);
            }
            var queryDate = from t in dtDataForSpline.AsEnumerable()
                            group t by new { t1 = t.Field<String>("CUR_DAY") } into m
                            select new
                            {
                                CUR_DAY = m.First().Field<String>("CUR_DAY")
                            };
            int j = 0;
            foreach (var data in queryDate)
            {
                hsDrawLinesX.Add(data.CUR_DAY.ToString(), j);
                strBuilderX.Append("'" + data.CUR_DAY.ToString() + "',");//X      
                j = j + 1;
            }
            int indexOfX = 0;
            Hashtable hsDrawPoint = new Hashtable();
            for (int i = 0; i < dtDataForSpline.Rows.Count; i++)
            {
                string dbKey = "";

                dbKey = dtDataForSpline.Rows[i]["LINE_CODE"].ToString();

                foreach (object key in hsDrawLinesY.Keys)
                {
                    strBuilderY = (StringBuilder)hsDrawLinesY[key];
                    string valueOfY = "";

                    string strPointOfX = "";
                    string strKey = (string)key;
                    if (string.Compare(strKey, dbKey, true) == 0)
                    {
                        if (String.IsNullOrEmpty(dtDataForSpline.Rows[i]["RATE_VALUE"].ToString()))
                        {
                            valueOfY = "null";
                        }
                        else
                        {
                            valueOfY = dtDataForSpline.Rows[i]["RATE_VALUE"].ToString();
                        }
                        if (hsDrawLinesX.ContainsKey(dtDataForSpline.Rows[i]["CUR_DAY"].ToString()))
                        {
                            indexOfX = (int)hsDrawLinesX[dtDataForSpline.Rows[i]["CUR_DAY"].ToString()];
                        }
                        strPointOfX = strKey + indexOfX.ToString();

                        if (hsDrawPoint.ContainsKey(strPointOfX))
                        {
                            if (valueOfY != "null")
                            {
                                hsDrawPoint[strPointOfX] = valueOfY;
                            }
                        }
                        else
                        {
                            hsDrawPoint.Add(strPointOfX, valueOfY);
                        }
                    }
                }
            }
            ArrayList XKey = new ArrayList(hsDrawLinesX.Values);
            XKey.Sort();

            foreach (object val in XKey)
            {
                foreach (object key in hsDrawLinesY.Keys)
                {
                    strBuilderY = (StringBuilder)hsDrawLinesY[key];
                    string strKey = key.ToString() + val.ToString();
                    if (hsDrawPoint.ContainsKey(strKey))
                    {
                        strBuilderY.Append("[" + val + "," + (string)hsDrawPoint[strKey] + "],");
                    }
                    else
                    {
                        strBuilderY.Append("[" + val + ",null],");
                    }
                }
            }

            var strx = @"[" + strBuilderX.ToString().Trim(',') + "]";
            var stry = "";
            stry = @"[";
            foreach (DictionaryEntry line in hsDrawLinesY)
            {
                stry = stry + "{ name: '" + line.Key + "',type:'spline', data: [" + line.Value.ToString().Trim(',') + "]},";
            }
            stry = stry.ToString().Trim(',');
            stry = stry + "]";

            sl.Add("mAxis", strx);
            sl.Add("nAxis", stry);
            return sl;
        }

        [HttpPost]
        public ActionResult RefreshDataDetail(SemiProductionViewModels model)
        {
            DataTable dtData = new DataTable();
            DataTable dtNew = new DataTable();
            string strx = string.Empty, stry = string.Empty, strup = string.Empty, strlow = string.Empty;

            using (QMSemiProductionServiceClient client = new QMSemiProductionServiceClient())
            {
                MethodReturnResult<DataSet> rst = client.GetSemiProdQtyForLine(new QMSemiProductionGetParameter()
                {
                    LocationName = model.LocationName,
                    IsProdReport = model.IsProdReport,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate
                });
                if (rst.Code <= 0 && rst.Data != null && rst.Data.Tables.Count > 0)
                {
                    dtData = rst.Data.Tables[0];
                    List<SemiProductionViewModels> list_curday = new List<SemiProductionViewModels>();
                    List<SemiProductionViewModels> list_curday_line = new List<SemiProductionViewModels>();
                    List<SemiProductionViewModels> list_grade = new List<SemiProductionViewModels>();
                    //取出日期
                    var cur_day = from t in dtData.AsEnumerable()
                                  group t by new { t1 = t.Field<String>("CUR_DAY") } into m
                                  select new
                                  {
                                      CUR_DAY = m.First().Field<String>("CUR_DAY")
                                  };
                    foreach (var data in cur_day)
                    {
                        SemiProductionViewModels m = new SemiProductionViewModels();
                        m.CurDay = data.CUR_DAY.ToString();
                        list_curday.Add(m);
                    }
                    //取出日期和线别的组合
                    var cur_day_line = from t in dtData.AsEnumerable()
                                       group t by new { t1 = t.Field<String>("CUR_DAY"), t2 = t.Field<String>("LINE_CODE") } into m
                                       select new
                                       {
                                           CUR_DAY = m.First().Field<String>("CUR_DAY"),
                                           LINE_CODE = m.First().Field<String>("LINE_CODE")
                                       };
                    foreach (var data in cur_day_line)
                    {
                        SemiProductionViewModels m = new SemiProductionViewModels();
                        m.CurDay = data.CUR_DAY.ToString();
                        m.LineCode = data.LINE_CODE;
                        list_curday_line.Add(m);
                    }
                    //取出等级
                    var grade = from t in dtData.AsEnumerable()
                                group t by new { t1 = t.Field<String>("GRADE") } into m
                                select new
                                {
                                    GRADE = m.First().Field<String>("GRADE")
                                };
                    foreach (var data in grade)
                    {
                        SemiProductionViewModels m = new SemiProductionViewModels();
                        m.Grade = data.GRADE;
                        list_grade.Add(m);
                    }
                    int count = list_grade.Where(m => m.Grade.ToUpper() == model.Grade.ToUpper())
                                                         .Count();
                    if (count == 0)
                    {
                        return Json(sl, JsonRequestBehavior.AllowGet);
                    }
                    string[] columns = new string[] { "CUR_DAY", "LINE_CODE" };
                    //组织新表数据结构

                    for (int i = 0; i < columns.Length; i++)
                    {
                        DataColumn dc = dtNew.Columns.Add(columns[i]);
                        dc.Caption = dicColumn[dc.ColumnName];
                    }
                    //添加等级列
                    foreach (var data in list_grade)
                    {
                        DataColumn dcGrade = dtNew.Columns.Add(data.Grade);
                        dcGrade.DataType = typeof(double);
                    }

                    //添加比率列
                    DataColumn dcRate = dtNew.Columns.Add("RATE_VALUE");
                    dcRate.DataType = typeof(double);
                    dcRate.Caption = dicColumn[dcRate.ColumnName];

                    //添加合计列
                    DataColumn dcSum = dtNew.Columns.Add("SUM_VALUE");
                    dcSum.DataType = typeof(double);
                    dcSum.Caption = dicColumn[dcSum.ColumnName];
                    //填充数据
                    //添加日期，线别行。
                    foreach (var item in list_curday_line)
                    {
                        DataRow dr = dtNew.NewRow();
                        dr["CUR_DAY"] = item.CurDay;
                        dr["LINE_CODE"] = item.LineCode;
                        dtNew.Rows.Add(dr);
                    }
                    for (int i = 0; i < dtNew.Rows.Count; i++)
                    {
                        string curDay = Convert.ToString(dtNew.Rows[i]["CUR_DAY"]);
                        string lineCode = Convert.ToString(dtNew.Rows[i]["LINE_CODE"]);
                        double sumQty = 0;
                        for (int j = 0; j < list_grade.Count; j++)
                        {
                            string colName = list_grade[j].Grade;
                            double qty = 0;
                            var lnq = from row in dtData.AsEnumerable()
                                      where Convert.ToString(row["CUR_DAY"]) == curDay
                                            && Convert.ToString(row["LINE_CODE"]) == lineCode
                                            && Convert.ToString(row["GRADE"]) == colName
                                      select Convert.ToDouble(row["QUANTITY"]);
                            qty = lnq.Sum();

                            dtNew.Rows[i][colName] = qty;
                            sumQty += qty;
                        }
                        dtNew.Rows[i]["SUM_VALUE"] = sumQty;
                        //根据选择的等级进行计算比率列
                        dtNew.Rows[i]["RATE_VALUE"] = Math.Round(Convert.ToDouble(dtNew.Rows[i][model.Grade]) / sumQty, 4) * 100;
                    }
                    foreach (var data in list_curday)
                    {
                        DataRow dr = dtNew.NewRow();
                        dr["CUR_DAY"] = data.CurDay;
                        dr["LINE_CODE"] = "总良率";
                        dtNew.Rows.Add(dr);

                    }
                    for (int i = 0; i < dtNew.Rows.Count; i++)
                    {
                        if (Convert.ToString(dtNew.Rows[i]["LINE_CODE"]) == "总良率")
                        {
                            string curDay = Convert.ToString(dtNew.Rows[i]["CUR_DAY"]);

                            double sumQty = 0;
                            for (int j = 0; j < list_grade.Count; j++)
                            {
                                string colName = list_grade[j].Grade;
                                double qty = 0;
                                var lnq = from row in dtData.AsEnumerable()
                                          where Convert.ToString(row["CUR_DAY"]) == curDay
                                                && Convert.ToString(row["GRADE"]) == colName
                                          select Convert.ToDouble(row["QUANTITY"]);
                                qty = lnq.Sum();

                                dtNew.Rows[i][colName] = qty;
                                sumQty += qty;
                            }
                            dtNew.Rows[i]["SUM_VALUE"] = sumQty;
                            //根据选择的等级进行计算比率列
                            dtNew.Rows[i]["RATE_VALUE"] = Math.Round(Convert.ToDouble(dtNew.Rows[i][model.Grade]) / sumQty, 4) * 100;
                        }
                    }
                }
            }
            if (dtNew != null && dtNew.Rows.Count > 0)
            {
                ViewBag.Grade = model.Grade;
                ViewBag.dtList = dtNew;
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("_ListPartial", new SemiProductionViewModels());
            }
            else
            {
                return View("Index", new SemiProductionViewModels());
            }
        }

        #endregion
        #region ForLocation 合格率日报 分车间

        public ActionResult IndexOfLocation()
        {
            return View(new SemiProductionViewModels());
        }
        public ActionResult GetHighchartsResultForLocation(SemiProductionViewModels model)
        {
            DataTable dtData = new DataTable();
            DataTable dtNew = new DataTable();
            string strx = string.Empty, stry = string.Empty, strup = string.Empty, strlow = string.Empty;

            using (QMSemiProductionServiceClient client = new QMSemiProductionServiceClient())
            {
                MethodReturnResult<DataSet> rst = client.GetSemiProdQtyForLocation(new QMSemiProductionGetParameter()
                {
                    IsProdReport = model.IsProdReport,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate
                });
                if (rst.Code <= 0 && rst.Data != null && rst.Data.Tables.Count > 0)
                {
                    dtData = rst.Data.Tables[0];
                    List<SemiProductionViewModels> list_curday = new List<SemiProductionViewModels>();
                    List<SemiProductionViewModels> list_curday_location = new List<SemiProductionViewModels>();
                    List<SemiProductionViewModels> list_grade = new List<SemiProductionViewModels>();
                    //取出日期
                    var cur_day = from t in dtData.AsEnumerable()
                                  group t by new { t1 = t.Field<String>("CUR_DAY") } into m
                                  select new
                                  {
                                      CUR_DAY = m.First().Field<String>("CUR_DAY")
                                  };
                    foreach (var data in cur_day)
                    {
                        SemiProductionViewModels m = new SemiProductionViewModels();
                        m.CurDay = data.CUR_DAY.ToString();
                        list_curday.Add(m);
                    }
                    //取出日期和线别的组合
                    var cur_day_location = from t in dtData.AsEnumerable()
                                           group t by new { t1 = t.Field<String>("CUR_DAY"), t2 = t.Field<String>("LOCATION_NAME") } into m
                                           select new
                                           {
                                               CUR_DAY = m.First().Field<String>("CUR_DAY"),
                                               LOCATION_NAME = m.First().Field<String>("LOCATION_NAME")
                                           };
                    foreach (var data in cur_day_location)
                    {
                        SemiProductionViewModels m = new SemiProductionViewModels();
                        m.CurDay = data.CUR_DAY.ToString();
                        m.LocationName = data.LOCATION_NAME;
                        list_curday_location.Add(m);
                    }
                    //取出等级
                    var grade = from t in dtData.AsEnumerable()
                                group t by new { t1 = t.Field<String>("GRADE") } into m
                                select new
                                {
                                    GRADE = m.First().Field<String>("GRADE")
                                };
                    foreach (var data in grade)
                    {
                        SemiProductionViewModels m = new SemiProductionViewModels();
                        m.Grade = data.GRADE;
                        list_grade.Add(m);
                    }
                    int count = list_grade.Where(m => m.Grade.ToUpper() == model.Grade.ToUpper())
                                                         .Count();
                    if (count == 0)
                    {
                        return Json(sl, JsonRequestBehavior.AllowGet);
                    }
                    string[] columns = new string[] { "CUR_DAY", "LOCATION_NAME" };
                    //组织新表数据结构

                    for (int i = 0; i < columns.Length; i++)
                    {
                        DataColumn dc = dtNew.Columns.Add(columns[i]);
                        dc.Caption = dicColumn[dc.ColumnName];
                    }
                    //添加等级列
                    foreach (var data in list_grade)
                    {
                        DataColumn dcGrade = dtNew.Columns.Add(data.Grade);
                        dcGrade.DataType = typeof(double);
                    }

                    //添加比率列
                    DataColumn dcRate = dtNew.Columns.Add("RATE_VALUE");
                    dcRate.DataType = typeof(double);
                    dcRate.Caption = dicColumn[dcRate.ColumnName];

                    //添加合计列
                    DataColumn dcSum = dtNew.Columns.Add("SUM_VALUE");
                    dcSum.DataType = typeof(double);
                    dcSum.Caption = dicColumn[dcSum.ColumnName];
                    //填充数据
                    //添加日期，线别行。
                    foreach (var item in list_curday_location)
                    {
                        DataRow dr = dtNew.NewRow();
                        dr["CUR_DAY"] = item.CurDay;
                        dr["LOCATION_NAME"] = item.LocationName;
                        dtNew.Rows.Add(dr);
                    }
                    for (int i = 0; i < dtNew.Rows.Count; i++)
                    {
                        string curDay = Convert.ToString(dtNew.Rows[i]["CUR_DAY"]);
                        string locationname = Convert.ToString(dtNew.Rows[i]["LOCATION_NAME"]);
                        double sumQty = 0;
                        for (int j = 0; j < list_grade.Count; j++)
                        {
                            string colName = list_grade[j].Grade;
                            double qty = 0;
                            var lnq = from row in dtData.AsEnumerable()
                                      where Convert.ToString(row["CUR_DAY"]) == curDay
                                            && Convert.ToString(row["LOCATION_NAME"]) == locationname
                                            && Convert.ToString(row["GRADE"]) == colName
                                      select Convert.ToDouble(row["QUANTITY"]);
                            qty = lnq.Sum();

                            dtNew.Rows[i][colName] = qty;
                            sumQty += qty;
                        }
                        dtNew.Rows[i]["SUM_VALUE"] = sumQty;
                        //根据选择的等级进行计算比率列
                        dtNew.Rows[i]["RATE_VALUE"] = Math.Round(Convert.ToDouble(dtNew.Rows[i][model.Grade]) / sumQty, 4) * 100;
                    }
                    foreach (var data in list_curday)
                    {
                        DataRow dr = dtNew.NewRow();
                        dr["CUR_DAY"] = data.CurDay;
                        dr["LOCATION_NAME"] = "总良率";
                        dtNew.Rows.Add(dr);

                    }
                    for (int i = 0; i < dtNew.Rows.Count; i++)
                    {
                        if (Convert.ToString(dtNew.Rows[i]["LOCATION_NAME"]) == "总良率")
                        {
                            string curDay = Convert.ToString(dtNew.Rows[i]["CUR_DAY"]);

                            double sumQty = 0;
                            for (int j = 0; j < list_grade.Count; j++)
                            {
                                string colName = list_grade[j].Grade;
                                double qty = 0;
                                var lnq = from row in dtData.AsEnumerable()
                                          where Convert.ToString(row["CUR_DAY"]) == curDay
                                                && Convert.ToString(row["GRADE"]) == colName
                                          select Convert.ToDouble(row["QUANTITY"]);
                                qty = lnq.Sum();

                                dtNew.Rows[i][colName] = qty;
                                sumQty += qty;
                            }
                            dtNew.Rows[i]["SUM_VALUE"] = sumQty;
                            //根据选择的等级进行计算比率列
                            dtNew.Rows[i]["RATE_VALUE"] = Math.Round(Convert.ToDouble(dtNew.Rows[i][model.Grade]) / sumQty, 4) * 100;
                        }
                    }
                }
            }

            if (dtNew != null && dtNew.Rows.Count > 0)
            {

                sl = AssembleSplineForLocation(dtNew);
            }
            return Json(sl, JsonRequestBehavior.AllowGet);
        }

        public SortedList<string, string> AssembleSplineForLocation(DataTable dtDataForSpline)
        {
            string strup = string.Empty, strlow = string.Empty;
            List<string> list = new List<string>();
            System.Collections.Hashtable hsDrawLinesY = new System.Collections.Hashtable();
            StringBuilder strBuilderX = new StringBuilder();
            StringBuilder strBuildera = new StringBuilder();
            System.Collections.Hashtable hsDrawLinesX = new System.Collections.Hashtable();
            StringBuilder strBuilderY = new StringBuilder();


            var query = from t in dtDataForSpline.AsEnumerable()
                        group t by new { t1 = t.Field<String>("LOCATION_NAME") } into m
                        select new
                        {
                            LOCATION_NAME = m.First().Field<String>("LOCATION_NAME")
                        };
            foreach (var data in query)
            {
                list.Add(data.LOCATION_NAME.ToString());
            }

            foreach (string botaId in list)
            {
                strBuilderY = new StringBuilder();
                hsDrawLinesY.Add(botaId, strBuilderY);
            }
            var queryDate = from t in dtDataForSpline.AsEnumerable()
                            group t by new { t1 = t.Field<String>("CUR_DAY") } into m
                            select new
                            {
                                CUR_DAY = m.First().Field<String>("CUR_DAY")
                            };
            int j = 0;
            foreach (var data in queryDate)
            {
                hsDrawLinesX.Add(data.CUR_DAY.ToString(), j);
                strBuilderX.Append("'" + data.CUR_DAY.ToString() + "',");//X      
                j = j + 1;
            }
            int indexOfX = 0;
            Hashtable hsDrawPoint = new Hashtable();
            for (int i = 0; i < dtDataForSpline.Rows.Count; i++)
            {
                string dbKey = "";

                dbKey = dtDataForSpline.Rows[i]["LOCATION_NAME"].ToString();

                foreach (object key in hsDrawLinesY.Keys)
                {
                    strBuilderY = (StringBuilder)hsDrawLinesY[key];
                    string valueOfY = "";

                    string strPointOfX = "";
                    string strKey = (string)key;
                    if (string.Compare(strKey, dbKey, true) == 0)
                    {
                        if (String.IsNullOrEmpty(dtDataForSpline.Rows[i]["RATE_VALUE"].ToString()))
                        {
                            valueOfY = "null";
                        }
                        else
                        {
                            valueOfY = dtDataForSpline.Rows[i]["RATE_VALUE"].ToString();
                        }
                        if (hsDrawLinesX.ContainsKey(dtDataForSpline.Rows[i]["CUR_DAY"].ToString()))
                        {
                            indexOfX = (int)hsDrawLinesX[dtDataForSpline.Rows[i]["CUR_DAY"].ToString()];
                        }
                        strPointOfX = strKey + indexOfX.ToString();

                        if (hsDrawPoint.ContainsKey(strPointOfX))
                        {
                            if (valueOfY != "null")
                            {
                                hsDrawPoint[strPointOfX] = valueOfY;
                            }
                        }
                        else
                        {
                            hsDrawPoint.Add(strPointOfX, valueOfY);
                        }
                    }
                }
            }
            ArrayList XKey = new ArrayList(hsDrawLinesX.Values);
            XKey.Sort();

            foreach (object val in XKey)
            {
                foreach (object key in hsDrawLinesY.Keys)
                {
                    strBuilderY = (StringBuilder)hsDrawLinesY[key];
                    string strKey = key.ToString() + val.ToString();
                    if (hsDrawPoint.ContainsKey(strKey))
                    {
                        strBuilderY.Append("[" + val + "," + (string)hsDrawPoint[strKey] + "],");
                    }
                    else
                    {
                        strBuilderY.Append("[" + val + ",null],");
                    }
                }
            }

            var strx = @"[" + strBuilderX.ToString().Trim(',') + "]";
            var stry = "";
            stry = @"[";
            foreach (DictionaryEntry line in hsDrawLinesY)
            {
                stry = stry + "{ name: '" + line.Key + "',type:'spline', data: [" + line.Value.ToString().Trim(',') + "]},";
            }
            stry = stry.ToString().Trim(',');
            stry = stry + "]";

            sl.Add("mAxis", strx);
            sl.Add("nAxis", stry);
            return sl;
        }

        [HttpPost]
        public ActionResult RefreshDataDetailForLocation(SemiProductionViewModels model)
        {
            DataTable dtData = new DataTable();
            DataTable dtNew = new DataTable();
            string strx = string.Empty, stry = string.Empty, strup = string.Empty, strlow = string.Empty;

            using (QMSemiProductionServiceClient client = new QMSemiProductionServiceClient())
            {
                MethodReturnResult<DataSet> rst = client.GetSemiProdQtyForLocation(new QMSemiProductionGetParameter()
                {
                    IsProdReport = model.IsProdReport,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate
                });
                if (rst.Code <= 0 && rst.Data != null && rst.Data.Tables.Count > 0)
                {
                    dtData = rst.Data.Tables[0];
                    List<SemiProductionViewModels> list_curday = new List<SemiProductionViewModels>();
                    List<SemiProductionViewModels> list_curday_location = new List<SemiProductionViewModels>();
                    List<SemiProductionViewModels> list_grade = new List<SemiProductionViewModels>();
                    //取出日期
                    var cur_day = from t in dtData.AsEnumerable()
                                  group t by new { t1 = t.Field<String>("CUR_DAY") } into m
                                  select new
                                  {
                                      CUR_DAY = m.First().Field<String>("CUR_DAY")
                                  };
                    foreach (var data in cur_day)
                    {
                        SemiProductionViewModels m = new SemiProductionViewModels();
                        m.CurDay = data.CUR_DAY.ToString();
                        list_curday.Add(m);
                    }
                    //取出日期和线别的组合
                    var cur_day_location = from t in dtData.AsEnumerable()
                                           group t by new { t1 = t.Field<String>("CUR_DAY"), t2 = t.Field<String>("LOCATION_NAME") } into m
                                           select new
                                           {
                                               CUR_DAY = m.First().Field<String>("CUR_DAY"),
                                               LOCATION_NAME = m.First().Field<String>("LOCATION_NAME")
                                           };
                    foreach (var data in cur_day_location)
                    {
                        SemiProductionViewModels m = new SemiProductionViewModels();
                        m.CurDay = data.CUR_DAY.ToString();
                        m.LocationName = data.LOCATION_NAME;
                        list_curday_location.Add(m);
                    }
                    //取出等级
                    var grade = from t in dtData.AsEnumerable()
                                group t by new { t1 = t.Field<String>("GRADE") } into m
                                select new
                                {
                                    GRADE = m.First().Field<String>("GRADE")
                                };
                    foreach (var data in grade)
                    {
                        SemiProductionViewModels m = new SemiProductionViewModels();
                        m.Grade = data.GRADE;
                        list_grade.Add(m);
                    }
                    int count = list_grade.Where(m => m.Grade.ToUpper() == model.Grade.ToUpper())
                                                         .Count();
                    if (count == 0)
                    {
                        return Json(sl, JsonRequestBehavior.AllowGet);
                    }
                    string[] columns = new string[] { "CUR_DAY", "LOCATION_NAME" };
                    //组织新表数据结构

                    for (int i = 0; i < columns.Length; i++)
                    {
                        DataColumn dc = dtNew.Columns.Add(columns[i]);
                        dc.Caption = dicColumn[dc.ColumnName];
                    }
                    //添加等级列
                    foreach (var data in list_grade)
                    {
                        DataColumn dcGrade = dtNew.Columns.Add(data.Grade);
                        dcGrade.DataType = typeof(double);
                    }

                    //添加比率列
                    DataColumn dcRate = dtNew.Columns.Add("RATE_VALUE");
                    dcRate.DataType = typeof(double);
                    dcRate.Caption = dicColumn[dcRate.ColumnName];

                    //添加合计列
                    DataColumn dcSum = dtNew.Columns.Add("SUM_VALUE");
                    dcSum.DataType = typeof(double);
                    dcSum.Caption = dicColumn[dcSum.ColumnName];
                    //填充数据
                    //添加日期，线别行。
                    foreach (var item in list_curday_location)
                    {
                        DataRow dr = dtNew.NewRow();
                        dr["CUR_DAY"] = item.CurDay;
                        dr["LOCATION_NAME"] = item.LocationName;
                        dtNew.Rows.Add(dr);
                    }
                    for (int i = 0; i < dtNew.Rows.Count; i++)
                    {
                        string curDay = Convert.ToString(dtNew.Rows[i]["CUR_DAY"]);
                        string locationname = Convert.ToString(dtNew.Rows[i]["LOCATION_NAME"]);
                        double sumQty = 0;
                        for (int j = 0; j < list_grade.Count; j++)
                        {
                            string colName = list_grade[j].Grade;
                            double qty = 0;
                            var lnq = from row in dtData.AsEnumerable()
                                      where Convert.ToString(row["CUR_DAY"]) == curDay
                                            && Convert.ToString(row["LOCATION_NAME"]) == locationname
                                            && Convert.ToString(row["GRADE"]) == colName
                                      select Convert.ToDouble(row["QUANTITY"]);
                            qty = lnq.Sum();

                            dtNew.Rows[i][colName] = qty;
                            sumQty += qty;
                        }
                        dtNew.Rows[i]["SUM_VALUE"] = sumQty;
                        //根据选择的等级进行计算比率列
                        dtNew.Rows[i]["RATE_VALUE"] = Math.Round(Convert.ToDouble(dtNew.Rows[i][model.Grade]) / sumQty, 4) * 100;
                    }
                    foreach (var data in list_curday)
                    {
                        DataRow dr = dtNew.NewRow();
                        dr["CUR_DAY"] = data.CurDay;
                        dr["LOCATION_NAME"] = "总良率";
                        dtNew.Rows.Add(dr);

                    }
                    for (int i = 0; i < dtNew.Rows.Count; i++)
                    {
                        if (Convert.ToString(dtNew.Rows[i]["LOCATION_NAME"]) == "总良率")
                        {
                            string curDay = Convert.ToString(dtNew.Rows[i]["CUR_DAY"]);

                            double sumQty = 0;
                            for (int j = 0; j < list_grade.Count; j++)
                            {
                                string colName = list_grade[j].Grade;
                                double qty = 0;
                                var lnq = from row in dtData.AsEnumerable()
                                          where Convert.ToString(row["CUR_DAY"]) == curDay
                                                && Convert.ToString(row["GRADE"]) == colName
                                          select Convert.ToDouble(row["QUANTITY"]);
                                qty = lnq.Sum();

                                dtNew.Rows[i][colName] = qty;
                                sumQty += qty;
                            }
                            dtNew.Rows[i]["SUM_VALUE"] = sumQty;
                            //根据选择的等级进行计算比率列
                            dtNew.Rows[i]["RATE_VALUE"] = Math.Round(Convert.ToDouble(dtNew.Rows[i][model.Grade]) / sumQty, 4) * 100;
                        }
                    }
                }
            }
            if (dtNew != null && dtNew.Rows.Count > 0)
            {
                ViewBag.Grade = model.Grade;
                ViewBag.dtList = dtNew;
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("_ListPartialForLocation", new SemiProductionViewModels());
            }
            else
            {
                return View("Index", new SemiProductionViewModels());
            }
        }
        #endregion
        #region 半成品当日每个车间每条线的不良柱状图
        public ActionResult IndexOfDefective()
        {
            return View(new SemiProductionViewModels());
        }
        public ActionResult GetHighchartsResultForDefective(SemiProductionViewModels model)
        {
            DataTable dtData = new DataTable();
            string strx = string.Empty, stry = string.Empty, strup = string.Empty, strlow = string.Empty;

            using (QMSemiProductionServiceClient client = new QMSemiProductionServiceClient())
            {
                MethodReturnResult<DataSet> rst = client.GetQtyForDefective(new QMSemiProductionGetParameter()
                {
                    LocationName = model.LocationName,
                    StartDate = model.StartDate
                });
                if (rst.Code <= 0 && rst.Data != null && rst.Data.Tables.Count > 0)
                {
                    dtData = rst.Data.Tables[0];
                }
            }

            if (dtData != null && dtData.Rows.Count > 0)
            {

                sl = AssembleSplineForDefective(dtData);
            }
            return Json(sl, JsonRequestBehavior.AllowGet);
        }

        public SortedList<string, string> AssembleSplineForDefective(DataTable dtData)
        {
            string strup = string.Empty, strlow = string.Empty;
            List<string> list = new List<string>();
            System.Collections.Hashtable hsDrawLinesY = new System.Collections.Hashtable();
            StringBuilder strBuilderX = new StringBuilder();
            StringBuilder strBuildera = new StringBuilder();
            System.Collections.Hashtable hsDrawLinesX = new System.Collections.Hashtable();
            StringBuilder strBuilderY = new StringBuilder();


            var query = from t in dtData.AsEnumerable()
                        group t by new { t1 = t.Field<String>("LOCATION_NAME") } into m
                        select new
                        {
                            LOCATION_NAME = m.First().Field<String>("LOCATION_NAME")
                        };
            foreach (var data in query)
            {
                list.Add(data.LOCATION_NAME.ToString());
            }

            foreach (string botaId in list)
            {
                strBuilderY = new StringBuilder();
                hsDrawLinesY.Add(botaId, strBuilderY);
            }
            var queryDate = from t in dtData.AsEnumerable()
                            group t by new { t1 = t.Field<String>("REASON_CODE_CATEGORY_NAME") } into m
                            select new
                            {
                                CUR_DAY = m.First().Field<String>("REASON_CODE_CATEGORY_NAME")
                            };
            int j = 0;
            foreach (var data in queryDate)
            {
                hsDrawLinesX.Add(data.CUR_DAY.ToString(), j);
                strBuilderX.Append("'" + data.CUR_DAY.ToString() + "',");//X      
                j = j + 1;
            }
            int indexOfX = 0;
            Hashtable hsDrawPoint = new Hashtable();
            for (int i = 0; i < dtData.Rows.Count; i++)
            {
                string dbKey = "";

                dbKey = dtData.Rows[i]["LOCATION_NAME"].ToString();

                foreach (object key in hsDrawLinesY.Keys)
                {
                    strBuilderY = (StringBuilder)hsDrawLinesY[key];
                    string valueOfY = "";

                    string strPointOfX = "";
                    string strKey = (string)key;
                    if (string.Compare(strKey, dbKey, true) == 0)
                    {
                        if (String.IsNullOrEmpty(dtData.Rows[i]["QUANTITY"].ToString()))
                        {
                            valueOfY = "null";
                        }
                        else
                        {
                            valueOfY = dtData.Rows[i]["QUANTITY"].ToString();
                        }
                        if (hsDrawLinesX.ContainsKey(dtData.Rows[i]["REASON_CODE_CATEGORY_NAME"].ToString()))
                        {
                            indexOfX = (int)hsDrawLinesX[dtData.Rows[i]["REASON_CODE_CATEGORY_NAME"].ToString()];
                        }
                        strPointOfX = strKey + indexOfX.ToString();

                        if (hsDrawPoint.ContainsKey(strPointOfX))
                        {
                            if (valueOfY != "null")
                            {
                                hsDrawPoint[strPointOfX] = valueOfY;
                            }
                        }
                        else
                        {
                            hsDrawPoint.Add(strPointOfX, valueOfY);
                        }
                    }
                }
            }
            ArrayList XKey = new ArrayList(hsDrawLinesX.Values);
            XKey.Sort();

            foreach (object val in XKey)
            {
                foreach (object key in hsDrawLinesY.Keys)
                {
                    strBuilderY = (StringBuilder)hsDrawLinesY[key];
                    string strKey = key.ToString() + val.ToString();
                    if (hsDrawPoint.ContainsKey(strKey))
                    {
                        strBuilderY.Append("[" + val + "," + (string)hsDrawPoint[strKey] + "],");
                    }
                    else
                    {
                        strBuilderY.Append("[" + val + ",null],");
                    }
                }
            }

            var strx = @"[" + strBuilderX.ToString().Trim(',') + "]";
            var stry = "";
            stry = @"[";
            foreach (DictionaryEntry line in hsDrawLinesY)
            {
                stry = stry + "{ name: '" + line.Key + "',type:'column', data: [" + line.Value.ToString().Trim(',') + "]},";
            }
            stry = stry.ToString().Trim(',');
            stry = stry + "]";

            sl.Add("mAxis", strx);
            sl.Add("nAxis", stry);
            return sl;
        }
        #endregion
        #region 位置不良数量
        public ActionResult QueryDefectPOS(SemiProductionViewModels model)
        {
            DataTable dtData = new DataTable();
            using (QMSemiProductionServiceClient client = new QMSemiProductionServiceClient())
            {
                MethodReturnResult<DataSet> rst = client.GetQtyForDefectPOS(new DefectPOSGetParameter()
                {
                    StepName = model.StepName,
                    PosX = model.PosX,
                    PosY = model.PosY,
                    LineCode = model.LineCode,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate
                });
                if (rst.Code <= 0 && rst.Data != null && rst.Data.Tables.Count > 0)
                {
                    dtData = rst.Data.Tables[0];
                }
            }
            if (dtData != null && dtData.Rows.Count > 0)
            {
                ViewBag.dtList = dtData;
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("_ListPartialForDefectPOS", new SemiProductionViewModels());
            }
            else
            {
                return View("IndexOfDefectPOS", new SemiProductionViewModels());
            }

        }
        #endregion
        #region 每个线每条线的不良原因柏拉图
        public ActionResult IndexOfDefectReason()
        {
            return View(new SemiProductionViewModels());
        }
        public ActionResult GetHighchartsResultForDefectReason(SemiProductionViewModels model)
        {
            DataTable dtData = new DataTable();

            using (QMSemiProductionServiceClient client = new QMSemiProductionServiceClient())
            {
                MethodReturnResult<DataSet> rst = client.GetQtyForDefectReason(new DefectPOSGetParameter()
                {
                    IsProdReport=model.IsProdReport,
                    LineCode = model.LineCode,
                    StartDate = model.StartDate,
                    EndDate=model.EndDate
                });
                if (rst.Code <= 0 && rst.Data != null && rst.Data.Tables.Count > 0)
                {
                    dtData = rst.Data.Tables[0];
                }
            }

            if (dtData != null && dtData.Rows.Count > 0)
            {

                sl = AssembleSplineForDefectReason(dtData);
            }
            return Json(sl, JsonRequestBehavior.AllowGet);
        }

        public SortedList<string, string> AssembleSplineForDefectReason(DataTable dtData)
        {
            string strup = string.Empty, strlow = string.Empty;
            List<string> list = new List<string>();
            System.Collections.Hashtable hsDrawLinesY = new System.Collections.Hashtable();
            StringBuilder strBuilderX = new StringBuilder();
            StringBuilder strBuildera = new StringBuilder();
            System.Collections.Hashtable hsDrawLinesX = new System.Collections.Hashtable();
            StringBuilder strBuilderY = new StringBuilder();


            var query = from t in dtData.AsEnumerable()
                        group t by new { t1 = t.Field<String>("LINE_CODE") } into m
                        select new
                        {
                            LINE_CODE = m.First().Field<String>("LINE_CODE")
                        };
            foreach (var data in query)
            {
                list.Add(data.LINE_CODE);
            }

            foreach (string botaId in list)
            {
                strBuilderY = new StringBuilder();
                hsDrawLinesY.Add(botaId, strBuilderY);
            }
            var queryDate = from t in dtData.AsEnumerable()
                            group t by new { t1 = t.Field<String>("REASON_CODE_NAME") } into m
                            select new
                            {
                                CUR_DAY = m.First().Field<String>("REASON_CODE_NAME")
                            };
            int j = 0;
            foreach (var data in queryDate)
            {
                hsDrawLinesX.Add(data.CUR_DAY.ToString(), j);
                strBuilderX.Append("'" + data.CUR_DAY.ToString() + "',");//X      
                j = j + 1;
            }
            int indexOfX = 0;
            Hashtable hsDrawPoint = new Hashtable();
            for (int i = 0; i < dtData.Rows.Count; i++)
            {
                string dbKey = "";

                dbKey = dtData.Rows[i]["LINE_CODE"].ToString();

                foreach (object key in hsDrawLinesY.Keys)
                {
                    strBuilderY = (StringBuilder)hsDrawLinesY[key];
                    string valueOfY = "";

                    string strPointOfX = "";
                    string strKey = (string)key;
                    if (string.Compare(strKey, dbKey, true) == 0)
                    {
                        if (String.IsNullOrEmpty(dtData.Rows[i]["qty"].ToString()))
                        {
                            valueOfY = "null";
                        }
                        else
                        {
                            valueOfY = dtData.Rows[i]["qty"].ToString();
                        }
                        if (hsDrawLinesX.ContainsKey(dtData.Rows[i]["REASON_CODE_NAME"].ToString()))
                        {
                            indexOfX = (int)hsDrawLinesX[dtData.Rows[i]["REASON_CODE_NAME"].ToString()];
                        }
                        strPointOfX = strKey + indexOfX.ToString();

                        if (hsDrawPoint.ContainsKey(strPointOfX))
                        {
                            if (valueOfY != "null")
                            {
                                hsDrawPoint[strPointOfX] = valueOfY;
                            }
                        }
                        else
                        {
                            hsDrawPoint.Add(strPointOfX, valueOfY);
                        }
                    }
                }
            }
            ArrayList XKey = new ArrayList(hsDrawLinesX.Values);
            XKey.Sort();

            foreach (object val in XKey)
            {
                foreach (object key in hsDrawLinesY.Keys)
                {
                    strBuilderY = (StringBuilder)hsDrawLinesY[key];
                    string strKey = key.ToString() + val.ToString();
                    if (hsDrawPoint.ContainsKey(strKey))
                    {
                        strBuilderY.Append("[" + val + "," + (string)hsDrawPoint[strKey] + "],");
                    }
                    else
                    {
                        strBuilderY.Append("[" + val + ",null],");
                    }
                }
            }

            var strx = @"[" + strBuilderX.ToString().Trim(',') + "]";
            var stry = "";
            stry = @"[";
            foreach (DictionaryEntry line in hsDrawLinesY)
            {
                stry = stry + "{ name: '" + line.Key + "',type:'column', data: [" + line.Value.ToString().Trim(',') + "]},";
            }
            stry = stry.ToString().Trim(',');
            stry = stry + "]";

            sl.Add("mAxis", strx);
            sl.Add("nAxis", stry);
            return sl;
        }
        #endregion
        #region ExportToExcel
        /// <summary>
        /// 银行流水单导出
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> ExportToExcel(string TesterID, string HeadID, string FromTime, string EndTime, string TrStep)
        {
            //TesterOutputPrecisionMonitorModel model = new TesterOutputPrecisionMonitorModel();
            //model.TesterID = TesterID;
            //model.HeadID = HeadID;
            //model.FromTime = FromTime;
            //model.EndTime = EndTime;
            //model.TrStep = TrStep;
            //IList<TesterOutputPrecisionMonitorModel> lstLot = new List<TesterOutputPrecisionMonitorModel>();
            //await Task.Run(() =>
            //{
            //    BusinessResult<DataTable> result = new BusinessResult<DataTable>();
            //    result = TesterOutputPrecisionMonitorBusiness.Instance.GetTesterOutputPrecisionMonitor(getWhere(model));

            //    lstLot = ToTesterOutputPrecisionMonitorList(result.ReturnValue);
            //});
            ////创建工作薄。
            IWorkbook wb = new HSSFWorkbook();
            ////设置EXCEL格式
            //ICellStyle style = wb.CreateCellStyle();
            //style.FillForegroundColor = 10;
            ////有边框
            //style.BorderBottom = BorderStyle.Thin;
            //style.BorderLeft = BorderStyle.Thin;
            //style.BorderRight = BorderStyle.Thin;
            //style.BorderTop = BorderStyle.Thin;
            //IFont font = wb.CreateFont();
            //font.Boldweight = 10;
            //style.SetFont(font);

            //ISheet ws = null;
            //for (int j = 0; j < lstLot.Count; j++)
            //{
            //    if (j % 65535 == 0)
            //    {
            //        ws = wb.CreateSheet();
            //        IRow row = ws.CreateRow(0);
            //        #region //列名
            //        ICell cell = row.CreateCell(row.Cells.Count);
            //        cell.CellStyle = style;
            //        cell.SetCellValue("SYSTEM_TIME");  //

            //        cell = row.CreateCell(row.Cells.Count);
            //        cell.CellStyle = style;
            //        cell.SetCellValue("Tested_Qty");  //

            //        cell = row.CreateCell(row.Cells.Count);
            //        cell.CellStyle = style;
            //        cell.SetCellValue("Test Step");  //

            //        cell = row.CreateCell(row.Cells.Count);
            //        cell.CellStyle = style;
            //        cell.SetCellValue("UPH");  //

            //        #endregion
            //        font.Boldweight = 5;
            //    }
            //    TesterOutputPrecisionMonitorModel obj = lstLot[j];
            //    IRow rowData = ws.CreateRow(j + 1);

            //    #region //数据
            //    ICell cellData = rowData.CreateCell(rowData.Cells.Count);
            //    cellData.CellStyle = style;
            //    cellData.SetCellValue(obj.SystemTime);  //

            //    cellData = rowData.CreateCell(rowData.Cells.Count);
            //    cellData.CellStyle = style;
            //    cellData.SetCellValue(obj.TestedQty);  //

            //    cellData = rowData.CreateCell(rowData.Cells.Count);
            //    cellData.CellStyle = style;
            //    cellData.SetCellValue(obj.TrStep);  //

            //    cellData = rowData.CreateCell(rowData.Cells.Count);
            //    cellData.CellStyle = style;
            //    cellData.SetCellValue(obj.UPH);  //


            //    #endregion
            //}

            MemoryStream ms = new MemoryStream();
            wb.Write(ms);
            ms.Flush();
            ms.Position = 0;
            return File(ms, "application/vnd.ms-excel", "TesterOutputPrecisionMonitor.xls");
        }
        #endregion
    }
}