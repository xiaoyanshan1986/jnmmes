using ServiceCenter.Client.Mvc.Areas.EDC.Models;
using EDCResources = ServiceCenter.Client.Mvc.Resources.EDC;
using ServiceCenter.MES.Model.EDC;
using ServiceCenter.MES.Service.Client.EDC;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Service.Client.FMM;

namespace ServiceCenter.Client.Mvc.Areas.EDC.Controllers
{
    public class PointController : Controller
    {
        //
        // GET: /EDC/Point/
        public async Task<ActionResult> Index()
        {
            using (PointServiceClient client = new PointServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = "GroupName"
                    };
                    MethodReturnResult<IList<Point>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new PointQueryViewModel());
        }
        //
        //POST: /EDC/Point/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(PointQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (PointServiceClient client = new PointServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            if (!string.IsNullOrEmpty(model.GroupName))
                            {
                                where.AppendFormat(" {0} GroupName LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.GroupName);
                            }
                            if (!string.IsNullOrEmpty(model.MaterialType))
                            {
                                where.AppendFormat(" {0} MaterialType LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.MaterialType);
                            }
                            if (!string.IsNullOrEmpty(model.MaterialCode))
                            {
                                where.AppendFormat(" {0} MaterialCode LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.MaterialCode);
                            }
                            if (!string.IsNullOrEmpty(model.RouteEnterpriseName))
                            {
                                where.AppendFormat(" {0} RouteEnterpriseName LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.RouteEnterpriseName);
                            }
                            if (!string.IsNullOrEmpty(model.RouteName))
                            {
                                where.AppendFormat(" {0} RouteName LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.RouteName);
                            }
                            if (!string.IsNullOrEmpty(model.RouteStepName))
                            {
                                where.AppendFormat(" {0} RouteStepName LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.RouteStepName);
                            }
                            if (!string.IsNullOrEmpty(model.RouteOperationName))
                            {
                                where.AppendFormat(" {0} RouteOperationName LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.RouteOperationName);
                            }
                            if (!string.IsNullOrEmpty(model.ProductionLineCode))
                            {
                                where.AppendFormat(" {0} ProductionLineCode LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.ProductionLineCode);
                            }
                            if (!string.IsNullOrEmpty(model.EquipmentCode))
                            {
                                where.AppendFormat(" {0} EquipmentCode LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.EquipmentCode);
                            }
                            if (!string.IsNullOrEmpty(model.CategoryName))
                            {
                                where.AppendFormat(" {0} CategoryName LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.CategoryName);
                            }
                            if (!string.IsNullOrEmpty(model.SamplingPlanName))
                            {
                                where.AppendFormat(" {0} SamplingPlanName LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.SamplingPlanName);
                            }
                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "GroupName",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<Point>> result = client.Get(ref cfg);

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
        //POST: /EDC/Point/PagingQuery
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

                using (PointServiceClient client = new PointServiceClient())
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
                        MethodReturnResult<IList<Point>> result = client.Get(ref cfg);
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
        // POST: /EDC/Point/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(PointViewModel model)
        {
            using (PointServiceClient client = new PointServiceClient())
            {
                Point obj = new Point()
                {
                    Key=Convert.ToString(Guid.NewGuid()),
                    GroupName = model.GroupName.ToUpper(),
                    ActionName=model.ActionName,
                    CategoryName=model.CategoryName,
                    EquipmentCode=model.EquipmentCode,
                    MaterialCode=model.MaterialCode,
                    MaterialType=model.MaterialType,
                    ProductionLineCode=model.ProductionLineCode,
                    RouteEnterpriseName=model.RouteEnterpriseName,
                    RouteName=model.RouteName,
                    RouteOperationName=model.RouteOperationName,
                    RouteStepName=model.RouteStepName,
                    SamplingPlanName=model.SamplingPlanName,
                    Status=model.Status,
                    Description = model.Description,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                    CreateTime = DateTime.Now,
                    Creator = User.Identity.Name
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(EDCResources.StringResource.Point_Save_Success
                                                , model.GroupName);
                }
                return Json(rst);
            }
        }
        //
        // GET: /EDC/Point/Modify
        public async Task<ActionResult> Modify(string key)
        {
            PointViewModel viewModel = new PointViewModel();
            using (PointServiceClient client = new PointServiceClient())
            {
                MethodReturnResult<Point> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    viewModel = new PointViewModel()
                    {
                        Key=result.Data.Key,
                        ActionName=result.Data.ActionName,
                        CategoryName=result.Data.CategoryName,
                        GroupName=result.Data.GroupName,
                        SamplingPlanName=result.Data.SamplingPlanName,
                        RouteStepName=result.Data.RouteStepName,
                        RouteOperationName=result.Data.RouteOperationName,
                        RouteName=result.Data.RouteName,
                        RouteEnterpriseName=result.Data.RouteEnterpriseName,
                        ProductionLineCode=result.Data.ProductionLineCode,
                        MaterialType=result.Data.MaterialType,
                        MaterialCode=result.Data.MaterialCode,
                        EquipmentCode=result.Data.EquipmentCode,
                        Status =result.Data.Status,
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
        // POST: /EDC/Point/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(PointViewModel model)
        {
            using (PointServiceClient client = new PointServiceClient())
            {
                MethodReturnResult<Point> result = await client.GetAsync(model.Key);

                if (result.Code == 0)
                {
                    result.Data.ActionName = model.ActionName;
                    result.Data.CategoryName = model.CategoryName;
                    result.Data.EquipmentCode = model.EquipmentCode;
                    result.Data.MaterialCode = model.MaterialCode;
                    result.Data.MaterialType = model.MaterialType;
                    result.Data.ProductionLineCode = model.ProductionLineCode;
                    result.Data.RouteEnterpriseName = model.RouteEnterpriseName;
                    result.Data.RouteName = model.RouteName;
                    result.Data.RouteStepName = model.RouteStepName;
                    result.Data.RouteOperationName = model.RouteOperationName;
                    result.Data.SamplingPlanName = model.SamplingPlanName;
                    result.Data.Status = model.Status;
                    result.Data.Description = model.Description;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(EDCResources.StringResource.Point_SaveModify_Success
                                                    , model.GroupName);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // GET: /EDC/Point/Detail
        public async Task<ActionResult> Detail(string key)
        {
            using (PointServiceClient client = new PointServiceClient())
            {
                MethodReturnResult<Point> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    PointViewModel viewModel = new PointViewModel()
                    {
                        Key = result.Data.Key,
                        ActionName = result.Data.ActionName,
                        CategoryName = result.Data.CategoryName,
                        GroupName = result.Data.GroupName,
                        SamplingPlanName = result.Data.SamplingPlanName,
                        RouteStepName = result.Data.RouteStepName,
                        RouteOperationName = result.Data.RouteOperationName,
                        RouteName = result.Data.RouteName,
                        RouteEnterpriseName = result.Data.RouteEnterpriseName,
                        ProductionLineCode = result.Data.ProductionLineCode,
                        MaterialType = result.Data.MaterialType,
                        MaterialCode = result.Data.MaterialCode,
                        EquipmentCode = result.Data.EquipmentCode,
                        Status=result.Data.Status,
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
        // POST: /EDC/Point/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (PointServiceClient client = new PointServiceClient())
            {
                result = await client.DeleteAsync(key);
                if (result.Code == 0)
                {
                    result.Message = string.Format(EDCResources.StringResource.Point_Delete_Success,key);
                }
                return Json(result);
            }
        }

        public ActionResult GetEquipmentCodes(string routeOperationName, string productionLineCode)
        {
            IList<Equipment> lstEquipments = new List<Equipment>();
            //根据车间和工序获取设备。
            using (EquipmentServiceClient client = new EquipmentServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"LineCode='{0}' AND EXISTS(FROM RouteOperationEquipment as p WHERE p.Key.EquipmentCode=self.Key AND p.Key.RouteOperationName='{1}')"
                                            , productionLineCode
                                            , routeOperationName)
                };
                MethodReturnResult<IList<Equipment>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstEquipments = result.Data;
                }
            }

            var lnq = from item in lstEquipments
                      select new
                      {
                          Key = item.Key,
                          Text = item.Key + "-" + item.Name
                      };
            return Json(lnq, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMaterialCodes(string materialCode,string materialType)
        {
            IList<Material> lst = new List<Material>();
            //根据物料类型获取物料。
            using (MaterialServiceClient client = new MaterialServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"Key LIKE '{0}%' AND Type = '{1}'"
                                            , materialCode
                                            , materialType)
                };
                MethodReturnResult<IList<Material>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lst = result.Data;
                }
            }

            var lnq = from item in lst
                      select new
                      {
                          value = item.Key,
                          label = item.Key
                      };
            return Json(lnq, JsonRequestBehavior.AllowGet);
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
                    OrderBy = "SortSeq"
                };
                MethodReturnResult<IList<RouteStep>> result = client.Get(ref cfg);
                if (result.Code <= 0)
                {
                    return Json(result.Data, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new List<RouteStep>(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPoint(string groupName)
        {
            using (PointServiceClient client = new PointServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("GroupName='{0}'", groupName)
                };
                MethodReturnResult<IList<Point>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data!=null && result.Data.Count>0)
                {
                    return Json(new
                                {
                                    ActionName = result.Data[0].ActionName.ToString(),
                                    CategoryName = result.Data[0].CategoryName,
                                    SamplingPlanName = result.Data[0].SamplingPlanName
                                }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }
}