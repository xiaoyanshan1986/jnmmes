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
    public class RouteStepAttributeController : Controller
    {

        //
        // GET: /FMM/RouteStepAttribute/
        public async Task<ActionResult> Index(string routeName,string routeStepName)
        {
            using (RouteStepServiceClient client = new RouteStepServiceClient())
            {
                MethodReturnResult<RouteStep> result = await client.GetAsync(new RouteStepKey()
                {
                        RouteName=routeName,
                        RouteStepName = routeStepName
                });
                if (result.Code > 0 || result.Data == null)
                {
                    return RedirectToAction("Index", "RouteStep", new { @RouteName=routeName });
                }
                ViewBag.RouteStep = result.Data;
            }

            using (RouteStepAttributeServiceClient client = new RouteStepAttributeServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        Where = string.Format(" Key.RouteName='{0}' AND Key.RouteStepName = '{1}'"
                                              , routeName
                                              , routeStepName),
                        OrderBy = "Key.AttributeName"
                    };
                    MethodReturnResult<IList<RouteStepAttribute>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new RouteStepAttributeQueryViewModel() { RouteName=routeName, RouteStepName = routeStepName });
        }

        //
        //POST: /FMM/RouteStepAttribute/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(RouteStepAttributeQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (RouteStepAttributeServiceClient client = new RouteStepAttributeServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            where.AppendFormat(" {0} Key.RouteName = '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.RouteName);

                            where.AppendFormat(" {0} Key.RouteStepName = '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.RouteStepName);

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
                        MethodReturnResult<IList<RouteStepAttribute>> result = client.Get(ref cfg);

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
        //POST: /FMM/RouteStepAttribute/PagingQuery
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

                using (RouteStepAttributeServiceClient client = new RouteStepAttributeServiceClient())
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
                        MethodReturnResult<IList<RouteStepAttribute>> result = client.Get(ref cfg);
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
        // POST: /FMM/RouteStepAttribute/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(RouteStepAttributeViewModel model)
        {
            using (RouteStepAttributeServiceClient client = new RouteStepAttributeServiceClient())
            {
                RouteStepAttribute obj = new RouteStepAttribute()
                {
                    Key = new RouteStepAttributeKey() { 
                         RouteName=model.RouteName,
                         RouteStepName=model.RouteStepName,
                         AttributeName=model.AttributeName
                    },
                    Value=model.Value,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(FMMResources.StringResource.RouteStepAttribute_Save_Success
                                                , obj.Key);
                }
                return Json(rst);
            }
        }
        //
        // GET: /FMM/RouteStepAttribute/Modify
        public async Task<ActionResult> Modify(string routeName,string routeStepName,string attributeName)
        {
            RouteStepAttributeViewModel viewModel = new RouteStepAttributeViewModel();
            using (RouteStepAttributeServiceClient client = new RouteStepAttributeServiceClient())
            {
                MethodReturnResult<RouteStepAttribute> result = await client.GetAsync(new RouteStepAttributeKey()
                {
                    RouteName=routeName,
                    RouteStepName=routeStepName,
                    AttributeName=attributeName
                });
                if (result.Code == 0)
                {
                    viewModel = new RouteStepAttributeViewModel()
                    {
                        RouteName = result.Data.Key.RouteName,
                        RouteStepName = result.Data.Key.RouteStepName,
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
        // POST: /FMM/RouteStepAttribute/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(RouteStepAttributeViewModel model)
        {
            using (RouteStepAttributeServiceClient client = new RouteStepAttributeServiceClient())
            {
                MethodReturnResult<RouteStepAttribute> result = await client.GetAsync(new RouteStepAttributeKey()
                {
                    RouteName=model.RouteName,
                    RouteStepName = model.RouteStepName,
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
                        rst.Message = string.Format(FMMResources.StringResource.RouteStepAttribute_SaveModify_Success
                                                    , result.Data.Key);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // GET: /FMM/RouteStepAttribute/Detail
        public async Task<ActionResult> Detail(string routeName, string routeStepName, string attributeName)
        {
            using (RouteStepAttributeServiceClient client = new RouteStepAttributeServiceClient())
            {
                MethodReturnResult<RouteStepAttribute> result = await client.GetAsync(new RouteStepAttributeKey()
                {
                    RouteName=routeName,
                    RouteStepName = routeStepName,
                    AttributeName = attributeName
                });
                if (result.Code == 0)
                {
                    RouteStepAttributeViewModel viewModel = new RouteStepAttributeViewModel()
                    {
                        RouteName=result.Data.Key.RouteName,
                        RouteStepName = result.Data.Key.RouteStepName,
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
        // POST: /FMM/RouteStepAttribute/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string routeName, string routeStepName, string attributeName)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (RouteStepAttributeServiceClient client = new RouteStepAttributeServiceClient())
            {
                result = await client.DeleteAsync(new RouteStepAttributeKey()
                {
                    RouteName=routeName,
                    RouteStepName = routeStepName,
                    AttributeName = attributeName
                });
                if (result.Code == 0)
                {
                    result.Message = string.Format(FMMResources.StringResource.RouteStepAttribute_Delete_Success
                                                    , attributeName);
                }
                return Json(result);
            }
        }
    }
}