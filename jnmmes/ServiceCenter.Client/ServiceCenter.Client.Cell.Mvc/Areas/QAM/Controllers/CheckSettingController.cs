using ServiceCenter.Client.Mvc.Areas.QAM.Models;
using QAMResources = ServiceCenter.Client.Mvc.Resources.QAM;
using ServiceCenter.MES.Model.QAM;
using ServiceCenter.MES.Service.Client.QAM;
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

namespace ServiceCenter.Client.Mvc.Areas.QAM.Controllers
{
    public class CheckSettingController : Controller
    {
        //
        // GET: /QAM/CheckSetting/
        public async Task<ActionResult> Index()
        {
            using (CheckSettingServiceClient client = new CheckSettingServiceClient())
            {
                await Task.Run(() =>
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        OrderBy = "GroupName"
                    };
                    MethodReturnResult<IList<CheckSetting>> result = client.Get(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PagingConfig = cfg;
                        ViewBag.List = result.Data;
                    }
                });
            }
            return View(new CheckSettingQueryViewModel());
        }
        //
        //POST: /QAM/CheckSetting/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(CheckSettingQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (CheckSettingServiceClient client = new CheckSettingServiceClient())
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
                        }
                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "GroupName",
                            Where = where.ToString()
                        };
                        MethodReturnResult<IList<CheckSetting>> result = client.Get(ref cfg);

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
        //POST: /QAM/CheckSetting/PagingQuery
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

                using (CheckSettingServiceClient client = new CheckSettingServiceClient())
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
                        MethodReturnResult<IList<CheckSetting>> result = client.Get(ref cfg);
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
        // POST: /QAM/CheckSetting/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(CheckSettingViewModel model)
        {
            using (CheckSettingServiceClient client = new CheckSettingServiceClient())
            {
                CheckSetting obj = new CheckSetting()
                {
                    Key=Convert.ToString(Guid.NewGuid()),
                    GroupName = model.GroupName.ToUpper(),
                    ActionName=model.ActionName,
                    EquipmentCode=model.EquipmentCode,
                    MaterialCode=model.MaterialCode,
                    MaterialType=model.MaterialType,
                    ProductionLineCode=model.ProductionLineCode,
                    RouteEnterpriseName=model.RouteEnterpriseName,
                    RouteName=model.RouteName,
                    RouteOperationName=model.RouteOperationName,
                    RouteStepName=model.RouteStepName,
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
                    rst.Message = string.Format(QAMResources.StringResource.CheckSetting_Save_Success
                                                , model.GroupName);
                }
                return Json(rst);
            }
        }
        //
        // GET: /QAM/CheckSetting/Modify
        public async Task<ActionResult> Modify(string key)
        {
            CheckSettingViewModel viewModel = new CheckSettingViewModel();
            using (CheckSettingServiceClient client = new CheckSettingServiceClient())
            {
                MethodReturnResult<CheckSetting> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    viewModel = new CheckSettingViewModel()
                    {
                        Key=result.Data.Key,
                        ActionName=result.Data.ActionName,
                        GroupName=result.Data.GroupName,
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
        // POST: /QAM/CheckSetting/SaveModify
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveModify(CheckSettingViewModel model)
        {
            using (CheckSettingServiceClient client = new CheckSettingServiceClient())
            {
                MethodReturnResult<CheckSetting> result = await client.GetAsync(model.Key);

                if (result.Code == 0)
                {
                    result.Data.ActionName = model.ActionName;
                    result.Data.EquipmentCode = model.EquipmentCode;
                    result.Data.MaterialCode = model.MaterialCode;
                    result.Data.MaterialType = model.MaterialType;
                    result.Data.ProductionLineCode = model.ProductionLineCode;
                    result.Data.RouteEnterpriseName = model.RouteEnterpriseName;
                    result.Data.RouteName = model.RouteName;
                    result.Data.RouteStepName = model.RouteStepName;
                    result.Data.RouteOperationName = model.RouteOperationName;
                    result.Data.Status = model.Status;
                    result.Data.Description = model.Description;
                    result.Data.Editor = User.Identity.Name;
                    result.Data.EditTime = DateTime.Now;
                    MethodReturnResult rst = await client.ModifyAsync(result.Data);
                    if (rst.Code == 0)
                    {
                        rst.Message = string.Format(QAMResources.StringResource.CheckSetting_SaveModify_Success
                                                    , model.GroupName);
                    }
                    return Json(rst);
                }
                return Json(result);
            }
        }
        //
        // GET: /QAM/CheckSetting/Detail
        public async Task<ActionResult> Detail(string key)
        {
            using (CheckSettingServiceClient client = new CheckSettingServiceClient())
            {
                MethodReturnResult<CheckSetting> result = await client.GetAsync(key);
                if (result.Code == 0)
                {
                    CheckSettingViewModel viewModel = new CheckSettingViewModel()
                    {
                        Key = result.Data.Key,
                        ActionName = result.Data.ActionName,
                        GroupName = result.Data.GroupName,
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
        // POST: /QAM/CheckSetting/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (CheckSettingServiceClient client = new CheckSettingServiceClient())
            {
                result = await client.DeleteAsync(key);
                if (result.Code == 0)
                {
                    result.Message = string.Format(QAMResources.StringResource.CheckSetting_Delete_Success,key);
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

        public ActionResult GetCheckSetting(string groupName)
        {
            using (CheckSettingServiceClient client = new CheckSettingServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("GroupName='{0}'", groupName)
                };
                MethodReturnResult<IList<CheckSetting>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data!=null && result.Data.Count>0)
                {
                    return Json(new
                                {
                                    ActionName = result.Data[0].ActionName.ToString()
                                }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }
}