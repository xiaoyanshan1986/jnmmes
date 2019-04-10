using ServiceCenter.Client.Mvc.Areas.WIP.Models;
using ServiceCenter.Common.Print;
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
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ServiceCenter.Common;

namespace ServiceCenter.Client.Mvc.Areas.WIP.Controllers
{
    public class LotCreateController : Controller
    {
        //
        // GET: /WIP/LotCreate/
        public ActionResult Index()
        {
            return View(new LotCreateMainViewModel());
        }

        //
        // POST: /WIP/LotCreate/Detail
        public ActionResult Detail(LotCreateMainViewModel model)
        {
            string sMsg = "";
            LotCreateMainViewModel detailmodel = new LotCreateMainViewModel();
            bool bis = false;

            try
            {
                bis = ModelState.IsValid;

                //复制原Model中数据
                TryUpdateModel(detailmodel);

                //获取工单信息。
                using (WorkOrderServiceClient client = new WorkOrderServiceClient())
                {
                    MethodReturnResult<WorkOrder> result = client.Get(model.OrderNumber.Trim());
                    if (result.Code > 0)
                    {
                        return Json(result);
                    }

                    //工单是否存在
                    if (result.Data == null)
                    {
                        sMsg = string.Format("工单:({0})不存在，请重新录入！", model.OrderNumber.Trim());

                        return Content("<script >alert('" + sMsg + "');</script >", "text/html");
                    }

                    //判断工单与车间是否一致
                    if (model.LocationName != result.Data.LocationName)
                    {
                        sMsg = string.Format("工单:({0})为{1}车间工单，请重新选择！", model.OrderNumber.Trim(), result.Data.LocationName);

                        return Content("<script >alert('" + sMsg + "');</script >", "text/html");
                    }

                    //根据工单代码取得工单类型（正常、实验、返工）
                    //switch (result.Data.OrderType)
                    //switch (model.OrderNumber.Substring(0,3))
                    //{
                    //    case "1MO":     //正常工单
                    //        DetailModel.LotType = EnumLotType.Normal;

                    //        break;
                    //    case "2MO":     //返工工单
                    //        DetailModel.LotType = EnumLotType.Rework;

                    //        break;
                    //    case "3MO":     //实验工单
                    //        DetailModel.LotType = EnumLotType.Experiment;

                    //        break;
                    //    default:
                    //        result.Code = 1000;
                    //        result.Message = "工单号类型错误！";
                    //        result.Detail = result.Message;

                    //        return Json(result);
                    //        //break;
                    //}   

                    #region 产品信息（取得单批次产品数量）
                    using (MaterialServiceClient materialclient = new MaterialServiceClient())
                    {
                        MethodReturnResult<Material> materialresult = materialclient.Get(result.Data.MaterialCode);
                        if (materialresult.Code <= 0 && materialresult.Data != null)
                        {
                            detailmodel.LotQuantity = materialresult.Data.MainProductQtyPerLot;     //单批次产品数量
                            detailmodel.MaterialCode = materialresult.Data.Key;                     //产品物料代码
                        }
                        else
                        {
                            sMsg = string.Format("工单:({0})产品{1}属性提取异常！", model.OrderNumber.Trim(), result.Data.MaterialCode);

                            return Content("<script >alert('" + sMsg + "');</script >", "text/html");
                        }
                    }
                    #endregion
                }

                #region 生成批次号
                IList<string> lstLot = new List<string>();

                using (LotCreateServiceClient client = new LotCreateServiceClient())
                {
                    MethodReturnResult<IList<string>> result = client.Generate(0, model.OrderNumber.Trim(), model.Count, model.LineCode);
                    if (result.Code <= 0 && result.Data != null && result.Data.Count > 0)
                    {
                        lstLot = result.Data;
                    }
                    else
                    {
                        sMsg = result.Message;

                        return Content("<script >alert('" + sMsg + "');</script >", "text/html");
                    }
                }

                ViewBag.LotList = lstLot;

                //获取需要录入的批次号自定义特性
                IList<BaseAttribute> lstAttribute = new List<BaseAttribute>();
                using (BaseAttributeServiceClient client = new BaseAttributeServiceClient())
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format("Key.CategoryName='{0}'", "LotCreateAttribute")
                    };

                    MethodReturnResult<IList<BaseAttribute>> result = client.Get(ref cfg);
                    if (result.Code <= 0 && result.Data != null && result.Data.Count > 0)
                    {
                        lstAttribute = result.Data;
                    }
                }

                ViewBag.AttributeList = lstAttribute;
                #endregion

                //清除原有Model数据便于装载新数据
                ModelState.Clear();
            }
            catch (Exception e)
            {
                return Content("<script >alert('" + e.Message + "');</script >", "text/html");
            }

