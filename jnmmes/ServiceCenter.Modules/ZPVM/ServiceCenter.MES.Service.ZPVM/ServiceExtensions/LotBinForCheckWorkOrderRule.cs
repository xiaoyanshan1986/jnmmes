using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.DataAccess.Interface.FMM;
using ServiceCenter.MES.DataAccess.Interface.WIP;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Service.Contract.WIP;
using ServiceCenter.Model;
using ServiceCenter.MES.DataAccess.Interface.ZPVM;
using ServiceCenter.MES.Model.ZPVM;

namespace ServiceCenter.MES.Service.WIP.ServiceExtensions
{
    /// <summary>
    /// 扩展批次包装检查，进行工单包装规则检查。
    /// </summary>
    class LotBinForCheckWorkOrderRule : ILotBinCheck
    {
        /// <summary>
        /// 批次数据访问类。
        /// </summary>
        public ILotDataEngine LotDataEngine
        {
            get;
            set;
        }
        /// <summary>
        /// 批次IV测试数据访问类。
        /// </summary>
        public IIVTestDataDataEngine IVTestDataDataEngine
        {
            get;
            set;
        }
        /// <summary>
        /// 包装数据访问类。
        /// </summary>
        public IPackageDataEngine PackageDataEngine { get; set; }
        /// <summary>
        /// 包装明细数据访问类。
        /// </summary>
        public IPackageDetailDataEngine PackageDetailDataEngine { get; set; }

        /// <summary>
        /// 工单等级包装规则数据访问类。
        /// </summary>
        public IWorkOrderGradeDataEngine WorkOrderGradeDataEngine 
        {
            get;
            set;
        }

        /// <summary>
        /// 工单分档规则数据访问类。
        /// </summary>
        public IWorkOrderPowersetDataEngine WorkOrderPowersetDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 在批次包装时，进行批次进站操作。
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public MethodReturnResult Check(InBinParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            if (p.LotNumbers==null)
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

            PagingConfig cfg;
            IList<IVTestData> lstTestData;
            //获取包装记录。
            Package packageObj = null;
            if(string.IsNullOrEmpty(p.PackageNo)==false )
                packageObj = this.PackageDataEngine.Get(p.PackageNo);

            if (packageObj != null && packageObj.Quantity>0)
            {
                isCheckPackage = true;
                //获取包装明细中的第一条记录。
                cfg = new PagingConfig()
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
                lstTestData = this.IVTestDataDataEngine.Get(cfg);
                if (lstTestData != null && lstTestData.Count > 0)
                {
                    packagePowersetCode = lstTestData[0].PowersetCode;
                    packagePowersetCodeItemNo = lstTestData[0].PowersetItemNo??-1;
                    packagePowersetSubCode = lstTestData[0].PowersetSubCode;
                }
            }
            string lotNumber = p.ScanLotNumber;

           
            //获取IV测试数据。
            cfg = new PagingConfig()
            {
                PageNo = 0,
                PageSize = 1,
                Where = string.Format("Key.LotNumber='{0}' AND IsDefault=1", lotNumber)
            };
            lstTestData = this.IVTestDataDataEngine.Get(cfg);

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

                WorkOrderPowerset wop = this.WorkOrderPowersetDataEngine.Get(new WorkOrderPowersetKey()
                {
                    OrderNumber=lot.OrderNumber,
                    MaterialCode=lot.MaterialCode,
                    Code=powersetCode,
                    ItemNo=powersetCodeItemNo
                });


                string msg = string.Format("组件（{4}）等级：{0} 花色:{1} 档位：{2} 子档位：{3}"
                                            , lot.Grade
                                            , lot.Color
                                            , wop != null ? string.Format("{0}({1})",wop.PowerName,powersetCode + ":" + powersetCodeItemNo) : powersetCode + ":" + powersetCodeItemNo
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
            
            return result;
        }

    }
}
