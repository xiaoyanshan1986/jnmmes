using ServiceCenter.Client.Mvc.Areas.ZPVM.Models;
using ZPVMResources = ServiceCenter.Client.Mvc.Resources.ZPVM;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.MES.Service.Client.ZPVM;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Service.Client.ERP;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ServiceCenter.MES.Model.FMM;
using System.Data;
using ServiceCenter.MES.Service.Client.PPM;
using ServiceCenter.MES.Model.PPM;

namespace ServiceCenter.Client.Mvc.Areas.ZPVM.Controllers
{
    public class SupplierToManufacturerController : Controller
    {
        //
        // GET: /ZPVM/SupplierToManufacturer/
        public async Task<ActionResult> Index()
        {
            using (SupplierToManufacturerServiceClient client = new SupplierToManufacturerServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = "Key"
                    };
                    MethodReturnResult<IList<SupplierToManufacturer>> result = client.Gets(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new SupplierToManufacturerQueryViewModel());
        }

        //
        //POST: /ZPVM/SupplierToManufacturer/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(SupplierToManufacturerQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (SupplierToManufacturerServiceClient client = new SupplierToManufacturerServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            if (!string.IsNullOrEmpty(model.MaterialCode))
                            {
                                where.AppendFormat(" {0} Key.MaterialCode LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.MaterialCode.ToString().Trim().ToUpper());
                            }

                            if (!string.IsNullOrEmpty(model.OrderNumber))
                            {
                                where.AppendFormat(" {0} Key.OrderNumber LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.OrderNumber.ToString().Trim().ToUpper());
                            }

                            if (!string.IsNullOrEmpty(model.SupplierCode))
                            {
                                where.AppendFormat(" {0} Key.SupplierCode LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.SupplierCode.ToString().Trim().ToUpper());
                            }

                            if (!string.IsNullOrEmpty(model.ManufacturerCode))
                            {
                                where.AppendFormat(" {0} ManufacturerCode LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.ManufacturerCode.ToString().Trim().ToUpper());
                            }

                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "Key",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<SupplierToManufacturer>> result = client.Gets(ref cfg);

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
        //POST: /ZPVM/SupplierToManufacturer/PagingQuery
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

                using (SupplierToManufacturerServiceClient client = new SupplierToManufacturerServiceClient())
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
                        MethodReturnResult<IList<SupplierToManufacturer>> result = client.Gets(ref cfg);
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
        // POST: /ZPVM/SupplierToManufacturer/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(SupplierToManufacturerViewModel model)
        {
            DateTime now = DateTime.Now;
            MethodReturnResult rst = new MethodReturnResult();

            #region 界面录入信息合规性检查
            using (MaterialServiceClient client = new MaterialServiceClient())
            {
                MethodReturnResult<Material> result = client.Get(model.MaterialCode);
                if (result.Code != 0)
                {
                    rst.Code = 1001;
                    rst.Message = String.Format("MES中不存在物料编码：{0}", model.MaterialCode);
                    return Json(rst);
                }
                else
                {
                    if (model.OrderNumber != "*")
                    {
                        using (WorkOrderServiceClient clientOfWorkOrder = new WorkOrderServiceClient())
                        {
                            MethodReturnResult<WorkOrder> resultOfOrder = clientOfWorkOrder.Get(model.OrderNumber);
                            if (resultOfOrder.Code != 0)
                            {
                                rst.Code = 1001;
                                rst.Message = String.Format("MES中不存在工单：{0}", model.OrderNumber);
                                return Json(rst);
                            }
                        }
                    }
                }
            }
            #endregion

            using (ERPClient erpClient = new ERPClient())
            {
                MethodReturnResult<DataSet> ds_supplier = erpClient.GetERPSupplier(model.SupplierCode);
                MethodReturnResult<DataSet> ds_manufacturer = erpClient.GetByCodeERPManufacturer(model.ManufacturerCode);
                if (ds_supplier == null || ds_supplier.Data.Tables[0].Rows.Count == 0)
                {
                    rst.Code = 1001;
                    rst.Message = String.Format("ERP中不存在供应商：{0}", model.SupplierCode);
                    return Json(rst);
                }
                if (ds_manufacturer == null || ds_manufacturer.Data.Tables[0].Rows.Count == 0)
                {
                    rst.Code = 1001;
                    rst.Message = String.Format("ERP中不存在生产厂商：{0}", model.ManufacturerCode);
                    return Json(rst);
                }
                //新增转换供应商
                using (SupplierServiceClient supplierClient = new SupplierServiceClient())
                {
                    MethodReturnResult<Supplier> result = await supplierClient.GetAsync(model.SupplierCode);
                    if (result.Code != 0)
                    {
                        Supplier supplier = new Supplier()
                        {
                            Key = ds_supplier.Data.Tables[0].Rows[0]["CUSCODE"].ToString(),
                            Name = ds_supplier.Data.Tables[0].Rows[0]["CUSNAME"].ToString(),
                            NickName = " ",
                            CreateTime = now,
                            EditTime = now,
                            Creator = User.Identity.Name,
                            Editor = User.Identity.Name,
                            Description = ""
                        };
                        rst = await supplierClient.AddAsync(supplier);
                        if (rst.Code != 0)
                        {
                            return Json(rst);
                        }
                    }
                }
                //新增转换生产厂商
                using (ManufacturerServiceClient manufacturerClient = new ManufacturerServiceClient())
                {
                    MethodReturnResult<Manufacturer> result = await manufacturerClient.GetAsync(model.ManufacturerCode);
                    if (result.Code != 0)
                    {
                        Manufacturer manufacturer = new Manufacturer()
                        {
                            Key = ds_manufacturer.Data.Tables[0].Rows[0]["CSCODE"].ToString(),
                            Name = ds_manufacturer.Data.Tables[0].Rows[0]["CSNAME"].ToString(),
                            NickName = " ",
                            CreateTime = now,
                            EditTime = now,
                            Creator = User.Identity.Name,
                            Editor = User.Identity.Name,
                            Description = ""
                        };
                        rst = await manufacturerClient.AddAsync(manufacturer);
                        if (rst.Code != 0)
                        {
                            return Json(rst);
                        }
                    }
                }
                //新增转换规则
                using (SupplierToManufacturerServiceClient client = new SupplierToManufacturerServiceClient())
                {
                    SupplierToManufacturer obj = new SupplierToManufacturer()
                    {
                        Key = new SupplierToManufacturerKey()
                        {
                            MaterialCode = model.MaterialCode.ToString().Trim().ToUpper(),
                            OrderNumber = model.OrderNumber.ToString().Trim().ToUpper(),
                            SupplierCode = model.SupplierCode.ToString().Trim().ToUpper()
                        },
                        ManufacturerCode = model.ManufacturerCode.ToString().Trim().ToUpper(),
                        CreateTime = now,
                        EditTime = now,
                        Creator = User.Identity.Name,
                        Editor = User.Identity.Name
                    };
                    rst = await client.AddAsync(obj);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(ZPVMResources.StringResource.SupplierToManufacturer_Save_Success);
                    }
                    return Json(rst);
                }
            }         
        }
        //
        // GET: /ZPVM/SupplierToManufacturer/Modify
        public async Task<ActionResult> Modify(string materialCode, string orderNumber, string supplierCode)
        {
            SupplierToManufacturerViewModel viewModel = new SupplierToManufacturerViewModel();
            using (SupplierToManufacturerServiceClient client = new SupplierToManufacturerServiceClient())
            {
                MethodReturnResult<SupplierToManufacturer> result = await client.GetAsync(new SupplierToManufacturerKey()
                {
                    MaterialCode = materialCode,
                    OrderNumber = orderNumber,
                    SupplierCode = supplierCode
                });
                if (result.Code == 0)
                {
                    viewModel = new SupplierToManufacturerViewModel()
                    {
                        MaterialCode = result.Data.Key.MaterialCode,
                        OrderNumber = result.Data.Key.OrderNumber,
                        SupplierCode = result.Data.Key.SupplierCode,
                        ManufacturerCode = result.Data.ManufacturerCode
                    };
                    return PartialView("_ModifyPartial", viewModel);
                }
                else
                {
                    ModelState.AddModelError("", result.Message);
                }
            }
            return PartialView("_ModifyPartial");
        }

        //
        // POST: /ZPVM/SupplierToManufacturer/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(SupplierToManufacturerViewModel model)
        {
            DateTime now = DateTime.Now;
            using (SupplierToManufacturerServiceClient client = new SupplierToManufacturerServiceClient())
            {
                SupplierToManufacturerKey key = new SupplierToManufacturerKey()
                {
                    MaterialCode = model.MaterialCode.ToString().Trim().ToUpper(),
                    OrderNumber = model.OrderNumber.ToString().Trim().ToUpper(),
                    SupplierCode = model.SupplierCode.ToString().Trim().ToUpper()
                };
                MethodReturnResult<SupplierToManufacturer> result = await client.GetAsync(key);
                if (result.Code == 0)
                { 
                    result.Data.ManufacturerCode = model.ManufacturerCode;  
                    result.Data.EditTime = now;
                    result.Data.Editor = User.Identity.Name;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format("供应商转换生产厂商规则修改成功！");
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // GET: /ZPVM/SupplierToManufacturer/Detail
        public async Task<ActionResult> Detail(string materialCode, string orderNumber, string supplierCode)
        {
            using (SupplierToManufacturerServiceClient client = new SupplierToManufacturerServiceClient())
            {
                SupplierToManufacturerKey key = new SupplierToManufacturerKey()
                {
                    MaterialCode = materialCode,
                    OrderNumber = orderNumber,
                    SupplierCode = supplierCode
                };
                MethodReturnResult<SupplierToManufacturer> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    SupplierToManufacturerViewModel viewModel = new SupplierToManufacturerViewModel()
                    {
                        MaterialCode = result.Data.Key.MaterialCode,
                        OrderNumber = result.Data.Key.OrderNumber,
                        SupplierCode = result.Data.Key.SupplierCode,
                        ManufacturerCode = result.Data.ManufacturerCode,
                        Creator = result.Data.Creator,
                        CreateTime = result.Data.CreateTime,
                        Editor = result.Data.Editor,
                        EditTime = result.Data.EditTime
                    };
                    return PartialView("_InfoPartial", viewModel);
                }
                else
                {
                    ModelState.AddModelError("", result.Message);
                }
            }
            return PartialView("_InfoPartial");
        }
        //
        // POST: /ZPVM/SupplierToManufacturer/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string materialCode, string orderNumber, string supplierCode)
        {
            MethodReturnResult result = new MethodReturnResult();
            SupplierToManufacturerKey key = new SupplierToManufacturerKey()
            {
                MaterialCode = materialCode,
                OrderNumber = orderNumber,
                SupplierCode = supplierCode
            };
            using (SupplierToManufacturerServiceClient client = new SupplierToManufacturerServiceClient())
            {
                result = await client.DeleteAsync(key);
                if (result.Code == 0)
                {
                    result.Message = string.Format("删除{0}成功",key);
                }
                return Json(result);
            }
        }
    }
}