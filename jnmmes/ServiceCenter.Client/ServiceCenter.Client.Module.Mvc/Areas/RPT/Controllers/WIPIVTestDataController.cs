using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using ServiceCenter.Client.Mvc.Areas.RPT.Models;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Model.SPC;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Service.Client.RPT;
using ServiceCenter.MES.Service.Client.SPC;
using ServiceCenter.MES.Service.Contract.RPT;
using ServiceCenter.Model;
using ServiceCenter.Service.Client;
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
    public class WIPIVTestDataController : Controller
    {
        SortedList<string, string> sl = new SortedList<string, string>();
        public ActionResult Index(WIPIVTestDataViewModel model)
        {
            return View(new WIPIVTestDataViewModel());
        }

        public ActionResult IvTestJZ(WIPIVTestDataViewModel model)
        {
            return View(new WIPIVTestDataViewModel());
        }

        public ActionResult GetHighchartsResult(WIPIVTestDataViewModel model)
        {
            DataTable dtIVData = new DataTable();
            string strx = string.Empty, stry = string.Empty, strup = string.Empty, strlow = string.Empty;

            using (WIPIVTestServiceClient client = new WIPIVTestServiceClient())
            {
                MethodReturnResult<DataSet> rst = client.Get(new WIPIVTestGetParameter()
                {
                   
                    EquipmentCode = AssembleChar(model.EquipmentCode == "Select options" ? "" : model.EquipmentCode),
                    Attr_1 = AssembleChar(model.Attr_1),
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    LineCode = model.LineCode,
                    CalibrationId = model.CalibrationPlateID
                });
                if (rst.Code <= 0 && rst.Data != null && rst.Data.Tables.Count > 0)
                {
                    dtIVData = rst.Data.Tables[0];
                }
            }

            if (dtIVData != null && dtIVData.Rows.Count > 0)
            {

                sl = AssembleSpline(dtIVData, new WIPIVTestGetParameter()
                {
                    EquipmentCode = (model.EquipmentCode == "Select options" ? "" : model.EquipmentCode),
                    Attr_1 = (model.Attr_1),
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    LineCode=model.LineCode,
                    CalibrationId=model.CalibrationPlateID
                });
            }
            return Json(sl, JsonRequestBehavior.AllowGet);
        }
        public string AssembleChar(string str)
        {
            string s = "";
            if (!string.IsNullOrWhiteSpace(str))
            {
                if (str.Contains(','))
                {
                    string[] strs = str.Split(',');
                    for (int i = 0; i < strs.Length; i++)
                    {
                        if (s == "")
                        { s += "'" + strs[i].TrimStart() + "'"; }
                        else
                        { s += ",'" + strs[i].TrimStart() + "'"; }
                    }
                }
                else
                {
                    s += "'" + str.TrimStart() + "'";
                }
            }
            return s;
        }
        public SortedList<string, string> AssembleSpline(DataTable dtIVTESTDataForSpline, WIPIVTestGetParameter parameter)
        {
            SPCJobParam jobParam = null;  
            using (SPCJobServiceClient client = new SPCJobServiceClient())
            {
                MethodReturnResult<IList<SPCJobParam>> retJobParams = client.GetJobParams("1");
                if (retJobParams.Code == 0 && retJobParams.Data != null && retJobParams.Data.Count > 0)
                {
                    foreach (SPCJobParam job in retJobParams.Data)
                    {
                        jobParam = job;

                    }
                }
            } 
           
            string strup = string.Empty, strlow = string.Empty;
            string strUb = string.Empty, strUs = string.Empty;
            string strLc = string.Empty, strLs = string.Empty, strLb = string.Empty;
            string strLmax = string.Empty, strLmin = string.Empty, strLYinterval = string.Empty;
            string strTg = string.Empty;
           // string[] arrEqpCode = parameter.EquipmentCode.Replace("'", "").Split(',');
            string[] arrEqpCode = dtIVTESTDataForSpline.Rows[0]["EQUIPMENT_CODE"].ToString().Replace("'", "").Split(',');
        
            List<string> list = new List<string>();
            System.Collections.Hashtable hsDrawLinesY = new System.Collections.Hashtable();
            StringBuilder strBuilderX = new StringBuilder();
            StringBuilder strBuildera = new StringBuilder();
            System.Collections.Hashtable hsDrawLinesX = new System.Collections.Hashtable();
            StringBuilder strBuilderY = new StringBuilder();

            if (arrEqpCode.Length > 0)
            {
                foreach (var data in arrEqpCode)
                {
                    if (!string.IsNullOrEmpty(parameter.Attr_1))
                    {
                        list.Add(data + parameter.Attr_1);
                    }
                    else
                    {
                        list.Add(data);
                    }
                }
            }

            foreach (string botaId in list)
            {
                strBuilderY = new StringBuilder();
                hsDrawLinesY.Add(botaId, strBuilderY);
            }
            var query = from t in dtIVTESTDataForSpline.AsEnumerable()
                        group t by new { t1 = t.Field<DateTime>("TEST_TIME") } into m
                        select new
                        {
                            TEST_TIME = m.First().Field<DateTime>("TEST_TIME")
                        };
            int j = 0;
            foreach (var data in query)
            {
                hsDrawLinesX.Add(data.TEST_TIME.ToString(), j);
                strBuilderX.Append("'" + data.TEST_TIME.ToString() + "',");//X      
                j = j + 1;
            }
            int indexOfX = 0;
            Hashtable hsDrawPoint = new Hashtable();
            for (int i = 0; i < dtIVTESTDataForSpline.Rows.Count; i++)
            {
                string dbKey = "";
                if (!string.IsNullOrEmpty(parameter.Attr_1))
                {
                    dbKey = dtIVTESTDataForSpline.Rows[i]["EQUIPMENT_CODE"].ToString() + dtIVTESTDataForSpline.Rows[i]["ATTR_1"].ToString();
                }
                else
                {
                    dbKey = dtIVTESTDataForSpline.Rows[i]["EQUIPMENT_CODE"].ToString();
                }

                foreach (object key in hsDrawLinesY.Keys)
                {
                    strBuilderY = (StringBuilder)hsDrawLinesY[key];
                    string valueOfY = "";

                    string strPointOfX = "";
                    string strKey = (string)key;
                    if (string.Compare(strKey, dbKey, true) == 0)
                    {
                        if (String.IsNullOrEmpty(dtIVTESTDataForSpline.Rows[i]["PM"].ToString()))
                        {
                            valueOfY = "null";
                        }
                        else
                        {
                            valueOfY = dtIVTESTDataForSpline.Rows[i]["PM"].ToString();
                        }

                        if (hsDrawLinesX.ContainsKey(dtIVTESTDataForSpline.Rows[i]["TEST_TIME"].ToString()))
                        {
                            indexOfX = (int)hsDrawLinesX[dtIVTESTDataForSpline.Rows[i]["TEST_TIME"].ToString()];
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
            int nMaxOfXPoint = hsDrawLinesX.Count;
            int nMinOfXPoint = 1;
            int nIndexOfXPoint = 1;
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
                  if (jobParam != null && (nIndexOfXPoint == nMinOfXPoint || nIndexOfXPoint == nMaxOfXPoint))
                 {
                     if (parameter.CalibrationId != null && parameter.CalibrationId != "")
                     {
                         using (CalibrationPlateServiceClient client = new CalibrationPlateServiceClient())
                         {
                             MethodReturnResult<CalibrationPlate> result = client.Get(parameter.CalibrationId);
                             if (result.Code == 0)
                             {
                                 //5条控制线
                                 //strUs = strUs + jobParam.UpperSpecification.ToString() + ",";
                                 //strup = strup + jobParam.UpperControl.ToString() + ",";
                                 //strTg = strTg + jobParam.Target.ToString() + ",";
                                 //strlow = strlow + jobParam.LowerControl.ToString() + ",";
                                 //strLs = strLs + jobParam.LowerSpecification.ToString() + ",";
                                 strUs = strUs + result.Data.MaxPM + ","; ;
                                 strup = strup + result.Data.MaxPM + ",";
                                 strTg = strTg + jobParam.Target.ToString() + ",";
                                 strlow = strlow + result.Data.MinPM + ",";
                                 strLs = strLs + result.Data.MinPM + ",";
                             }
                         }

                     }

                     else
                     {  
                         strUs = strUs + jobParam.UpperSpecification.ToString() + ",";
                         strup = strup + jobParam.UpperControl.ToString() + ",";
                         strTg = strTg + jobParam.Target.ToString() + ",";
                         strlow = strlow + jobParam.LowerControl.ToString() + ",";
                         strLs = strLs + jobParam.LowerSpecification.ToString() + ",";
                     
                     }

                    //y轴最大最小值以及间距
                    strLmax = jobParam.LineUpper.ToString();
                    strLmin = jobParam.LineLower.ToString();
                    strLYinterval = jobParam.LineYinterval.ToString();
                }
                else
                {
                    strup = strup + "null,";
                    strlow = strlow + "null,";
                    strUb = strUb + "null,";
                    strUs = strUs + "null,";
                    strTg = strTg + "null,";
                    strLb = strLb + "null,";
                    strLs = strLs + "null,";
                }
                nIndexOfXPoint = nIndexOfXPoint + 1;

            }

            var strx = @"[" + strBuilderX.ToString().Trim(',') + "]";
            string stryLevel = strLmax + "|" + strLmin + "|" + strLYinterval;
            var stry = "";            
            stry = @"[";
            stry = stry + "{ name: 'USL=" + strUs.Substring(0, strUs.IndexOf(',')) + "',type:'spline', data: [" + strUs.Trim(',') + "]},";
            stry = stry + "{ name: 'UCL=" + strup.Substring(0, strup.IndexOf(',')) + "',type:'spline', data: [" + strup.Trim(',') + "]},";
            stry = stry + "{ name: 'Target=" + strTg.Substring(0, strTg.IndexOf(',')) + "',type:'spline', data: [" + strTg.Trim(',') + "]},";
            stry = stry + "{ name: 'LCL=" + strlow.Substring(0, strlow.IndexOf(',')) + "',type:'spline', data: [" + strlow.Trim(',') + "]},";
            stry = stry + "{ name: 'LSL=" + strLs.Substring(0, strLs.IndexOf(',')) + "',type:'spline', data: [" + strLs.Trim(',') + "]},";
            foreach (DictionaryEntry line in hsDrawLinesY)
            {
                stry = stry + "{ name: '" + line.Key + "',type:'spline', data: [" + line.Value.ToString().Trim(',') + "]},";
            }
            stry = stry.ToString().Trim(',');
            stry = stry + "]";

            sl.Add("mAxis", strx);
            sl.Add("level", stryLevel);
            sl.Add("nAxis", stry);
            return sl;
        }

        #region 校准
        public ActionResult GetHighchartsResultForJZ(WIPIVTestDataViewModel model)
        {
            DataTable dtIVData = new DataTable();
            string strx = string.Empty, stry = string.Empty, strup = string.Empty, strlow = string.Empty;

            using (WIPIVTestServiceClient client = new WIPIVTestServiceClient())
            {
                MethodReturnResult<DataTable> rst = client.GetIVDataForJZ(new WIPIVTestGetParameter()
                {
                    //EquipmentCode = AssembleChar(model.EquipmentCode == "Select options" ? "" : model.EquipmentCode),
                    CalibrationId = model.CalibrationPlateID,
                    LineCode=model.LineCode,
                    Lot_Number = "JZ",
                    StartDate = model.StartDate,
                    EndDate = model.EndDate
                });
                if (rst.Code <= 0 && rst.Data != null && rst.Data.Rows.Count > 0)
                {
                    dtIVData = rst.Data;
                }
            }

            if (dtIVData != null && dtIVData.Rows.Count > 0)
            {

                sl = AssembleSpline(dtIVData, new WIPIVTestGetParameter()
                {
                    Lot_Number = "JZ",
                    EquipmentCode = (model.EquipmentCode == "Select options" ? "" : model.EquipmentCode),
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    CalibrationId = model.CalibrationPlateID,
                    LineCode = model.LineCode
                });
            }
            return Json(sl, JsonRequestBehavior.AllowGet);
        }
        public SortedList<string, string> AssembleSplineForJZ(DataTable dtIVTESTDataForSpline, WIPIVTestGetParameter parameter)
        {
            SPCJobParam jobParam = null;
            using (SPCJobServiceClient client = new SPCJobServiceClient())
            {
                MethodReturnResult<IList<SPCJobParam>> retJobParams = client.GetJobParams("1");
                if (retJobParams.Code == 0 && retJobParams.Data != null && retJobParams.Data.Count > 0)
                {
                    foreach (SPCJobParam job in retJobParams.Data)
                    {
                        jobParam = job;

                    }
                }
            }
            string strup = string.Empty, strlow = string.Empty;
            string strUb = string.Empty, strUs = string.Empty;
            string strLc = string.Empty, strLs = string.Empty, strLb = string.Empty;
            string strLmax = string.Empty, strLmin = string.Empty, strLYinterval = string.Empty;
            string strTg = string.Empty;
            string[] arrEqpCode = parameter.EquipmentCode.Replace("'", "").Split(',');
            List<string> list = new List<string>();
            System.Collections.Hashtable hsDrawLinesY = new System.Collections.Hashtable();
            StringBuilder strBuilderX = new StringBuilder();
            StringBuilder strBuildera = new StringBuilder();
            System.Collections.Hashtable hsDrawLinesX = new System.Collections.Hashtable();
            StringBuilder strBuilderY = new StringBuilder();

            if (arrEqpCode.Length > 0)
            {
                foreach (var data in arrEqpCode)
                {
                    list.Add(data);
                }
            }

            foreach (string botaId in list)
            {
                strBuilderY = new StringBuilder();
                hsDrawLinesY.Add(botaId, strBuilderY);
            }
            var query = from t in dtIVTESTDataForSpline.AsEnumerable()
                        group t by new { t1 = t.Field<DateTime>("TEST_TIME") } into m
                        select new
                        {
                            TEST_TIME = m.First().Field<DateTime>("TEST_TIME")
                        };
            int j = 0;
            foreach (var data in query)
            {
                hsDrawLinesX.Add(data.TEST_TIME.ToString(), j);
                strBuilderX.Append("'" + data.TEST_TIME.ToString() + "',");//X      
                j = j + 1;
            }
            int indexOfX = 0;
            Hashtable hsDrawPoint = new Hashtable();
            for (int i = 0; i < dtIVTESTDataForSpline.Rows.Count; i++)
            {
                string dbKey = "";
                dbKey = dtIVTESTDataForSpline.Rows[i]["EQUIPMENT_CODE"].ToString();

                foreach (object key in hsDrawLinesY.Keys)
                {
                    strBuilderY = (StringBuilder)hsDrawLinesY[key];
                    string valueOfY = "";

                    string strPointOfX = "";
                    string strKey = (string)key;
                    if (string.Compare(strKey, dbKey, true) == 0)
                    {
                        if (String.IsNullOrEmpty(dtIVTESTDataForSpline.Rows[i]["PM"].ToString()))
                        {
                            valueOfY = "null";
                        }
                        else
                        {
                            valueOfY = dtIVTESTDataForSpline.Rows[i]["PM"].ToString();
                        }

                        if (hsDrawLinesX.ContainsKey(dtIVTESTDataForSpline.Rows[i]["TEST_TIME"].ToString()))
                        {
                            indexOfX = (int)hsDrawLinesX[dtIVTESTDataForSpline.Rows[i]["TEST_TIME"].ToString()];
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
            int nMaxOfXPoint = hsDrawLinesX.Count;
            int nMinOfXPoint = 1;
            int nIndexOfXPoint = 1;
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
                if (jobParam != null && (nIndexOfXPoint == nMinOfXPoint || nIndexOfXPoint == nMaxOfXPoint))
                {
                    //5条控制线
                    strUs = strUs + jobParam.UpperSpecification.ToString() + ",";
                    strup = strup + jobParam.UpperControl.ToString() + ",";
                    strTg = strTg + jobParam.Target.ToString() + ",";
                    strlow = strlow + jobParam.LowerControl.ToString() + ",";
                    strLs = strLs + jobParam.LowerSpecification.ToString() + ",";

                    //y轴最大最小值以及间距
                    strLmax = jobParam.LineUpper.ToString();
                    strLmin = jobParam.LineLower.ToString();
                    strLYinterval = jobParam.LineYinterval.ToString();
                }
                else
                {
                    strup = strup + "null,";
                    strlow = strlow + "null,";
                    strUb = strUb + "null,";
                    strUs = strUs + "null,";
                    strTg = strTg + "null,";
                    strLb = strLb + "null,";
                    strLs = strLs + "null,";
                }
                nIndexOfXPoint = nIndexOfXPoint + 1;
            }

            var strx = @"[" + strBuilderX.ToString().Trim(',') + "]";
            string stryLevel = strLmax + "|" + strLmin + "|" + strLYinterval;
            var stry = "";
            stry = @"[";
            stry = stry + "{ name: 'USL=" + strUs.Substring(0, strUs.IndexOf(',')) + "',type:'spline', data: [" + strUs.Trim(',') + "]},";
            stry = stry + "{ name: 'UCL=" + strup.Substring(0, strup.IndexOf(',')) + "',type:'spline', data: [" + strup.Trim(',') + "]},";
            stry = stry + "{ name: 'Target=" + strTg.Substring(0, strTg.IndexOf(',')) + "',type:'spline', data: [" + strTg.Trim(',') + "]},";
            stry = stry + "{ name: 'LCL=" + strlow.Substring(0, strlow.IndexOf(',')) + "',type:'spline', data: [" + strlow.Trim(',') + "]},";
            stry = stry + "{ name: 'LSL=" + strLs.Substring(0, strLs.IndexOf(',')) + "',type:'spline', data: [" + strLs.Trim(',') + "]},";
            foreach (DictionaryEntry line in hsDrawLinesY)
            {
                stry = stry + "{ name: '" + line.Key + "',type:'spline', data: [" + line.Value.ToString().Trim(',') + "]},";
            }
            stry = stry.ToString().Trim(',');
            stry = stry + "]";

            sl.Add("mAxis", strx);
            sl.Add("level", stryLevel);
            sl.Add("nAxis", stry);
            return sl;
        }
        #endregion

        #region CTM
        public ActionResult IvTestForCTM(WIPIVTestDataViewModel model)
        {
            return View(new WIPIVTestDataViewModel());
        }

        public ActionResult GetHighchartsResultForCTM(WIPIVTestDataViewModel model)
        {
            DataTable dtIVData = new DataTable();
            string strx = string.Empty, stry = string.Empty, strup = string.Empty, strlow = string.Empty;

            using (WIPIVTestServiceClient client = new WIPIVTestServiceClient())
            {
                MethodReturnResult<DataSet> rst = client.GetIVDataForCTM(new WIPIVTestGetParameter()
                {
                    EquipmentCode = AssembleChar(model.EquipmentCode == "Select options" ? "" : model.EquipmentCode),
                    Attr_1 = AssembleChar(model.Attr_1),
                    StartDate = model.StartDate,
                    EndDate = model.EndDate
                });
                if (rst.Code <= 0 && rst.Data != null && rst.Data.Tables.Count > 0)
                {
                    dtIVData = rst.Data.Tables[0];
                }
            }

            if (dtIVData != null && dtIVData.Rows.Count > 0)
            {

                sl = AssembleSplineForCTM(dtIVData, new WIPIVTestGetParameter()
                {
                    EquipmentCode = (model.EquipmentCode == "Select options" ? "" : model.EquipmentCode),
                    Attr_1 = (model.Attr_1),
                    StartDate = model.StartDate,
                    EndDate = model.EndDate
                });
            }
            return Json(sl, JsonRequestBehavior.AllowGet);
        }
     
        public SortedList<string, string> AssembleSplineForCTM(DataTable dtIVTESTDataForSpline, WIPIVTestGetParameter parameter)
        {
            SPCJobParam jobParam = null;
            using (SPCJobServiceClient client = new SPCJobServiceClient())
            {
                MethodReturnResult<IList<SPCJobParam>> retJobParams = client.GetJobParams("2");
                if (retJobParams.Code == 0 && retJobParams.Data != null && retJobParams.Data.Count > 0)
                {
                    foreach (SPCJobParam job in retJobParams.Data)
                    {
                        jobParam = job;

                    }
                }
            }
            string strup = string.Empty, strlow = string.Empty;
            string strUb = string.Empty, strUs = string.Empty;
            string strLc = string.Empty, strLs = string.Empty, strLb = string.Empty;
            string strLmax = string.Empty, strLmin = string.Empty, strLYinterval = string.Empty;
            string strTg = string.Empty;
            string[] arrEqpCode = parameter.EquipmentCode.Replace("'", "").Split(',');
            List<string> list = new List<string>();
            System.Collections.Hashtable hsDrawLinesY = new System.Collections.Hashtable();
            StringBuilder strBuilderX = new StringBuilder();
            StringBuilder strBuildera = new StringBuilder();
            System.Collections.Hashtable hsDrawLinesX = new System.Collections.Hashtable();
            StringBuilder strBuilderY = new StringBuilder();

            if (arrEqpCode.Length > 0)
            {
                foreach (var data in arrEqpCode)
                {
                    if (!string.IsNullOrEmpty(parameter.Attr_1))
                    {
                        list.Add(data + parameter.Attr_1);
                    }
                    else
                    {
                        list.Add(data);
                    }
                }
            }

            foreach (string botaId in list)
            {
                strBuilderY = new StringBuilder();
                hsDrawLinesY.Add(botaId, strBuilderY);
            }
            var query = from t in dtIVTESTDataForSpline.AsEnumerable()
                        group t by new { t1 = t.Field<DateTime>("TEST_TIME") } into m
                        select new
                        {
                            TEST_TIME = m.First().Field<DateTime>("TEST_TIME")
                        };
            int j = 0;
            foreach (var data in query)
            {
                hsDrawLinesX.Add(data.TEST_TIME.ToString(), j);
                strBuilderX.Append("'" + data.TEST_TIME.ToString() + "',");//X      
                j = j + 1;
            }
            int indexOfX = 0;
            Hashtable hsDrawPoint = new Hashtable();
            for (int i = 0; i < dtIVTESTDataForSpline.Rows.Count; i++)
            {
                string dbKey = "";
                if (!string.IsNullOrEmpty(parameter.Attr_1))
                {
                    dbKey = dtIVTESTDataForSpline.Rows[i]["EQUIPMENT_CODE"].ToString() + dtIVTESTDataForSpline.Rows[i]["ATTR_1"].ToString();
                }
                else
                {
                    dbKey = dtIVTESTDataForSpline.Rows[i]["EQUIPMENT_CODE"].ToString();
                }

                foreach (object key in hsDrawLinesY.Keys)
                {
                    strBuilderY = (StringBuilder)hsDrawLinesY[key];
                    string valueOfY = "";

                    string strPointOfX = "";
                    string strKey = (string)key;
                    if (string.Compare(strKey, dbKey, true) == 0)
                    {
                        if (String.IsNullOrEmpty(dtIVTESTDataForSpline.Rows[i]["RATE"].ToString()))
                        {
                            valueOfY = "null";
                        }
                        else
                        {
                            valueOfY = dtIVTESTDataForSpline.Rows[i]["RATE"].ToString();
                        }

                        if (hsDrawLinesX.ContainsKey(dtIVTESTDataForSpline.Rows[i]["TEST_TIME"].ToString()))
                        {
                            indexOfX = (int)hsDrawLinesX[dtIVTESTDataForSpline.Rows[i]["TEST_TIME"].ToString()];
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
            int nMaxOfXPoint = hsDrawLinesX.Count;
            int nMinOfXPoint = 1;
            int nIndexOfXPoint = 1;
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
                if (jobParam != null && (nIndexOfXPoint == nMinOfXPoint || nIndexOfXPoint == nMaxOfXPoint))
                {
                    //5条控制线
                    strUs = strUs + jobParam.UpperSpecification.ToString() + ",";
                    strup = strup + jobParam.UpperControl.ToString() + ",";
                    strTg = strTg + jobParam.Target.ToString() + ",";
                    strlow = strlow + jobParam.LowerControl.ToString() + ",";
                    strLs = strLs + jobParam.LowerSpecification.ToString() + ",";

                    //y轴最大最小值以及间距
                    strLmax = jobParam.LineUpper.ToString();
                    strLmin = jobParam.LineLower.ToString();
                    strLYinterval = jobParam.LineYinterval.ToString();
                }
                else
                {
                    strup = strup + "null,";
                    strlow = strlow + "null,";
                    strUb = strUb + "null,";
                    strUs = strUs + "null,";
                    strTg = strTg + "null,";
                    strLb = strLb + "null,";
                    strLs = strLs + "null,";
                }
                nIndexOfXPoint = nIndexOfXPoint + 1;

            }

            var strx = @"[" + strBuilderX.ToString().Trim(',') + "]";
            string stryLevel = strLmax + "|" + strLmin + "|" + strLYinterval;
            var stry = "";
            stry = @"[";
            stry = stry + "{ name: 'USL=" + strUs.Substring(0, strUs.IndexOf(',')) + "',type:'spline', data: [" + strUs.Trim(',') + "]},";
            stry = stry + "{ name: 'UCL=" + strup.Substring(0, strup.IndexOf(',')) + "',type:'spline', data: [" + strup.Trim(',') + "]},";
            stry = stry + "{ name: 'Target=" + strTg.Substring(0, strTg.IndexOf(',')) + "',type:'spline', data: [" + strTg.Trim(',') + "]},";
            stry = stry + "{ name: 'LCL=" + strlow.Substring(0, strlow.IndexOf(',')) + "',type:'spline', data: [" + strlow.Trim(',') + "]},";
            stry = stry + "{ name: 'LSL=" + strLs.Substring(0, strLs.IndexOf(',')) + "',type:'spline', data: [" + strLs.Trim(',') + "]},";
            foreach (DictionaryEntry line in hsDrawLinesY)
            {
                stry = stry + "{ name: '" + line.Key + "',type:'spline', data: [" + line.Value.ToString().Trim(',') + "]},";
            }
            stry = stry.ToString().Trim(',');
            stry = stry + "]";

            sl.Add("mAxis", strx);
            sl.Add("level", stryLevel);
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