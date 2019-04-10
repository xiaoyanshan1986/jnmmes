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
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Model.FMM;

namespace ServiceCenter.Client.Mvc.Areas.ZPVM.Controllers
{
    public class ProductControlObjectController : Controller
    {
        public ActionResult Index()
        {
            //using (MaterialServiceClient client = new MaterialServiceClient())
            //{

            //        PagingConfig cfg = new PagingConfig()
            //        {
            //            Where="Key like '12%'",
            //            OrderBy = "Key"
            //        };
            //        MethodReturnResult<IList<Material>> result = client.Get(ref cfg);

            //        if (result.Code == 0)
            //        {
            //            ViewBag.PagingConfig = cfg;
            //            ViewBag.List = result.Data;
            //        }

            //}
            return View("Index",new ProductControlObjectQueryViewModel());
        }
        //
        //POST: /ZPVM/ProductControlObject/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MaterialQuery(ProductControlObjectQueryViewModel model)
        {
            //if (ModelState.IsValid)
            //{
            //    using (MaterialServiceClient client = new MaterialServiceClient())
            //    {
            //        await Task.Run(() =>
            //        {
            //            StringBuilder where = new StringBuilder();
            //            where.Append("Key like '12%'");
            //            if (model != null)
            //            {
            //                if (!string.IsNullOrEmpty(model.ProductCode))
            //                {
            //                    where.AppendFormat(" {0} Key LIKE '{1}%'"
            //                                        , where.Length > 0 ? "AND" : string.Empty
            //                                        , model.ProductCode);
            //                }
            //                if (!string.IsNullOrEmpty(model.ProductName))
            //                {
            //                    where.AppendFormat(" {0} Name LIKE '{1}%'"
            //                                        , where.Length > 0 ? "AND" : string.Empty
            //                                        , model.ProductName);
            //                }
            //            }
            //            PagingConfig cfg = new PagingConfig()
            //            {
            //                OrderBy = "Key",
            //                Where = where.ToString()
            //            };
            //            MethodReturnResult<IList<Material>> result = client.Get(ref cfg);

            //            if (result.Code == 0)
            //            {
            //                ViewBag.PagingConfig = cfg;
            //                ViewBag.List = result.Data;
            //            }
            //        });
            //    }
            //}
            return PartialView("_MaterialListPartial");
        }

