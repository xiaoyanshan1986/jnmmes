using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NPOI.SS.UserModel;
using ServiceCenter.Client.Mvc.Areas.RPT.Models;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Service.Client.RPT;
using ServiceCenter.MES.Service.Client.WIP;
using ServiceCenter.MES.Service.Contract.RPT;
using ServiceCenter.Model;
using ServiceCenter.Common;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Client.Mvc.Resources;
using NPOI.HSSF.UserModel;
using System.IO;
using System.Collections;

namespace ServiceCenter.Client.Mvc.Areas.RPT.Controllers
{
    public class OEEDailyDataController : Controller
    {
        SortedList<string, string> sl = new SortedList<string, string>();
        #region OEE报表 总界面
        // GET: RPT/WIPOperateDaily
        public ActionResult Index()
        {
            OEEDailyDataViewModel model = new OEEDailyDataViewModel
            {
                StartDate = System.DateTime.Now.ToString("yyyy-MM-dd"),
                EndDate = System.DateTime.Now.ToString("yyyy-MM-dd"),
            };
            return View(model);
        }
        [HttpPost]
        public ActionResult Query(OEEDailyDataViewModel model)
        {
            DataTable dtWIPDailyData = new DataTable();



            using (OEEDataServiceClient client = new OEEDataServiceClient())
            {
                MethodReturnResult<DataSet> rst = client.GetOEEDailyData(new OEEDataGetParameter()
                {
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    StepName = model.EquipmentName,
                    //LocationName = model.LocationName
                    LocationName = model.LocationName

                });
                if (rst.Code <= 0 && rst.Data != null && rst.Data.Tables.Count > 0)
                {
                    dtWIPDailyData = rst.Data.Tables["dtQTY"];
                }
            }
            DataTable dt = new DataTable();
            //增加状态列
            DataColumn dcStatus = new DataColumn("项目");
            dt.Columns.Add(dcStatus);
            dcStatus = new DataColumn("LINK");
            dt.Columns.Add(dcStatus);
            #region 创建动态列
            DateTime dCurDateTime = System.DateTime.Now;
            if (DateTime.TryParse(model.EndDate, out dCurDateTime) == false)
            {
                //日期格式不正确
                return View();
            }
            System.Collections.Hashtable hsDrawLinesX = new System.Collections.Hashtable();
            List<string> lstColums = new List<string>();
            var query = from t in dtWIPDailyData.AsEnumerable()
                        //where (t.Field<string>("DataType") == "D")
                        group t by new { t1 = t.Field<string>("RPT_DATE") }
                            into m
                            select new
                            {
                                RPT_DATE = m.First().Field<string>("RPT_DATE")
                            } into r
                            orderby r.RPT_DATE
                            select r;


            int icount = 0;
            foreach (var data in query)
            {
                if (data.RPT_DATE.Contains("累计"))
                {
                    //string s = data.RPT_DATE.Substring(5, data.RPT_DATE.Length - 7);
                    //string date = string.Format("{0:M}", Convert.ToDateTime(data.RPT_DATE.Substring(5, data.RPT_DATE.Length - 7).ToString()));
                    lstColums.Add("期间累计");
                }
                else
                {
                    lstColums.Add(Convert.ToDateTime(data.RPT_DATE).GetDateTimeFormats('M')[0].ToString());
                }
            }

            DataColumn col;
            string strColumns = string.Empty;
            foreach (string s in lstColums)
            {
                if (dt.Columns.Contains(s) == false)
                {
                    if (s.Contains("累计"))
                    {
                        dt.Columns.Add(s).SetOrdinal(icount + 2);
                    }
                    else
                    {
                        col = new DataColumn(s);
                        dt.Columns.Add(col);
                    }
                }
            }

            #endregion

            #region //定义行
            DataRow dr = dt.NewRow();

            #region 根据用户选项，得到对应线别
            using (ProductionLineServiceClient client = new ProductionLineServiceClient())
            {
                model.LocationName = "102A";
                StringBuilder where = new StringBuilder();
                if (model != null)
                {
                    if (!string.IsNullOrEmpty(model.LocationName))
                    {
                        where.AppendFormat(" {0} Key LIKE '{1}%'"
                                            , where.Length > 0 ? "AND" : string.Empty
                                            , model.LocationName);
                    }
                }
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = where.ToString()
                };

                MethodReturnResult<IList<ProductionLine>> result = client.Get(ref cfg);
                if (result.Code == 0)
                {
                    ViewBag.List = result.Data;
                }

            }
            #endregion

            #region 得到相应原因代码
            IList<EquipmentReasonCodeCategory> lst = null;
            MethodReturnResult<IList<EquipmentReasonCodeCategory>> resultReason = null;
            using (EquipmentReasonCodeCategoryServiceClient client = new EquipmentReasonCodeCategoryServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    //Where = "Key.Type='0'"
                };
                 resultReason = client.Get(ref cfg);
                if (resultReason.Code <= 0 && resultReason.Data != null)
                {
                    lst = resultReason.Data;

                }
            }

