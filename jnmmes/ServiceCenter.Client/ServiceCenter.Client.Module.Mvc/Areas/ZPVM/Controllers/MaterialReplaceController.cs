using ServiceCenter.Client.Mvc.Areas.ZPVM.Models;
using ZPVMResources = ServiceCenter.Client.Mvc.Resources.ZPVM;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.MES.Service.Client.ZPVM;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ServiceCenter.MES.Service.Client.ERP;
using System.Data;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Service.Client.PPM;

namespace ServiceCenter.Client.Mvc.Areas.ZPVM.Controllers
{
    public class MaterialReplaceController : Controller
    {
        //
        // GET: /ZPVM/MaterialReplace/
        public async Task<ActionResult> Index()
        {
            using (MaterialReplaceServiceClient client = new MaterialReplaceServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = "Key"
                    };
                    MethodReturnResult<IList<MaterialReplace>> result = client.Gets(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new MaterialQueryReplaceViewModel());
        }

        //
        //POST: /ZPVM/MaterialReplace/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(MaterialQueryReplaceViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (MaterialReplaceServiceClient client = new MaterialReplaceServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            if (!string.IsNullOrEmpty(model.ProductCode))
                            {
                                where.AppendFormat(" {0} Key.ProductCode LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.ProductCode.ToString().Trim().ToUpper());
                            }

                            if (!string.IsNullOrEmpty(model.OrderNumber))
                            {
                                where.AppendFormat(" {0} Key.OrderNumber LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.OrderNumber.ToString().Trim().ToUpper());
                            }

                            if (!string.IsNullOrEmpty(model.OldMaterialCode))
                            {
                                where.AppendFormat(" {0} Key.OldMaterialCode LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.OldMaterialCode.ToString().Trim().ToUpper());
                            }

                            if (!string.IsNullOrEmpty(model.OldMaterialSupplier))
                            {
                                where.AppendFormat(" {0} Key.OldMaterialSupplier LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.OldMaterialSupplier.ToString().Trim().ToUpper());
                            }

                            if (!string.IsNullOrEmpty(model.NewMaterialCode))
                            {
                                where.AppendFormat(" {0} NewMaterialCode LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.NewMaterialCode.ToString().Trim().ToUpper());
                            }

                            if (!string.IsNullOrEmpty(model.NewMaterialSupplier))
                            {
                                where.AppendFormat(" {0} NewMaterialSupplier LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.NewMaterialSupplier.ToString().Trim().ToUpper());
                            }

                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "Key.ProductCode desc,EditTime desc ",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<MaterialReplace>> result = client.Gets(ref cfg);

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
        //POST: /ZPVM/MaterialReplace/PagingQuery
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

                using (MaterialReplaceServiceClient client = new MaterialReplaceServiceClient())
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
                        MethodReturnResult<IList<MaterialReplace>> result = client.Gets(ref cfg);
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
        // POST: /ZPVM/MaterialReplace/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(MaterialReplaceViewModel model)
        {
            DateTime now = DateTime.Now;
            MethodReturnResult rst = new MethodReturnResult();

            #region 界面录入信息合规性检查
            using (MaterialServiceClient client = new MaterialServiceClient())
            {
                MethodReturnResult<Material> result = client.Get(model.ProductCode);
                if (result.Code != 0)
                {
                    rst.Code = 1001;
                    rst.Message = String.Format("MES中不存在产品编码：{0}", model.ProductCode);
                    return Json(rst);
                }
                else
                {
                    if (result.Data.IsProduct != true)
                    {
                        rst.Code = 1001;
                        rst.Message = String.Format("物料编码[{0}]非产品!", model.ProductCode);
                        return Json(rst);
                    }
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
                            else
                            {
                                if (resultOfOrder.Data.MaterialCode != model.ProductCode)
                                {
                                    rst.Code = 1001;
                                    rst.Message = String.Format("产品编码{0}与工单{1}不匹配！",model.ProductCode, model.OrderNumber);
                                    return Json(rst);
                                }
                            }
                        }                        
                    }
                }
            }
            #endregion

            using (ERPClient erpClient = new ERPClient())
            {
                MethodReturnResult<DataSet> ds_supplier = erpClient.GetERPSupplier(model.OldMaterialSupplier);
                MethodReturnResult<DataSet> ds_supplier1 = erpClient.GetERPSupplier(model.NewMaterialSupplier);
                if (model.OldMaterialSupplier != "*")
                {
                    if (ds_supplier.Data == null || ds_supplier.Data.Tables[0].Rows.Count == 0)
                    {
                        rst.Code = 1001;
                        rst.Message = String.Format("ERP中不存在供应商：{0}", model.OldMaterialSupplier);
                        return Json(rst);
                    }
                    if (ds_supplier.Data != null || ds_supplier.Data.Tables[0].Rows.Count > 0)
                    {
                        //新增替换前供应商
                        using (SupplierServiceClient supplierClient = new SupplierServiceClient())
                        {
                            MethodReturnResult<Supplier> result = await supplierClient.GetAsync(model.OldMaterialSupplier);
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
                    }   
                }
                if (model.NewMaterialSupplier != "000000")
                {
                    if (ds_supplier1.Data == null || ds_supplier1.Data.Tables[0].Rows.Count == 0)
                    {
                        rst.Code = 1001;
                        rst.Message = String.Format("ERP中不存在供应商：{0}", model.NewMaterialSupplier);
                        return Json(rst);
                    }
                    //新增替换后供应商
                    using (SupplierServiceClient supplierClient1 = new SupplierServiceClient())
                    {
                        MethodReturnResult<Supplier> result = await supplierClient1.GetAsync(model.NewMaterialSupplier);
                        if (result.Code != 0)
                        {
                            Supplier supplier = new Supplier()
                            {
                                Key = ds_supplier1.Data.Tables[0].Rows[0]["CUSCODE"].ToString(),
                                Name = ds_supplier1.Data.Tables[0].Rows[0]["CUSNAME"].ToString(),
                                NickName = " ",
                                CreateTime = now,
                                EditTime = now,
                                Creator = User.Identity.Name,
                                Editor = User.Identity.Name,
                                Description = ""
                            };
                            rst = await supplierClient1.AddAsync(supplier);
                            if (rst.Code != 0)
                            {
                                return Json(rst);
                            }
                        }
                    }
                }               
                                          
                //新增替换规则
                using (MaterialReplaceServiceClient client = new MaterialReplaceServiceClient())
                {
                    MaterialReplace obj = new MaterialReplace()
                    {
                        Key = new MaterialReplaceKey()
                        {
                            ProductCode = model.ProductCode.ToString().Trim().ToUpper(),
                            OrderNumber = model.OrderNumber.ToString().Trim().ToUpper(),
                            OldMaterialCode = model.OldMaterialCode.ToString().Trim().ToUpper(),
                            OldMaterialSupplier = model.OldMaterialSupplier.ToString().Trim().ToUpper()
                        },
                        NewMaterialCode = model.NewMaterialCode.ToString().Trim().ToUpper(),
                        NewMaterialSupplier = model.NewMaterialSupplier.ToString().Trim().ToUpper(),
                        Creator = User.Identity.Name,
                        Editor = User.Identity.Name,
                        CreateTime = DateTime.Now,
                        EditTime = DateTime.Now,
                        Description = model.Description
                    };
                    rst = await client.AddAsync(obj);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(ZPVMResources.StringResource.MaterialReplace_Save_Success);
                    }
                    return Json(rst);
                }
            }           
        }
        //
        // GET: /ZPVM/MaterialReplace/Modify
        public async Task<ActionResult> Modify(string productCode, string orderNumber, string oldMaterialCode, string oldMaterialSupplier)
        {
            MaterialReplaceViewModel viewModel = new MaterialReplaceViewModel();
            using (MaterialReplaceServiceClient client = new MaterialReplaceServiceClient())
            {
                MethodReturnResult<MaterialReplace> result = await client.GetAsync(new MaterialReplaceKey()
                {
                    ProductCode = productCode,
                    OrderNumber = orderNumber,
                    OldMaterialCode = oldMaterialCode,
                    OldMaterialSupplier = oldMaterialSupplier
                });
                if (result.Code == 0)
                {
                    viewModel = new MaterialReplaceViewModel()
                    {
                        ProductCode = result.Data.Key.ProductCode,
                        OrderNumber = result.Data.Key.OrderNumber,
                        OldMaterialCode = result.Data.Key.OldMaterialCode,
                        OldMaterialSupplier = result.Data.Key.OldMaterialSupplier,
                        NewMaterialCode = result.Data.NewMaterialCode,
                        NewMaterialSupplier = result.Data.NewMaterialSupplier,
                        Description = result.Data.Description
                    };
                    return PartialView("_ModifyPartial", viewModel);
                }
                else
                {
                    ModelState.AddModelError("", result.Message);
                }
            }
            return PartialView("_ModifyPartial", viewModel);
        }

        //
        // POST: /ZPVM/MaterialReplace/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(MaterialReplaceViewModel model)
        {
            using (MaterialReplaceServiceClient client = new MaterialReplaceServiceClient())
            {
                MaterialReplaceKey key = new MaterialReplaceKey()
                {
                    ProductCode = model.ProductCode.ToString().Trim().ToUpper(),
                    OrderNumber = model.OrderNumber.ToString().Trim().ToUpper(),
                    OldMaterialCode = model.OldMaterialCode.ToString().Trim().ToUpper(),
                    OldMaterialSupplier = model.OldMaterialSupplier.ToString().Trim().ToUpper()
                };
                MethodReturnResult<MaterialReplace> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    result.Data.NewMaterialCode = model.NewMaterialCode;
                    result.Data.NewMaterialSupplier = model.NewMaterialSupplier;
                    result.Data.Description = model.Description;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format("物料替换规则修改成功！");
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // GET: /ZPVM/MaterialReplace/Detail
        public async Task<ActionResult> Detail(string productCode, string orderNumber, string oldMaterialCode, string oldMaterialSupplier)
        {
            using (MaterialReplaceServiceClient client = new MaterialReplaceServiceClient())
            {
                MaterialReplaceKey key = new MaterialReplaceKey()
                {
                    ProductCode = productCode,
                    OrderNumber = orderNumber,
                    OldMaterialCode = oldMaterialCode,
                    OldMaterialSupplier = oldMaterialSupplier
                };
                MethodReturnResult<MaterialReplace> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    MaterialReplaceViewModel viewModel = new MaterialReplaceViewModel()
                    {
                        ProductCode = result.Data.Key.ProductCode,
                        OrderNumber = result.Data.Key.OrderNumber,
                        OldMaterialCode = result.Data.Key.OldMaterialCode,
                        OldMaterialSupplier = result.Data.Key.OldMaterialSupplier,
                        NewMaterialCode = result.Data.NewMaterialCode,
                        NewMaterialSupplier = result.Data.NewMaterialSupplier,
                        CreateTime = result.Data.CreateTime,
                        Creator = result.Data.Creator,
                        Editor = result.Data.Editor,
                        EditTime = result.Data.EditTime,
                        Description = result.Data.Description
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
        // POST: /ZPVM/MaterialReplace/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string productCode, string orderNumber, string oldMaterialCode, string oldMaterialSupplier)
        {
            MethodReturnResult result = new MethodReturnResult();
            MaterialReplaceKey key = new MaterialReplaceKey()
            {
                ProductCode = productCode,
                OrderNumber = orderNumber,
                OldMaterialCode = oldMaterialCode,
                OldMaterialSupplier = oldMaterialSupplier
            };
            using (MaterialReplaceServiceClient client = new MaterialReplaceServiceClient())
            {
                result = await client.DeleteAsync(key);
                if (result.Code == 0)
                {
                    result.Message = string.Format("删除成功");
                }
                return Json(result);
            }
        }
    }
}