        //
        //POST: /ZPVM/ProductControlObject/Query
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(string ProductCode, string ProductName)
        //public ActionResult Query(string ProductCode, string ProductName)
        {
            if (ModelState.IsValid)
            {
                ViewBag.ProductName = ProductName;
                ViewBag.ProductCode = ProductCode;
                using (ProductControlObjectServiceClient client = new ProductControlObjectServiceClient())
                {
                    await Task.Run(() =>
                    {
                        string where = null;
                        where = string.Format("Key.ProductCode='{0}' AND ProductName='{1}'", ProductCode, ProductName);
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "Key.Object",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<ProductControlObject>> result = client.Get(ref cfg);

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
                return PartialView("_ListPartial", new ProductControlObjectViewModel() { ProductName = ProductName, ProductCode = ProductCode });
            }
            else
            {
                return View("_Query", new ProductControlObjectViewModel() { ProductName = ProductName, ProductCode = ProductCode });
            }
            
        }
        //
        //POST: /ZPVM/ProductControlObject/PagingQuery
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

                using (ProductControlObjectServiceClient client = new ProductControlObjectServiceClient())
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
                        MethodReturnResult<IList<ProductControlObject>> result = client.Get(ref cfg);
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
        //POST: /ZPVM/ProductControlObject/PagingQuery
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MaterialPagingQuery(string where, string orderBy, int? currentPageNo, int? currentPageSize)
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

                using (MaterialServiceClient client = new MaterialServiceClient())
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
                        MethodReturnResult<IList<Material>> result = client.Get(ref cfg);
                        if (result.Code == 0)
                        {
                            ViewBag.PagingConfig = cfg;
                            ViewBag.List = result.Data;
                        }
                    });
                }
            }
            return PartialView("_MaterialListPartial");
        }
        
        //
        // POST: /ZPVM/ProductControlObject/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(ProductControlObjectViewModel model)
        {
            using (ProductControlObjectServiceClient client = new ProductControlObjectServiceClient())
            {
                ProductControlObject obj = new ProductControlObject()
                {
                    Key = new ProductControlObjectKey(){
                        ProductCode = model.ProductCode,
                        CellEff=model.CellEff,
                        SupplierCode=model.SupplierCode,
                        Object=model.Object,
                        Type=model.Type
                    },
                    ProductName=model.ProductName,
                    SupplierName=model.SupplierName,
                    Value=model.Value,
                    IsUsed=model.IsUsed,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                    CreateTime = DateTime.Now,
                    Creator = User.Identity.Name
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(ZPVMResources.StringResource.ProductControlObject_Save_Success
                                                , obj.Key);
                }
                return Json(rst);
            }
        }
        //
        // GET: /ZPVM/ProductControlObject/Modify
        public async Task<ActionResult> Modify(string ProductCode, string CellEff, string SupplierCode, EnumPVMTestDataType obj, string type)
        {
            ProductControlObjectViewModel viewModel = new ProductControlObjectViewModel();
            using (ProductControlObjectServiceClient client = new ProductControlObjectServiceClient())
            {
                MethodReturnResult<ProductControlObject> result = await client.GetAsync(new ProductControlObjectKey()
                {
                    ProductCode = ProductCode,
                    CellEff = CellEff,
                    SupplierCode = SupplierCode,
                    Object=obj,
                    Type=type
                });
                if (result.Code == 0)
                {
                    viewModel = new ProductControlObjectViewModel()
                    {
                        ProductCode = result.Data.Key.ProductCode,
                        CellEff = result.Data.Key.CellEff,
                        SupplierCode=result.Data.Key.SupplierCode,
                        ProductName = result.Data.ProductName,
                        SupplierName = result.Data.SupplierName,
                        Value=result.Data.Value,
                        Type=result.Data.Key.Type,
                        Object=result.Data.Key.Object,
                        IsUsed=result.Data.IsUsed,
                        CreateTime = result.Data.CreateTime,
                        Creator = result.Data.Creator,
                        Editor = result.Data.Editor,
                        EditTime = result.Data.EditTime
                    };
                    return PartialView("_ModifyPartial", viewModel);
                }
                else
                {
                    ModelState.AddModelError("", result.Message);
                }
            }
            return PartialView("_ModifyPartial", new ProductControlObjectViewModel());
        }

        //
        // POST: /ZPVM/ProductControlObject/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(ProductControlObjectViewModel model)
        {
            using (ProductControlObjectServiceClient client = new ProductControlObjectServiceClient())
            {
                ProductControlObjectKey key = new ProductControlObjectKey()
                {
                    ProductCode = model.ProductCode,
                    CellEff = model.CellEff,
                    SupplierCode=model.SupplierCode,
                    Type=model.Type,
                    Object=model.Object
                };
                MethodReturnResult<ProductControlObject> result = await client.GetAsync(key);

                if (result.Code == 0)
                {
                    result.Data.ProductName = model.ProductName;
                    result.Data.SupplierName = model.SupplierName;
                    result.Data.Value = model.Value;
                    result.Data.IsUsed = model.IsUsed;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(ZPVMResources.StringResource.ProductControlObject_SaveModify_Success
                                                    , key);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // GET: /ZPVM/ProductControlObject/Detail
        public async Task<ActionResult> Detail(string ProductCode, string CellEff, string SupplierCode, EnumPVMTestDataType obj, string type)
        {
            using (ProductControlObjectServiceClient client = new ProductControlObjectServiceClient())
            {
                ProductControlObjectKey key = new ProductControlObjectKey()
                {
                    ProductCode = ProductCode,
                    CellEff = CellEff,
                    SupplierCode = SupplierCode,
                    Object=obj,
                    Type=type
                };
                MethodReturnResult<ProductControlObject> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    ProductControlObjectViewModel viewModel = new ProductControlObjectViewModel()
                    {
                        ProductCode = result.Data.Key.ProductCode,
                        CellEff = result.Data.Key.CellEff,
                        SupplierCode = result.Data.Key.SupplierCode,
                        ProductName=result.Data.ProductName,
                        SupplierName = result.Data.SupplierName,
                        Value = result.Data.Value,
                        Type = result.Data.Key.Type,
                        Object = result.Data.Key.Object,
                        IsUsed = result.Data.IsUsed,
                        CreateTime = result.Data.CreateTime,
                        Creator = result.Data.Creator,
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
        // POST: /ZPVM/ProductControlObject/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string ProductCode, string CellEff, string SupplierCode, EnumPVMTestDataType obj, string type)
        {
            MethodReturnResult result = new MethodReturnResult();
            ProductControlObjectKey key = new ProductControlObjectKey()
            {
                ProductCode = ProductCode,
                CellEff = CellEff,
                SupplierCode=SupplierCode,
                Object=obj,
                Type=type
            };
            using (ProductControlObjectServiceClient client = new ProductControlObjectServiceClient())
            {
                result = await client.DeleteAsync(key);
                if (result.Code == 0)
                {
                    result.Message = string.Format(ZPVMResources.StringResource.ProductControlObject_Delete_Success
                                                    , key);
                }
                return Json(result);
            }
        }

        public string GetQueryCondition(ProductControlObjectQueryViewModel model)
        {
            StringBuilder where = new StringBuilder();
            if (model != null)
            {
                if (!string.IsNullOrEmpty(model.CellEff))
                {
                    where.AppendFormat(" {0} Key.CellEff = '{1}'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.CellEff);
                }

                if (!string.IsNullOrEmpty(model.ProductCode))
                {
                    where.AppendFormat(" {0} Key.ProductCode = '{1}'"
                                            , where.Length > 0 ? "AND" : string.Empty
                                            , model.ProductCode);
                }
                if (!string.IsNullOrEmpty(model.SupplierCode))
                {
                    where.AppendFormat(" {0} Key.SupplierCode = '{1}'"
                                            , where.Length > 0 ? "AND" : string.Empty
                                            , model.SupplierCode);
                }

                if (!string.IsNullOrEmpty(model.SupplierName))
                {
                    where.AppendFormat(" {0} SupplierName LIKE '{1}%'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.SupplierName);
                }

                if (!string.IsNullOrEmpty(model.ProductName))
                {
                    where.AppendFormat(" {0} ProductName LIKE '{1}%'"
                                        , where.Length > 0 ? "AND" : string.Empty
                                        , model.ProductName);
                }
 
            }
            return where.ToString();
        }
    }
}