            return View(detailmodel);
        }

        //
        // POST: /WIP/LotCreate/Detail
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(LotCreateMainViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();

            try
            {
                if (ModelState.IsValid)
                {
                    CreateParameter p = new CreateParameter()
                    {                        
                        OrderNumber = model.OrderNumber.Trim(),     //工单号
                        LotQuantity = model.LotQuantity,            //单批次产品数量
                        Remark = model.Description,                 //备注
                        LineCode=model.LineCode,                    //线别代码
                        LotNumbers = new List<string>(),            //批次列表
                        Creator = User.Identity.Name,               //创建人
                        OperateComputer = Request.UserHostAddress,  //操作电脑
                        Operator = User.Identity.Name,              //操作人
                    };

                    //批次分隔符
                    char splitChar = ',';

                    //获取批次号
                    string[] lotNumbers = Request["LotNumber"].ToUpper().Split(splitChar);

                    p.LotNumbers = lotNumbers.ToList();

                    #region 自定义属性???
                    IList<BaseAttribute> lstAttribute = new List<BaseAttribute>();
                    using (BaseAttributeServiceClient client = new BaseAttributeServiceClient())
                    {
                        PagingConfig cfg = new PagingConfig()
                        {
                            IsPaging = false,
                            Where = string.Format("Key.CategoryName='{0}'", "LotCreateAttribute")
                        };

                        MethodReturnResult<IList<BaseAttribute>> rst = client.Get(ref cfg);
                        if (rst.Code <= 0 && rst.Data != null && rst.Data.Count > 0)
                        {
                            lstAttribute = rst.Data;
                        }
                    }

                    p.Attributes = new Dictionary<string, IList<TransactionParameter>>();
                    foreach (BaseAttribute attr in lstAttribute)
                    {
                        string vals = Request["ATTR_"+attr.Key.AttributeName];
                        if(string.IsNullOrEmpty(vals))
                        {
                            continue;
                        }

                        string[] attrValues = vals.Split(splitChar);
                        for(int i=0;i<p.LotNumbers.Count && i<attrValues.Length;i++)
                        {
                            string lotNumber=p.LotNumbers[i];
                            string tpVal = attrValues[i];
                            //如果没有设置值，则不进行数据存储。
                            if(string.IsNullOrEmpty(tpVal))
                            {
                                continue;
                            }
                            if (!p.Attributes.ContainsKey(lotNumber))
                            {
                                p.Attributes.Add(lotNumber, new List<TransactionParameter>());
                            }
                            if(attr.DataType==EnumDataType.Boolean)
                            {
                                tpVal = tpVal == "on" ? "true" : "false";
                            }
                            TransactionParameter tp = new TransactionParameter()
                            {
                                Index = attr.Order,
                                Name = attr.Key.AttributeName,
                                Value = tpVal
                            };
                            p.Attributes[lotNumber].Add(tp);
                        }
                    }
                    #endregion

                    //创建批次。
                    using (LotCreateServiceClient client = new LotCreateServiceClient())
                    {
                        result = client.Create(p);
                    }

                    //标签打印。
                    //if (result.Code == 0)
                    //{
                    //    result = PrintPrivate(model);
                    //}

                    if(result.Code==0)
                    {
                        result.Message = "保存成功。";
                        result.Detail = result.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }

            return Json(result);
        }

        //
        // POST: /WIP/LotCreate/Print
        [HttpPost]
        public ActionResult Print(LotCreateDetailViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                result = PrintPrivate(model);
                if (result.Code == 0)
                {
                    result.Message = "打印标签成功。";
                }
                return Json(result);
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

        //private MethodReturnResult PrintPrivate(LotCreateDetailViewModel model)
        private MethodReturnResult PrintPrivate(LotCreateMainViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();
            result.Code = 0;

            try
            {
                //不需要进行标签打印。
                if (model.PrintQty <= 0
                    || string.IsNullOrEmpty(model.PrinterName)
                    || string.IsNullOrEmpty(model.PrintLabelCode))
                {
                    return result;
                }

                char splitChar = ',';

                //获取批次号值。
                string[] lotNumbers = Request["LotNumber"].ToUpper().Split(splitChar);

                //获取打印机名称
                ClientConfig printer = null;
                using (ClientConfigServiceClient client = new ClientConfigServiceClient())
                {
                    MethodReturnResult<ClientConfig> rst = client.Get(model.PrinterName);
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
                    MethodReturnResult<PrintLabel> rst = client.Get(model.PrintLabelCode);
                    if (rst.Code > 0)
                    {
                        return rst;
                    }
                    label = rst.Data;
                }

                DateTime PrintStart = DateTime.Now;
                
                //根据打印数量设置打印机模板。
                using (IPrintHelper helper = PrintHelperFactory.CreatePrintHelper(label.Content))
                {
                    foreach (string lotNumber in lotNumbers)
                    {
                        PrintStart = DateTime.Now;          //打印开始时间

                        //打印动态内容。
                        dynamic printData = new ExpandoObject();
                        printData.LotNumber = lotNumber;
                        printData.PrintQty = model.PrintQty;
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
                                LotNumber = lotNumber,                  //批次号
                                ClientName = Request.UserHostAddress,   //客户端
                                PrintQty = model.PrintQty,              //打印数量 
                                PrintLabelCode = model.PrintLabelCode,  //打印标签代码
                                PrinterName = model.PrinterName,        //打印机名称
                                PrintType = printer.ClientType.GetDisplayName(),    //打印机类型
                                IsSucceed = true,                       //打印是否成功
                                PrintData = "",                         //打印数据
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
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }

            return result;
        }

        public ActionResult GetOrderNumbers(string lineStoreName)
        {
            IList<WorkOrder> lstWorkOrder = new List<WorkOrder>();
            using (WorkOrderServiceClient client = new WorkOrderServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"CloseType='{0}'
                                           AND LocationName=(SELECT p.LocationName 
                                                             FROM LineStore as p
                                                             WHERE p.Key='{1}')"
                                           , Convert.ToInt32(EnumCloseType.None)
                                           , lineStoreName)
                };

                MethodReturnResult<IList<WorkOrder>> result = client.Get(ref cfg);
                if (result.Code <= 0)
                {
                    lstWorkOrder = result.Data;
                }
            }
            return Json(from item in lstWorkOrder
                        select new
                        {
                            Text = item.Key,
                            Value = item.Key
                        }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProductionLines(string lineStoreName)
        {
            IList<ProductionLine> lst = new List<ProductionLine>();
            //获取车间名称
            string locationName = string.Empty;
            using (LineStoreServiceClient client = new LineStoreServiceClient())
            {
                MethodReturnResult<LineStore> result = client.Get(lineStoreName);
                if (result.Code <= 0 && result.Data!=null)
                {
                    locationName = result.Data.LocationName;
                }
            }

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
                                           , Convert.ToInt32(LocationLevel.Area))
                };

                MethodReturnResult<IList<ProductionLine>> result = client.Get(ref cfg);
                if (result.Code <= 0)
                {
                    lst = result.Data;
                }
            }
            return Json(from item in lst
                        select new
                        {
                            Text = item.Key,
                            Value = item.Key
                        }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMaterialCodes(string orderNumber, string lineStoreName)
        {
            //根据物料类型获取物料。
            IList<LineStoreMaterialDetail> lstLineStoreMaterial = new List<LineStoreMaterialDetail>();
            using (LineStoreMaterialServiceClient client = new LineStoreMaterialServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"Key.LineStoreName ='{0}'
                                            AND Key.OrderNumber='{1}'
                                            AND CurrentQty>0
                                            AND (Key.MaterialCode LIKE '11%' OR Key.MaterialCode LIKE '1301%'  OR Key.MaterialCode LIKE '1803%'  OR Key.MaterialCode LIKE '251102%')"  //电池片或硅片
                                            , lineStoreName
                                            , orderNumber)
                };
                MethodReturnResult<IList<LineStoreMaterialDetail>> result = client.GetDetail(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstLineStoreMaterial = result.Data;
                }
            }

            var lnq = from item in lstLineStoreMaterial
                      select item.Key.MaterialCode;
            return Json(lnq.Distinct(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMaterialLots(string orderNumber, string lineStoreName, string materialCode)
        {
            //根据物料类型获取物料。
            IList<LineStoreMaterialDetail> lstLineStoreMaterial = new List<LineStoreMaterialDetail>();
            using (LineStoreMaterialServiceClient client = new LineStoreMaterialServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"Key.LineStoreName ='{0}' 
                                            AND Key.MaterialCode='{1}'
                                            AND Key.OrderNumber='{2}'
                                            AND CurrentQty>0"
                                            , lineStoreName
                                            , materialCode
                                            , orderNumber)
                };
                MethodReturnResult<IList<LineStoreMaterialDetail>> result = client.GetDetail(ref cfg);
                if (result.Code <= 0 && result.Data != null)
                {
                    lstLineStoreMaterial = result.Data;
                }
            }

            var lnq = from item in lstLineStoreMaterial
                      select new
                      {
                          Text = string.Format("{0}[{1}][{2}]", item.Key.MaterialLot, item.CurrentQty, item.Attr1),
                          Value = item.Key.MaterialLot
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
                if (result.Code <= 0 && result.Data!=null)
                {
                    lstRouteEnterpriseDetail = result.Data;
                }
            }
            return Json(from item in lstRouteEnterpriseDetail
                        select new { 
                            Text=item.Key.RouteName,
                            Value=item.Key.RouteName
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
                if (result.Code <= 0 && result.Data!=null)
                {
                    lst=result.Data;
                }
            }
            return Json(from item in lst
                         select new { 
                            Text=item.Key.RouteStepName,
                            Value=item.Key.RouteStepName
                        }, JsonRequestBehavior.AllowGet);
        }

        /// <summary> 取得车间线别清单 </summary>
        /// <param name="LocationName">车间代码</param>
        /// <returns></returns>
        public ActionResult GetLocationLines(string LocationName)
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
                                           , LocationName
                                           , Convert.ToInt32(LocationLevel.Area))
                };

                MethodReturnResult<IList<ProductionLine>> result = client.Get(ref cfg);
                if (result.Code <= 0)
                {
                    lst = result.Data;
                }
            }

            return Json(from item in lst
                        select new
                        {
                            Text = item.Key,
                            Value = item.Key
                        }, JsonRequestBehavior.AllowGet);
        }
	}
}

