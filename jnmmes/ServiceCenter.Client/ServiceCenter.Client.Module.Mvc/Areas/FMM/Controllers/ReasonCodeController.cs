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
    public class ReasonCodeController : Controller
    {
        //
        // GET: /FMM/ReasonCode/
        public ActionResult Index()
        {
            using (ReasonCodeServiceClient client = new ReasonCodeServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo=1,
                    OrderBy = "Key"
                };
                MethodReturnResult<IList<ReasonCode>> result = client.Get(ref cfg);

                if (result.Code == 0)
                {
                    ViewBag.PagingConfig = cfg;
                    ViewBag.List = result.Data;
                }
            }
            return View(new ReasonCodeQueryViewModel());
        }

        //
        //POST: /FMM/ReasonCode/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Query(ReasonCodeQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (ReasonCodeServiceClient client = new ReasonCodeServiceClient())
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
                        if (model.Type != null)
                        {
                            where.AppendFormat(" {0} Type = '{1}'"
                                                , where.Length > 0 ? "AND" : string.Empty
                                                , Convert.ToInt32(model.Type));
                        }
                    }
                    PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = "Key",
                        Where = where.ToString()
                    };
                    MethodReturnResult<IList<ReasonCode>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                }
            }
            return PartialView("_ListPartial");
        }
        //
        //POST: /FMM/ReasonCode/PagingQuery
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PagingQuery(string where, string orderBy, int? currentPageNo, int? currentPageSize)
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

                using (ReasonCodeServiceClient client = new ReasonCodeServiceClient())
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        PageNo = pageNo,
                        PageSize = pageSize,
                        Where = where ?? string.Empty,
                        OrderBy = orderBy ?? string.Empty
                    };
                    MethodReturnResult<IList<ReasonCode>> result = client.Get(ref cfg);
                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                }
            }
            return PartialView("_ListPartial");
        }
        //
        // POST: /FMM/ReasonCode/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(ReasonCodeViewModel model)
        {
            using (ReasonCodeServiceClient client = new ReasonCodeServiceClient())
            {
                ReasonCode obj = new ReasonCode()
                {
                    Key = model.Name,
                    Type = model.Type,
                    Class=model.Class,
                    Status = EnumObjectStatus.Available,
                    Description = model.Description,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                    CreateTime = DateTime.Now,
                    Creator = User.Identity.Name
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(FMMResources.StringResource.ReasonCode_Save_Success
                                                , model.Name);
                }
                return Json(rst);
            }
        }
        //
        // GET: /FMM/ReasonCode/Modify
        public async Task<ActionResult> Modify(string key)
        {
            ReasonCodeViewModel viewModel = new ReasonCodeViewModel();
            using (ReasonCodeServiceClient client = new ReasonCodeServiceClient())
            {
                MethodReturnResult<ReasonCode> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    viewModel = new ReasonCodeViewModel()
                    {
                        Name = result.Data.Key,
                        Type = result.Data.Type,
                        Class=result.Data.Class,
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
        // POST: /FMM/ReasonCode/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(ReasonCodeViewModel model)
        {
            using (ReasonCodeServiceClient client = new ReasonCodeServiceClient())
            {
                MethodReturnResult<ReasonCode> result = await client.GetAsync(model.Name);

                if (result.Code == 0)
                {
                    result.Data.Class = model.Class;
                    result.Data.Type = model.Type;
                    result.Data.Description = model.Description;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(FMMResources.StringResource.ReasonCode_SaveModify_Success
                                                    , model.Name);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // GET: /FMM/ReasonCode/Detail
        public async Task<ActionResult> Detail(string key)
        {
            using (ReasonCodeServiceClient client = new ReasonCodeServiceClient())
            {
                MethodReturnResult<ReasonCode> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    ReasonCodeViewModel viewModel = new ReasonCodeViewModel()
                    {
                        Name = result.Data.Key,
                        Type = result.Data.Type,
                        Class=result.Data.Class,
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
        // POST: /FMM/ReasonCode/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (ReasonCodeServiceClient client = new ReasonCodeServiceClient())
            {
                result = await client.DeleteAsync(key);
                if (result.Code == 0)
                {
                    result.Message = string.Format(FMMResources.StringResource.ReasonCode_Delete_Success
                                                    , key);
                }
                return Json(result);
            }
        }
    }
}