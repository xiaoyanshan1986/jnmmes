using ServiceCenter.Client.Mvc.Areas.QAM.Models;
using QAMResources = ServiceCenter.Client.Mvc.Resources.QAM;
using ServiceCenter.MES.Model.QAM;
using ServiceCenter.MES.Service.Client.QAM;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ServiceCenter.Client.Mvc.Areas.QAM.Controllers
{
    public class CheckCategoryController : Controller
    {
        //
        // GET: /QAM/CheckCategory/
        public async Task<ActionResult> Index()
        {
            using (CheckCategoryServiceClient client = new CheckCategoryServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = "Key"
                    };
                    MethodReturnResult<IList<CheckCategory>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new CheckCategoryQueryViewModel());
        }

        //
        //POST: /QAM/CheckCategory/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(CheckCategoryQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (CheckCategoryServiceClient client = new CheckCategoryServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            if (!string.IsNullOrEmpty(model.Name))
                            {
                                where.AppendFormat(" {0} Key LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.Name);
                            }
                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "Key",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<CheckCategory>> result = client.Get(ref cfg);

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
        //POST: /QAM/CheckCategory/PagingQuery
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

                using (CheckCategoryServiceClient client = new CheckCategoryServiceClient())
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
                        MethodReturnResult<IList<CheckCategory>> result = client.Get(ref cfg);
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
        // POST: /QAM/CheckCategory/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(CheckCategoryViewModel model)
        {
            using (CheckCategoryServiceClient client = new CheckCategoryServiceClient())
            {
                CheckCategory obj = new CheckCategory()
                {
                    Key = model.Name.ToUpper(),
                    Status=model.Status,
                    Description = model.Description,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                    CreateTime = DateTime.Now,
                    Creator = User.Identity.Name
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(QAMResources.StringResource.CheckCategory_Save_Success
                                                , model.Name);
                }
                return Json(rst);
            }
        }
        //
        // GET: /QAM/CheckCategory/Modify
        public async Task<ActionResult> Modify(string key)
        {
            CheckCategoryViewModel viewModel = new CheckCategoryViewModel();
            using (CheckCategoryServiceClient client = new CheckCategoryServiceClient())
            {
                MethodReturnResult<CheckCategory> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    viewModel = new CheckCategoryViewModel()
                    {
                        Name = result.Data.Key,
                        Status =result.Data.Status,
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
        // POST: /QAM/CheckCategory/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(CheckCategoryViewModel model)
        {
            using (CheckCategoryServiceClient client = new CheckCategoryServiceClient())
            {
                MethodReturnResult<CheckCategory> result = await client.GetAsync(model.Name);

                if (result.Code == 0)
                {
                    result.Data.Status = model.Status;
                    result.Data.Description = model.Description;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(QAMResources.StringResource.CheckCategory_SaveModify_Success
                                                    , model.Name);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // GET: /QAM/CheckCategory/Detail
        public async Task<ActionResult> Detail(string key)
        {
            using (CheckCategoryServiceClient client = new CheckCategoryServiceClient())
            {
                MethodReturnResult<CheckCategory> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    CheckCategoryViewModel viewModel = new CheckCategoryViewModel()
                    {
                        Name = result.Data.Key,
                        Status=result.Data.Status,
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
        // POST: /QAM/CheckCategory/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (CheckCategoryServiceClient client = new CheckCategoryServiceClient())
            {
                result = await client.DeleteAsync(key);
                if (result.Code == 0)
                {
                    result.Message = string.Format(QAMResources.StringResource.CheckCategory_Delete_Success
                                                    , key);
                }
                return Json(result);
            }
        }
    }
}