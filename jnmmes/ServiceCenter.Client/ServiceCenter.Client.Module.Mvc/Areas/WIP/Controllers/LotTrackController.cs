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
using System.Speech.Synthesis;
using System.Threading;
using System.IO;
using ServiceCenter.Client.Mvc.PackageBinTouchForA;
using Newtonsoft.Json;
using System.Threading.Tasks;
using ServiceCenter.Common.Print;
using System.Dynamic;
using ServiceCenter.Client.Mvc.Areas.ZPVM.Models;


namespace ServiceCenter.Client.Mvc.Areas.WIP.Controllers
{
    public class LotTrackController : Controller
    {
        //获取厂别标志位
        string localName = System.Configuration.ConfigurationSettings.AppSettings["LocalName"];

        //获取SE背板打印模板
        string SEPrintLab = System.Configuration.ConfigurationSettings.AppSettings["SEPrintLab"];
        //获取SE背板打印模板参数
        string SEPrintParam = System.Configuration.ConfigurationSettings.AppSettings["SEPrintParam"];
        //获取英利或SE的产品编码
        string productCodes = System.Configuration.ConfigurationSettings.AppSettings["NeedAttr3ProductCodes"];  


        //获取IV有效数据
        public ActionResult GetIVTestData(string lotNumber)
        {
            using (IVTestDataServiceClient client = new IVTestDataServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key.LotNumber='{0}' AND IsDefault=1", lotNumber)
                };
                MethodReturnResult<IList<IVTestData>> result = client.Get(ref cfg);

                if (result.Code == 0 && result.Data != null & result.Data.Count > 0)
                {
                    return Json(result.Data[0],JsonRequestBehavior.AllowGet);
                }
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        /// <summary> 显示工作站作业界面 </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View(new LotTrackViewModel() { IsShowDialog = false });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(LotTrackViewModel model)
        {
            
            MethodReturnResult result = new MethodReturnResult();
            Response.StatusDescription = "JSON";
            bool isAutoDeductMaterial = false;
            IList<RouteStepAttribute> lstRouteStepAttribute = new List<RouteStepAttribute>();   //工序属性
            string strLayerEquipmentNo = "";                        //层压机号
            bool IsPowersetDetail = false;//电视电流分档
            try
            {
                #region 取得批次信息
                string lotNumber = model.LotNumber.Trim().ToUpper();
                result = GetLot(lotNumber);
                if (result.Code > 0)
                {
                    return Json(result);
                }
                MethodReturnResult<Lot> rst = result as MethodReturnResult<Lot>;
                Lot obj = rst.Data;
                #endregion

                #region 批次合规性校验
                
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
                                                    , obj.Key
                                                    , obj.RouteStepName
                                                    , model.RouteOperationName);
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
                #endregion

                #region 是否自动扣料/是否显示参数录入表单
                
                bool isShowModal = false;
                
                //获取工序物料列表
                IList<RouteStepParameter> lstRouteStepParameter = GetParameterList(obj.RouteName, obj.RouteStepName, obj.StateFlag);

                //获取工序控制属性列表                
                using (RouteStepAttributeServiceClient client = new RouteStepAttributeServiceClient())
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format("Key.RouteName='{0}' AND Key.RouteStepName='{1}'"
                                                , obj.RouteName
                                                , obj.RouteStepName)
                    };
                    MethodReturnResult<IList<RouteStepAttribute>> r = client.Get(ref cfg);
                    if (r.Code <= 0 && r.Data != null)
                    {
                        lstRouteStepAttribute = r.Data;
                    }
                }

                //根据工序判断是否自动扣料
                var lnqAutoDeductMaterial = from item in lstRouteStepAttribute
                                            where item.Key.AttributeName == "isAutoDeductMaterial"
                                            select item;
                RouteStepAttribute autoDeductMaterial = lnqAutoDeductMaterial.FirstOrDefault();
                if (autoDeductMaterial != null)
                {
                    bool.TryParse(autoDeductMaterial.Value, out isAutoDeductMaterial);
                }

                //需要显示工序参数录入表单。(过站界面弹框)
                if (lstRouteStepParameter != null && lstRouteStepParameter.Count > 0 && isAutoDeductMaterial == false)
                {
                    ViewBag.lstRouteStepParameter = lstRouteStepParameter;

                    isShowModal = true;
                }
                #endregion                
                
