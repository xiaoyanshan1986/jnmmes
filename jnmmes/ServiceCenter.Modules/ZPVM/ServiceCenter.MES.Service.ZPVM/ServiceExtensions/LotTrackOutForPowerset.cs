using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.DataAccess.Interface.WIP;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Service.Contract.WIP;
using ServiceCenter.MES.Service.ZPVM.Resources;
using ServiceCenter.Model;
using ServiceCenter.Common;
using ServiceCenter.MES.DataAccess.Interface.FMM;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.DataAccess.Interface.ZPVM;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.MES.DataAccess.Interface.PPM;
using ServiceCenter.MES.Model.PPM;

namespace ServiceCenter.MES.Service.ZPVM.ServiceExtensions
{
    /// <summary>
    /// 扩展批次出站，进行批次分档。
    /// </summary>
    class LotTrackOutForPowerset : ILotTrackOut
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
        /// 工步属性数据访问类。
        /// </summary>
        public IRouteStepAttributeDataEngine RouteStepAttributeDataEngine
        {
            get;
            set;
        }
        /// <summary>
        /// 批次IV测试数据数据访问类。
        /// </summary>
        public IIVTestDataDataEngine IVTestDataDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 工单规则数据访问类。
        /// </summary>
        public IWorkOrderRuleDataEngine WorkOrderRuleDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 工单产品数据访问类。
        /// </summary>
        public IWorkOrderProductDataEngine WorkOrderProductDataEngine
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
        /// 工单分档明细规则数据访问类。
        /// </summary>
        public IWorkOrderPowersetDetailDataEngine WorkOrderPowersetDetailDataEngine
        {
            get;
            set;
        }
        /// <summary>
        /// 工单衰减规则数据访问类。
        /// </summary>
        public IWorkOrderDecayDataEngine WorkOrderDecayDataEngine
        {
            get;
            set;
        }
        /// <summary>
        /// 衰减数据访问类。
        /// </summary>
        public IDecayDataEngine DecayDataEngine
        {
            get;
            set;
        }
        /// <summary>
        /// 工单控制对象规则数据访问类。
        /// </summary>
        public IWorkOrderControlObjectDataEngine WorkOrderControlObjectDataEngine
        {
            get;
            set;
        }
        /// <summary>
        /// 在批次出站时进行分档。
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public MethodReturnResult Execute(TrackOutParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };
            /*
            foreach(string lotNumber in p.LotNumbers)
            {
                Lot lotObj = this.LotDataEngine.Get(lotNumber);

                #region TrackoutForPowerset
                //获取工步属性数据。
                RouteStepAttributeKey key = new RouteStepAttributeKey()
                {
                    RouteName = lotObj.RouteName,
                    RouteStepName = lotObj.RouteStepName,
                    AttributeName = "IsExecutePowerset"
                };
                RouteStepAttribute rsa = this.RouteStepAttributeDataEngine.Get(key);
                bool isExecute = false;
                //需要进行分档。
                if(rsa!=null
                    && bool.TryParse(rsa.Value, out isExecute)
                    && isExecute)
                {
                    #region  //判断IV测试数据是否存在。
                    PagingConfig cfg=new PagingConfig(){
                        PageNo=0,
                        PageSize=1,
                        Where=string.Format("Key.LotNumber='{0}' AND IsDefault=1",lotNumber),
                        OrderBy="Key.TestTime Desc"
                    };
                    IList<IVTestData> lstTestData = this.IVTestDataDataEngine.Get(cfg);
                    if (lstTestData.Count == 0)
                    {
                        result.Code = 2000;
                        result.Message = string.Format("批次（{0}）IV测试数据不存在，请确认。", lotNumber);
                        return result;
                    }
                    #endregion

                    IVTestData testData = lstTestData[0].Clone() as IVTestData;
                    Lot lot = lotObj.Clone() as Lot;

                    //获取工单产品设置。
                    cfg.Where = string.Format(@"Key.OrderNumber='{0}'"
                                              , lot.OrderNumber);
                    cfg.OrderBy = "ItemNo";
                    IList<WorkOrderProduct> lstWorkOrderProduct = this.WorkOrderProductDataEngine.Get(cfg);
                    StringBuilder sbMessage = new StringBuilder();
                    bool bSuccess = false;

                    for (int i = 0; i < lstWorkOrderProduct.Count;i++ )
                    {
                        lot.MaterialCode = lstWorkOrderProduct[i].Key.MaterialCode;

                        sbMessage.AppendFormat("检查批次（{0}）工单（{1}:{2}）分档规则要求。\n"
                                                       , lot.Key
                                                       , lot.OrderNumber
                                                       , lot.MaterialCode);

                        #region //进行衰减。
                        //获取工单衰减规则。
                        cfg.Where = string.Format(@"Key.OrderNumber='{0}' 
                                                    AND Key.MaterialCode='{1}' 
                                                    AND Key.MinPower<='{2}'
                                                    AND Key.MaxPower>='{2}'
                                                    AND IsUsed=1"
                                                   , lot.OrderNumber
                                                   , lot.MaterialCode
                                                   , testData.PM
                                                   , testData.PM);
                        cfg.OrderBy = "Key.MinPower";
                        //进行衰减。
                        IList<WorkOrderDecay> lstWorkOrderDecay = this.WorkOrderDecayDataEngine.Get(cfg);
                        if (lstWorkOrderDecay.Count > 0)
                        {
                            cfg.Where = string.Format("Key.Code='{0}' AND IsUsed=1", lstWorkOrderDecay[0].DecayCode);
                            cfg.OrderBy = "Key";
                            IList<Decay> lstDecay = this.DecayDataEngine.Get(cfg);
                            foreach (Decay item in lstDecay)
                            {
                                //根据功率计算出衰减系数。
                                double rate = 1;
                                if (item.Type == EnumDecayType.Aim)
                                {
                                    rate = item.Value / testData.PM;
                                }
                                else
                                {
                                    rate = item.Value;
                                }
                                //根据衰减系数计算实际功率值
                                switch (item.Key.Object)
                                {
                                    case EnumPVMTestDataType.PM:
                                        testData.CoefPM = testData.PM * rate;
                                        break;
                                    case EnumPVMTestDataType.FF:
                                        testData.CoefFF = testData.FF * rate;
                                        break;
                                    case EnumPVMTestDataType.IPM:
                                        testData.CoefIPM = testData.IPM * rate;
                                        break;
                                    case EnumPVMTestDataType.ISC:
                                        testData.CoefISC = testData.ISC * rate;
                                        break;
                                    case EnumPVMTestDataType.VOC:
                                        testData.CoefVOC = testData.VOC * rate;
                                        break;
                                    case EnumPVMTestDataType.VPM:
                                        testData.CoefVPM = testData.VPM * rate;
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                        #endregion

                        #region //判断功率是否符合工单功率范围要求。
                        //获取工单规则。
                        WorkOrderRule wor = this.WorkOrderRuleDataEngine.Get(new WorkOrderRuleKey()
                        {
                            OrderNumber = lot.OrderNumber,
                            MaterialCode = lot.MaterialCode
                        });
                        if (wor != null)
                        {
                            testData.CoefPM = Math.Round(testData.CoefPM, wor.PowerDegree, MidpointRounding.AwayFromZero);
                        }
                        if (wor != null
                            && (testData.CoefPM < wor.MinPower || testData.CoefPM > wor.MaxPower))
                        {
                            sbMessage.AppendFormat("批次（{0}）功率（{1}）不符合工单（{2}:{3}）功率范围（{4}-{5}）要求。\n"
                                                    , lot.Key
                                                    , testData.CoefPM
                                                    , lot.OrderNumber
                                                    , lot.MaterialCode
                                                    , wor.MinPower
                                                    , wor.MaxPower);
                            continue;
                        }
                        #endregion

                        #region //判断是否设置并符合控制参数要求。
                        cfg.IsPaging = false;
                        cfg.Where = string.Format(@"Key.OrderNumber='{0}' 
                                                 AND Key.MaterialCode='{1}'
                                                 AND IsUsed=1"
                                                 , lot.OrderNumber
                                                 , lot.MaterialCode);
                        cfg.OrderBy = "Key";
                        IList<WorkOrderControlObject> lstWorkOrderControlObject = this.WorkOrderControlObjectDataEngine.Get(cfg);
                        bool bCheckControlObject = true;
                        foreach (WorkOrderControlObject item in lstWorkOrderControlObject)
                        {
                            double value = double.MinValue;
                            switch (item.Key.Object)
                            {
                                case EnumPVMTestDataType.PM:
                                    value = testData.CoefPM;
                                    break;
                                case EnumPVMTestDataType.FF:
                                    value = testData.CoefFF;
                                    break;
                                case EnumPVMTestDataType.IPM:
                                    value = testData.CoefIPM;
                                    break;
                                case EnumPVMTestDataType.ISC:
                                    value = testData.CoefISC;
                                    break;
                                case EnumPVMTestDataType.VOC:
                                    value = testData.CoefVOC;
                                    break;
                                case EnumPVMTestDataType.VPM:
                                    value = testData.CoefVPM;
                                    break;
                                case EnumPVMTestDataType.CTM:
                                    value = testData.CTM;
                                    break;
                                default:
                                    break;
                            }
                            //控制参数检查。
                            if (value != double.MinValue
                                && CheckControlObject(item.Key.Type, value, item.Value) == false)
                            {
                                sbMessage.AppendFormat("批次（{0}）{1} ({4})不符合工单（{5}:{6}）控制对象（{4}{2}{3}）要求。\n"
                                                        , lot.Key
                                                        , item.Key.Object.GetDisplayName()
                                                        , item.Key.Type
                                                        , item.Value
                                                        , value
                                                        , lot.OrderNumber
                                                        , lot.MaterialCode);
                                bCheckControlObject=false;
                                break;
                            }
                        }
                        if (bCheckControlObject==false)
                        {
                            continue;
                        }
                        #endregion

                        #region //进行分档。
                        cfg.IsPaging = true;
                        cfg.Where = string.Format(@"Key.OrderNumber='{0}' 
                                                AND Key.MaterialCode='{1}'
                                                AND MinValue<='{2}'
                                                AND MaxValue>'{2}'
                                                AND IsUsed=1"
                                               , lot.OrderNumber
                                               , lot.MaterialCode
                                               , testData.CoefPM);
                        cfg.OrderBy = "Key";
                        IList<WorkOrderPowerset> lstWorkOrderPowerset = this.WorkOrderPowersetDataEngine.Get(cfg);
                        if (lstWorkOrderPowerset == null || lstWorkOrderPowerset.Count == 0)
                        {
                            sbMessage.AppendFormat("批次（{0}）功率({1})不符合工单({2}：{3})分档规则要求。\n"
                                                    , lot.Key
                                                    , testData.CoefPM
                                                    , lot.OrderNumber
                                                    , lot.MaterialCode);
                            continue;
                        }
                        WorkOrderPowerset ps = lstWorkOrderPowerset[0];
                        testData.PowersetCode = ps.Key.Code;
                        testData.PowersetItemNo = ps.Key.ItemNo;
                        //需要进行子分档
                        if (ps.SubWay != EnumPowersetSubWay.None)
                        {
                            double value = double.MinValue;
                            //电流子分档。
                            if (ps.SubWay == EnumPowersetSubWay.ISC)
                            {
                                value = testData.CoefISC;
                            }
                            else if (ps.SubWay == EnumPowersetSubWay.VOC)
                            {
                                value = testData.CoefVOC;
                            }
                            else if (ps.SubWay == EnumPowersetSubWay.IPM)
                            {
                                value = testData.CoefIPM;
                            }
                            else if (ps.SubWay == EnumPowersetSubWay.VPM)
                            {
                                value = testData.CoefVPM;
                            }
                            cfg.Where = string.Format(@"Key.OrderNumber='{0}' 
                                                AND Key.MaterialCode='{1}'
                                                AND Key.Code='{3}'
                                                AND Key.ItemNo='{4}'
                                                AND MinValue<='{2}'
                                                AND MaxValue>'{2}'
                                                AND IsUsed=1"
                                                , lot.OrderNumber
                                                , lot.MaterialCode
                                                , value
                                                , ps.Key.Code
                                                , ps.Key.ItemNo);
                            cfg.OrderBy = "Key";
                            IList<WorkOrderPowersetDetail> lstWorkOrderPowersetDetail = this.WorkOrderPowersetDetailDataEngine.Get(cfg);
                            if (lstWorkOrderPowersetDetail.Count > 0)
                            {
                                testData.PowersetSubCode = lstWorkOrderPowersetDetail[0].Key.SubCode;
                            }
                        }
                        #endregion

                        sbMessage.AppendFormat("批次（{0}）符合工单（{1}:{2}）分档规则<font size='20' color='red'>({3}-{4})</font>要求。"
                                                , lot.Key
                                                , lot.OrderNumber
                                                , lot.MaterialCode
                                                , ps.PowerName
                                                , testData.PowersetSubCode);

                        bSuccess = true;
                        break;
                    }
                    result.Message = sbMessage.ToString();
                    //没有找到符合要求的工单规则。
                    if (bSuccess==false)
                    {
                        result.Code = 2000;
                        return result;
                    }
                    //更新批次数据
                    lot.Editor = p.Creator;
                    lot.EditTime = DateTime.Now;
                    this.LotDataEngine.Update(lot);
                    //更新测试数据。
                    testData.Editor = p.Creator;
                    testData.EditTime = DateTime.Now;
                    this.IVTestDataDataEngine.Update(testData);
                }
                #endregion
            }*/
            return result;
        }

        public bool CheckControlObject(string type,double value,double controlValue)
        {
            switch (type)
            {
                case ">":
                    return value > controlValue;
                case "<":
                    return value < controlValue;
                case "=":
                case "==":
                    return value == controlValue;
                case ">=":
                    return value >= controlValue;
                case "<=":
                    return value <= controlValue;
                case "<>":
                case "!=":
                    return value != controlValue;
                default:
                    break;
            }
            return false;
        }
    }
}
