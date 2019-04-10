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
            return View(new PackageReworkViewModel());
        }
        //
        // POST: /WIP/PackageRework/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(PackageReworkViewModel model)
       {
            MethodReturnResult result = new MethodReturnResult();
            string error_mes = string.Empty;            

            try
            {
                //创建参数
                PackageReworkParameter p = new PackageReworkParameter()
                {
                    Creator = User.Identity.Name,
                    OperateComputer = Request.UserHostAddress,
                    Operator = User.Identity.Name,
                    PackageNo = model.PackageNo,                //托号
                    LotNumber = model.LotNumber,                //批次号
                    LocationName = model.LocationName,          //车间
                    LineCode = model.LineCode,                  //线别
                    RetainPackageNo = model.RetainPackageNo,    //是否保留托号
                    IsLot = model.IsLot                         //是否按批次号投料
                };

                if (model.LotNumber != "" && model.LotNumber != null)
                {
                    p.LotNumber = p.LotNumber.Trim();
                    char splitChar = ',';
                    //获取批次号值。
                    string[] lotNumbers = p.LotNumber.ToUpper().Split(splitChar);
                    p.LotNumbers = lotNumbers.ToList();
                }
                if (model.PackageNo != "" && model.PackageNo != null && p.IsLot == false)
                {
                    p.PackageNo = p.PackageNo.Trim();
                    char splitChar = ',';
                    //获取托号值。
                    string[] packageNos = p.PackageNo.ToUpper().Split(splitChar);
                    p.PackageNos = packageNos.ToList();
                }
               
                //整托投料不得输入批次号
                if (p.IsLot == false && (p.LotNumber != "" && p.LotNumber != null))
                {
                    result.Code = 1000;
                    result.Message = string.Format(" 未勾选按批次号投料，请勿输入批次号！");
                }
                //按批次号投料必须输批次号
                else if (p.IsLot == true && (p.LotNumber == "" || p.LotNumber == null))
                {
                    result.Code = 1000;
                    result.Message = string.Format(" 已勾选按批次号投料，请输入批次号！");
                }
                else
                {                    
                    using (LotReworkServiceClient client = new LotReworkServiceClient())
                    {
                        string message = "";
                        if (p.IsLot)
                        {
                            for (int i = 0; i < p.LotNumbers.Count; i++)
                            {
                                p.LotNumber = p.LotNumbers[i];
                                result = client.Rework(p);
                                message = result.Message;

                                if (result.Code == 0)
                                {
                                    if (p.LotNumbers.Count == 1)
                                    {
                                        result.Message = string.Format(" 托号 {0} 中批次 {1} 投料操作成功！", model.PackageNo, p.LotNumber);
                                    }
                                    if(p.LotNumbers.Count > 1)
                                    {
                                        result.Message = string.Format(" 投料操作成功！") ;
                                    }
                                }
                            }                           
                        }
                        else
                        {
                            for (int i = 0; i < p.PackageNos.Count; i++)
                            {
                                p.PackageNo = p.PackageNos[i];
                                result = client.Rework(p);
                                message = result.Message;

                                if (result.Code == 0)
                                {
                                    if (p.PackageNos.Count == 1)
                                    {
                                        result.Message = string.Format(" {0} 投料操作成功！", model.PackageNo);
                                    }
                                    if (p.PackageNos.Count > 1)
                                    {
                                        result.Message = string.Format("投料操作成功！");
                                    }
                                }
                            }        
                        }
                        if (result.Code == 0)
                        {
                            result.Message += message;
                        }                       
                    }      
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

        /// <summary>
        /// 根据车间取得相应的线别列表
        /// </summary>
        /// <param name="locatioName"></param>
        /// <returns></returns>
        public ActionResult GetLineCodeListByLocation(string locationName)
        {
            IList<ProductionLine> lst = new List<ProductionLine>();

            using (ProductionLineServiceClient client = new ProductionLineServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"EXISTS (FROM Location as p
                                                    WHERE p.Key=self.LocationName
                                                    AND p.ParentLocationName='{0}'
                                                    AND p.Level='{1}')"
                                           , locationName
                                           , Convert.ToInt32(LocationLevel.Area)),
                    OrderBy = "Key"
                };

                MethodReturnResult<IList<ProductionLine>> result = client.Get(ref cfg);

                if (result.Code <= 0 && result.Data != null)
                {
                    lst = result.Data;
                }
            }

            return Json(from item in lst
                        select new
                        {
                            Text = item.Name,
                            Value = item.Key
                        }, JsonRequestBehavior.AllowGet);
        }
    }
}