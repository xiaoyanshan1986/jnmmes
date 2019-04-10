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
using ServiceCenter.MES.Service.Client.ERP;
using System.Data;
using ServiceCenter.MES.Service.Contract.ERP;

namespace ServiceCenter.Client.Mvc.Areas.WIP.Controllers
{
    public class PackageReworkController : Controller
    {
        //
        // GET: /WIP/PackageRework/
        public ActionResult Index()
        {
            return View(new LotReworkViewModel());
        }
        //
        // POST: /WIP/PackageRework/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(LotReworkViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();
            string error_mes = string.Empty;
            try
            {
                ReworkParameter p = new ReworkParameter()
                {
                    Creator = User.Identity.Name,
                    OperateComputer = Request.UserHostAddress,
                    Operator = User.Identity.Name,
                    RouteEnterpriseName = model.RouteEnterpriseName,
                    RouteName = model.RouteName,
                    RouteStepName = model.RouteStepName,
                    Remark = model.Description,
                    LotNumbers = new List<string>(),
                    MaterialCode = model.NewMaterialCode,
                    OrderNumber = model.NewOrderNumber,
                    IsPackageRework = true
                };
                //按包装返工。
                //string lotNumber = model.LotNumber.ToUpper();
                //string[] arrlotNumber = model.LotNumber.Split(',');
                //foreach (var data in arrlotNumber)
                //{
                //    using (PackageQueryServiceClient client = new PackageQueryServiceClient())
                //    {

                //        PagingConfig cfg = new PagingConfig()
                //        {
                //            IsPaging = false,
                //            Where = string.Format("Key.PackageNo='{0}'", data.Trim())
                //        };
                //        MethodReturnResult<IList<PackageDetail>> rst1 = client.GetDetail(ref cfg);
                //        if (rst1.Code > 0)
                //        {
                //            return Json(rst1);
                //        }

                //        foreach (PackageDetail item in rst1.Data)
                //        {
                //            p.LotNumbers.Add(item.Key.ObjectNumber);
                //        }
                //    }
                //}

                using (PackageQueryServiceClient client = new PackageQueryServiceClient())
                {      
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format("Key.PackageNo='{0}'", model.LotNumber)
                    };
                    MethodReturnResult<IList<PackageDetail>> rst1 = client.GetDetail(ref cfg);
                    if (rst1.Code > 0)
                    {
                        return Json(rst1);
                    }

                    foreach (PackageDetail item in rst1.Data)
                    {
                        p.LotNumbers.Add(item.Key.ObjectNumber);
                    }
                }
              
                if (p.LotNumbers.Count <= 0)
                {
                    //result.Code = 5;
                    //result.Message = string.Format("包装 {0} 不存在。", model.LotNumber);
                    //return Json(result);
                    error_mes = REback(model.LotNumber);
                    if (!string.IsNullOrEmpty(error_mes))
                    {
                        result.Code = 5;
                        result.Message = string.Format(error_mes);
                        return Json(result);
                    }
                }
                //string error_mes = string.Empty;
                //error_mes = REback(p.LotNumbers[0]);
                //if (!string.IsNullOrEmpty(error_mes))
                //{
                //    result.Code = 5;
                //    result.Message = string.Format(error_mes);
                //    return Json(result);
                //}
                using (LotReworkServiceClient client = new LotReworkServiceClient())
                {
                    result = client.Rework(p);
                }
                if (result.Code == 0)
                {
                    result.Message = string.Format(" {0} 投料操作成功。", model.LotNumber);
                    //using (LotReworkServiceClient client = new LotReworkServiceClient())
                    //{                        
                    //   result = client.LotUpdate();
                    //   if (result.Code == 0)
                    //   {
                    //       result.Message =result.Message + string.Format(" 更新批次操作成功。");
                    //   }
                    //}
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
        public string REback(string lotNumber)
        {
            MethodReturnResult result = new MethodReturnResult();
            using (ERPClient client = new ERPClient())
            {
                REbackdataParameter param = new REbackdataParameter();
                param.PackageNo = lotNumber;
                param.ErrorMsg = "";
                param.ReType = 2;
                param.IsDelete = 1;
                result = client.GetREbackdata(param);
                //if (result.Code == 1000)
                //{
                //    result.Message += string.Format(WIPResources.StringResource.REbackdata_Fail);
                //}
            }
            return result.Message;
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
            else if (obj.StateFlag < EnumLotState.Finished)
            {
                result.Code = 2002;
                result.Message = string.Format("批次({0})未完成。", lotNumber);
                return result;
            }
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
            else if (obj.PackageFlag == true)
            {
                result.Code = 2005;
                result.Message = string.Format("批次({0})已入包装。", lotNumber);
                return result;
            }
            return rst;
        }

        public ActionResult GetOrderNumberByPackageNo(string lotNumber)
        {
            MethodReturnResult<DataSet> ds = new MethodReturnResult<DataSet>();

            if (lotNumber.Contains(','))
            {
                lotNumber = lotNumber.Split(',')[0].ToString();
            }
            using (ERPClient client = new ERPClient())
            {
                ds = client.GetReceiptOrderNumberByPackageNo(lotNumber);
            }


            //MethodReturnResult<Package> rst = new MethodReturnResult<Package>();
            //List<Package> listPackage = new List<Package>();
            //using (PackageQueryServiceClient client = new PackageQueryServiceClient())
            //{
            //    rst = client.Get(lotNumber);
            //    if (rst.Code <= 0 && rst.Data != null)
            //    {
            //        listPackage.Add(rst.Data);
            //    }
            //}       
            var data = from item in ds.Data.Tables[0].AsEnumerable()
                       select new { OrderNumber = item["ORDER_NUMBER"] };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetOrderNumber(string orderNumber)
        {
            IList<WorkOrderProduct> lstWorkOrderProduct = new List<WorkOrderProduct>();
            using (WorkOrderProductServiceClient client = new WorkOrderProductServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Key.OrderNumber='{0}'", orderNumber),
                    OrderBy = "ItemNo"
                };
                MethodReturnResult<IList<WorkOrderProduct>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null && result.Data.Count > 0)
                {
                    lstWorkOrderProduct = result.Data;
                }
            }
            return Json(from item in lstWorkOrderProduct
                        select new
                        {
                            Text = item.Key.MaterialCode,
                            Value = item.Key.MaterialCode
                        }, JsonRequestBehavior.AllowGet);
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
                    Where = string.Format("Key.OrderNumber='{0}' AND IsRework = false", orderNumber),
                    OrderBy = "Key.ItemNo"
                };

                MethodReturnResult<IList<WorkOrderRoute>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null && result.Data.Count > 0)
                {
                    lstWorkOrderRoute = result.Data;
                }
            }
            var lnq = from item in lstWorkOrderRoute
                      select new
                      {
                          RouteEnterpriseName = item.RouteEnterpriseName,
                          RouteName = item.RouteName,
                          RouteStepName = item.RouteStepName
                      };

            return Json(lnq, JsonRequestBehavior.AllowGet);
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
                    IsPaging = false,
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