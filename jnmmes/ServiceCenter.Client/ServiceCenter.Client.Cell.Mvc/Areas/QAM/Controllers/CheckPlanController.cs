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
    public class CheckPlanController : Controller
    {
        //
        // GET: /QAM/CheckPlan/
        public async Task<ActionResult> Index()
        {
            using (CheckPlanServiceClient client = new CheckPlanServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = "Key"
                    };
                    MethodReturnResult<IList<CheckPlan>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new CheckPlanQueryViewModel());
        }

        //
        //POST: /QAM/CheckPlan/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(CheckPlanQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (CheckPlanServiceClient client = new CheckPlanServiceClient())
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
                        MethodReturnResult<IList<CheckPlan>> result = client.Get(ref cfg);

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
        //POST: /QAM/CheckPlan/PagingQuery
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

                using (CheckPlanServiceClient client = new CheckPlanServiceClient())
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
                        MethodReturnResult<IList<CheckPlan>> result = client.Get(ref cfg);
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
        // POST: /QAM/CheckPlan/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(CheckPlanViewModel model)
        {
            using (CheckPlanServiceClient client = new CheckPlanServiceClient())
            {
                CheckPlan obj = new CheckPlan()
                {
                    Key = model.Name.ToUpper(),
                    Status=model.Status,
                    Size=model.Size,
                    Mode=model.Mode,
                    Type=model.Type,
                    Description = model.Description,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                    CreateTime = DateTime.Now,
                    Creator = User.Identity.Name
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(QAMResources.StringResource.CheckPlan_Save_Success
                                                , model.Name);
                }
                return Json(rst);
            }
        }
        //
        // GET: /QAM/CheckPlan/Modify
        public async Task<ActionResult> Modify(string key)
        {
            CheckPlanViewModel viewModel = new CheckPlanViewModel();
            using (CheckPlanServiceClient client = new CheckPlanServiceClient())
            {
                MethodReturnResult<CheckPlan> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    viewModel = new CheckPlanViewModel()
                    {
                        Name = result.Data.Key,
                        Mode=result.Data.Mode,
                        Type=result.Data.Type,
                        Size=result.Data.Size,
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
        // POST: /QAM/CheckPlan/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(CheckPlanViewModel model)
        {
            using (CheckPlanServiceClient client = new CheckPlanServiceClient())
            {
                MethodReturnResult<CheckPlan> result = await client.GetAsync(model.Name);

                if (result.Code == 0)
                {
                    result.Data.Mode = model.Mode;
                    result.Data.Type = model.Type;
                    result.Data.Size = model.Size;
                    result.Data.Status = model.Status;
                    result.Data.Description = model.Description;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(QAMResources.StringResource.CheckPlan_SaveModify_Success
                                                    , model.Name);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // GET: /QAM/CheckPlan/Detail
        public async Task<ActionResult> Detail(string key)
        {
            using (CheckPlanServiceClient client = new CheckPlanServiceClient())
            {
                MethodReturnResult<CheckPlan> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    CheckPlanViewModel viewModel = new CheckPlanViewModel()
                    {
                        Name = result.Data.Key,
                        Size=result.Data.Size,
                        Type=result.Data.Type,
                        Mode=result.Data.Mode,
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
        // POST: /QAM/CheckPlan/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (CheckPlanServiceClient client = new CheckPlanServiceClient())
            {
                result = await client.DeleteAsync(key);
                if (result.Code == 0)
                {
                    result.Message = string.Format(QAMResources.StringResource.CheckPlan_Delete_Success
                                                    , key);
                }
                return Json(result);
            }
        }
    }
}