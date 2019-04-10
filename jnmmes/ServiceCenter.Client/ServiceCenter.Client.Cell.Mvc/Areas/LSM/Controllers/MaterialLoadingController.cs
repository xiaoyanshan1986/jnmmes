﻿using NPOI.HSSF.UserModel;
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
    public class MaterialLoadingController : Controller
    {
        //
        // GET: /LSM/MaterialLoading/
        public async Task<ActionResult> Index()
        {
            return await Query(new MaterialLoadingQueryViewModel());
        }
        //
        //POST: /LSM/MaterialLoading/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(MaterialLoadingQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (MaterialLoadingServiceClient client = new MaterialLoadingServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            if (!string.IsNullOrEmpty(model.LoadingNo))
                            {
                                where.AppendFormat(" {0} Key LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.LoadingNo);
                            }
                            if (!string.IsNullOrEmpty(model.RouteOperationName))
                            {
                                where.AppendFormat(" {0} RouteOperationName LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.RouteOperationName);
                            }

                            if (!string.IsNullOrEmpty(model.ProductionLineCode))
                            {
                                where.AppendFormat(" {0} ProductionLineCode LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.ProductionLineCode);
                            }

                            if (!string.IsNullOrEmpty(model.EquipmentCode))
                            {
                                where.AppendFormat(" {0} EquipmentCode LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.EquipmentCode);
                            }

                            if (model.StartLoadingTime != null)
                            {
                                where.AppendFormat(" {0} LoadingTime >= '{1:yyyy-MM-dd HH:mm:ss}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.StartLoadingTime);
                            }

                            if (model.EndLoadingTime != null)
                            {
                                where.AppendFormat(" {0} LoadingTime <= '{1:yyyy-MM-dd HH:mm:ss}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.EndLoadingTime);
                            }
                        }

                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "CreateTime Desc",
                            Where = where.ToString()
                        };

                        MethodReturnResult<IList<MaterialLoading>> result = client.Get(ref cfg);

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
                return PartialView("_ListPartial",new MaterialLoadingViewModel());
            }
            else
            {
                return View("Index", new MaterialLoadingQueryViewModel());
            }
        }
        //
        //POST: /LSM/MaterialLoading/PagingQuery
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

                using (MaterialLoadingServiceClient client = new MaterialLoadingServiceClient())
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
                        MethodReturnResult<IList<MaterialLoading>> result = client.Get(ref cfg);
                        if (result.Code == 0)
                        {
                            ViewBag.PagingConfig = cfg;
                            ViewBag.List = result.Data;
                        }
                    });
                }
            }
            return PartialView("_ListPartial", new MaterialLoadingViewModel());
        }
        //
        // GET: /LSM/MaterialLoading/
        public async Task<ActionResult> Detail(MaterialLoadingDetailQueryViewModel model)
        {
            return await DetailQuery(model);
        }
        //
        //POST: /LSM/MaterialLoading/DetailQuery
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DetailQuery(MaterialLoadingDetailQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (MaterialLoadingServiceClient client = new MaterialLoadingServiceClient())
                {
                    await Task.Run(() =>
                    {
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "CreateTime Desc,Key.LoadingKey,Key.ItemNo",
                            Where = GetWhereCondition(model)
                        };
                        MethodReturnResult<IList<MaterialLoadingDetail>> result = client.GetDetail(ref cfg);

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
                return PartialView("_DetailListPartial", new MaterialLoadingDetailViewModel());
            }
            else
            {
                return View("Detail", model);
            }
        }

        public string GetWhereCondition(MaterialLoadingDetailQueryViewModel model)
        {
            StringBuilder where = new StringBuilder();
            StringBuilder whereExists = new StringBuilder();
            bool isUseWhereExists = false;
            if (model != null)
            {
                if (model.IsShowZeroData==true)
                {
                    where.Append(" CurrentQty>0");
                }

                if (!string.IsNullOrEmpty(model.LoadingNo))
                {
                    where.AppendFormat(" {0} Key.LoadingKey = '{1}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.LoadingNo);

                    whereExists.AppendFormat(" {0} p.Key = '{1}'"
                                        , whereExists.Length > 0 ? "AND" : string.Empty
                                        , model.LoadingNo);
                }

                if (!string.IsNullOrEmpty(model.RouteOperationName))
                {
                    whereExists.AppendFormat(" {0} p.RouteOperationName LIKE '{1}%'"
                                            , whereExists.Length > 0 ? "AND" : string.Empty
                                            , model.RouteOperationName);
                    isUseWhereExists = true;
                }

                if (!string.IsNullOrEmpty(model.ProductionLineCode))
                {
                    whereExists.AppendFormat(" {0} p.ProductionLineCode LIKE '{1}%'"
                                            , whereExists.Length > 0 ? "AND" : string.Empty
                                            , model.ProductionLineCode);
                    isUseWhereExists = true;
                }

                if (!string.IsNullOrEmpty(model.EquipmentCode))
                {
                    whereExists.AppendFormat(" {0} p.EquipmentCode LIKE '{1}%'"
                                            , whereExists.Length > 0 ? "AND" : string.Empty
                                            , model.EquipmentCode);
                    isUseWhereExists = true;
                }

                if (model.StartLoadingTime != null)
                {
                    whereExists.AppendFormat(" {0} p.LoadingTime >= '{1:yyyy-MM-dd HH:mm:ss}'"
                                            , whereExists.Length > 0 ? "AND" : string.Empty
                                            , model.StartLoadingTime);
                    isUseWhereExists = true;
                }

                if (model.EndLoadingTime != null)
                {
                    whereExists.AppendFormat(" {0} p.LoadingTime <= '{1:yyyy-MM-dd HH:mm:ss}'"
                                            , whereExists.Length > 0 ? "AND" : string.Empty
                                            , model.EndLoadingTime);
                    isUseWhereExists = true;
                }

                if (!string.IsNullOrEmpty(model.OrderNumber))
                {
                    where.AppendFormat(" {0} OrderNumber = '{1}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.OrderNumber);
                }

                if (!string.IsNullOrEmpty(model.MaterialCode))
                {
                    where.AppendFormat(" {0} MaterialCode = '{1}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.MaterialCode);
                }

                if (!string.IsNullOrEmpty(model.MaterialLot))
                {
                    where.AppendFormat(" {0} MaterialLot = '{1}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.MaterialLot);
                }
            }

            if (isUseWhereExists)
            {
                where.AppendFormat(" {0} EXISTS( From MaterialLoading as p WHERE p.Key=self.Key.LoadingKey AND {1})"
                                   , where.Length > 0 ? "AND" : string.Empty
                                   , whereExists);
            }
            return where.ToString();
        }

        //
        //POST: /WIP/MaterialLoading/ExportToExcel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExportToExcel(MaterialLoadingDetailQueryViewModel model)
        {
            IList<MaterialLoadingDetail> lst = new List<MaterialLoadingDetail>();
            using (MaterialLoadingServiceClient client = new MaterialLoadingServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        OrderBy = "CreateTime Desc,Key.LoadingKey,Key.ItemNo",
                        Where = GetWhereCondition(model)
                    };
                    MethodReturnResult<IList<MaterialLoadingDetail>> result = client.GetDetail(ref cfg);

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
                    cell.SetCellValue(LSMResources.StringResource.MaterialLoadingViewModel_LoadingNo);  //上料号

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialLoadingDetailViewModel_ItemNo);  //项目号

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialLoadingViewModel_RouteOperationName);  //工序

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialLoadingViewModel_ProductionLineCode);  //生产线

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialLoadingViewModel_EquipmentCode);  //设备

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("上料日期");  //上料时间

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialLoadingViewModel_LoadingTime);  //上料时间

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialLoadingViewModel_Operator);  //操作人

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialLoadingDetailViewModel_LineStoreName);  //线边仓

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialLoadingViewModel_OrderNumber);  //工单号

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialLoadingDetailViewModel_MaterialCode);  //物料编码

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("物料名称");  //物料名称

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialLoadingDetailViewModel_MaterialLot);  //物料批号

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialLoadingDetailViewModel_LoadingQty);  //上料数量

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialLoadingDetailViewModel_UnloadingQty);  //下料数量

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialLoadingDetailViewModel_CurrentQty);  //当前数量
                   
                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("编辑人");  //编辑人

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("编辑时间");  //编辑时间
                    #endregion
                    font.Boldweight = 5;
                }

                MaterialLoadingDetail obj = lst[j];
                MaterialLoading objMaterialLoading = model.GetMaterialLoading(obj.Key.LoadingKey);
                Material m = model.GetMaterial(obj.MaterialCode);
                row = ws.CreateRow(j + 1);

                #region //数据
                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(obj.Key.LoadingKey);  //上料号

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(obj.Key.ItemNo);  //项目号

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(objMaterialLoading.RouteOperationName);  //工序

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(objMaterialLoading.ProductionLineCode);  //生产线

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(objMaterialLoading.EquipmentCode);  //设备

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(string.Format("{0:yyyy-MM-dd}", objMaterialLoading.LoadingTime));  //上料日期

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(string.Format("{0:yyyy-MM-dd HH:mm:ss}", objMaterialLoading.LoadingTime));  //上料时间

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(objMaterialLoading.Operator);  //操作人

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(obj.LineStoreName);  //线边仓

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(obj.OrderNumber);  //工单号

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
                cell.SetCellValue(obj.LoadingQty);  //上料数量

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(obj.UnloadingQty);  //下料数量

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(obj.CurrentQty);  //当前数量

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
            return File(ms, "application/vnd.ms-excel", "MaterialLoadingData.xls");
        }

        //
        //POST: /LSM/MaterialLoading/DetailPagingQuery
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

                using (MaterialLoadingServiceClient client = new MaterialLoadingServiceClient())
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
                        MethodReturnResult<IList<MaterialLoadingDetail>> result = client.GetDetail(ref cfg);
                        if (result.Code == 0)
                        {
                            ViewBag.PagingConfig = cfg;
                            ViewBag.List = result.Data;
                        }
                    });
                }
            }
            return PartialView("_DetailListPartial", new MaterialLoadingDetailViewModel());
        }

        //
        // POST: /PPM/MaterialLoading/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(MaterialLoadingViewModel model)
        {
            MethodReturnResult rst = new MethodReturnResult();
            try
            {
                using (MaterialLoadingServiceClient client = new MaterialLoadingServiceClient())
                {
                    MaterialLoading obj = new MaterialLoading()
                    {
                        Key = model.LoadingNo.ToUpper(),
                        RouteOperationName = model.RouteOperationName,
                        ProductionLineCode=model.ProductionLineCode,
                        EquipmentCode=model.EquipmentCode,
                        Operator=model.Operator,
                        LoadingTime = model.LoadingTime,
                        Description = model.Description,
                        Editor = User.Identity.Name,
                        Creator = User.Identity.Name
                    };

                    char splitChar = ',';
                    var ItemNos = Request["ItemNo"].Split(splitChar);
                    var LineStoreNames = Request["LineStoreName"].Split(splitChar);
                    var MaterialCodes = Request["MaterialCode"].Split(splitChar);
                    var MaterialLots = Request["MaterialLot"].Split(splitChar);
                    var Qtys = Request["LoadingQty"].Split(splitChar);

                    List<MaterialLoadingDetail> lst = new List<MaterialLoadingDetail>();
                    for (int i = 0; i < ItemNos.Length; i++)
                    {
                        lst.Add(new MaterialLoadingDetail()
                        {
                            Key = new MaterialLoadingDetailKey()
                            {
                                 LoadingKey = model.LoadingNo,
                                ItemNo = i + 1
                            },
                            LineStoreName = LineStoreNames[i].ToUpper(),
                            OrderNumber=model.OrderNumber,
                            MaterialCode = MaterialCodes[i].ToUpper(),
                            MaterialLot = MaterialLots[i].ToUpper(),
                            LoadingQty = Convert.ToDouble(Qtys[i]),
                            CurrentQty = Convert.ToDouble(Qtys[i]),
                            Editor = User.Identity.Name,
                            Creator = User.Identity.Name
                        });
                    }

                    rst = await client.AddAsync(obj, lst);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(LSMResources.StringResource.MaterialLoading_Save_Success
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

        public ActionResult GetEquipmentCodes(string routeOperationName, string productionLineCode)
        {
            IList<Equipment> lstEquipments = new List<Equipment>();
            //根据生产线和工序获取设备。
            using (EquipmentServiceClient client = new EquipmentServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"LineCode='{0}' 
                                            AND EXISTS(FROM RouteOperationEquipment as p 
                                                       WHERE p.Key.EquipmentCode=self.Key 
                                                       AND p.Key.RouteOperationName='{1}')"
                                            , productionLineCode
                                            , routeOperationName)
                };
                MethodReturnResult<IList<Equipment>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstEquipments = result.Data;
                }
            }

            var lnq = from item in lstEquipments
                      select new
                      {
                          Key = item.Key,
                          Text = item.Key+"-"+item.Name
                      };
            return Json(lnq, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetLineStoreNames(string routeOperationName, string productionLineCode)
        {
            string locationName = string.Empty;
            //获取生产线所在区域
            using (ProductionLineServiceClient client = new ProductionLineServiceClient())
            {
                MethodReturnResult<ProductionLine> result = client.Get(productionLineCode);
                if (result.Code <= 0 && result.Data != null)
                {
                    locationName = result.Data.LocationName;
                }
            }
            //获取区域所在车间。
            using (LocationServiceClient client = new LocationServiceClient())
            {
                MethodReturnResult<Location> result = client.Get(locationName);
                if (result.Code <= 0 && result.Data != null)
                {
                    locationName = result.Data.ParentLocationName;
                }
            }
            IList<LineStore> lstLineStore = new List<LineStore>();
            //根据车间和工序获取线边仓。
            using (LineStoreServiceClient client = new LineStoreServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"LocationName='{0}' 
                                            AND (RouteOperationName IS NULL OR RouteOperationName='' OR RouteOperationName='{1}')
                                            AND Type='{2}'
                                            AND Status='{3}'"
                                            , locationName
                                            , routeOperationName
                                            , Convert.ToInt32(EnumLineStoreType.Material)
                                            , Convert.ToInt32(EnumObjectStatus.Available))
                };

                MethodReturnResult<IList<LineStore>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstLineStore = result.Data;
                }
            }
            //根据用户获取拥有权限的线边仓。
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
                      where lstResource.Any(m=>m.Data==item.Key)
                      select new
                      {
                          Key = item.Key
                      };
            return Json(lnq, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMaterialLot(string materialLot, string orderNumber, string lineStoreName)
        {
            IList<LineStoreMaterialDetail> lstDetail = new List<LineStoreMaterialDetail>();
            using (LineStoreMaterialServiceClient client = new LineStoreMaterialServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"Key.LineStoreName='{0}'
                                            AND Key.MaterialLot LIKE '{1}%'
                                            AND Key.OrderNumber='{2}'
                                            AND CurrentQty>0"
                                            , lineStoreName
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

            IList<Material> lstMaterial = new List<Material>();
            using (MaterialServiceClient client = new MaterialServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@" EXISTS(FROM LineStoreMaterial as p
                                                    WHERE p.Key.MaterialCode=self.Key
                                                    AND p.Key.LineStoreName='{0}')"
                                            , lineStoreName)
                };

                MethodReturnResult<IList<Material>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstMaterial = result.Data;
                }
            }
            var lnq = from item in lstDetail
                      where item.Key.MaterialCode.StartsWith("11") == false
                      select new
                      {
                          @label = string.Format("{0}[{1} {2}]"
                                                  , item.Key.MaterialLot
                                                  , item.Key.MaterialCode
                                                  , (lstMaterial.Where(m => m.Key == item.Key.MaterialCode).SingleOrDefault() != null
                                                   ? lstMaterial.Where(m => m.Key == item.Key.MaterialCode).SingleOrDefault().Name
                                                   : string.Empty)),
                          @value = item.Key.MaterialLot,
                          @materialCode = item.Key.MaterialCode,
                          @qty = item.CurrentQty,
                          @desc = (lstMaterial.Where(m => m.Key == item.Key.MaterialCode).SingleOrDefault() != null
                                                   ? lstMaterial.Where(m => m.Key == item.Key.MaterialCode).SingleOrDefault().Name
                                                   : string.Empty)
                      };
            return Json(lnq, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLoadingNo()
        {
            string prefix = string.Format("MLM{0:yyMM}", DateTime.Now);
            int itemNo = 0;
            using (MaterialLoadingServiceClient client = new MaterialLoadingServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key LIKE '{0}%'", prefix),
                    OrderBy = "Key Desc"
                };
                MethodReturnResult<IList<MaterialLoading>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null && result.Data.Count > 0)
                {
                    string sItemNo = result.Data[0].Key.Replace(prefix, "");
                    int.TryParse(sItemNo, out itemNo);
                }
            }
            return Json(prefix + (itemNo + 1).ToString("000000"), JsonRequestBehavior.AllowGet);
        }

    }
}