//using ServiceCenter.Client.Mvc.Areas.WIP.Models;
//using ServiceCenter.Common.Print;
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
//using ServiceCenter.Model;
//using System;
//using System.Collections.Generic;
//using System.Dynamic;
//using System.Linq;
//using System.Text;
//using System.Web;
//using System.Web.Mvc;

//namespace ServiceCenter.Client.Mvc.Areas.WIP.Controllers
//{
//    public class LotCreateController : Controller
//    {
//        //
//        // GET: /WIP/LotCreate/
//        public ActionResult Index()
//        {
//            return View(new LotCreateMainViewModel());
//        }

//        //
//        // POST: /WIP/LotCreate/Detail
//        public ActionResult Detail(LotCreateMainViewModel model)
//        {
//            string sMsg = "";

//            //LotCreateDetailViewModel viewModel = new LotCreateDetailViewModel()
//            //{
//            //    LineStoreName = model.LineStoreName,
//            //    //MaterialLot=model.MaterialLot,
//            //    //MaterialCode=model.MaterialCode,
//            //    //LotType=model.LotType,
//            //    OrderNumber=model.OrderNumber,          //工单号
//            //    Count=model.Count,                      //建批数量
//            //    Description=string.Empty,               //描述
//            //    MaterialQty=0,                          //单批次物料消耗数据量
//            //    ProductCode=string.Empty,               //产品代码
//            //    Quantity=0,
//            //    RawQuantity=0,
//            //    RouteEnterpriseName=string.Empty,
//            //    RouteName=string.Empty,
//            //    RouteStepName=string.Empty,
//            //    SupplierCode=string.Empty
//            //};

