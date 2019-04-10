using ServiceCenter.Client.Mvc.Areas.RPT.Models;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Service.Client.RPT;
using ServiceCenter.MES.Service.Contract.RPT;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ServiceCenter.Common;
using System.Threading.Tasks;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using ServiceCenter.Client.Mvc.Resources;
using System.IO;

namespace ServiceCenter.Client.Mvc.Areas.RPT.Controllers
{
    public class RPTEquipmentController : Controller
    {
        private EnumLotActivity[] activities = new EnumLotActivity[]
        {
            EnumLotActivity.TrackIn,
            EnumLotActivity.TrackOut,
            EnumLotActivity.Defect,
            EnumLotActivity.Scrap,
            EnumLotActivity.Patch
        };
        // GET: RPT/RPTEquipment
        Dictionary<string, string> dicColumn = new Dictionary<string, string>()
        {
            {"EQUIPMENT_CODE","设备"}
        };
        //
        // GET: /RPT/WIPMove/
        public ActionResult Index(RTPEquipmentViewModels model)
        {
            return View(model);
        }
        //
        // POST: /RPT/WIPMove/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Query(RTPEquipmentViewModels model)
        {
            DataTable dtData = new DataTable();
            //获取工序MOVE数据。
            using (QMSemiProductionServiceClient client = new QMSemiProductionServiceClient())
            {
                MethodReturnResult<DataSet> rst = client.GetEquipmentDailyMoveForOEE(model.EquipmentCode, model.curDate);
                if (rst.Code <= 0 && rst.Data != null && rst.Data.Tables.Count > 0)
                {
                    dtData = rst.Data.Tables[0];
                }
            }
            //获取设备数据。
            List<string> lstEquipment = new List<string>();
            var queryEqp = from t in dtData.AsEnumerable()
                           group t by new { t1 = t.Field<String>("EQUIPMENT_CODE") } into m
                           select new
                           {
                               EQUIPMENT_CODE = m.First().Field<String>("EQUIPMENT_CODE")
                           };
            foreach (var data in queryEqp)
            {
                lstEquipment.Add(data.EQUIPMENT_CODE);
            }

            //获取状态数据。
            List<string> lstState = new List<string>();
            var queryState = from t in dtData.AsEnumerable()
                             group t by new { t1 = t.Field<String>("EQUIPMENT_FROM_STATE_NAME") } into m
                             select new
                             {
                                 EQUIPMENT_FROM_STATE_NAME = m.First().Field<String>("EQUIPMENT_FROM_STATE_NAME")
                             };
            foreach (var data in queryState)
            {
                lstState.Add(data.EQUIPMENT_FROM_STATE_NAME);
            }
            #region 整理成显示格式的数据
            DataTable dt = new DataTable();
            //增加状态列
            DataColumn dcStatus = new DataColumn("EQUIPMENT_CODE");
            dt.Columns.Add(dcStatus);

            #region 创建动态列
            DataColumn col;
            foreach (string s in lstState)
            {
                if (dt.Columns.Contains(s) == false)
                {
                    col = new DataColumn(s);
                    dt.Columns.Add(col);
                }
            }
            #endregion

            #region //定义行
            DataRow dr0;
            foreach (string s in lstEquipment)
            {
                dr0 = dt.NewRow();
                dr0[0] = s;
                dt.Rows.Add(dr0);
            }
            #endregion

            #region Builder Table //整理日运营数据，月运营数据到 Table

            string strState = "";
            string strEqp = "";
            int indexOfCol = 0;
            int selOfCol = -1;
            int nValue = 0;
            for (int i = 0; i < dtData.Rows.Count; i++)
            {
                strState = dtData.Rows[i]["EQUIPMENT_FROM_STATE_NAME"].ToString();
                strEqp = dtData.Rows[i]["EQUIPMENT_CODE"].ToString();


                selOfCol = -1;
                for (indexOfCol = 0; indexOfCol < dt.Columns.Count; indexOfCol++)
                {
                    if (dt.Columns[indexOfCol].ColumnName == strState)
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
                    if (dt.Rows[j]["EQUIPMENT_CODE"].ToString() == strEqp)
                    {
                        try
                        {
                            nValue = Convert.ToInt32(dtData.Rows[i]["TotalMinutes"]);
                        }
                        catch
                        {
                            nValue = 0;
                        }
                        dt.Rows[j][selOfCol] = nValue;
                    }
                }
            }



            #endregion

            #region //格式化汇总列
            int nCellValue = 0;
            int nIndexRowOfT1 = -1;
            for (int j = 1; j < dt.Columns.Count; j++)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    if (lstEquipment.Contains(dt.Rows[i][0].ToString()))
                    {
                        try
                        {
                            nCellValue = Convert.ToInt32(dt.Rows[i][j] == DBNull.Value ? 0 : dt.Rows[i][j] == null ? 0 : dt.Rows[i][j]);
                        }
                        catch
                        {
                            nCellValue = 0;
                            dt.Rows[i][j] = 0;
                        }

                    }
                }
            }

            #endregion

            #region //Builder 一张汇率表
            DataTable dtRate = dt.Copy();
            nCellValue = 0;
            nIndexRowOfT1 = -1;

            for (int j = 1; j < dt.Columns.Count; j++)
            {
                float dRate = 0;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (i != nIndexRowOfT1)
                    {
                        try
                        {
                            nCellValue = Convert.ToInt32(dt.Rows[i][j] == DBNull.Value ? 0 : dt.Rows[i][j] == null ? 0 : dt.Rows[i][j]);
                        }
                        catch
                        {
                            nCellValue = 0;
                        }
                        dRate = (float)nCellValue / (float)(24 * 60);
                        dtRate.Rows[i][j] = Math.Round(dRate, 4) * 100 + "%";
                    }
                }
            }
            #endregion
            #endregion
            //缓存数据。
            //ViewBag.ListData = dt;
            ViewBag.ListDataRate = dtRate;
            if (Request.IsAjaxRequest())
            {
                return PartialView("_ListPartial", model);
            }
            else
            {
                return View(model);
            }
        }
    }
}