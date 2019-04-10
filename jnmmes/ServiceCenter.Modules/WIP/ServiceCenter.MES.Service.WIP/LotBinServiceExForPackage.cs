using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using ServiceCenter.MES.DataAccess.Interface.FMM;
using ServiceCenter.MES.DataAccess.Interface.RBAC;
using ServiceCenter.MES.DataAccess.Interface.WIP;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Model.RBAC;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Service.Contract.WIP;
using ServiceCenter.MES.Service.WIP.Resources;
using ServiceCenter.Model;
using ServiceCenter.Common;
using ServiceCenter.Common.DataAccess.NHibernate;
using System.ServiceModel.Activation;
using ServiceCenter.MES.DataAccess.Interface.PPM;
using ServiceCenter.MES.Model.EMS;
using NHibernate;
using ServiceCenter.MES.DataAccess.Interface.EMS;
using ServiceCenter.MES.DataAccess.Interface.ZPVM;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.MES.Model.PPM;
using System.Data;
using ServiceCenter.MES.Model.BaseData;

namespace ServiceCenter.MES.Service.WIP
{
    
    public partial class LotBinServiceEx
    {
        public MethodReturnResult PackageEx(PackageParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };
            #region //判断 包装输入参数是否合格
            if (string.IsNullOrEmpty(p.LineCode))
            {
                result.Code = 1001;
                result.Message = string.Format("{0} {1}"
                                                , "线别代码"
                                                , StringResource.ParameterIsNull);
                return result;
            }

            if (string.IsNullOrEmpty(p.RouteOperationName))
            {
                result.Code = 1001;
                result.Message = string.Format("{0} {1}"
                                                , "工序名称"
                                                , StringResource.ParameterIsNull);
                return result;
            }

            if (p.IsFinishPackage == false
                && (p.LotNumbers == null || p.LotNumbers.Count == 0))
            {
                result.Code = 1001;
                result.Message = string.Format("{0} {1}"
                                                , "批次号"
                                                , StringResource.ParameterIsNull);
                return result;
            }

            if (string.IsNullOrEmpty(p.PackageNo))
            {
                result.Code = 1001;
                result.Message = string.Format("{0} {1}"
                                                , "包装号"
                                                , StringResource.ParameterIsNull);
                return result;
            }
            //检查工序是否是包装工序。
            bool isPackageOperation = false;
            RouteOperationAttribute roAttr = this.RouteOperationAttributeDataEngine.Get(new RouteOperationAttributeKey()
            {
                RouteOperationName = p.RouteOperationName,
                AttributeName = "IsPackageOperation"
            });

            //如果没有设置为包装工序，则直接返回。
            if (roAttr == null
                || !bool.TryParse(roAttr.Value, out isPackageOperation)
                || isPackageOperation == false)
            {
                result.Code = 1009;
                result.Message = string.Format("{0} 非包装工序，请确认。"
                                                , p.RouteOperationName);
                return result;
            }
           
            #endregion

            //获取最近一次批次信息
            result = CheckPackageForLot(p);
            if (result.Code > 0)
            {
                return result;
            }

            #region //判断是否是自动包装线的包装号
            PagingConfig cfg = new PagingConfig()
            {
                IsPaging = false,
                Where = string.Format(@"Key.PackageNo='{0}' and BinPackaged='0' "
                                        , p.PackageNo)
            };

            IList<PackageBin> lstPackageBin = PackageBinDataEngine.Get(cfg);
            if (lstPackageBin != null && lstPackageBin.Count > 0)
            {
                result.Code = 1003;
                result.Message = string.Format("包装号（{0}）为自动包装未满托，请用手动入Bin功能进行包装。", p.PackageNo);
                return result;
            }
            lstPackageBin = null;
            #endregion

            //判断批次的效率、等级及颜色信息是否和托信息一致
            result = CheckPackageForPowerset(p);
            if(result.Code>0)
            {
                return result;
            }

            try
            {
                result = ExecutePackage(p, null, false);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLogError("PackageEx>", ex);
                result.Code = 1000;
                result.Message += string.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }           
            return result;
        }

        /// <summary>
        /// 检查批次信息是否合法
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private MethodReturnResult CheckPackageForLot(PackageParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            //获取线别车间。
            string locationName = string.Empty;
            ProductionLine pl = this.ProductionLineDataEngine.Get(p.LineCode);
            if (pl != null)
            {
                //根据线别所在区域，获取车间名称。
                Location loc = this.LocationDataEngine.Get(pl.LocationName);
                locationName = loc.ParentLocationName ?? string.Empty;
            }

            //循环批次。
            foreach (string lotNumber in p.LotNumbers)
            {
                #region //检查批次属性是否合格

                Lot lot = this.LotDataEngine.Get(lotNumber);
                //判定是否存在批次记录。
                if (lot == null || lot.Status == EnumObjectStatus.Disabled)
                {
                    result.Code = 1002;
                    result.Message = string.Format("批次（{0}）不存在。", lotNumber);
                    return result;
                }
                //批次需要已进站 
                if (lot.StateFlag == EnumLotState.WaitTrackIn)
                {
                    result.Code = 1003;
                    result.Message = string.Format("批次（{0}）还未进工序（{1}），请先做进站作业。", lotNumber, lot.RouteStepName);
                    return result;
                }

                //批次已完成。
                if (lot.StateFlag != EnumLotState.Finished && lot.StateFlag != EnumLotState.WaitTrackOut)
                {
                    result.Code = 1003;
                    result.Message = string.Format("批次（{0}）已完成。", lotNumber);
                    return result;
                }

                //批次已结束
                if (lot.DeletedFlag == true)
                {
                    result.Code = 1004;
                    result.Message = string.Format("批次（{0}）已结束。", lotNumber);
                    return result;
                }
                //批次已暂停
                if (lot.HoldFlag == true)
                {
                    result.Code = 1005;
                    result.Message = string.Format("批次（{0}）已暂停。", lotNumber);
                    return result;
                }
                //批次已包装。
                if (lot.PackageFlag == true)
                {
                    result.Code = 1006;
                    result.Message = string.Format("批次（{0}）已包装。", lotNumber);
                    return result;
                }
                //检查批次所在工序是否处于指定工序
                if (lot.RouteStepName != p.RouteOperationName)
                {
                    result.Code = 1007;
                    result.Message = string.Format("批次（{0}）目前在（{1}）工序，不能在({2})操作。"
                                                    , lotNumber
                                                    , lot.RouteStepName
                                                    , p.RouteOperationName);
                    return result;
                }

                //检查批次车间和线别车间是否匹配。
                if (lot.LocationName != locationName)
                {
                    result.Code = 1008;
                    result.Message = string.Format("批次（{0}）属于({1})车间，不能在({2})车间线别上操作。"
                                                    , lotNumber
                                                    , lot.LocationName
                                                    , locationName);
                    return result;
                }

                #endregion
            }
          
            return result;
        }

