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
    public class RouteStepController : Controller
    {

        //
        // GET: /FMM/RouteStep/
        public async Task<ActionResult> Index(string routeName)
        {
            using (RouteServiceClient client = new RouteServiceClient())
            {
                MethodReturnResult<Route> result = await client.GetAsync(routeName ?? string.Empty);
                if (result.Code > 0 || result.Data == null)
                {
                    return RedirectToAction("Index", "Route");
                }
                ViewBag.Route = result.Data;
            }

            using (RouteStepServiceClient client = new RouteStepServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = "SortSeq",
                        Where=string.Format("Key.RouteName='{0}'",routeName)
                    };
                    MethodReturnResult<IList<RouteStep>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new RouteStepQueryViewModel() {  RouteName= routeName });
        }

        //
        //POST: /FMM/RouteStep/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(RouteStepQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (RouteStepServiceClient client = new RouteStepServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            where.AppendFormat(" {0} Key.RouteName = '{1}'"
                                                  , where.Length > 0 ? "AND" : string.Empty
                                                  , model.RouteName);

                            if (!string.IsNullOrEmpty(model.RouteStepName))
                            {
                                where.AppendFormat(" {0} Key.RouteStepName LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.RouteStepName);
                            }
                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "SortSeq",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<RouteStep>> result = client.Get(ref cfg);

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
        //POST: /FMM/RouteStep/PagingQuery
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

                using (RouteStepServiceClient client = new RouteStepServiceClient())
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
                        MethodReturnResult<IList<RouteStep>> result = client.Get(ref cfg);
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
        // POST: /FMM/RouteStep/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(RouteStepViewModel model)
        {
            using (RouteStepServiceClient client = new RouteStepServiceClient())
            {
                RouteStep obj = new RouteStep()
                {
                    Key = new RouteStepKey(){
                        RouteName=model.RouteName,
                        RouteStepName=model.RouteStepName
                    },
                    DefectReasonCodeCategoryName = model.DefectReasonCodeCategoryName,
                    Duration = model.Duration,
                    ScrapReasonCodeCategoryName = model.ScrapReasonCodeCategoryName,
                    SortSeq = model.SortSeq,
                    RouteOperationName = model.RouteStepName,
                    Description = model.Description,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                    CreateTime = DateTime.Now,
                    Creator = User.Identity.Name
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(FMMResources.StringResource.RouteStep_Save_Success
                                                , obj.Key);
                }
                return Json(rst);
            }
        }
        //
        // GET: /FMM/RouteStep/Modify
        public async Task<ActionResult> Modify(string routeName,string routeStepName)
        {
            RouteStepViewModel viewModel = new RouteStepViewModel();
            using (RouteStepServiceClient client = new RouteStepServiceClient())
            {
                MethodReturnResult<RouteStep> result = await client.GetAsync(new RouteStepKey()
                {
                    RouteName = routeName,
                    RouteStepName = routeStepName
                });
                if (result.Code == 0)
                {
                    viewModel = new RouteStepViewModel()
                    {
                        RouteName = result.Data.Key.RouteName,
                        RouteStepName = result.Data.Key.RouteStepName,
                        SortSeq = result.Data.SortSeq,
                        ScrapReasonCodeCategoryName = result.Data.ScrapReasonCodeCategoryName,
                        Duration = result.Data.Duration,
                        DefectReasonCodeCategoryName = result.Data.DefectReasonCodeCategoryName,
                        RouteOperationName = result.Data.RouteOperationName,
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
        // POST: /FMM/RouteStep/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(RouteStepViewModel model)
        {
            using (RouteStepServiceClient client = new RouteStepServiceClient())
            {
                MethodReturnResult<RouteStep> result = await client.GetAsync(new RouteStepKey()
                {
                    RouteName = model.RouteName,
                    RouteStepName = model.RouteStepName
                });

                if (result.Code == 0)
                {
                    result.Data.SortSeq = model.SortSeq;
                    result.Data.DefectReasonCodeCategoryName = model.DefectReasonCodeCategoryName;
                    result.Data.Duration = model.Duration;
                    result.Data.ScrapReasonCodeCategoryName = model.ScrapReasonCodeCategoryName;
                    result.Data.Description = model.Description;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(FMMResources.StringResource.RouteStep_SaveModify_Success
                                                    , result.Data.Key);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // GET: /FMM/RouteStep/Detail
        public async Task<ActionResult> Detail(string routeName, string routeStepName)
        {
            using (RouteStepServiceClient client = new RouteStepServiceClient())
            {
                MethodReturnResult<RouteStep> result = await client.GetAsync(new RouteStepKey()
                {
                    RouteName = routeName,
                    RouteStepName = routeStepName
                });
                if (result.Code == 0)
                {
                    RouteStepViewModel viewModel = new RouteStepViewModel()
                    {
                        RouteName = result.Data.Key.RouteName,
                        RouteStepName = result.Data.Key.RouteStepName,
                        SortSeq = result.Data.SortSeq,
                        ScrapReasonCodeCategoryName = result.Data.ScrapReasonCodeCategoryName,
                        Duration = result.Data.Duration,
                        DefectReasonCodeCategoryName = result.Data.DefectReasonCodeCategoryName,
                        RouteOperationName = result.Data.RouteOperationName,
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
        // POST: /FMM/RouteStep/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string routeName, string routeStepName)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (RouteStepServiceClient client = new RouteStepServiceClient())
            {
                RouteStepKey key = new RouteStepKey()
                {
                    RouteName = routeName,
                    RouteStepName = routeStepName
                };
                result = await client.DeleteAsync(key);
                if (result.Code == 0)
                {
                    result.Message = string.Format(FMMResources.StringResource.RouteStep_Delete_Success
                                                    , key);
                }
                return Json(result);
            }
        }
        public ActionResult GetMaxSeqNo(string routeName)
        {
            using (RouteStepServiceClient client = new RouteStepServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where=string.Format("Key.RouteName='{0}'",routeName),
                    OrderBy = "SortSeq Desc"
                };
                MethodReturnResult<IList<RouteStep>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null && result.Data.Count > 0)
                {
                    return Json(result.Data[0].SortSeq + 1, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(1, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRouteOperation(string routeOperationName)
        {
            using (RouteOperationServiceClient client = new RouteOperationServiceClient())
            {
                MethodReturnResult<RouteOperation> result = client.Get(routeOperationName);
                if (result.Code <= 0 && result.Data != null)
                {
                    return Json(result.Data, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }
}