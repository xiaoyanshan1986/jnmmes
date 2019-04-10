using ServiceCenter.Client.Mvc.Areas.PPM.Models;
using PPMResources = ServiceCenter.Client.Mvc.Resources.PPM;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Service.Client.PPM;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Model.FMM;

namespace ServiceCenter.Client.Mvc.Areas.PPM.Controllers
{
    public class WorkOrderRouteController : Controller
    {

        //
        // GET: /PPM/WorkOrderRoute/
        public async Task<ActionResult> Index(string orderNumber)
        {
            using (WorkOrderServiceClient client = new WorkOrderServiceClient())
            {
                MethodReturnResult<WorkOrder> result = await client.GetAsync(orderNumber ?? string.Empty);
                if (result.Code > 0 || result.Data == null)
                {
                    return RedirectToAction("Index", "WorkOrder");
                }
                ViewBag.WorkOrder = result.Data;
            }

            using (WorkOrderRouteServiceClient client = new WorkOrderRouteServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        Where = string.Format(" Key.OrderNumber = '{0}'"
                                                    , orderNumber)
                    };
                    MethodReturnResult<IList<WorkOrderRoute>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new WorkOrderRouteQueryViewModel() { OrderNumber = orderNumber });
        }

        //
        //POST: /PPM/WorkOrderRoute/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(WorkOrderRouteQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (WorkOrderRouteServiceClient client = new WorkOrderRouteServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            where.AppendFormat(" {0} Key.OrderNumber = '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.OrderNumber);
                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "Key",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<WorkOrderRoute>> result = client.Get(ref cfg);

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
        //POST: /PPM/WorkOrderRoute/PagingQuery
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

                using (WorkOrderRouteServiceClient client = new WorkOrderRouteServiceClient())
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
                        MethodReturnResult<IList<WorkOrderRoute>> result = client.Get(ref cfg);
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
        // POST: /PPM/WorkOrderRoute/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(WorkOrderRouteViewModel model)
        {
            using (WorkOrderRouteServiceClient client = new WorkOrderRouteServiceClient())
            {
                WorkOrderRoute obj = new WorkOrderRoute()
                {
                    Key = new WorkOrderRouteKey()
                    {
                        OrderNumber = model.OrderNumber.ToUpper(),
                        ItemNo = model.ItemNo
                    },
                    RouteEnterpriseName = model.RouteEnterpriseName,
                    RouteName = model.RouteName,
                    RouteStepName = model.RouteStepName,
                    IsRework=model.IsRework,
                    CreateTime=DateTime.Now,
                    Creator=User.Identity.Name,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(PPMResources.StringResource.WorkOrderRoute_Save_Success
                                                , obj.Key);
                }
                return Json(rst);
            }
        }
        //
        // GET: /PPM/WorkOrderRoute/Modify
        public async Task<ActionResult> Modify(string orderNumber, int itemNo)
        {
            WorkOrderRouteViewModel viewModel = new WorkOrderRouteViewModel();
            using (WorkOrderRouteServiceClient client = new WorkOrderRouteServiceClient())
            {
                MethodReturnResult<WorkOrderRoute> result = await client.GetAsync(new WorkOrderRouteKey()
                {
                    OrderNumber = orderNumber,
                    ItemNo = itemNo
                });
                if (result.Code == 0)
                {
                    viewModel = new WorkOrderRouteViewModel()
                    {
                        OrderNumber = result.Data.Key.OrderNumber,
                        ItemNo = result.Data.Key.ItemNo,
                        RouteEnterpriseName = result.Data.RouteEnterpriseName,
                        RouteName = result.Data.RouteName,
                        RouteStepName = result.Data.RouteStepName,
                        IsRework=result.Data.IsRework,
                        CreateTime=result.Data.CreateTime,
                        Creator=result.Data.Creator,
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
        // POST: /PPM/WorkOrderRoute/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(WorkOrderRouteViewModel model)
        {
            using (WorkOrderRouteServiceClient client = new WorkOrderRouteServiceClient())
            {
                MethodReturnResult<WorkOrderRoute> result = await client.GetAsync(new WorkOrderRouteKey()
                {
                    OrderNumber = model.OrderNumber.ToUpper(),
                    ItemNo = model.ItemNo
                });

                if (result.Code == 0)
                {
                    result.Data.RouteEnterpriseName = model.RouteEnterpriseName;
                    result.Data.RouteName = model.RouteName;
                    result.Data.RouteStepName = model.RouteStepName;
                    result.Data.IsRework = model.IsRework;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(PPMResources.StringResource.WorkOrderRoute_SaveModify_Success
                                                    , result.Data.Key);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // GET: /PPM/WorkOrderRoute/Detail
        public async Task<ActionResult> Detail(string orderNumber, int itemNo)
        {
            using (WorkOrderRouteServiceClient client = new WorkOrderRouteServiceClient())
            {
                MethodReturnResult<WorkOrderRoute> result = await client.GetAsync(new WorkOrderRouteKey()
                {
                    OrderNumber = orderNumber,
                    ItemNo = itemNo
                });
                if (result.Code == 0)
                {
                    WorkOrderRouteViewModel viewModel = new WorkOrderRouteViewModel()
                    {
                        OrderNumber = result.Data.Key.OrderNumber,
                        ItemNo = result.Data.Key.ItemNo,
                        RouteEnterpriseName = result.Data.RouteEnterpriseName,
                        RouteName = result.Data.RouteName,
                        RouteStepName = result.Data.RouteStepName,
                        IsRework=result.Data.IsRework,
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
        // POST: /PPM/WorkOrderRoute/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string orderNumber, int itemNo)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (WorkOrderRouteServiceClient client = new WorkOrderRouteServiceClient())
            {
                result = await client.DeleteAsync(new WorkOrderRouteKey()
                {
                    OrderNumber = orderNumber,
                    ItemNo = itemNo
                });
                if (result.Code == 0)
                {
                    result.Message = string.Format(PPMResources.StringResource.WorkOrderRoute_Delete_Success
                                                    , itemNo);
                }
                return Json(result);
            }
        }

        public ActionResult GetMaxItemNo(string orderNumber)
        {
            using (WorkOrderRouteServiceClient client = new WorkOrderRouteServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key.OrderNumber='{0}'", orderNumber),
                    OrderBy = "Key.ItemNo Desc"
                };
                MethodReturnResult<IList<WorkOrderRoute>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null && result.Data.Count > 0)
                {
                    return Json(result.Data[0].Key.ItemNo + 1, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(1, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRouteList(string routeEnterpriseName)
        {
            using (RouteEnterpriseDetailServiceClient client = new RouteEnterpriseDetailServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Key.RouteEnterpriseName='{0}'", routeEnterpriseName),
                    OrderBy = "ItemNo"
                };
                MethodReturnResult<IList<RouteEnterpriseDetail>> result = client.Get(ref cfg);
                if (result.Code <= 0)
                {
                    return Json(result.Data, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new List<RouteEnterpriseDetail>(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRouteStepList(string routeName)
        {
            using (RouteStepServiceClient client = new RouteStepServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Key.RouteName='{0}'", routeName),
                    OrderBy="SortSeq"
                };
                MethodReturnResult<IList<RouteStep>> result = client.Get(ref cfg);
                if (result.Code <= 0)
                {
                    return Json(result.Data, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new List<RouteStep>(), JsonRequestBehavior.AllowGet);
        }
    }
}