        /// <summary>
        /// 检查Lot的电流档信息是否满足要求
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private MethodReturnResult CheckPackageForPowerset(PackageParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            if (p.LotNumbers == null)
            {
                return result;
            }

            bool packageMixPowerset = false;      //是否允许混档位
            bool packageMixSubPowerset = false;   //是否允许混子档位包装。
            bool packageMixColor = false;         //是否允许混花包装。
            string packageGroup = string.Empty;//包装组。
            string packageGrade = string.Empty;//包装等级，相同包装组的等级才能包装在一起。
            string packageColor = string.Empty;//包装花色
            string packagePowersetCode = string.Empty;
            int packagePowersetCodeItemNo = -1;
            string packagePowersetSubCode = string.Empty;
            bool isCheckPackage = false;
            //获取包装记录。
            Package packageObj = this.PackageDataEngine.Get(p.PackageNo);
            if (packageObj != null && packageObj.Quantity > 0)
            {
                isCheckPackage = true;
                //获取包装明细中的第一条记录。
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key.PackageNo='{0}' AND ItemNo=1", p.PackageNo)
                };
                IList<PackageDetail> lstPackageDetail = this.PackageDetailDataEngine.Get(cfg);
                PackageDetail packageDetailObj = lstPackageDetail[0];
                //获取包等级。
                Lot lot = this.LotDataEngine.Get(packageDetailObj.Key.ObjectNumber);
                packageGrade = lot.Grade;//包装等级，相同包装组的等级才能包装在一起。
                packageColor = lot.Color;//包装花色
                //获取工单包装规则。
                WorkOrderGrade wog = this.WorkOrderGradeDataEngine.Get(new WorkOrderGradeKey()
                {
                    OrderNumber = lot.OrderNumber,
                    MaterialCode = lot.MaterialCode,
                    Grade = lot.Grade
                });
                if (wog != null)
                {
                    packageMixPowerset = wog.MixPowerset;
                    packageMixSubPowerset = wog.MixSubPowerset;
                    packageMixColor = wog.MixColor;
                    packageGroup = wog.PackageGroup;
                }
                //获取IV测试数据。
                cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key.LotNumber='{0}' AND IsDefault=1", packageDetailObj.Key.ObjectNumber)
                };
                IList<IVTestData> lstTestData = this.IVTestDataDataEngine.Get(cfg);
                if (lstTestData != null && lstTestData.Count > 0)
                {
                    packagePowersetCode = lstTestData[0].PowersetCode;
                    packagePowersetCodeItemNo = lstTestData[0].PowersetItemNo ?? -1;
                    packagePowersetSubCode = lstTestData[0].PowersetSubCode;
                }

