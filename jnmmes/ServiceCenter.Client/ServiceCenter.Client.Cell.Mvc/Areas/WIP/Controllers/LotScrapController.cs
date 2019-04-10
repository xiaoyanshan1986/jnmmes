using ServiceCenter.Client.Mvc.Areas.WIP.Models;
using ServiceCenter.MES.Model.BaseData;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Model.LSM;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Service.Client.BaseData;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Service.Client.LSM;
using ServiceCenter.MES.Service.Client.PPM;
using ServiceCenter.MES.Service.Client.WIP;
using ServiceCenter.MES.Service.Contract.WIP;
using WIPResources = ServiceCenter.Client.Mvc.Resources.WIP;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using ServiceCenter.Client.Mvc.Resources;
using System.IO;
using System.Text;

namespace ServiceCenter.Client.Mvc.Areas.WIP.Controllers
{
    public class LotScrapController : Controller
    {
        //
        // GET: /WIP/LotScrap/
        public ActionResult Index()
        {
            return View(new LotScrapViewModel());
        }
        //
        // POST: /WIP/LotScrap/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(LotScrapViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                ScrapParameter p = new ScrapParameter()
                {
                    Creator = User.Identity.Name,
                    OperateComputer = Request.UserHostAddress,
                    Operator = User.Identity.Name,
                    ReasonCodes=new Dictionary<string,IList<ScrapReasonCodeParameter>>(),
                    Remark = model.Description,
                    LotNumbers = new List<string>()
                };
                //获取批值。
                string lotNumber = model.LotNumber.ToUpper();
                result = GetLot(lotNumber);
                if (result.Code > 0)
                {
                    return Json(result);
                }
                p.LotNumbers.Add(lotNumber);

                //组织报废原因代码
                if(!p.ReasonCodes.ContainsKey(lotNumber))
                {
                    p.ReasonCodes.Add(lotNumber, new List<ScrapReasonCodeParameter>());
                }

                p.ReasonCodes[lotNumber].Add(new ScrapReasonCodeParameter()
                {
                    Description=model.ReasonDescription,
                    Quantity=model.ScrapQuantity,
                    ReasonCodeCategoryName=model.ReasonCodeCategoryName,
                    ReasonCodeName=model.ReasonCodeName,
                    ResponsiblePerson=model.ResponsiblePerson,
                    RouteOperationName=model.RouteOperationName,
                });

                //批次报废操作。
                using (LotScrapServiceClient client = new LotScrapServiceClient())
                {
                    result = client.Scrap(p);
                }
                if (result.Code == 0)
                {
                    result.Message = string.Format("批次 {0} 报废操作成功。",model.LotNumber);
                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }
            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return Json(result);
        }

