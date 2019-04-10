using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using ServiceCenter.Client.Mvc.Areas.LSM.Models;
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
    public class MaterialReturnController : Controller
    {
        //
        // GET: /LSM/MaterialReturn/
        public async Task<ActionResult> Index()
        {
            return await Query(new MaterialReturnQueryViewModel());
        }
        //
        //POST: /LSM/MaterialReturn/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(MaterialReturnQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (MaterialReturnServiceClient client = new MaterialReturnServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            if (!string.IsNullOrEmpty(model.ReturnNo))
                            {
                                where.AppendFormat(" {0} Key = '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.ReturnNo);
                            }
                            if (!string.IsNullOrEmpty(model.OrderNumber))
                            {
                                where.AppendFormat(" {0} OrderNumber = '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.OrderNumber);
                            }

                            if (model.StartReturnDate != null)
                            {
                                where.AppendFormat(" {0} ReturnDate >= '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.StartReturnDate);
                            }

                            if (model.EndReturnDate != null)
                            {
                                where.AppendFormat(" {0} ReturnDate <= '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.EndReturnDate);
                            }
                        }

                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "CreateTime Desc",
                            Where = where.ToString()
                        };

                        MethodReturnResult<IList<MaterialReturn>> result = client.Get(ref cfg);

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
                return PartialView("_ListPartial");
            }
            else
            {
                return View("Index");
            }
        }
        //
        //POST: /LSM/MaterialReturn/PagingQuery
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

                using (MaterialReturnServiceClient client = new MaterialReturnServiceClient())
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
                        MethodReturnResult<IList<MaterialReturn>> result = client.Get(ref cfg);
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
        // GET: /LSM/MaterialReturn/
        public async Task<ActionResult> Detail(MaterialReturnDetailQueryViewModel model)
        {
            return await DetailQuery(model);
        }
        //
        //POST: /LSM/MaterialReturn/DetailQuery
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DetailQuery(MaterialReturnDetailQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (MaterialReturnServiceClient client = new MaterialReturnServiceClient())
                {
                    await Task.Run(() =>
                    {
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "CreateTime Desc,Key.ReturnNo,Key.ItemNo",
                            Where = GetWhereCondition(model)
                        };
                        MethodReturnResult<IList<MaterialReturnDetail>> result = client.GetDetail(ref cfg);

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
                return PartialView("_DetailListPartial", new MaterialReturnDetailViewModel());
            }
            else
            {
                return View("Detail", model);
            }
        }

        public string GetWhereCondition(MaterialReturnDetailQueryViewModel model)
        {
            StringBuilder where = new StringBuilder();
            if (model != null)
            {
                if (!string.IsNullOrEmpty(model.ReturnNo))
                {
                    where.AppendFormat(" {0} Key.ReturnNo = '{1}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.ReturnNo);
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
                    where.AppendFormat(@" {0} EXISTS(FROM MaterialReturn as p
                                                    WHERE p.Key=self.Key.ReturnNo
                                                    AND p.OrderNumber = '{1}')"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.OrderNumber);
                }

                if (!string.IsNullOrEmpty(model.ReturnDate))
                {
                    where.AppendFormat(@" {0} EXISTS(FROM MaterialReturn as p
                                                    WHERE p.Key=self.Key.ReturnNo
                                                    AND p.ReturnDate = '{1}')"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.ReturnDate);
                }
            }
            return where.ToString();
        }

        //
        //POST: /LSM/MaterialReturn/DetailPagingQuery
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

                using (MaterialReturnServiceClient client = new MaterialReturnServiceClient())
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
                        MethodReturnResult<IList<MaterialReturnDetail>> result = client.GetDetail(ref cfg);
                        if (result.Code == 0)
                        {
                            ViewBag.PagingConfig = cfg;
                            ViewBag.List = result.Data;
                        }
                    });
                }
            }
            return PartialView("_DetailListPartial",new MaterialReturnDetailQueryViewModel());
        }

        //
        //POST: /WIP/MaterialReturn/ExportToExcel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExportToExcel(MaterialReturnDetailQueryViewModel model)
        {
            IList<MaterialReturnDetail> lst = new List<MaterialReturnDetail>();
            using (MaterialReturnServiceClient client = new MaterialReturnServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        OrderBy = "CreateTime Desc,Key.ReturnNo,Key.ItemNo",
                        Where = GetWhereCondition(model)
                    };
                    MethodReturnResult<IList<MaterialReturnDetail>> result = client.GetDetail(ref cfg);

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
                    cell.SetCellValue(LSMResources.StringResource.MaterialReturnViewModel_ReturnNo);  //退料号

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialReturnViewModel_OrderNumber);  //工单号

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialReturnViewModel_ReturnDate);  //领料日期

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialReturnDetailViewModel_ItemNo);  //项目号

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialReturnDetailViewModel_LineStoreName);  //线别仓

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialReturnDetailViewModel_MaterialCode);  //物料编码

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("物料名称");  //物料名称

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialReturnDetailViewModel_MaterialLot);  //物料批号

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialReturnDetailViewModel_Qty);  //数量


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

                MaterialReturnDetail obj = lst[j];
                MaterialReturn mrObj = model.GetMaterialReturn(obj.Key.ReturnNo);
                Material m = model.GetMaterial(obj.MaterialCode);
                row = ws.CreateRow(j + 1);

                #region //数据
                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(obj.Key.ReturnNo);  //领料号

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(mrObj == null ? string.Empty : mrObj.OrderNumber);  //工单号

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(mrObj == null ? string.Empty : string.Format("{0:yyyy-MM-dd}", mrObj.ReturnDate));  //领料日期

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
            return File(ms, "application/vnd.ms-excel", "MaterialReturnData.xls");
        }

        //
        // POST: /PPM/MaterialReturn/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(MaterialReturnViewModel model)
        {
            MethodReturnResult rst = new MethodReturnResult();
            try
            {
                using (MaterialReturnServiceClient client = new MaterialReturnServiceClient())
                {
                    MaterialReturn obj = new MaterialReturn()
                    {
                        Key = model.ReturnNo.ToUpper(),
                        OrderNumber = model.OrderNumber.ToUpper(),
                        ReturnDate = model.ReturnDate,
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
                    var Descriptions = Request["DetailDescription"].Split(splitChar);

                    List<MaterialReturnDetail> lst = new List<MaterialReturnDetail>();
                    for (int i = 0; i < ItemNos.Length; i++)
                    {
                        lst.Add(new MaterialReturnDetail()
                        {
                            Key = new MaterialReturnDetailKey()
                            {
                                ReturnNo = model.ReturnNo,
                                ItemNo = i + 1
                            },
                            LineStoreName = LineStoreNames[i].ToUpper(),
                            MaterialCode = MaterialCodes[i].ToUpper(),
                            MaterialLot = MaterialLots[i].ToUpper(),
                            Qty = Convert.ToDouble(Qtys[i]),
                            Description = Descriptions[i],
                            Editor = User.Identity.Name,
                            Creator = User.Identity.Name
                        });
                    }

                    rst = await client.AddAsync(obj, lst);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(LSMResources.StringResource.MaterialReturn_Save_Success
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
                                            , Convert.ToInt32(EnumWorkOrderCloseType.None))
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
            using (WorkOrderServiceClient client = new WorkOrderServiceClient())
            {
                MethodReturnResult<WorkOrder> result = client.Get(orderNumber);
                if (result.Code <= 0 && result.Data != null)
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
                    Where = string.Format("LocationName='{0}' AND Type='{1}'", locationName, Convert.ToInt32(EnumLineStoreType.Material))
                };

                MethodReturnResult<IList<LineStore>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstLineStore = result.Data;
                }
            }

            IList<Resource> lstResource = new List<Resource>();
            using (UserAuthenticateServiceClient client = new UserAuthenticateServiceClient())
            {
                MethodReturnResult<IList<Resource>> result = client.GetResourceList(User.Identity.Name, ResourceType.LineStore);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstResource = result.Data;
                }
            }

            var lnq = from item in lstLineStore
                      where lstResource.Any(m => m.Data == item.Key)
                      select new
                      {
                          Key = item.Key
                      };
            return Json(lnq, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMaterialCode(string q, string orderNumber, string lineStoreName)
        {
            string routeOperationName = string.Empty;
            using (LineStoreServiceClient client = new LineStoreServiceClient())
            {
                MethodReturnResult<LineStore> result = client.Get(lineStoreName);
                if (result.Code <= 0 && result.Data != null)
                {
                    routeOperationName = result.Data.RouteOperationName;
                }
            }
            //根据工单获取物料编码。
            IList<WorkOrderBOM> lstBOM = new List<WorkOrderBOM>();
            using (WorkOrderBOMServiceClient client = new WorkOrderBOMServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"MaterialCode LIKE '{0}%' AND Key.OrderNumber='{1}'"
                                            , q
                                            , orderNumber),
                    OrderBy = "Key.ItemNo"
                };
                //工作中心为空的可以领到任何线边仓。
                //线边仓对应工序为空的可以领任何料。
                if (!string.IsNullOrEmpty(routeOperationName))
                {
                    cfg.Where += string.Format(" AND (WorkCenter='' OR WorkCenter IS NULL Or WorkCenter='{0}')", routeOperationName);
                }

                MethodReturnResult<IList<WorkOrderBOM>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstBOM = result.Data;
                }
            }
            //获取线边仓中已有物料明细数据。
            IList<LineStoreMaterialDetail> lstDetail=new List<LineStoreMaterialDetail>();
            using(LineStoreMaterialServiceClient client=new LineStoreMaterialServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"Key.MaterialCode LIKE '{0}%' AND Key.LineStoreName='{1}' AND CurrentQty>0"
                                            , q
                                            , lineStoreName)
                };
                MethodReturnResult<IList<LineStoreMaterialDetail>> result = client.GetDetail(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstDetail = result.Data;
                }
            }

            return Json(from item in lstBOM
                        where lstDetail.Any(m=>m.Key.MaterialCode==item.MaterialCode)
                        select new
                        {
                            @label = string.Format("{0}[{1}]", item.MaterialCode, item.Description),
                            @value = item.MaterialCode,
                            @desc = item.Description
                        }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMaterialLot(string materialLot, string materialCode, string orderNumber, string lineStoreName)
        {
            IList<LineStoreMaterialDetail> lstDetail = new List<LineStoreMaterialDetail>();
            using (LineStoreMaterialServiceClient client = new LineStoreMaterialServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging=false,
                    Where = string.Format(@"Key.LineStoreName='{0}'
                                            AND Key.MaterialCode='{1}'
                                            AND Key.OrderNumber='{3}'
                                            AND Key.MaterialLot LIKE '{2}%'
                                            AND CurrentQty>0"
                                            , lineStoreName
                                            , materialCode
                                            , materialLot
                                            , orderNumber),
                    OrderBy = "Key"
                };

                MethodReturnResult<IList<LineStoreMaterialDetail>> result = client.GetDetail(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstDetail = result.Data;
                }
            }

            return Json(from item in lstDetail
                        select new
                        {
                            @label = string.Format("{0}[{1}]",item.Key.MaterialLot,item.Key.MaterialCode),
                            @value = item.Key.MaterialLot,
                            @qty=item.CurrentQty
                        }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetReturnNo()
        {
            string prefix = string.Format("TMK{0:yyMM}", DateTime.Now);
            int itemNo = 0;
            using (MaterialReturnServiceClient client = new MaterialReturnServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key LIKE '{0}%'", prefix),
                    OrderBy = "Key Desc"
                };
                MethodReturnResult<IList<MaterialReturn>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null && result.Data.Count > 0)
                {
                    string sItemNo = result.Data[0].Key.Replace(prefix, "");
                    int.TryParse(sItemNo, out itemNo);
                }
            }
            return Json(prefix + (itemNo + 1).ToString("0000"), JsonRequestBehavior.AllowGet);
        }
    }
}