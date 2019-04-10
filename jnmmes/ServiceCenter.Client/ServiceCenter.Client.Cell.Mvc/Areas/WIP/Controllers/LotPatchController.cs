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
using System.Text;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using ServiceCenter.Client.Mvc.Resources;
using System.IO;

namespace ServiceCenter.Client.Mvc.Areas.WIP.Controllers
{
    public class LotPatchController : Controller
    {

        //
        // GET: /WIP/LotPatch/
        public ActionResult Index()
        {
            return View(new LotPatchViewModel());
        }
        //
        // POST: /WIP/LotPatch/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(LotPatchViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                PatchParameter p = new PatchParameter()
                {
                    Creator = User.Identity.Name,
                    OperateComputer = Request.UserHostAddress,
                    Operator = User.Identity.Name,
                    LineStoreName=model.LineStoreName,
                    RawMaterialCode=model.MaterialCode,
                    RawMaterialLot=model.MaterialLot,
                    ReasonCodes=new Dictionary<string,IList<PatchReasonCodeParameter>>(),
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
                Lot obj = (result as MethodReturnResult<Lot>).Data;

                if (obj.OrderNumber != model.OrderNumber)
                {
                    result.Code = 1001;
                    result.Message = string.Format("批次 {0} 工单号（{1}）不符合指定工单号。"
                                                    , model.LotNumber
                                                    , obj.OrderNumber
                                                    , model.OrderNumber);
                    return Json(result);
                }

                p.LotNumbers.Add(lotNumber);

                //组织补料原因代码
                if(!p.ReasonCodes.ContainsKey(lotNumber))
                {
                    p.ReasonCodes.Add(lotNumber, new List<PatchReasonCodeParameter>());
                }

                p.ReasonCodes[lotNumber].Add(new PatchReasonCodeParameter()
                {
                    Description=model.ReasonDescription,
                    Quantity=model.PatchQuantity,
                    ReasonCodeCategoryName=model.ReasonCodeCategoryName,
                    ReasonCodeName=model.ReasonCodeName,
                    ResponsiblePerson=model.ResponsiblePerson,
                    RouteOperationName=model.RouteOperationName,
                });

                //批次补料操作。
                using (LotPatchServiceClient client = new LotPatchServiceClient())
                {
                    result = client.Patch(p);
                }
                if (result.Code == 0)
                {
                    result.Message = string.Format("批次 {0} 补料操作成功。",model.LotNumber);
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


        public ActionResult GetOrderNumbers(string lineStoreName)
        {
            IList<WorkOrder> lstWorkOrder = new List<WorkOrder>();
            using (WorkOrderServiceClient client = new WorkOrderServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"CloseType='{0}'
                                           AND LocationName=(SELECT p.LocationName 
                                                             FROM LineStore as p
                                                             WHERE p.Key='{1}')"
                                           , Convert.ToInt32(EnumCloseType.None)
                                           , lineStoreName)
                };

                MethodReturnResult<IList<WorkOrder>> result = client.Get(ref cfg);
                if (result.Code <= 0)
                {
                    lstWorkOrder = result.Data;
                }
            }
            return Json(from item in lstWorkOrder
                        select new
                        {
                            Text = item.Key,
                            Value = item.Key
                        }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMaterialCodes(string orderNumber, string lineStoreName)
        {
            //根据工单获取领料记录
            IList<MaterialReceiptDetail> lstMaterialReceipt = new List<MaterialReceiptDetail>();
            using (MaterialReceiptServiceClient client = new MaterialReceiptServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@" LineStoreName ='{0}' 
                                             AND EXISTS(FROM MaterialReceipt as p
                                                        WHERE p.Key=self.Key.ReceiptNo
                                                        AND p.OrderNumber='{1}')
                                             AND (MaterialCode LIKE '11%' OR MaterialCode LIKE '1301%')"  //电池片或硅片"
                                            , lineStoreName
                                            , orderNumber)
                };
                MethodReturnResult<IList<MaterialReceiptDetail>> result = client.GetDetail(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstMaterialReceipt = result.Data;
                }
            }
            //根据物料类型获取物料。
            IList<LineStoreMaterialDetail> lstLineStoreMaterial = new List<LineStoreMaterialDetail>();
            using (LineStoreMaterialServiceClient client = new LineStoreMaterialServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"Key.LineStoreName ='{0}' AND CurrentQty>0"
                                            , lineStoreName)
                };
                MethodReturnResult<IList<LineStoreMaterialDetail>> result = client.GetDetail(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstLineStoreMaterial = result.Data;
                }
            }