        public MethodReturnResult GetLot(string lotNumber)
        {
            MethodReturnResult result = new MethodReturnResult();
            MethodReturnResult<Lot> rst = null;
            Lot obj = null;
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                rst = client.Get(lotNumber);
                if (rst.Code <= 0 && rst.Data != null)
                {
                    obj = rst.Data;
                }
                else
                {
                    result.Code = rst.Code;
                    result.Message = rst.Message;
                    result.Detail = rst.Detail;
                    return result;
                }
            }
            if (obj == null || obj.Status == EnumObjectStatus.Disabled)
            {
                result.Code = 2001;
                result.Message = string.Format(WIPResources.StringResource.LotIsNotExists, lotNumber);
                return result;
            }
            else if (obj.StateFlag == EnumLotState.Finished)
            {
                result.Code = 2002;
                result.Message = string.Format("批次({0})已完成。", lotNumber);
                return result;
            }
            else if (obj.Status == EnumObjectStatus.Disabled || obj.DeletedFlag == true)
            {
                result.Code = 2003;
                result.Message = string.Format("批次({0})已结束。", lotNumber);
                return result;
            }
            else if (obj.HoldFlag == true)
            {
                result.Code = 2004;
                result.Message = string.Format("批次({0})已暂停。", lotNumber);
                return result;
            }
            return rst;
        }

        public ActionResult GetLotInfo(string lotNumber)
        {
            MethodReturnResult result = GetLot(lotNumber);
            if (result.Code > 0)
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json(result as MethodReturnResult<Lot>, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetReasonCodeCategoryName(string routeName,string routeStepName)
        {
            using(RouteStepServiceClient client=new RouteStepServiceClient())
            {
                MethodReturnResult<RouteStep> result = client.Get(new RouteStepKey()
                {
                    RouteName=routeName,
                    RouteStepName=routeStepName
                });
                if (result.Code <= 0 && result.Data != null)
                {
                    return Json(new { ReasonCodeCategoryName=result.Data.ScrapReasonCodeCategoryName }
                                , JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new {  ReasonCodeCategoryName=string.Empty }
                        , JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetReasonCode(string categoryName)
        {
            //根据暂停代码组获取暂停代码。
            IList<ReasonCodeCategoryDetail> lst = new List<ReasonCodeCategoryDetail>();
            using (ReasonCodeCategoryDetailServiceClient client = new ReasonCodeCategoryDetailServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"Key.ReasonCodeCategoryName ='{0}'"
                                          , categoryName),
                    OrderBy="ItemNo"
                };
                MethodReturnResult<IList<ReasonCodeCategoryDetail>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lst = result.Data;
                }
            }
            return Json(from item in lst
                        select new 
                        {
                            Text=item.Key.ReasonCodeName,
                            Value=item.Key.ReasonCodeName
                        }, JsonRequestBehavior.AllowGet);
        }


        //
        //Get: /WIP/LotScrap/Query
        public ActionResult Query()
        {
            LotScrapQueryViewModel model = new LotScrapQueryViewModel();
            return Query(model);
        }

        //
        //POST: /WIP/LotScrap/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Query(LotScrapQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (LotQueryServiceClient client = new LotQueryServiceClient())
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = "EditTime",
                        Where = GetQueryCondition(model)
                    };
                    MethodReturnResult<IList<LotTransactionScrap>> result = client.GetLotTransactionScrap(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                }
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("_ListPartial");
            }
            else
            {
                return View(model);
            }
        }
        //
        //POST: /WIP/LotScrap/PagingQuery
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
                        MethodReturnResult<IList<LotTransactionScrap>> result = client.GetLotTransactionScrap(ref cfg);
                        if (result.Code == 0)
                        {
                            ViewBag.PagingConfig = cfg;
                            ViewBag.List = result.Data;
                        }
                    });
                }
            }
            return PartialView("_ListPartial");
        }
        //
        //POST: /WIP/LotScrap/ExportToExcel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExportToExcel(LotScrapQueryViewModel model)
        {
            IList<LotTransactionScrap> lst = new List<LotTransactionScrap>();
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        OrderBy = "EditTime",
                        Where = GetQueryCondition(model)
                    };
                    MethodReturnResult<IList<LotTransactionScrap>> result = client.GetLotTransactionScrap(ref cfg);

                    if (result.Code == 0)
                    {
                        lst = result.Data;
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
            ICell cell = null;
            IRow row = null;
            ISheet ws = null;
            for (int j = 0; j < lst.Count; j++)
            {
                if (j % 65535 == 0)
                {
                    ws = wb.CreateSheet();
                    row = ws.CreateRow(0);
                    #region //列名
                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(StringResource.ItemNo);  //项目号

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(WIPResources.StringResource.LotNumber);  //批次号

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("工单号");  //工单号

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("工序");  //工序

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("线别");  //线别

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(WIPResources.StringResource.LotScrapViewModel_ScrapQuantity);  //数量

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(WIPResources.StringResource.ReasonCodeCategoryName);  //原因代码组

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(WIPResources.StringResource.ReasonCodeName);  //原因代码

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(WIPResources.StringResource.ReasonDescription);  //原因描述

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(WIPResources.StringResource.LotScrapViewModel_RouteOperationName);  //责任工序

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(WIPResources.StringResource.LotScrapViewModel_ResponsiblePerson);  //责任人

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(StringResource.Description);  //描述

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("操作时间");  //操作时间

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("操作人");  //操作人
                    #endregion
                    font.Boldweight = 5;
                }
                LotTransactionScrap obj = lst[j];
                LotTransaction transObj = null;
                LotTransactionHistory lotHisObj = null;
                using (LotQueryServiceClient client = new LotQueryServiceClient())
                {
                    MethodReturnResult<LotTransaction> result = client.GetTransaction(obj.Key.TransactionKey);
                    if (result.Code == 0)
                    {
                        transObj = result.Data;
                    }

                    MethodReturnResult<LotTransactionHistory> result1 = client.GetLotTransactionHistory(obj.Key.TransactionKey);
                    if (result1.Code == 0)
                    {
                        lotHisObj = result1.Data;
                    }
                }
                row = ws.CreateRow(j + 1);

                #region //数据
                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(j + 1);  //项目号

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(transObj.LotNumber);  //批次号

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(transObj.OrderNumber);  //工单号

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(transObj.RouteStepName);  //工序

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(lotHisObj != null ? lotHisObj.LineCode : string.Empty);  //线别

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(obj.Quantity);  //数量

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(obj.Key.ReasonCodeCategoryName);  //原因代码组

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(obj.Key.ReasonCodeName);  //原因代码

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(obj.Description);  //原因描述

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(obj.RouteOperationName);  //责任工序

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(obj.ResponsiblePerson);  //责任人

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(obj.Description);  //描述

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(string.Format("{0:yyyy-MM-dd HH:mm:ss}", obj.EditTime));  //操作时间

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(obj.Editor);  //操作人
                #endregion
            }

            MemoryStream ms = new MemoryStream();
            wb.Write(ms);
            ms.Flush();
            ms.Position = 0;
            return File(ms, "application/vnd.ms-excel", "LotScrapData.xls");
        }

        public string GetQueryCondition(LotScrapQueryViewModel model)
        {
            StringBuilder where = new StringBuilder();
            if (model != null)
            {
                where.Append(@" EXISTS ( From LotTransaction as p 
                                         WHERE p.Key=self.Key.TransactionKey 
                                         AND p.UndoFlag='0')");

                if (!string.IsNullOrEmpty(model.OrderNumber))
                {
                    where.AppendFormat(@" {0} EXISTS( From LotTransaction as p 
                                                      WHERE p.Key=self.Key.TransactionKey 
                                                      AND p.OrderNumber='{1}')"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.OrderNumber);
                }

                if (!string.IsNullOrEmpty(model.RouteStepName))
                {
                    where.AppendFormat(@" {0} EXISTS( From LotTransaction as p 
                                                      WHERE p.Key=self.Key.TransactionKey 
                                                      AND p.RouteStepName='{1}')"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.RouteStepName);
                }

                if (!string.IsNullOrEmpty(model.ReasonCodeName))
                {
                    where.AppendFormat(" {0} Key.ReasonCodeName LIKE '{1}%'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.ReasonCodeName);
                }

                if (!string.IsNullOrEmpty(model.RouteOperationName))
                {
                    where.AppendFormat(" {0} RouteOperationName LIKE '{1}%'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.RouteOperationName);
                }

                if (!string.IsNullOrEmpty(model.ResponsiblePerson))
                {
                    where.AppendFormat(" {0} ResponsiblePerson LIKE '{1}%'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.ResponsiblePerson);
                }

                if (model.StartTime != null)
                {
                    where.AppendFormat(" {0} EditTime >= '{1:yyyy-MM-dd HH:mm:ss}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.StartTime);
                }

                if (model.EndTime != null)
                {
                    where.AppendFormat(" {0} EditTime <= '{1:yyyy-MM-dd HH:mm:ss}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.EndTime);
                }

            }
            return where.ToString();
        }
	}
}