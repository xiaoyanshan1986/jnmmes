using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using ServiceCenter.Client.Mvc.Areas.WIP.Models;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Service.Client.WIP;
using ServiceCenter.Model;
using ServiceCenter.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ServiceCenter.Client.Mvc.Resources;
using WIPResources = ServiceCenter.Client.Mvc.Resources.WIP;
using System.IO;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Service.Client.RPT;
using System.Data;

namespace ServiceCenter.Client.Mvc.Areas.WIP.Controllers
{
    public class LotQueryController : Controller
    {
        //
        // GET: /WIP/LotQuery/
        public async Task<ActionResult> Index()
        {
            LotQueryViewModel model = new LotQueryViewModel
            {
                //初始化参数
                //ReportCode = "DAY01",       //报表代码
                //StartDate = System.DateTime.Now.AddDays(1 - System.DateTime.Now.Day).ToString("yyyy-MM-dd"),
                //EndDate = System.DateTime.Now.ToString("yyyy-MM-dd")
            };
            return View(model);
        }

        public async Task<ActionResult> Index_ws()
        {
            LotQueryViewModel model = new LotQueryViewModel
            {
                //初始化参数
                //ReportCode = "DAY01",       //报表代码
                //StartDate = System.DateTime.Now.AddDays(1 - System.DateTime.Now.Day).ToString("yyyy-MM-dd"),
                //EndDate = System.DateTime.Now.ToString("yyyy-MM-dd")
            };
            return View(model);
        }

        //
        //POST: /WIP/LotQuery/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(LotQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (LotQueryServiceClient client = new LotQueryServiceClient())
                {
                    await Task.Run(() =>
                    {
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "CreateTime DESC,Key Desc",
                            Where = GetQueryCondition(model)
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
            if (Request.IsAjaxRequest())
            {
                return PartialView("_ListPartial", new LotViewModel());
            }
            else
            {
                return View("Index", model);
            }
        }

        //
        //POST: /WIP/LotQuery/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> QueryWS(LotQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (LotQueryServiceClient client = new LotQueryServiceClient())
                {
                    await Task.Run(() =>
                    {
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "CreateTime DESC,Key Desc",
                            Where = GetQueryCondition(model)
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
            if (Request.IsAjaxRequest())
            {
                return PartialView("_ListPartial_ws", new LotViewModel());
            }
            else
            {
                return View("Index_ws", model);
            }
        }

        //
        //POST: /WIP/LotQuery/PagingQuery
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PagingQuery(string where, string orderBy, int? currentPageNo, int? currentPageSize)
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
            return PartialView("_ListPartial",new LotViewModel());
        }

        //
        //POST: /WIP/LotQuery/PagingQuery
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PagingQueryWS(string where, string orderBy, int? currentPageNo, int? currentPageSize)
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
            return PartialView("_ListPartial_ws", new LotViewModel());
        }

        //
        //POST: /WIP/LotQuery/ExportToExcel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExportToExcel(LotQueryViewModel model)
        {
            IList<Lot> lstLot = new List<Lot>();
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging=false,
                        OrderBy = "CreateTime DESC,Key Desc",
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
            style.BorderBottom = BorderStyle.Thin;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderTop = BorderStyle.Thin;
            IFont font = wb.CreateFont();
            font.Boldweight = 10;
            style.SetFont(font);

            LotViewModel m = new LotViewModel();
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
                    cell.SetCellValue(StringResource.ItemNo);  //项目号

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(WIPResources.StringResource.LotNumber);  //批次号

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(WIPResources.StringResource.LotViewModel_OriginalOrderNumber);  //原始工单号

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(WIPResources.StringResource.LotViewModel_OrderNumber);  //工单号

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(WIPResources.StringResource.LotViewModel_MaterialCode);  //产品号

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(WIPResources.StringResource.LotViewModel_Quantity);  //数量

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("效率档");  //效率档

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(WIPResources.StringResource.LotViewModel_Grade);  //等级

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(WIPResources.StringResource.LotViewModel_Color);  //花色

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(WIPResources.StringResource.LotViewModel_LineCode);  //线别代码

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(WIPResources.StringResource.LotViewModel_EquipmentCode);  //设备代码


                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(WIPResources.StringResource.LotViewModel_LocationName);  //车间名称

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(WIPResources.StringResource.LotViewModel_RouteEnterpriseName);  //工艺流程组

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(WIPResources.StringResource.LotViewModel_RouteName);  //工艺流程

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(WIPResources.StringResource.LotViewModel_RouteStepName);  //工步

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(WIPResources.StringResource.LotViewModel_StartWaitTime);  //开始等待时间

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(WIPResources.StringResource.LotViewModel_StartProcessTime);  //开始处理时间

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(WIPResources.StringResource.LotViewModel_StateFlag);  //批次状态

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(WIPResources.StringResource.LotViewModel_LotType);  //批次类型

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(WIPResources.StringResource.LotViewModel_HoldFlag);  //暂停标志

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(WIPResources.StringResource.LotViewModel_DeletedFlag);  //结束标志

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(WIPResources.StringResource.LotViewModel_PackageFlag);  //包装标志

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(WIPResources.StringResource.LotViewModel_PackageNo);  //包装号

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(WIPResources.StringResource.LotViewModel_RepairFlag);  //返修标志

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(WIPResources.StringResource.LotViewModel_ReworkFlag);  //返工标志

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(WIPResources.StringResource.LotViewModel_ReworkTime);  //返工时间

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(WIPResources.StringResource.LotViewModel_Reworker);  //返工操作人

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("电池片物料名称");  //电池片物料

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("电池片物料");  //电池片物料

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("电池片批号");  //电池片批号

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("电池片供应商");  //电池片供应商

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(StringResource.CreateTime);  //创建时间

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(StringResource.Creator);  //创建人

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(StringResource.EditTime);  //编辑时间

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(StringResource.Editor);  //编辑人
                    #endregion
                    font.Boldweight = 5;
                }
                Lot obj = lstLot[j];
                IRow rowData = ws.CreateRow(j + 1);

                LotBOM lotBOMObj = m.GetLotCellMaterial(obj.Key);        

                #region //数据
                ICell cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(j + 1);  //项目号

                cellData =rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.Key);  //批次号

                cellData =rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.OriginalOrderNumber);  //原始工单号

