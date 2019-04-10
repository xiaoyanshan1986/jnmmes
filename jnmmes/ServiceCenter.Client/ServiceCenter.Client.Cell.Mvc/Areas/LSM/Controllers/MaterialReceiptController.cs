using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using ServiceCenter.Client.Mvc.Areas.LSM.Models;
using ServiceCenter.Client.Mvc.Resources.FMM;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Model.LSM;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Model.RBAC;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Service.Client.LSM;
using ServiceCenter.MES.Service.Client.PPM;
using ServiceCenter.MES.Service.Client.RBAC;
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
                    //var attr1s = Request["Attr1"].Split(splitChar);
                    List<MaterialReceiptDetail> lst=new List<MaterialReceiptDetail>();
                    for (int i = 0; i < ItemNos.Length;i++)
                    {
                        string materialCode = MaterialCodes[i].ToUpper();
                        string materialLot = MaterialLots[i].ToUpper();
                        string lineStoreName = LineStoreNames[i].ToUpper();

                        //根据物料批号获取当前线别仓中的物料
                        using (LineStoreMaterialServiceClient client1 = new LineStoreMaterialServiceClient())
                        {
                            PagingConfig cfg = new PagingConfig()
                            {
                                PageNo = 0,
                                PageSize = 1,
                                Where = string.Format("Key.MaterialLot='{0}' AND Key.OrderNumber='{1}' AND Key.LineStoreName='{2}'"
                                                    , materialLot
                                                    , obj.OrderNumber
                                                    , lineStoreName)
                            };
                            MethodReturnResult<IList<LineStoreMaterialDetail>> rst1 = client1.GetDetail(ref cfg);
                            if (rst1.Code > 0)
                            {
                                return Json(rst1);
                            }
                            if (rst1.Data != null
                                && rst1.Data.Count > 0
                                && rst1.Data[0].Key.MaterialCode != materialCode)
                            {
                                rst.Code = 2002;
                                rst.Message = string.Format("项目号[{0}]的物料批号({1})已经由物料（{2}）领料使用，请更换新的物料批号。"
                                                                , i + 1
                                                                , materialLot
                                                                , rst1.Data[0].Key.MaterialCode);
                                return Json(rst);
                            }
                        }
                        //判断物料批号在本次领料中是否重复。
                        int count = MaterialLots.Count(m => m == materialLot);
                        if (count > 1)
                        {
                            int lastedIndex = MaterialLots.ToList().LastIndexOf(materialLot);
                            rst.Code = 2003;
                            rst.Message = string.Format("项目号[{0}]物料批号({1})与项目号[{2}]的物料批号重复，如是相同物料料号请在同一个项目号中领取。"
                                                            , i + 1
                                                            , materialLot
                                                            , lastedIndex + 1);
                            return Json(rst);
                        }

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
                            //Attr1 = attr1,
                            Editor = User.Identity.Name,
                            Creator = User.Identity.Name
                        });
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

        public ActionResult GetOrderNumber(string q)
        {
            using (WorkOrderServiceClient client = new WorkOrderServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Key LIKE '{0}%' AND CloseType='{1}'"
                                            , q
                                            ,Convert.ToInt32(EnumWorkOrderCloseType.None))
                };

                MethodReturnResult<IList<WorkOrder>> result = client.Get(ref cfg);
                if (result.Code <= 0)
                {
                    return Json(from item in result.Data
                                select new
                                {
                                    @label = item.Key,
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

        public ActionResult GetMaterialCode(string q,string orderNumber,string lineStoreName)
        {
            string routeOperationName = string.Empty;
            using(LineStoreServiceClient client=new LineStoreServiceClient())
            {
                MethodReturnResult<LineStore> result = client.Get(lineStoreName);
                if (result.Code <= 0 && result.Data != null)
                {
                    routeOperationName = result.Data.RouteOperationName;
                }
            }

            IList<WorkOrderBOM> lstBOM = new List<WorkOrderBOM>();
            using (WorkOrderBOMServiceClient client = new WorkOrderBOMServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"MaterialCode LIKE '{0}%' AND Key.OrderNumber='{1}'"
                                            , q
                                            , orderNumber),
                    OrderBy="Key.ItemNo"
                };
                //工作中心为空的可以领到任何线边仓。
                //线边仓对应工序为空的可以领任何料。
                if (!string.IsNullOrEmpty(routeOperationName))
                {
                    cfg.Where += string.Format(" AND (WorkCenter='' OR WorkCenter IS NULL Or WorkCenter='{0}')", routeOperationName);
                }

                MethodReturnResult<IList<WorkOrderBOM>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data!=null)
                {
                    lstBOM = result.Data;
                }
            }

            return Json(from item in lstBOM
                        select new
                        {
                            @label = string.Format("{0}[{1}]", item.MaterialCode,item.Description),
                            @value = item.MaterialCode,
                            @desc=item.Description
                        }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSupplierName(string q)
        {
            IList<Supplier> lstSupplier = new List<Supplier>();
            using (SupplierServiceClient client = new SupplierServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"Key LIKE '{0}%'"
                                            , q),
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
                            @label = item.Key+"-"+item.Name,
                            @value = item.Key,
                            @SupplierName=item.Name
                        }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetMaterialLot(string materialLot, string materialCode,string orderNumber)
        {
            LineStoreMaterialDetail lineStoreMaterialDetail = new LineStoreMaterialDetail();
            using (LineStoreMaterialServiceClient client = new LineStoreMaterialServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageSize=1,
                    Where = string.Format(@"Key.MaterialCode='{0}' 
                                            AND Key.MaterialLot = '{1}'
                                            AND Key.OrderNumber = '{2}'"
                                            , materialCode
                                            , materialLot
                                            , orderNumber),
                    OrderBy = "Key"
                };
                
                MethodReturnResult<IList<LineStoreMaterialDetail>> result = client.GetDetail(ref cfg);
                if (result.Code <= 0 && result.Data != null && result.Data.Count>0)
                {
                    lineStoreMaterialDetail = result.Data[0];
                }
            }

            return Json( new
                        {
                            @SupplierCode = lineStoreMaterialDetail.SupplierCode ?? string.Empty,
                            @SupplierMaterialLot = lineStoreMaterialDetail.SupplierMaterialLot ?? string.Empty,
                            @Attr1 = lineStoreMaterialDetail.Attr1??string.Empty
                        }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetReceiptNo()
        {
            string prefix = string.Format("LMK{0:yyMM}", DateTime.Now);
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
	}
}