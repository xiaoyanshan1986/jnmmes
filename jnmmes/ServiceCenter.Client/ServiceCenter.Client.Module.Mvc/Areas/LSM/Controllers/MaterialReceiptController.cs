using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using ServiceCenter.Client.Mvc.Areas.LSM.Models;
using ServiceCenter.Client.Mvc.Resources.FMM;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Model.LSM;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Model.RBAC;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Service.Client.LSM;
using ServiceCenter.MES.Service.Client.PPM;
using ServiceCenter.MES.Service.Client.RBAC;
using ServiceCenter.MES.Service.Client.ZPVM;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using LSMResources = ServiceCenter.Client.Mvc.Resources.LSM;

namespace ServiceCenter.Client.Mvc.Areas.LSM.Controllers
{
    public class MaterialReceiptController : Controller
    {
        //
        // GET: /LSM/MaterialReceipt/
        public async Task<ActionResult> Index()
        {
            return await Query(new MaterialReceiptQueryViewModel());
        }
        //
        //POST: /LSM/MaterialReceipt/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(MaterialReceiptQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (MaterialReceiptServiceClient client = new MaterialReceiptServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            if (!string.IsNullOrEmpty(model.ReceiptNo))
                            {
                                where.AppendFormat(" {0} Key = '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.ReceiptNo);
                            }
                            if (!string.IsNullOrEmpty(model.OrderNumber))
                            {
                                where.AppendFormat(" {0} OrderNumber = '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.OrderNumber);
                            }

                            if (model.StartReceiptDate!=null)
                            {
                                where.AppendFormat(" {0} ReceiptDate >= '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.StartReceiptDate);
                            }

                            if (model.EndReceiptDate != null)
                            {
                                where.AppendFormat(" {0} ReceiptDate <= '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.EndReceiptDate);
                            }
                        }

                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "CreateTime Desc",
                            Where = where.ToString()
                        };

                        MethodReturnResult<IList<MaterialReceipt>> result = client.Get(ref cfg);

                        if (result.Code == 0)
                        {
                            ViewBag.PagingConfig = cfg;
                            ViewBag.List = result.Data;
                        }
                    });
                }
            }
            if(Request.IsAjaxRequest())
            {
                return PartialView("_ListPartial");
            }
            else
            {
                return View("Index");
            }
        }

        public async Task<ActionResult> ParentQuery(MaterialReceiptDetailParentQueryViewModel model)
        {
            LineStoreMaterialDetail lsmd = null;
            MaterialReceiptDetail mrd = null;
            if (ModelState.IsValid&&model.MaterialLot!=null)
            {
                using (LineStoreMaterialServiceClient client = new LineStoreMaterialServiceClient())
                {
                    await Task.Run(() =>
                    {
                        PagingConfig cfg = new PagingConfig()
                        {
                            Where = string.Format("  Key.MaterialLot Like  '{0}'"
                                        , model.MaterialLot)
                        };
                        MethodReturnResult<IList<LineStoreMaterialDetail>> result = client.GetDetail(ref cfg);

                        if (result.Code <= 0 && result.Data != null && result.Data.Count > 0)
                        {
                            lsmd = result.Data[0];
                        }
                    });
                }
                if (lsmd!=null)
                {
                    using (MaterialReceiptServiceClient client = new MaterialReceiptServiceClient())
                    {
                        await Task.Run(() =>
                        {
                            PagingConfig cfg = new PagingConfig()
                            {
                                OrderBy = "CreateTime Desc,Key.ReceiptNo,Key.ItemNo",
                                Where = string.Format("MaterialLot LIKE '{0}'"
                                                , lsmd.Attr2)
                            };
                            MethodReturnResult<IList<MaterialReceiptDetail>> result = client.GetDetail(ref cfg);

                            if (result.Code == 0 && result.Data.Count > 0 && result.Data[0] != null)
                            {
                                ViewBag.lsmd = lsmd;
                                ViewBag.mrd = result.Data[0];
                            }
                        });
                    }
                }
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("_ParentListPartial");
            }
            else
            {
                return View("ParentQueryIndex");
            }
        }
        //
        //POST: /LSM/MaterialReceipt/PagingQuery
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

                using (MaterialReceiptServiceClient client = new MaterialReceiptServiceClient())
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
                        MethodReturnResult<IList<MaterialReceipt>> result = client.Get(ref cfg);
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
        // GET: /LSM/MaterialReceipt/
        public async Task<ActionResult> Detail(MaterialReceiptDetailQueryViewModel model)
        {
            return await DetailQuery(model);
        }
        //
        //POST: /LSM/MaterialReceipt/DetailQuery
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DetailQuery(MaterialReceiptDetailQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (MaterialReceiptServiceClient client = new MaterialReceiptServiceClient())
                {
                    await Task.Run(() =>
                    {
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "CreateTime Desc,Key.ReceiptNo,Key.ItemNo",
                            Where = GetWhereCondition(model)
                        };
                        MethodReturnResult<IList<MaterialReceiptDetail>> result = client.GetDetail(ref cfg);

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
                return PartialView("_DetailListPartial", new MaterialReceiptDetailViewModel());
            }
            else
            {
                return View("Detail", model);
            }
        }

        public string GetWhereCondition(MaterialReceiptDetailQueryViewModel model)
        {
            StringBuilder where = new StringBuilder();
            if (model != null)
            {
                if (!string.IsNullOrEmpty(model.ReceiptNo))
                {
                    where.AppendFormat(" {0} Key.ReceiptNo = '{1}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.ReceiptNo);
                }

                if (!string.IsNullOrEmpty(model.LineStoreName))
                {
                    where.AppendFormat(" {0} LineStoreName = '{1}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.LineStoreName);
                }

                if (!string.IsNullOrEmpty(model.MaterialCode))
                {
                    where.AppendFormat(" {0} MaterialCode LIKE '{1}%'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.MaterialCode);
                }

                if (!string.IsNullOrEmpty(model.MaterialLot))
                {
                    where.AppendFormat(" {0} MaterialLot LIKE '{1}%'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.MaterialLot);
                }

                if (!string.IsNullOrEmpty(model.OrderNumber))
                {
                    where.AppendFormat(@" {0} EXISTS(FROM MaterialReceipt as p
                                                    WHERE p.Key=self.Key.ReceiptNo
                                                    AND p.OrderNumber = '{1}')"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.OrderNumber);
                }

                if (!string.IsNullOrEmpty(model.ReceiptDate))
                {
                    where.AppendFormat(@" {0} EXISTS(FROM MaterialReceipt as p
                                                    WHERE p.Key=self.Key.ReceiptNo
                                                    AND p.ReceiptDate = '{1}')"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.ReceiptDate);
                }
            }
            return where.ToString();
        }
        //
        //POST: /LSM/MaterialReceipt/DetailPagingQuery
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

                using (MaterialReceiptServiceClient client = new MaterialReceiptServiceClient())
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
                        MethodReturnResult<IList<MaterialReceiptDetail>> result = client.GetDetail(ref cfg);
                        if (result.Code == 0)
                        {
                            ViewBag.PagingConfig = cfg;
                            ViewBag.List = result.Data;
                        }
                    });
                }
            }
            return PartialView("_DetailListPartial", new MaterialReceiptDetailViewModel());
        }
        //
        //POST: /WIP/MaterialReceipt/ExportToExcel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExportToExcel(MaterialReceiptDetailQueryViewModel model)
        {
            IList<MaterialReceiptDetail> lst = new List<MaterialReceiptDetail>();
            using (MaterialReceiptServiceClient client = new MaterialReceiptServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging=false,
                        OrderBy = "CreateTime Desc,Key.ReceiptNo,Key.ItemNo",
                        Where = GetWhereCondition(model)
                    };
                    MethodReturnResult<IList<MaterialReceiptDetail>> result = client.GetDetail(ref cfg);

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
            style.BorderBottom = BorderStyle.Thin;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderTop = BorderStyle.Thin;
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
                    cell.SetCellValue(LSMResources.StringResource.MaterialReceiptViewModel_ReceiptNo);  //领料号

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialReceiptViewModel_OrderNumber);  //工单号

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialReceiptViewModel_ReceiptDate);  //领料日期

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialReceiptDetailViewModel_ItemNo);  //项目号

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialReceiptDetailViewModel_LineStoreName);  //线别仓

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialReceiptDetailViewModel_MaterialCode);  //物料编码

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("物料名称");  //物料名称

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialReceiptDetailViewModel_MaterialLot);  //物料批号

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialReceiptDetailViewModel_Qty);  //数量

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialReceiptDetailViewModel_SupplierMaterialLot);  //供应商批号

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialReceiptDetailViewModel_SupplierCode);  //供应商编码

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("供应商名称");  //供应商名称

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("效率档");  //效率档

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("描述");  //描述

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("编辑人");  //编辑人

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("编辑时间");  //编辑时间
                    #endregion
                    font.Boldweight = 5;
                }

                MaterialReceiptDetail obj = lst[j];
                MaterialReceipt mrObj = model.GetMaterialReceipt(obj.Key.ReceiptNo);
                Material m = model.GetMaterial(obj.MaterialCode);
                Supplier s = model.GetSupplier(obj.SupplierCode);
                row = ws.CreateRow(j + 1);

                #region //数据
                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(obj.Key.ReceiptNo);  //领料号

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(mrObj == null ? string.Empty : mrObj.OrderNumber);  //工单号

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(mrObj == null ? string.Empty : string.Format("{0:yyyy-MM-dd}", mrObj.ReceiptDate));  //领料日期

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(obj.Key.ItemNo);  //项目号

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(obj.LineStoreName);  //线别仓

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(obj.MaterialCode);  //物料编码

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(m == null ? string.Empty : m.Name);  //物料名称

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(obj.MaterialLot);  //物料批号

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(obj.Qty);  //数量

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(obj.SupplierMaterialLot);  //供应商批号

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(obj.SupplierCode);  //供应商编码

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(s == null ? string.Empty : s.Name); //供应商名称

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(obj.Attr1);  //效率档

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(obj.Description);  //描述

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(obj.Editor);  //编辑人

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(string.Format("{0:yyyy-MM-dd HH:mm:ss}", obj.EditTime));  //编辑时间
                #endregion
            }

            MemoryStream ms = new MemoryStream();
            wb.Write(ms);
            ms.Flush();
            ms.Position = 0;
            return File(ms, "application/vnd.ms-excel", "MaterialReceiptData.xls");
        }

        //
        // POST: /PPM/MaterialReceipt/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(MaterialReceiptViewModel model)
        {
            MethodReturnResult rst = new MethodReturnResult();
            try
            {
                using (MaterialReceiptServiceClient client = new MaterialReceiptServiceClient())
                {
                    MaterialReceipt obj = new MaterialReceipt()
                    {
                        Key = model.ReceiptNo.ToUpper(),
                        OrderNumber = model.OrderNumber.ToUpper(),
                        ReceiptDate = model.ReceiptDate,
                        Description = model.Description,
                        Editor = User.Identity.Name,
                        Creator = User.Identity.Name
                    };

                    char splitChar = ',';
                    var ItemNos = Request["ItemNo"].Split(splitChar);
                    var LineStoreNames = Request["LineStoreName"].Split(splitChar);
                    var MaterialCodes = Request["MaterialCode"].Split(splitChar);
                    var MaterialLots = Request["MaterialLot"].Split(splitChar);
                    var Qtys = Request["Qty"].Split(splitChar);
                    var SupplierMaterialLots = Request["SupplierMaterialLot"].Split(splitChar);
                    var SupplierCodes = Request["SupplierCode"].Split(splitChar);
                    //var Descriptions = Request["DetailDescription"].Split(splitChar);
                    var attr1s = Request["Attr1"].Split(splitChar);
                    var attr2s = Request["Attr2"].Split(splitChar);

                    List<MaterialReceiptDetail> lst=new List<MaterialReceiptDetail>();
                    for (int i = 0; i < ItemNos.Length;i++)
                    {
                        string materialCode=MaterialCodes[i].ToUpper();
                        string materialLot=MaterialLots[i].ToUpper();
                        string lineStoreName = LineStoreNames[i].ToUpper();
                        string attr1=  attr1s[i];
                        string attr2 = attr2s[i];
                        //领料电池片，必需输入效率档。
                        if (materialCode.StartsWith("11") && string.IsNullOrEmpty(attr1))
                        {
                            rst.Code = 2001;
                            rst.Message = string.Format("项目号[{0}]是电池片({1})领料，必需输入效率档。", i + 1, materialCode);
                            return Json(rst);
                        }

                        if (materialCode.StartsWith("11") && string.IsNullOrEmpty(attr2))
                        {
                            rst.Code = 2001;
                            rst.Message = string.Format("项目号[{0}]是电池片({1})领料，必需输入颜色。", i + 1, materialCode);
                            return Json(rst);
                        }

                        //根据物料批号获取当前线别仓中的物料
                        using (LineStoreMaterialServiceClient client1 = new LineStoreMaterialServiceClient())
                        {
                            PagingConfig cfg=new PagingConfig(){
                                PageNo=0,
                                PageSize=1,
                                Where=string.Format("Key.MaterialLot='{0}' AND Key.OrderNumber='{1}' AND Key.LineStoreName='{2}'"
                                                    ,materialLot
                                                    ,obj.OrderNumber
                                                    ,lineStoreName)
                            };
                            MethodReturnResult<IList<LineStoreMaterialDetail>> rst1 = client1.GetDetail(ref cfg);
                            if (rst1.Code>0)
                            {
                                return Json(rst1);
                            }

                            if (rst1.Data != null 
                                && rst1.Data.Count>0
                                && rst1.Data[0].Key.MaterialCode!=materialCode)
                            {
                                rst.Code = 2002;
                                rst.Message = string.Format("项目号[{0}]的物料批号({1})已经由物料（{2}）领料使用，请更换新的物料批号。"
                                                                , i + 1
                                                                , materialLot
                                                                , rst1.Data[0].Key.MaterialCode);
                                return Json(rst);
                            }
                           
                            if (rst1.Data != null && rst1.Data.Count>0)
                            {
                                //判断批次物料号的颜色及效率档
                                if(materialCode.StartsWith("11")&& (string.Compare(rst1.Data[0].Attr1,attr1)!=0 || string.Compare(rst1.Data[0].Attr2,attr2)!=0))
                                {
                                    rst.Code = 2003;
                                    rst.Message = string.Format("项目号[{0}]的物料批号({1})已经领料.效率及颜色为({2},{3})与当前效率及颜色({4},{5})不符."
                                                                    , i + 1
                                                                    , materialLot
                                                                    , rst1.Data[0].Attr1, rst1.Data[0].Attr2,attr1,attr2);
                                    return Json(rst);
                                }
                            }
                            
                        }
                        //判断物料批号在本次领料中是否重复。
                        int count = MaterialLots.Count(m => m == materialLot);
                        if (count > 1)
                        {
                            int lastedIndex=MaterialLots.ToList().LastIndexOf(materialLot);
                            rst.Code = 2003;
                            rst.Message = string.Format("项目号[{0}]物料批号({1})与项目号[{2}]的物料批号重复，如是相同物料料号请在同一个项目号中领取。"
                                                            , i + 1
                                                            , materialLot
                                                            , lastedIndex+1);
                            return Json(rst);
                        }
                        //添加领料明细到集合中。
                        lst.Add(new MaterialReceiptDetail()
                        {
                            Key = new MaterialReceiptDetailKey()
                            {
                                ReceiptNo = model.ReceiptNo.ToUpper(),
                                ItemNo = i+1
                            },

                            LineStoreName = lineStoreName,
                            MaterialCode = materialCode,
                            MaterialLot = materialLot,
                            Qty = Convert.ToDouble(Qtys[i]),
                            SupplierMaterialLot = SupplierMaterialLots[i].ToUpper(),
                            SupplierCode = SupplierCodes[i].ToUpper(),
                            //Description = Descriptions[i],
                            Attr1 = attr1,
                            Attr2 = attr2,
                            Editor = User.Identity.Name,
                            Creator = User.Identity.Name  
                            
                        });
                        if (Convert.ToDouble(Qtys[i]) == 0)
                        {
                            rst.Code = 1008;
                            rst.Message = string.Format("物料批号({0})领料数量不能为0。"
                                                            , materialLot );
                            return Json(rst);
                        } 
                        
                    }

                  
                    rst = await client.AddAsync(obj,lst);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(LSMResources.StringResource.MaterialReceipt_Save_Success
                                                    , obj.Key);
                    }
                }
            }
            catch (Exception ex)
            {
                rst.Code = 1000;
                rst.Message = ex.Message;
                rst.Detail = ex.ToString();
            }
            return Json(rst);
        }

        public ActionResult GetOrderNumber(string q, string lineCode)
        {
            string where = string.Format(@"Key LIKE '{0}%' AND CloseType = '{1}'
                                            AND OrderState = '{2}'"
                                            , q
                                            , Convert.ToInt32(EnumWorkOrderCloseType.None)
                                            , EnumWorkOrderState.Open.GetHashCode());

            if (lineCode != null && lineCode != "")
            {
                where += string.Format(@" AND EXISTS(FROM Location as loca
                                                     WHERE loca.ParentLocationName = self.LocationName
                                                        AND EXISTS(FROM ProductionLine as line
                                                                   WHERE line.LocationName = loca.Key
                                                                      AND line.Key = '{0}'))",
                                        lineCode);
            }

            //ProductionLine pl = new ProductionLine();
            //Location l = new Location();

            using (WorkOrderServiceClient client = new WorkOrderServiceClient())
            {              
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = where,
                    OrderBy = "EditTime DESC"
                };

                MethodReturnResult<IList<WorkOrder>> result = client.Get(ref cfg);

                if (result.Code <= 0)
                {
                    return Json(from item in result.Data
                                select new
                                {
                                    @label = item.Key + " " + item.Description,
                                    @value = item.Key
                                }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLineStoreNames(string orderNumber)
        {
            string locationName = string.Empty;
            using(WorkOrderServiceClient client=new WorkOrderServiceClient())
            {
                MethodReturnResult<WorkOrder> result = client.Get(orderNumber);
                if (result.Code <= 0 && result.Data!=null)
                {
                    locationName = result.Data.LocationName;
                }
            }

            IList<LineStore> lstLineStore = new List<LineStore>();
            using (LineStoreServiceClient client = new LineStoreServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("LocationName='{0}' AND Type='{1}' AND Status='{2}'"
                                            , locationName
                                            , Convert.ToInt32(EnumLineStoreType.Material)
                                            , Convert.ToInt32(EnumObjectStatus.Available))
                };

                MethodReturnResult<IList<LineStore>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstLineStore = result.Data;
                }
            }

            IList<Resource> lstResource = new List<Resource>();
            using (UserAuthenticateServiceClient client=new UserAuthenticateServiceClient())
            {
                MethodReturnResult<IList<Resource>> result = client.GetResourceList(User.Identity.Name, ResourceType.LineStore);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstResource = result.Data;
                }
            }

            var lnq = from item in lstLineStore
                      where lstResource.Any(m=>m.Data==item.Key)
                      select new
                      {
                          Key = item.Key
                      };
            return Json(lnq, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMaterialCode(string q, string orderNumber, string lineStoreName)
        {
            IList<Material> lstMaterial = new List<Material>();
 
            using (MaterialServiceClient client = new MaterialServiceClient())
            {
//                 Where = string.Format(@"Key LIKE '{0}%' 
//                                            AND EXISTS(FROM WorkOrderBOM as bom
//                                                     WHERE bom.MaterialCode = self.Key
//                                                        AND Key.OrderNumber = '{1}'
//                                                        AND EXISTS(FROM RouteStepParameter as rsparm
//                                                                   WHERE rsparm.MaterialType = bom.MaterialCode
//                                                                      AND EXISTS(FROM RouteStep as rstep
//                                                                                 WHERE rstep.Key.RouteStepName = rsparm.Key.RouteStepName
//                                                                                    AND EXISTS(FROM WorkOrderRoute as woroute
//                                                                                               WHERE woroute.RouteName = rstep.Key.RouteName
//                                                                                                  AND woroute.Key.OrderNumber = '{1}'))))
//                                            OR EXISTS(FROM WorkOrderBOM as bom
//                                                     WHERE bom.ReplaceMaterial = self.Key
//                                                        AND Key.OrderNumber = '{1}')"

                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"Key LIKE '{0}%' 
                                            AND EXISTS(FROM WorkOrderBOM as bom
                                                     WHERE bom.MaterialCode = self.Key
                                                        AND Key.OrderNumber = '{1}'
                                                        AND EXISTS(FROM RouteStepParameter as rsparm
                                                                   WHERE rsparm.MaterialType = bom.MaterialCode
                                                                      AND EXISTS(FROM RouteStep as rstep
                                                                                 WHERE rstep.Key.RouteStepName = rsparm.Key.RouteStepName
                                                                                    AND EXISTS(FROM WorkOrderRoute as woroute
                                                                                               WHERE woroute.RouteName = rstep.Key.RouteName
                                                                                                  AND woroute.Key.OrderNumber = '{1}'))))
                                            OR EXISTS(FROM WorkOrderBOM as bom
                                                     WHERE bom.MaterialCode = self.Key
                                                        AND Key.OrderNumber = '{1}'
                                                        AND EXISTS(FROM RouteStepParameter as rsparm
                                                                   WHERE rsparm.MaterialType = bom.ReplaceMaterial
                                                                      AND EXISTS(FROM RouteStep as rstep
                                                                                 WHERE rstep.Key.RouteStepName = rsparm.Key.RouteStepName
                                                                                    AND EXISTS(FROM WorkOrderRoute as woroute
                                                                                               WHERE woroute.RouteName = rstep.Key.RouteName
                                                                                                  AND woroute.Key.OrderNumber = '{1}'))))"
                                            , q
                                            , orderNumber),
                    OrderBy = "Key"
                };

                MethodReturnResult<IList<Material>> result = client.Get(ref cfg);

                if (result.Code <= 0 && result.Data != null)
                {
                    lstMaterial = result.Data;
                }
            }

            return Json(from item in lstMaterial
                        select new
                        {
                            @label = string.Format("{0} [{1},{2},{3},{4}]", item.Key, item.Name, item.Spec, item.ModelName,item.Description),
                            @value = item.Key,
                            @desc = item.Name
                        }, JsonRequestBehavior.AllowGet);
            
        }

        //public ActionResult GetMaterialCode(string q,string orderNumber,string lineStoreName)
        //{
        //    string routeOperationName = string.Empty;

        //    using(LineStoreServiceClient client=new LineStoreServiceClient())
        //    {
        //        MethodReturnResult<LineStore> result = client.Get(lineStoreName);
        //        if (result.Code <= 0 && result.Data != null)
        //        {
        //            routeOperationName = result.Data.RouteOperationName;
        //        }
        //    }

        //    IList<WorkOrderBOM> lstBOM = new List<WorkOrderBOM>();

        //    using (WorkOrderBOMServiceClient client = new WorkOrderBOMServiceClient())
        //    {
        //        PagingConfig cfg = new PagingConfig()
        //        {
        //            IsPaging = false,
        //            Where = string.Format(@"MaterialCode LIKE '{0}%' AND Key.OrderNumber='{1}'"
        //                                    , q
        //                                    , orderNumber),
        //            OrderBy="Key.ItemNo"
        //        };
        //        //工作中心为空的可以领到任何线边仓。
        //        //线边仓对应工序为空的可以领任何料。
        //        if (!string.IsNullOrEmpty(routeOperationName))
        //        {
        //            cfg.Where += string.Format(" AND (WorkCenter='' OR WorkCenter IS NULL Or WorkCenter='{0}')", routeOperationName);
        //        }

        //        MethodReturnResult<IList<WorkOrderBOM>> result = client.Get(ref cfg);

        //        if (result.Code <= 0 && result.Data!=null)
        //        {
        //            lstBOM = result.Data;
        //        }
        //    }

        //    return Json(from item in lstBOM
        //                select new
        //                {
        //                    @label = string.Format("{0}[{1}]", item.MaterialCode,item.Description),
        //                    @value = item.MaterialCode,
        //                    @desc=item.Description
        //                }, JsonRequestBehavior.AllowGet);
        //}
     
        public ActionResult GetSupplierName(string q, string materialCode)
        {
            IList<Supplier> lstSupplier = new List<Supplier>();
            string sWhere = "";

            if (materialCode == null || materialCode == "")
            {
                sWhere = string.Format(@"Key LIKE '{0}%' 
                                            AND EXISTS(FROM MaterialReceiptDetail as p
                                                       WHERE p.SupplierCode = self.Key
                                                        )"
                                            , q
                                            , materialCode);
            }
            else
            {
                sWhere = string.Format(@"Key LIKE '{0}%' 
                                            AND EXISTS(FROM MaterialReceiptDetail as p
                                                       WHERE p.SupplierCode = self.Key
                                                        AND p.MaterialCode = '{1}')"
                                            , q
                                            , materialCode);
            }

            using (SupplierServiceClient client = new SupplierServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = sWhere,
                    OrderBy = "Key"
                };
                MethodReturnResult<IList<Supplier>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstSupplier = result.Data;
                }
            }

            return Json(from item in lstSupplier
                        select new
                        {
                            @label = item.Key + "   [" + item.Name + "]",
                            @value = item.Key,
                            @SupplierName = item.Name
                        }, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult GetSupplierName(string q)
        //{
        //    IList<Supplier> lstSupplier = new List<Supplier>();

        //    using (SupplierServiceClient client = new SupplierServiceClient())
        //    {
        //        PagingConfig cfg = new PagingConfig()
        //        {
        //            IsPaging = false,
        //            Where = string.Format(@"Key LIKE '{0}%'"
        //                                    , q),
        //            OrderBy = "Key"
        //        };
        //        MethodReturnResult<IList<Supplier>> result = client.Get(ref cfg);
        //        if (result.Code <= 0 && result.Data != null)
        //        {
        //            lstSupplier = result.Data;
        //        }
        //    }

        //    return Json(from item in lstSupplier
        //                select new
        //                {
        //                    @label = item.Key + "-" + item.Name,
        //                    @value = item.Key,
        //                    @SupplierName = item.Name
        //                }, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult GetEfficiency(string q)
        {
            IList<Efficiency> lst = new List<Efficiency>();
            using (EfficiencyServiceClient client = new EfficiencyServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"Name LIKE '{0}%' and IsUsed = 1"
                                            , q),
                    OrderBy = "Name"
                };
                MethodReturnResult<IList<Efficiency>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lst = result.Data;
                }
            }

            var lnq = from item in lst
                      select item.Name;
            
            return Json(from item in lnq.Distinct<string>()
                        select new
                        {
                            @label = item,
                            @value = item
                        }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMaterialLot(string materialLot, string materialCode,string orderNumber)
        {
            LineStoreMaterialDetail lineStoreMaterialDetail = new LineStoreMaterialDetail();
            if (materialLot.StartsWith("JNC") || materialLot.StartsWith("jnc"))
            {
                MaterialReceiptDetail  obj= this.GetBox(materialLot);
                lineStoreMaterialDetail.Attr1 = obj.Attr1;
                lineStoreMaterialDetail.SupplierCode = "000000";
                lineStoreMaterialDetail.LoadingQty = obj.Qty;
                lineStoreMaterialDetail.Attr2 = obj.Attr2;
            }
            else
            {
                using (LineStoreMaterialServiceClient client = new LineStoreMaterialServiceClient())
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        PageSize = 1,
                        Where = string.Format(@"Key.MaterialCode='{0}' 
                                            AND Key.MaterialLot = '{1}'
                                            AND Key.OrderNumber = '{2}'"
                                                , materialCode
                                                , materialLot
                                                , orderNumber),
                        OrderBy = "Key"
                    };

                    MethodReturnResult<IList<LineStoreMaterialDetail>> result = client.GetDetail(ref cfg);
                    if (result.Code <= 0 && result.Data != null && result.Data.Count > 0)
                    {
                        lineStoreMaterialDetail = result.Data[0];
                    }
                }
            }


            return Json( new
                        {
                            @SupplierCode = lineStoreMaterialDetail.SupplierCode ?? string.Empty,
                            @SupplierMaterialLot = lineStoreMaterialDetail.SupplierMaterialLot ?? string.Empty,
                            @Attr1 = lineStoreMaterialDetail.Attr1??string.Empty,
                            @Attr2 = lineStoreMaterialDetail.Attr2 ?? string.Empty,
                            @Qty = lineStoreMaterialDetail.LoadingQty
                        }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetReceiptNo()
        {
            string prefix = string.Format("LMK{0:yyMMdd}", DateTime.Now);
            int itemNo = 0;
            using (MaterialReceiptServiceClient client = new MaterialReceiptServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key LIKE '{0}%'", prefix),
                    OrderBy = "Key Desc"
                };
                MethodReturnResult<IList<MaterialReceipt>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null && result.Data.Count > 0)
                {
                    string sItemNo = result.Data[0].Key.Replace(prefix,"");
                    int.TryParse(sItemNo, out itemNo);
                }
            }
            return Json(prefix+(itemNo+1).ToString("0000"), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 根据电池箱号获取电池箱信息，数量，效率，颜色，供应商
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
         MaterialReceiptDetail GetBox(string q)
        {
            MaterialReceiptDetail obj = new MaterialReceiptDetail ();
            using (MaterialReceiptServiceClient client = new MaterialReceiptServiceClient())
            {
                MethodReturnResult<MaterialReceiptDetail> result = client.GetBoxDetail(q);
                if (result.Code <= 0 && result.Data != null)
                {
                     obj = result.Data;
                }
            }

            return obj;
            //return Json(new
            //{
            //    @SupplierCode = obj.SupplierCode ?? string.Empty,
            //    @SupplierMaterialLot = obj.SupplierMaterialLot ?? string.Empty,
            //    @Attr1 = obj.Attr1 ?? string.Empty
            //}, JsonRequestBehavior.AllowGet);
        }
	}
}