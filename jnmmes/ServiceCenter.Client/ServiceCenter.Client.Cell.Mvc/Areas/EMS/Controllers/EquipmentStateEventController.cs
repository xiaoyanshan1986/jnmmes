using ServiceCenter.Client.Mvc.Areas.EMS.Models;
using ServiceCenter.MES.Model.EMS;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Model.RBAC;
using ServiceCenter.MES.Service.Client.EMS;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Service.Client.RBAC;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using EMSResources = ServiceCenter.Client.Mvc.Resources.EMS;

namespace ServiceCenter.Client.Mvc.Areas.EMS.Controllers
{
    public class EquipmentStateEventController : Controller
    {
        //
        // GET: /EMS/EquipmentStateEvent/
        public async Task<ActionResult> Index()
        {
            return await Query(new EquipmentStateEventQueryViewModel());
        }
        //
        //POST: /EMS/EquipmentStateEvent/Query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Query(EquipmentStateEventQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (EquipmentStateEventServiceClient client = new EquipmentStateEventServiceClient())
                {
                    await Task.Run(() =>
                    {
                        StringBuilder where = new StringBuilder();
                        if (model != null)
                        {
                            if (string.IsNullOrEmpty(model.EquipmentCode))
                            {
                                where.AppendFormat(" {0} EquipmentCode LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.EquipmentCode);
                            }

                            if (!string.IsNullOrEmpty(model.ChangeStateName))
                            {
                                where.AppendFormat(" {0} EquipmentChangeStateName LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.ChangeStateName);
                            }

                            if (!string.IsNullOrEmpty(model.FromStateName))
                            {
                                where.AppendFormat(" {0} EquipmentFromStateName LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.FromStateName);
                            }

                            if (!string.IsNullOrEmpty(model.ToStateName))
                            {
                                where.AppendFormat(" {0} EquipmentToStateName LIKE '{1}%'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.ToStateName);
                            }

                            if (model.EndCreateTime!=null)
                            {
                                where.AppendFormat(" {0} CreateTime <= '{1:yyyy-MM-dd HH:mm:ss}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.EndCreateTime);
                            }

                            if (model.StartCreateTime!=null)
                            {
                                where.AppendFormat(" {0} CreateTime >= '{1:yyyy-MM-dd HH:mm:ss}'"
                                                    , where.Length > 0 ? "AND" : string.Empty
                                                    , model.StartCreateTime);
                            }
                        }

                        PagingConfig cfg = new PagingConfig()
                        {
                            OrderBy = "CreateTime DESC",
                            Where = where.ToString()
                        };

                        MethodReturnResult<IList<EquipmentStateEvent>> result = client.Get(ref cfg);

                        if (result.Code == 0)
                        {
                            ViewBag.PagingConfig = cfg;
                            ViewBag.List = result.Data;
                        }
                    });
                }
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("_ListPartial",new EquipmentStateEventViewModel());
            }
            else
            {
                return View("Index", model);
            }
        }
        //
        //POST: /EMS/EquipmentStateEvent/PagingQuery
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

                using (EquipmentStateEventServiceClient client = new EquipmentStateEventServiceClient())
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
                        MethodReturnResult<IList<EquipmentStateEvent>> result = client.Get(ref cfg);
                        if (result.Code == 0)
                        {
                            ViewBag.PagingConfig = cfg;
                            ViewBag.List = result.Data;
                        }
                    });
                }
            }
            return PartialView("_ListPartial", new EquipmentStateEventViewModel());
        }
        //
        // POST: /EMS/EquipmentStateEvent/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(EquipmentStateEventViewModel model)
        {
            using (EquipmentStateEventServiceClient client = new EquipmentStateEventServiceClient())
            {
                EquipmentStateEvent obj = new EquipmentStateEvent()
                {
                    Key  = Guid.NewGuid().ToString(),
                    EquipmentChangeStateName=model.ChangeStateName,
                    EquipmentCode=model.EquipmentCode,
                    EquipmentFromStateName=model.FromStateName,
                    EquipmentToStateName=model.ToStateName,
                    Description = model.Description,
                    Editor = User.Identity.Name,
                    EditTime = DateTime.Now,
                    CreateTime = DateTime.Now,
                    Creator = User.Identity.Name
                };
                MethodReturnResult rst = await client.AddAsync(obj);
                if (rst.Code == 0)
                {
                    rst.Message = string.Format(EMSResources.StringResource.EquipmentStateEvent_Save_Success
                                                , model.ChangeStateName);
                }
                return Json(rst);
            }
        }


        public ActionResult GetEquipmentCodes(string routeOperationName, string productionLineCode)
        {
            IList<Equipment> lstEquipments = new List<Equipment>();
            //根据生产线和工序获取设备。
            using (EquipmentServiceClient client = new EquipmentServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"LineCode='{0}' 
                                            AND EXISTS(FROM RouteOperationEquipment as p 
                                                       WHERE p.Key.EquipmentCode=self.Key 
                                                       AND p.Key.RouteOperationName='{1}')"
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

        public ActionResult GetEquipmentState(string equipmentCode)
        {
            string stateName = string.Empty;
            //根据生产线和工序获取设备。
            using (EquipmentServiceClient client = new EquipmentServiceClient())
            {
                MethodReturnResult<Equipment> result = client.Get(equipmentCode);
                if (result.Code <= 0 && result.Data != null)
                {
                    stateName = result.Data.StateName;
                }
            }
            string stateColor = string.Empty;
            string description=string.Empty;
            if (!string.IsNullOrEmpty(stateName))
            {
                using (EquipmentStateServiceClient client = new EquipmentStateServiceClient())
                {
                    MethodReturnResult<EquipmentState> result = client.Get(stateName);
                    if (result.Code <= 0 && result.Data != null)
                    {
                        stateColor = result.Data.StateColor;
                        description=result.Data.Description;
                    }
                }
            }
            return Json(new
            {
                StateName = stateName,
                StateColor = stateColor,
                Description = description
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetEquipmentChangeState(string stateName)
        {
            //获取用户拥有权限的设备状态。
            IList<Resource> lstResource = new List<Resource>();
            using (UserAuthenticateServiceClient client = new UserAuthenticateServiceClient())
            {
                MethodReturnResult<IList<Resource>> result = client.GetResourceList(User.Identity.Name, ResourceType.EquipmentState);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstResource = result.Data;
                }
            }
            var lnq=from item in lstResource
                    where item.Data==stateName
                    select item;

            IList<EquipmentChangeState> lst = new List<EquipmentChangeState>();
            if(lnq.Count()>0)
            {
                //根据设备状态获取设备状态事件。
                using (EquipmentChangeStateServiceClient client = new EquipmentChangeStateServiceClient())
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format(@"FromStateName='{0}'", stateName)
                    };
                    MethodReturnResult<IList<EquipmentChangeState>> result = client.Get(ref cfg);
                    if (result.Code <= 0 && result.Data != null)
                    {
                        lst = result.Data;
                    }
                }
            }


            var lnq1 = from item in lst
                       select new
                       {
                          Key = item.Key,
                          Text = item.Key,
                          ToStateName = item.ToStateName
                       };
            return Json(lnq1, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult GetEquipmentStateColor(string stateName)
        {
            string stateColor = string.Empty;
            string description = string.Empty;
            //根据设备状态获取设备状态。
            using (EquipmentStateServiceClient client = new EquipmentStateServiceClient())
            {
                MethodReturnResult<EquipmentState> result = client.Get(stateName);
                if (result.Code <= 0 && result.Data != null)
                {
                    stateColor = result.Data.StateColor;
                    description = result.Data.Description;
                }
            }
            return Json(new
            {
                StateName = stateName,
                StateColor = stateColor,
                Description = description
            }, JsonRequestBehavior.AllowGet);
        }
	}
}