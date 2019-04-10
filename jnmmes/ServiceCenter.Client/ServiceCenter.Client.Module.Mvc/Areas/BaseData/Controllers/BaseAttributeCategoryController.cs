using ServiceCenter.Client.Mvc.Areas.BaseData.Models;
using ServiceCenter.Client.Mvc.Resources.BaseData;
using ServiceCenter.MES.Model.BaseData;
using ServiceCenter.MES.Service.Client.BaseData;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ServiceCenter.Client.Mvc.Areas.BaseData.Controllers
{
    public class BaseAttributeCategoryController : Controller
    {
        //
        // GET: /BaseData/BaseAttributeCategory/
        public async Task<ActionResult> Index()
        {
            using (BaseAttributeCategoryServiceClient client = new BaseAttributeCategoryServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = "Key"
                    };
                    MethodReturnResult<IList<BaseAttributeCategory>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }

            return View(new BaseAttributeCategoryQueryViewModel());
        }

        public async Task<ActionResult> IndexOfCK()
        {
            using (BaseAttributeCategoryServiceClient client = new BaseAttributeCategoryServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = "Key",
                        Where = string.Format(@"Key = 'StoreLocation'")
                    };
                    MethodReturnResult<IList<BaseAttributeCategory>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }

            return View(new BaseAttributeCategoryQueryViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> QueryOfCK(BaseAttributeCategoryQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (BaseAttributeCategoryServiceClient client = new BaseAttributeCategoryServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            if (!string.IsNullOrEmpty(model.CategoryName))
                            {
                                where.AppendFormat(" {0} Key LIKE '{1}%' AND (Key = 'StoreLocation')"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.CategoryName);
                            }
                            else
                            {
                                where.AppendFormat(string.Format(@"Key = 'StoreLocation'"));
                            }
                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "Key",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<BaseAttributeCategory>> result = client.Get(ref cfg);

                        if (result.Code == 0)
                        {
                            ViewBag.PagingConfig = cfg;
                            ViewBag.List = result.Data;
                        }
                    });
                }
            }
            return PartialView("_ListPartialOfCK");
        }

        //
        //POST: /BaseData/BaseAttributeCategory/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(BaseAttributeCategoryQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (BaseAttributeCategoryServiceClient client = new BaseAttributeCategoryServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            if (!string.IsNullOrEmpty(model.CategoryName))
                            {
                                where.AppendFormat(" {0} Key LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.CategoryName);
                            }
                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "Key",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<BaseAttributeCategory>> result = client.Get(ref cfg);

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
        //POST: /BaseData/BaseAttributeCategory/PagingQuery
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

                using (BaseAttributeCategoryServiceClient client = new BaseAttributeCategoryServiceClient())
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
                        MethodReturnResult<IList<BaseAttributeCategory>> result = client.Get(ref cfg);
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
        // POST: /BaseData/BaseAttributeCategory/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(BaseAttributeCategoryViewModel model)
        {
            using (BaseAttributeCategoryServiceClient client = new BaseAttributeCategoryServiceClient())
            {
                BaseAttributeCategory obj = new BaseAttributeCategory()
                {
                    Key = model.Name,
                    Description = model.Description,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                    CreateTime = DateTime.Now,
                    Creator = User.Identity.Name
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(StringResource.BaseAttributeCategory_Save_Success
                                                , model.Name);
                }
                return Json(rst);
            }
        }
        //
        // GET: /BaseData/BaseAttributeCategory/Modify
        public async Task<ActionResult> Modify(string name)
        {
            using (BaseAttributeCategoryServiceClient client = new BaseAttributeCategoryServiceClient())
            {
                MethodReturnResult<BaseAttributeCategory> result = await client.GetAsync(name);
                if (result.Code == 0)
                {
                    BaseAttributeCategoryViewModel viewModel = new BaseAttributeCategoryViewModel()
                    {
                        Name = result.Data.Key,
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
        // POST: /BaseData/BaseAttributeCategory/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(BaseAttributeCategoryViewModel model)
        {
            using (BaseAttributeCategoryServiceClient client = new BaseAttributeCategoryServiceClient())
            {
                MethodReturnResult<BaseAttributeCategory> result = await client.GetAsync(model.Name);

                if (result.Code == 0)
                {
                    result.Data.Description = model.Description;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(StringResource.BaseAttributeCategory_SaveModify_Success
                                                    , model.Name);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // GET: /BaseData/BaseAttributeCategory/Detail
        public async Task<ActionResult> Detail(string name)
        {
            using (BaseAttributeCategoryServiceClient client = new BaseAttributeCategoryServiceClient())
            {
                MethodReturnResult<BaseAttributeCategory> result = await client.GetAsync(name);
                if (result.Code == 0)
                {
                    BaseAttributeCategoryViewModel viewModel = new BaseAttributeCategoryViewModel()
                    {
                        Name = result.Data.Key,
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
        // POST: /BaseData/BaseAttributeCategory/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string name)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (BaseAttributeCategoryServiceClient client = new BaseAttributeCategoryServiceClient())
            {
                result = await client.DeleteAsync(name);
                if (result.Code == 0)
                {
                    result.Message = string.Format(StringResource.BaseAttributeCategory_Delete_Success
                                                    ,name);
                }
                return Json(result);
            }
        }

	}
}