//            //if (string.IsNullOrEmpty(model.MaterialLot))
//            //{
//            //    return View(viewModel);
//            //}

//            //获取工单信息。
//            using(WorkOrderServiceClient client = new WorkOrderServiceClient())
//            {
//                MethodReturnResult<WorkOrder> result = client.Get(model.OrderNumber);
//                if ( result.Code > 0 )
//                {                    
//                    return Json(result);
//                }

//                if(result.Data == null)
//                {
//                    sMsg = string.Format("工单:({0})不存在，请重新录入！", model.OrderNumber);

//                    return Content("<script >alert('" + sMsg + "');</script >", "text/html");
//                }

//                //判断工单与车间是否一致
//                if (model.LocationName != result.Data.LocationName)
//                {
//                    sMsg = string.Format("工单:({0})为{1}车间工单，请重新选择！", model.OrderNumber, result.Data.LocationName);

//                    return Content("<script >alert('" + sMsg + "');</script >", "text/html");
//                }

//                //根据工单代码取得工单类型（正常、实验、返工）
//                //switch (result.Data.OrderType)
//                //switch (model.OrderNumber.Substring(0,3))
//                //{
//                //    case "1MO":     //正常工单
//                //        model.LotType = EnumLotType.Normal;

//                //        break;
//                //    case "2MO":     //返工工单
//                //        model.LotType = EnumLotType.Rework;

//                //        break;
//                //    case "3MO":     //实验工单
//                //        model.LotType = EnumLotType.Experiment;

//                //        break;
//                //    default:
//                //        result.Code = 1000;
//                //        result.Message = "工单号类型错误！";
//                //        result.Detail = result.Message;

//                //        return Json(result);
//                //        //break;
//                //}   

//                #region 产品信息（取得单批次产品数量）
//                using (MaterialServiceClient materialclient = new MaterialServiceClient())
//                {
//                    MethodReturnResult<Material> materialresult = materialclient.Get(result.Data.MaterialCode);
//                    if (materialresult.Code <= 0 && materialresult.Data != null)
//                    {
//                        //单批次产品数量
//                        model.Quantity = materialresult.Data.MainProductQtyPerLot;
//                    }
//                    else
//                    {
//                        sMsg = string.Format("工单:({0})产品{1}属性提取异常！", model.OrderNumber, result.Data.MaterialCode);

//                        return Content("<script >alert('" + sMsg + "');</script >", "text/html");
//                    }
//                }
//                #endregion
//            }

