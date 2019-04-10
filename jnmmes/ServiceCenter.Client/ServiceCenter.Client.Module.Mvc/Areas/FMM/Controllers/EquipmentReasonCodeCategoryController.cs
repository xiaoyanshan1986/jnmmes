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
    public class EquipmentReasonCodeCategoryController : Controller
    {
        //
        // GET: /FMM/ReasonCodeCategory/
        public ActionResult Index()
        {
            using (EquipmentReasonCodeCategoryServiceClient client = new EquipmentReasonCodeCategoryServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    OrderBy = "Key"
                };
                MethodReturnResult<IList<EquipmentReasonCodeCategory>> result = client.Get(ref cfg);

                if (result.Code == 0)
                {
                    ViewBag.PagingConfig = cfg;
                    ViewBag.List = result.Data;
                }
            }
            return View(new EquipmentReasonCodeCategoryQueryViewModel());
        }

        //
        //POST: /FMM/ReasonCodeCategory/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Query(EquipmentReasonCodeCategoryQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (EquipmentReasonCodeCategoryServiceClient client = new EquipmentReasonCodeCategoryServiceClient())
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
                    MethodReturnResult<IList<EquipmentReasonCodeCategory>> result = client.Get(ref cfg);

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
        //POST: /FMM/ReasonCodeCategory/PagingQuery
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

                using (EquipmentReasonCodeCategoryServiceClient client = new EquipmentReasonCodeCategoryServiceClient())
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        PageNo = pageNo,
                        PageSize = pageSize,
                        Where = where ?? string.Empty,
                        OrderBy = orderBy ?? string.Empty
                    };
                    MethodReturnResult<IList<EquipmentReasonCodeCategory>> result = client.Get(ref cfg);
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
        // POST: /FMM/ReasonCodeCategory/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(EquipmentReasonCodeCategoryViewModel model)
        {
            using (EquipmentReasonCodeCategoryServiceClient client = new EquipmentReasonCodeCategoryServiceClient())
            {
                EquipmentReasonCodeCategory obj = new EquipmentReasonCodeCategory()
                {
                    Key = model.Name,
                    Type = model.Type,
                    Description = model.Description,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                    CreateTime = DateTime.Now,
                    Creator = User.Identity.Name
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(FMMResources.StringResource.ReasonCodeCategory_Save_Success
                                                , model.Name);
                }
                return Json(rst);
            }
        }
        //
        // GET: /FMM/ReasonCodeCategory/Modify
        public async Task<ActionResult> Modify(string key)
        {
            EquipmentReasonCodeCategoryViewModel viewModel = new EquipmentReasonCodeCategoryViewModel();
            using (EquipmentReasonCodeCategoryServiceClient client = new EquipmentReasonCodeCategoryServiceClient())
            {
                MethodReturnResult<EquipmentReasonCodeCategory> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    viewModel = new EquipmentReasonCodeCategoryViewModel()
                    {
                        Name = result.Data.Key,
                        Type = result.Data.Type,
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
        // POST: /FMM/ReasonCodeCategory/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(EquipmentReasonCodeCategoryViewModel model)
        {
            using (EquipmentReasonCodeCategoryServiceClient client = new EquipmentReasonCodeCategoryServiceClient())
            {
                MethodReturnResult<EquipmentReasonCodeCategory> result = await client.GetAsync(model.Name);

                if (result.Code == 0)
                {
                    result.Data.Type = model.Type;
                    result.Data.Description = model.Description;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(FMMResources.StringResource.ReasonCodeCategory_SaveModify_Success
                                                    , model.Name);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // GET: /FMM/ReasonCodeCategory/Detail
        public async Task<ActionResult> Detail(string key)
        {
            using (EquipmentReasonCodeCategoryServiceClient client = new EquipmentReasonCodeCategoryServiceClient())
            {
                MethodReturnResult<EquipmentReasonCodeCategory> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    EquipmentReasonCodeCategoryViewModel viewModel = new EquipmentReasonCodeCategoryViewModel()
                    {
                        Name = result.Data.Key,
                        Type = result.Data.Type,
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
        // POST: /FMM/ReasonCodeCategory/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (EquipmentReasonCodeCategoryServiceClient client = new EquipmentReasonCodeCategoryServiceClient())
            {
                result = await client.DeleteAsync(key);
                if (result.Code == 0)
                {
                    result.Message = string.Format(FMMResources.StringResource.ReasonCodeCategory_Delete_Success
                                                    , key);
                }
                return Json(result);
            }
        }
    }
}