            #endregion


            dr = dt.NewRow();
            dr[0] = "OEE";
            dr[1] = "###";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr[0] = "稼动率";
            dr[1] = "/RPT/OEEDailyData/ActRate";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr[0] = "良率";
            dr[1] = "###";
            dt.Rows.Add(dr);


            #region 一整天数据
            for (int i = 0; i < resultReason.Data.Count; i++)
            {
                EquipmentReasonCodeCategory obj = lst[i];
                dr = dt.NewRow();
                dr[0] = "&nbsp;&nbsp;" + obj.Key;
                dr[1] = "####";
                dt.Rows.Add(dr);
                //obj.Key
            }

            #endregion

            #region 白班数据
            for (int i = 0; i < resultReason.Data.Count; i++)
            {
                EquipmentReasonCodeCategory obj = lst[i];
                dr = dt.NewRow();
                dr[0] = "&nbsp;&nbsp;白班" + obj.Key;
                dr[1] = "####";
                dt.Rows.Add(dr);
                //obj.Key
            }

            dr = dt.NewRow();
            dr[0] = "&nbsp;&nbsp;白班合计";
            dr[1] = "###";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr[0] = "&nbsp;&nbsp;白班可用总时间";
            dr[1] = "###";
            dt.Rows.Add(dr);

            for (int i = 0; i < resultReason.Data.Count; i++)
            {
                EquipmentReasonCodeCategory obj = lst[i];
                dr = dt.NewRow();
                dr[0] = "&nbsp;&nbsp;白班" + obj.Key +"%";
                dr[1] = "####";
                dt.Rows.Add(dr);
                //obj.Key
            }
            dr = dt.NewRow();
            dr[0] = "&nbsp;&nbsp;白班时间稼动率";
            dr[1] = "###";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr[0] = "&nbsp;&nbsp;白班性能稼动率";
            dr[1] = "###";
            dt.Rows.Add(dr);

            #endregion

            #region 夜班数据
            for (int i = 0; i < resultReason.Data.Count; i++)
            {
                EquipmentReasonCodeCategory obj = lst[i];
                dr = dt.NewRow();
                dr[0] = "&nbsp;&nbsp;夜班" + obj.Key;
                dr[1] = "####";
                dt.Rows.Add(dr);
                //obj.Key
            }

            dr = dt.NewRow();
            dr[0] = "&nbsp;&nbsp;夜班合计";
            dr[1] = "###";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr[0] = "&nbsp;&nbsp;夜班可用总时间";
            dr[1] = "###";
            dt.Rows.Add(dr);

            for (int i = 0; i < resultReason.Data.Count; i++)
            {
                EquipmentReasonCodeCategory obj = lst[i];
                dr = dt.NewRow();
                dr[0] = "&nbsp;&nbsp;夜班" + obj.Key + "%";
                dr[1] = "####";
                dt.Rows.Add(dr);
                //obj.Key
            }
            dr = dt.NewRow();
            dr[0] = "&nbsp;&nbsp;夜班时间稼动率";
            dr[1] = "###";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr[0] = "&nbsp;&nbsp;夜班性能稼动率";
            dr[1] = "###";
            dt.Rows.Add(dr);

            #endregion

            #region 白班分线别数据
            for (int i = 0; i < ViewBag.List.Length; i++)
            {
                ProductionLine obj = ViewBag.List[i];
                for (int j = 0; j < resultReason.Data.Count; j++)
                {
                    EquipmentReasonCodeCategory objReason = lst[j];
                    dr = dt.NewRow();
                    dr[0] = "&nbsp;&nbsp;白班" + obj.Key + objReason.Key;
                    dr[1] = "####";
                    dt.Rows.Add(dr);
                }
                dr = dt.NewRow();
                dr[0] = "&nbsp;&nbsp;白班" + obj .Key + "合计";
                dr[1] = "###";
                dt.Rows.Add(dr);

                dr = dt.NewRow();
                dr[0] = "&nbsp;&nbsp;白班可用总时间";
                dr[1] = "###";
                dt.Rows.Add(dr);
                for (int j = 0; j < resultReason.Data.Count; j++)
                {
                    EquipmentReasonCodeCategory objReason = lst[j];
                    dr = dt.NewRow();
                    dr[0] = "&nbsp;&nbsp;白班" + obj.Key + objReason.Key + "%";
                    dr[1] = "####";
                    dt.Rows.Add(dr);
                }
                dr = dt.NewRow();
                dr[0] = "&nbsp;&nbsp;白班" + obj.Key + "时间稼动率";
                dr[1] = "###";
                dt.Rows.Add(dr);

                dr = dt.NewRow();
                dr[0] = "&nbsp;&nbsp;白班" + obj.Key + "性能稼动率";
                dr[1] = "###";
                dt.Rows.Add(dr);
            }
            