//            #region 获取工单工艺信息
//            //using (WorkOrderRouteServiceClient client = new WorkOrderRouteServiceClient())
//            //{
//            //    PagingConfig cfg = new PagingConfig()
//            //    {
//            //        PageNo = 0,
//            //        PageSize = 1,
//            //        Where = string.Format("Key.OrderNumber='{0}'", model.OrderNumber),
//            //        OrderBy = "Key.ItemNo"
//            //    };

//            //    //工单批次类型（正常、返工、实验）
//            //    //if (model.LotType == EnumLotType.Rework)
//            //    //{
//            //    //    cfg.Where += " AND IsRework=1";
//            //    //}
//            //    //else
//            //    //{
//            //    //    cfg.Where += " AND IsRework=0";
//            //    //}

//            //    MethodReturnResult<IList<WorkOrderRoute>> result = client.Get(ref cfg);
//            //    if (result.Code <= 0 && result.Data != null && result.Data.Count>0)
//            //    {
//            //        viewModel.RouteEnterpriseName = result.Data[0].RouteEnterpriseName;
//            //        viewModel.RouteName = result.Data[0].RouteName;
//            //        viewModel.RouteStepName = result.Data[0].RouteStepName;
//            //    }
//            //}
//            #endregion

//            #region 获取线边仓物料信息
//            //using (LineStoreMaterialServiceClient client = new LineStoreMaterialServiceClient())
//            //{
//            //    LineStoreMaterialDetailKey key = new LineStoreMaterialDetailKey()
//            //    {
//            //         LineStoreName=model.LineStoreName,
//            //         OrderNumber=model.OrderNumber,
//            //         MaterialCode=model.MaterialCode,
//            //         MaterialLot=model.MaterialLot
//            //    };
//            //    MethodReturnResult<LineStoreMaterialDetail> result = client.GetDetail(key);
//            //    if (result.Code <= 0 && result.Data != null)
//            //    {
//            //        viewModel.MaterialQty = result.Data.CurrentQty;
//            //        viewModel.SupplierCode = result.Data.SupplierCode;
//            //    }
//            //}
//            ////根据物料获取每批原材料建议数量和每批产品建议数量。
//            //using(MaterialServiceClient client=new MaterialServiceClient())
//            //{
//            //    MethodReturnResult<Material> result = client.Get(viewModel.ProductCode);
//            //    if (result.Code <= 0 && result.Data != null)
//            //    {
//            //        viewModel.RawQuantity = result.Data.MainRawQtyPerLot;
//            //        viewModel.Quantity = result.Data.MainProductQtyPerLot;
//            //    }
//            //}
//            #endregion

//            #region 根据工单号和批次个数生成批次号。
//            IList<string> lstLot = new List<string>();

//            using(LotCreateServiceClient client=new LotCreateServiceClient())
//            {
//                MethodReturnResult<IList<string>> result = client.Generate(0, model.OrderNumber, model.Count, model.LineCode);
//                if (result.Code <= 0 && result.Data != null && result.Data.Count > 0)
//                {
//                    lstLot = result.Data;
//                }
//                else
//                {
//                    sMsg = result.Message;

//                    return Content("<script >alert('" + sMsg + "');</script >", "text/html");
//                }
//            }

//            ViewBag.LotList = lstLot;

//            //获取需要录入的批次号自定义特性
//            IList<BaseAttribute> lstAttribute = new List<BaseAttribute>();
//            using (BaseAttributeServiceClient client = new BaseAttributeServiceClient())
//            {
//                PagingConfig cfg = new PagingConfig()
//                {
//                    IsPaging = false,
//                    Where = string.Format("Key.CategoryName='{0}'", "LotCreateAttribute")
//                };

//                MethodReturnResult<IList<BaseAttribute>> result = client.Get(ref cfg);
//                if (result.Code <= 0 && result.Data != null && result.Data.Count > 0)
//                {
//                    lstAttribute = result.Data;
//                }
//            }

//            ViewBag.AttributeList = lstAttribute;
//            #endregion

//            return View(model);
//        }

//        //
//        // POST: /WIP/LotCreate/Detail
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Save(LotCreateMainViewModel model)
//        {
//            MethodReturnResult result = new MethodReturnResult();

//            try
//            {
//                if (ModelState.IsValid)
//                {
//                    CreateParameter p = new CreateParameter()
//                    {
//                        Creator = User.Identity.Name,
//                        //LineStoreName = model.LineStoreName,
//                        //LotType = model.LotType,
//                        OperateComputer = Request.UserHostAddress,
//                        Operator = User.Identity.Name,
//                        OrderNumber = model.OrderNumber,
//                        //Quantity = model.Quantity,
//                        //RawMaterialCode = model.MaterialCode,
//                        //RawMaterialLot = model.MaterialLot,
//                        //RawQuantity = model.RawQuantity,
//                        Remark = model.Description,
//                        //RouteEnterpriseName = model.RouteEnterpriseName,
//                        //RouteName = model.RouteName,
//                        //RouteStepName = model.RouteStepName,
//                        LineCode=model.LineCode,
//                        LotNumbers = new List<string>()
//                    };

