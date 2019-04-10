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
    public class RouteOperationAttributeController : Controller
    {

        //
        // GET: /FMM/RouteOperationAttribute/
        public async Task<ActionResult> Index(string routeOperationName)
        {
            using (RouteOperationServiceClient client = new RouteOperationServiceClient())
            {
                MethodReturnResult<RouteOperation> result = await client.GetAsync(routeOperationName ?? string.Empty);
                if (result.Code > 0 || result.Data == null)
                {
                    return RedirectToAction("Index", "RouteOperation");
                }
                ViewBag.RouteOperation = result.Data;
            }

            using (RouteOperationAttributeServiceClient client = new RouteOperationAttributeServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        Where = string.Format(" Key.RouteOperationName = '{0}'"
                                                    , routeOperationName),
                        OrderBy = "Key.RouteOperationName,Key.AttributeName"
                    };
                    MethodReturnResult<IList<RouteOperationAttribute>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new RouteOperationAttributeQueryViewModel() { RouteOperationName=routeOperationName });
        }

        //
        //POST: /FMM/RouteOperationAttribute/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(RouteOperationAttributeQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (RouteOperationAttributeServiceClient client = new RouteOperationAttributeServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            where.AppendFormat(" {0} Key.RouteOperationName = '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.RouteOperationName);

                            if (!string.IsNullOrEmpty(model.AttributeName))
                            {
                                where.AppendFormat(" {0} Key.AttributeName LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.AttributeName);
                            }
                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "Key",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<RouteOperationAttribute>> result = client.Get(ref cfg);

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
        //POST: /FMM/RouteOperationAttribute/PagingQuery
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

                using (RouteOperationAttributeServiceClient client = new RouteOperationAttributeServiceClient())
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
                        MethodReturnResult<IList<RouteOperationAttribute>> result = client.Get(ref cfg);
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
        // POST: /FMM/RouteOperationAttribute/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(RouteOperationAttributeViewModel model)
        {
            using (RouteOperationAttributeServiceClient client = new RouteOperationAttributeServiceClient())
            {
                RouteOperationAttribute obj = new RouteOperationAttribute()
                {
                    Key = new RouteOperationAttributeKey() { 
                         RouteOperationName=model.RouteOperationName,
                         AttributeName=model.AttributeName
                    },
                    Value=model.Value,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(FMMResources.StringResource.RouteOperationAttribute_Save_Success
                                                , obj.Key);
                }
                return Json(rst);
            }
        }
        //
        // GET: /FMM/RouteOperationAttribute/Modify
        public async Task<ActionResult> Modify(string routeOperationName,string attributeName)
        {
            RouteOperationAttributeViewModel viewModel = new RouteOperationAttributeViewModel();
            using (RouteOperationAttributeServiceClient client = new RouteOperationAttributeServiceClient())
            {
                MethodReturnResult<RouteOperationAttribute> result = await client.GetAsync(new RouteOperationAttributeKey()
                {
                    RouteOperationName=routeOperationName,
                    AttributeName=attributeName
                });
                if (result.Code == 0)
                {
                    viewModel = new RouteOperationAttributeViewModel()
                    {
                        RouteOperationName = result.Data.Key.RouteOperationName,
                        AttributeName = result.Data.Key.AttributeName,
                        Value = result.Data.Value,
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
        // POST: /FMM/RouteOperationAttribute/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(RouteOperationAttributeViewModel model)
        {
            using (RouteOperationAttributeServiceClient client = new RouteOperationAttributeServiceClient())
            {
                MethodReturnResult<RouteOperationAttribute> result = await client.GetAsync(new RouteOperationAttributeKey()
                {
                    RouteOperationName = model.RouteOperationName,
                    AttributeName = model.AttributeName
                });

                if (result.Code == 0)
                {
                    result.Data.Value = model.Value;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(FMMResources.StringResource.RouteOperationAttribute_SaveModify_Success
                                                    , result.Data.Key);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // GET: /FMM/RouteOperationAttribute/Detail
        public async Task<ActionResult> Detail(string routeOperationName, string attributeName)
        {
            using (RouteOperationAttributeServiceClient client = new RouteOperationAttributeServiceClient())
            {
                MethodReturnResult<RouteOperationAttribute> result = await client.GetAsync(new RouteOperationAttributeKey()
                {
                    RouteOperationName = routeOperationName,
                    AttributeName = attributeName
                });
                if (result.Code == 0)
                {
                    RouteOperationAttributeViewModel viewModel = new RouteOperationAttributeViewModel()
                    {
                        RouteOperationName = result.Data.Key.RouteOperationName,
                        AttributeName = result.Data.Key.AttributeName,
                        Value = result.Data.Value,
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
        // POST: /FMM/RouteOperationAttribute/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string routeOperationName, string attributeName)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (RouteOperationAttributeServiceClient client = new RouteOperationAttributeServiceClient())
            {
                result = await client.DeleteAsync(new RouteOperationAttributeKey()
                {
                    RouteOperationName = routeOperationName,
                    AttributeName = attributeName
                });
                if (result.Code == 0)
                {
                    result.Message = string.Format(FMMResources.StringResource.RouteOperationAttribute_Delete_Success
                                                    , attributeName);
                }
                return Json(result);
            }
        }
    }
}