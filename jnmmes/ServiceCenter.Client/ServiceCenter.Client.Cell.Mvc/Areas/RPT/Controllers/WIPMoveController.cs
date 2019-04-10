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
    public class WIPMoveController : Controller
    {
        public const string ChartTheme = @"<Chart BackColor=""#C9DC87"" BackGradientStyle=""TopBottom"" BorderColor=""181, 64, 1"" BorderWidth=""2"" BorderlineDashStyle=""Solid"" Palette=""BrightPastel"">
  <ChartAreas>
    <ChartArea Name=""Default"" _Template_=""All"" BackColor=""Transparent"" BackSecondaryColor=""White"" BorderColor=""64, 64, 64, 64"" BorderDashStyle=""Solid"" ShadowColor=""Transparent"">
      <AxisY LineColor=""64, 64, 64, 64"">
        <MajorGrid Interval=""Auto"" LineColor=""64, 64, 64, 64"" />
        <LabelStyle Font=""Trebuchet MS, 8.25pt, style=Bold"" />
      </AxisY>
      <AxisX LineColor=""64, 64, 64, 64"" Interval=""1"">
        <MajorGrid LineColor=""64, 64, 64, 64"" />
        <LabelStyle Font=""Trebuchet MS, 8.25pt, style=Bold"" />
      </AxisX>
      <Area3DStyle Inclination=""15"" IsClustered=""False"" IsRightAngleAxes=""False"" Perspective=""10"" Rotation=""10"" WallWidth=""0"" />
    </ChartArea>
  </ChartAreas>
  <Legends>
    <Legend _Template_=""All"" Alignment=""Center"" BackColor=""Transparent"" Docking=""Bottom"" Font=""Trebuchet MS, 8.25pt, style=Bold"" IsTextAutoFit =""False"" LegendStyle=""Row"">
    </Legend>
  </Legends>
  <BorderSkin SkinStyle=""Emboss"" />
</Chart>";

        private EnumLotActivity [] activities = new EnumLotActivity[]
        {
            EnumLotActivity.TrackIn,
            EnumLotActivity.TrackOut,
            EnumLotActivity.Defect,
            EnumLotActivity.Scrap,
            EnumLotActivity.Patch
        };

        Dictionary<string, string> dicColumn = new Dictionary<string, string>()
        {
            {"CUR_DAY","日期"},
            {"SHIFT_NAME","班别"},
            {"ACTIVITY",""},
            {"SUM_VALUE","合计"}
        };
        //
        // GET: /RPT/WIPMove/
        public ActionResult Index(WIPMoveQueryViewModel model)
        {
            return Query(model);
        }
        //
        // POST: /RPT/WIPMove/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Query(WIPMoveQueryViewModel model)
        {
            DataTable dtData = new DataTable();
            //获取工序MOVE数据。
            using (WIPMoveServiceClient client = new WIPMoveServiceClient())
            {
                MethodReturnResult<DataSet> rst = client.Get(new WIPMoveGetParameter()
                {
                    LocationName = model.LocationName,
                    MaterialCode = model.MaterialCode,
                    OrderNumber = model.OrderNumber,
                    ShiftName=model.ShiftName,
                    StartDate=model.StartDate,
                    EndDate=model.EndDate
                });
                if (rst.Code <= 0 && rst.Data != null && rst.Data.Tables.Count > 0)
                {
                    dtData = rst.Data.Tables[0];
                }
            }
            //获取工序数据。
            IList<RouteOperation> lstRouteOperation = new List<RouteOperation>();
            using (RouteOperationServiceClient client = new RouteOperationServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = "Status=1",
                    OrderBy = "SortSeq"
                };
                MethodReturnResult<IList<RouteOperation>> rst = client.Get(ref cfg);
                if (rst.Code <= 0 && rst.Data != null)
                {
                    lstRouteOperation = rst.Data;
                }
            }
            //获取班次数据。
            IList<Shift> lstShift = new List<Shift>();
            using (ShiftServiceClient client = new ShiftServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false
                };
                if (!string.IsNullOrEmpty(model.ShiftName))
                {
                    cfg.Where = string.Format("Key='{0}'", model.ShiftName);
                }
                MethodReturnResult<IList<Shift>> rst = client.Get(ref cfg);
                if (rst.Code <= 0)
                {
                    lstShift = rst.Data;
                }
            }
            string [] columns = new string[] { "CUR_DAY", "SHIFT_NAME", "ACTIVITY" };
            //组织新表数据结构
            DataTable dtNew = new DataTable();
            for (int i = 0; i < columns.Length; i++)
            {
                DataColumn dc = dtNew.Columns.Add(columns[i]);
                dc.Caption = dicColumn[dc.ColumnName];
            }
            //添加工序列
            for (int i = 0; i < lstRouteOperation.Count; i++)
            {
                string colName = lstRouteOperation[i].Key;
                DataColumn col = new DataColumn(colName);
                col.DataType = typeof(double);
                dtNew.Columns.Add(col);
            }
            //添加合计列
            DataColumn dcSum= dtNew.Columns.Add("SUM_VALUE");
            dcSum.DataType = typeof(double);
            dcSum.Caption = dicColumn[dcSum.ColumnName];
            //填充数据
            //添加合计行。
            foreach (Shift shift in lstShift)
            {
                for (int i = 0; i < activities.Length; i++)
                {
                    DataRow dr = dtNew.NewRow();
                    dr["CUR_DAY"] = "合计";
                    dr["SHIFT_NAME"] = shift.Key;
                    dr["ACTIVITY"] = Convert.ToInt32(activities[i]);
                    dtNew.Rows.Add(dr);
                }
            }
            //添加日期行。
            for (DateTime start = model.StartDate; start <= model.EndDate; start = start.AddDays(1))
            {
                foreach (Shift shift in lstShift)
                {
                    for (int i = 0; i < activities.Length; i++)
                    {
                        DataRow dr = dtNew.NewRow();
                        dr["CUR_DAY"] = start;
                        dr["SHIFT_NAME"] = shift.Key;
                        dr["ACTIVITY"] = Convert.ToInt32(activities[i]);
                        dtNew.Rows.Add(dr);
                    }
                }
            }

            for (int i = 0; i < dtNew.Rows.Count; i++)
            {
                string curDay = Convert.ToString(dtNew.Rows[i]["CUR_DAY"]);
                string shiftName = Convert.ToString(dtNew.Rows[i]["SHIFT_NAME"]);
                int activity = Convert.ToInt32(dtNew.Rows[i]["ACTIVITY"]);
                double sumQty = 0;
                for (int j = 0; j < lstRouteOperation.Count; j++)
                {
                    string colName = lstRouteOperation[j].Key;
                    double qty = 0;
                    if (curDay == "合计")
                    {
                        var lnq = from row in dtData.AsEnumerable()
                                  where Convert.ToString(row["SHIFT_NAME"]) == shiftName
                                        && Convert.ToInt32(row["ACTIVITY"]) == activity
                                        && Convert.ToString(row["ROUTE_STEP_NAME"]) == colName
                                  select Convert.ToDouble(row["QUANTITY"]);
                        qty = lnq.Sum();
                    }
                    else
                    {
                        var lnq = from row in dtData.AsEnumerable()
                                  where Convert.ToString(row["CUR_DAY"]) == curDay
                                        && Convert.ToString(row["SHIFT_NAME"]) == shiftName
                                        && Convert.ToInt32(row["ACTIVITY"]) == activity
                                        && Convert.ToString(row["ROUTE_STEP_NAME"]) == colName
                                  select Convert.ToDouble(row["QUANTITY"]);
                        qty = lnq.Sum();
                    }
                    dtNew.Rows[i][colName] = qty;
                    sumQty+=qty;
                }
                dtNew.Rows[i]["SUM_VALUE"] = sumQty;
            }
            //缓存数据。
            string key = Convert.ToString(Session.SessionID);
            string routeOperationKey = string.Format("{0}_RouteOperation", key);
            HttpContext.Cache[key] = dtData;
            HttpContext.Cache[routeOperationKey] = lstRouteOperation;
            ViewBag.ListData = dtNew;
            ViewBag.Key = key;
            if (Request.IsAjaxRequest())
            {
                return PartialView("_ListPartial", model);
            }
            else
            {
                return View(model);
            }
        }
        //
        // GET: /RPT/WIPMove/ShowChartImage
        public ActionResult ShowChartImage(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return View();
            }

            string routeOperationKey = string.Format("{0}_RouteOperation", key);
            DataTable dtData = HttpContext.Cache[key] as DataTable;
            IList<RouteOperation> lstRouteOperation = HttpContext.Cache[routeOperationKey] as IList<RouteOperation>;

            if (dtData == null)
            {
                return View();
            }

            if (lstRouteOperation == null)
            {
                using (RouteOperationServiceClient client = new RouteOperationServiceClient())
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = "Status=1",
                        OrderBy = "SortSeq"
                    };
                    MethodReturnResult<IList<RouteOperation>> rst = client.Get(ref cfg);
                    if (rst.Code <= 0 && rst.Data != null)
                    {
                        lstRouteOperation = rst.Data;
                    }
                }
            }

            #region  把原始数据绑定到chart上
            DataTable dtChart = new DataTable();
            DataColumn colOpeartion = new DataColumn("STEP_NAME");
            dtChart.Columns.Add(colOpeartion);

            for (int i = 0; i < 2; i++)
            {
                DataColumn dc = new DataColumn(activities[i].ToString());
                dc.Caption = activities[i].GetDisplayName();
                dtChart.Columns.Add(dc);
            }

            for (int i = 0; i < lstRouteOperation.Count; i++)
            {
                DataRow dr = dtChart.NewRow();
                dtChart.Rows.Add(dr);
                dr[0] = lstRouteOperation[i].Key;
                for (int j = 0; j < 2; j++)
                {
                    string colName = activities[j].ToString();
                    var query = from row in dtData.AsEnumerable()
                                where Convert.ToString(row["ROUTE_STEP_NAME"]) == Convert.ToString(dr[0])
                                      && Convert.ToInt32(row["ACTIVITY"]) == Convert.ToInt32(activities[j])
                                select Convert.ToDouble(row["QUANTITY"]);
                    var qty=query.Sum();
                    dr[colName] = qty;
                }
            }
            #endregion

            ViewBag.ChartData = dtChart;
            return View();
        }

        //
        // GET: /RPT/WIPMove/Detail
        public ActionResult Detail(WIPMoveDetailQueryViewModel model)
        {
            WIPMoveDetailGetParameter p = GetQueryCondition(model);
            //获取工序MOVE明细数据。
            using (WIPMoveServiceClient client = new WIPMoveServiceClient())
            {
                MethodReturnResult<DataSet> rst = client.GetDetail(ref p);
                if (rst.Code <= 0 && rst.Data != null && rst.Data.Tables.Count > 0)
                {
                    ViewBag.List = rst.Data.Tables[0];
                    ViewBag.PagingConfig = new PagingConfig()
                    {
                        PageNo = model.PageNo,
                        PageSize = model.PageSize,
                        Records = p.TotalRecords
                    };
                }
            }
            model.TotalRecords = p.TotalRecords;
            if (Request.IsAjaxRequest())
            {
                return PartialView("_DetailListPartial");
            }
            else
            {
                return View(model);
            }
        }
        //
        //POST: /WIP/WIPMove/ExportToExcel
        [HttpPost]
        public ActionResult ExportToExcel(WIPMoveDetailQueryViewModel model)
        {
            WIPMoveDetailGetParameter p = GetQueryCondition(model);
            p.PageSize = model.TotalRecords;
            DataTable dtData = new DataTable();
            using (WIPMoveServiceClient client = new WIPMoveServiceClient())
            {
                MethodReturnResult<DataSet> rst = client.GetDetail(ref p);
                if (rst.Code <= 0 && rst.Data != null && rst.Data.Tables.Count > 0)
                {
                    dtData = rst.Data.Tables[0];
                }
            }
            //创建工作薄。
            IWorkbook wb = new HSSFWorkbook();
            //设置EXCEL格式
            ICellStyle style = wb.CreateCellStyle();
            style.FillForegroundColor = 10;
            //有边框
            style.BorderBottom = BorderStyle.THIN;
            style.BorderLeft = BorderStyle.THIN;
            style.BorderRight = BorderStyle.THIN;
            style.BorderTop = BorderStyle.THIN;
            IFont font = wb.CreateFont();
            font.Boldweight = 10;
            style.SetFont(font);

            ISheet ws = null;
            for (int j = 0; j < dtData.Rows.Count; j++)
            {
                if (j % 65535 == 0)
                {
                    ws = wb.CreateSheet();
                    IRow row = ws.CreateRow(0);
                    #region //列名
                    ICell cell = null;
                    foreach (DataColumn dc in dtData.Columns)
                    {
                        cell = row.CreateCell(row.Cells.Count);
                        cell.CellStyle = style;
                        cell.SetCellValue(dc.Caption);
                    }
                    #endregion
                    font.Boldweight = 5;
                }

                IRow rowData = ws.CreateRow(j + 1);

                #region //数据
                ICell cellData = null;
                foreach (DataColumn dc in dtData.Columns)
                {
                    cellData = rowData.CreateCell(rowData.Cells.Count);
                    cellData.CellStyle = style;
                    if(dc.DataType==typeof(double) || dc.DataType==typeof(float))
                    {
                        cellData.SetCellValue(Convert.ToDouble(dtData.Rows[j][dc]));
                    }
                    else if (dc.DataType == typeof(int))
                    {
                        cellData.SetCellValue(Convert.ToInt32(dtData.Rows[j][dc]));
                    }
                    else
                    {
                        cellData.SetCellValue(Convert.ToString(dtData.Rows[j][dc]));
                    }
                }
                #endregion
            }
            MemoryStream ms = new MemoryStream();
            wb.Write(ms);
            ms.Flush();
            ms.Position = 0;
            return File(ms, "application/vnd.ms-excel", "WIPMoveDetailData.xls");
        }


        public WIPMoveDetailGetParameter GetQueryCondition(WIPMoveDetailQueryViewModel model)
        {
            WIPMoveDetailGetParameter p = new WIPMoveDetailGetParameter()
            {
                LocationName = model.LocationName,
                MaterialCode = model.MaterialCode,
                OrderNumber = model.OrderNumber,
                ShiftName = model.ShiftName,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Activity=-1000,
                RouteOperationName=string.Empty,
                PageNo=model.PageNo,
                PageSize=model.PageSize
            };

            if (model.Date != "合计")
            {
                p.StartDate = Convert.ToDateTime(model.Date);
                p.EndDate = Convert.ToDateTime(model.Date);
            }
            if (model.RouteOperationName != "合计")
            {
                p.RouteOperationName = model.RouteOperationName;
            }
            p.Activity = Convert.ToInt32(model.Activity);
            return p;
        }

    }
}