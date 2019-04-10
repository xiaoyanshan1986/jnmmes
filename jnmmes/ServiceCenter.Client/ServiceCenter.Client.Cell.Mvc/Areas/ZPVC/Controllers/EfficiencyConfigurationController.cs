using ServiceCenter.Client.Mvc.Areas.ZPVC.Models;
using ZPVCResources = ServiceCenter.Client.Mvc.Resources.ZPVC;
using ServiceCenter.MES.Model.ZPVC;
using ServiceCenter.MES.Service.Client.ZPVC;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ServiceCenter.Client.Mvc.Areas.ZPVC.Controllers
{
    public class EfficiencyConfigurationController : Controller
    {
        //
        // GET: /ZPVC/EfficiencyConfiguration/
        public async Task<ActionResult> Index()
        {
            return await Query(new EfficiencyConfigurationQueryViewModel());
        }

        //
        //POST: /ZPVC/EfficiencyConfiguration/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(EfficiencyConfigurationQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (EfficiencyConfigurationServiceClient client = new EfficiencyConfigurationServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            if (!string.IsNullOrEmpty(model.Group))
                            {
                                where.AppendFormat(" {0} Key.Group LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.Group);
                            }
                            if (!string.IsNullOrEmpty(model.Code))
                            {
                                where.AppendFormat(" {0} Key.Code LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.Code);
                            }

                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "Key",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<EfficiencyConfiguration>> result = client.Get(ref cfg);

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
                return PartialView("_ListPartial",new EfficiencyConfigurationViewModel());
            }
            else
            {
                return View("Index", model);
            }
        }
        //
        //POST: /ZPVC/EfficiencyConfiguration/PagingQuery
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

                using (EfficiencyConfigurationServiceClient client = new EfficiencyConfigurationServiceClient())
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
                        MethodReturnResult<IList<EfficiencyConfiguration>> result = client.Get(ref cfg);
                        if (result.Code == 0)
                        {
                            ViewBag.PagingConfig = cfg;
                            ViewBag.List = result.Data;
                        }
                    });
                }
            }
            return PartialView("_ListPartial", new EfficiencyConfigurationViewModel());
        }
        //
        // POST: /ZPVC/EfficiencyConfiguration/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(EfficiencyConfigurationViewModel model)
        {
            using (EfficiencyConfigurationServiceClient client = new EfficiencyConfigurationServiceClient())
            {
                EfficiencyConfiguration obj = new EfficiencyConfiguration()
                {
                    Key = new EfficiencyConfigurationKey(){
                        Code = model.Code.ToUpper(),
                        Group = model.Group.ToUpper()
                    },
                    Code=model.EffiCode,
                    Name = model.EffiName,
                    Lower=model.Lower,
                    Upper=model.Upper,
                    Description = model.Description,
                    Color=model.Color,
                    Grade=model.Grade,
                    MaterialCode=model.MaterialCode,
                    IsUsed=model.IsUsed,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                    CreateTime = DateTime.Now,
                    Creator = User.Identity.Name
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(ZPVCResources.StringResource.EfficiencyConfiguration_Save_Success
                                                , obj.Key);
                }
                return Json(rst);
            }
        }
        //
        // GET: /ZPVC/EfficiencyConfiguration/Modify
        public async Task<ActionResult> Modify(string group, string code)
        {
            EfficiencyConfigurationViewModel viewModel = new EfficiencyConfigurationViewModel();
            using (EfficiencyConfigurationServiceClient client = new EfficiencyConfigurationServiceClient())
            {
                MethodReturnResult<EfficiencyConfiguration> result = await client.GetAsync(new EfficiencyConfigurationKey()
                {
                    Code=code,
                    Group=group
                });
                if (result.Code == 0)
                {
                    viewModel = new EfficiencyConfigurationViewModel()
                    {
                        Code=result.Data.Key.Code,
                        Group = result.Data.Key.Group,
                        Description=result.Data.Description,
                        EffiCode=result.Data.Code,
                        EffiName = result.Data.Name,
                        Lower=result.Data.Lower,
                        Upper=result.Data.Upper,
                        Color=result.Data.Color,
                        Grade=result.Data.Grade,
                        MaterialCode=result.Data.MaterialCode,
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
            return PartialView("_ModifyPartial");
        }

        //
        // POST: /ZPVC/EfficiencyConfiguration/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(EfficiencyConfigurationViewModel model)
        {
            using (EfficiencyConfigurationServiceClient client = new EfficiencyConfigurationServiceClient())
            {
                EfficiencyConfigurationKey key = new EfficiencyConfigurationKey()
                {
                    Code = model.Code,
                    Group=model.Group
                };
                MethodReturnResult<EfficiencyConfiguration> result = await client.GetAsync(key);

                if (result.Code == 0)
                {
                    result.Data.Lower = model.Lower;
                    result.Data.Upper = model.Upper;
                    result.Data.Code = model.EffiCode;
                    result.Data.Name = model.EffiName;
                    result.Data.Color = model.Color;
                    result.Data.Grade = model.Grade;
                    result.Data.MaterialCode = model.MaterialCode;
                    result.Data.Description = model.Description;
                    result.Data.IsUsed = model.IsUsed;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(ZPVCResources.StringResource.EfficiencyConfiguration_SaveModify_Success
                                                    , key);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // GET: /ZPVC/EfficiencyConfiguration/Detail
        public async Task<ActionResult> Detail(string group, string code)
        {
            using (EfficiencyConfigurationServiceClient client = new EfficiencyConfigurationServiceClient())
            {
                EfficiencyConfigurationKey key = new EfficiencyConfigurationKey()
                {
                    Code = code,
                    Group=group
                };
                MethodReturnResult<EfficiencyConfiguration> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    EfficiencyConfigurationViewModel viewModel = new EfficiencyConfigurationViewModel()
                    {
                        Description = result.Data.Description,
                        IsUsed =result.Data.IsUsed,
                        Group=result.Data.Key.Group,
                        EffiName=result.Data.Name,
                        EffiCode=result.Data.Code,
                        Lower=result.Data.Lower,
                        Upper=result.Data.Upper,
                        Color=result.Data.Color,
                        MaterialCode=result.Data.MaterialCode,
                        Grade=result.Data.Grade,
                        Code=result.Data.Key.Code,
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
        // POST: /ZPVC/EfficiencyConfiguration/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string group, string code)
        {
            MethodReturnResult result = new MethodReturnResult();
            EfficiencyConfigurationKey key = new EfficiencyConfigurationKey()
            {
                Code = code,
                Group = group
            };
            using (EfficiencyConfigurationServiceClient client = new EfficiencyConfigurationServiceClient())
            {
                result = await client.DeleteAsync(key);
                if (result.Code == 0)
                {
                    result.Message = string.Format(ZPVCResources.StringResource.EfficiencyConfiguration_Delete_Success
                                                    , key);
                }
                return Json(result);
            }
        }
    }
}