            #endregion

            #region 夜班分线别数据
            for (int i = 0; i < ViewBag.List.Length; i++)
            {
                ProductionLine obj = ViewBag.List[i];
                for (int j = 0; j < resultReason.Data.Count; j++)
                {
                    EquipmentReasonCodeCategory objReason = lst[j];
                    dr = dt.NewRow();
                    dr[0] = "&nbsp;&nbsp;夜班" + obj.Key + objReason.Key;
                    dr[1] = "####";
                    dt.Rows.Add(dr);
                }
                dr = dt.NewRow();
                dr[0] = "&nbsp;&nbsp;夜班" + obj.Key + "合计";
                dr[1] = "###";
                dt.Rows.Add(dr);

                dr = dt.NewRow();
                dr[0] = "&nbsp;&nbsp;夜班" + obj.Key + "可用总时间";
                dr[1] = "###";
                dt.Rows.Add(dr);
                for (int j = 0; j < resultReason.Data.Count; j++)
                {
                    EquipmentReasonCodeCategory objReason = lst[j];
                    dr = dt.NewRow();
                    dr[0] = "&nbsp;&nbsp;夜班" + obj.Key + objReason.Key + "%";
                    dr[1] = "####";
                    dt.Rows.Add(dr);
                }
                dr = dt.NewRow();
                dr[0] = "&nbsp;&nbsp;夜班" + obj.Key + "时间稼动率";
                dr[1] = "###";
                dt.Rows.Add(dr);

                dr = dt.NewRow();
                dr[0] = "&nbsp;&nbsp;夜班" + obj.Key + "性能稼动率";
                dr[1] = "###";
                dt.Rows.Add(dr);
            }


            #endregion
            #endregion

            string strColName = "";
            string strIType = "";
            string strFlag = "";
            int indexOfCol = 0;
            int selOfCol = -1;
            int selOfRow = -1;
            string nValue = "0";
            string a = "1";
            string b = "KPcs";
            try
            {
                for (int i = 0; i < dtWIPDailyData.Rows.Count; i++)
                {
                    strColName = dtWIPDailyData.Rows[i]["RPT_DATE"].ToString();//获取日期
                    //获取数据类型字段
                    strFlag = dtWIPDailyData.Rows[i]["QTY_FLAG"].ToString();//获取数据类型信息

                    strIType = dtWIPDailyData.Rows[i]["BUSINESS_TYPE"].ToString();//获取字段信息
                    if (strColName.Contains("累计"))
                    {
                        string s = strColName.Substring(5, strColName.Length - 7);
                        string date = string.Format("{0:M}", Convert.ToDateTime(strColName.Substring(5, strColName.Length - 7).ToString()));
                        //strColName = date + "累计";
                        strColName = "期间累计";
                    }
                    //天数据
                    else
                    {
                        strColName = Convert.ToDateTime(strColName).GetDateTimeFormats('M')[0].ToString();
                    }
                    selOfCol = -1;

                    //查询结果赋给每一列
                    for (indexOfCol = 0; indexOfCol < dt.Columns.Count; indexOfCol++)
                    {
                        if (dt.Columns[indexOfCol].ColumnName == strColName)
                        {
                            selOfCol = indexOfCol;
                            break;
                        }
                    }
                    if (selOfCol == -1)
                    {
                        continue;
                    }

                    for (int j = 0; j < dt.Rows.Count; j++)
                    {
                        string strTType = "";
                        //strTType = dt.Rows[j]["项目"].ToString();
                        strTType = dt.Rows[j]["项目"].ToString().Trim("&.nbsp;".ToCharArray());

                        if (strTType.Equals(strIType))
                        {
                            try
                            {
                                //如果为比例，则在数据后边加%
                                if (strFlag.Equals(a))
                                {
                                    //nValue = Convert.ToString(dtWIPDailyData.Rows[i]["QTY"] + "%");
                                    nValue = Math.Round((Convert.ToDecimal(dtWIPDailyData.Rows[i]["QTY"])), 2).ToString() + "%";
                                }
                                else
                                {

                                    //nValue = Convert.ToString(dtWIPDailyData.Rows[i]["QTY"]);
                                    nValue = Math.Round((Convert.ToDecimal(dtWIPDailyData.Rows[i]["QTY"])), 2).ToString();
                                }

                            }
                            catch
                            {
                                nValue = "0";
                            }
                            dt.Rows[j][selOfCol] = nValue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            ViewBag.ListData = dt;
            ViewBag.StartDateTime = model.StartDate;
            ViewBag.EndDateTime = model.EndDate;
            ViewBag.StepName = model.EquipmentName;
            if (Request.IsAjaxRequest())
            {
                return PartialView("_ListPartial", model);
            }
            else
            {
                return View(model);
            }
        }
        #endregion

    }
}