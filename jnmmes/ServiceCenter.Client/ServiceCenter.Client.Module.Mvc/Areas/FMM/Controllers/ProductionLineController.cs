using ServiceCenter.Client.Mvc.Areas.FMM.Models;
using FMMResources = ServiceCenter.Client.Mvc.Resources.FMM;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ServiceCenter.Client.Mvc.Areas.FMM.Controllers
{
    public class ProductionLineController : Controller
    {
        //
        // GET: /FMM/ProductionLine/
        public async Task<ActionResult> Index()
        {
            using (ProductionLineServiceClient client = new ProductionLineServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = "Key"
                    };
                    MethodReturnResult<IList<ProductionLine>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new ProductionLineQueryViewModel());
        }

        //
        //POST: /FMM/ProductionLine/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(ProductionLineQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (ProductionLineServiceClient client = new ProductionLineServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            if (!string.IsNullOrEmpty(model.Code))
                            {
                                where.AppendFormat(" {0} Key LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.Code);
                            }

                            if (!string.IsNullOrEmpty(model.Name))
                            {
                                where.AppendFormat(" {0} Name LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.Name);
                            }

                            if (!string.IsNullOrEmpty(model.LocationName))
                            {
                                where.AppendFormat(" {0} LocationName LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.LocationName);
                            }
                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "Key",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<ProductionLine>> result = client.Get(ref cfg);

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
        //POST: /FMM/ProductionLine/PagingQuery
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

                using (ProductionLineServiceClient client = new ProductionLineServiceClient())
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
                        MethodReturnResult<IList<ProductionLine>> result = client.Get(ref cfg);
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
        // POST: /FMM/ProductionLine/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(ProductionLineViewModel model)
        {
            using (ProductionLineServiceClient client = new ProductionLineServiceClient())
            {
                ProductionLine obj = new ProductionLine()
                {
                    Key = model.Code,
                    Name=model.Name,
                    LocationName = model.LocationName,
                    Description = model.Description,
                    Attr2=model.BinNo,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                    CreateTime = DateTime.Now,
                    Creator = User.Identity.Name
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(FMMResources.StringResource.ProductionLine_Save_Success
                                                , model.Code);
                }
                return Json(rst);
            }
        }
        //
        // GET: /FMM/ProductionLine/Modify
        public async Task<ActionResult> Modify(string key)
        {
            ProductionLineViewModel viewModel = new ProductionLineViewModel();
            using (ProductionLineServiceClient client = new ProductionLineServiceClient())
            {
                MethodReturnResult<ProductionLine> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    viewModel = new ProductionLineViewModel()
                    {
                        Code = result.Data.Key,
                        Name=result.Data.Name,
                        LocationName=result.Data.LocationName,
                        BinNo=result.Data.Attr2,
                        CreateTime = result.Data.CreateTime,
                        Creator = result.Data.Creator,
                        Description = result.Data.Description,
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
            return PartialView("_ModifyPartial");
        }

        //
        // POST: /FMM/ProductionLine/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(ProductionLineViewModel model)
        {
            using (ProductionLineServiceClient client = new ProductionLineServiceClient())
            {
                MethodReturnResult<ProductionLine> result = await client.GetAsync(model.Code);

                if (result.Code == 0)
                {
                    result.Data.Name = model.Name;
                    result.Data.LocationName = model.LocationName;
                    result.Data.Attr2 = model.BinNo;
                    result.Data.Description = model.Description;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(FMMResources.StringResource.ProductionLine_SaveModify_Success
                                                    , model.Code);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // GET: /FMM/ProductionLine/Detail
        public async Task<ActionResult> Detail(string key)
        {
            using (ProductionLineServiceClient client = new ProductionLineServiceClient())
            {
                MethodReturnResult<ProductionLine> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    ProductionLineViewModel viewModel = new ProductionLineViewModel()
                    {
                        Code = result.Data.Key,
                        Name = result.Data.Name,
                        LocationName = result.Data.LocationName,
                        BinNo=result.Data.Attr2,
                        CreateTime = result.Data.CreateTime,
                        Creator = result.Data.Creator,
                        Description = result.Data.Description,
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
        // POST: /FMM/ProductionLine/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (ProductionLineServiceClient client = new ProductionLineServiceClient())
            {
                result = await client.DeleteAsync(key);
                if (result.Code == 0)
                {
                    result.Message = string.Format(FMMResources.StringResource.ProductionLine_Delete_Success
                                                    , key);
                }
                return Json(result);
            }
        }
    }
}