//                    char splitChar = ',';

//                    //获取批次号值。
//                    string[] lotNumbers = Request["LotNumber"].ToUpper().Split(splitChar);

//                    p.LotNumbers = lotNumbers.ToList();

//                    #region 自定义属性???
//                    IList<BaseAttribute> lstAttribute = new List<BaseAttribute>();
//                    using (BaseAttributeServiceClient client = new BaseAttributeServiceClient())
//                    {
//                        PagingConfig cfg = new PagingConfig()
//                        {
//                            IsPaging = false,
//                            Where = string.Format("Key.CategoryName='{0}'", "LotCreateAttribute")
//                        };

//                        MethodReturnResult<IList<BaseAttribute>> rst = client.Get(ref cfg);
//                        if (rst.Code <= 0 && rst.Data != null && rst.Data.Count > 0)
//                        {
//                            lstAttribute = rst.Data;
//                        }
//                    }

//                    p.Attributes = new Dictionary<string, IList<TransactionParameter>>();
//                    foreach (BaseAttribute attr in lstAttribute)
//                    {
//                        string vals = Request["ATTR_"+attr.Key.AttributeName];
//                        if(string.IsNullOrEmpty(vals))
//                        {
//                            continue;
//                        }

//                        string[] attrValues = vals.Split(splitChar);
//                        for(int i=0;i<p.LotNumbers.Count && i<attrValues.Length;i++)
//                        {
//                            string lotNumber=p.LotNumbers[i];
//                            string tpVal = attrValues[i];
//                            //如果没有设置值，则不进行数据存储。
//                            if(string.IsNullOrEmpty(tpVal))
//                            {
//                                continue;
//                            }
//                            if (!p.Attributes.ContainsKey(lotNumber))
//                            {
//                                p.Attributes.Add(lotNumber, new List<TransactionParameter>());
//                            }
//                            if(attr.DataType==EnumDataType.Boolean)
//                            {
//                                tpVal = tpVal == "on" ? "true" : "false";
//                            }
//                            TransactionParameter tp = new TransactionParameter()
//                            {
//                                Index = attr.Order,
//                                Name = attr.Key.AttributeName,
//                                Value = tpVal
//                            };
//                            p.Attributes[lotNumber].Add(tp);
//                        }
//                    }
//                    #endregion

//                    //创建批次。
//                    using (LotCreateServiceClient client = new LotCreateServiceClient())
//                    {
//                        result = client.Create(p);
//                    }

//                    //标签打印。
//                    if (result.Code == 0)
//                    {
//                        result = PrintPrivate(model);
//                    }

//                    if(result.Code==0)
//                    {
//                        result.Message = "保存成功。";
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                result.Code = 1000;
//                result.Message = ex.Message;
//                result.Detail = ex.ToString();
//            }

//            return Json(result);
//        }

//        //
//        // POST: /WIP/LotCreate/Print
//        [HttpPost]
//        public ActionResult Print(LotCreateDetailViewModel model)
//        {
//            MethodReturnResult result = new MethodReturnResult();
//            try
//            {
//                result = PrintPrivate(model);
//                if (result.Code == 0)
//                {
//                    result.Message = "打印标签成功。";
//                }
//                return Json(result);
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

//        //private MethodReturnResult PrintPrivate(LotCreateDetailViewModel model)
//        private MethodReturnResult PrintPrivate(LotCreateMainViewModel model)
//        {
//            MethodReturnResult result = new MethodReturnResult();
//            //不需要进行标签打印。
//            if (model.PrintQty <= 0
//                || string.IsNullOrEmpty(model.PrinterName)
//                || string.IsNullOrEmpty(model.PrintLabelCode))
//            {
//                return result;
//            }

//            char splitChar = ',';
//            //获取批次号值。
//            string[] lotNumbers = Request["LotNumber"].ToUpper().Split(splitChar);