                cellData =rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.OrderNumber);  //工单号

                cellData =rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.MaterialCode);  //产品号

                cellData =rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.Quantity);  //数量

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.Attr1);  //效率档

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.Grade);  //等级

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.Color);  //花色

                cellData =rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.LineCode);  //线别代码

                cellData =rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.EquipmentCode);  //设备代码


                cellData =rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.LocationName);  //车间名称

                cellData =rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.RouteEnterpriseName);  //工艺流程组

                cellData =rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.RouteName);  //工艺流程

                cellData =rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.RouteStepName);  //工步

                cellData =rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(string.Format("{0:yyyy-MM-dd HH:mm:ss}",obj.StartWaitTime));  //开始等待时间

                cellData =rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(string.Format("{0:yyyy-MM-dd HH:mm:ss}",obj.StartProcessTime));  //开始处理时间

                cellData =rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.StateFlag.GetDisplayName());  //批次状态

                cellData =rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.LotType.GetDisplayName());  //批次类型

                cellData =rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.HoldFlag ? StringResource.Yes : StringResource.No);  //暂停标志

                cellData =rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.DeletedFlag ? StringResource.Yes : StringResource.No);  //结束标志

                cellData =rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.PackageFlag ? StringResource.Yes : StringResource.No);  //包装标志

                cellData =rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.PackageNo);  //包装号

                cellData =rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.RepairFlag);  //返修次数

                cellData =rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.ReworkFlag);  //返工次数

                cellData =rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(string.Format("{0:yyyy-MM-dd HH:mm:ss}",obj.ReworkTime));  //返工时间

                cellData =rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.Reworker);  //返工操作人

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(lotBOMObj != null ? lotBOMObj.MaterialCode : string.Empty);  //电池片物料编码

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(lotBOMObj != null ? lotBOMObj.MaterialName : string.Empty);  //电池片物料编码

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(lotBOMObj != null ? lotBOMObj.Key.MaterialLot : string.Empty);  //电池片批号

                cellData = rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(lotBOMObj != null ? lotBOMObj.SupplierName : string.Empty);  //供应商名称

                cellData =rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(string.Format("{0:yyyy-MM-dd HH:mm:ss}",obj.CreateTime));  //创建时间

                cellData =rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.Creator);  //创建人

                cellData =rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(string.Format("{0:yyyy-MM-dd HH:mm:ss}",obj.EditTime));  //编辑时间

                cellData =rowData.CreateCell(rowData.Cells.Count);
                cellData.CellStyle = style;
                cellData.SetCellValue(obj.Editor);  //编辑人
                #endregion
            }

            MemoryStream ms = new MemoryStream();
            wb.Write(ms);
            ms.Flush();
            ms.Position = 0;
            return File(ms, "application/vnd.ms-excel", "LotData.xls");
        }

        public string GetQueryCondition(LotQueryViewModel model)
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

                if (!string.IsNullOrEmpty(model.LotNumber1) && !string.IsNullOrEmpty(model.LotNumber))
                {
                    where.AppendFormat(" {0} Key >= '{1}' AND Key<='{2}'"
                                            , where.Length > 0 ? "AND" : string.Empty
                                            , model.LotNumber
                                            , model.LotNumber1);
                }
                else
                {
                    if (!string.IsNullOrEmpty(model.LotNumber))
                    {
                        where.AppendFormat(" {0} Key LIKE '{1}%'"
                                            , where.Length > 0 ? "AND" : string.Empty
                                            , model.LotNumber);
                    }
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

                if (!string.IsNullOrEmpty(model.LineCode))
                {
                    where.AppendFormat(" {0} LineCode = '{1}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.LineCode);
                }

                if (!string.IsNullOrEmpty(model.RouteStepName))
                {
                    where.AppendFormat(" {0} RouteStepName = '{1}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.RouteStepName);
                }

                if (!string.IsNullOrEmpty(model.PackageNo))
                {
                    where.AppendFormat(" {0} PackageNo LIKE '{1}%'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.PackageNo);
                }

                if (model.StateFlag != null)
                {
                    int stateFlag = Convert.ToInt32(model.StateFlag);
                    where.AppendFormat(" {0} StateFlag = '{1}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , stateFlag);
                }

                if (model.HoldFlag != null)
                {
                    where.AppendFormat(" {0} HoldFlag = '{1}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.HoldFlag);
                }

                if (model.DeletedFlag != null)
                {
                    where.AppendFormat(" {0} DeletedFlag = '{1}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.DeletedFlag);
                }

                if (model.StartCreateTime != null)
                {
                    where.AppendFormat(" {0} CreateTime >= '{1:yyyy-MM-dd HH:mm:ss}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.StartCreateTime);
                }

                if (model.EndCreateTime != null)
                {
                    where.AppendFormat(" {0} CreateTime <= '{1:yyyy-MM-dd HH:mm:ss}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.EndCreateTime);
                }
            }
            return where.ToString();
        }

        //
        //POST: /WIP/LotQuery/Detail
        public async Task<ActionResult> Detail(string lotNumber)
        {
            if (ModelState.IsValid)
            {
                using (LotQueryServiceClient client = new LotQueryServiceClient())
                {
                    await Task.Run(() =>
                    {
                        MethodReturnResult<Lot> result = client.Get(lotNumber);

                        if (result.Code == 0)
                        {
                            ViewBag.Lot = result.Data;
                        }
                    });
                }
            }
            if (ViewBag.Lot == null)
            {
                ViewBag.Lot = new Lot();
            }
            return View("Detail", new LotViewModel());
        }

        //
        //POST: /WIP/LotQuery/Detail
        public async Task<ActionResult> DetailWS(string lotNumber)
        {
            if (ModelState.IsValid)
            {
                using (LotQueryServiceClient client = new LotQueryServiceClient())
                {
                    await Task.Run(() =>
                    {
                        MethodReturnResult<Lot> result = client.Get(lotNumber);

                        if (result.Code == 0)
                        {
                            ViewBag.Lot = result.Data;
                        }
                    });
                }
            }
            if (ViewBag.Lot == null)
            {
                ViewBag.Lot = new Lot();
            }
            return View("Detail_ws", new LotViewModel());
        }

        //
        //POST: /WIP/LotQuery/GetLotAttribute
        public async Task<ActionResult> GetLotAttribute(string lotNumber)
        {
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format("Key.LotNumber='{0}'", lotNumber),
                        OrderBy = "EditTime"
                    };
                    MethodReturnResult<IList<LotAttribute>> result = client.GetAttribute(ref cfg);
                    if (result.Code == 0)
                    {
                        ViewBag.AttributeList = result.Data;
                    }
                });
            }
            return PartialView("_AttributeListPartial");
        }

        //
        //POST: /WIP/LotQuery/GetLotAttribute
        public async Task<ActionResult> GetLotAttributeWS(string lotNumber)
        {
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format("Key.LotNumber='{0}'", lotNumber),
                        OrderBy = "EditTime"
                    };
                    MethodReturnResult<IList<LotAttribute>> result = client.GetAttribute(ref cfg);
                    if (result.Code == 0)
                    {
                        ViewBag.AttributeList = result.Data;
                    }
                });
            }
            return PartialView("_AttributeListPartial_ws");
        }

        //
        //POST: /WIP/LotQuery/GetLotTransaction
        public async Task<ActionResult> GetLotTransaction(string lotNumber)
        {
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format("LotNumber='{0}' AND Activity>-1", lotNumber),
                        OrderBy = "CreateTime"
                    };
                    MethodReturnResult<IList<LotTransaction>> result = client.GetTransaction(ref cfg);
                    if (result.Code == 0)
                    {
                        ViewBag.TransactionList = result.Data;
                    }
                });
            }
            return PartialView("_TransactionListPartial");
        }

        //
        //POST: /WIP/LotQuery/GetLotTransaction
        public async Task<ActionResult> GetLotTransactionWS(string lotNumber)
        {
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format("LotNumber='{0}' AND Activity>-1", lotNumber),
                        OrderBy = "CreateTime"
                    };
                    MethodReturnResult<IList<LotTransaction>> result = client.GetTransaction(ref cfg);
                    if (result.Code == 0)
                    {
                        ViewBag.TransactionList = result.Data;
                    }
                });
            }
            return PartialView("_TransactionListPartial_ws");
        }

        //
        //POST: /WIP/LotQuery/GetLotEquipment
        public async Task<ActionResult> GetLotEquipment(string lotNumber)
        {
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format("LotNumber='{0}' AND State>-1", lotNumber),
                        OrderBy = "CreateTime"
                    };
                    MethodReturnResult<IList<LotTransactionEquipment>> result = client.GetLotTransactionEquipment(ref cfg);
                    if (result.Code == 0)
                    {
                        ViewBag.EquipmentList = result.Data;
                    }
                });
            }
            return PartialView("_EquipmentListPartial");
        }

        //
        //POST: /WIP/LotQuery/GetLotEquipment
        public async Task<ActionResult> GetLotEquipmentWS(string lotNumber)
        {
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format("LotNumber='{0}' AND State>-1", lotNumber),
                        OrderBy = "CreateTime"
                    };
                    MethodReturnResult<IList<LotTransactionEquipment>> result = client.GetLotTransactionEquipment(ref cfg);
                    if (result.Code == 0)
                    {
                        ViewBag.EquipmentList = result.Data;
                    }
                });
            }
            return PartialView("_EquipmentListPartial_ws");
        }

        //
        //POST: /WIP/LotQuery/GetLotDefect
        public async Task<ActionResult> GetLotDefect(string lotNumber)
        {
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format(@" EXISTS(FROM LotTransaction as p
                                                        WHERE p.Key=self.Key.TransactionKey
                                                        AND p.LotNumber='{0}'
                                                        AND p.UndoFlag=0
                                                        AND p.Activity='{1}')"
                                                 , lotNumber
                                                 , Convert.ToInt32(EnumLotActivity.Defect)),
                        OrderBy = "EditTime"
                    };
                    MethodReturnResult<IList<LotTransactionDefect>> result = client.GetLotTransactionDefect(ref cfg);
                    if (result.Code == 0)
                    {
                        ViewBag.DefectList = result.Data;
                    }
                });
            }
            return PartialView("_DefectListPartial");
        }

        //
        //POST: /WIP/LotQuery/GetLotDefect
        public async Task<ActionResult> GetLotDefectWS(string lotNumber)
        {
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format(@" EXISTS(FROM LotTransaction as p
                                                        WHERE p.Key=self.Key.TransactionKey
                                                        AND p.LotNumber='{0}'
                                                        AND p.UndoFlag=0
                                                        AND p.Activity='{1}')"
                                                 , lotNumber
                                                 , Convert.ToInt32(EnumLotActivity.Defect)),
                        OrderBy = "EditTime"
                    };
                    MethodReturnResult<IList<LotTransactionDefect>> result = client.GetLotTransactionDefect(ref cfg);
                    if (result.Code == 0)
                    {
                        ViewBag.DefectList = result.Data;
                    }
                });
            }
            return PartialView("_DefectListPartial_ws");
        }

        //
        //POST: /WIP/LotQuery/GetLotScrap
        public async Task<ActionResult> GetLotScrap(string lotNumber)
        {
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format(@" EXISTS(FROM LotTransaction as p
                                                        WHERE p.Key=self.Key.TransactionKey
                                                        AND p.LotNumber='{0}'
                                                        AND p.UndoFlag=0
                                                        AND p.Activity='{1}')"
                                                 , lotNumber
                                                 , Convert.ToInt32(EnumLotActivity.Scrap)),
                        OrderBy = "EditTime"
                    };
                    MethodReturnResult<IList<LotTransactionScrap>> result = client.GetLotTransactionScrap(ref cfg);
                    if (result.Code == 0)
                    {
                        ViewBag.ScrapList = result.Data;
                    }
                });
            }
            return PartialView("_ScrapListPartial");
        }

        //
        //POST: /WIP/LotQuery/GetLotScrap
        public async Task<ActionResult> GetLotScrapWS(string lotNumber)
        {
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format(@" EXISTS(FROM LotTransaction as p
                                                        WHERE p.Key=self.Key.TransactionKey
                                                        AND p.LotNumber='{0}'
                                                        AND p.UndoFlag=0
                                                        AND p.Activity='{1}')"
                                                 , lotNumber
                                                 , Convert.ToInt32(EnumLotActivity.Scrap)),
                        OrderBy = "EditTime"
                    };
                    MethodReturnResult<IList<LotTransactionScrap>> result = client.GetLotTransactionScrap(ref cfg);
                    if (result.Code == 0)
                    {
                        ViewBag.ScrapList = result.Data;
                    }
                });
            }
            return PartialView("_ScrapListPartial_ws");
        }

        //
        //POST: /WIP/LotQuery/GetLotPatch
        public async Task<ActionResult> GetLotPatch(string lotNumber)
        {
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format(@" EXISTS(FROM LotTransaction as p
                                                        WHERE p.Key=self.Key.TransactionKey
                                                        AND p.LotNumber='{0}'
                                                        AND p.UndoFlag=0
                                                        AND p.Activity='{1}')"
                                                 , lotNumber
                                                 , Convert.ToInt32(EnumLotActivity.Scrap)),
                        OrderBy = "EditTime"
                    };
                    MethodReturnResult<IList<LotTransactionPatch>> result = client.GetLotTransactionPatch(ref cfg);
                    if (result.Code == 0)
                    {
                        ViewBag.PatchList = result.Data;
                    }
                });
            }
            return PartialView("_PatchListPartial");
        }

        //
        //POST: /WIP/LotQuery/GetLotPatch
        public async Task<ActionResult> GetLotPatchWS(string lotNumber)
        {
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format(@" EXISTS(FROM LotTransaction as p
                                                        WHERE p.Key=self.Key.TransactionKey
                                                        AND p.LotNumber='{0}'
                                                        AND p.UndoFlag=0
                                                        AND p.Activity='{1}')"
                                                 , lotNumber
                                                 , Convert.ToInt32(EnumLotActivity.Scrap)),
                        OrderBy = "EditTime"
                    };
                    MethodReturnResult<IList<LotTransactionPatch>> result = client.GetLotTransactionPatch(ref cfg);
                    if (result.Code == 0)
                    {
                        ViewBag.PatchList = result.Data;
                    }
                });
            }
            return PartialView("_PatchListPartial_ws");
        }

        //
        //POST: /WIP/LotQuery/GetLotParam
        public async Task<ActionResult> GetLotParam(string lotNumber)
        {
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format(@" EXISTS(FROM LotTransaction as p
                                                        WHERE p.Key=self.Key.TransactionKey
                                                        AND p.LotNumber='{0}'
                                                        AND p.UndoFlag=0)"
                                                 , lotNumber
                                                 , Convert.ToInt32(EnumLotActivity.Scrap)),
                        OrderBy = "EditTime"
                    };
                    MethodReturnResult<IList<LotTransactionParameter>> result = client.GetTransactionParameter(ref cfg);
                    if (result.Code == 0)
                    {
                        ViewBag.ParamList = result.Data;
                    }
                });
            }
            return PartialView("_ParamListPartial");
        }

        //
        //POST: /WIP/LotQuery/GetLotParam
        public async Task<ActionResult> GetLotParamWS(string lotNumber)
        {
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format(@" EXISTS(FROM LotTransaction as p
                                                        WHERE p.Key=self.Key.TransactionKey
                                                        AND p.LotNumber='{0}'
                                                        AND p.UndoFlag=0)"
                                                 , lotNumber
                                                 , Convert.ToInt32(EnumLotActivity.Scrap)),
                        OrderBy = "EditTime"
                    };
                    MethodReturnResult<IList<LotTransactionParameter>> result = client.GetTransactionParameter(ref cfg);
                    if (result.Code == 0)
                    {
                        ViewBag.ParamList = result.Data;
                    }
                });
            }
            return PartialView("_ParamListPartial_ws");
        }

        //
        //POST: /WIP/LotQuery/GetLotMaterial
        public async Task<ActionResult> GetLotMaterial(string lotNumber)
        {
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                await Task.Run(() =>
                {
                    MethodReturnResult<Lot> result = client.Get(lotNumber);

                    if (result.Code == 0)
                    {
                        ViewBag.Lot = result.Data;
                    }
                });
            }


            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format(@" Key.LotNumber='{0}'"
                                                 , lotNumber),
                        OrderBy = "Key.ItemNo"
                    };
                    MethodReturnResult<IList<LotBOM>> result = client.GetLotBOM(ref cfg);
                    if (result.Code == 0)
                    {
                        ViewBag.MaterialList = result.Data;
                    }
                });
            }
            return PartialView("_MaterialListPartial");
        }

        //
        //POST: /WIP/LotQuery/GetLotMaterial
        public async Task<ActionResult> GetLotMaterialWS(string lotNumber)
        {
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                await Task.Run(() =>
                {
                    MethodReturnResult<Lot> result = client.Get(lotNumber);

                    if (result.Code == 0)
                    {
                        ViewBag.Lot = result.Data;
                    }
                });
            }


            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format(@" Key.LotNumber='{0}'"
                                                 , lotNumber),
                        OrderBy = "Key.ItemNo"
                    };
                    MethodReturnResult<IList<LotBOM>> result = client.GetLotBOM(ref cfg);
                    if (result.Code == 0)
                    {
                        ViewBag.MaterialList = result.Data;
                    }
                });
            }
            return PartialView("_MaterialListPartial_ws");
        }

        //
        //POST: /WIP/LotQuery/GetLotJob
        public async Task<ActionResult> GetLotJob(string lotNumber)
        {
            
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format(@" LotNumber='{0}'"
                                                 , lotNumber),
                        OrderBy = "CreateTime"
                    };
                    MethodReturnResult<IList<LotJob>> result = client.GetLotJob(ref cfg);
                    if (result.Code == 0)
                    {
                        ViewBag.JobList = result.Data;
                    }
                });
            }
            return PartialView("_JobListPartial");
        }


        public ActionResult LotIndex()
        {
            return View();
        }
        public async Task<ActionResult> GetLotInformation(LotQueryViewModel model)
        {
            if (model.LotNumber!=null)
            {
                using (WIPMoveServiceClient client = new WIPMoveServiceClient())
                {
                    await Task.Run(() =>
                    {

                        MethodReturnResult<DataSet> result = client.GetLotInformation(model.LotNumber);
                        if (result.Code == 0)
                        {
                            ViewBag.List = result.Data.Tables[0];
                        }
                    });
                }
            }
            return PartialView("_LotListPartial");
        }
	}
}