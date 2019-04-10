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
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ServiceCenter.Client.Mvc.Resources;
using NPOI.HSSF.UserModel;
using System.IO;

namespace ServiceCenter.Client.Mvc.Areas.RPT.Controllers
{
    public class WIPDisplayController : Controller
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
        //
        // GET: /RPT/WIPDisplay/
        public ActionResult Index(WIPDisplayQueryViewModel model)
        {
            return Query(model);
        }
        //
        // POST: /RPT/WIPDisplay/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Query(WIPDisplayQueryViewModel model)
        {
            DataTable dtWIPData = new DataTable();
            using (WIPDisplayServiceClient client = new WIPDisplayServiceClient())
            {
                MethodReturnResult<DataSet> rst = client.Get(new WIPDisplayGetParameter()
                {
                    LocationName = model.LocationName,
                    MaterialCode = model.MaterialCode,
                    OnlineTime = model.OnlineTime,
                    OrderNumber = model.OrderNumber
                });
                if (rst.Code <= 0 && rst.Data != null && rst.Data.Tables.Count > 0)
                {
                    dtWIPData = rst.Data.Tables[0];
                }
            }

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

            #region 整理成显示格式的数据
            const string SUMMARY_COLUMN_NAME = "合计";
            DataTable dt = new DataTable();
            //增加状态列
            DataColumn dcStatus = new DataColumn("状态");
            dt.Columns.Add(dcStatus);
            DataRow dr0 = dt.NewRow();
            dr0[0] = "暂停";
            dt.Rows.Add(dr0);
            DataRow dr1 = dt.NewRow();
            dr1[0] = "等待";
            dt.Rows.Add(dr1);
            DataRow dr2 = dt.NewRow();
            dr2[0] = "运行";
            dt.Rows.Add(dr2);
            DataRow dr3 = dt.NewRow();
            dr3[0] = "总计";
            dt.Rows.Add(dr3);
            //增加工序列表
            for (int i = 0; i < lstRouteOperation.Count; i++)
            {
                string colName = lstRouteOperation[i].Key;
                DataColumn col = new DataColumn(colName);
                col.DataType = typeof(double);
                dt.Columns.Add(col);
                dr0[col] = "0";
                dr1[col] = "0";
                dr2[col] = "0";
                dr3[col] = "0";
            }
            //增加合计列
            DataColumn dcSum = new DataColumn(SUMMARY_COLUMN_NAME);
            dcSum.DataType = typeof(double);
            dt.Columns.Add(dcSum);

            double sumHold = 0;
            double sumWait = 0;
            double sumRun = 0;
            ///凑数据源的表头
            for (int i = 0; i < dtWIPData.Rows.Count; i++)
            {
                //工序
                string stepName = Convert.ToString(dtWIPData.Rows[i]["ROUTE_STEP_NAME"]);
                //状态
                string strState_flag = Convert.ToString(dtWIPData.Rows[i]["STATE_FLAG"]);
                //数量
                double qty = Convert.ToDouble(dtWIPData.Rows[i]["QTY"]);
                switch (strState_flag)
                {
                    case "暂停":
                        dr0[stepName] = qty.ToString();
                        sumHold += qty;
                        break;
                    case "等待":
                        dr1[stepName] = qty.ToString();
                        sumWait += qty;
                        break;
                    case "运行":
                        dr2[stepName] = qty.ToString();
                        sumRun += qty;
                        break;
                    default:
                        break;
                }
                dr3[stepName] = Convert.ToInt32(dr0[stepName]) + Convert.ToInt32(dr1[stepName]) + Convert.ToInt32(dr2[stepName]);
            }
            dr0[SUMMARY_COLUMN_NAME] = sumHold;
            dr1[SUMMARY_COLUMN_NAME] = sumWait;
            dr2[SUMMARY_COLUMN_NAME] = sumRun;
            dr3[SUMMARY_COLUMN_NAME] = sumWait + sumRun + sumHold;
            #endregion

            string key = Convert.ToString(Session.SessionID);
            string routeOperationKey = string.Format("{0}_RouteOperation", key);
            HttpContext.Cache[key] = dtWIPData;
            HttpContext.Cache[routeOperationKey] = lstRouteOperation;
            ViewBag.ListData = dt;
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
        // GET: /RPT/WIPDisplay/ShowChartImage
        public ActionResult ShowChartImage(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return View();
            }

            string routeOperationKey = string.Format("{0}_RouteOperation", key);
            DataTable dtWIPData = HttpContext.Cache[key] as DataTable;
            IList<RouteOperation> lstRouteOperation = HttpContext.Cache[routeOperationKey] as IList<RouteOperation>;

            if (dtWIPData==null)
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
            DataColumn colWait = new DataColumn("WAIT");
            colWait.Caption = "等待";
            dtChart.Columns.Add(colWait);
            DataColumn colRun = new DataColumn("RUN");
            colRun.Caption = "运行";
            dtChart.Columns.Add(colRun);
            DataColumn colHold = new DataColumn("HOLD");
            colHold.Caption = "暂停";
            dtChart.Columns.Add(colHold);
            for (int i = 0; i < lstRouteOperation.Count; i++)
            {
                DataRow dr = dtChart.NewRow();
                dtChart.Rows.Add(dr);
                dr[0] = lstRouteOperation[i].Key;
                dr[1] = 0;
                dr[2] = 0;
                dr[3] = 0;
                var query = from row in dtWIPData.AsEnumerable()
                            where Convert.ToString(row["ROUTE_STEP_NAME"]) == Convert.ToString(dr[0])
                                  && row["STATE_FLAG"].ToString() == "等待"
                            select new { QTY = Convert.ToDouble(row["QTY"]) };
                if (query.Count() > 0)
                {
                    double qty = query.Sum(p => p.QTY);
                    dr[1] = qty;
                }

                query = from row in dtWIPData.AsEnumerable()
                        where Convert.ToString(row["ROUTE_STEP_NAME"]) == Convert.ToString(dr[0])
                              && row["STATE_FLAG"].ToString() == "运行"
                        select new { QTY = Convert.ToDouble(row["QTY"]) };
                if (query.Count() > 0)
                {
                    double qty = query.Sum(p => p.QTY);
                    dr[2] = qty;
                }

                query = from row in dtWIPData.AsEnumerable()
                        where Convert.ToString(row["ROUTE_STEP_NAME"]) == Convert.ToString(dr[0])
                              && row["STATE_FLAG"].ToString() == "暂停"
                        select new { QTY = Convert.ToDouble(row["QTY"]) };
                if (query.Count() > 0)
                {
                    double qty = query.Sum(p => p.QTY);
                    dr[3] = qty;
                }
            }
            #endregion

            ViewBag.ChartData = dtChart;
            return View();
        }