                #region 只有出站才处理的事物
                if (obj.StateFlag==EnumLotState.WaitTrackOut)
                {
                    #region 英利项目必须有防伪编码绑定批次号才可以终检出站-SE项目必须有优化器序列号才可以终检出站
                    bool NeedAttr3 = false;
                    MaterialAttributeServiceClient clientOfMattr = new MaterialAttributeServiceClient();
                    MaterialAttributeKey materialAttributeKey = new MaterialAttributeKey()
                    {
                        MaterialCode = obj.MaterialCode,
                        AttributeName = "MapBitCount"
                    };
                    MethodReturnResult<MaterialAttribute> materialAttributeOfMap = clientOfMattr.Get(materialAttributeKey);
                    if (materialAttributeOfMap.Code == 0 && materialAttributeOfMap.Data != null)
                    {
                        NeedAttr3 = true;
                    }

                    if (NeedAttr3 && obj.RouteStepName == "终检")
                    {
                        if (obj.Attr3 == "" || obj.Attr3 == null)
                        {
                            result.Code = 10;
                            result.Message = string.Format("批次（{0}）的防伪编码或匹配优化器数据不存在。"
                                                            , lotNumber);
                            return Json(result);
                        }
                    }
                    #endregion

                    #region 是否获取层压机信息
                    bool isGetLayerEquipment = false;
                    var lnq = from item in lstRouteStepAttribute
                              where item.Key.AttributeName == "isGetLayerEquipment"
                              select item;
                    RouteStepAttribute rsaTmp = lnq.FirstOrDefault();
                    if (rsaTmp != null)
                    {
                        bool.TryParse(rsaTmp.Value, out isGetLayerEquipment);
                    }

                    if(isGetLayerEquipment)
                    {
                        using (JC.JNMES.ProdLine.TradLevel tClient = new JC.JNMES.ProdLine.TradLevel())
                        {
                            strLayerEquipmentNo = tClient.SearhModulePosition(lotNumber);
                            if (string.IsNullOrEmpty(strLayerEquipmentNo) == false)
                            {
                                string[] arrEqpInfo = strLayerEquipmentNo.Split('*');
                                if (arrEqpInfo.Length > 1)
                                {
                                    strLayerEquipmentNo = arrEqpInfo[1];
                                }
                            }
                        }
                        //strLayerEquipmentNo = "this a test";
                    }
                    ViewBag.IsGetLayerEquipment = isGetLayerEquipment;
                    model.LotLayerEquipmentNo = strLayerEquipmentNo;
                    #endregion

                    #region 是否需要验证【IsCheckELImage，IsCheckIVImage】
                    bool isCheckELImage = false;
                    lnq = from item in lstRouteStepAttribute
                              where item.Key.AttributeName == "IsCheckELImage"
                              select item;
                    rsaTmp = lnq.FirstOrDefault();
                    if (rsaTmp != null)
                    {
                        bool.TryParse(rsaTmp.Value, out isCheckELImage);
                    }

                    //是否检查IV图片。
                    bool isCheckIVImage = false;
                    lnq = from item in lstRouteStepAttribute
                          where item.Key.AttributeName == "IsCheckIVImage"
                          select item;
                    rsaTmp = lnq.FirstOrDefault();
                    if (rsaTmp != null)
                    {
                        bool.TryParse(rsaTmp.Value, out isCheckIVImage);
                    }
                    if(isCheckELImage || isCheckIVImage)
                    {
                        IList<LotAttribute> lstLotAttributes = new List<LotAttribute>();
                        using (LotAttributeServiceClient client = new LotAttributeServiceClient())
                        {
                            PagingConfig cfg = new PagingConfig()
                            {
                                IsPaging = false,
                                Where = string.Format("Key.LotNumber='{0}'"
                                                      ,lotNumber)
                            };
                            MethodReturnResult<IList<LotAttribute>> r = client.Get(ref cfg);
                            if (r.Code <= 0 && r.Data != null)
                            {
                                lstLotAttributes = r.Data;
                            }
                        }

                        //是否显示EL图片
                        bool isExistedELImage = false;
                        LotAttribute rstAttr = new LotAttribute();
                        var lnqOfLotAtts = from item in lstLotAttributes
                                where item.Key.AttributeName == "ELImagePath"
                                select item;
                        rstAttr = lnqOfLotAtts.FirstOrDefault();
                        if (rstAttr != null)
                        {
                            isExistedELImage = true;
                        }
                        if(isCheckELImage && isExistedELImage==false)
                        {
                            result.Code = 10;
                            result.Message = string.Format("批次（{0}）的EL3图片不存在。"
                                                            , lotNumber);
                            return Json(result);
                        }

                        //是否显示IV图片
                        bool isExistedIVImage = false;
                        lnqOfLotAtts = from item in lstLotAttributes
                                            where item.Key.AttributeName == "IVImagePath"
                                            select item;
                        rstAttr = lnqOfLotAtts.FirstOrDefault();
                        if (rstAttr != null)
                        {
                            isExistedIVImage = true;
                        }

                        if (isCheckIVImage && isExistedIVImage == false)
                        {
                            result.Code = 11;
                            result.Message = string.Format("批次（{0}）的IV图片不存在。"
                                                            , lotNumber);
                            return Json(result);
                        }
                    }
                    #endregion
                          
                    //是否输入等级。
                    bool isInputGrade = false;
                    lnq = from item in lstRouteStepAttribute
                              where item.Key.AttributeName == "IsInputGrade"
                              select item;
                    rsaTmp = lnq.FirstOrDefault();
                    if (rsaTmp!=null)
                    {
                        bool.TryParse(rsaTmp.Value, out isInputGrade);
                    }

                    //是否输入电流分档。
                   
                    lnq = from item in lstRouteStepAttribute
                              where item.Key.AttributeName == "IsExecutePowerset"
                              select item;
                    rsaTmp = lnq.FirstOrDefault();
                    if (rsaTmp!=null)
                    {
                        bool.TryParse(rsaTmp.Value, out IsPowersetDetail);
                    }
                    
                    
                    //是否校验条码
                    bool isCheckBarCode = false;
                    lnq = from item in lstRouteStepAttribute
                          where item.Key.AttributeName == "IsCheckBarCode"
                              select item;
                    rsaTmp = lnq.FirstOrDefault();
                    if (rsaTmp != null)
                    {
                        bool.TryParse(rsaTmp.Value, out isCheckBarCode);
                    }
                    if (isCheckBarCode)
                    {
                        ViewBag.IsCheckBarCode = isCheckBarCode;
                        isShowModal = true;
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

                    //是否输入批次路径。
                    bool isShowLotPath = false;
                    lnq = from item in lstRouteStepAttribute
                          where item.Key.AttributeName == "IsShowLotPath"
                          select item;
                    rsaTmp = lnq.FirstOrDefault();
                    if (rsaTmp != null)
                    {
                        bool.TryParse(rsaTmp.Value, out isShowLotPath);
                    }

                    //是否显示EL图片
                    bool isShowELImage = false;
                    lnq = from item in lstRouteStepAttribute
                          where item.Key.AttributeName == "IsShowELImage"
                          select item;
                    rsaTmp = lnq.FirstOrDefault();
                    if (rsaTmp != null)
                    {
                        bool.TryParse(rsaTmp.Value, out isShowELImage);
                    }
                    //是否显示IV图片
                    bool isShowIVImage = false;
                    lnq = from item in lstRouteStepAttribute
                          where item.Key.AttributeName == "IsShowIVImage"
                          select item;
                    rsaTmp = lnq.FirstOrDefault();
                    if (rsaTmp != null)
                    {
                        bool.TryParse(rsaTmp.Value, out isShowIVImage);
                    }

                    //是否显示电流档标签
                    bool IsExecutePowerset = false;
                    lnq = from item in lstRouteStepAttribute
                          where item.Key.AttributeName == "IsExecutePowerset"
                          select item;
                    rsaTmp = lnq.FirstOrDefault();
                    if (rsaTmp != null)
                    {
                        bool.TryParse(rsaTmp.Value, out IsExecutePowerset);
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

                    //获取是否显示返修数据。
                    bool isShowDefect = false;
                    lnq = from item in lstRouteStepAttribute
                          where item.Key.AttributeName == "IsShowDefect"
                          select item;
                    rsaTmp = lnq.FirstOrDefault();
                    if (rsaTmp != null)
                    {
                        bool.TryParse(rsaTmp.Value, out isShowDefect);
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

                    //是否输入批次路径
                    if (isShowLotPath)
                    {
                        ViewBag.IsShowLotPath = isShowLotPath;
                        isShowModal = true;
                    }

                    //是否显示电流档标签
                    if (IsExecutePowerset)
                    {
                        ViewBag.IsExecutePowerset = IsExecutePowerset;
     
                    }


                    //是否显示EL图片
                    if (isShowELImage)
                    {
                        using (LotQueryServiceClient client = new LotQueryServiceClient())
                        {
                            MethodReturnResult<LotAttribute> r = client.GetAttribute(new LotAttributeKey()
                            {
                                LotNumber=obj.Key,
                                AttributeName="ELImagePath"
                            });
                            if (r.Code <= 0 && r.Data != null)
                            {
                                ViewBag.ELImagePath = r.Data.AttributeValue;
                            }
                        }
                        ViewBag.IsShowELImage = isShowELImage;
                        isShowModal = true;
                    }

                    //是否显示IV图片
                    if (isShowIVImage)
                    {
                        using (LotQueryServiceClient client = new LotQueryServiceClient())
                        {
                            MethodReturnResult<LotAttribute> r = client.GetAttribute(new LotAttributeKey()
                            {
                                LotNumber = obj.Key,
                                AttributeName = "IVImagePath"
                            });
                            if (r.Code <= 0 && r.Data != null)
                            {
                                ViewBag.IVImagePath = r.Data.AttributeValue;
                            }
                        }
                        ViewBag.IsShowIVImage = isShowIVImage;
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

                    //是否显示返修数据。
                    if (isShowDefect)
                    {
                        ViewBag.IsShowDefect = isShowDefect;
                        isShowModal = true;
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
                #endregion

                //显示附加对话框。
                if(isShowModal)
                {
                    string barCode1 = "";
                    if (model.RouteOperationName == "终检")
                    {
                        IList<PrintLabelLog> lstPrintLabelLog;//TrackFlag=0，打印铭牌完毕但未出站的数据
                        IList<PrintLabelLog> lstPrintLabelLog1;//TrackFlag=1，出站的数据
                        using (WipEngineerServiceClient client = new WipEngineerServiceClient())//barcode1赋值（yanshan.xiao）
                        {
                            PagingConfig cfg = new PagingConfig()
                            {
                                IsPaging = false,
                                Where = string.Format("Key.LineCode='{0}' AND TrackFlag=0"
                                                        , model.LineCode),
                                OrderBy = "EditTime desc"
                            };
                            PagingConfig cfg1 = new PagingConfig()
                            {
                                IsPaging = false,
                                Where = string.Format("Key.LineCode='{0}' AND TrackFlag=1"
                                                        , model.LineCode),
                                OrderBy = "EditTime desc"
                            };
                            MethodReturnResult<IList<PrintLabelLog>> r = client.Get(ref cfg);//如果等待出站的时间小于已出站的时间，则不操作
                            MethodReturnResult<IList<PrintLabelLog>> r1 = client.Get(ref cfg1);
                            if (r.Data.Count > 0 && r1.Data.Count == 0)
                            {
                                lstPrintLabelLog = r.Data;
                                barCode1 = lstPrintLabelLog[0].Key.LotNumber;
                            }
                            else if (r.Data.Count > 0 && r1.Data.Count > 0)
                            {
                                lstPrintLabelLog = r.Data;
                                lstPrintLabelLog1 = r1.Data;
                                if (lstPrintLabelLog[0].EditTime > lstPrintLabelLog1[0].EditTime)
                                {
                                    barCode1 = lstPrintLabelLog[0].Key.LotNumber;
                                }
                            }
                        }
                    }
                    
                    Response.StatusDescription = "Partial";
                    ViewBag.Lot = obj;

                    LotTrackViewModel m = new LotTrackViewModel
                    {
                        LotLayerEquipmentNo = strLayerEquipmentNo,      //层压机号
                        RouteName = obj.RouteName,                      //工艺流程
                        RouteOperationName = obj.RouteStepName,         //工序
                        OrderNumber = obj.OrderNumber,                  //工单号
                        Barcode1=barCode1,                               //自动读头检验条码1
                        IsShowDialog=model.IsShowDialog
                    };
                   
                    return PartialView("_ModalContentPartial",m);
                }

                result = Track(obj, model, isAutoDeductMaterial);
                result.HelpLink = Convert.ToString(IsPowersetDetail);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveModal(LotTrackViewModel model)
        {       
            MethodReturnResult result = new MethodReturnResult();
            bool isGetMaterialLot = false;
            string lotNumber = "";                          //批次代码
            Lot lot = null;                                 //批次对象
            try
            {
                //创建批次对象
                lotNumber = model.LotNumber.ToUpper();      //取得批次代码（字母调整为大写）

                //取得批次对象并进行基本校验
                result = GetLot(lotNumber);

                if (result.Code > 0)
                {
                    return Json(result);
                }
                
                MethodReturnResult<Lot> rst = result as MethodReturnResult<Lot>;
                lot = rst.Data;

                //取得当前工步属性
                IList<RouteStepAttribute> lstRouteStepAttribute = new List<RouteStepAttribute>();
                using (RouteStepAttributeServiceClient client = new RouteStepAttributeServiceClient())
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format("Key.RouteName='{0}' AND Key.RouteStepName='{1}'"
                                              , lot.RouteName
                                              , lot.RouteStepName)
                    };
                    MethodReturnResult<IList<RouteStepAttribute>> r = client.Get(ref cfg);
                    if (r.Code <= 0 && r.Data != null)
                    {
                        lstRouteStepAttribute = r.Data;
                    }
                }
                
                #region 是否自动获取物料批号
                var lnqGetMaterialLot = from item in lstRouteStepAttribute
                                        where item.Key.AttributeName == "isGetMaterialLot"
                                        select item;
                RouteStepAttribute getMaterialLot = lnqGetMaterialLot.FirstOrDefault();

                if (getMaterialLot != null)
                {
                    bool.TryParse(getMaterialLot.Value, out isGetMaterialLot);
                }
                #endregion
                
                //出入站操作
                result = Track(rst.Data, model, isGetMaterialLot);
             
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }
            
            return Json(result);
        }

        public MethodReturnResult PackageMessage(string lotNumber,string packageLine)
        {
             MethodReturnResult resultPackage = new MethodReturnResult();//包装护角信息
             Lot lot = null;                                 //批次对象
             lotNumber = lotNumber.ToUpper();      //取得批次代码（字母调整为大写）
            using(LotQueryServiceClient client = new LotQueryServiceClient())
            {
                lot = client.Get(lotNumber).Data;
                if (lot == null)
                {
                    resultPackage.Code = 1000;
                    resultPackage.Message = "无符合条件的批次信息";
                    return resultPackage;
                }
            }
            using (WipEngineerServiceClient client = new WipEngineerServiceClient())
            {
                resultPackage = client.ExecuteInPackageDetail(lot, packageLine);
                if (localName == "G01")
                {
                    if (resultPackage.Code > 0)
                    {
                        resultPackage = client.ExecuteInAbnormalBIN(lot, packageLine);
                    }
                }
                return resultPackage;
            }
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void ProcessRequest(string voice)
        {
            Thread t = null;
            HttpContext.Response.ContentType = "application/wav";
            using (MemoryStream ms = new MemoryStream())
            {
                t = new Thread(() =>
                {
                    SpeechSynthesizer ss = new SpeechSynthesizer();
                    try
                    {
                        ss.Rate = -5;
                        ss.Volume = 100;
                        ss.SetOutputToWaveStream(ms);
                        ss.Speak(voice);
                    }
                    catch (Exception ex)
                    {
                        ss.Dispose();
                        HttpContext.Response.Write(ex.Message);
                    }
                });
                t.Start();
                t.Join();
                ms.Position = 0;
                if (ms.Length > 0)
                {
                    ms.WriteTo(HttpContext.Response.OutputStream);
                }
               HttpContext.Response.End();

            }

        }

        /// <summary>
        /// 播放语音信息
        /// </summary>
        /// <param name="message">语音内容</param>
        //static void GetVoice(object message)
        //{
        //    SpeechSynthesizer reader = new SpeechSynthesizer();
        //    reader.Volume = 100;
        //    reader.SpeakAsync(message.ToString());
           
        //}

        /// <summary> 批次过站作业 </summary>
        /// <param name="lot">批次对象</param>
        /// <param name="model">过站模型对象</param>
        /// <param name="isAutoDeductMaterial">是否自动扣料</param>
        /// <returns>返回结果。</returns>
        private MethodReturnResult Track(Lot lot, LotTrackViewModel model, bool isAutoDeductMaterial)
        {
           
            //批次代码
            string lotNumber = model.LotNumber.ToUpper();

            MethodReturnResult result = new MethodReturnResult();
          
            MethodReturnResult<DataTable> dtResult = new MethodReturnResult<DataTable>();
            IDictionary<string, IList<MaterialConsumptionParameter>> dicMaterialParams = new Dictionary<string, IList<MaterialConsumptionParameter>>();

            //获取工序参数列表(物料扣料列表)
            IList<RouteStepParameter> lstRouteStepParameter = GetParameterList(lot.RouteName, lot.RouteStepName, lot.StateFlag);

            //工序参数列表(物料扣料列表)
            if (lstRouteStepParameter != null && lstRouteStepParameter.Count > 0)
            {
                //非自动扣料时创建扣料列表
                if (isAutoDeductMaterial == false)
                {
                    //数据分隔符
                    char splitChar = ',';

                    //获取物料数据
                    string[] materialCodes = Request["MaterialCode"].ToUpper().Split(splitChar);    //物料代码
                    string[] materialLots = Request["MaterialLot"].ToUpper().Split(splitChar);      //物料批次
                    string[] loadingQtys = Request["LoadingQty"].ToUpper().Split(splitChar);        //扣料数量

                    string sMaterialCode = "";
                    string sMaterialLot = "";
                    string sLoadingQty = "";
                    bool bIsFind = false;
                    string sCookiesName = "";

                    dicMaterialParams.Add(lot.Key, new List<MaterialConsumptionParameter>());

                    #region 创建物料信息列表
                    foreach (RouteStepParameter routeStepParameter in lstRouteStepParameter)
                    {
                        sMaterialCode = routeStepParameter.MaterialType;

                        bIsFind = false;

                        //取得对应的记录序号
                        for (int i = 0; i < materialCodes.Count(); i++)
                        {
                            if (materialCodes[i] == sMaterialCode)          //物料号相等
                            {
                                sMaterialLot = materialLots[i];             //批次号
                                sLoadingQty = loadingQtys[i];               //扣料数量

                                //创建数据节点
                                MaterialConsumptionParameter mcp = new MaterialConsumptionParameter()
                                {
                                    MaterialCode = sMaterialCode,
                                    MaterialLot = sMaterialLot,
                                    LoadingQty = Convert.ToDecimal(sLoadingQty)
                                };

                                //加入节点
                                dicMaterialParams[lot.Key].Add(mcp);

                                bIsFind = true;

                                //记录本次录入值，作为下次录入的默认值
                                sCookiesName = sMaterialCode;

                                if (Request.Cookies.Get(sCookiesName) != null)
                                {
                                    Response.SetCookie(new HttpCookie(sCookiesName, sMaterialLot));
                                }
                                else if (!string.IsNullOrEmpty(sCookiesName))
                                {
                                    Response.Cookies.Add(new HttpCookie(sCookiesName, sMaterialLot));
                                }
                            }
                        }

                        if (bIsFind == false)       //未发现数据
                        {
                            result.Code = 1000;
                            result.Message = string.Format("物料{0}无扣料数据！请录入。",
                                                            routeStepParameter.Key.ParameterName);

                            return result;
                        }
                    }
                    #endregion

                    #region 获取物料信息列表（已废止！！！）
             
                    #endregion
                } 
            }

            //bool istest = true;

            //if (istest)
            //{
            //    return result;
            //}

            #region 批次进站
            if (lot.StateFlag == EnumLotState.WaitTrackIn)
            {
                //创建进站参数
                TrackInParameter p = new TrackInParameter()
                {
                    Creator = User.Identity.Name,
                    EquipmentCode = model.EquipmentCode,
                    LineCode = model.LineCode,
                    LotNumbers = new List<string>(),
                    OperateComputer = Request.UserHostAddress,
                    Operator = User.Identity.Name,
                    MaterialParamters = dicMaterialParams,              //扣料明细
                    Remark = model.Description,
                    RouteName = lot.RouteName,
                    RouteOperationName = model.RouteOperationName
                };

                p.LotNumbers.Add(lotNumber);

                //进行批次进站                
                using (WipEngineerServiceClient client = new WipEngineerServiceClient())
                {
                    //调用进站操作
                    result = client.TrackInLot(p);

                    if (result.Code == 0)
                    {      
                        result.Message = string.Format("批次 {0} 进站成功。", lotNumber);
                    }
                    else
                    {
                        return result;
                    }
                }
            }
            #endregion

            #region 批次出站
            else if(lot.StateFlag==EnumLotState.WaitTrackOut)
            {
                TrackOutParameter p = new TrackOutParameter()
                {
                    LineCode = model.LineCode,                          //线别
                    LotNumbers = new List<string>(),                    //批次数组     
                    MaterialParamters = dicMaterialParams,              //扣料明细
                    Remark = model.Description,                         //备注
                    RouteName = lot.RouteName,                          //工艺流程
                    RouteOperationName = model.RouteOperationName,      //工序
                    EquipmentCode = model.EquipmentCode,                //设备代码
                    Color = model.Color,                                //颜色
                    Grade = model.Grade,                                //等级
                    AutoDeductMaterial = isAutoDeductMaterial,          //自动扣料标识
                    Creator = User.Identity.Name,                       //创建人
                    OperateComputer = Request.UserHostAddress,          //操作客户端
                    Operator = User.Identity.Name,                      //操作人
                };
                p.LotNumbers.Add(lotNumber);

                //增加检验数据。
                if(string.IsNullOrEmpty(model.Color)==false
                   || string.IsNullOrEmpty(model.Grade) == false)
                {
                    if (!string.IsNullOrEmpty(model.Barcode1)
                        && model.Barcode1!=lot.Key)
                    {
                        result.Code = 3001;
                        result.Message = string.Format("检验条码1 （{0}）同组件序列号（{1}）不一致,请确认。", model.Barcode1, lot.Key);
                        return result;
                    }

                    //if (!string.IsNullOrEmpty(model.Barcode2)
                    //    && model.Barcode2 != lot.Key)
                    //{
                    //    result.Code = 3002;
                    //    result.Message = string.Format("检验条码2 （{0}）同组件序列号（{1}）不一致,请确认。", model.Barcode2, lot.Key);
                    //    return result;
                    //}

                    p.CheckBarcodes = new Dictionary<string, IList<string>>();
                    if (!p.CheckBarcodes.ContainsKey(lot.Key))
                    {
                        p.CheckBarcodes.Add(lot.Key, new List<string>());
                    }
                    p.CheckBarcodes[lot.Key].Add(model.Barcode1);
                    //p.CheckBarcodes[lot.Key].Add(model.Barcode2);
                }

                #region 进行不良数量记录(取消)
                //IList<ReasonCodeCategoryDetail> lstDefectReasonCodes = GetDefectReasonCodes(lot.RouteName, lot.RouteStepName);
                //p.DefectReasonCodes=new Dictionary<string,IList<DefectReasonCodeParameter>>();
                //if (lstDefectReasonCodes != null && lstDefectReasonCodes.Count > 0)
                //{

                //    foreach (ReasonCodeCategoryDetail item in lstDefectReasonCodes)
                //    {
                //        string hashcode = string.Format("{0}{1}", item.Key.ReasonCodeCategoryName, item.Key.ReasonCodeName)
                //                                .GetHashCode()
                //                                .ToString()
                //                                .Replace('-', '_');
                //        string inputControlName = string.Format("DefectReasonCode_{0}", hashcode);
                //        string val = Request.Form[inputControlName];
                //        double dVal = 0;
                //        if (string.IsNullOrEmpty(val)
                //            || double.TryParse(val, out dVal)==false
                //            || dVal == 0)
                //        {
                //            continue;
                //        }
                //        if (!p.DefectReasonCodes.ContainsKey(lot.Key))
                //        {
                //            p.DefectReasonCodes.Add(lot.Key, new List<DefectReasonCodeParameter>());
                //        }
                //        DefectReasonCodeParameter drcp = new DefectReasonCodeParameter()
                //        {
                //            ReasonCodeCategoryName=item.Key.ReasonCodeCategoryName,
                //            ReasonCodeName=item.Key.ReasonCodeName,
                //            Quantity=dVal,
                //            Description=string.Empty,
                //            ResponsiblePerson=string.Empty,
                //            RouteOperationName=string.Empty
                //        };
                //        p.DefectReasonCodes[lot.Key].Add(drcp);
                //    }
                //}
                #endregion

                #region 进行报废数量记录(取消)
                //IList<ReasonCodeCategoryDetail> lstScrapReasonCodes = GetScrapReasonCodes(lot.RouteName, lot.RouteStepName);
                //p.ScrapReasonCodes = new Dictionary<string, IList<ScrapReasonCodeParameter>>();
                //if (lstScrapReasonCodes != null && lstScrapReasonCodes.Count > 0)
                //{
                //    foreach (ReasonCodeCategoryDetail item in lstScrapReasonCodes)
                //    {
                //        string hashcode = string.Format("{0}{1}", item.Key.ReasonCodeCategoryName, item.Key.ReasonCodeName)
                //                                .GetHashCode()
                //                                .ToString()
                //                                .Replace('-', '_');
                //        string inputControlName = string.Format("ScrapReasonCode_{0}", hashcode);
                //        string val = Request.Form[inputControlName];
                //        double dVal = 0;
                //        if (string.IsNullOrEmpty(val)
                //            || double.TryParse(val, out dVal) == false
                //            || dVal == 0)
                //        {
                //            continue;
                //        }
                //        if (!p.ScrapReasonCodes.ContainsKey(lot.Key))
                //        {
                //            p.ScrapReasonCodes.Add(lot.Key, new List<ScrapReasonCodeParameter>());
                //        }
                //        ScrapReasonCodeParameter srcp = new ScrapReasonCodeParameter()
                //        {
                //            ReasonCodeCategoryName = item.Key.ReasonCodeCategoryName,
                //            ReasonCodeName = item.Key.ReasonCodeName,
                //            Quantity = dVal,
                //            Description = string.Empty,
                //            ResponsiblePerson = string.Empty,
                //            RouteOperationName = string.Empty
                //        };
                //        p.ScrapReasonCodes[lot.Key].Add(srcp);
                //    }
                //}
                #endregion

                #region 层压机设备号
                p.Attributes = new Dictionary<string, IList<TransactionParameter>>();
                if (model.LotLayerEquipmentNo != null && model.LotLayerEquipmentNo.Length>0)
                {
                    p.Attributes.Add(lotNumber, new List<TransactionParameter>());
                    TransactionParameter transactionParameter = new TransactionParameter
                    {
                        Name = "LayerEquipmentNo",
                        Value = model.LotLayerEquipmentNo
                    };
                    p.Attributes[lotNumber].Add(transactionParameter);
                }
                #endregion

                //进行批次出站
                using (WipEngineerServiceClient client = new WipEngineerServiceClient())
                {
                    MethodReturnResult resultPackage = new MethodReturnResult();

                    //晋中A线机械手触发动作
                    //if (localName == "G01")
                    //{
                    //    if (p.RouteOperationName == "终检")
                    //    {
                    //        ClassifyContractClient clientForA = new ClassifyContractClient("httpService");
                    //        WebForBin webForBin = new WebForBin();
                    //        webForBin.LotNumber = lotNumber;
                    //        webForBin.LineName = p.LineCode;
                    //        string jsonOfPackageBin = JsonConvert.SerializeObject(webForBin);
                    //        clientForA.TriggerClassifyByJsonForMES(jsonOfPackageBin);
                    //    }
                    //}

                    result = client.TrackOutLot(p);
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


                        //晋中A线机械手触发动作
                        if (localName == "G01" && model.IsAutoToBin==true)
                        {
                            if (p.RouteOperationName == "终检")
                            {
                                Task ts = ToJinChen(lotNumber, p);
                            }
                        }

                        //SE组件打印背面条码
                        if (lotNumber.ToUpper().Contains("SE") || lotNumber.ToUpper().Contains("SY") || lotNumber.ToUpper().Contains("SN"))
                        {
                            MethodReturnResult resultOfPrint = new MethodReturnResult();
                            string IsPrintQR = System.Configuration.ConfigurationSettings.AppSettings["IsPrintQR"];
                            if (IsPrintQR=="是")
                            {
                                resultOfPrint = Print(lot);
                                if (!string.IsNullOrEmpty(result.Message))
                                {
                                    if (!result.Message.EndsWith("\n"))
                                    {
                                        result.Message += "\n";
                                    }
                                    result.Message = result.Message.Replace("\n", "<br/>");
                                }
                                result.Message += resultOfPrint.Message;
                            }                           
                        }
                        
                        if (model.IsShowDialog == true)
                        {
                           
                               resultPackage = PackageMessage(lotNumber,model.LineCode);
                               //Thread thVoice = new Thread(new ParameterizedThreadStart(GetVoice));
                               //if (resultPackage.Code > 0)
                               //{
                               //    thVoice.Start("异常组件");
                               //}
                               //else
                               //{
                               //    thVoice.Start(resultPackage.Message);
                               //}
                               result.Message += "<font size='100' color='red' >" + resultPackage.Message + "</font>";
                        }  
                    }
                }
                
            }
            #endregion
            else
            {
                 result.Code = 100;
                 result.Message = string.Format("批次 {0} 状态为（{1}），不能进行工作站作业。"
                                                , lotNumber
                                                , lot.StateFlag.GetDisplayName());
            }
            return result;
        }


        /// <summary>
        /// 异步触发调用金辰接口
        /// </summary>
        /// <param name="lotNumber"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        private async Task ToJinChen(string lotNumber,TrackOutParameter p)
        {
            await Task.Run(() =>
            {
                ClassifyContractClient clientForA = new ClassifyContractClient("httpService");
                WebForBin webForBin = new WebForBin();
                webForBin.LotNumber = lotNumber;
                webForBin.LineName = p.LineCode;
                string jsonOfPackageBin = JsonConvert.SerializeObject(webForBin);
                clientForA.TriggerClassifyByJsonForMES(jsonOfPackageBin);
            });
        }

        /// <summary> 自动获取上料物料批号 </summary>
        /// <param name="materialType"></param>
        /// <param name="lineCode"></param>
        /// <param name="routeStepName"></param>
        /// <param name="orderNumber"></param>
        /// <param name="equipmentCode"></param>
        /// <returns></returns>
        private  MethodReturnResult<DataTable> GetParameterLotList(string materialType, string lineCode, string routeStepName, string orderNumber, string equipmentCode)
        {
            MethodReturnResult<DataTable> dtResult = null;
            string sql2 = string.Format(@"SELECT TOP 1 t1.MATERIAL_LOT
                                          FROM [dbo].[LSM_MATERIAL_LOADING_DETAIL] t1 
                                              INNER JOIN dbo.LSM_MATERIAL_LOADING t2  ON t2.LOADING_KEY=t1.LOADING_KEY
                                              INNER JOIN [dbo].[FMM_MATERIAL] t3 ON t3.MATERIAL_CODE=t1.MATERILA_CODE
                                          WHERE t3.MATERIAL_TYPE = '{0}' AND t2.LINE_CODE='{1}'
		                                             AND t2.ROUTE_OPERATION_NAME='{2}' AND t1.ORDER_NUMBER='{3}'
		                                             AND t2.EQUIPMENT_CODE='{4}' AND t1.CURRENT_QTY>0
										  ORDER BY t1.EDIT_TIME ASC,t1.ITEM_NO ASC"
                                         , materialType
                                         , lineCode
                                         , routeStepName
                                         , orderNumber
                                         , equipmentCode
                                         );
           
            using (DBServiceClient client = new DBServiceClient())
            {
                dtResult = client.ExecuteQuery(sql2);
            }
            return dtResult;
        }

        /// <summary> 自动获取电池片物料批号 </summary>
        /// <param name="materialType"></param>
        /// <param name="lineCode"></param>
        /// <param name="routeStepName"></param>
        /// <param name="orderNumber"></param>
        /// <param name="equipmentCode"></param>
        /// <returns></returns>
        private MethodReturnResult<DataTable> GetCellLotList(string materialType, string lineCode, string routeStepName, string orderNumber, string equipmentCode)
        {
            //string materialLot = null;
            MethodReturnResult<DataTable> dtResult = null;
            string sql2 = string.Format(@"SELECT TOP 1 t1.MATERIAL_LOT
                                          FROM [dbo].[LSM_MATERIAL_LOADING_DETAIL] t1 
                                              INNER JOIN dbo.LSM_MATERIAL_LOADING t2  ON t2.LOADING_KEY=t1.LOADING_KEY
                                              INNER JOIN [dbo].[FMM_MATERIAL] t3 ON t3.MATERIAL_CODE=t1.MATERILA_CODE
                                          WHERE t3.MATERIAL_TYPE like '{0}%' AND t2.LINE_CODE='{1}'
		                                             AND t2.ROUTE_OPERATION_NAME='{2}' AND t1.ORDER_NUMBER='{3}'
		                                             AND t2.EQUIPMENT_CODE='{4}' AND t1.CURRENT_QTY>0
                                          ORDER BY t1.EDIT_TIME ASC,t1.ITEM_NO ASC"
                                         , materialType
                                         , lineCode
                                         , routeStepName
                                         , orderNumber
                                         , equipmentCode
                                         );
            DataTable dt2 = new DataTable();
            using (DBServiceClient client = new DBServiceClient())
            {
                 dtResult = client.ExecuteQuery(sql2);
                if (dtResult.Code == 0)
                {
                    dt2 = dtResult.Data;
                    //materialLot = dt2.Rows[0][0].ToString();
                }
            }
            //return materialLot;
            return dtResult;
        }
    
         public ActionResult GetLotObj(string lotNumber)
        {
            MethodReturnResult result = new MethodReturnResult();
            MethodReturnResult<Lot> rst = null;
            Lot obj = null;

            using (LotQueryServiceClient client = new LotQueryServiceClient())
            {
                rst = client.Get(lotNumber);
                if (rst.Code == 0 && rst.Data != null)
                {                    
                    obj = rst.Data;
                    return Json(obj,JsonRequestBehavior.AllowGet);
                  
               
                }
                return Json(null,JsonRequestBehavior.AllowGet);
            }
            
        }


         MethodReturnResult GetLot(string lotNumber)
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
                 if (rst.Code == 0 && rst.Data != null)
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

             //测试节点请删除！！！！！！！！！
             //bool istest = true;
             //if (istest)
             //{

             //    obj.RouteStepName = "功率测试";
             //    obj.StateFlag = EnumLotState.WaitTrackOut;
             //    obj.Status = EnumObjectStatus.Available;
             //}

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
                 string res = null;
                 string res2 = null;

                 string sql = string.Format(@"select ATTR_4  from WIP_LOT where LOT_NUMBER='{0}'", lotNumber);
                 DataTable dt = new DataTable();
                 using (DBServiceClient client = new DBServiceClient())
                 {
                     MethodReturnResult<DataTable> dtResult = client.ExecuteQuery(sql);
                     if (result.Code == 0)
                     {
                         dt = dtResult.Data;
                         res = dt.Rows[0][0].ToString();
                     }
                 }

                 string sql2 = string.Format(@"select top 1 t2.HOLD_DESCRIPTION  from  WIP_TRANSACTION  t1
                                                   inner join [dbo].[WIP_TRANSACTION_HOLD_RELEASE]  t2 on  t1.TRANSACTION_KEY=t2.TRANSACTION_KEY
                                                   inner join WIP_LOT t3  on t3.LOT_NUMBER = t1.LOT_NUMBER  
                                                   where t1.LOT_NUMBER='{0}'
                                                   order by t2.HOLD_TIME  desc", lotNumber);
                 DataTable dt2 = new DataTable();
                 using (DBServiceClient client2 = new DBServiceClient())
                 {
                     MethodReturnResult<DataTable> dtResult2 = client2.ExecuteQuery(sql2);
                     if (result.Code == 0 && dtResult2.Data != null && dtResult2.Data.Rows.Count > 0)
                     {
                         dt2 = dtResult2.Data;
                         res2 = dt2.Rows[0][0].ToString();
                     }
                 }

                 if (dt != null && dt.Rows.Count > 0 && res != null && res != "")
                 {
                     result.Code = 2004;
                     result.Message = string.Format("批次（{0}）已暂停,原因为：{1}。", lotNumber, res);
                 }
                 else if (dt != null && dt.Rows.Count > 0 && res2 != null && res2 != "")
                 {
                     result.Code = 2004;
                     result.Message = string.Format("批次（{0}）已暂停。", lotNumber);
                 }
                 else
                 {
                     result.Code = 2004;
                     result.Message = string.Format("批次（{0}）已暂停。", lotNumber);
                 }
                 return result;
             }
             return rst;
         }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="routeName"></param>
        /// <param name="routeStepName"></param>
        /// <param name="stateFlag"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="routeName"></param>
        /// <param name="routeStepName"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="routeName"></param>
        /// <param name="routeStepName"></param>
        /// <returns></returns>
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

        public ActionResult Prints(LotTrackViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                string lotNumber = "";
                if (model.LotNumber != null && model.LotNumber != "")
                {
                    lotNumber = model.LotNumber.Trim().ToUpper();
                }
                else
                {
                    result.Code = 1000;
                    result.Message = "输入的批次号为空。";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                if (lotNumber.ToUpper().Contains("SE") || lotNumber.ToUpper().Contains("SY") || lotNumber.ToUpper().Contains("SN"))
                {
                    result = GetLot(lotNumber);
                    if (result.Code > 0)
                    {
                        result.Message = string.Format("批次{0}不存在", lotNumber);
                        return Json(result);
                    }

                    //取得批次信息
                    MethodReturnResult<Lot> rst = result as MethodReturnResult<Lot>;
                    Lot lot = rst.Data;
                    if ((lot.RouteStepName == "终检" && lot.StateFlag == EnumLotState.WaitTrackOut)
                        || (lot.RouteStepName == "包装" && lot.StateFlag == EnumLotState.WaitTrackOut))
                    {
                        result = PrintPrivate(lot);
                        if (result.Code == 0)
                        {
                            result.Message = "打印标签成功。";
                        }
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        result.Code = 1000;
                        result.Message = string.Format("批次{0}非（终检或包装）等待出站状态。",lotNumber);
                    }
                }
                else
                {
                    result.Code = 1000;
                    result.Message = "非SE组件，不可打印标签。";
                }
                
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }
            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public MethodReturnResult Print(Lot lot)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                result = PrintPrivate(lot);
                if (result.Code == 0)
                {
                    result.Message = "打印正面二维码标签成功。";
                }
                return result;
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }
            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return result;
        }

        private MethodReturnResult PrintPrivate(Lot lot)
        {
            MethodReturnResult result = new MethodReturnResult();
            result.Code = 0;

            try
            {
                LotPackageSEModulesViewModel SEModulesModel = new LotPackageSEModulesViewModel();
                IList<ClientConfigAttribute> lst = new List<ClientConfigAttribute>();
                string hostName = HttpContext.Request.UserHostName;
                string attributeName = "PrinterName";

                using (ClientConfigAttributeServiceClient client = new ClientConfigAttributeServiceClient())
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format("Key.ClientName='{0}' AND Key.AttributeName LIKE '{1}%'"
                                              , hostName
                                              , attributeName),
                        OrderBy = "Key.AttributeName"
                    };
                    MethodReturnResult<IList<ClientConfigAttribute>> resultOfPrint = client.Get(ref cfg);
                    if (resultOfPrint.Code <= 0 && resultOfPrint.Data != null && resultOfPrint.Data.Count > 0)
                    {
                        lst = resultOfPrint.Data;
                    }
                    else
                    {
                        result.Code = 1000;
                        result.Message = string.Format("当前电脑{0}未挂载网络打印机。"
                                                , hostName);
                        return result;
                    }
                }                

                //获取当前电脑IP所挂载的网络打印机名称
                ClientConfig printer = null;
                using (ClientConfigServiceClient client = new ClientConfigServiceClient())
                {
                    MethodReturnResult<ClientConfig> rst = client.Get(lst[0].Value);
                    if (rst.Code > 0)
                    {
                        return rst;
                    }
                    printer = rst.Data;
                }

                //获取打印条码内容
                PrintLabel label = null;
                using (PrintLabelServiceClient client = new PrintLabelServiceClient())
                {
                    MethodReturnResult<PrintLabel> rst = client.Get(SEPrintLab);
                    if (rst.Code > 0)
                    {
                        return rst;
                    }
                    label = rst.Data;
                }

                DateTime PrintStart = DateTime.Now;

                #region 获取批次IV数据
                IVTestData ivTestData = GetIVTestDataOfSE(lot.Key);
                #endregion

                #region 获取批次所属档位
                //WorkOrderPowerset workOrderPowerset=GetWorkOrderPowersetOfSE(lot,ivTestData);
                #endregion
                
                string dataNumber = "";
                string attr3 = "";
                if (lot.Attr3 != null && lot.Attr3 != "")
                {
                    attr3 = lot.Attr3.Trim();
                    dataNumber = attr3.Substring(attr3.Length - 11, 11);
                }
                else
                {
                    result.Code = 1000;
                    result.Message = string.Format("批次号{0}优化器序列号不存在。", lot.Key);
                    return result;
                }

                if (SEPrintParam == "ProductType")
                {
                    #region 获取对应副标签上的产品型号
                    dataNumber = SEModulesModel.GetProductTypes(lot.MaterialCode, lot.OrderNumber, ivTestData.PowersetCode, ivTestData.PowersetItemNo.Value, lot);
                    #endregion
                }                

                //根据打印数量设置打印机模板。
                using (IPrintHelper helper = PrintHelperFactory.CreatePrintHelper(label.Content))
                {
                    PrintStart = DateTime.Now;          //打印开始时间

                    //打印动态内容。
                    dynamic printData = new ExpandoObject();
                    printData.DataNumber = dataNumber;
                    printData.PrintQty = 1;
                    bool bSuccess = false;

                    //根据打印机类型，调用不同的打印方法。
                    if (printer.ClientType == EnumClientType.NetworkPrinter)        //网络打印机
                    {
                        string[] vals = printer.IPAddress.Split(':');
                        string port = "9100";

                        if (vals.Length > 1)
                        {
                            port = vals[1];
                        }

                        bSuccess = helper.NetworkPrint(vals[0], port, label.Content, printData);
                    }
                    else if (printer.ClientType == EnumClientType.RawPrinter)       //本地打印机
                    {
                        bSuccess = helper.RAWPrint(printer.IPAddress, label.Content, printData);
                    }
                    else
                    {
                        result.Code = 1001;
                        result.Message = "打印失败,打印机类型不正确。";
                        return result;
                    }

                    //返回打印结果。
                    if (bSuccess == false)
                    {
                        result.Code = 1001;
                        result.Message = "批次标签打印失败。";
                        return result;
                    }

                    //打印日志
                    using (PrintLogServiceClient client = new PrintLogServiceClient())
                    {
                        PrintLog obj = new PrintLog()
                        {
                            LotNumber = lot.Key,                  //批次号
                            ClientName = Request.UserHostAddress,   //客户端
                            PrintQty = 1,                           //打印数量 
                            PrintLabelCode = SEPrintLab,            //打印标签代码
                            PrinterName = lst[0].Value,             //打印机名称
                            PrintType = printer.ClientType.GetDisplayName(),    //打印机类型
                            IsSucceed = true,                       //打印是否成功
                            PrintData = dataNumber,                 //打印数据
                            Creator = User.Identity.Name,           //创建人
                            CreateTime = PrintStart,                //创建日期                               
                            FinishTime = DateTime.Now               //编辑日期     
                        };

                        result = client.Add(obj);

                        if (result.Code > 0)
                        {
                            return result;
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

            return result;
        }

        /// <summary>
        /// 获取批次IV数据
        /// </summary>
        /// <param name="lotNumber"></param>
        /// <returns></returns>
        public IVTestData GetIVTestDataOfSE(string lotNumber)
        {
            IVTestData ivTestData = null;
            using (IVTestDataServiceClient client = new IVTestDataServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key.LotNumber='{0}' AND IsDefault=1", lotNumber)
                };
                MethodReturnResult<IList<IVTestData>> result = client.Get(ref cfg);

                if (result.Code == 0 && result.Data != null & result.Data.Count > 0)
                {
                    ivTestData = result.Data[0];
                    return ivTestData;
                }
            }
            return ivTestData;
        }


        /// <summary>
        /// 获取功率对应分档规则
        /// </summary>
        /// <param name="lot"></param>
        /// <param name="ivTestData"></param>
        /// <returns></returns>
        public WorkOrderPowerset GetWorkOrderPowersetOfSE(Lot lot, IVTestData ivTestData)
        {
            WorkOrderPowerset workOrderPowerset = null;
            using (WorkOrderPowersetServiceClient client = new WorkOrderPowersetServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key.OrderNumber='{0}' AND Key.MaterialCode='{1}' AND Key.Code='{2}' AND Key.ItemNo='{3}'"
                                                , lot.OrderNumber,lot.MaterialCode,ivTestData.PowersetCode,ivTestData.PowersetItemNo)
                };
                MethodReturnResult<IList<WorkOrderPowerset>> result = client.Get(ref cfg);

                if (result.Code == 0 && result.Data != null & result.Data.Count > 0)
                {
                    workOrderPowerset = result.Data[0];
                    return workOrderPowerset;
                }
            }
            return workOrderPowerset;
        }

	}
}