//            //获取打印机名称
//            ClientConfig printer = null;
//            using (ClientConfigServiceClient client = new ClientConfigServiceClient())
//            {
//                MethodReturnResult<ClientConfig> rst = client.Get(model.PrinterName);
//                if (rst.Code > 0)
//                {
//                    return rst;
//                }
//                printer = rst.Data;
//            }
//            //获取打印条码内容
//            PrintLabel label = null;
//            using (PrintLabelServiceClient client = new PrintLabelServiceClient())
//            {
//                MethodReturnResult<PrintLabel> rst = client.Get(model.PrintLabelCode);
//                if (rst.Code > 0)
//                {
//                    return rst;
//                }
//                label = rst.Data;
//            }
//            //根据打印数量设置打印机模板。
//            using (IPrintHelper helper = PrintHelperFactory.CreatePrintHelper(label.Content))
//            {
//                foreach (string lotNumber in lotNumbers)
//                {
//                    //打印动态内容。
//                    dynamic d = new ExpandoObject();
//                    d.LotNumber = lotNumber;
//                    d.PrintQty = model.PrintQty;
//                    bool bSuccess = false;
//                    //根据打印机类型，调用不同的打印方法。
//                    if (printer.ClientType == EnumClientType.NetworkPrinter)
//                    {
//                        string[] vals = printer.IPAddress.Split(':');
//                        string port = "9100";
//                        if (vals.Length > 1)
//                        {
//                            port = vals[1];
//                        }
//                        bSuccess = helper.NetworkPrint(vals[0], port, label.Content, d);
//                    }
//                    else if (printer.ClientType == EnumClientType.RawPrinter)
//                    {
//                        bSuccess = helper.RAWPrint(printer.IPAddress, label.Content, d);
//                    }
//                    else
//                    {
//                        result.Code = 1001;
//                        result.Message = "打印失败,打印机类型不正确。";
//                        return result;
//                    }
//                    //返回打印结果。
//                    if (bSuccess == false)
//                    {
//                        result.Code = 1001;
//                        result.Message = "批次标签打印失败。";
//                        return result;
//                    }
//                }
//            }
//            return result;
//        }

//        public ActionResult GetOrderNumbers(string lineStoreName)
//        {
//            IList<WorkOrder> lstWorkOrder = new List<WorkOrder>();
//            using (WorkOrderServiceClient client = new WorkOrderServiceClient())
//            {
//                PagingConfig cfg = new PagingConfig()
//                {
//                    IsPaging = false,
//                    Where = string.Format(@"CloseType='{0}'
//                                           AND LocationName=(SELECT p.LocationName 
//                                                             FROM LineStore as p
//                                                             WHERE p.Key='{1}')"
//                                           , Convert.ToInt32(EnumCloseType.None)
//                                           , lineStoreName)
//                };

//                MethodReturnResult<IList<WorkOrder>> result = client.Get(ref cfg);
//                if (result.Code <= 0)
//                {
//                    lstWorkOrder = result.Data;
//                }
//            }
//            return Json(from item in lstWorkOrder
//                        select new
//                        {
//                            Text = item.Key,
//                            Value = item.Key
//                        }, JsonRequestBehavior.AllowGet);
//        }

//        public ActionResult GetProductionLines(string lineStoreName)
//        {
//            IList<ProductionLine> lst = new List<ProductionLine>();
//            //获取车间名称
//            string locationName = string.Empty;
//            using (LineStoreServiceClient client = new LineStoreServiceClient())
//            {
//                MethodReturnResult<LineStore> result = client.Get(lineStoreName);
//                if (result.Code <= 0 && result.Data!=null)
//                {
//                    locationName = result.Data.LocationName;
//                }
//            }

//            using (ProductionLineServiceClient client = new ProductionLineServiceClient())
//            {
//                PagingConfig cfg = new PagingConfig()
//                {
//                    IsPaging = false,
//                    Where = string.Format(@"EXISTS (FROM Location as p
//                                                    WHERE p.Key=self.LocationName
//                                                    AND p.ParentLocationName='{0}'
//                                                    AND p.Level='{1}')"
//                                           , locationName
//                                           , Convert.ToInt32(LocationLevel.Area))
//                };

//                MethodReturnResult<IList<ProductionLine>> result = client.Get(ref cfg);
//                if (result.Code <= 0)
//                {
//                    lst = result.Data;
//                }
//            }
//            return Json(from item in lst
//                        select new
//                        {
//                            Text = item.Key,
//                            Value = item.Key
//                        }, JsonRequestBehavior.AllowGet);
//        }

//        public ActionResult GetMaterialCodes(string orderNumber, string lineStoreName)
//        {
//            //根据物料类型获取物料。
//            IList<LineStoreMaterialDetail> lstLineStoreMaterial = new List<LineStoreMaterialDetail>();
//            using (LineStoreMaterialServiceClient client = new LineStoreMaterialServiceClient())
//            {
//                PagingConfig cfg = new PagingConfig()
//                {
//                    IsPaging = false,
//                    Where = string.Format(@"Key.LineStoreName ='{0}'
//                                            AND Key.OrderNumber='{1}'
//                                            AND CurrentQty>0
//                                            AND (Key.MaterialCode LIKE '11%' OR Key.MaterialCode LIKE '1301%')"  //电池片或硅片
//                                            , lineStoreName
//                                            , orderNumber)
//                };
//                MethodReturnResult<IList<LineStoreMaterialDetail>> result = client.GetDetail(ref cfg);
//                if (result.Code <= 0 && result.Data != null)
//                {
//                    lstLineStoreMaterial = result.Data;
//                }
//            }