        //
        // GET: /RPT/WIPDisplay/Detail
        public ActionResult Detail(WIPDisplayDetailQueryViewModel model)
        {
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    OrderBy = "CreateTime DESC",
                    Where = GetQueryCondition(model)
                };
                MethodReturnResult<IList<Lot>> result = client.Get(ref cfg);

                if (result.Code == 0)
                {
                    ViewBag.PagingConfig = cfg;
                    ViewBag.List = result.Data;
                }
            }
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
        //POST: /WIP/WIPDisplay/ExportToExcel
        [HttpPost]
        public async Task<ActionResult> ExportToExcel(WIPDisplayDetailQueryViewModel model)
        {
            IList<Lot> lstLot = new List<Lot>();
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        OrderBy = "CreateTime DESC",
                        Where = GetQueryCondition(model)
                    };
                    MethodReturnResult<IList<Lot>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        lstLot = result.Data;
                    }
                });
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
            for (int j = 0; j < lstLot.Count; j++)
            {
                if (j % 65535 == 0)
                {
                    ws = wb.CreateSheet();
                    IRow row = ws.CreateRow(0);
                    #region //列名
                    ICell cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("序号");  //序号

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("批次号(组件序列号)");  //批次号

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("工单号");  //工单号

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("物料编码");  //产品号

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("数量");  //数量

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("等级");  //等级

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("花色");  //花色

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("线别");  //线别代码

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("设备代码");  //设备代码


                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("车间名称");  //车间名称

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("工艺流程");  //工艺流程

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("工序");  //工序

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("状态");  //状态


                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("批次类型");  //批次类型

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("暂停？");  //暂停标志


                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("包装号");  //包装号

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("返修？");  //返修标志

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("返工？");  //返工标志

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("返工时间");  //返工时间

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("返工操作人");  //返工操作人

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("创建时间");  //创建时间

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("创建人");  //创建人

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("编辑时间");  //编辑时间

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("编辑人");  //编辑人
                    #endregion
                    font.Boldweight = 5;
                }
                Lot obj = lstLot[j];
                IRow rowData = ws.CreateRow(j + 1);

                #region //数据
                ICell cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(j + 1);  //序号

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.Key);  //批次号


                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.OrderNumber);  //工单号

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.MaterialCode);  //产品号

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.Quantity);  //数量

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.Grade);  //等级

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.Color);  //花色

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.LineCode);  //线别代码

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.EquipmentCode);  //设备代码


                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.LocationName);  //车间名称


                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.RouteName);  //工艺流程

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.RouteStepName);  //工步

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.StateFlag.GetDisplayName());  //批次状态

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.LotType.GetDisplayName());  //批次类型

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.HoldFlag ? StringResource.Yes : StringResource.No);  //暂停标志

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.PackageNo);  //包装号

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.RepairFlag);  //返修次数

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.ReworkFlag);  //返工次数

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(string.Format("{0:yyyy-MM-dd HH:mm:ss}", obj.ReworkTime));  //返工时间

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.Reworker);  //返工操作人

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(string.Format("{0:yyyy-MM-dd HH:mm:ss}", obj.CreateTime));  //创建时间

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.Creator);  //创建人

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(string.Format("{0:yyyy-MM-dd HH:mm:ss}", obj.EditTime));  //编辑时间

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.Editor);  //编辑人
                #endregion
            }

            MemoryStream ms = new MemoryStream();
            wb.Write(ms);
            ms.Flush();
            ms.Position = 0;
            return File(ms, "application/vnd.ms-excel", "WIPDetailData.xls");
        }


        public string GetQueryCondition(WIPDisplayDetailQueryViewModel model)
        {
            StringBuilder where = new StringBuilder();
            if (model != null)
            {
                if (!string.IsNullOrEmpty(model.LocationName))
                {
                    where.AppendFormat(" {0} LocationName = '{1}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.LocationName);
                }


                if (!string.IsNullOrEmpty(model.OrderNumber))
                {
                    where.AppendFormat(" {0} OrderNumber LIKE '{1}%'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.OrderNumber);
                }

                if (!string.IsNullOrEmpty(model.MaterialCode))
                {
                    where.AppendFormat(" {0} MaterialCode LIKE '{1}%'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.MaterialCode);
                }

                if (!string.IsNullOrEmpty(model.RouteOperationName) && model.RouteOperationName!="合计")
                {
                    where.AppendFormat(" {0} RouteStepName = '{1}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.RouteOperationName);
                }

                if (model.Status == "等待")
                {
                    where.AppendFormat(" {0} StateFlag = '{1}'  AND HoldFlag = '0'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , Convert.ToInt32(EnumLotState.WaitTrackIn));
                }
                else if (model.Status == "运行")
                {
                    where.AppendFormat(" {0} StateFlag > '{1}' AND StateFlag<'{2}' AND HoldFlag = '0'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , Convert.ToInt32(EnumLotState.WaitTrackIn)
                                        , Convert.ToInt32(EnumLotState.Finished));
                }
                else if (model.Status == "暂停")
                {
                    where.AppendFormat(" {0} HoldFlag = '1'"
                                        , where.Length > 0 ? "AND" : string.Empty);
                }
            }
            return where.ToString();
        }


        //
        //POST: /WIP/WIPDisplay/DetailPagingQuery
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DetailPagingQuery(string where, string orderBy, int? currentPageNo, int? currentPageSize)
        {
            if (ModelState.IsValid)
            {
                int pageNo = currentPageNo ?? 0;
                int pageSize = currentPageSize ?? 20;
                if (Request["PageNo"] != null)
                {
                    pageNo = Convert.ToInt32(Request["PageNo"]);
                }
                if (Request["PageSize"] != null)
                {
                    pageSize = Convert.ToInt32(Request["PageSize"]);
                }

                using (LotQueryServiceClient client = new LotQueryServiceClient())
                {
                    await Task.Run(() =>
                    {
                        PagingConfig cfg = new PagingConfig()
                        {
                            PageNo = pageNo,
                            PageSize = pageSize,
                            Where = where ?? string.Empty,
                            OrderBy = orderBy ?? string.Empty
                        };
                        MethodReturnResult<IList<Lot>> result = client.Get(ref cfg);
                        if (result.Code == 0)
                        {
                            ViewBag.PagingConfig = cfg;
                            ViewBag.List = result.Data;
                        }
                    });
                }
            }
            return PartialView("_DetailListPartial");
        }
    }
}