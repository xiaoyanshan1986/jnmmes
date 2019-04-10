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
    public class RouteStepParameterController : Controller
    {
        //
        // GET: /FMM/RouteStepParameter/
        public async Task<ActionResult> Index(string routeName,string routeStepName)
        {
            using (RouteStepServiceClient client = new RouteStepServiceClient())
            {
                MethodReturnResult<RouteStep> result = await client.GetAsync(new RouteStepKey()
                {
                    RouteName = routeName,
                    RouteStepName=routeStepName
                });

                if (result.Code > 0 || result.Data == null)
                {
                    return RedirectToAction("Index", "RouteStep", new { @RouteName = routeName });
                }
                ViewBag.RouteStep = result.Data;
            }

            using (RouteStepParameterServiceClient client = new RouteStepParameterServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        Where=string.Format(" Key.RouteName='{0}' AND Key.RouteStepName = '{1}'"
                                            ,routeName
                                            ,routeStepName),
                        OrderBy = "ParamIndex"
                    };
                    MethodReturnResult<IList<RouteStepParameter>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new RouteStepParameterQueryViewModel() { RouteName=routeName, RouteStepName = routeStepName });
        }

        //
        //POST: /FMM/RouteStepParameter/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(RouteStepParameterQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (RouteStepParameterServiceClient client = new RouteStepParameterServiceClient())
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

                            if (!string.IsNullOrEmpty(model.ParameterName))
                            {
                                where.AppendFormat(" {0} Key.ParameterName LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.ParameterName);
                            }
                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "ParamIndex",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<RouteStepParameter>> result = client.Get(ref cfg);

                        if (result.Code == 0)
                        {
                            ViewBag.PagingConfig = cfg;
                            ViewBag.List = result.Data;
                        }
                    });
                }
            }
            return PartialView("_ListPartial", new RouteStepParameterViewModel()
            {
                RouteName=model.RouteName,
                RouteStepName = model.RouteStepName
            });
        }
        //
        //POST: /FMM/RouteStepParameter/PagingQuery
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

                using (RouteStepParameterServiceClient client = new RouteStepParameterServiceClient())
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
                        MethodReturnResult<IList<RouteStepParameter>> result = client.Get(ref cfg);
                        if (result.Code == 0)
                        {
                            ViewBag.PagingConfig = cfg;
                            ViewBag.List = result.Data;
                        }
                    });
                }
            }
            return PartialView("_ListPartial", new RouteStepParameterViewModel() { });
        }
        //
        // POST: /FMM/RouteStepParameter/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(RouteStepParameterViewModel model)
        {
            using (RouteStepParameterServiceClient client = new RouteStepParameterServiceClient())
            {
                RouteStepParameter obj = new RouteStepParameter()
                {
                    Key = new RouteStepParameterKey()
                    {
                        RouteName=model.RouteName,
                        RouteStepName = model.RouteStepName,
                        ParameterName = model.ParameterName
                    },
                    MaterialType=model.MaterialType,
                    DataFrom=model.DataFrom,
                    DataType=model.DataType,
                    DCType=model.DCType,
                    IsDeleted=model.IsDeleted,
                    IsMustInput=model.IsMustInput,
                    IsReadOnly=model.IsReadOnly,
                    IsUsePreValue = model.IsUsePreValue,
                    ParamIndex=model.ParamIndex,
                    ValidateFailedMessage=model.ValidateFailedMessage,
                    ValidateFailedRule=model.ValidateFailedRule,
                    ValidateRule=model.ValidateRule,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(FMMResources.StringResource.RouteStepParameter_Save_Success
                                                , obj.Key);
                }
                return Json(rst);
            }
        }

        //
        // GET: /FMM/RouteStepParameter/Modify
        public async Task<ActionResult> Modify(string routeName,string routeStepName, string parameterName)
        {
            RouteStepParameterViewModel viewModel = new RouteStepParameterViewModel();
            using (RouteStepParameterServiceClient client = new RouteStepParameterServiceClient())
            {
                MethodReturnResult<RouteStepParameter> result = await client.GetAsync(new RouteStepParameterKey()
                {
                    RouteName=routeName,
                    RouteStepName = routeStepName,
                    ParameterName = parameterName
                });
                if (result.Code == 0)
                {
                    viewModel = new RouteStepParameterViewModel()
                    {
                        RouteName=result.Data.Key.RouteName,
                        RouteStepName = result.Data.Key.RouteStepName,
                        ParameterName = result.Data.Key.ParameterName,
                        DataFrom = result.Data.DataFrom,
                        DataType = result.Data.DataType,
                        DCType = result.Data.DCType,
                        IsDeleted = result.Data.IsDeleted,
                        MaterialType=result.Data.MaterialType,
                        IsMustInput = result.Data.IsMustInput,
                        IsReadOnly = result.Data.IsReadOnly,
                        IsUsePreValue = result.Data.IsUsePreValue,
                        ParamIndex = result.Data.ParamIndex,
                        ValidateFailedMessage = result.Data.ValidateFailedMessage,
                        ValidateFailedRule = result.Data.ValidateFailedRule,
                        ValidateRule = result.Data.ValidateRule,
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
        // POST: /FMM/RouteStepParameter/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(RouteStepParameterViewModel model)
        {
            using (RouteStepParameterServiceClient client = new RouteStepParameterServiceClient())
            {
                MethodReturnResult<RouteStepParameter> result = await client.GetAsync(new RouteStepParameterKey()
                {
                    RouteName=model.RouteName,
                    RouteStepName = model.RouteStepName,
                    ParameterName = model.ParameterName
                });

                if (result.Code == 0)
                {
                    result.Data.DataFrom = model.DataFrom;
                    result.Data.DataType = model.DataType;
                    result.Data.DCType = model.DCType;
                    result.Data.IsDeleted = model.IsDeleted;
                    result.Data.IsMustInput = model.IsMustInput;
                    result.Data.IsReadOnly = model.IsReadOnly;
                    result.Data.MaterialType = model.MaterialType;
                    result.Data.IsUsePreValue = model.IsUsePreValue;
                    result.Data.ParamIndex = model.ParamIndex;
                    result.Data.ValidateFailedMessage = model.ValidateFailedMessage;
                    result.Data.ValidateFailedRule = model.ValidateFailedRule;
                    result.Data.ValidateRule = model.ValidateRule;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(FMMResources.StringResource.RouteStepParameter_SaveModify_Success
                                                    , result.Data.Key);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // GET: /FMM/RouteStepParameter/Detail
        public async Task<ActionResult> Detail(string routeName,string routeStepName, string parameterName)
        {
            using (RouteStepParameterServiceClient client = new RouteStepParameterServiceClient())
            {
                MethodReturnResult<RouteStepParameter> result = await client.GetAsync(new RouteStepParameterKey()
                {
                    RouteName=routeName,
                    RouteStepName = routeStepName,
                    ParameterName = parameterName
                });
                if (result.Code == 0)
                {
                    RouteStepParameterViewModel viewModel = new RouteStepParameterViewModel()
                    {
                        RouteName=result.Data.Key.RouteName,
                        RouteStepName = result.Data.Key.RouteStepName,
                        ParameterName = result.Data.Key.ParameterName,
                        DataFrom = result.Data.DataFrom,
                        DataType = result.Data.DataType,
                        DCType = result.Data.DCType,
                        MaterialType=result.Data.MaterialType,
                        IsDeleted = result.Data.IsDeleted,
                        IsMustInput = result.Data.IsMustInput,
                        IsReadOnly = result.Data.IsReadOnly,
                        IsUsePreValue = result.Data.IsUsePreValue,
                        ParamIndex = result.Data.ParamIndex,
                        ValidateFailedMessage = result.Data.ValidateFailedMessage,
                        ValidateFailedRule = result.Data.ValidateFailedRule,
                        ValidateRule = result.Data.ValidateRule,
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

        // POST: /FMM/RouteStepParameter/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string routeName,string routeStepName, string parameterName)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (RouteStepParameterServiceClient client = new RouteStepParameterServiceClient())
            {
                result = await client.DeleteAsync(new RouteStepParameterKey()
                {
                    RouteName=routeName,
                    RouteStepName = routeStepName,
                    ParameterName = parameterName
                });
                if (result.Code == 0)
                {
                    result.Message = string.Format(FMMResources.StringResource.RouteStepParameter_Delete_Success
                                                    , parameterName);
                }
                return Json(result);
            }
        }


        public ActionResult GetMaxParamterIndex(string routeName,string routeStepName)
        {
            using (RouteStepParameterServiceClient client = new RouteStepParameterServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    OrderBy = "ParamIndex Desc",
                    Where = string.Format("Key.RouteName='{0}' AND Key.RouteStepName='{1}'"
                                         , routeName
                                         , routeStepName)
                };
                MethodReturnResult<IList<RouteStepParameter>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null && result.Data.Count>0)
                {
                    return Json(result.Data[0].ParamIndex+1, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(1, JsonRequestBehavior.AllowGet);
        }
    }
}