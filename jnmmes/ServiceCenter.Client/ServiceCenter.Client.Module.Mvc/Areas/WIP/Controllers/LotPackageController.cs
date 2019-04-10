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
using System.Text;

namespace ServiceCenter.Client.Mvc.Areas.WIP.Controllers
{
    public class LotPackageController : Controller
    {
        //
        // GET: /WIP/LotPackage/
        /// <summary>
        /// 显示包装作业界面。
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View(new LotPackageViewModel());
        }

        //
        //POST: /WIP/LotPackage/Query
        [HttpPost]
        public ActionResult Query(string packageNo)
        {
            
            //获取需要录入的批次号自定义特性
            IList<BaseAttribute> lstAttribute = new List<BaseAttribute>();
            using (BaseAttributeServiceClient client = new BaseAttributeServiceClient())
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("Key.CategoryName='{0}'", "LotPackageAttribute")
                };

                MethodReturnResult<IList<BaseAttribute>> result = client.Get(ref cfg);
                if (result.Code <= 0 && result.Data != null && result.Data.Count > 0)
                {
                    lstAttribute = result.Data;
                }
            }
            ViewBag.AttributeList = lstAttribute;

            if (!string.IsNullOrEmpty(packageNo))
            {
                using (PackageQueryServiceClient client = new PackageQueryServiceClient())
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        OrderBy = "ItemNo",
                        Where = string.Format(@"Key.PackageNo='{0}' 
                                                AND EXISTS(FROM Package as p
                                                           WHERE p.Key=self.Key.PackageNo
                                                           AND PackageState='{1}' 
                                                           AND PackageType='{2}')"
                                            , packageNo
                                            ,Convert.ToInt32(EnumPackageState.Packaging)
                                            ,Convert.ToInt32(EnumPackageType.Packet))
                    };
                    MethodReturnResult<IList<PackageDetail>> result = client.GetDetail(ref cfg);

                    if (result.Code == 0)
                    {
                        ViewBag.PackageDetailList = result.Data;
                    }
                }
            }
            return PartialView("_ListPartial", new LotViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(LotPackageViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                //获取批次数据。
                string lotNumber = model.LotNumber.ToUpper();
                result = GetLot(lotNumber);
                if (result.Code > 0)
                {
                    return Json(result);
                }
                MethodReturnResult<Lot> rst = result as MethodReturnResult<Lot>;
                Lot obj = rst.Data;

                //如果包装号为空。生成包装号。
                if(string.IsNullOrEmpty(model.PackageNo))
                {
                    using (LotPackageServiceClient client = new LotPackageServiceClient())
                    {
                        MethodReturnResult<string> rst1 = client.Generate(model.LotNumber, model.IsLastestPackage);
                        if (rst1.Code > 0)
                        {
                            return Json(rst1);
                        }
                        else
                        {
                            model.PackageNo = rst1.Data;
                        }
                    }
                }
                //重新获取当前数量。
                using (PackageQueryServiceClient client = new PackageQueryServiceClient())
                {
                    MethodReturnResult<Package> rst2 = client.Get(model.PackageNo);
                    if (rst2.Code == 1000)
                    {
                        return Json(rst2);
                    }
                    //检查包装状态
                    if (rst2.Data!=null && rst2.Data.PackageState != EnumPackageState.Packaging)
                    {
                        result.Code = 1001;
                        result.Message = string.Format("包 {0} 非 [{1}] 状态，不能包装。"
                                                        , model.PackageNo.ToUpper()
                                                        , EnumPackageState.Packaging.GetDisplayName());
                        return Json(result);
                    }
                    //设置当前数量。
                    if (rst2.Code <= 0 && rst2.Data != null)
                    {
                        model.CurrentQuantity = rst2.Data.Quantity;
                    }
                }
                //如果满包数量为空，获取满包数量
                if (model.FullQuantity == 0)
                {
                    using (WorkOrderAttributeServiceClient client = new WorkOrderAttributeServiceClient())
                    {
                        MethodReturnResult<WorkOrderAttribute> rst1 = client.Get(new WorkOrderAttributeKey()
                        {
                            OrderNumber = obj.OrderNumber,
                            AttributeName="FullPackageQuantity"
                        });
                        double fullQuantity = 25;
                        if (rst1.Code == 1000)
                        {
                            return Json(rst1);
                        }

                        if ( rst1.Data != null
                            && !string.IsNullOrEmpty(rst1.Data.AttributeValue))
                        {
                            double.TryParse(rst1.Data.AttributeValue, out fullQuantity);
                        }
                        model.FullQuantity = fullQuantity;
                    }
                }


                double newCurrentQuantity = model.CurrentQuantity + obj.Quantity;
                //当前数量超过满包数量，不能继续包装。
                if (newCurrentQuantity > model.FullQuantity)
                {
                    result.Code = 1;
                    result.Message = string.Format("包({0}) 当前数量({1})加上批次（{2}）数量（{3}），超过满包数量。"
                                                    , model.PackageNo.ToUpper()
                                                    , model.CurrentQuantity
                                                    , obj.Key
                                                    , obj.Quantity);
                    return Json(result);
                }
                model.CurrentQuantity = newCurrentQuantity;

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
                //判断批次所在车间和当前线所在车间是否匹配。
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
                                                    , lotNumber.ToUpper()
                                                    , obj.LocationName
                                                    , locationName);
                    return Json(result);
                }
                result = Package(model);
                //返回包装结果。
                if (result.Code <= 0)
                {
                    MethodReturnResult<LotPackageViewModel> rstFinal = new MethodReturnResult<LotPackageViewModel>()
                    {
                        Code = result.Code,
                        Data = model,
                        Detail = result.Detail,
                        HelpLink = result.HelpLink,
                        Message = result.Message
                    };
                    return Json(rstFinal);
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


        [HttpPost]
        public ActionResult Finish(LotPackageViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                //如果包装号为空。
                if (string.IsNullOrEmpty(model.PackageNo))
                {
                    result.Code = 1001;
                    result.Message = string.Format("包装号不能为空。");
                    return Json(result);
                }
                Package obj = null;
                //如果当前数量为空，获取当前数量
                using (PackageQueryServiceClient client = new PackageQueryServiceClient())
                {
                    MethodReturnResult<Package> rst2 = client.Get(model.PackageNo);
                    if (rst2.Code >0 )
                    {
                        return Json(rst2);
                    }
                    //检查包装状态
                    if (rst2.Data.PackageState != EnumPackageState.Packaging)
                    {
                        result.Code = 1001;
                        result.Message = string.Format("包 {0} 非{1}状态，不能操作。"
                                                        , model.PackageNo.ToUpper()
                                                        , EnumPackageState.Packaging.GetDisplayName());
                        return Json(result);
                    }
                    //设置当前数量。
                    if (rst2.Code <= 0 && rst2.Data != null)
                    {
                        obj = rst2.Data;
                        model.CurrentQuantity = rst2.Data.Quantity;
                    }
                }
                //如果满包数量为空，获取满包数量
                if (model.FullQuantity == 0)
                {
                    using (WorkOrderAttributeServiceClient client = new WorkOrderAttributeServiceClient())
                    {
                        MethodReturnResult<WorkOrderAttribute> rst1 = client.Get(new WorkOrderAttributeKey()
                        {
                            OrderNumber = obj.OrderNumber,
                            AttributeName = "FullPackageQuantity"
                        });
                        double fullQuantity = 25;
                        if (rst1.Code == 1000)
                        {
                            return Json(rst1);
                        }

                        if (rst1.Data != null
                            && !string.IsNullOrEmpty(rst1.Data.AttributeValue))
                        {
                            double.TryParse(rst1.Data.AttributeValue, out fullQuantity);
                        }
                        model.FullQuantity = fullQuantity;
                    }
                }
                //非尾包，不能完成包装并过站
                if (model.IsFinishPackage == true
                    && model.IsLastestPackage == false
                    && model.CurrentQuantity != model.FullQuantity)
                {
                    result.Code = 1;
                    result.Message = string.Format("包({0})非尾包，包装数量未达到满包数量，不能完成包装。"
                                                    , model.PackageNo);
                    return Json(result);
                }
                //判断批次所在车间和当前线所在车间是否匹配。
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
                //获取包装号所在车间。
                string currentLocationName = string.Empty;
                using (WorkOrderServiceClient client = new WorkOrderServiceClient())
                {
                    MethodReturnResult<WorkOrder> rst1 = client.Get(obj.OrderNumber);
                    if (rst1.Code <= 0 && rst1.Data != null)
                    {
                        currentLocationName = rst1.Data.LocationName;
                    }
                }
                //检查包所在车间和线别车间是否匹配。
                if (currentLocationName != locationName)
                {
                    result.Code = 3;
                    result.Message = string.Format("包（{0}）属于({1})车间，不能在({2})车间线别上操作。"
                                                    , obj.Key
                                                    , currentLocationName
                                                    , locationName);
                    return Json(result);
                }
                result = Package(model);
                //返回包装结果。
                if (result.Code <= 0)
                {
                    MethodReturnResult<LotPackageViewModel> rstFinal = new MethodReturnResult<LotPackageViewModel>()
                    {
                        Code = result.Code,
                        Data = model,
                        Detail = result.Detail,
                        HelpLink = result.HelpLink,
                        Message = result.Message
                    };
                    return Json(rstFinal);
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


        [HttpPost]
        public ActionResult TrackOutPackage(LotPackageViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                //如果包装号为空。
                if (string.IsNullOrEmpty(model.PackageNo))
                {
                    result.Code = 1001;
                    result.Message = string.Format("包装号不能为空。");
                    return Json(result);
                }
                Package obj = null;
                //如果当前数量为空，获取当前数量
                using (PackageQueryServiceClient client = new PackageQueryServiceClient())
                {
                    MethodReturnResult<Package> rst2 = client.Get(model.PackageNo);
                    if (rst2.Code > 0)
                    {
                        return Json(rst2);
                    }
                    //检查包装状态
                    if (rst2.Data.PackageState != EnumPackageState.Packaged)
                    {
                        result.Code = 1001;
                        result.Message = string.Format("包 {0} 非{1}状态，不能操作。"
                                                        , model.PackageNo.ToUpper()
                                                        , EnumPackageState.Packaged.GetDisplayName());
                        return Json(result);
                    }
                  
                }
             
                //result = Package(model);
                //返回包装结果。
                if (result.Code <= 0)
                {
                    MethodReturnResult<LotPackageViewModel> rstFinal = new MethodReturnResult<LotPackageViewModel>()
                    {
                        Code = result.Code,
                        Data = model,
                        Detail = result.Detail,
                        HelpLink = result.HelpLink,
                        Message = result.Message
                    };
                    return Json(rstFinal);
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
        

        [HttpPost]
        public ActionResult Delete(string packageNo,int itemNo,string lotNumber)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                //获取包装记录
                using (PackageQueryServiceClient client = new PackageQueryServiceClient())
                {
                    MethodReturnResult<Package> rst1=client.Get(packageNo);
                    if (rst1.Code > 0 || rst1.Data == null )
                    {
                        return Json(rst1);
                    }
                    //判断包装记录目前是否处于包装中状态。
                    if (rst1.Data.PackageState != EnumPackageState.Packaging)
                    {
                        result.Code = 1001;
                        result.Message = string.Format("包装{0}为非{1}状态，不能删除。"
                                                        , packageNo
                                                        , EnumPackageState.Packaging.GetDisplayName());
                        return Json(result);
                    }
                }
                //获取最后一笔包装记录。
                IList<string> lstTransactionKey = new List<string>();
                using (LotQueryServiceClient client = new LotQueryServiceClient())
                {
                    PagingConfig cfg = new PagingConfig()
                    {
                        IsPaging=false,
                        Where=string.Format("LotNumber='{0}' AND UndoFlag=0"
                                            ,lotNumber)
                    };

                    MethodReturnResult<IList<LotTransaction>> rst1 = client.GetTransaction(ref cfg);
                    if (rst1.Code <= 0 && rst1.Data!=null)
                    {
                        var lnq = from item in rst1.Data
                                  orderby item.CreateTime descending
                                  select item;
                        foreach (LotTransaction item in lnq)
                        {
                            lstTransactionKey.Add(item.Key);
                            if (item.Activity == EnumLotActivity.Package)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        return Json(rst1);
                    }
                }
                //可撤销操作主键不为空。
                if (lstTransactionKey.Count > 0)
                {
                    UndoParameter p = new UndoParameter()
                    {
                        Creator=User.Identity.Name,
                        LotNumbers=new List<string>(),
                        OperateComputer=Request.UserHostAddress,
                        Operator=User.Identity.Name,
                        UndoTransactionKeys=new Dictionary<string,IList<string>>()
                    };

                    p.LotNumbers.Add(lotNumber);
                    p.UndoTransactionKeys.Add(lotNumber, lstTransactionKey);

                    using (LotUndoServiceClient client = new LotUndoServiceClient())
                    {
                        result = client.Undo(p);
                        if (result.Code == 0)
                        {
                            result.Message = string.Format("删除 ({0}：{1}：{2}) 成功。"
                                                          ,packageNo,itemNo,lotNumber);
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


        [HttpPost]
        public ActionResult UnPackage(string packageNo, int itemNo, string lotNumber)
        {

            MethodReturnResult result = new MethodReturnResult();
            //进行批次包装作业。
            PackageParameter p = new PackageParameter()
            {
                Creator = User.Identity.Name,             
                LotNumbers = new List<string>(),
                OperateComputer = Request.UserHostAddress,
                Operator = User.Identity.Name,
                PackageNo = packageNo.ToUpper(),
            };
            p.LotNumbers.Add(lotNumber.ToUpper());

            try
            {
                //获取包装记录
                using (PackageQueryServiceClient client = new PackageQueryServiceClient())
                {
                    MethodReturnResult<Package> rst1 = client.Get(packageNo);
                    if (rst1.Code > 0 || rst1.Data == null)
                    {
                        return Json(rst1);
                    }
                    //判断包装记录目前是否处于包装中状态。
                    //if (rst1.Data.PackageState != EnumPackageState.Packaging)
                    //{
                    //    result.Code = 1001;
                    //    result.Message = string.Format("包装{0}为非{1}状态，不能删除。"
                    //                                    , packageNo
                    //                                    , EnumPackageState.Packaging.GetDisplayName());
                    //    return Json(result);
                    //}
                }

                using (LotPackageServiceClient client = new LotPackageServiceClient())
                {
                    result = client.UnPackage(p);

                    if (result.Code == 0 )
                    {
                        result.Message = string.Format("拆包（{1}）成功。"
                                                       , p.PackageNo);
                    }
                 
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
        /// <summary>
        /// 批次包装作业。
        /// </summary>
        /// <param name="obj">批次对象。</param>
        /// <param name="model">包装模型对象。</param>
        /// <returns>返回结果。</returns>
        private MethodReturnResult Package(LotPackageViewModel model)
        {
            MethodReturnResult result = new MethodReturnResult();
            //进行批次包装作业。
            PackageParameter p = new PackageParameter()
            {
                Creator=User.Identity.Name,
                EquipmentCode=model.EquipmentCode,
                IsFinishPackage = model.IsFinishPackage,
                IsLastestPackage=model.IsLastestPackage,
                LineCode=model.LineCode,
                LotNumbers=new List<string>(),
                OperateComputer=Request.UserHostAddress,
                Operator=User.Identity.Name,
                PackageNo=model.PackageNo.ToUpper(),
                Remark=model.Description,
                RouteOperationName = model.RouteOperationName
            };

            if (model.IsFinishPackage == false)
            {
                p.LotNumbers.Add(model.LotNumber.ToUpper());
            }

            if(p.IsFinishPackage==false
                && model.CurrentQuantity==model.FullQuantity)
            {
                p.IsFinishPackage = true;
                model.IsFinishPackage = true;
            }

            using (LotPackageServiceClient client = new LotPackageServiceClient())
            {
                result = client.Package(p);

                if (result.Code == 0 && model.IsFinishPackage==false)
                {
                    result.Message = string.Format("批次 {0} 成功包装到（{1}）。"
                                                   , model.LotNumber
                                                   , model.PackageNo);
                }
                else if(result.Code == 0 && model.IsFinishPackage==true)
                {
                    result.Message = string.Format("包 {0} 已完成。"
                                                    , model.PackageNo);
                }
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
            else if (obj.PackageFlag == true)
            {
                result.Code = 2005;
                result.Message = string.Format("批次({0})已包装到（{1}）。", lotNumber,obj.PackageNo);
                return result;
            }
            return rst;
        }
        /// <summary>
        /// 获取包装信息。
        /// </summary>
        /// <param name="packageNo"></param>
        /// <returns></returns>
        public ActionResult GetPackageInfo(string packageNo)
        {
            double currentQuantity = 0;
            double fullQuantity = 25;
            string orderNumber = string.Empty;
            bool isLastestPackage = false;

            if (!string.IsNullOrEmpty(packageNo))
            {
                //获取当前数量
                using (PackageQueryServiceClient client = new PackageQueryServiceClient())
                {
                    MethodReturnResult<Package> rst2 = client.Get(packageNo);
                    if (rst2.Code == 1000)
                    {
                        return Json(rst2);
                    }
                    if (rst2.Code <= 0 && rst2.Data != null && rst2.Data.PackageState==EnumPackageState.Packaging)
                    {
                        currentQuantity = rst2.Data.Quantity;
                        orderNumber = rst2.Data.OrderNumber;
                        isLastestPackage = rst2.Data.IsLastPackage;
                    }
                }

                if (!string.IsNullOrEmpty(orderNumber))
                {
                    //获取满包数量
                    using (WorkOrderAttributeServiceClient client = new WorkOrderAttributeServiceClient())
                    {
                        MethodReturnResult<WorkOrderAttribute> rst1 = client.Get(new WorkOrderAttributeKey()
                        {
                            OrderNumber = orderNumber,
                            AttributeName = "FullPackageQuantity"
                        });
                        if (rst1.Code == 1000)
                        {
                            return Json(rst1);
                        }

                        if (rst1.Data != null
                            && !string.IsNullOrEmpty(rst1.Data.AttributeValue))
                        {
                            double.TryParse(rst1.Data.AttributeValue, out fullQuantity);
                        }
                    }
                }
            }

            return Json(new{ 
                              CurrentQuantity = currentQuantity,
                              FullQuantity = fullQuantity,
                              IsLastestPackage = isLastestPackage
                            }, JsonRequestBehavior.AllowGet);
            
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