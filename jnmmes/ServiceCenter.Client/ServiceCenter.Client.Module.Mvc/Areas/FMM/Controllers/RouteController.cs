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
    public class RouteController : Controller
    {
        /// <summary>
        /// 工艺流程主界面初始化
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Index()
        {
            using (RouteServiceClient client = new RouteServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = "Key"
                        //OrderBy = "EditTime desc"
                    };
                    MethodReturnResult<IList<Route>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }

            return View(new RouteQueryViewModel());
        }

        /// <summary>
        /// 工艺流程列表查询
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(RouteQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (RouteServiceClient client = new RouteServiceClient())
                {
                    await Task.Run(() =>
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
                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "Key",
                            //OrderBy = "EditTime desc",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<Route>> result = client.Get(ref cfg);

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
        
        /// <summary>
        /// 页面列表查询
        /// </summary>
        /// <param name="where"></param>
        /// <param name="orderBy"></param>
        /// <param name="currentPageNo"></param>
        /// <param name="currentPageSize"></param>
        /// <returns></returns>
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

                using (RouteServiceClient client = new RouteServiceClient())
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
                        MethodReturnResult<IList<Route>> result = client.Get(ref cfg);
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
        
        /// <summary>
        /// 工艺流程保存
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(RouteViewModel model)
        {
            using (RouteServiceClient client = new RouteServiceClient())
            {
                Route obj = new Route()
                {
                    Key = model.Name,
                    Status=model.Status,
                    RouteType = model.RouteType,
                    Description = model.Description,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                    CreateTime = DateTime.Now,
                    Creator = User.Identity.Name
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(FMMResources.StringResource.Route_Save_Success
                                                , model.Name);
                }
                return Json(rst);
            }
        }
        
        /// <summary>
        /// 工艺流程修改查询
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<ActionResult> Modify(string key)
        {
            RouteViewModel viewModel = new RouteViewModel();
            using (RouteServiceClient client = new RouteServiceClient())
            {
                MethodReturnResult<Route> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    viewModel = new RouteViewModel()
                    {
                        Name = result.Data.Key,
                        Status = result.Data.Status,
                        RouteType = result.Data.RouteType,
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

        /// <summary>
        /// 工艺流程修改保存
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(RouteViewModel model)
        {
            using (RouteServiceClient client = new RouteServiceClient())
            {
                MethodReturnResult<Route> result = await client.GetAsync(model.Name);

                if (result.Code == 0)
                {
                    result.Data.Status = model.Status;
                    result.Data.RouteType = model.RouteType;
                    result.Data.Description = model.Description;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(FMMResources.StringResource.Route_SaveModify_Success
                                                    , model.Name);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        
        /// <summary>
        /// 工艺流程信息查询
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<ActionResult> Detail(string key)
        {
            using (RouteServiceClient client = new RouteServiceClient())
            {
                MethodReturnResult<Route> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    RouteViewModel viewModel = new RouteViewModel()
                    {
                        Name = result.Data.Key,
                        Status = result.Data.Status,
                        RouteType = result.Data.RouteType,
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

        public async Task<ActionResult> Copy(string key)
        {
            using (RouteServiceClient client = new RouteServiceClient())
            {
                MethodReturnResult<Route> result = await client.GetAsync(key);
                
                if (result.Code == 0)
                {
                    RouteViewModel model = new RouteViewModel()
                    {
                        Name = result.Data.Key,
                        RouteType=result.Data.RouteType,
                        Description=result.Data.Description,
                        Status=result.Data.Status,
                        Creator = User.Identity.Name,
                        CreateTime = DateTime.Now,
                        Editor = User.Identity.Name,
                        EditTime = DateTime.Now,
                        ParentName = key
                    };
                    return PartialView("_CopyPartial", model);
                }
                else
                {
                    ModelState.AddModelError("", result.Message);
                    return PartialView("_CopyPartial");
                }
                   
            }
        }
        
        /// <summary>
        /// 工艺流程删除
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (RouteServiceClient client = new RouteServiceClient())
            {
                result = await client.DeleteAsync(key);
                if (result.Code == 0)
                {
                    result.Message = string.Format(FMMResources.StringResource.Route_Delete_Success
                                                    , key);
                }
                return Json(result);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveCopy(RouteViewModel model)
        {
            Route obj = new Route()
            {
                Key = model.Name,
                RouteType = model.RouteType,
                Status = model.Status,
                Description = model.Description,
                Creator = User.Identity.Name,
                CreateTime = DateTime.Now,
                Editor = User.Identity.Name,
                EditTime = DateTime.Now
            };
            MethodReturnResult result = new MethodReturnResult();
            using (RouteServiceClient client = new RouteServiceClient())
            {
                result = await client.AddAsync(obj);
                if (result.Code == 0)
                {
                    result.Message = "复制工艺流程主体成功";
                    StringBuilder where = new StringBuilder();

                    using (RouteStepServiceClient routeStepServiceClient = new RouteStepServiceClient())
                    {
                        where.AppendFormat(" Key.RouteName ='{0}'",
                                        model.ParentName);


                        PagingConfig cfg = new PagingConfig()
                        {
                            IsPaging = false,
                            OrderBy = "SortSeq",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<RouteStep>> routeStep = routeStepServiceClient.Get(ref cfg);
                        if (routeStep.Code == 0)
                        {
                            List<RouteStep> listOfRouteStep = routeStep.Data.ToList<RouteStep>();
                            foreach (RouteStep itemOfRouteStep in listOfRouteStep)
                            {
                                #region 复制工步信息
                                RouteStep objRouteStep = new RouteStep()
                                {
                                    Key = new RouteStepKey()
                                    {
                                        RouteName = model.Name,
                                        RouteStepName = itemOfRouteStep.Key.RouteStepName
                                    },
                                    DefectReasonCodeCategoryName = itemOfRouteStep.DefectReasonCodeCategoryName,
                                    Duration = itemOfRouteStep.Duration,
                                    ScrapReasonCodeCategoryName = itemOfRouteStep.ScrapReasonCodeCategoryName,
                                    SortSeq = itemOfRouteStep.SortSeq,
                                    RouteOperationName = itemOfRouteStep.RouteOperationName,
                                    Description = itemOfRouteStep.Description,
                                    Editor = User.Identity.Name,
                                    EditTime = DateTime.Now,
                                    CreateTime = DateTime.Now,
                                    Creator = User.Identity.Name
                                };
                                MethodReturnResult rstOfRouteStep = await routeStepServiceClient.AddAsync(objRouteStep);
                                if (rstOfRouteStep.Code != 0)
                                {
                                    result.Code = 1001;
                                    result.Message = "复制工步信息失败" + rstOfRouteStep.Message;
                                    return Json(result);
                                }                                
                                #endregion

                                #region 复制工步参数
                                using (RouteStepParameterServiceClient routeStepParameterServiceClient = new RouteStepParameterServiceClient())
                                {
                                    #region 删除新增工艺流程工步带过来的工步参数
                                    cfg = new PagingConfig()
                                    {
                                        Where = string.Format(" Key.RouteName='{0}' AND Key.RouteStepName = '{1}'"
                                                            , model.Name
                                                            , itemOfRouteStep.Key.RouteStepName),
                                        OrderBy = "ParamIndex"
                                    };
                                    MethodReturnResult<IList<RouteStepParameter>> routeStepParameterNewAdd = routeStepParameterServiceClient.Get(ref cfg);
                                    if (routeStepParameterNewAdd.Code == 0)
                                    {
                                        List<RouteStepParameter> listOfRouteStepParameterNewAdd = routeStepParameterNewAdd.Data.ToList<RouteStepParameter>();
                                        foreach (RouteStepParameter itemOfRouteStepParameterNewAdd in listOfRouteStepParameterNewAdd)
                                        {
                                            MethodReturnResult rstOfRouteStepParameterNewAdd = routeStepParameterServiceClient.Delete(itemOfRouteStepParameterNewAdd.Key);
                                            if (rstOfRouteStepParameterNewAdd.Code != 0)
                                            {
                                                result.Code = 1001;
                                                result.Message = "删除初始工步参数信息失败" + rstOfRouteStepParameterNewAdd.Message;
                                                return Json(result);
                                            }
                                        }
                                    }
                                    #endregion

                                    #region 复制工步参数
                                    cfg = new PagingConfig()
                                    {
                                        Where = string.Format(" Key.RouteName='{0}' AND Key.RouteStepName = '{1}'"
                                                            , model.ParentName
                                                            , itemOfRouteStep.Key.RouteStepName),
                                        OrderBy = "ParamIndex"
                                    };
                                    MethodReturnResult<IList<RouteStepParameter>> routeStepParameter = routeStepParameterServiceClient.Get(ref cfg);
                                    if (routeStepParameter.Code == 0)
                                    {
                                        List<RouteStepParameter> listOfRouteStepParameter = routeStepParameter.Data.ToList<RouteStepParameter>();
                                        foreach (RouteStepParameter itemOfRouteStepParameter in listOfRouteStepParameter)
                                        {
                                            RouteStepParameter objRouteStepParameter = new RouteStepParameter()
                                            {
                                                Key = new RouteStepParameterKey()
                                                {
                                                    RouteName = model.Name,
                                                    RouteStepName = itemOfRouteStepParameter.Key.RouteStepName,
                                                    ParameterName = itemOfRouteStepParameter.Key.ParameterName
                                                },
                                                MaterialType = itemOfRouteStepParameter.MaterialType,
                                                DataFrom = itemOfRouteStepParameter.DataFrom,
                                                DataType = itemOfRouteStepParameter.DataType,
                                                DCType = itemOfRouteStepParameter.DCType,
                                                IsDeleted = itemOfRouteStepParameter.IsDeleted,
                                                IsMustInput = itemOfRouteStepParameter.IsMustInput,
                                                IsReadOnly = itemOfRouteStepParameter.IsReadOnly,
                                                IsUsePreValue = itemOfRouteStepParameter.IsUsePreValue,
                                                ParamIndex = itemOfRouteStepParameter.ParamIndex,
                                                ValidateFailedMessage = itemOfRouteStepParameter.ValidateFailedMessage,
                                                ValidateFailedRule = itemOfRouteStepParameter.ValidateFailedRule,
                                                ValidateRule = itemOfRouteStepParameter.ValidateRule,
                                                Editor = User.Identity.Name,
                                                EditTime = DateTime.Now,
                                            };
                                            MethodReturnResult rstOfRouteStepParameter = await routeStepParameterServiceClient.AddAsync(objRouteStepParameter);
                                            if (rstOfRouteStepParameter.Code != 0)
                                            {
                                                result.Code = 1001;
                                                result.Message = "复制工步参数信息失败" + rstOfRouteStepParameter.Message;
                                                return Json(result);
                                            }
                                        }
                                    }
                                    #endregion
                                }
                                #endregion

                                #region 复制工步属性
                                using (RouteStepAttributeServiceClient routeStepAttributeServiceClient = new RouteStepAttributeServiceClient())
                                {
                                    #region 删除新增工艺流程工步带过来的工步属性
                                    cfg = new PagingConfig()
                                    {
                                        Where = string.Format(" Key.RouteName='{0}' AND Key.RouteStepName = '{1}'"
                                                              , model.Name
                                                              , itemOfRouteStep.Key.RouteStepName),
                                        OrderBy = "Key.AttributeName"
                                    };
                                    MethodReturnResult<IList<RouteStepAttribute>> routeStepAttributeNewAdd = routeStepAttributeServiceClient.Get(ref cfg);
                                    if (routeStepAttributeNewAdd.Code == 0)
                                    {
                                        List<RouteStepAttribute> listOfRouteStepAttributeNewAdd = routeStepAttributeNewAdd.Data.ToList<RouteStepAttribute>();
                                        foreach (RouteStepAttribute itemOfRouteStepAttributeNewAdd in listOfRouteStepAttributeNewAdd)
                                        {
                                            MethodReturnResult rstOfRouteStepAttributeNewAdd = routeStepAttributeServiceClient.Delete(itemOfRouteStepAttributeNewAdd.Key);
                                            if (rstOfRouteStepAttributeNewAdd.Code != 0)
                                            {
                                                result.Code = 1001;
                                                result.Message = "删除初始工步属性信息失败" + rstOfRouteStepAttributeNewAdd.Message;
                                                return Json(result);
                                            }
                                        }
                                    }
                                    #endregion

                                    #region 复制工步属性
                                    cfg = new PagingConfig()
                                    {
                                        Where = string.Format(" Key.RouteName='{0}' AND Key.RouteStepName = '{1}'"
                                                              , model.ParentName
                                                              , itemOfRouteStep.Key.RouteStepName),
                                        OrderBy = "Key.AttributeName"
                                    };
                                    MethodReturnResult<IList<RouteStepAttribute>> routeStepAttribute = routeStepAttributeServiceClient.Get(ref cfg);
                                    if (routeStepAttribute.Code == 0)
                                    {
                                        List<RouteStepAttribute> listOfRouteStepAttribute = routeStepAttribute.Data.ToList<RouteStepAttribute>();
                                        foreach (RouteStepAttribute itemOfRouteStepAttribute in listOfRouteStepAttribute)
                                        {
                                            RouteStepAttribute objRouteStepAttribute = new RouteStepAttribute()
                                            {
                                                Key = new RouteStepAttributeKey()
                                                {
                                                    RouteName = model.Name,
                                                    RouteStepName = itemOfRouteStepAttribute.Key.RouteStepName,
                                                    AttributeName = itemOfRouteStepAttribute.Key.AttributeName
                                                },
                                                Value = itemOfRouteStepAttribute.Value, 
                                                Editor = User.Identity.Name,
                                                EditTime = DateTime.Now,
                                            };
                                            MethodReturnResult rstOfRouteStepAttribute = await routeStepAttributeServiceClient.AddAsync(objRouteStepAttribute);
                                            if (rstOfRouteStepAttribute.Code != 0)
                                            {
                                                result.Code = 1001;
                                                result.Message = "复制工步属性信息失败" + rstOfRouteStepAttribute.Message;
                                                return Json(result);
                                            }
                                        }
                                    }
                                    #endregion
                                }
                                #endregion
                            }
                            result.Message += " \r\n 复制工艺流程工步参数及工步属性成功";
                        }
                    }
                }
            }
            return Json(result);
        }
    }
}