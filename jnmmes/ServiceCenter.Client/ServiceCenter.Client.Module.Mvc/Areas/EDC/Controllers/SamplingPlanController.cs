using ServiceCenter.Client.Mvc.Areas.EDC.Models;
using EDCResources = ServiceCenter.Client.Mvc.Resources.EDC;
using ServiceCenter.MES.Model.EDC;
using ServiceCenter.MES.Service.Client.EDC;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ServiceCenter.Client.Mvc.Areas.EDC.Controllers
{
    public class SamplingPlanController : Controller
    {
        //
        // GET: /EDC/SamplingPlan/
        public async Task<ActionResult> Index()
        {
            using (SamplingPlanServiceClient client = new SamplingPlanServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = "Key"
                    };
                    MethodReturnResult<IList<SamplingPlan>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new SamplingPlanQueryViewModel());
        }

        //
        //POST: /EDC/SamplingPlan/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(SamplingPlanQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (SamplingPlanServiceClient client = new SamplingPlanServiceClient())
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
                        MethodReturnResult<IList<SamplingPlan>> result = client.Get(ref cfg);

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
        //POST: /EDC/SamplingPlan/PagingQuery
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

                using (SamplingPlanServiceClient client = new SamplingPlanServiceClient())
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
                        MethodReturnResult<IList<SamplingPlan>> result = client.Get(ref cfg);
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
        // POST: /EDC/SamplingPlan/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(SamplingPlanViewModel model)
        {
            using (SamplingPlanServiceClient client = new SamplingPlanServiceClient())
            {
                SamplingPlan obj = new SamplingPlan()
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
                    rst.Message = string.Format(EDCResources.StringResource.SamplingPlan_Save_Success
                                                , model.Name);
                }
                return Json(rst);
            }
        }
        //
        // GET: /EDC/SamplingPlan/Modify
        public async Task<ActionResult> Modify(string key)
        {
            SamplingPlanViewModel viewModel = new SamplingPlanViewModel();
            using (SamplingPlanServiceClient client = new SamplingPlanServiceClient())
            {
                MethodReturnResult<SamplingPlan> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    viewModel = new SamplingPlanViewModel()
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
        // POST: /EDC/SamplingPlan/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(SamplingPlanViewModel model)
        {
            using (SamplingPlanServiceClient client = new SamplingPlanServiceClient())
            {
                MethodReturnResult<SamplingPlan> result = await client.GetAsync(model.Name);

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
                        rst.Message = string.Format(EDCResources.StringResource.SamplingPlan_SaveModify_Success
                                                    , model.Name);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // GET: /EDC/SamplingPlan/Detail
        public async Task<ActionResult> Detail(string key)
        {
            using (SamplingPlanServiceClient client = new SamplingPlanServiceClient())
            {
                MethodReturnResult<SamplingPlan> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    SamplingPlanViewModel viewModel = new SamplingPlanViewModel()
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
        // POST: /EDC/SamplingPlan/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (SamplingPlanServiceClient client = new SamplingPlanServiceClient())
            {
                result = await client.DeleteAsync(key);
                if (result.Code == 0)
                {
                    result.Message = string.Format(EDCResources.StringResource.SamplingPlan_Delete_Success
                                                    , key);
                }
                return Json(result);
            }
        }
    }
}