                //获取工单分档信息，此批次就是允许混颜色
                WorkOrderPowerset wop = this.WorkOrderPowersetDataEngine.Get(new WorkOrderPowersetKey()
                {
                    OrderNumber = lot.OrderNumber,
                    MaterialCode = lot.MaterialCode,
                    Code = packagePowersetCode,
                    ItemNo = packagePowersetCodeItemNo
                });
                if (wop.MixColor)
                {
                    packageMixColor = true;
                }
            }

            foreach (string lotNumber in p.LotNumbers)
            {
                //获取IV测试数据。
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key.LotNumber='{0}' AND IsDefault=1", lotNumber)
                };
                IList<IVTestData> lstTestData = this.IVTestDataDataEngine.Get(cfg);

                //检查批次特性和包装特性是否匹配。
                if (isCheckPackage)
                {
                    //获取批次数据
                    Lot lot = this.LotDataEngine.Get(lotNumber);

                    bool mixPowerset = false;
                    bool mixSubPowerset = false;
                    bool mixColor = false;
                    string powersetCode = string.Empty;
                    int powersetCodeItemNo = -1;
                    string powersetSubCode = string.Empty;
                    string lotPackageGroup = lot.Key + lot.Grade;

                    if (lstTestData.Count > 0)
                    {
                        powersetCode = lstTestData[0].PowersetCode;
                        powersetCodeItemNo = lstTestData[0].PowersetItemNo ?? -1;
                        powersetSubCode = lstTestData[0].PowersetSubCode;
                    }
                    //检查料号是否匹配
                    if (lot.MaterialCode != packageObj.MaterialCode)
                    {
                        result.Code = 3005;
                        result.Message = string.Format("批次料号（{0}）和包料号（{1}）不匹配，无法进行包装。"
                                                        , lot.MaterialCode
                                                        , packageObj.MaterialCode);
                        return result;
                    }
                    //获取工单包装规则。
                    WorkOrderGrade wog = this.WorkOrderGradeDataEngine.Get(new WorkOrderGradeKey()
                    {
                        OrderNumber = lot.OrderNumber,
                        MaterialCode = lot.MaterialCode,
                        Grade = lot.Grade
                    });
                    if (wog != null)
                    {
                        mixPowerset = wog.MixPowerset;
                        mixSubPowerset = wog.MixSubPowerset;
                        mixColor = wog.MixColor;
                        lotPackageGroup = wog.PackageGroup;
                    }

                    //获取工单分档信息，此批次就是允许混颜色
                    WorkOrderPowerset wop = this.WorkOrderPowersetDataEngine.Get(new WorkOrderPowersetKey()
                    {
                        OrderNumber = lot.OrderNumber,
                        MaterialCode = lot.MaterialCode,
                        Code = powersetCode,
                        ItemNo = powersetCodeItemNo
                    });

                    if (wop.MixColor)
                    {
                        mixColor = true;
                    }

                    string msg = string.Format("组件（{4}）等级：{0} 花色:{1} 档位：{2} 子档位：{3}"
                                                , lot.Grade
                                                , lot.Color
                                                , wop != null ? string.Format("{0}({1})", wop.PowerName, powersetCode + ":" + powersetCodeItemNo) : powersetCode + ":" + powersetCodeItemNo
                                                , powersetSubCode
                                                , lot.Key);
                    //检查等级
                    //等级不相等 并且 不允许混等级包装。
                    if (lot.Grade != packageGrade && lotPackageGroup != packageGroup)
                    {
                        result.Code = 3000;
                        result.Message = string.Format("批次等级值（{0}）和包等级值（{1}）不匹配，无法进行包装。\n {2}"
                                                        , lot.Grade
                                                        , packageGrade
                                                        , msg);
                        return result;
                    }
                    //检查花色
                    if (mixColor == false  // 当前批次所在工单不允许混花色包装。
                        && packageMixColor == false //包装不允许混花色包装。
                        && packageColor != lot.Color) //包装花色和当前批次花色不一致。
                    {
                        result.Code = 3001;
                        result.Message = string.Format("批次花色（{0}）和包花色（{1}）不匹配，无法进行包装。\n {2}"
                                                        , lot.Color
                                                        , packageColor
                                                        , msg);
                        return result;
                    }
                    //检查功率分档。

                    if (mixPowerset == false  // 当前批次所在工单不允许混档位包装。
                       && packageMixPowerset == false //包装不允许混档位包装。
                       && (powersetCode != packagePowersetCode
                       || powersetCodeItemNo != packagePowersetCodeItemNo)
                    ) //包装档位和当前批次档位不一致。
                    {
                        result.Code = 3002;
                        result.Message = string.Format("批次档位（{0}）和包档位（{1}）不匹配，无法进行包装。 \n {2}"
                                                        , powersetCode + ":" + powersetCodeItemNo
                                                        , packagePowersetCode + ":" + packagePowersetCodeItemNo
                                                        , msg);
                        return result;
                    }
                    //检查功率子分档。
                    if (string.IsNullOrEmpty(packagePowersetSubCode))
                    {
                        packagePowersetSubCode = "";
                    }
                    if (string.IsNullOrEmpty(powersetSubCode))
                    {
                        powersetSubCode = "";
                    }
                    if (mixSubPowerset == false  // 当前批次所在工单不允许混子档位包装。
                       && packageMixSubPowerset == false //包装不允许混子档位包装。
                       && powersetSubCode != packagePowersetSubCode) //包装子档位和当前批次子档位不一致。
                    {
                        result.Code = 3003;
                        result.Message = string.Format("批次子档位（{0}）和包子档位（{1}）不匹配，无法进行包装。\n {2}"
                                                        , powersetSubCode
                                                        , packagePowersetSubCode
                                                        , msg);
                        return result;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 检查入托是否符合工单要求
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private MethodReturnResult CheckPackageForWorkOrderRule(PackageParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            if (p.LotNumbers == null)
            {
                return result;
            }
            bool isPackageLimitedForPWorkOrder = false;
            Package packageObj = this.PackageDataEngine.Get(p.PackageNo);

            //更新包装数据。
            if (packageObj != null)
            {
               
                #region //获取批次工单的包装属性
                WorkOrderAttribute workOrderAttribute = this.WorkOrderAttributeDataEngine.Get(new WorkOrderAttributeKey()
                {
                    OrderNumber = packageObj.OrderNumber,
                    AttributeName = "PackageLimited"
                });

                //如果没有设置为包装工序，则直接返回。
                if (workOrderAttribute == null
                    || !bool.TryParse(workOrderAttribute.AttributeValue, out isPackageLimitedForPWorkOrder)
                   )
                {
                    isPackageLimitedForPWorkOrder = false;
                }
                #endregion
            }

            PagingConfig cfg = null;
            string strSupplierCodeForLot = "";
            //string strSupplierCodeForPackage = "";
            //Lot lotForCreatePackageNo = null;

            foreach (string lotNumber in p.LotNumbers)
            {
                cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"Key.ObjectNumber='{0}'", lotNumber),
                    OrderBy = ""
                };
                IList<PackageDetail> lstObj = this.PackageDetailDataEngine.Get(cfg);
                if (lstObj != null && lstObj.Count > 0)
                {
                    result.Code = 2001;
                    result.Message = string.Format("批次（{0}）已存在在托号（{1}）中!"
                                                    , lotNumber
                                                    , lstObj[0].Key.PackageNo);
                    return result;
                }
                lstObj = null;

                Lot lot = this.LotDataEngine.Get(lotNumber);

                cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"Key.LotNumber='{0}' and SupplierCode in ('{1}','{2}') and MaterialName like '%{3}%'", lotNumber, "010022", "010035", "电池片"),
                    OrderBy = ""
                };

                IList<LotBOM> lstLotBom = this.LotBOMDataEngine.Get(cfg);
                if (lstLotBom != null && lstLotBom.Count > 0)
                {
                    LotBOM lotBom = lstLotBom.FirstOrDefault();
                    strSupplierCodeForLot = lotBom.SupplierCode;
                }


                //判断工单是否可以混托
                if (isPackageLimitedForPWorkOrder)
                {
                    if (string.Compare(packageObj.OrderNumber, lot.OrderNumber, true) != 0)
                    {
                        result.Code = 1012;
                        result.Message = string.Format("托{0}所属工单号（{1}）不能混装别的工单号{2}中."
                                                        , packageObj.Key
                                                        , packageObj.OrderNumber
                                                        , lot.OrderNumber);
                        return result;
                    }
                }


                #region //获取批次工单的包装属性
                bool isPackageLimited = false;
                if (string.Compare(lot.OrderNumber, packageObj.OrderNumber, true) != 0)
                {
                    //包装号里的工单若跟批次的工单相同，则不需要判断属性
                    WorkOrderAttribute workOrderAttribute = this.WorkOrderAttributeDataEngine.Get(new WorkOrderAttributeKey()
                    {
                        OrderNumber = lot.OrderNumber,
                        AttributeName = "PackageLimited"
                    });

                    //如果没有设置为包装工序，则直接返回。
                    if (workOrderAttribute == null
                        || !bool.TryParse(workOrderAttribute.AttributeValue, out isPackageLimited)
                        )
                    {
                        isPackageLimited = false;
                    }
                }
                #endregion

                //检查批次是否不能混工单
                if (isPackageLimited)
                {
                    if (string.Compare(packageObj.OrderNumber, lot.OrderNumber, true) != 0)
                    {
                        result.Code = 1012;
                        result.Message = string.Format("批次{0}所属工单号（{1}）不能混装到此托的工单号{2}中."
                                                        , lotNumber
                                                        , lot.OrderNumber
                                                        , packageObj.OrderNumber
                                                        );
                        return result;
                    }
                }

                #region 判断要包装批次的工单是否设置混工单组
                cfg = new PagingConfig()
                {
                    Where = string.Format(@"Key.OrderNumber = '{0}'", lot.OrderNumber)
                };
                IList<WorkOrderGroupDetail> lstLotWorkOrderGroupDetail = WorkOrderGroupDetailDataEngine.Get(cfg);
                //批次所在工单设置了混工单组
                if (lstLotWorkOrderGroupDetail != null && lstLotWorkOrderGroupDetail.Count > 0)
                {
                    #region 判断要托工单是否设置混工单组
                    cfg = new PagingConfig()
                    {
                        Where = string.Format(@"Key.OrderNumber = '{0}'", packageObj.OrderNumber)
                    };
                    IList<WorkOrderGroupDetail> lstPackageWorkOrderGroupDetail = WorkOrderGroupDetailDataEngine.Get(cfg);
                    //托工单设置了混工单组
                    if (lstPackageWorkOrderGroupDetail != null && lstPackageWorkOrderGroupDetail.Count > 0)
                    {
                        if (lstLotWorkOrderGroupDetail[0].Key.WorkOrderGroupNo.ToString() != lstPackageWorkOrderGroupDetail[0].Key.WorkOrderGroupNo.ToString())
                        {
                            result.Code = 0;
                            result.Message = string.Format("托：（{0}）所在工单（{1} 设置的混工单组（{2}）与入托批次（{3}）所在工单（{4}）设置的混工单组（{5}）不一致！",
                                                            packageObj.Key, packageObj.OrderNumber, lstPackageWorkOrderGroupDetail[0].Key.WorkOrderGroupNo.ToString(),
                                                            lot.Key, lot.OrderNumber, lstLotWorkOrderGroupDetail[0].Key.WorkOrderGroupNo.ToString());
                            return result;
                        }
                    }
                    //托工单没设混工单组
                    else
                    {
                        result.Code = 0;
                        result.Message = string.Format("托：（{0}）所在工单（{1} 未设置混工单组规则，但要入托批次（{2}）所在工单（{3}）设置了混工单组！",
                                                        packageObj.Key, packageObj.OrderNumber, lot.Key, lot.OrderNumber);
                        return result;
                    }
                    #endregion
                }
                //批次所在工单没设混工单组
                else
                {
                    #region 判断要托工单是否设置混工单组
                    cfg = new PagingConfig()
                    {
                        Where = string.Format(@"Key.OrderNumber = '{0}'", packageObj.OrderNumber)
                    };
                    IList<WorkOrderGroupDetail> lstPackageWorkOrderGroupDetail = WorkOrderGroupDetailDataEngine.Get(cfg);
                    //托工单设置了混工单组
                    if (lstPackageWorkOrderGroupDetail != null && lstPackageWorkOrderGroupDetail.Count > 0)
                    {
                        result.Code = 0;
                        result.Message = string.Format("托：（{0}）所在工单（{1} 已设置混工单组规则，但要入托批次（{2}）所在工单（{3}）未设置混工单组！",
                                                        packageObj.Key, packageObj.OrderNumber, lot.Key, lot.OrderNumber);
                        return result;
                    }
                    #endregion
                }
                #endregion

                #region 检查电池片供应商批次号是否能混
                if (String.IsNullOrEmpty(packageObj.SupplierCode) == true || packageObj.SupplierCode == "")
                {
                    packageObj.SupplierCode = strSupplierCodeForLot;
                }
                else
                {
                    bool blChkSupplierCode = false;
                    //检查 packageNO
                    if (String.IsNullOrEmpty(strSupplierCodeForLot) == false && strSupplierCodeForLot != "")
                    {
                        //BaseAttributeValueDataEngine

                        cfg = new PagingConfig()
                        {
                            IsPaging = false,
                            Where = string.Format(@"Key.CategoryName='{0}'
                                        AND Key.AttributeName='{1}'"
                                                    , "SystemParameters"
                                                    , "PackageChkMaterialSupplier"),
                            OrderBy = "Key.ItemOrder"
                        };
                        IList<BaseAttributeValue> lstBaseAttributeValues = BaseAttributeValueDataEngine.Get(cfg);
                        if (lstBaseAttributeValues != null && lstBaseAttributeValues.Count > 0)
                        {
                            BaseAttributeValue obj = lstBaseAttributeValues.FirstOrDefault();
                            if (String.IsNullOrEmpty(obj.Value) == false)
                            {
                                Boolean.TryParse(obj.Value, out blChkSupplierCode);
                            }
                        }
                        if (blChkSupplierCode)
                        {
                            //系统判断是否需要进行供应商的检测
                            if (string.Compare(packageObj.SupplierCode, strSupplierCodeForLot, true) != 0)
                            {
                                result.Code = 1010;
                                result.Message = string.Format("此托已存在电池片供应商编号为（{2}）。批次（{0}）所用的电池片供应商编号为（{1}），不能混装入托。"
                                                                , lotNumber
                                                                , strSupplierCodeForLot
                                                                , packageObj.SupplierCode);
                                return result;
                            }
                        }
                    }
                }
                #endregion
            }

            return result;
        }

        /// <summary>
        /// 执行包装入库
        /// </summary>
        /// <param name="p"></param>
        /// <param name="db"></param>
        /// <param name="executedWithTransaction"></param>
        /// <returns></returns>
        public MethodReturnResult ExecutePackage(PackageParameter p, ISession db, bool executedWithTransaction)
        {
            DateTime now = DateTime.Now;
            string strNewPackageNo = "";
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            p.TransactionKeys = new Dictionary<string, string>();
            strNewPackageNo = p.PackageNo;

            #region define List of DataEngine
            lstLotDataEngineForUpdate = new List<Lot>();
            lstLotTransactionForInsert = new List<LotTransaction>();
            lstLotTransactionHistoryForInsert = new List<LotTransactionHistory>();
            lstLotTransactionParameterDataEngineForInsert = new List<LotTransactionParameter>();
            lstLotTransactionStepDataEngineForInsert = new List<LotTransactionStep>();

            //LotTransactionEquipment ,Equipment ,EquipmentStateEvent
            lstLotTransactionEquipmentForUpdate = new List<LotTransactionEquipment>();
            lstLotTransactionEquipmentForInsert = new List<LotTransactionEquipment>();

            lstEquipmentForUpdate = new List<Equipment>();
            lstEquipmentStateEventForInsert = new List<EquipmentStateEvent>();

            //Package
            lstPackageDataForUpdate = new List<Package>();
            lstPackageDataForInsert = new List<Package>();
            lstPackageDetailForInsert = new List<PackageDetail>();
            lstLotTransactionPackageForInsert = new List<LotTransactionPackage>();
            #endregion


            bool isPackageLimitedForPWorkOrder = false;

            Package packageObj = this.PackageDataEngine.Get(p.PackageNo);
            Package packageUpdate = null;
            //更新包装数据。
            if (packageObj != null)
            {
                packageUpdate = packageObj.Clone() as Package;
                packageUpdate.Editor = p.Creator;
                packageUpdate.EditTime = now;
                packageUpdate.IsLastPackage = p.IsLastestPackage;
                if (p.IsFinishPackage)
                {
                    packageUpdate.PackageState = EnumPackageState.Packaged;
                }

                #region //获取批次工单的包装属性

                WorkOrderAttribute workOrderAttribute = this.WorkOrderAttributeDataEngine.Get(new WorkOrderAttributeKey()
                {
                    OrderNumber = packageUpdate.OrderNumber,
                    AttributeName = "PackageLimited"
                });

                //如果没有设置为包装工序，则直接返回。
                if (workOrderAttribute == null
                    || !bool.TryParse(workOrderAttribute.AttributeValue, out isPackageLimitedForPWorkOrder)
                   )
                {
                    isPackageLimitedForPWorkOrder = false;
                }
                #endregion
            }

            PagingConfig cfg = null;
            string strSupplierCodeForLot = "";
            //string strSupplierCodeForPackage = "";
            Lot lotForCreatePackageNo = null;

            #region //循环批次
            foreach (string lotNumber in p.LotNumbers)
            {
                cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"Key.ObjectNumber='{0}'", lotNumber),
                    OrderBy = ""
                };
                IList<PackageDetail> lstObj = this.PackageDetailDataEngine.Get(cfg);
                if (lstObj != null && lstObj.Count > 0)
                {
                    result.Code = 2001;
                    result.Message = string.Format("批次（{0}）已存在在托号（{1}）中!"
                                                    , lotNumber
                                                    , lstObj[0].Key.PackageNo);
                    return result;
                }
                lstObj = null;

                Lot lot = this.LotDataEngine.Get(lotNumber);

                cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@"Key.LotNumber='{0}' and SupplierCode in ('{1}','{2}') and MaterialName like '%{3}%'", lotNumber, "010022", "010035", "电池片"),
                    OrderBy = ""
                };

                IList<LotBOM> lstLotBom = this.LotBOMDataEngine.Get(cfg);
                if (lstLotBom != null && lstLotBom.Count > 0)
                {
                    LotBOM lotBom = lstLotBom.FirstOrDefault();
                    strSupplierCodeForLot = lotBom.SupplierCode;
                }
                //生成操作事务主键。
                string transactionKey = Guid.NewGuid().ToString();
                p.TransactionKeys.Add(lotNumber, transactionKey);

                //更新批次记录。
                Lot lotUpdate = lot.Clone() as Lot;
                lotUpdate.PackageFlag = true;
                lotUpdate.PackageNo = p.PackageNo;
                lotUpdate.OperateComputer = p.OperateComputer;
                lotUpdate.Editor = p.Creator;
                lotUpdate.EditTime = now;

                #region //记录包装数据。
                if (packageObj == null)
                {
                    lotForCreatePackageNo = lot;
                    packageObj = new Package()
                    {
                        CreateTime = now,
                        Creator = p.Creator,
                        Checker = null,
                        CheckTime = null,
                        ContainerNo = null,
                        Description = p.Remark,
                        Editor = p.Creator,
                        EditTime = now,
                        IsLastPackage = p.IsLastestPackage,
                        Key = p.PackageNo,
                        MaterialCode = lot.MaterialCode,
                        OrderNumber = lot.OrderNumber,
                        PackageState = p.IsFinishPackage ? EnumPackageState.Packaged : EnumPackageState.Packaging,
                        PackageType = EnumPackageType.Packet,
                        Quantity = lot.Quantity,
                        ShipmentPerson = null,
                        ShipmentTime = null,
                        ToWarehousePerson = null,
                        ToWarehouseTime = null,
                        SupplierCode = strSupplierCodeForLot,
                        PackageMixedType = EnumPackageMixedType.UnMixedType
                    };
                    lstPackageDataForInsert.Add(packageObj);
                    //this.PackageDataEngine.Insert(packageObj);
                }
                else
                {
                    //更新包装数据。
                    if (packageUpdate == null)
                    {
                        packageUpdate = packageObj.Clone() as Package;
                    }
                    if (packageUpdate.Quantity == 0)
                    {
                        packageUpdate.OrderNumber = lot.OrderNumber;
                        packageUpdate.MaterialCode = lot.MaterialCode;
                    }

                    //判断工单是否可以混托
                    if (isPackageLimitedForPWorkOrder)
                    {
                        if (string.Compare(packageUpdate.OrderNumber, lot.OrderNumber, true) != 0)
                        {
                            result.Code = 1012;
                            result.Message = string.Format("托{0}所属工单号（{1}）不能混装别的工单号{2}中."
                                                            , packageUpdate.Key
                                                            , packageUpdate.OrderNumber
                                                            , lot.OrderNumber);
                            return result;
                        }
                    }


                    #region //获取批次工单的包装属性
                    bool isPackageLimited = false;
                    if (string.Compare(lot.OrderNumber, packageUpdate.OrderNumber, true) != 0)
                    {
                        //包装号里的工单若跟批次的工单相同，则不需要判断属性
                        WorkOrderAttribute workOrderAttribute = this.WorkOrderAttributeDataEngine.Get(new WorkOrderAttributeKey()
                        {
                            OrderNumber = lot.OrderNumber,
                            AttributeName = "PackageLimited"
                        });

                        //如果没有设置为包装工序，则直接返回。
                        if (workOrderAttribute == null
                            || !bool.TryParse(workOrderAttribute.AttributeValue, out isPackageLimited)
                           )
                        {
                            isPackageLimited = false;
                        }
                    }
                    #endregion

                    //检查批次是否不能混工单
                    if (isPackageLimited)
                    {
                        if (string.Compare(packageUpdate.OrderNumber, lot.OrderNumber, true) != 0)
                        {
                            result.Code = 1012;
                            result.Message = string.Format("批次{0}所属工单号（{1}）不能混装到此托的工单号{2}中."
                                                            , lotNumber
                                                            , lot.OrderNumber
                                                            , packageUpdate.OrderNumber
                                                            );
                            return result;
                        }
                    }

                    #region 判断要包装批次的工单是否设置混工单组
                    cfg = new PagingConfig()
                    {
                        Where = string.Format(@"Key.OrderNumber = '{0}'", lot.OrderNumber)
                    };
                    IList<WorkOrderGroupDetail> lstLotWorkOrderGroupDetail = WorkOrderGroupDetailDataEngine.Get(cfg);
                    //批次所在工单设置了混工单组
                    if (lstLotWorkOrderGroupDetail != null && lstLotWorkOrderGroupDetail.Count > 0)
                    {
                        #region 判断要托工单是否设置混工单组
                        cfg = new PagingConfig()
                        {
                            Where = string.Format(@"Key.OrderNumber = '{0}'", packageObj.OrderNumber)
                        };
                        IList<WorkOrderGroupDetail> lstPackageWorkOrderGroupDetail = WorkOrderGroupDetailDataEngine.Get(cfg);
                        //托工单设置了混工单组
                        if (lstPackageWorkOrderGroupDetail != null && lstPackageWorkOrderGroupDetail.Count > 0)
                        {
                            if (lstLotWorkOrderGroupDetail[0].Key.WorkOrderGroupNo.ToString() != lstPackageWorkOrderGroupDetail[0].Key.WorkOrderGroupNo.ToString())
                            {
                                result.Code = 1012;
                                result.Message = string.Format("托：（{0}）所在工单（{1} 设置的混工单组（{2}）与入托批次（{3}）所在工单（{4}）设置的混工单组（{5}）不一致！",
                                                                packageObj.Key, packageObj.OrderNumber, lstPackageWorkOrderGroupDetail[0].Key.WorkOrderGroupNo.ToString(),
                                                                lot.Key, lot.OrderNumber, lstLotWorkOrderGroupDetail[0].Key.WorkOrderGroupNo.ToString());
                                return result;
                            }
                        }
                        //托工单没设混工单组
                        else
                        {
                            result.Code = 1012;
                            result.Message = string.Format("托：（{0}）所在工单（{1} 未设置混工单组规则，但要入托批次（{2}）所在工单（{3}）设置了混工单组！",
                                                            packageObj.Key, packageObj.OrderNumber, lot.Key, lot.OrderNumber);
                            return result;
                        }
                        #endregion
                    }
                    //批次所在工单没设混工单组
                    else
                    {
                        #region 判断要托工单是否设置混工单组
                        cfg = new PagingConfig()
                        {
                            Where = string.Format(@"Key.OrderNumber = '{0}'", packageObj.OrderNumber)
                        };
                        IList<WorkOrderGroupDetail> lstPackageWorkOrderGroupDetail = WorkOrderGroupDetailDataEngine.Get(cfg);
                        //托工单设置了混工单组
                        if (lstPackageWorkOrderGroupDetail != null && lstPackageWorkOrderGroupDetail.Count > 0)
                        {
                            result.Code = 1012;
                            result.Message = string.Format("托：（{0}）所在工单（{1} 已设置混工单组规则，但要入托批次（{2}）所在工单（{3}）未设置混工单组！",
                                                            packageObj.Key, packageObj.OrderNumber, lot.Key, lot.OrderNumber);
                            return result;
                        }
                        #endregion
                    }
                    #endregion

                    if (String.IsNullOrEmpty(packageUpdate.SupplierCode) == true || packageUpdate.SupplierCode == "")
                    {
                        packageUpdate.SupplierCode = strSupplierCodeForLot;
                    }
                    else
                    {
                        bool blChkSupplierCode = false;
                        //检查 packageNO
                        if (String.IsNullOrEmpty(strSupplierCodeForLot) == false && strSupplierCodeForLot != "")
                        {
                            //BaseAttributeValueDataEngine

                            cfg = new PagingConfig()
                            {
                                IsPaging = false,
                                Where = string.Format(@"Key.CategoryName='{0}'
                                           AND Key.AttributeName='{1}'"
                                                        , "SystemParameters"
                                                        , "PackageChkMaterialSupplier"),
                                OrderBy = "Key.ItemOrder"
                            };
                            IList<BaseAttributeValue> lstBaseAttributeValues = BaseAttributeValueDataEngine.Get(cfg);
                            if (lstBaseAttributeValues != null && lstBaseAttributeValues.Count > 0)
                            {
                                BaseAttributeValue obj = lstBaseAttributeValues.FirstOrDefault();
                                if (String.IsNullOrEmpty(obj.Value) == false)
                                {
                                    Boolean.TryParse(obj.Value, out blChkSupplierCode);
                                }
                            }
                            if (blChkSupplierCode)
                            {
                                //系统判断是否需要进行供应商的检测
                                if (string.Compare(packageUpdate.SupplierCode, strSupplierCodeForLot, true) != 0)
                                {
                                    result.Code = 1010;
                                    result.Message = string.Format("此托已存在电池片供应商编号为（{2}）。批次（{0}）所用的电池片供应商编号为（{1}），不能混装入托。"
                                                                    , lotNumber
                                                                    , strSupplierCodeForLot
                                                                    , packageUpdate.SupplierCode);
                                    return result;
                                }
                            }
                        }
                    }

                    packageUpdate.Quantity += lot.Quantity;
                }
                //记录包装明细数据。
                int itemNo = 1;
                cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key.PackageNo='{0}'", p.PackageNo),
                    OrderBy = "ItemNo Desc"
                };

                IList<PackageDetail> lstPackageDetail = this.PackageDetailDataEngine.Get(cfg);
                if (lstPackageDetail != null && lstPackageDetail.Count > 0)
                {
                    itemNo = lstPackageDetail[0].ItemNo + 1;
                }

                PackageDetail packageDetail = new PackageDetail()
                {
                    Key = new PackageDetailKey()
                    {
                        PackageNo = p.PackageNo,
                        ObjectNumber = lot.Key,
                        ObjectType = EnumPackageObjectType.Lot
                    },
                    ItemNo = itemNo,
                    CreateTime = now,
                    Creator = p.Creator,
                    MaterialCode = lot.MaterialCode,
                    OrderNumber = lot.OrderNumber
                };
                //this.PackageDetailDataEngine.Insert(packageDetail);
                lstPackageDetailForInsert.Add(packageDetail);
                #endregion

                LotTransaction transObj = null;
                LotTransactionHistory lotHistory = null;

                if (lotUpdate.StateFlag == EnumLotState.WaitTrackIn)
                {
                    #region //LOT FOR TrackIn
                    /*
                    lotUpdate.StartProcessTime = now;
                    lotUpdate.EquipmentCode = p.EquipmentCode;
                    lotUpdate.LineCode = p.LineCode;
                    lotUpdate.OperateComputer = p.OperateComputer;
                    lotUpdate.PreLineCode = lot.LineCode;
                    lotUpdate.StateFlag = EnumLotState.WaitTrackOut;


                    //记录批次设备加工历史数据
                    if (!string.IsNullOrEmpty(p.EquipmentCode))
                    {
                        LotTransactionEquipment transEquipment = new LotTransactionEquipment()
                        {
                            Key = Guid.NewGuid().ToString(),
                            EndTransactionKey = null,
                            CreateTime = now,
                            Creator = p.Creator,
                            Editor = p.Creator,
                            EditTime = now,
                            EndTime = null,
                            EquipmentCode = p.EquipmentCode,
                            LotNumber = lotNumber,
                            Quantity = lot.Quantity,
                            StartTime = now,
                            State = EnumLotTransactionEquipmentState.Start
                        };
                        lstLotTransactionEquipmentForInsert.Add(transEquipment);
                        //this.LotTransactionEquipmentDataEngine.Insert(transEquipment);
                    }

                    #region //更新设备状态
                    bool blEquipmentContinue = false;
                    if (string.IsNullOrEmpty(p.EquipmentCode))
                    {
                        blEquipmentContinue = false;
                    }
                    Equipment e = null;
                    if (blEquipmentContinue)
                    {
                        //获取设备数据。
                        e = this.EquipmentDataEngine.Get(p.EquipmentCode);
                        if (e == null)
                        {
                            blEquipmentContinue = false;
                        }
                    }
                    EquipmentState es = null;
                    if (blEquipmentContinue)
                    {
                        //获取设备当前状态。
                        es = this.EquipmentStateDataEngine.Get(e.StateName);
                        if (es == null)
                        {
                            blEquipmentContinue = false;
                        }
                    }
                    #region //blEquipmentContinue =true
                    EquipmentState runState = null;
                    if (blEquipmentContinue)
                    {
                        //获取设备RUN的主键
                        runState = this.EquipmentStateDataEngine.Get("RUN");

                        //获取设备当前状态->RUN的状态切换数据。
                        EquipmentChangeState ecsToRun = this.EquipmentChangeStateDataEngine.Get(es.Key, runState.Key);
                        if (ecsToRun != null)
                        {
                            //更新父设备状态。
                            if (!string.IsNullOrEmpty(e.ParentEquipmentCode))
                            {
                                Equipment ep = this.EquipmentDataEngine.Get(e.ParentEquipmentCode);
                                if (ep != null)
                                {
                                    Equipment epUpdate = ep.Clone() as Equipment;
                                    //更新设备状态。
                                    epUpdate.StateName = runState.Key;
                                    epUpdate.ChangeStateName = ecsToRun.Key;

                                    lstEquipmentForUpdate.Add(epUpdate);
                                    //this.EquipmentDataEngine.Update(epUpdate);

                                    //新增设备状态事件数据
                                    EquipmentStateEvent newStateEvent = new EquipmentStateEvent()
                                    {
                                        Key = Guid.NewGuid().ToString(),
                                        CreateTime = now,
                                        Creator = p.Creator,
                                        Description = p.Remark,
                                        Editor = p.Creator,
                                        EditTime = now,
                                        EquipmentChangeStateName = ecsToRun.Key,
                                        EquipmentCode = e.ParentEquipmentCode,
                                        EquipmentFromStateName = es.Key,
                                        EquipmentToStateName = runState.Key,
                                        IsCurrent = true
                                    };
                                    lstEquipmentStateEventForInsert.Add(newStateEvent);
                                    //this.EquipmentStateEventDataEngine.Insert(newStateEvent);
                                }
                            }

                            //更新设备状态。
                            Equipment eUpdate = e.Clone() as Equipment;
                            eUpdate.StateName = runState.Key;
                            eUpdate.ChangeStateName = ecsToRun.Key;
                            lstEquipmentForUpdate.Add(eUpdate);
                            //this.EquipmentDataEngine.Update(eUpdate);


                            //新增设备状态事件数据
                            EquipmentStateEvent stateEvent = new EquipmentStateEvent()
                            {
                                Key = Guid.NewGuid().ToString(),
                                CreateTime = now,
                                Creator = p.Creator,
                                Description = p.Remark,
                                Editor = p.Creator,
                                EditTime = now,
                                EquipmentChangeStateName = ecsToRun.Key,
                                EquipmentCode = e.Key,
                                EquipmentFromStateName = es.Key,
                                EquipmentToStateName = runState.Key,
                                IsCurrent = true
                            };
                            lstEquipmentStateEventForInsert.Add(stateEvent);
                            // this.EquipmentStateEventDataEngine.Insert(stateEvent);
                        }
                    }
                    #endregion

                    #endregion

                    #region//记录TrackIn操作历史。
                    transactionKey = Guid.NewGuid().ToString();
                    transObj = new LotTransaction()
                    {
                        Key = transactionKey,
                        Activity = EnumLotActivity.TrackIn,
                        CreateTime = now,
                        Creator = p.Creator,
                        Description = p.Remark,
                        Editor = p.Creator,
                        EditTime = now,
                        InQuantity = lot.Quantity,
                        LotNumber = lotNumber,
                        OperateComputer = p.OperateComputer,
                        OrderNumber = lot.OrderNumber,
                        OutQuantity = lotUpdate.Quantity,
                        RouteEnterpriseName = lot.RouteEnterpriseName,
                        RouteName = lot.RouteName,
                        RouteStepName = lot.RouteStepName,
                        ShiftName = p.ShiftName,
                        UndoFlag = false,
                        UndoTransactionKey = null
                    };
                    lstLotTransactionForInsert.Add(transObj);
                    //this.LotTransactionDataEngine.Insert(transObj);
                    //新增批次历史记录。
                    lotHistory = new LotTransactionHistory(transactionKey, lot);

                    lstLotTransactionHistoryForInsert.Add(lotHistory);
                    //this.LotTransactionHistoryDataEngine.Insert(lotHistory);
                    #endregion
                    */
                    #endregion //End of LOT FOR TrackIn
                }

                //批次处于等待出站状态。
                if (lot.StateFlag == EnumLotState.WaitTrackOut)
                {
                    #region //获取 Lot信息，下一道工序


                    //根据批次当前工艺工步获取下一个工步
                    RouteStepKey rsKey = new RouteStepKey()
                    {
                        RouteName = lot.RouteName,
                        RouteStepName = lot.RouteStepName
                    };
                    RouteStep rsObj = this.RouteStepDataEngine.Get(rsKey);
                    if (rsObj == null)
                    {
                        result.Code = 2001;
                        result.Message = string.Format("批次（{0}）所在工艺流程（{1}）不存在。"
                                                        , lotNumber
                                                        , rsKey);
                        return result;
                    }
                    cfg = new PagingConfig()
                    {
                        PageNo = 0,
                        PageSize = 1,
                        Where = string.Format(@"Key.RouteName='{0}'
                                        AND SortSeq>='{1}'"
                                                , rsObj.Key.RouteName
                                                , rsObj.SortSeq + 1),
                        OrderBy = "SortSeq"
                    };
                    IList<RouteStep> lstRouteStep = this.RouteStepDataEngine.Get(cfg);
                    if (lstRouteStep.Count == 0)
                    {
                        //获取下一个工艺流程。
                        RouteEnterpriseDetail reDetail = this.RouteEnterpriseDetailDataEngine.Get(new RouteEnterpriseDetailKey()
                        {
                            RouteEnterpriseName = lot.RouteEnterpriseName,
                            RouteName = lot.RouteName
                        });


                        if (reDetail != null)
                        {
                            cfg = new PagingConfig()
                            {
                                PageNo = 0,
                                PageSize = 1,
                                Where = string.Format(@"Key.RouteEnterpriseName='{0}'
                                                AND ItemNo='{1}'"
                                                        , reDetail.Key.RouteEnterpriseName
                                                        , reDetail.ItemNo + 1),
                                OrderBy = "ItemNo"
                            };
                            IList<RouteEnterpriseDetail> lstRouteEnterpriseDetail = this.RouteEnterpriseDetailDataEngine.Get(cfg);
                            if (lstRouteEnterpriseDetail.Count > 0)
                            {
                                //获取下一工艺流程的第一个工艺工步。
                                reDetail = lstRouteEnterpriseDetail[0];
                                cfg = new PagingConfig()
                                {
                                    PageNo = 0,
                                    PageSize = 1,
                                    Where = string.Format(@"Key.RouteName='{0}'"
                                                            , reDetail.Key.RouteName),
                                    OrderBy = "SortSeq"
                                };
                                lstRouteStep = this.RouteStepDataEngine.Get(cfg);
                            }
                        }
                    }
                    bool isFinish = true;
                    string toRouteEnterpriseName = lot.RouteEnterpriseName;
                    string toRouteName = lot.RouteName;
                    string toRouteStepName = lot.RouteStepName;
                    if (lstRouteStep != null && lstRouteStep.Count > 0)
                    {
                        isFinish = false;
                        toRouteName = lstRouteStep[0].Key.RouteName;
                        toRouteStepName = lstRouteStep[0].Key.RouteStepName;
                    }

                    //更新批次记录。
                    if (isFinish)
                    {
                        lotUpdate.StartWaitTime = null;
                    }
                    else
                    {
                        lotUpdate.StartWaitTime = now;
                    }
                    lotUpdate.StartProcessTime = null;
                    lotUpdate.StateFlag = EnumLotState.WaitTrackIn;
                    lotUpdate.RouteEnterpriseName = toRouteEnterpriseName;
                    lotUpdate.RouteName = toRouteName;
                    lotUpdate.RouteStepName = toRouteStepName;
                    lotUpdate.OperateComputer = p.OperateComputer;
                    lotUpdate.PreLineCode = lot.LineCode;
                    lotUpdate.LineCode = p.LineCode;
                    lotUpdate.Editor = p.Creator;
                    lotUpdate.EditTime = now;
                    lotUpdate.EquipmentCode = null;

                    #endregion

                    #region //更新设备信息 , 设备的Event ,设备的Transaction

                    //bool blLogEquipment = true;
                    string equipmentCode = lot.EquipmentCode;
                    //进站时没有选择设备，则设置出站时批次为当前设备。
                    if (string.IsNullOrEmpty(equipmentCode))
                    {
                        equipmentCode = p.EquipmentCode;
                    }

                    //进站时已经确定设备。              
                    if (!string.IsNullOrEmpty(lot.EquipmentCode))
                    {
                        //更新批次设备加工历史数据。
                        cfg = new PagingConfig()
                        {
                            IsPaging = false,
                            Where = string.Format("LotNumber='{0}' AND EquipmentCode='{1}' AND State=0",
                                                    lotNumber,
                                                    lot.EquipmentCode)
                        };
                        IList<LotTransactionEquipment> lstLotTransactionEquipment = this.LotTransactionEquipmentDataEngine.Get(cfg);
                        foreach (LotTransactionEquipment lotTransactionEquipment in lstLotTransactionEquipment)
                        {
                            LotTransactionEquipment itemUpdate = lotTransactionEquipment.Clone() as LotTransactionEquipment;
                            itemUpdate.EndTransactionKey = transactionKey;
                            itemUpdate.EditTime = now;
                            itemUpdate.Editor = p.Creator;
                            itemUpdate.EndTime = now;
                            itemUpdate.State = EnumLotTransactionEquipmentState.End;
                            lstLotTransactionEquipmentForUpdate.Add(itemUpdate);
                            //this.LotTransactionEquipmentDataEngine.Update(itemUpdate, session);
                        }
                    }
                    else
                    {//记录批次设备加工历史数据
                        LotTransactionEquipment transEquipment = new LotTransactionEquipment()
                        {
                            Key = transactionKey,
                            EndTransactionKey = transactionKey,
                            CreateTime = now,
                            Creator = p.Creator,
                            Editor = p.Creator,
                            EditTime = now,
                            EndTime = now,
                            EquipmentCode = p.EquipmentCode,
                            LotNumber = lotNumber,
                            Quantity = lot.Quantity,
                            StartTime = lot.StartProcessTime,
                            State = EnumLotTransactionEquipmentState.End
                        };
                        lstLotTransactionEquipmentForInsert.Add(transEquipment);
                        //this.LotTransactionEquipmentDataEngine.Insert(transEquipment, session);
                    }

                    #region 设备状态信息更新
                    /*
                        //如果出站也没有选择设备，直接返回。                       
                        if (string.IsNullOrEmpty(equipmentCode) == false)
                        {
                            //获取设备数据
                            Equipment e = this.EquipmentDataEngine.Get(equipmentCode ?? string.Empty);
                            if (e != null)
                            {
                                //获取设备当前状态。
                                EquipmentState es = this.EquipmentStateDataEngine.Get(e.StateName ?? string.Empty);
                                if (es != null)
                                {
                                    //获取设备LOST的主键
                                    EquipmentState lostState = this.EquipmentStateDataEngine.Get("LOST");
                                    //获取设备当前状态->LOST的状态切换数据。
                                    EquipmentChangeState ecsToLost = this.EquipmentChangeStateDataEngine.Get(es.Key, lostState.Key);

                                    if (ecsToLost != null)
                                    {
                                        //根据设备编码获取当前加工批次数据。
                                        cfg = new PagingConfig()
                                        {
                                            PageSize = 1,
                                            PageNo = 0,
                                            Where = string.Format("EquipmentCode='{0}' AND STATE='{1}'"
                                                                    , equipmentCode
                                                                    , Convert.ToInt32(EnumLotTransactionEquipmentState.Start))
                                        };
                                        IList<LotTransactionEquipment> lst = this.LotTransactionEquipmentDataEngine.Get(cfg);
                                        if (lst.Count == 0)//设备当前加工批次>0，则直接返回。
                                        {
                                            #region Change EqupmentState
                                            //更新父设备状态。
                                            if (!string.IsNullOrEmpty(e.ParentEquipmentCode))
                                            {
                                                Equipment ep = this.EquipmentDataEngine.Get(e.ParentEquipmentCode);
                                                if (ep != null)
                                                {
                                                    Equipment epUpdate = ep.Clone() as Equipment;
                                                    //更新设备状态。
                                                    epUpdate.StateName = lostState.Key;
                                                    epUpdate.ChangeStateName = ecsToLost.Key;
                                                    //this.EquipmentDataEngine.Update(epUpdate);
                                                    lstEquipmentForUpdate.Add(epUpdate);
                                                    //新增设备状态事件数据
                                                    EquipmentStateEvent newStateEvent = new EquipmentStateEvent()
                                                    {
                                                        Key = Guid.NewGuid().ToString(),
                                                        CreateTime = now,
                                                        Creator = p.Creator,
                                                        Description = p.Remark,
                                                        Editor = p.Creator,
                                                        EditTime = now,
                                                        EquipmentChangeStateName = ecsToLost.Key,
                                                        EquipmentCode = e.ParentEquipmentCode,
                                                        EquipmentFromStateName = es.Key,
                                                        EquipmentToStateName = lostState.Key,
                                                        IsCurrent = true
                                                    };
                                                    //this.EquipmentStateEventDataEngine.Insert(newStateEvent);
                                                    lstEquipmentStateEventForInsert.Add(newStateEvent);
                                                }
                                            }
                                            //更新设备状态。
                                            Equipment eUpdate = e.Clone() as Equipment;
                                            eUpdate.StateName = lostState.Key;
                                            eUpdate.ChangeStateName = ecsToLost.Key;
                                            lstEquipmentForUpdate.Add(eUpdate);
                                            //this.EquipmentDataEngine.Update(eUpdate);

                                            //新增设备状态事件数据
                                            EquipmentStateEvent stateEvent = new EquipmentStateEvent()
                                            {
                                                Key = Guid.NewGuid().ToString(),
                                                CreateTime = now,
                                                Creator = p.Creator,
                                                Description = p.Remark,
                                                Editor = p.Creator,
                                                EditTime = now,
                                                EquipmentChangeStateName = ecsToLost.Key,
                                                EquipmentCode = e.Key,
                                                EquipmentFromStateName = es.Key,
                                                EquipmentToStateName = lostState.Key,
                                                IsCurrent = true
                                            };
                                            //this.EquipmentStateEventDataEngine.Insert(stateEvent);
                                            lstEquipmentStateEventForInsert.Add(stateEvent);
                                            #endregion
                                        }
                                    }
                                }
                            }
                        }*/
                    #endregion

                    #endregion

                    #region//记录操作历史。
                    transObj = new LotTransaction()
                    {
                        Key = transactionKey,
                        Activity = EnumLotActivity.TrackOut,
                        CreateTime = now,
                        Creator = p.Creator,
                        Description = p.Remark,
                        Editor = p.Creator,
                        EditTime = now,
                        InQuantity = lot.Quantity,
                        LotNumber = lotNumber,
                        OperateComputer = p.OperateComputer,
                        OrderNumber = lot.OrderNumber,
                        OutQuantity = lotUpdate.Quantity,
                        RouteEnterpriseName = lot.RouteEnterpriseName,
                        RouteName = lot.RouteName,
                        RouteStepName = lot.RouteStepName,
                        ShiftName = p.ShiftName,
                        UndoFlag = false,
                        UndoTransactionKey = null
                    };
                    lstLotTransactionForInsert.Add(transObj);
                    //this.LotTransactionDataEngine.Insert(transObj, session);

                    //新增批次历史记录。
                    lotHistory = new LotTransactionHistory(transactionKey, lot);
                    lstLotTransactionHistoryForInsert.Add(lotHistory);
                    //this.LotTransactionHistoryDataEngine.Insert(lotHistory, session);

                    //新增工艺下一步记录。
                    LotTransactionStep nextStep = new LotTransactionStep()
                    {
                        Key = transactionKey,
                        ToRouteEnterpriseName = toRouteEnterpriseName,
                        ToRouteName = toRouteName,
                        ToRouteStepName = toRouteStepName,
                        Editor = p.Creator,
                        EditTime = now
                    };
                    lstLotTransactionStepDataEngineForInsert.Add(nextStep);
                    //this.LotTransactionStepDataEngine.Insert(nextStep, session);
                    #endregion
                }
                lstLotDataEngineForUpdate.Add(lotUpdate);

                #region//记录Package操作历史。
                transactionKey = Guid.NewGuid().ToString();
                now = System.DateTime.Now;
                transObj = new LotTransaction()
                {
                    Key = transactionKey,
                    Activity = EnumLotActivity.Package,
                    CreateTime = now,
                    Creator = p.Creator,
                    Description = p.Remark,
                    Editor = p.Creator,
                    EditTime = now,
                    InQuantity = lot.Quantity,
                    LotNumber = lotNumber,
                    OperateComputer = p.OperateComputer,
                    OrderNumber = lot.OrderNumber,
                    OutQuantity = lotUpdate.Quantity,
                    RouteEnterpriseName = lot.RouteEnterpriseName,
                    RouteName = lot.RouteName,
                    RouteStepName = lot.RouteStepName,
                    ShiftName = p.ShiftName,
                    UndoFlag = false,
                    UndoTransactionKey = null
                };
                //暂时忽略打包的事物,事物设置为撤销
                lstLotTransactionForInsert.Add(transObj);
                //this.LotTransactionDataEngine.Insert(transObj);
                //新增批次历史记录。
                lotHistory = new LotTransactionHistory(transactionKey, lot);
                lotHistory.StateFlag = EnumLotState.WaitTrackOut;
                lstLotTransactionHistoryForInsert.Add(lotHistory);
                //this.LotTransactionHistoryDataEngine.Insert(lotHistory);

                LotTransactionPackage transPackage = new LotTransactionPackage()
                {
                    Key = transactionKey,
                    PackageNo = p.PackageNo,
                    Editor = p.Creator,
                    EditTime = now
                };
                lstLotTransactionPackageForInsert.Add(transPackage);
                //this.LotTransactionPackageDataEngine.Insert(transPackage);
                #endregion

                #region //有附加参数记录附加参数数据。
                if (p.Paramters != null && p.Paramters.ContainsKey(lotNumber))
                {
                    foreach (TransactionParameter tp in p.Paramters[lotNumber])
                    {
                        LotTransactionParameter lotParamObj = new LotTransactionParameter()
                        {
                            Key = new LotTransactionParameterKey()
                            {
                                TransactionKey = transactionKey,
                                ParameterName = tp.Name,
                                ItemNo = tp.Index,
                            },
                            ParameterValue = tp.Value,
                            Editor = p.Creator,
                            EditTime = now
                        };
                        lstLotTransactionParameterDataEngineForInsert.Add(lotParamObj);
                        //this.LotTransactionParameterDataEngine.Insert(lotParamObj);
                    }
                }
                #endregion
            }
            #endregion

            if (packageUpdate != null)
            {
                lstPackageDataForUpdate.Add(packageUpdate);
            }

            #region //若已满包，需要进行出站处理
            if (p.IsFinishPackage)
            {



            }
            #endregion

            ISession session = this.LotDataEngine.SessionFactory.OpenSession();
            ITransaction transaction = session.BeginTransaction();
            try
            {
                #region //开始事物处理

                #region //更新Package基本信息
                foreach (Package package in lstPackageDataForUpdate)
                {
                    this.PackageDataEngine.Update(package, session);
                }

                foreach (Package package in lstPackageDataForInsert)
                {
                    bool blContinue = false;
                    if (PackageDataEngine.IsExists(package.Key, session) == true)
                    {
                        blContinue = true;
                        while (blContinue)
                        {
                            strNewPackageNo = GenerateEx(lotForCreatePackageNo.Key, session);
                            blContinue = PackageDataEngine.IsExists(strNewPackageNo, session);
                        }
                        package.Key = strNewPackageNo;
                    }
                    this.PackageDataEngine.Insert(package, session);
                }

                foreach (PackageDetail packageDetail in lstPackageDetailForInsert)
                {
                    PackageDetailKey Key = new PackageDetailKey()
                    {
                        PackageNo = strNewPackageNo,
                        ObjectNumber = packageDetail.Key.ObjectNumber,
                        ObjectType = EnumPackageObjectType.Lot
                    };
                    packageDetail.Key = Key;
                    this.PackageDetailDataEngine.Insert(packageDetail, session);
                }

                foreach (LotTransactionPackage lotTransactionPackage in lstLotTransactionPackageForInsert)
                {
                    lotTransactionPackage.PackageNo = strNewPackageNo;
                    this.LotTransactionPackageDataEngine.Insert(lotTransactionPackage, session);
                }
                #endregion

                #region 更新批次LOT 的信息
                //更新批次基本信息
                foreach (Lot lot in lstLotDataEngineForUpdate)
                {
                    lot.PackageNo = strNewPackageNo;
                    this.LotDataEngine.Update(lot, session);
                }

                //更新批次LotTransaction信息,包装及拆包不需要撤销
                foreach (LotTransaction lotTransaction in lstLotTransactionForInsert)
                {

                    this.LotTransactionDataEngine.Insert(lotTransaction, session);
                }

                //更新批次TransactionHistory信息
                foreach (LotTransactionHistory lotTransactionHistory in lstLotTransactionHistoryForInsert)
                {
                    lotTransactionHistory.PackageNo = strNewPackageNo;
                    this.LotTransactionHistoryDataEngine.Insert(lotTransactionHistory, session);
                }

                //LotTransactionParameter
                foreach (LotTransactionParameter lotTransactionParameter in lstLotTransactionParameterDataEngineForInsert)
                {
                    this.LotTransactionParameterDataEngine.Insert(lotTransactionParameter, session);
                }
                #endregion

                #region //更新设备信息 , 设备的Event ,设备的Transaction
                //LotTransactionEquipment ,Equipment ,EquipmentStateEvent

                foreach (LotTransactionEquipment lotTransactionEquipment in lstLotTransactionEquipmentForUpdate)
                {
                    this.LotTransactionEquipmentDataEngine.Update(lotTransactionEquipment, session);
                }

                foreach (LotTransactionEquipment lotTransactionEquipment in lstLotTransactionEquipmentForInsert)
                {
                    this.LotTransactionEquipmentDataEngine.Insert(lotTransactionEquipment, session);
                }

                foreach (Equipment equipment in lstEquipmentForUpdate)
                {
                    this.EquipmentDataEngine.Update(equipment, session);
                }

                foreach (EquipmentStateEvent equipmentStateEvent in lstEquipmentStateEventForInsert)
                {
                    this.EquipmentStateEventDataEngine.Insert(equipmentStateEvent, session);
                }
                #endregion

                result.ObjectNo = strNewPackageNo;

                transaction.Commit();
                session.Close();
                #endregion
            }
            catch (Exception err)
            {
                transaction.Rollback();
                session.Close();
                result.Code = 1000;
                result.Message += string.Format(StringResource.Error, err.Message);
                result.Detail = err.ToString();
                return result;
            }
            return result;
        }
        
    }
}
