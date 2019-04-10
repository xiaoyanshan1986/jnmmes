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
using ServiceCenter.MES.DataAccess.Interface.FMM;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.DataAccess.Interface.ZPVM;
using ServiceCenter.MES.Model.ZPVM;

namespace ServiceCenter.MES.Service.ZPVM.ServiceExtensions
{
    /// <summary>
    /// 扩展批次出站，检查IV测试数据是否存在、IV测试数据校准周期，校准板类型是否匹配等。
    /// </summary>
    class LotTrackOutForCheckIVTestData : ILotTrackOutCheck
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
        /// 在批次出站时进行IV测试数据检查。
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public MethodReturnResult Check(TrackOutParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };
            foreach(string lotNumber in p.LotNumbers)
            {
                Lot lot = this.LotDataEngine.Get(lotNumber);
                //获取工步属性数据。
                RouteStepAttribute rs = this.RouteStepAttributeDataEngine.Get(new RouteStepAttributeKey()
                {
                    RouteName=lot.RouteName,
                    RouteStepName=lot.RouteStepName,
                    AttributeName = "IsCheckIVTestData"
                });
                IVTestData testData = null;
                bool isCheck = false;

                //需要检查IV测试数据。
                if(rs!=null
                    && bool.TryParse(rs.Value, out isCheck)
                    && isCheck)
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
                        result.Message = string.Format("批次（{0}）IV测试数据不存在，请确认。", lot.Key);
                        return result;
                    }
                    testData = lstTestData[0];
                    #endregion

                    WorkOrderRule wor = null;

                    #region //检查校准板类型。
                    rs = this.RouteStepAttributeDataEngine.Get(new RouteStepAttributeKey()
                    {
                        RouteName = lot.RouteName,
                        RouteStepName = lot.RouteStepName,
                        AttributeName = "IsCheckCalibrationType"
                    });
                    if (rs != null
                       && bool.TryParse(rs.Value, out isCheck)
                       && isCheck)
                    {
                        //获取工单规则。
                        wor = this.WorkOrderRuleDataEngine.Get(new WorkOrderRuleKey()
                        {
                            OrderNumber = lot.OrderNumber,
                            MaterialCode = lot.MaterialCode
                        });
                        //工单没有设置规则。
                        if (wor == null)
                        {
                            result.Code = 2001;
                            result.Message = string.Format("工单（{0}:{1}）规则没有维护，请确认。", lot.OrderNumber, lot.MaterialCode);
                            return result;
                        }
                        //IV测试数据使用的校准板类型是否正确。
                        if (testData.CalibrationNo==null 
                            || !testData.CalibrationNo.StartsWith(wor.CalibrationType))
                        {
                            result.Code = 2002;
                            result.Message = string.Format("批次（{0}）使用的校准板（{1}）和工单规则设置的校准板类型（{2}）不匹配，请确认。"
                                                            , lotNumber
                                                            , testData.CalibrationNo
                                                            , wor.CalibrationType);
                            return result;
                        }
                    }

                    rs = this.RouteStepAttributeDataEngine.Get(new RouteStepAttributeKey()
                    {
                        RouteName = lot.RouteName,
                        RouteStepName = lot.RouteStepName,
                        AttributeName = "IsCheckCalibrationCycle"
                    });
                    #endregion

                    #region //检查校准周期。
                    if (rs != null
                        && bool.TryParse(rs.Value, out isCheck)
                        && isCheck)
                    {
                        if (wor == null)
                        {
                            //获取工单规则。
                            wor = this.WorkOrderRuleDataEngine.Get(new WorkOrderRuleKey()
                            {
                                OrderNumber = lot.OrderNumber,
                                MaterialCode = lot.MaterialCode
                            });
                            //工单没有设置规则。
                            if (wor == null)
                            {
                                result.Code = 2001;
                                result.Message = string.Format("工单（{0}:{1}）规则没有维护，请确认。", lot.OrderNumber, lot.MaterialCode);
                                return result;
                            }
                        }
                        //IV测试数据的校准时间是否满足校准周期要求。
                        DateTime calibrateTime = testData.CalibrateTime ?? DateTime.MinValue;
                        double calibrateCycle = (testData.Key.TestTime - calibrateTime).TotalMinutes;
                        if (calibrateCycle > wor.CalibrationCycle)
                        {
                            result.Code = 2002;
                            result.Message = string.Format("批次（{0}）校准时间（{1:yyyy-MM-dd HH:mm:ss}）超过工单规则设置的校准周期（{2}）分钟，请确认。"
                                                            , lotNumber
                                                            , testData.CalibrateTime
                                                            , wor.CalibrationCycle);
                            return result;
                        }
                    }
                    #endregion

                }
            }
            return result;
        }
    }
}
