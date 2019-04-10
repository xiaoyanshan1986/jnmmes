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
using System.Data;
using ServiceCenter.Service.Client;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.MES.Service.Client.ZPVM;

namespace ServiceCenter.Client.Mvc.Areas.WIP.Controllers
{
    public class LotPackageCornerController : Controller
    {
        /// <summary> 显示工作站作业界面 </summary>
        /// <returns></returns>
        public ActionResult Index()
        {

            return View(new LotPackageCornerViewModels() { UndoPackageCorner=false});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(LotPackageCornerViewModels model)
        {
            MethodReturnResult resultPackage = new MethodReturnResult();//包装护角信息
            Lot lot = null;                                     //批次对象
            string lotNumber = model.LotNumber.ToUpper();       //取得批次代码（字母调整为大写）
            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                lot = client.Get(lotNumber).Data;
                if (lot == null)
                {
                    resultPackage.Code = 1000;
                    resultPackage.Message = "无符合条件的批次信息";
                    return Json(resultPackage);
                }
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
                if (lot.LocationName != locationName)
                {
                    resultPackage.Code = 3;
                    resultPackage.Message = string.Format("批次（{0}）属于({1})车间，不能在({2})车间线别上操作。"
                                                    , lotNumber
                                                    , lot.LocationName
                                                    , locationName);
                    return Json(resultPackage);
                }
               
            using (WipEngineerServiceClient client = new WipEngineerServiceClient())
            {
                if (model.UndoPackageCorner == true)
                {
                    resultPackage = client.UnDoPackageCorner(lot.Key);
                    if (resultPackage.Code > 0)
                    {
                        resultPackage.Message = "撤销包装护角作业失败，请手动尝试撤销！";
                    }
                    else
                    {
                        resultPackage = client.ExecuteInPackageDetail(lot,model.LineCode);
                    }

                }
                else
                {
                    resultPackage = client.ExecuteInPackageDetail(lot, model.LineCode);
                }
                
               
                return Json(resultPackage);
            }
            
        }
   
	}
}

//2016-06-28
//using ServiceCenter.Client.Mvc.Areas.WIP.Models;
//using ServiceCenter.MES.Model.BaseData;
//using ServiceCenter.MES.Model.FMM;
//using ServiceCenter.MES.Model.LSM;
//using ServiceCenter.MES.Model.PPM;
//using ServiceCenter.MES.Model.WIP;
//using ServiceCenter.MES.Service.Client.BaseData;
//using ServiceCenter.MES.Service.Client.FMM;
//using ServiceCenter.MES.Service.Client.LSM;
//using ServiceCenter.MES.Service.Client.PPM;
//using ServiceCenter.MES.Service.Client.WIP;
//using ServiceCenter.MES.Service.Contract.WIP;
//using WIPResources = ServiceCenter.Client.Mvc.Resources.WIP;
//using ServiceCenter.Model;
//using ServiceCenter.Common;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using System.Data;
//using ServiceCenter.Service.Client;

//namespace ServiceCenter.Client.Mvc.Areas.WIP.Controllers
//{
//    public class LotTrackController : Controller
//    {
//        //
//        // GET: /WIP/LotTrack/
//        /// <summary>
//        /// 显示工作站作业界面。
//        /// </summary>
//        /// <returns></returns>
//        public ActionResult Index()
//        {
//            return View(new LotTrackViewModel());
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Save(LotTrackViewModel model)
//        {
//            MethodReturnResult result = new MethodReturnResult();
//            Response.StatusDescription = "JSON";
//            bool isGetMaterialLot = false;
//            try
//            {
//                string lotNumber = model.LotNumber.ToUpper();
//                result = GetLot(lotNumber);
//                if (result.Code > 0)
//                {
//                    return Json(result);
//                }
                
//                MethodReturnResult<Lot> rst = result as MethodReturnResult<Lot>;
//                Lot obj = rst.Data;
//                //等待进站批次，设备代码必须选择。
//                if (obj.StateFlag == EnumLotState.WaitTrackIn 
//                    && string.IsNullOrEmpty(model.EquipmentCode))
//                {
//                    result.Code = 1;
//                    result.Message = string.Format("设备代码不能为空。");
//                    return Json(result);
//                }
//                //判断批次工序是否在当前工序。
//                if (obj.RouteStepName != model.RouteOperationName)
//                {
//                    result.Code = 2;
//                    result.Message = string.Format("批次({0})当前所在工序（{1}），不能在（{2}）工序上操作。"
//                                                    ,obj.Key
//                                                    ,obj.RouteStepName
//                                                    ,model.RouteOperationName);
//                    return Json(result);
//                }
//                //判断批次所在车间和当前线边所在车间是否匹配。
//                //获取线别车间。
//                string locationName = string.Empty;
//                using (ProductionLineServiceClient client = new ProductionLineServiceClient())
//                {
//                    MethodReturnResult<ProductionLine> r = client.Get(model.LineCode);
//                    if (r.Code <= 0)
//                    {
//                        locationName = r.Data.LocationName;
//                    }
//                }
//                if (!string.IsNullOrEmpty(locationName))
//                {
//                    using (LocationServiceClient client = new LocationServiceClient())
//                    {
//                        MethodReturnResult<Location> r = client.Get(locationName);
//                        if (r.Code <= 0)
//                        {
//                            locationName = r.Data.ParentLocationName;
//                        }
//                    }
//                }
//                //检查批次车间和线别车间是否匹配。
//                if (obj.LocationName != locationName)
//                {
//                    result.Code = 3;
//                    result.Message = string.Format("批次（{0}）属于({1})车间，不能在({2})车间线别上操作。"
//                                                    , lotNumber
//                                                    , obj.LocationName
//                                                    , locationName);
//                    return Json(result);
//                }
//                bool isShowModal = false;
//                //获取工序参数列表。
//                IList<RouteStepParameter> lstRouteStepParameter = GetParameterList(obj.RouteName, obj.RouteStepName, obj.StateFlag);

//                //出站，判断是否显示不良和报废录入对话框。
//                string strLayerEquipmentNo = "";
//                if (obj.StateFlag==EnumLotState.WaitTrackOut)
//                {
//                    IList<RouteStepAttribute> lstRouteStepAttribute = new List<RouteStepAttribute>();
//                    using (RouteStepAttributeServiceClient client = new RouteStepAttributeServiceClient())
//                    {
//                        PagingConfig cfg = new PagingConfig()
//                        {
//                            IsPaging=false,
//                            Where = string.Format("Key.RouteName='{0}' AND Key.RouteStepName='{1}'"
//                                                  ,obj.RouteName
//                                                  ,obj.RouteStepName)
//                        };
//                        MethodReturnResult<IList<RouteStepAttribute>> r = client.Get(ref cfg);
//                        if (r.Code <= 0 && r.Data != null)
//                        {
//                            lstRouteStepAttribute = r.Data;
//                        }
//                    }

//                    #region 是否自动获取物料批号

//                    var lnqGetMaterialLot = from item in lstRouteStepAttribute
//                              where item.Key.AttributeName == "isGetMaterialLot"
//                              select item;
//                    RouteStepAttribute getMaterialLot = lnqGetMaterialLot.FirstOrDefault();
//                    if (getMaterialLot != null)
//                    {
//                        bool.TryParse(getMaterialLot.Value, out isGetMaterialLot);
//                    }
//                    //需要显示工序参数录入表单。
//                    if (lstRouteStepParameter != null && lstRouteStepParameter.Count > 0 && isGetMaterialLot==false)
//                    {
//                        isShowModal = true;
//                    }
//                    #endregion


//                    #region //是否获取层压机信息的站别
//                    bool isGetLayerEquipment = false;
//                    var lnq = from item in lstRouteStepAttribute
//                              where item.Key.AttributeName == "isGetLayerEquipment"
//                              select item;
//                    RouteStepAttribute rsaTmp = lnq.FirstOrDefault();
//                    if (rsaTmp != null)
//                    {
//                        bool.TryParse(rsaTmp.Value, out isGetLayerEquipment);
//                    }

//                    if(isGetLayerEquipment)
//                    {
//                        using(JC.JNMES.ProdLine.TradLevel tClient = new JC.JNMES.ProdLine.TradLevel())
//                        {
//                            strLayerEquipmentNo = tClient.SearhModulePosition(lotNumber);
//                            if(string.IsNullOrEmpty(strLayerEquipmentNo)==false)
//                            {
//                                string[] arrEqpInfo = strLayerEquipmentNo.Split('*');
//                                if(arrEqpInfo.Length>1)
//                                {
//                                    strLayerEquipmentNo = arrEqpInfo[1];
//                                }
//                            }
//                        }
//                    }
//                    ViewBag.IsGetLayerEquipment = true;
//                    model.LotLayerEquipmentNo = strLayerEquipmentNo;
//                    #endregion

//                    #region //是否需要验证【IsCheckELImage，IsCheckIVImage】
//                    bool isCheckELImage = false;
//                    lnq = from item in lstRouteStepAttribute
//                              where item.Key.AttributeName == "IsCheckELImage"
//                              select item;
//                    rsaTmp = lnq.FirstOrDefault();
//                    if (rsaTmp != null)
//                    {
//                        bool.TryParse(rsaTmp.Value, out isCheckELImage);
//                    }

//                    //是否检查IV图片。
//                    bool isCheckIVImage = false;
//                    lnq = from item in lstRouteStepAttribute
//                          where item.Key.AttributeName == "IsCheckIVImage"
//                          select item;
//                    rsaTmp = lnq.FirstOrDefault();
//                    if (rsaTmp != null)
//                    {
//                        bool.TryParse(rsaTmp.Value, out isCheckIVImage);
//                    }
//                    if(isCheckELImage || isCheckIVImage)
//                    {
//                        IList<LotAttribute> lstLotAttributes = new List<LotAttribute>();
//                        using (LotAttributeServiceClient client = new LotAttributeServiceClient())
//                        {
//                            PagingConfig cfg = new PagingConfig()
//                            {
//                                IsPaging = false,
//                                Where = string.Format("Key.LotNumber='{0}'"
//                                                      ,lotNumber)
//                            };
//                            MethodReturnResult<IList<LotAttribute>> r = client.Get(ref cfg);
//                            if (r.Code <= 0 && r.Data != null)
//                            {
//                                lstLotAttributes = r.Data;
//                            }
//                        }

//                        //是否输入等级。
//                        bool isExistedELImage = false;
//                        LotAttribute rstAttr = new LotAttribute();
//                        var lnqOfLotAtts = from item in lstLotAttributes
//                                where item.Key.AttributeName == "ELImagePath"
//                                select item;
//                        rstAttr = lnqOfLotAtts.FirstOrDefault();
//                        if (rstAttr != null)
//                        {
//                            isExistedELImage = true;
//                        }
//                        if(isCheckELImage && isExistedELImage==false)
//                        {
//                            result.Code = 10;
//                            result.Message = string.Format("批次（{0}）的EL3图片不存在。"
//                                                            , lotNumber);
//                            return Json(result);
//                        }

//                        //是否输入等级。
//                        bool isExistedIVImage = false;
//                        lnqOfLotAtts = from item in lstLotAttributes
//                                            where item.Key.AttributeName == "IVImagePath"
//                                            select item;
//                        rstAttr = lnqOfLotAtts.FirstOrDefault();
//                        if (rstAttr != null)
//                        {
//                            isExistedIVImage = true;
//                        }

//                        if (isCheckIVImage && isExistedIVImage == false)
//                        {
//                            result.Code = 11;
//                            result.Message = string.Format("批次（{0}）的IV图片不存在。"
//                                                            , lotNumber);
//                            return Json(result);
//                        }
//                    }
//                    #endregion

//                    //是否输入等级。
//                    bool isInputGrade = false;
//                    lnq = from item in lstRouteStepAttribute
//                              where item.Key.AttributeName == "IsInputGrade"
//                              select item;
//                    rsaTmp = lnq.FirstOrDefault();
//                    if (rsaTmp!=null)
//                    {
//                        bool.TryParse(rsaTmp.Value, out isInputGrade);
//                    }


//                    //是否校验条码
//                    bool isCheckBarCode = false;
//                    lnq = from item in lstRouteStepAttribute
//                          where item.Key.AttributeName == "IsCheckBarCode"
//                              select item;
//                    rsaTmp = lnq.FirstOrDefault();
//                    if (rsaTmp != null)
//                    {
//                        bool.TryParse(rsaTmp.Value, out isCheckBarCode);
//                    }
//                    if (isCheckBarCode)
//                    {
//                        ViewBag.IsCheckBarCode = isCheckBarCode;
//                        isShowModal = true;
//                    }

//                    //是否输入花色。
//                    bool isInputColor = false;
//                    lnq = from item in lstRouteStepAttribute
//                          where item.Key.AttributeName == "IsInputColor"
//                          select item;
//                    rsaTmp = lnq.FirstOrDefault();
//                    if (rsaTmp != null)
//                    {
//                        bool.TryParse(rsaTmp.Value, out isInputColor);
//                    }
//                    //是否输入批次路径。
//                    bool isShowLotPath = false;
//                    lnq = from item in lstRouteStepAttribute
//                          where item.Key.AttributeName == "IsShowLotPath"
//                          select item;
//                    rsaTmp = lnq.FirstOrDefault();
//                    if (rsaTmp != null)
//                    {
//                        bool.TryParse(rsaTmp.Value, out isShowLotPath);
//                    }
//                    //是否显示EL图片
//                    bool isShowELImage = false;
//                    lnq = from item in lstRouteStepAttribute
//                          where item.Key.AttributeName == "IsShowELImage"
//                          select item;
//                    rsaTmp = lnq.FirstOrDefault();
//                    if (rsaTmp != null)
//                    {
//                        bool.TryParse(rsaTmp.Value, out isShowELImage);
//                    }
//                    //是否显示IV图片
//                    bool isShowIVImage = false;
//                    lnq = from item in lstRouteStepAttribute
//                          where item.Key.AttributeName == "IsShowIVImage"
//                          select item;
//                    rsaTmp = lnq.FirstOrDefault();
//                    if (rsaTmp != null)
//                    {
//                        bool.TryParse(rsaTmp.Value, out isShowIVImage);
//                    }
//                    //获取是否显示不良原因录入对话框。
//                    bool isShowDefectModal = false;
//                    lnq = from item in lstRouteStepAttribute
//                          where item.Key.AttributeName == "IsShowDefectModal"
//                          select item;
//                    rsaTmp = lnq.FirstOrDefault();
//                    if (rsaTmp != null)
//                    {
//                        bool.TryParse(rsaTmp.Value, out isShowDefectModal);
//                    }
//                    //获取是否显示返修数据。
//                    bool isShowDefect = false;
//                    lnq = from item in lstRouteStepAttribute
//                          where item.Key.AttributeName == "IsShowDefect"
//                          select item;
//                    rsaTmp = lnq.FirstOrDefault();
//                    if (rsaTmp != null)
//                    {
//                        bool.TryParse(rsaTmp.Value, out isShowDefect);
//                    }
//                    //获取是否显示报废原因录入对话框。
//                    bool isShowScrapModal = false;
//                    lnq = from item in lstRouteStepAttribute
//                          where item.Key.AttributeName == "IsShowScrapModal"
//                          select item;
//                    rsaTmp = lnq.FirstOrDefault();
//                    if (rsaTmp != null)
//                    {
//                        bool.TryParse(rsaTmp.Value, out isShowScrapModal);
//                    }
//                    //是否输入等级
//                    if (isInputGrade)
//                    {
//                        ViewBag.IsInputGrade = isInputGrade;
//                        isShowModal = true;
//                    }
//                    //是否输入花色
//                    if (isInputColor)
//                    {
//                        ViewBag.IsInputColor = isInputColor;
//                        isShowModal = true;
//                    }
//                    //是否输入批次路径
//                    if (isShowLotPath)
//                    {
//                        ViewBag.IsShowLotPath = isShowLotPath;
//                        isShowModal = true;
//                    }
//                    //是否显示EL图片
//                    if (isShowELImage)
//                    {
//                        using (LotQueryServiceClient client = new LotQueryServiceClient())
//                        {
//                            MethodReturnResult<LotAttribute> r = client.GetAttribute(new LotAttributeKey()
//                            {
//                                LotNumber=obj.Key,
//                                AttributeName="ELImagePath"
//                            });
//                            if (r.Code <= 0 && r.Data != null)
//                            {
//                                ViewBag.ELImagePath = r.Data.AttributeValue;
//                            }
//                        }
//                        ViewBag.IsShowELImage = isShowELImage;
//                        isShowModal = true;
//                    }
//                    //是否显示IV图片
//                    if (isShowIVImage)
//                    {
//                        using (LotQueryServiceClient client = new LotQueryServiceClient())
//                        {
//                            MethodReturnResult<LotAttribute> r = client.GetAttribute(new LotAttributeKey()
//                            {
//                                LotNumber = obj.Key,
//                                AttributeName = "IVImagePath"
//                            });
//                            if (r.Code <= 0 && r.Data != null)
//                            {
//                                ViewBag.IVImagePath = r.Data.AttributeValue;
//                            }
//                        }
//                        ViewBag.IsShowIVImage = isShowIVImage;
//                        isShowModal = true;
//                    }
//                    //是否显示不良原因录入对话框。
//                    if (isShowDefectModal)
//                    {
//                        IList<ReasonCodeCategoryDetail> lstDefectReasonCodes = GetDefectReasonCodes(obj.RouteName, obj.RouteStepName);
//                        if (lstDefectReasonCodes!=null && lstDefectReasonCodes.Count > 0)
//                        {
//                            isShowModal = true;
//                        }
//                    }
//                    //是否显示返修数据。
//                    if (isShowDefect)
//                    {
//                        ViewBag.IsShowDefect = isShowDefect;
//                        isShowModal = true;
//                    }
//                    //是否显示报废原因录入对话框。
//                    if (isShowScrapModal)
//                    {
//                        IList<ReasonCodeCategoryDetail> lstScrapReasonCodes = GetScrapReasonCodes(obj.RouteName, obj.RouteStepName);
//                        if (lstScrapReasonCodes!=null && lstScrapReasonCodes.Count > 0)
//                        {
//                            isShowModal = true;
//                        }
//                    }
//                }
//                //显示附加对话框。
//                if(isShowModal)
//                {
//                    Response.StatusDescription = "Partial";
//                    ViewBag.Lot = obj;
//                    LotTrackViewModel m = new LotTrackViewModel
//                    {
//                        LotLayerEquipmentNo = strLayerEquipmentNo
//                    };
//                    return PartialView("_ModalContentPartial",m);
//                }
//                result = Track(obj, model,isGetMaterialLot);
//            }
//            catch (Exception ex)
//            {
//                result.Code = 1000;
//                result.Message = ex.Message;
//                result.Detail = ex.ToString();
//            }
//            // 如果我们进行到这一步时某个地方出错，则重新显示表单
//            return Json(result);
//        }

//        //
//        // POST: /WIP/LotTrack/SaveModal
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult SaveModal(LotTrackViewModel model)
//        {
//            MethodReturnResult result = new MethodReturnResult();
//            bool isGetMaterialLot = false;
//            try
//            {
//                string lotNumber = model.LotNumber.ToUpper();
//                result = GetLot(lotNumber);
//                if (result.Code > 0)
//                {
//                    return Json(result);
//                }
//                MethodReturnResult<Lot> rst = result as MethodReturnResult<Lot>;


//                Lot obj = rst.Data;
//                IList<RouteStepAttribute> lstRouteStepAttribute = new List<RouteStepAttribute>();
//                using (RouteStepAttributeServiceClient client = new RouteStepAttributeServiceClient())
//                {
//                    PagingConfig cfg = new PagingConfig()
//                    {
//                        IsPaging = false,
//                        Where = string.Format("Key.RouteName='{0}' AND Key.RouteStepName='{1}'"
//                                              , obj.RouteName
//                                              , obj.RouteStepName)
//                    };
//                    MethodReturnResult<IList<RouteStepAttribute>> r = client.Get(ref cfg);
//                    if (r.Code <= 0 && r.Data != null)
//                    {
//                        lstRouteStepAttribute = r.Data;
//                    }
//                }

//                #region 是否自动获取物料批号
//                var lnqGetMaterialLot = from item in lstRouteStepAttribute
//                                        where item.Key.AttributeName == "isGetMaterialLot"
//                                        select item;
//                RouteStepAttribute getMaterialLot = lnqGetMaterialLot.FirstOrDefault();
//                if (getMaterialLot != null)
//                {
//                    bool.TryParse(getMaterialLot.Value, out isGetMaterialLot);
//                }
//                #endregion


//                result = Track(rst.Data, model, isGetMaterialLot);

//            }
//            catch (Exception ex)
//            {
//                result.Code = 1000;
//                result.Message = ex.Message;
//                result.Detail = ex.ToString();
//            }
//            // 如果我们进行到这一步时某个地方出错，则重新显示表单
//            return Json(result);
//        }

//        /// <summary>
//        /// 批次过站作业。
//        /// </summary>
//        /// <param name="obj">批次对象。</param>
//        /// <param name="model">过站模型对象。</param>
//        /// <returns>返回结果。</returns>
//        private MethodReturnResult Track(Lot obj, LotTrackViewModel model, bool isGetMaterialLot)
//        {
//            //批次代码
//            string lotNumber = model.LotNumber.ToUpper();

//            MethodReturnResult result = new MethodReturnResult();
//            MethodReturnResult<DataTable> dtResult = new MethodReturnResult<DataTable>();
//            IDictionary<string, IList<TransactionParameter>> dicParams = new Dictionary<string, IList<TransactionParameter>>();

//            //获取工序参数列表。
//            IList<RouteStepParameter> lstRouteStepParameter = GetParameterList(obj.RouteName, obj.RouteStepName, obj.StateFlag);

//            //组织批次附加参数。
//            if(lstRouteStepParameter!=null)
//            {

//                if (isGetMaterialLot)
//                {
//                    #region 自动过站扣料
//                    foreach (RouteStepParameter item in lstRouteStepParameter)
//                    {
//                        if (!dicParams.ContainsKey(obj.Key))
//                        {
//                            dicParams.Add(obj.Key, new List<TransactionParameter>());
//                        }
//                        string val = null;
//                        if (item.Key.ParameterName == "电池片批号" || item.Key.ParameterName == "电池片小包装号")
//                        {
//                            dtResult = GetCellLotList(item.MaterialType, obj.LineCode, obj.RouteStepName, obj.OrderNumber, obj.EquipmentCode);
//                            if (dtResult.Code<=0&&dtResult.Data.Rows.Count>0)
//                            {
//                                val = dtResult.Data.Rows[0][0].ToString();
//                            }
//                            else if (dtResult.Code<=0&&dtResult.Data.Rows.Count<=0)
//                            {
//                                result.Code = 4001;
//                                result.Message = string.Format("未找到工单{0}工序{1}线别{2}设备{3}物料类型{4}的物料批次，请上料"
//                                                                    ,obj.OrderNumber
//                                                                    ,obj.RouteStepName
//                                                                    ,obj.LineCode
//                                                                    ,obj.EquipmentCode
//                                                                    ,item.MaterialType);
//                                return result;
//                            }
//                            else
//                            {
//                                result.Code = dtResult.Code;
//                                result.Detail = dtResult.Detail;
//                                result.Message = dtResult.Message;

//                                return result;
//                            }
                        
//                        }
//                        else
//                        {
//                            dtResult = GetParameterLotList(item.MaterialType, obj.LineCode, obj.RouteStepName, obj.OrderNumber, obj.EquipmentCode);
//                            if (dtResult.Code <= 0 && dtResult.Data.Rows.Count > 0)
//                            {
//                                val = dtResult.Data.Rows[0][0].ToString();
//                            }
//                            else if (dtResult.Code <= 0 && dtResult.Data.Rows.Count <= 0)
//                            {
//                                result.Code = 4001;
//                                result.Message = string.Format("未找到工单{0}工序{1}线别{2}设备{3}物料类型{4}的物料批次，请上料"
//                                                                    , obj.OrderNumber
//                                                                    , obj.RouteStepName
//                                                                    , obj.LineCode
//                                                                    , obj.EquipmentCode
//                                                                    , item.MaterialType);
//                                return result;
//                            }
//                            else
//                            {
//                                result.Code = dtResult.Code;
//                                result.Detail = dtResult.Detail;
//                                result.Message = dtResult.Message;

//                                return result;
//                            }
                        
//                        }
//                        TransactionParameter tp = new TransactionParameter()
//                        {
//                            Index = item.ParamIndex,
//                            Name = item.Key.ParameterName,
//                            Value = val
//                        };
//                        dicParams[obj.Key].Add(tp);
//                    }
//                    #endregion
//                }
//                else
//                {
//                    #region 手动进站获取物料批次号
//                    foreach (RouteStepParameter item in lstRouteStepParameter)
//                    {
//                        string hashcode = string.Format("{0}{1}{2}", item.Key.RouteName, item.Key.RouteStepName, item.Key.ParameterName)
//                                          .GetHashCode()
//                                          .ToString()
//                                          .Replace('-', '_');
//                        string paramName = string.Format("PARAM_{0}", hashcode);
//                        string val = Request.Form[paramName];
//                        //记录上一次值。
//                        if (item.IsUsePreValue)
//                        {
//                            if (Request.Cookies.Get(paramName) != null)
//                            {
//                                Response.SetCookie(new HttpCookie(paramName, val));
//                            }
//                            else if (!string.IsNullOrEmpty(val))
//                            {
//                                Response.Cookies.Add(new HttpCookie(paramName, val));
//                            }
//                        }
//                        if (string.IsNullOrEmpty(val))
//                        {
//                            continue;
//                        }
//                        if (!dicParams.ContainsKey(obj.Key))
//                        {
//                            dicParams.Add(obj.Key, new List<TransactionParameter>());
//                        }
//                        if (item.DataType == EnumDataType.Boolean)
//                        {
//                            val = val == "on" ? "true" : "false";
//                        }

//                        TransactionParameter tp = new TransactionParameter()
//                        {
//                            Index = item.ParamIndex,
//                            Name = item.Key.ParameterName,
//                            Value = val
//                        };
//                        dicParams[obj.Key].Add(tp);
//                    }
//                    #endregion
//                }

//            }

//            //批次当前状态为等待进站。
//            if (obj.StateFlag == EnumLotState.WaitTrackIn)
//            {
//                TrackInParameter p = new TrackInParameter()
//                {
//                    Creator = User.Identity.Name,
//                    EquipmentCode = model.EquipmentCode,
//                    LineCode = model.LineCode,
//                    LotNumbers = new List<string>(),
//                    OperateComputer = Request.UserHostAddress,
//                    Operator = User.Identity.Name,
//                    Paramters = dicParams,
//                    Remark = model.Description,
//                    RouteOperationName = model.RouteOperationName
//                };

//                p.LotNumbers.Add(lotNumber);

//                //进行批次进站。
//                //using (LotTrackInServiceClient client = new LotTrackInServiceClient())
//                using (WipEngineerServiceClient client = new WipEngineerServiceClient())
//                {
//                    result = client.TrackInLot(p);
//                    if (result.Code == 0)
//                    {
//                        if (!string.IsNullOrEmpty(result.Message))
//                        {
//                            if (!result.Message.EndsWith("\n"))
//                            {
//                                result.Message += "\n";
//                            }
//                            result.Message = result.Message.Replace("\n", "<br/>");
//                        }
//                        result.Message = string.Format("批次 {0} 进站成功。", lotNumber);
//                    }
//                }
//            }

//            //批次当前状态为等待出站。
//            else if(obj.StateFlag==EnumLotState.WaitTrackOut)
//            {

//                TrackOutParameter p = new TrackOutParameter()
//                {
//                    Creator = User.Identity.Name,
//                    LineCode = model.LineCode,
//                    LotNumbers = new List<string>(),
//                    OperateComputer = Request.UserHostAddress,
//                    Operator = User.Identity.Name,
//                    Paramters = dicParams,
//                    Remark = model.Description,
//                    RouteOperationName = model.RouteOperationName,
//                    EquipmentCode=model.EquipmentCode,
//                    Color=model.Color,
//                    Grade=model.Grade
//                };
//                p.LotNumbers.Add(lotNumber);
//                //增加检验数据。
//                if(string.IsNullOrEmpty(model.Color)==false
//                   || string.IsNullOrEmpty(model.Grade) == false)
//                {
//                    if (!string.IsNullOrEmpty(model.Barcode1)
//                        && model.Barcode1!=obj.Key)
//                    {
//                        result.Code = 3001;
//                        result.Message = string.Format("检验条码1 （{0}）同组件序列号（{1}）不一致,请确认。", model.Barcode1, obj.Key);
//                        return result;
//                    }

//                    if (!string.IsNullOrEmpty(model.Barcode2)
//                        && model.Barcode2 != obj.Key)
//                    {
//                        result.Code = 3002;
//                        result.Message = string.Format("检验条码2 （{0}）同组件序列号（{1}）不一致,请确认。", model.Barcode1, obj.Key);
//                        return result;
//                    }

//                    p.CheckBarcodes = new Dictionary<string, IList<string>>();
//                    if (!p.CheckBarcodes.ContainsKey(obj.Key))
//                    {
//                        p.CheckBarcodes.Add(obj.Key, new List<string>());
//                    }
//                    p.CheckBarcodes[obj.Key].Add(model.Barcode1);
//                    p.CheckBarcodes[obj.Key].Add(model.Barcode2);
//                }
//                //进行不良数量记录
//                IList<ReasonCodeCategoryDetail> lstDefectReasonCodes = GetDefectReasonCodes(obj.RouteName, obj.RouteStepName);
//                p.DefectReasonCodes=new Dictionary<string,IList<DefectReasonCodeParameter>>();
//                if (lstDefectReasonCodes != null && lstDefectReasonCodes.Count > 0)
//                {

//                    foreach (ReasonCodeCategoryDetail item in lstDefectReasonCodes)
//                    {
//                        string hashcode = string.Format("{0}{1}", item.Key.ReasonCodeCategoryName, item.Key.ReasonCodeName)
//                                                .GetHashCode()
//                                                .ToString()
//                                                .Replace('-', '_');
//                        string inputControlName = string.Format("DefectReasonCode_{0}", hashcode);
//                        string val = Request.Form[inputControlName];
//                        double dVal = 0;
//                        if (string.IsNullOrEmpty(val)
//                            || double.TryParse(val, out dVal)==false
//                            || dVal == 0)
//                        {
//                            continue;
//                        }
//                        if (!p.DefectReasonCodes.ContainsKey(obj.Key))
//                        {
//                            p.DefectReasonCodes.Add(obj.Key, new List<DefectReasonCodeParameter>());
//                        }
//                        DefectReasonCodeParameter drcp = new DefectReasonCodeParameter()
//                        {
//                            ReasonCodeCategoryName=item.Key.ReasonCodeCategoryName,
//                            ReasonCodeName=item.Key.ReasonCodeName,
//                            Quantity=dVal,
//                            Description=string.Empty,
//                            ResponsiblePerson=string.Empty,
//                            RouteOperationName=string.Empty
//                        };
//                        p.DefectReasonCodes[obj.Key].Add(drcp);
//                    }
//                }
//                //进行报废数量记录
//                IList<ReasonCodeCategoryDetail> lstScrapReasonCodes = GetScrapReasonCodes(obj.RouteName, obj.RouteStepName);
//                p.ScrapReasonCodes = new Dictionary<string, IList<ScrapReasonCodeParameter>>();
//                if (lstScrapReasonCodes != null && lstScrapReasonCodes.Count > 0)
//                {
//                    foreach (ReasonCodeCategoryDetail item in lstScrapReasonCodes)
//                    {
//                        string hashcode = string.Format("{0}{1}", item.Key.ReasonCodeCategoryName, item.Key.ReasonCodeName)
//                                                .GetHashCode()
//                                                .ToString()
//                                                .Replace('-', '_');
//                        string inputControlName = string.Format("ScrapReasonCode_{0}", hashcode);
//                        string val = Request.Form[inputControlName];
//                        double dVal = 0;
//                        if (string.IsNullOrEmpty(val)
//                            || double.TryParse(val, out dVal) == false
//                            || dVal == 0)
//                        {
//                            continue;
//                        }
//                        if (!p.ScrapReasonCodes.ContainsKey(obj.Key))
//                        {
//                            p.ScrapReasonCodes.Add(obj.Key, new List<ScrapReasonCodeParameter>());
//                        }
//                        ScrapReasonCodeParameter srcp = new ScrapReasonCodeParameter()
//                        {
//                            ReasonCodeCategoryName = item.Key.ReasonCodeCategoryName,
//                            ReasonCodeName = item.Key.ReasonCodeName,
//                            Quantity = dVal,
//                            Description = string.Empty,
//                            ResponsiblePerson = string.Empty,
//                            RouteOperationName = string.Empty
//                        };
//                        p.ScrapReasonCodes[obj.Key].Add(srcp);
//                    }
//                }

//                #region //LOT Attributes [LotLayerEquipmentNo]
//                p.Attributes = new Dictionary<string, IList<TransactionParameter>>();
//                if (model.LotLayerEquipmentNo != null && model.LotLayerEquipmentNo.Length>0)
//                {
//                    p.Attributes.Add(lotNumber, new List<TransactionParameter>());
//                    TransactionParameter transactionParameter = new TransactionParameter
//                    {
//                        Name = "LayerEquipmentNo",
//                        Value = model.LotLayerEquipmentNo
//                    };
//                    p.Attributes[lotNumber].Add(transactionParameter);
//                }
//                #endregion

//                //进行批次出站。
//                //using (LotTrackOutServiceClient client = new LotTrackOutServiceClient())
//                using (WipEngineerServiceClient client = new WipEngineerServiceClient())
//                {
//                    result = client.TrackOutLot(p);
//                    if (result.Code == 0)
//                    {
//                        if (!string.IsNullOrEmpty(result.Message))
//                        {
//                            if (!result.Message.EndsWith("\n"))
//                            {
//                                result.Message += "\n";
//                            }
//                            result.Message = result.Message.Replace("\n", "<br/>");
//                        }
//                        result.Message += string.Format("批次 {0} 出站成功。", lotNumber);
//                    }
//                }
//            }
//            else
//            {
//                 result.Code = 100;
//                 result.Message = string.Format("批次 {0} 状态为（{1}），不能进行工作站作业。"
//                                                , lotNumber
//                                                , obj.StateFlag.GetDisplayName());
//            }
//            return result;
//        }




//        /// <summary>
//        /// 自动获取上料物料批号
//        /// </summary>
//        /// <param name="materialType"></param>
//        /// <param name="lineCode"></param>
//        /// <param name="routeStepName"></param>
//        /// <param name="orderNumber"></param>
//        /// <param name="equipmentCode"></param>
//        /// <returns></returns>
//        private  MethodReturnResult<DataTable> GetParameterLotList(string materialType, string lineCode, string routeStepName, string orderNumber, string equipmentCode)
//        {
//            MethodReturnResult<DataTable> dtResult = null;
//            string sql2 = string.Format(@"SELECT TOP 1 t1.MATERIAL_LOT
//                                          FROM [dbo].[LSM_MATERIAL_LOADING_DETAIL] t1 
//                                              INNER JOIN dbo.LSM_MATERIAL_LOADING t2  ON t2.LOADING_KEY=t1.LOADING_KEY
//                                              INNER JOIN [dbo].[FMM_MATERIAL] t3 ON t3.MATERIAL_CODE=t1.MATERILA_CODE
//                                          WHERE t3.MATERIAL_TYPE = '{0}' AND t2.LINE_CODE='{1}'
//		                                             AND t2.ROUTE_OPERATION_NAME='{2}' AND t1.ORDER_NUMBER='{3}'
//		                                             AND t2.EQUIPMENT_CODE='{4}' AND t1.CURRENT_QTY>0
//										  ORDER BY t1.EDIT_TIME ASC,t1.ITEM_NO ASC"
//                                         , materialType
//                                         , lineCode
//                                         , routeStepName
//                                         , orderNumber
//                                         , equipmentCode
//                                         );
           
//            using (DBServiceClient client = new DBServiceClient())
//            {
//                dtResult = client.ExecuteQuery(sql2);
//            }
//            return dtResult;
//        }
//        /// <summary>
//        /// 自动获取电池片物料批号
//        /// </summary>
//        /// <param name="materialType"></param>
//        /// <param name="lineCode"></param>
//        /// <param name="routeStepName"></param>
//        /// <param name="orderNumber"></param>
//        /// <param name="equipmentCode"></param>
//        /// <returns></returns>
//        private MethodReturnResult<DataTable> GetCellLotList(string materialType, string lineCode, string routeStepName, string orderNumber, string equipmentCode)
//        {
//            //string materialLot = null;
//            MethodReturnResult<DataTable> dtResult = null;
//            string sql2 = string.Format(@"SELECT TOP 1 t1.MATERIAL_LOT
//                                          FROM [dbo].[LSM_MATERIAL_LOADING_DETAIL] t1 
//                                              INNER JOIN dbo.LSM_MATERIAL_LOADING t2  ON t2.LOADING_KEY=t1.LOADING_KEY
//                                              INNER JOIN [dbo].[FMM_MATERIAL] t3 ON t3.MATERIAL_CODE=t1.MATERILA_CODE
//                                          WHERE t3.MATERIAL_TYPE like '{0}%' AND t2.LINE_CODE='{1}'
//		                                             AND t2.ROUTE_OPERATION_NAME='{2}' AND t1.ORDER_NUMBER='{3}'
//		                                             AND t2.EQUIPMENT_CODE='{4}' AND t1.CURRENT_QTY>0
//                                          ORDER BY t1.EDIT_TIME ASC,t1.ITEM_NO ASC"
//                                         , materialType
//                                         , lineCode
//                                         , routeStepName
//                                         , orderNumber
//                                         , equipmentCode
//                                         );
//            DataTable dt2 = new DataTable();
//            using (DBServiceClient client = new DBServiceClient())
//            {
//                 dtResult = client.ExecuteQuery(sql2);
//                if (dtResult.Code == 0)
//                {
//                    dt2 = dtResult.Data;
//                    //materialLot = dt2.Rows[0][0].ToString();
//                }
//            }
//            //return materialLot;
//            return dtResult;
//        }
    
//        private MethodReturnResult GetLot(string lotNumber)
//        {
//            //如果本次请求有成功获取到批次对象，直接返回。
//            if (ViewBag.Lot != null)
//            {
//                return ViewBag.Lot;
//            }

//            MethodReturnResult result = new MethodReturnResult();
//            MethodReturnResult<Lot> rst = null;
//            Lot obj = null;
//            using (LotQueryServiceClient client = new LotQueryServiceClient())
//            {
//                rst = client.Get(lotNumber);
//                if (rst.Code <= 0 && rst.Data != null)
//                {
//                    obj = rst.Data;
//                    ViewBag.Lot = rst;
//                }
//                else
//                {
//                    result.Code = rst.Code;
//                    result.Message = rst.Message;
//                    result.Detail = rst.Detail;
//                    return result;
//                }
//            }
//            if (obj == null || obj.Status == EnumObjectStatus.Disabled)
//            {
//                result.Code = 2001;
//                result.Message = string.Format(WIPResources.StringResource.LotIsNotExists, lotNumber);
//                return result;
//            }
//            else if (obj.StateFlag == EnumLotState.Finished)
//            {
//                result.Code = 2002;
//                result.Message = string.Format("批次({0})已完成。", lotNumber);
//                return result;
//            }
//            else if (obj.Status == EnumObjectStatus.Disabled || obj.DeletedFlag == true)
//            {
//                result.Code = 2003;
//                result.Message = string.Format("批次({0})已结束。", lotNumber);
//                return result;
//            }
//            else if (obj.HoldFlag == true)
//            {
//                string res = null;
//                string res2 = null;

//                string sql = string.Format(@"select ATTR_4  from WIP_LOT where LOT_NUMBER='{0}'", lotNumber);
//                DataTable dt = new DataTable();
//                using (DBServiceClient client = new DBServiceClient())
//                {
//                    MethodReturnResult<DataTable> dtResult = client.ExecuteQuery(sql);
//                    if (result.Code == 0)
//                    {
//                        dt = dtResult.Data;
//                        res = dt.Rows[0][0].ToString();
//                    }
//                }

//                string sql2 = string.Format(@"select top 1 t2.HOLD_DESCRIPTION  from  WIP_TRANSACTION  t1
//                                                   inner join [dbo].[WIP_TRANSACTION_HOLD_RELEASE]  t2 on  t1.TRANSACTION_KEY=t2.TRANSACTION_KEY
//                                                   inner join WIP_LOT t3  on t3.LOT_NUMBER = t1.LOT_NUMBER  
//                                                   where t1.LOT_NUMBER='{0}'
//                                                   order by t2.HOLD_TIME  desc", lotNumber);
//                DataTable dt2 = new DataTable();
//                using (DBServiceClient client2 = new DBServiceClient())
//                {
//                    MethodReturnResult<DataTable> dtResult2 = client2.ExecuteQuery(sql2);
//                    if (result.Code == 0 && dtResult2.Data != null && dtResult2.Data.Rows.Count>0)
//                    {
//                        dt2 = dtResult2.Data;
//                        res2 = dt2.Rows[0][0].ToString();
//                    }
//                }
                
//                if (dt != null && dt.Rows.Count > 0 && res!=null && res != "")
//                {
//                    result.Code = 2004;
//                    result.Message = string.Format("批次（{0}）已暂停,原因为：{1}。", lotNumber, res);
//                }else if (dt != null && dt.Rows.Count > 0 && res2!=null && res2!="")
//                {
//                    result.Code = 2004;
//                    result.Message = string.Format("批次（{0}）已暂停。", lotNumber);
//                }
//                else
//                {
//                    result.Code = 2004;
//                    result.Message = string.Format("批次（{0}）已暂停。", lotNumber);
//                }
//                return result;
//            }
//            return rst;
//        }
//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="routeName"></param>
//        /// <param name="routeStepName"></param>
//        /// <param name="stateFlag"></param>
//        /// <returns></returns>
//        private IList<RouteStepParameter> GetParameterList(string routeName, string routeStepName, EnumLotState stateFlag)
//        {
//            if (stateFlag != EnumLotState.WaitTrackIn && stateFlag!=EnumLotState.WaitTrackOut)
//            {
//                return null;
//            }
//            //如果本次请求有获取过参数清单，则直接返回。
//            if (ViewBag.ParameterList != null)
//            {
//                return ViewBag.ParameterList;
//            }
//            using (RouteStepParameterServiceClient client = new RouteStepParameterServiceClient())
//            {
//                PagingConfig cfg = new PagingConfig()
//                {
//                    IsPaging = false,
//                    OrderBy = "ParamIndex",
//                    Where = string.Format(@"DataFrom='{0}' AND DCType='{1}' AND IsDeleted=0
//                                           AND Key.RouteName='{2}'
//                                           AND Key.RouteStepName='{3}'"
//                                           ,Convert.ToInt32(EnumDataFrom.Manual)
//                                           ,stateFlag==EnumLotState.WaitTrackIn
//                                                ? Convert.ToInt32(EnumDataCollectionAction.TrackIn)
//                                                : Convert.ToInt32(EnumDataCollectionAction.TrackOut)
//                                           ,routeName
//                                           ,routeStepName)
//                };
//                MethodReturnResult<IList<RouteStepParameter>> result = client.Get(ref cfg);
//                if (result.Code <=0  && result.Data != null)
//                {
//                    ViewBag.ParameterList = result.Data;
//                    return result.Data;
//                }
//            }
//            return null;
//        }
//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="routeName"></param>
//        /// <param name="routeStepName"></param>
//        /// <returns></returns>
//        private IList<ReasonCodeCategoryDetail> GetDefectReasonCodes(string routeName, string routeStepName)
//        {

//            //如果本次请求有获取过参数清单，则直接返回。
//            if (ViewBag.DefectReasonCodeList != null)
//            {
//                return ViewBag.DefectReasonCodeList;
//            }

//            string categoryName = string.Empty;
//            using (RouteStepServiceClient client = new RouteStepServiceClient())
//            {
//                MethodReturnResult<RouteStep> result = client.Get(new RouteStepKey()
//                {
//                    RouteName=routeName,
//                    RouteStepName=routeStepName
//                });
//                if (result.Code <= 0 && result.Data != null)
//                {
//                    categoryName = result.Data.DefectReasonCodeCategoryName;
//                }
//            }
//             //获取原因代码明细。
//            if (!string.IsNullOrEmpty(categoryName))
//            {
//                using (ReasonCodeCategoryDetailServiceClient client = new ReasonCodeCategoryDetailServiceClient())
//                {
//                    PagingConfig cfg = new PagingConfig()
//                    {
//                        IsPaging = false,
//                        Where = string.Format(@"Key.ReasonCodeCategoryName='{0}'"
//                                               , categoryName)
//                    };
//                    MethodReturnResult<IList<ReasonCodeCategoryDetail>> result = client.Get(ref cfg);
//                    if (result.Code <= 0 && result.Data != null)
//                    {
//                        ViewBag.DefectReasonCodeList = result.Data;
//                        return result.Data;
//                    }
//                }
//            }
//            return null;
//        }
//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="routeName"></param>
//        /// <param name="routeStepName"></param>
//        /// <returns></returns>
//        private IList<ReasonCodeCategoryDetail> GetScrapReasonCodes(string routeName, string routeStepName)
//        {
//            //如果本次请求有获取过参数清单，则直接返回。
//            if (ViewBag.ScrapReasonCodeList != null)
//            {
//                return ViewBag.ScrapReasonCodeList;
//            }

//            string categoryName = string.Empty;
//            using (RouteStepServiceClient client = new RouteStepServiceClient())
//            {
//                MethodReturnResult<RouteStep> result = client.Get(new RouteStepKey()
//                {
//                    RouteName = routeName,
//                    RouteStepName = routeStepName
//                });
//                if (result.Code <= 0 && result.Data != null)
//                {
//                    categoryName = result.Data.ScrapReasonCodeCategoryName;
//                }
//            }
//            //获取原因代码明细。
//            if (!string.IsNullOrEmpty(categoryName))
//            {
//                using (ReasonCodeCategoryDetailServiceClient client = new ReasonCodeCategoryDetailServiceClient())
//                {
//                    PagingConfig cfg = new PagingConfig()
//                    {
//                        IsPaging = false,
//                        Where = string.Format(@"Key.ReasonCodeCategoryName='{0}'"
//                                               , categoryName)
//                    };
//                    MethodReturnResult<IList<ReasonCodeCategoryDetail>> result = client.Get(ref cfg);
//                    if (result.Code <= 0 && result.Data != null)
//                    {
//                        ViewBag.ScrapReasonCodeList = result.Data;
//                        return result.Data;
//                    }
//                }
//            }
//            return null;
//        }

//        public ActionResult GetEquipments(string routeOperationName, string productionLineCode)
//        {
//            IList<Equipment> lstEquipments = new List<Equipment>();
//            //根据生产线和工序获取设备。
//            using (EquipmentServiceClient client = new EquipmentServiceClient())
//            {
//                PagingConfig cfg = new PagingConfig()
//                {
//                    IsPaging = false,
//                    Where = string.Format(@"LineCode='{0}' AND EXISTS(FROM RouteOperationEquipment as p 
//                                                                      WHERE p.Key.EquipmentCode=self.Key 
//                                                                      AND p.Key.RouteOperationName='{1}')"
//                                            , productionLineCode
//                                            , routeOperationName)
//                };
//                MethodReturnResult<IList<Equipment>> result = client.Get(ref cfg);
//                if (result.Code <= 0 && result.Data != null)
//                {
//                    lstEquipments = result.Data;
//                }
//            }

//            var lnq = from item in lstEquipments
//                      select new
//                      {
//                          Key = item.Key,
//                          Text = string.Format("{0}[{1}]",item.Key , item.Name)
//                      };
//            return Json(lnq, JsonRequestBehavior.AllowGet);
//        }

//        public ActionResult GetEquipmentState(string equipmentCode)
//        {
//            string stateName = string.Empty;
//            //根据生产线和工序获取设备。
//            using (EquipmentServiceClient client = new EquipmentServiceClient())
//            {
//                MethodReturnResult<Equipment> result = client.Get(equipmentCode);
//                if (result.Code <= 0 && result.Data != null)
//                {
//                    stateName = result.Data.StateName;
//                }
//            }
//            string stateColor = string.Empty;
//            if(!string.IsNullOrEmpty(stateName))
//            {
//                using (EquipmentStateServiceClient client = new EquipmentStateServiceClient())
//                {
//                    MethodReturnResult<EquipmentState> result = client.Get(stateName);
//                    if (result.Code <= 0 && result.Data != null)
//                    {
//                        stateColor = result.Data.StateColor;
//                    }
//                }
//            }
//            return Json(new
//            {
//                StateName = stateName,
//                StateColor = stateColor
//            }, JsonRequestBehavior.AllowGet);
//        }
//    }
//}