using ServiceCenter.Client.Mvc.Areas.RPT.Models;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Service.Client.RPT;
using ServiceCenter.MES.Service.Contract.RPT;
using ServiceCenter.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ServiceCenter.Client.Mvc.Areas.RPT.Controllers
{
    public class WIPMoveForStepController : Controller
    {
        SortedList<string, string> sl = new SortedList<string, string>();
        //
        // GET: /RPT/WIPMoveForStep/
        public ActionResult Index()
        {
            return View( new WIPMoveForStepDataViewModel());
        }

        public ActionResult GetHighchartsResult(WIPMoveForStepDataViewModel model)
        {
            string lineName="102B-D线";

            //1 进站 2 出站 5 报废 6 不良 7 补料
            DataTable dtMoveDataForStepData = new DataTable();
            WIPMoveGetParameter Parameter = new WIPMoveGetParameter()
            {
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                MaterialCode = model.MaterialCode,
                StepName = model.StepName,
                LocationName = model.LocationName,
                ShiftName = model.ShiftName,
                OrderNumber = model.OrderNumber
            };

            IList<ProductionLine> lst = new List<ProductionLine>();
            PagingConfig cfg = new PagingConfig()
            {
                IsPaging = false,
                Where = string.Format("Name!='{0}'",lineName)
            };
            using (ProductionLineServiceClient client = new ProductionLineServiceClient())
            {
                MethodReturnResult<IList<ProductionLine>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lst = result.Data;
                }
            }



            using (WIPMoveServiceClient client = new WIPMoveServiceClient())
            {
                MethodReturnResult<DataSet> rst = client.GetWipMoveForStep(Parameter);
                if (rst.Code <= 0 && rst.Data != null && rst.Data.Tables.Count > 0)
                {
                    dtMoveDataForStepData = rst.Data.Tables[0];
                }
            }

            if (dtMoveDataForStepData != null && dtMoveDataForStepData.Rows.Count > 0)
            {
                sl = AssembleSpline(dtMoveDataForStepData, lst);
            }
            return Json(sl, JsonRequestBehavior.AllowGet);
        }

        public SortedList<string, string> AssembleSpline(DataTable dtMoveDataForStepData, IList<ProductionLine> lstLineCode)
        {

            List<string> list = new List<string>() { "1", "2", "5", "6", "7" };
            string strup = string.Empty, strlow = string.Empty;



            System.Collections.Hashtable hsDrawLinesY = new System.Collections.Hashtable();
            StringBuilder strBuilderX = new StringBuilder();
            StringBuilder strBuildera = new StringBuilder();
            System.Collections.Hashtable hsDrawLinesX = new System.Collections.Hashtable();
            StringBuilder strBuilderY = new StringBuilder();


            //组装柱子类型
            foreach (string botaId in list)
            {
                strBuilderY = new StringBuilder();
                hsDrawLinesY.Add(botaId, strBuilderY);
            }

            int j = 0;
            foreach (var data in lstLineCode)
            {
                hsDrawLinesX.Add(data.Key, j);
                strBuilderX.Append("'" + data.Name.ToString() + "',");//X      
                j = j + 1;
            }



            int indexOfX = 0;
            Hashtable hsDrawPoint = new Hashtable();
            for (int i = 0; i < dtMoveDataForStepData.Rows.Count; i++)
            {
                string dbKey = "";
                dbKey = dtMoveDataForStepData.Rows[i]["ACTIVITY"].ToString();


                foreach (object key in hsDrawLinesY.Keys)
                {
                    strBuilderY = (StringBuilder)hsDrawLinesY[key];
                    string valueOfY = "";

                    string strPointOfX = "";
                    string strKey = (string)key;
                    if (string.Compare(strKey, dbKey, true) == 0)
                    {
                        if (String.IsNullOrEmpty(dtMoveDataForStepData.Rows[i]["QUANTITY"].ToString()))
                        {
                            valueOfY = "null";
                        }
                        else
                        {
                            valueOfY = dtMoveDataForStepData.Rows[i]["QUANTITY"].ToString();
                        }


                        if (hsDrawLinesX.ContainsKey(dtMoveDataForStepData.Rows[i]["LINE_NAME"].ToString()))
                        {
                            indexOfX = (int)hsDrawLinesX[dtMoveDataForStepData.Rows[i]["LINE_NAME"].ToString()];
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
                string strname = string.Empty;//1 进站 2 出站 5 报废 6 不良 7 补料
                if (line.Key == "1")
                { strname = "进站"; }
                if (line.Key == "2")
                { strname = "出站"; }
                if (line.Key == "5")
                { strname = "报废"; }
                if (line.Key == "6")
                { strname = "不良"; }
                if (line.Key == "7")
                { strname = "补料"; }
                stry = stry + "{ name: '" + strname + "',type:'column', data: [" + line.Value.ToString().Trim(',') + "]},";
            }
            stry = stry.ToString().Trim(',');
//            string dataLabels = @",dataLabels: 
//                            {
//                                enabled: true,rotation: 0,color: '#262626',align: 'center',x: -3,y: 15,
//                                formatter: function () {if (this.y != 0) {return this.y;}},
//                                style: { fontSize: '13px',fontFamily: 'Microsoft YaHei'}
//                            }";
//            stry = stry + dataLabels+"]";
            stry = stry + "]";

            sl.Add("mAxis", strx);
            sl.Add("nAxis", stry);
            return sl;
        }


        #region
        public ActionResult IndexOfWIP()
        {
            return View(new WIPMoveForStepDataViewModel());
        }
        public ActionResult QueryForCharts(WIPMoveForStepDataViewModel model)
        {
            DataTable dtData = new DataTable();
            //获取工序MOVE数据。
            using (WIPMoveServiceClient client = new WIPMoveServiceClient())
            {
                MethodReturnResult<DataSet> rst = client.GetDailyQuantityOfWIP(new QMSemiProductionGetParameter()
                {
                    LocationName = model.LocationName,
                    StartDate = model.StartDate,
                });
                if (rst.Code <= 0 && rst.Data != null && rst.Data.Tables.Count > 0)
                {
                    dtData = rst.Data.Tables[0];
                }
            }
            AssembleSplineForWIP(dtData);
            return Json(sl, JsonRequestBehavior.AllowGet);
        }

        public SortedList<string, string> AssembleSplineForWIP(DataTable dtData)
        {
            string strup = string.Empty, strlow = string.Empty;
            List<string> list = new List<string>();
            System.Collections.Hashtable hsDrawLinesY = new System.Collections.Hashtable();
            StringBuilder strBuilderX = new StringBuilder();
            StringBuilder strBuildera = new StringBuilder();
            System.Collections.Hashtable hsDrawLinesX = new System.Collections.Hashtable();
            StringBuilder strBuilderY = new StringBuilder();

            var queryStep = from t in dtData.AsEnumerable()
                            group t by new { t1 = t.Field<String>("ROUTE_STEP_NAME") } into m
                            select new
                            {
                                ROUTE_STEP_NAME = m.First().Field<String>("ROUTE_STEP_NAME")
                            };
            int j = 0;
            foreach (var data in queryStep)
            {
                hsDrawLinesX.Add(data.ROUTE_STEP_NAME.ToString(), j);
                strBuilderX.Append("'" + data.ROUTE_STEP_NAME.ToString() + "',");//X      
                j = j + 1;
            }
            strBuilderY = new StringBuilder();
            hsDrawLinesY.Add("WIP", strBuilderY);
            int indexOfX = 0;
            Hashtable hsDrawPoint = new Hashtable();
            for (int i = 0; i < dtData.Rows.Count; i++)
            {
                string dbKey = "";

                dbKey = "WIP";

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
                        if (hsDrawLinesX.ContainsKey(dtData.Rows[i]["ROUTE_STEP_NAME"].ToString()))
                        {
                            indexOfX = (int)hsDrawLinesX[dtData.Rows[i]["ROUTE_STEP_NAME"].ToString()];
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
            var stry = "[";
            foreach (DictionaryEntry line in hsDrawLinesY)
            {
                stry = stry + "{ name: '" + line.Key + "',type:'column', data: [" + line.Value.ToString().Trim(',') + "]},{ name: '" + line.Key + "',type:'spline', data: [" + line.Value.ToString().Trim(',') + "]}";
            }
            stry = stry.ToString().Trim(',');
            stry = stry + "]";

            sl.Add("mAxis", strx);
            sl.Add("nAxis", stry);
            return sl;
        }
        #endregion
    }
}