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
    public class RouteOperationParameterController : Controller
    {
        //
        // GET: /FMM/RouteOperationParameter/
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

            using (RouteOperationParameterServiceClient client = new RouteOperationParameterServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        Where=string.Format(" Key.RouteOperationName = '{0}'"
                                                    , routeOperationName),
                        OrderBy = "ParamIndex"
                    };
                    MethodReturnResult<IList<RouteOperationParameter>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new RouteOperationParameterQueryViewModel() { RouteOperationName = routeOperationName });
        }

        //
        //POST: /FMM/RouteOperationParameter/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(RouteOperationParameterQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (RouteOperationParameterServiceClient client = new RouteOperationParameterServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            where.AppendFormat(" {0} Key.RouteOperationName = '{1}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.RouteOperationName);

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
                        MethodReturnResult<IList<RouteOperationParameter>> result = client.Get(ref cfg);

                        if (result.Code == 0)
                        {
                            ViewBag.PagingConfig = cfg;
                            ViewBag.List = result.Data;
                        }
                    });
                }
            }
            return PartialView("_ListPartial", new RouteOperationParameterViewModel()
            {
                RouteOperationName = model.RouteOperationName
            });
        }
        //
        //POST: /FMM/RouteOperationParameter/PagingQuery
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

                using (RouteOperationParameterServiceClient client = new RouteOperationParameterServiceClient())
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
                        MethodReturnResult<IList<RouteOperationParameter>> result = client.Get(ref cfg);
                        if (result.Code == 0)
                        {
                            ViewBag.PagingConfig = cfg;
                            ViewBag.List = result.Data;
                        }
                    });
                }
            }
            return PartialView("_ListPartial", new RouteOperationParameterViewModel() { });
        }
        //
        // POST: /FMM/RouteOperationParameter/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(RouteOperationParameterViewModel model)
        {
            using (RouteOperationParameterServiceClient client = new RouteOperationParameterServiceClient())
            {
                RouteOperationParameter obj = new RouteOperationParameter()
                {
                    Key = new RouteOperationParameterKey()
                    {
                        RouteOperationName = model.RouteOperationName,
                        ParameterName = model.ParameterName
                    },
                    DataFrom=model.DataFrom,
                    DataType=model.DataType,
                    DCType=model.DCType,
                    IsDeleted=model.IsDeleted,
                    IsMustInput=model.IsMustInput,
                    IsReadOnly=model.IsReadOnly,
                    MaterialType=model.MaterialType,
                    IsUsePreValue=model.IsUsePreValue,
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
                    rst.Message = string.Format(FMMResources.StringResource.RouteOperationParameter_Save_Success
                                                , obj.Key);
                }
                return Json(rst);
            }
        }

        //
        // GET: /FMM/RouteOperationParameter/Modify
        public async Task<ActionResult> Modify(string routeOperationName, string parameterName)
        {
            RouteOperationParameterViewModel viewModel = new RouteOperationParameterViewModel();
            using (RouteOperationParameterServiceClient client = new RouteOperationParameterServiceClient())
            {
                MethodReturnResult<RouteOperationParameter> result = await client.GetAsync(new RouteOperationParameterKey()
                {
                    RouteOperationName = routeOperationName,
                    ParameterName = parameterName
                });
                if (result.Code == 0)
                {
                    viewModel = new RouteOperationParameterViewModel()
                    {
                        RouteOperationName = result.Data.Key.RouteOperationName,
                        ParameterName = result.Data.Key.ParameterName,
                        DataFrom = result.Data.DataFrom,
                        DataType = result.Data.DataType,
                        DCType = result.Data.DCType,
                        IsDeleted = result.Data.IsDeleted,
                        IsMustInput = result.Data.IsMustInput,
                        IsReadOnly = result.Data.IsReadOnly,
                        MaterialType = result.Data.MaterialType,
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
        // POST: /FMM/RouteOperationParameter/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(RouteOperationParameterViewModel model)
        {
            using (RouteOperationParameterServiceClient client = new RouteOperationParameterServiceClient())
            {
                MethodReturnResult<RouteOperationParameter> result = await client.GetAsync(new RouteOperationParameterKey()
                {
                    RouteOperationName = model.RouteOperationName,
                    ParameterName = model.ParameterName
                });

                if (result.Code == 0)
                {
                    result.Data.DataFrom = model.DataFrom;
                    result.Data.DataType = model.DataType;
                    result.Data.DCType = model.DCType;
                    result.Data.MaterialType = model.MaterialType;
                    result.Data.IsDeleted = model.IsDeleted;
                    result.Data.IsMustInput = model.IsMustInput;
                    result.Data.IsReadOnly = model.IsReadOnly;
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
                        rst.Message = string.Format(FMMResources.StringResource.RouteOperationParameter_SaveModify_Success
                                                    , result.Data.Key);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // GET: /FMM/RouteOperationParameter/Detail
        public async Task<ActionResult> Detail(string routeOperationName, string parameterName)
        {
            using (RouteOperationParameterServiceClient client = new RouteOperationParameterServiceClient())
            {
                MethodReturnResult<RouteOperationParameter> result = await client.GetAsync(new RouteOperationParameterKey()
                {
                    RouteOperationName = routeOperationName,
                    ParameterName = parameterName
                });
                if (result.Code == 0)
                {
                    RouteOperationParameterViewModel viewModel = new RouteOperationParameterViewModel()
                    {
                        RouteOperationName = result.Data.Key.RouteOperationName,
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

        // POST: /FMM/RouteOperationParameter/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string routeOperationName, string parameterName)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (RouteOperationParameterServiceClient client = new RouteOperationParameterServiceClient())
            {
                result = await client.DeleteAsync(new RouteOperationParameterKey()
                {
                    RouteOperationName = routeOperationName,
                    ParameterName = parameterName
                });
                if (result.Code == 0)
                {
                    result.Message = string.Format(FMMResources.StringResource.RouteOperationParameter_Delete_Success
                                                    , parameterName);
                }
                return Json(result);
            }
        }


        public ActionResult GetMaxParamterIndex(string routeOperationName)
        {
            using (RouteOperationParameterServiceClient client = new RouteOperationParameterServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    OrderBy = "ParamIndex Desc",
                    Where = string.Format("Key.RouteOperationName='{0}'", routeOperationName)
                };
                MethodReturnResult<IList<RouteOperationParameter>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null && result.Data.Count>0)
                {
                    return Json(result.Data[0].ParamIndex+1, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(1, JsonRequestBehavior.AllowGet);
        }
    }
}