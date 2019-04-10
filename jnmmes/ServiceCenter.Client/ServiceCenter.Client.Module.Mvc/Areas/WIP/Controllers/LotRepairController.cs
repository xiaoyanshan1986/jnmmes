using ServiceCenter.Client.Mvc.Areas.WIP.Models;
using ServiceCenter.MES.Model.BaseData;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Model.LSM;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Service.Client.BaseData;
using ServiceCenter.MES.Service.Client.FMM;
using ServiceCenter.MES.Service.Client.LSM;
using ServiceCenter.MES.Service.Client.PPM;
using ServiceCenter.MES.Service.Client.WIP;
using ServiceCenter.MES.Service.Contract.WIP;
using WIPResources = ServiceCenter.Client.Mvc.Resources.WIP;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ServiceCenter.Client.Mvc.Areas.WIP.Controllers
{
    public class LotRepairController : Controller
    {
        //
        // GET: /WIP/LotRepair/
        public ActionResult Index()
        {
            return View(new LotRepairViewModel());
        }
        //
        // POST: /WIP/LotRepair/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(LotRepairViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                RepairParameter p = new RepairParameter()
                {
                    Creator = User.Identity.Name,
                    OperateComputer = Request.UserHostAddress,
                    Operator = User.Identity.Name,
                    RouteEnterpriseName=model.RouteEnterpriseName,
                    RouteName=model.RouteName,
                    RouteStepName=model.RouteStepName,
                    Remark = model.Description,
                    LotNumbers = new List<string>()
                };
                //获取批值。
                string lotNumber = model.LotNumber.ToUpper();
                result = GetLot(lotNumber);
                if (result.Code > 0)
                {
                    return Json(result);
                }
                p.LotNumbers.Add(lotNumber);

                //批次返修操作。
                using (LotRepairServiceClient client = new LotRepairServiceClient())
                {
                    result = client.Repair(p);
                }
                if (result.Code == 0)
                {
                    result.Message = string.Format("批次 {0} 返修操作成功。",model.LotNumber);
                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }
            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return Json(result);
        }

        public MethodReturnResult GetLot(string lotNumber)
        {
            MethodReturnResult result = new MethodReturnResult();
            MethodReturnResult<Lot> rst = null;
            Lot obj = null;
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                rst = client.Get(lotNumber);
                if (rst.Code <= 0 && rst.Data != null)
                {
                    obj = rst.Data;
                }
                else
                {
                    result.Code = rst.Code;
                    result.Message = rst.Message;
                    result.Detail = rst.Detail;
                    return result;
                }
            }
            if (obj == null || obj.Status == EnumObjectStatus.Disabled)
            {
                result.Code = 2001;
                result.Message = string.Format(WIPResources.StringResource.LotIsNotExists, lotNumber);
                return result;
            }
            //else if (obj.StateFlag == EnumLotState.Finished)
            //{
            //    result.Code = 2002;
            //    result.Message = string.Format("批次({0})已完成。", lotNumber);
            //    return result;
            //}
            else if (obj.Status == EnumObjectStatus.Disabled || obj.DeletedFlag == true)
            {
                result.Code = 2003;
                result.Message = string.Format("批次({0})已结束。", lotNumber);
                return result;
            }
            else if (obj.HoldFlag == true)
            {
                result.Code = 2004;
                result.Message = string.Format("批次({0})已暂停。", lotNumber);
                return result;
            }
            return rst;
        }

        public ActionResult GetLotInfo(string lotNumber)
        {
            MethodReturnResult result = GetLot(lotNumber);
            if (result.Code > 0)
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json(result as MethodReturnResult<Lot>, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRouteEnterpriseNames(string orderNumber)
        {
            IList<WorkOrderRoute> lstWorkOrderRoute = new List<WorkOrderRoute>();

            //获取工单工艺信息。
            using (WorkOrderRouteServiceClient client = new WorkOrderRouteServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Key.OrderNumber='{0}' AND IsRework=1", orderNumber),
                    OrderBy = "Key.ItemNo"
                };

                MethodReturnResult<IList<WorkOrderRoute>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null && result.Data.Count > 0)
                {
                    lstWorkOrderRoute = result.Data;
                }
            }
            var lnq = from item in lstWorkOrderRoute
                      select item.RouteEnterpriseName;

            return Json(from item in lnq.Distinct()
                        select new
                        {
                            Text = item,
                            Value = item
                        }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRouteNames(string routeEnterpriseName)
        {
            IList<RouteEnterpriseDetail> lstRouteEnterpriseDetail = new List<RouteEnterpriseDetail>();
            using (RouteEnterpriseDetailServiceClient client = new RouteEnterpriseDetailServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Key.RouteEnterpriseName='{0}'", routeEnterpriseName),
                    OrderBy = "ItemNo"
                };
                MethodReturnResult<IList<RouteEnterpriseDetail>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstRouteEnterpriseDetail = result.Data;
                }
            }
            return Json(from item in lstRouteEnterpriseDetail
                        select new
                        {
                            Text = item.Key.RouteName,
                            Value = item.Key.RouteName
                        }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRouteStepNames(string routeName)
        {
            IList<RouteStep> lst = new List<RouteStep>();

            using (RouteStepServiceClient client = new RouteStepServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = true,
                    PageNo =0,
                    PageSize =1,
                    Where = string.Format("Key.RouteName='{0}'", routeName),
                    OrderBy = "SortSeq"
                };
                MethodReturnResult<IList<RouteStep>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lst = result.Data;
                }
            }
            return Json(from item in lst
                        select new
                        {
                            Text = item.Key.RouteStepName,
                            Value = item.Key.RouteStepName
                        }, JsonRequestBehavior.AllowGet);
        }


	}
}