using ServiceCenter.Client.Mvc.Areas.RBAC.Models;
using ServiceCenter.Client.Mvc.Resources.RBAC;
using ServiceCenter.Common;
using ServiceCenter.MES.Model.RBAC;
using ServiceCenter.MES.Service.Client.RBAC;
using ServiceCenter.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ServiceCenter.Client.Mvc.Areas.RBAC.Controllers
{
    [Authorize]
    public class ResourceController : Controller
    {
        //
        // GET: /RBAC/Resource/
        public async Task<ActionResult> Index()
        {
            using (ResourceServiceClient client = new ResourceServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig() { 
                        OrderBy="Key.Code"
                    };
                    MethodReturnResult<IList<Resource>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.ResourceList = result.Data;
                    }
                });
            }

            return View(new ResourceQueryViewModel());
        }
        //
        //POST: /RBAC/Resource/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(ResourceQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (ResourceServiceClient client = new ResourceServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            if (model.Type!=null)
                            {
                                where.AppendFormat("Key.Type = {0}", Convert.ToInt32(model.Type));
                            }
                            if (!string.IsNullOrEmpty(model.Code))
                            {
                                where.AppendFormat(" {0} Key.Code LIKE '{1}%'"
                                                    , where.Length>0?"AND":string.Empty
                                                    , model.Code);
                            }
                            if (!string.IsNullOrEmpty(model.Name))
                            {
                                where.AppendFormat(" {0} Name LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.Name);
                            }
                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy="Key.Code",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<Resource>> result = client.Get(ref cfg);

                        if (result.Code == 0)
                        {
                            ViewBag.PagingConfig = cfg;
                            ViewBag.ResourceList = result.Data;
                        }
                    });
                }
            }
            return PartialView("_ResourceListPartial");
        }
        //
        //POST: /RBAC/Resource/PagingQuery
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

                using (ResourceServiceClient client = new ResourceServiceClient())
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
                        MethodReturnResult<IList<Resource>> result = client.Get(ref cfg);
                        if (result.Code == 0)
                        {
                            ViewBag.PagingConfig = cfg;
                            ViewBag.ResourceList = result.Data;
                        }
                    });
                }
            }
            return PartialView("_ResourceListPartial");
        }
        //
        // POST: /RBAC/Resource/SaveResource
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveResource(ResourceViewModel model)
        {
            using (ResourceServiceClient client = new ResourceServiceClient())
            {
                Resource obj = new Resource()
                {
                    Key = new ResourceKey()
                    {
                        Code=model.Code,
                        Type=model.Type
                    },
                    Data=model.Data,
                    Name=model.Name,
                    Description = model.Description,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                    CreateTime = DateTime.Now,
                    Creator = User.Identity.Name
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(StringResource.Resource_SaveResource_Success
                                                ,model.Type.GetDisplayName()
                                                ,model.Code);
                }
                return Json(rst);
            }
        }
        //
        // GET: /RBAC/Resource/Modify
        public async Task<ActionResult> Modify(ResourceType? type,string code)
        {
            if (type == null)
            {
                ModelState.AddModelError("", StringResource.Resource_Modify_ParamterError);
            }
            else
            {
                using (ResourceServiceClient client = new ResourceServiceClient())
                {
                    ResourceKey key = new ResourceKey() { 
                        Code=code,
                        Type=type.Value
                    };

                    MethodReturnResult<Resource> result = await client.GetAsync(key);
                    if (result.Code == 0)
                    {
                        ResourceViewModel viewModel = new ResourceViewModel()
                        {
                            Type=result.Data.Key.Type,
                            Code=result.Data.Key.Code,
                            Data=result.Data.Data,
                            Name=result.Data.Name,
                            CreateTime = result.Data.CreateTime,
                            Creator = result.Data.Creator,
                            Description = result.Data.Description,
                            Editor = result.Data.Editor,
                            EditTime = result.Data.EditTime
                        };
                        return PartialView("_ModifyResourcePartial", viewModel);
                    }
                    else
                    {
                        ModelState.AddModelError("", result.Message);
                    }
                }
            }
            return PartialView("_ModifyResourcePartial");
        }
        //
        // POST: /RBAC/Resource/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(ResourceViewModel model)
        {
            using (ResourceServiceClient client = new ResourceServiceClient())
            {
                MethodReturnResult<Resource> result = await client.GetAsync(new ResourceKey()
                {
                     Type=model.Type,
                     Code=model.Code
                });

                if (result.Code == 0)
                {
                    result.Data.Data = model.Data;
                    result.Data.Name = model.Name;
                    result.Data.Description = model.Description;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(StringResource.Resource_SaveModify_Success
                                                    , model.Type.GetDisplayName()
                                                    , model.Code);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // GET: /RBAC/Resource/Detail
        public async Task<ActionResult> Detail(ResourceType? type, string code)
        {
            if (type == null)
            {
                ModelState.AddModelError("", StringResource.Resource_Detail_ParamterError);
            }
            else
            {
                using (ResourceServiceClient client = new ResourceServiceClient())
                {
                    ResourceKey key = new ResourceKey()
                    {
                        Code = code,
                        Type = type.Value
                    };

                    MethodReturnResult<Resource> result = await client.GetAsync(key);
                    if (result.Code == 0)
                    {
                        ResourceViewModel viewModel = new ResourceViewModel()
                        {
                            Type = result.Data.Key.Type,
                            Code = result.Data.Key.Code,
                            Data = result.Data.Data,
                            Name = result.Data.Name,
                            CreateTime = result.Data.CreateTime,
                            Creator = result.Data.Creator,
                            Description = result.Data.Description,
                            Editor = result.Data.Editor,
                            EditTime = result.Data.EditTime
                        };
                        return PartialView("_ResourceInfoPartial", viewModel);
                    }
                    else
                    {
                        ModelState.AddModelError("", result.Message);
                    }
                }
            }
            return PartialView("_ResourceInfoPartial");
        }
        //
        // POST: /RBAC/Resource/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(ResourceType? type, string code)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (type == null)
            {
                result.Code = 1000;
                result.Message = string.Format(StringResource.Resource_Delete_ParamterError);
                return Json(result);
            }

            using (ResourceServiceClient client = new ResourceServiceClient())
            {
                ResourceKey key = new ResourceKey()
                {
                    Code = code,
                    Type = type.Value
                };
                result = await client.DeleteAsync(key);
                if (result.Code == 0)
                {
                    result.Message = string.Format(StringResource.Resource_Delete_Success
                                                    ,type.GetDisplayName()
                                                    ,code);
                }
                return Json(result);
            }
        }

    }
}