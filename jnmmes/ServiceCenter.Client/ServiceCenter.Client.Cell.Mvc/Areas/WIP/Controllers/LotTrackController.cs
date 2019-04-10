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
using ServiceCenter.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ServiceCenter.Client.Mvc.Areas.WIP.Controllers
{
    public class LotTrackController : Controller
    {
        //
        // GET: /WIP/LotTrack/
        /// <summary>
        /// 显示工作站作业界面。
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View(new LotTrackViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(LotTrackViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();
            Response.StatusDescription = "JSON";
            try
            {
                string lotNumber = model.LotNumber.ToUpper();
                result = GetLot(lotNumber);
                if (result.Code > 0)
                {
                    return Json(result);
                }
                
                MethodReturnResult<Lot> rst = result as MethodReturnResult<Lot>;
                Lot obj = rst.Data;
                //等待进站批次，设备代码必须选择。
                if (obj.StateFlag == EnumLotState.WaitTrackIn 
                    && string.IsNullOrEmpty(model.EquipmentCode))
                {
                    result.Code = 1;
                    result.Message = string.Format("设备代码不能为空。");
                    return Json(result);
                }
                //判断批次工序是否在当前工序。
                if (obj.RouteStepName != model.RouteOperationName)
                {
                    result.Code = 2;
                    result.Message = string.Format("批次({0})当前所在工序（{1}），不能在（{2}）工序上操作。"
                                                    ,obj.Key
                                                    ,obj.RouteStepName
                                                    ,model.RouteOperationName);
                    return Json(result);
                }
                //判断批次所在车间和当前线边所在车间是否匹配。
                //获取线别车间。
                string locationName = string.Empty;
                using (ProductionLineServiceClient client = new ProductionLineServiceClient())
                {
                    MethodReturnResult<ProductionLine> r = client.Get(model.LineCode);
                    if (r.Code <= 0)
                    {
                        locationName = r.Data.LocationName;
                    }
                }
                if (!string.IsNullOrEmpty(locationName))
                {
                    using (LocationServiceClient client = new LocationServiceClient())
                    {
                        MethodReturnResult<Location> r = client.Get(locationName);
                        if (r.Code <= 0)
                        {
                            locationName = r.Data.ParentLocationName;
                        }
                    }
                }
                //检查批次车间和线别车间是否匹配。
                if (obj.LocationName != locationName)
                {
                    result.Code = 3;
                    result.Message = string.Format("批次（{0}）属于({1})车间，不能在({2})车间线别上操作。"
                                                    , lotNumber
                                                    , obj.LocationName
                                                    , locationName);
                    return Json(result);
                }
                bool isShowModal = false;
                //获取工序参数列表。
                IList<RouteStepParameter> lstRouteStepParameter = GetParameterList(obj.RouteName, obj.RouteStepName, obj.StateFlag);
                if (lstRouteStepParameter != null && lstRouteStepParameter.Count > 0) //需要显示工序参数录入表单。
                {
                    isShowModal = true;
                }
                //出站，判断是否显示不良和报废录入对话框。
                if (obj.StateFlag==EnumLotState.WaitTrackOut)
                {
                    IList<RouteStepAttribute> lstRouteStepAttribute = new List<RouteStepAttribute>();
                    using (RouteStepAttributeServiceClient client = new RouteStepAttributeServiceClient())
                    {
                        PagingConfig cfg = new PagingConfig()
                        {
                            IsPaging=false,
                            Where = string.Format("Key.RouteName='{0}' AND Key.RouteStepName='{1}'"
                                                  ,obj.RouteName
                                                  ,obj.RouteStepName)
                        };
                        MethodReturnResult<IList<RouteStepAttribute>> r = client.Get(ref cfg);
                        if (r.Code <= 0 && r.Data != null)
                        {
                            lstRouteStepAttribute = r.Data;
                        }
                    }

                    //是否输入等级。
                    bool isInputGrade = false;
                    var lnq = from item in lstRouteStepAttribute
                              where item.Key.AttributeName == "IsInputGrade"
                              select item;
                    RouteStepAttribute rsaTmp = lnq.FirstOrDefault();
                    if (rsaTmp!=null)
                    {
                        bool.TryParse(rsaTmp.Value, out isInputGrade);
                    }
                    //是否输入花色。
                    bool isInputColor = false;
                    lnq = from item in lstRouteStepAttribute
                          where item.Key.AttributeName == "IsInputColor"
                          select item;
                    rsaTmp = lnq.FirstOrDefault();
                    if (rsaTmp != null)
                    {
                        bool.TryParse(rsaTmp.Value, out isInputColor);
                    }
                   
                    //获取是否显示不良原因录入对话框。
                    bool isShowDefectModal = false;
                    lnq = from item in lstRouteStepAttribute
                          where item.Key.AttributeName == "IsShowDefectModal"
                          select item;
                    rsaTmp = lnq.FirstOrDefault();
                    if (rsaTmp != null)
                    {
                        bool.TryParse(rsaTmp.Value, out isShowDefectModal);
                    }
                    //获取是否显示报废原因录入对话框。
                    bool isShowScrapModal = false;
                    lnq = from item in lstRouteStepAttribute
                          where item.Key.AttributeName == "IsShowScrapModal"
                          select item;
                    rsaTmp = lnq.FirstOrDefault();
                    if (rsaTmp != null)
                    {
                        bool.TryParse(rsaTmp.Value, out isShowScrapModal);
                    }
                    //是否输入等级
                    if (isInputGrade)
                    {
                        ViewBag.IsInputGrade = isInputGrade;
                        isShowModal = true;
                    }
                    //是否输入花色
                    if (isInputColor)
                    {
                        ViewBag.IsInputColor = isInputColor;
                        isShowModal = true;
                    }
                    //是否显示不良原因录入对话框。
                    if (isShowDefectModal)
                    {
                        IList<ReasonCodeCategoryDetail> lstDefectReasonCodes = GetDefectReasonCodes(obj.RouteName, obj.RouteStepName);
                        if (lstDefectReasonCodes!=null && lstDefectReasonCodes.Count > 0)
                        {
                            isShowModal = true;
                        }
                    }
                    //是否显示报废原因录入对话框。
                    if (isShowScrapModal)
                    {
                        IList<ReasonCodeCategoryDetail> lstScrapReasonCodes = GetScrapReasonCodes(obj.RouteName, obj.RouteStepName);
                        if (lstScrapReasonCodes!=null && lstScrapReasonCodes.Count > 0)
                        {
                            isShowModal = true;
                        }
                    }
                }
                //显示附加对话框。
                if(isShowModal)
                {
                    Response.StatusDescription = "Partial";
                    ViewBag.Lot = obj;
                    return PartialView("_ModalContentPartial",new LotTrackViewModel());
                }
                result = Track(obj, model);
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

        //
        // POST: /WIP/LotTrack/SaveModal
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveModal(LotTrackViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                string lotNumber = model.LotNumber.ToUpper();
                result = GetLot(lotNumber);
                if (result.Code > 0)
                {
                    return Json(result);
                }
                MethodReturnResult<Lot> rst = result as MethodReturnResult<Lot>;
                result = Track(rst.Data, model);

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

        /// <summary>
        /// 批次过站作业。
        /// </summary>
        /// <param name="obj">批次对象。</param>
        /// <param name="model">过站模型对象。</param>
        /// <returns>返回结果。</returns>
        private MethodReturnResult Track(Lot obj, LotTrackViewModel model)
        {

            string lotNumber = model.LotNumber.ToUpper();
            MethodReturnResult result = new MethodReturnResult();
            IDictionary<string, IList<TransactionParameter>> dicParams = new Dictionary<string, IList<TransactionParameter>>();
            //获取工序参数列表。
            IList<RouteStepParameter> lstRouteStepParameter = GetParameterList(obj.RouteName, obj.RouteStepName, obj.StateFlag);
            //组织批次附加参数。
            if(lstRouteStepParameter!=null)
            {
                foreach (RouteStepParameter item in lstRouteStepParameter)
                {
                    string hashcode = string.Format("{0}{1}{2}", item.Key.RouteName, item.Key.RouteStepName, item.Key.ParameterName)
                                      .GetHashCode()
                                      .ToString()
                                      .Replace('-', '_');
                    string paramName = string.Format("PARAM_{0}", hashcode);
                    string val = Request.Form[paramName];
                    //记录上一次值。
                    if (item.IsUsePreValue)
                    {
                        if (Request.Cookies.Get(paramName) != null)
                        {
                            Response.SetCookie(new HttpCookie(paramName, val));
                        }
                        else if(!string.IsNullOrEmpty(val))
                        {
                            Response.Cookies.Add(new HttpCookie(paramName, val));
                        }
                    }
                    if (string.IsNullOrEmpty(val))
                    {
                        continue;
                    }
                    if (!dicParams.ContainsKey(obj.Key))
                    {
                        dicParams.Add(obj.Key, new List<TransactionParameter>());
                    }
                    if (item.DataType == EnumDataType.Boolean)
                    {
                        val = val == "on" ? "true" : "false";
                    }

                   


                    TransactionParameter tp = new TransactionParameter()
                    {
                        Index = item.ParamIndex,
                        Name = item.Key.ParameterName,
                        Value = val
                    };
                    dicParams[obj.Key].Add(tp);
                }
            }
            //批次当前状态为等待进站。
            if (obj.StateFlag == EnumLotState.WaitTrackIn)
            {
                TrackInParameter p = new TrackInParameter()
                {
                    Creator = User.Identity.Name,
                    EquipmentCode = model.EquipmentCode,
                    LineCode = model.LineCode,
                    LotNumbers = new List<string>(),
                    OperateComputer = Request.UserHostAddress,
                    Operator = User.Identity.Name,
                    Paramters = dicParams,
                    Remark = model.Description,
                    RouteOperationName = model.RouteOperationName
                };
                p.LotNumbers.Add(lotNumber);
                //进行批次进站。
                using (LotTrackInServiceClient client = new LotTrackInServiceClient())
                {
                    result = client.TrackIn(p);
                    if (result.Code == 0)
                    {
                        if (!string.IsNullOrEmpty(result.Message))
                        {
                            if (!result.Message.EndsWith("\n"))
                            {
                                result.Message += "\n";
                            }
                            result.Message = result.Message.Replace("\n", "<br/>");
                        }
                        result.Message = string.Format("批次 {0} 进站成功。", lotNumber);
                    }
                }
            }
            //批次当前状态为等待出站。
            else if(obj.StateFlag==EnumLotState.WaitTrackOut)
            {

                TrackOutParameter p = new TrackOutParameter()
                {
                    Creator = User.Identity.Name,
                    LineCode = model.LineCode,
                    LotNumbers = new List<string>(),
                    OperateComputer = Request.UserHostAddress,
                    Operator = User.Identity.Name,
                    Paramters = dicParams,
                    Remark = model.Description,
                    RouteOperationName = model.RouteOperationName,
                    EquipmentCode=model.EquipmentCode,
                    Color=model.Color,
                    Grade=model.Grade
                };
                p.LotNumbers.Add(lotNumber);
                //进行不良数量记录
                IList<ReasonCodeCategoryDetail> lstDefectReasonCodes = GetDefectReasonCodes(obj.RouteName, obj.RouteStepName);
                p.DefectReasonCodes=new Dictionary<string,IList<DefectReasonCodeParameter>>();
                if (lstDefectReasonCodes != null && lstDefectReasonCodes.Count > 0)
                {

                    foreach (ReasonCodeCategoryDetail item in lstDefectReasonCodes)
                    {
                        string hashcode = string.Format("{0}{1}", item.Key.ReasonCodeCategoryName, item.Key.ReasonCodeName)
                                                .GetHashCode()
                                                .ToString()
                                                .Replace('-', '_');
                        string inputControlName = string.Format("DefectReasonCode_{0}", hashcode);
                        string val = Request.Form[inputControlName];
                        double dVal = 0;
                        if (string.IsNullOrEmpty(val)
                            || double.TryParse(val, out dVal)==false
                            || dVal == 0)
                        {
                            continue;
                        }
                        if (!p.DefectReasonCodes.ContainsKey(obj.Key))
                        {
                            p.DefectReasonCodes.Add(obj.Key, new List<DefectReasonCodeParameter>());
                        }
                        DefectReasonCodeParameter drcp = new DefectReasonCodeParameter()
                        {
                            ReasonCodeCategoryName=item.Key.ReasonCodeCategoryName,
                            ReasonCodeName=item.Key.ReasonCodeName,
                            Quantity=dVal,
                            Description=string.Empty,
                            ResponsiblePerson=string.Empty,
                            RouteOperationName=string.Empty
                        };
                        p.DefectReasonCodes[obj.Key].Add(drcp);
                    }
                }
                //进行报废数量记录
                IList<ReasonCodeCategoryDetail> lstScrapReasonCodes = GetScrapReasonCodes(obj.RouteName, obj.RouteStepName);
                p.ScrapReasonCodes = new Dictionary<string, IList<ScrapReasonCodeParameter>>();
                if (lstScrapReasonCodes != null && lstScrapReasonCodes.Count > 0)
                {
                    foreach (ReasonCodeCategoryDetail item in lstScrapReasonCodes)
                    {
                        string hashcode = string.Format("{0}{1}", item.Key.ReasonCodeCategoryName, item.Key.ReasonCodeName)
                                                .GetHashCode()
                                                .ToString()
                                                .Replace('-', '_');
                        string inputControlName = string.Format("ScrapReasonCode_{0}", hashcode);
                        string val = Request.Form[inputControlName];
                        double dVal = 0;
                        if (string.IsNullOrEmpty(val)
                            || double.TryParse(val, out dVal) == false
                            || dVal == 0)
                        {
                            continue;
                        }
                        if (!p.ScrapReasonCodes.ContainsKey(obj.Key))
                        {
                            p.ScrapReasonCodes.Add(obj.Key, new List<ScrapReasonCodeParameter>());
                        }
                        ScrapReasonCodeParameter srcp = new ScrapReasonCodeParameter()
                        {
                            ReasonCodeCategoryName = item.Key.ReasonCodeCategoryName,
                            ReasonCodeName = item.Key.ReasonCodeName,
                            Quantity = dVal,
                            Description = string.Empty,
                            ResponsiblePerson = string.Empty,
                            RouteOperationName = string.Empty
                        };
                        p.ScrapReasonCodes[obj.Key].Add(srcp);
                    }
                }

                //进行批次出站。
                using (LotTrackOutServiceClient client = new LotTrackOutServiceClient())
                {
                    result = client.TrackOut(p);
                    if (result.Code == 0)
                    {
                        if (!string.IsNullOrEmpty(result.Message))
                        {
                            if (!result.Message.EndsWith("\n"))
                            {
                                result.Message += "\n";
                            }
                            result.Message = result.Message.Replace("\n", "<br/>");
                        }
                        result.Message += string.Format("批次 {0} 出站成功。", lotNumber);
                    }
                }
            }
            else
            {
                 result.Code = 100;
                 result.Message = string.Format("批次 {0} 状态为（{1}），不能进行工作站作业。"
                                                , lotNumber
                                                , obj.StateFlag.GetDisplayName());
            }
            return result;
        }

        private MethodReturnResult GetLot(string lotNumber)
        {
            //如果本次请求有成功获取到批次对象，直接返回。
            if (ViewBag.Lot != null)
            {
                return ViewBag.Lot;
            }

            MethodReturnResult result = new MethodReturnResult();
            MethodReturnResult<Lot> rst = null;
            Lot obj = null;
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                rst = client.Get(lotNumber);
                if (rst.Code <= 0 && rst.Data != null)
                {
                    obj = rst.Data;
                    ViewBag.Lot = rst;
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
            else if (obj.StateFlag == EnumLotState.Finished)
            {
                result.Code = 2002;
                result.Message = string.Format("批次({0})已完成。", lotNumber);
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
            return rst;
        }

        private IList<RouteStepParameter> GetParameterList(string routeName, string routeStepName, EnumLotState stateFlag)
        {
            if (stateFlag != EnumLotState.WaitTrackIn && stateFlag!=EnumLotState.WaitTrackOut)
            {
                return null;
            }
            //如果本次请求有获取过参数清单，则直接返回。
            if (ViewBag.ParameterList != null)
            {
                return ViewBag.ParameterList;
            }
            using (RouteStepParameterServiceClient client = new RouteStepParameterServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    OrderBy = "ParamIndex",
                    Where = string.Format(@"DataFrom='{0}' AND DCType='{1}' AND IsDeleted=0
                                           AND Key.RouteName='{2}'
                                           AND Key.RouteStepName='{3}'"
                                           ,Convert.ToInt32(EnumDataFrom.Manual)
                                           ,stateFlag==EnumLotState.WaitTrackIn
                                                ? Convert.ToInt32(EnumDataCollectionAction.TrackIn)
                                                : Convert.ToInt32(EnumDataCollectionAction.TrackOut)
                                           ,routeName
                                           ,routeStepName)
                };
                MethodReturnResult<IList<RouteStepParameter>> result = client.Get(ref cfg);
                if (result.Code <=0  && result.Data != null)
                {
                    ViewBag.ParameterList = result.Data;
                    return result.Data;
                }
            }
            return null;
        }

        private IList<ReasonCodeCategoryDetail> GetDefectReasonCodes(string routeName, string routeStepName)
        {

            //如果本次请求有获取过参数清单，则直接返回。
            if (ViewBag.DefectReasonCodeList != null)
            {
                return ViewBag.DefectReasonCodeList;
            }

            string categoryName = string.Empty;
            using (RouteStepServiceClient client = new RouteStepServiceClient())
            {
                MethodReturnResult<RouteStep> result = client.Get(new RouteStepKey()
                {
                    RouteName=routeName,
                    RouteStepName=routeStepName
                });
                if (result.Code <= 0 && result.Data != null)
                {
                    categoryName = result.Data.DefectReasonCodeCategoryName;
                }
            }
             //获取原因代码明细。
            if (!string.IsNullOrEmpty(categoryName))
            {
                using (ReasonCodeCategoryDetailServiceClient client = new ReasonCodeCategoryDetailServiceClient())
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format(@"Key.ReasonCodeCategoryName='{0}'"
                                               , categoryName)
                    };
                    MethodReturnResult<IList<ReasonCodeCategoryDetail>> result = client.Get(ref cfg);
                    if (result.Code <= 0 && result.Data != null)
                    {
                        ViewBag.DefectReasonCodeList = result.Data;
                        return result.Data;
                    }
                }
            }
            return null;
        }

        private IList<ReasonCodeCategoryDetail> GetScrapReasonCodes(string routeName, string routeStepName)
        {
            //如果本次请求有获取过参数清单，则直接返回。
            if (ViewBag.ScrapReasonCodeList != null)
            {
                return ViewBag.ScrapReasonCodeList;
            }

            string categoryName = string.Empty;
            using (RouteStepServiceClient client = new RouteStepServiceClient())
            {
                MethodReturnResult<RouteStep> result = client.Get(new RouteStepKey()
                {
                    RouteName = routeName,
                    RouteStepName = routeStepName
                });
                if (result.Code <= 0 && result.Data != null)
                {
                    categoryName = result.Data.ScrapReasonCodeCategoryName;
                }
            }
            //获取原因代码明细。
            if (!string.IsNullOrEmpty(categoryName))
            {
                using (ReasonCodeCategoryDetailServiceClient client = new ReasonCodeCategoryDetailServiceClient())
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format(@"Key.ReasonCodeCategoryName='{0}'"
                                               , categoryName)
                    };
                    MethodReturnResult<IList<ReasonCodeCategoryDetail>> result = client.Get(ref cfg);
                    if (result.Code <= 0 && result.Data != null)
                    {
                        ViewBag.ScrapReasonCodeList = result.Data;
                        return result.Data;
                    }
                }
            }
            return null;
        }

        public ActionResult GetEquipments(string routeOperationName, string productionLineCode)
        {
            IList<Equipment> lstEquipments = new List<Equipment>();
            //根据生产线和工序获取设备。
            using (EquipmentServiceClient client = new EquipmentServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"LineCode='{0}' AND EXISTS(FROM RouteOperationEquipment as p 
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
                          Text = string.Format("{0}[{1}]",item.Key , item.Name)
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
            if(!string.IsNullOrEmpty(stateName))
            {
                using (EquipmentStateServiceClient client = new EquipmentStateServiceClient())
                {
                    MethodReturnResult<EquipmentState> result = client.Get(stateName);
                    if (result.Code <= 0 && result.Data != null)
                    {
                        stateColor = result.Data.StateColor;
                    }
                }
            }
            return Json(new
            {
                StateName = stateName,
                StateColor = stateColor
            }, JsonRequestBehavior.AllowGet);
        }
	}
}