//            var lnq = from item in lstLineStoreMaterial
//                      select item.Key.MaterialCode;
//            return Json(lnq.Distinct(), JsonRequestBehavior.AllowGet);
//        }

//        public ActionResult GetMaterialLots(string orderNumber, string lineStoreName, string materialCode)
//        {
//            //根据物料类型获取物料。
//            IList<LineStoreMaterialDetail> lstLineStoreMaterial = new List<LineStoreMaterialDetail>();
//            using (LineStoreMaterialServiceClient client = new LineStoreMaterialServiceClient())
//            {
//                PagingConfig cfg = new PagingConfig()
//                {
//                    IsPaging = false,
//                    Where = string.Format(@"Key.LineStoreName ='{0}' 
//                                            AND Key.MaterialCode='{1}'
//                                            AND Key.OrderNumber='{2}'
//                                            AND CurrentQty>0"
//                                            , lineStoreName
//                                            , materialCode
//                                            , orderNumber)
//                };
//                MethodReturnResult<IList<LineStoreMaterialDetail>> result = client.GetDetail(ref cfg);
//                if (result.Code <= 0 && result.Data != null)
//                {
//                    lstLineStoreMaterial = result.Data;
//                }
//            }

//            var lnq = from item in lstLineStoreMaterial
//                      select new
//                      {
//                          Text = string.Format("{0}[{1}][{2}]", item.Key.MaterialLot, item.CurrentQty, item.Attr1),
//                          Value = item.Key.MaterialLot
//                      };
//            return Json(lnq, JsonRequestBehavior.AllowGet);
//        }

//        public ActionResult GetRouteNames(string routeEnterpriseName)
//        {
//            IList<RouteEnterpriseDetail> lstRouteEnterpriseDetail = new List<RouteEnterpriseDetail>();
//            using (RouteEnterpriseDetailServiceClient client = new RouteEnterpriseDetailServiceClient())
//            {
//                PagingConfig cfg = new PagingConfig()
//                {
//                    IsPaging = false,
//                    Where = string.Format("Key.RouteEnterpriseName='{0}'", routeEnterpriseName),
//                    OrderBy = "ItemNo"
//                };
//                MethodReturnResult<IList<RouteEnterpriseDetail>> result = client.Get(ref cfg);
//                if (result.Code <= 0 && result.Data!=null)
//                {
//                    lstRouteEnterpriseDetail = result.Data;
//                }
//            }
//            return Json(from item in lstRouteEnterpriseDetail
//                        select new { 
//                            Text=item.Key.RouteName,
//                            Value=item.Key.RouteName
//                        }, JsonRequestBehavior.AllowGet);
//        }

//        public ActionResult GetRouteStepNames(string routeName)
//        {
//            IList<RouteStep> lst = new List<RouteStep>();

//            using (RouteStepServiceClient client = new RouteStepServiceClient())
//            {
//                PagingConfig cfg = new PagingConfig()
//                {
//                    IsPaging = false,
//                    Where = string.Format("Key.RouteName='{0}'", routeName),
//                    OrderBy = "SortSeq"
//                };
//                MethodReturnResult<IList<RouteStep>> result = client.Get(ref cfg);
//                if (result.Code <= 0 && result.Data!=null)
//                {
//                    lst=result.Data;
//                }
//            }
//            return Json(from item in lst
//                         select new { 
//                            Text=item.Key.RouteStepName,
//                            Value=item.Key.RouteStepName
//                        }, JsonRequestBehavior.AllowGet);
//        }

//        /// <summary> 取得车间线别清单 </summary>
//        /// <param name="LocationName">车间代码</param>
//        /// <returns></returns>
//        public ActionResult GetLocationLines(string LocationName)
//        {
//            IList<ProductionLine> lst = new List<ProductionLine>();
            
//            using (ProductionLineServiceClient client = new ProductionLineServiceClient())
//            {
//                PagingConfig cfg = new PagingConfig()
//                {
//                    IsPaging = false,
//                    Where = string.Format(@"EXISTS (FROM Location as p
//                                                    WHERE p.Key=self.LocationName
//                                                    AND p.ParentLocationName='{0}'
//                                                    AND p.Level='{1}')"
//                                           , LocationName
//                                           , Convert.ToInt32(LocationLevel.Area))
//                };

//                MethodReturnResult<IList<ProductionLine>> result = client.Get(ref cfg);
//                if (result.Code <= 0)
//                {
//                    lst = result.Data;
//                }
//            }

//            return Json(from item in lst
//                        select new
//                        {
//                            Text = item.Key,
//                            Value = item.Key
//                        }, JsonRequestBehavior.AllowGet);
//        }
//    }
//}