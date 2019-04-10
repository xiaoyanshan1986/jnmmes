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
    public class MaterialUnloadingController : Controller
    {
        //
        // GET: /LSM/MaterialUnloading/
        public async Task<ActionResult> Index()
        {
            return await Query(new MaterialUnloadingQueryViewModel());
        }
        //
        //POST: /LSM/MaterialUnloading/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(MaterialUnloadingQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (MaterialUnloadingServiceClient client = new MaterialUnloadingServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            if (!string.IsNullOrEmpty(model.UnloadingNo))
                            {
                                where.AppendFormat(" {0} Key = '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.UnloadingNo);
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

                            if (model.StartUnloadingTime != null)
                            {
                                where.AppendFormat(" {0} UnloadingTime >= '{1:yyyy-MM-dd HH:mm:ss}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.StartUnloadingTime);
                            }

                            if (model.EndUnloadingTime != null)
                            {
                                where.AppendFormat(" {0} UnloadingTime <= '{1:yyyy-MM-dd HH:mm:ss}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.EndUnloadingTime);
                            }
                        }

                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "CreateTime Desc",
                            Where = where.ToString()
                        };

                        MethodReturnResult<IList<MaterialUnloading>> result = client.Get(ref cfg);

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
                return PartialView("_ListPartial",new MaterialUnloadingViewModel());
            }
            else
            {
                return View("Index", new MaterialUnloadingQueryViewModel());
            }
        }
        //
        //POST: /LSM/MaterialUnloading/PagingQuery
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

                using (MaterialUnloadingServiceClient client = new MaterialUnloadingServiceClient())
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
                        MethodReturnResult<IList<MaterialUnloading>> result = client.Get(ref cfg);
                        if (result.Code == 0)
                        {
                            ViewBag.PagingConfig = cfg;
                            ViewBag.List = result.Data;
                        }
                    });
                }
            }
            return PartialView("_ListPartial", new MaterialUnloadingViewModel());
        }
        //
        // GET: /LSM/MaterialUnloading/
        public async Task<ActionResult> Detail(MaterialUnloadingDetailQueryViewModel model)
        {
            return await DetailQuery(model);
        }
        //
        //POST: /LSM/MaterialUnloading/DetailQuery
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DetailQuery(MaterialUnloadingDetailQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (MaterialUnloadingServiceClient client = new MaterialUnloadingServiceClient())
                {
                    await Task.Run(() =>
                    {
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "CreateTime Desc,Key.UnloadingKey,Key.ItemNo",
                            Where = GetWhereCondition(model)
                        };
                        MethodReturnResult<IList<MaterialUnloadingDetail>> result = client.GetDetail(ref cfg);

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
                return PartialView("_DetailListPartial", new MaterialUnloadingDetailViewModel());
            }
            else
            {
                return View("Detail",model);
            }
        }
        //
        //POST: /WIP/MaterialUnloading/ExportToExcel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExportToExcel(MaterialUnloadingDetailQueryViewModel model)
        {
            IList<MaterialUnloadingDetail> lst = new List<MaterialUnloadingDetail>();
            using (MaterialUnloadingServiceClient client = new MaterialUnloadingServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        OrderBy = "CreateTime Desc,Key.UnloadingKey,Key.ItemNo",
                        Where = GetWhereCondition(model)
                    };
                    MethodReturnResult<IList<MaterialUnloadingDetail>> result = client.GetDetail(ref cfg);

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
                    cell.SetCellValue(LSMResources.StringResource.MaterialUnloadingViewModel_UnloadingNo);  //下料号

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialUnloadingDetailViewModel_ItemNo);  //项目号

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialUnloadingViewModel_RouteOperationName);  //工序

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialUnloadingViewModel_ProductionLineCode);  //生产线

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialUnloadingViewModel_EquipmentCode);  //设备

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("下料日期");  //下料时间

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialUnloadingViewModel_UnloadingTime);  //下料时间

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialUnloadingViewModel_Operator);  //操作人

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialUnloadingDetailViewModel_LoadingNo);  //对应上料号

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialUnloadingDetailViewModel_LoadingItemNo);  //对应上料项目号

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialUnloadingDetailViewModel_LineStoreName);  //线边仓

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialUnloadingViewModel_OrderNumber);  //工单号

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialUnloadingDetailViewModel_MaterialCode);  //物料编码

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("物料名称");  //物料名称

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialUnloadingDetailViewModel_MaterialLot);  //物料批号

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue(LSMResources.StringResource.MaterialUnloadingDetailViewModel_UnloadingQty);  //下料数量

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("编辑人");  //编辑人

                    cell = row.CreateCell(row.Cells.Count);
                    cell.CellStyle = style;
                    cell.SetCellValue("编辑时间");  //编辑时间
                    #endregion
                    font.Boldweight = 5;
                }

                MaterialUnloadingDetail obj = lst[j];
                MaterialUnloading objMaterialUnloading = model.GetMaterialUnloading(obj.Key.UnloadingKey);
                Material m = model.GetMaterial(obj.MaterialCode);
                row = ws.CreateRow(j + 1);

                #region //数据
                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(obj.Key.UnloadingKey);  //下料号

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(obj.Key.ItemNo);  //项目号

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(objMaterialUnloading.RouteOperationName);  //工序

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(objMaterialUnloading.ProductionLineCode);  //生产线

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(objMaterialUnloading.EquipmentCode);  //设备

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(string.Format("{0:yyyy-MM-dd}", objMaterialUnloading.UnloadingTime));  //下料日期

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(string.Format("{0:yyyy-MM-dd HH:mm:ss}", objMaterialUnloading.UnloadingTime));  //下料时间

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(objMaterialUnloading.Operator);  //操作人

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(@obj.LoadingKey);  //对应上料号

                cell = row.CreateCell(row.Cells.Count);
                cell.CellStyle = style;
                cell.SetCellValue(obj.LoadingItemNo);  //对应上料项目号

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
                cell.SetCellValue(obj.UnloadingQty);  //下料数量

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
            return File(ms, "application/vnd.ms-excel", "MaterialUnloadingData.xls");
        }

        public string GetWhereCondition(MaterialUnloadingDetailQueryViewModel model)
        {
            StringBuilder where = new StringBuilder();
            StringBuilder whereExists = new StringBuilder();
            bool isUseWhereExists = false;
            if (model != null)
            {
                if (!string.IsNullOrEmpty(model.UnloadingNo))
                {
                    where.AppendFormat(" {0} Key.UnloadingKey = '{1}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.UnloadingNo);

                    whereExists.AppendFormat(" {0} p.Key = '{1}'"
                                        , whereExists.Length > 0 ? "AND" : string.Empty
                                        , model.UnloadingNo);
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

                if (model.StartUnloadingTime != null)
                {
                    whereExists.AppendFormat(" {0} p.UnloadingTime >= '{1:yyyy-MM-dd HH:mm:ss}'"
                                            , whereExists.Length > 0 ? "AND" : string.Empty
                                            , model.StartUnloadingTime);
                    isUseWhereExists = true;
                }

                if (model.EndUnloadingTime != null)
                {
                    whereExists.AppendFormat(" {0} p.UnloadingTime <= '{1:yyyy-MM-dd HH:mm:ss}'"
                                            , whereExists.Length > 0 ? "AND" : string.Empty
                                            , model.EndUnloadingTime);
                    isUseWhereExists = true;
                }
            }

            if (isUseWhereExists)
            {
                where.AppendFormat(" {0} EXISTS( From MaterialUnloading as p WHERE p.Key=self.Key.UnloadingKey AND {1})"
                                   , where.Length > 0 ? "AND" : string.Empty
                                   , whereExists);
            }
            return where.ToString();
        }
        //
        //POST: /LSM/MaterialUnloading/DetailPagingQuery
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

                using (MaterialUnloadingServiceClient client = new MaterialUnloadingServiceClient())
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
                        MethodReturnResult<IList<MaterialUnloadingDetail>> result = client.GetDetail(ref cfg);
                        if (result.Code == 0)
                        {
                            ViewBag.PagingConfig = cfg;
                            ViewBag.List = result.Data;
                        }
                    });
                }
            }
            return PartialView("_DetailListPartial", new MaterialUnloadingDetailViewModel());
        }

        //
        // POST: /PPM/MaterialUnloading/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(MaterialUnloadingViewModel model)
        {
            MethodReturnResult rst = new MethodReturnResult();
            try
            {
                using (MaterialUnloadingServiceClient client = new MaterialUnloadingServiceClient())
                {
                    MaterialUnloading obj = new MaterialUnloading()
                    {
                        Key = model.UnloadingNo.ToUpper(),
                        RouteOperationName = model.RouteOperationName,
                        ProductionLineCode = model.ProductionLineCode,
                        EquipmentCode = model.EquipmentCode,
                        Operator = model.Operator,
                        UnloadingTime = model.UnloadingTime,
                        Description = model.Description,
                        Editor = User.Identity.Name,
                        Creator = User.Identity.Name
                    };

                    char splitChar = ',';
                    var ItemNos = Request["ItemNo"].Split(splitChar);
                    var LineStoreNames = Request["LineStoreName"].Split(splitChar);
                    var MaterialCodes = Request["MaterialCode"].Split(splitChar);
                    var MaterialLots = Request["MaterialLot"].Split(splitChar);
                    var Qtys = Request["UnloadingQty"].Split(splitChar);
                    var LoadingKeys = Request["LoadingNo"].Split(splitChar);
                    var LoadingItemNos = Request["LoadingItemNo"].Split(splitChar);

                    List<MaterialUnloadingDetail> lst = new List<MaterialUnloadingDetail>();
                    for (int i = 0; i < ItemNos.Length; i++)
                    {
                        lst.Add(new MaterialUnloadingDetail()
                        {
                            Key = new MaterialUnloadingDetailKey()
                            {
                                UnloadingKey = model.UnloadingNo,
                                ItemNo = i + 1
                            },
                            LoadingKey=LoadingKeys[i].ToUpper(),
                            LoadingItemNo = Convert.ToInt32(LoadingItemNos[i]),
                            LineStoreName = LineStoreNames[i].ToUpper(),
                            OrderNumber = model.OrderNumber,
                            MaterialCode = MaterialCodes[i].ToUpper(),
                            MaterialLot = MaterialLots[i].ToUpper(),
                            UnloadingQty = Convert.ToDouble(Qtys[i]),
                            Editor = User.Identity.Name,
                            Creator = User.Identity.Name
                        });
                    }

                    rst = await client.AddAsync(obj, lst);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(LSMResources.StringResource.MaterialUnloading_Save_Success
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
            //根据车间和工序获取线边仓。
            using (EquipmentServiceClient client = new EquipmentServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"LineCode='{0}' AND EXISTS(FROM RouteOperationEquipment as p WHERE p.Key.EquipmentCode=self.Key AND p.Key.RouteOperationName='{1}')"
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
                          Text = item.Key + "-" + item.Name
                      };
            return Json(lnq, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLoadingNo(string q,string orderNumber, string equipmentCode)
        {
            IList<MaterialLoadingDetail> lstDetail = new List<MaterialLoadingDetail>();
            using (MaterialLoadingServiceClient client = new MaterialLoadingServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"(Key.LoadingKey LIKE '{0}%'
                                             OR MaterialCode LIKE '{0}%'
                                             OR MaterialLot LIKE '{0}%')
                                             AND EXISTS(FROM MaterialLoading as p 
                                                        WHERE p.Key=self.Key.LoadingKey
                                                        AND p.EquipmentCode='{1}')
                                             AND CurrentQty>0
                                             AND OrderNumber='{2}'"
                                             , q
                                             , equipmentCode
                                             , orderNumber),
                    OrderBy = "Key"
                };

                MethodReturnResult<IList<MaterialLoadingDetail>> result = client.GetDetail(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstDetail = result.Data;
                }
            }

            return Json(from item in lstDetail
                        select new
                        {
                            @value = item.MaterialLot,
                            @label = @LSMResources.StringResource.MaterialUnloadingDetailViewModel_LoadingNo +"："+ item.Key.LoadingKey
                                     + " " + @LSMResources.StringResource.MaterialUnloadingDetailViewModel_LoadingItemNo + "：" + item.Key.ItemNo
                                     + " " + @LSMResources.StringResource.MaterialUnloadingDetailViewModel_MaterialLot + "：" + item.MaterialLot
                                     + " " + GetMaterialName(item.MaterialCode),
                            @LoadingNo = item.Key.LoadingKey,
                            @LoadingItemNo = item.Key.ItemNo,
                            @LineStoreName = item.LineStoreName,
                            @MaterialCode = item.MaterialCode,
                            @CurrentQty = item.CurrentQty
                        }, JsonRequestBehavior.AllowGet);
        }


        private string GetMaterialName(string materialCode)
        {
            using (MaterialServiceClient client = new MaterialServiceClient())
            {
                MethodReturnResult<Material> result = client.Get(materialCode);
                if (result.Code <= 0 && result.Data!=null)
                {
                    return result.Data.Name;
                }
            }
            return string.Empty;
        }

        public ActionResult GetUnloadingNo()
        {
            string prefix = string.Format("MUM{0:yyMM}", DateTime.Now);
            int itemNo = 0;
            using (MaterialUnloadingServiceClient client = new MaterialUnloadingServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key LIKE '{0}%'", prefix),
                    OrderBy = "Key Desc"
                };
                MethodReturnResult<IList<MaterialUnloading>> result = client.Get(ref cfg);
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