            var lnq = from item in lstLineStoreMaterial
                      where lstMaterialReceipt.Any(m => m.MaterialCode == item.Key.MaterialCode)
                      select item.Key.MaterialCode;
            return Json(lnq.Distinct(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMaterialLots(string orderNumber, string lineStoreName, string materialCode)
        {
            //根据工单获取领料记录
            IList<MaterialReceiptDetail> lstMaterialReceipt = new List<MaterialReceiptDetail>();
            using (MaterialReceiptServiceClient client = new MaterialReceiptServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@" LineStoreName ='{0}' 
                                             AND MaterialCode='{2}'
                                             AND EXISTS(FROM MaterialReceipt as p
                                                        WHERE p.Key=self.Key.ReceiptNo
                                                        AND p.OrderNumber='{1}')"
                                            , lineStoreName
                                            , orderNumber
                                            , materialCode)
                };
                MethodReturnResult<IList<MaterialReceiptDetail>> result = client.GetDetail(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstMaterialReceipt = result.Data;
                }
            }
            //根据物料类型获取物料。
            IList<LineStoreMaterialDetail> lstLineStoreMaterial = new List<LineStoreMaterialDetail>();
            using (LineStoreMaterialServiceClient client = new LineStoreMaterialServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"Key.LineStoreName ='{0}' 
                                            AND Key.MaterialCode='{1}'
                                            AND CurrentQty>0"
                                            , lineStoreName
                                            , materialCode)
                };
                MethodReturnResult<IList<LineStoreMaterialDetail>> result = client.GetDetail(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstLineStoreMaterial = result.Data;
                }
            }

            var lnq = from item in lstLineStoreMaterial
                      where lstMaterialReceipt.Any(m => m.MaterialLot == item.Key.MaterialLot)
                      select new
                      {
                          Text = string.Format("{0}[{1}][{2}]", item.Key.MaterialLot, item.CurrentQty, item.Attr1),
                          Value = item.Key.MaterialLot
                      };
            return Json(lnq, JsonRequestBehavior.AllowGet);
        }

        //
        //Get: /WIP/LotPatch/Query
        public ActionResult Query()
        {
            LotPatchQueryViewModel model = new LotPatchQueryViewModel();
            return Query(model);
        }

        //
        //POST: /WIP/LotPatch/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Query(LotPatchQueryViewModel model)
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
                    MethodReturnResult<IList<LotTransactionPatch>> result = client.GetLotTransactionPatch(ref cfg);

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
        //POST: /WIP/LotPatch/PagingQuery
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
                        MethodReturnResult<IList<LotTransactionPatch>> result = client.GetLotTransactionPatch(ref cfg);
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
        //POST: /WIP/LotPatch/ExportToExcel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExportToExcel(LotPatchQueryViewModel model)
        {
            IList<LotTransactionPatch> lst = new List<LotTransactionPatch>();
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
                    MethodReturnResult<IList<LotTransactionPatch>> result = client.GetLotTransactionPatch(ref cfg);

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
                    cell.SetCellValue(WIPResources.StringResource.LotPatchViewModel_OrderNumber);  //工单号

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(WIPResources.StringResource.LotPatchViewModel_LineStoreName);  //线边仓

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(WIPResources.StringResource.LotPatchViewModel_MaterialCode);  //物料编码

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(WIPResources.StringResource.LotPatchViewModel_MaterialLot);  //物料批号

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(WIPResources.StringResource.LotPatchViewModel_PatchQuantity);  //数量

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
                    cell.SetCellValue(WIPResources.StringResource.LotPatchViewModel_RouteOperationName);  //责任工序

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(WIPResources.StringResource.LotPatchViewModel_ResponsiblePerson);  //责任人

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("补料时间");  //补料时间

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("补料操作人");  //补料操作人
                    #endregion
                    font.Boldweight = 5;
                }
                LotTransactionPatch obj = lst[j];
                LotTransaction transObj = null;
                using (LotQueryServiceClient client = new LotQueryServiceClient())
                {
                    MethodReturnResult<LotTransaction> result = client.GetTransaction(obj.Key.TransactionKey);

                    if (result.Code == 0)
                    {
                        transObj = result.Data;
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
                cell.SetCellValue(obj.LineStoreName);  //线边仓

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(obj.MaterialCode);  //物料编码

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(obj.MaterialLot);  //物料批号

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
                cell.SetCellValue(string.Format("{0:yyyy-MM-dd HH:mm:ss}", obj.EditTime));  //补料时间

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(obj.Editor);  //补料操作人
                #endregion
            }

            MemoryStream ms = new MemoryStream();
            wb.Write(ms);
            ms.Flush();
            ms.Position = 0;
            return File(ms, "application/vnd.ms-excel", "LotPatchData.xls");
        }

        public string GetQueryCondition(LotPatchQueryViewModel model)
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

                if (!string.IsNullOrEmpty(model.MaterialLot))
                {
                    where.AppendFormat(" {0} MaterialLot LIKE '{1}%'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.MaterialLot);
                }


                if (model.StartPatchTime != null)
                {
                    where.AppendFormat(" {0} EditTime >= '{1:yyyy-MM-dd HH:mm:ss}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.StartPatchTime);
                }

                if (model.EndPatchTime != null)
                {
                    where.AppendFormat(" {0} EditTime <= '{1:yyyy-MM-dd HH:mm:ss}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.EndPatchTime);
                }

            }
            return where.ToString();
        }


	}
}