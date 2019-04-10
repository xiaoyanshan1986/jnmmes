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
    public class RouteEnterpriseDetailController : Controller
    {

        //
        // GET: /FMM/RouteEnterpriseDetail/
        public async Task<ActionResult> Index(string routeEnterpriseName)
        {
            using (RouteEnterpriseServiceClient client = new RouteEnterpriseServiceClient())
            {
                MethodReturnResult<RouteEnterprise> result = await client.GetAsync(routeEnterpriseName ?? string.Empty);
                if (result.Code > 0 || result.Data == null)
                {
                    return RedirectToAction("Index", "RouteEnterprise");
                }
                ViewBag.RouteEnterprise = result.Data;
            }

            using (RouteEnterpriseDetailServiceClient client = new RouteEnterpriseDetailServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        Where = string.Format(" Key.RouteEnterpriseName = '{0}'"
                                                    , routeEnterpriseName),
                        OrderBy = "ItemNo"
                    };
                    MethodReturnResult<IList<RouteEnterpriseDetail>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new RouteEnterpriseDetailQueryViewModel() { RouteEnterpriseName = routeEnterpriseName });
        }

        //
        //POST: /FMM/RouteEnterpriseDetail/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(RouteEnterpriseDetailQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (RouteEnterpriseDetailServiceClient client = new RouteEnterpriseDetailServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            where.AppendFormat(" {0} Key.RouteEnterpriseName = '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.RouteEnterpriseName);

                            if (!string.IsNullOrEmpty(model.RouteName))
                            {
                                where.AppendFormat(" {0} Key.RouteName LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.RouteName);
                            }
                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "ItemNo",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<RouteEnterpriseDetail>> result = client.Get(ref cfg);

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
        //POST: /FMM/RouteEnterpriseDetail/PagingQuery
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

                using (RouteEnterpriseDetailServiceClient client = new RouteEnterpriseDetailServiceClient())
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
                        MethodReturnResult<IList<RouteEnterpriseDetail>> result = client.Get(ref cfg);
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
        // POST: /FMM/RouteEnterpriseDetail/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(RouteEnterpriseDetailViewModel model)
        {
            using (RouteEnterpriseDetailServiceClient client = new RouteEnterpriseDetailServiceClient())
            {
                RouteEnterpriseDetail obj = new RouteEnterpriseDetail()
                {
                    Key = new RouteEnterpriseDetailKey()
                    {
                        RouteEnterpriseName = model.RouteEnterpriseName,
                        RouteName = model.RouteName
                    },
                    ItemNo = model.ItemNo,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                    CreateTime = DateTime.Now,
                    Creator = User.Identity.Name
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(FMMResources.StringResource.RouteEnterpriseDetail_Save_Success
                                                , obj.Key);
                }
                return Json(rst);
            }
        }
        //
        // GET: /FMM/RouteEnterpriseDetail/Modify
        public async Task<ActionResult> Modify(string routeEnterpriseName, string routeName)
        {
            RouteEnterpriseDetailViewModel viewModel = new RouteEnterpriseDetailViewModel();
            using (RouteEnterpriseDetailServiceClient client = new RouteEnterpriseDetailServiceClient())
            {
                MethodReturnResult<RouteEnterpriseDetail> result = await client.GetAsync(new RouteEnterpriseDetailKey()
                {
                    RouteEnterpriseName = routeEnterpriseName,
                    RouteName = routeName
                });
                if (result.Code == 0)
                {
                    viewModel = new RouteEnterpriseDetailViewModel()
                    {
                        RouteEnterpriseName = result.Data.Key.RouteEnterpriseName,
                        RouteName = result.Data.Key.RouteName,
                        ItemNo = result.Data.ItemNo,
                        Editor = result.Data.Editor,
                        EditTime = result.Data.EditTime,
                        CreateTime = result.Data.CreateTime,
                        Creator = result.Data.Creator
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
        // POST: /FMM/RouteEnterpriseDetail/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(RouteEnterpriseDetailViewModel model)
        {
            using (RouteEnterpriseDetailServiceClient client = new RouteEnterpriseDetailServiceClient())
            {
                MethodReturnResult<RouteEnterpriseDetail> result = await client.GetAsync(new RouteEnterpriseDetailKey()
                {
                    RouteEnterpriseName = model.RouteEnterpriseName,
                    RouteName = model.RouteName
                });

                if (result.Code == 0)
                {
                    result.Data.ItemNo = model.ItemNo;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(FMMResources.StringResource.RouteEnterpriseDetail_SaveModify_Success
                                                    , result.Data.Key);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // GET: /FMM/RouteEnterpriseDetail/Detail
        public async Task<ActionResult> Detail(string routeEnterpriseName, string routeName)
        {
            using (RouteEnterpriseDetailServiceClient client = new RouteEnterpriseDetailServiceClient())
            {
                MethodReturnResult<RouteEnterpriseDetail> result = await client.GetAsync(new RouteEnterpriseDetailKey()
                {
                    RouteEnterpriseName = routeEnterpriseName,
                    RouteName = routeName
                });
                if (result.Code == 0)
                {
                    RouteEnterpriseDetailViewModel viewModel = new RouteEnterpriseDetailViewModel()
                    {
                        RouteEnterpriseName = result.Data.Key.RouteEnterpriseName,
                        RouteName = result.Data.Key.RouteName,
                        ItemNo = result.Data.ItemNo,
                        Editor = result.Data.Editor,
                        EditTime = result.Data.EditTime,
                        CreateTime = result.Data.CreateTime,
                        Creator = result.Data.Creator
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
        // POST: /FMM/RouteEnterpriseDetail/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string routeEnterpriseName, string routeName)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (RouteEnterpriseDetailServiceClient client = new RouteEnterpriseDetailServiceClient())
            {
                result = await client.DeleteAsync(new RouteEnterpriseDetailKey()
                {
                    RouteEnterpriseName = routeEnterpriseName,
                    RouteName = routeName
                });
                if (result.Code == 0)
                {
                    result.Message = string.Format(FMMResources.StringResource.RouteEnterpriseDetail_Delete_Success
                                                    , routeName);
                }
                return Json(result);
            }
        }
    }
}