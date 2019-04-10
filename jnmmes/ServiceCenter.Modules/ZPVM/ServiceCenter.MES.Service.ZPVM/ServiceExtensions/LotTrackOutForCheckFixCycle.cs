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
    /// 扩展批次出站，检查固化周期。
    /// </summary>
    class LotTrackOutForCheckFixCycle : ILotTrackOutCheck
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
        /// 工单规则数据访问类。
        /// </summary>
        public IWorkOrderRuleDataEngine WorkOrderRuleDataEngine
        {
            get;
            set;
        }
        /// <summary>
        /// 在批次出站时进行固化周期检查。
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
                    AttributeName = "IsCheckFixCycle"
                });
                //如果设置固化周期检查。
                if(rs!=null)
                {
                    bool isCheckFixCycle = false;
                    bool.TryParse(rs.Value, out isCheckFixCycle);
                    //需要检查固化周期。
                    if(isCheckFixCycle)
                    {
                        DateTime dtStartProcessTime = lot.StartProcessTime.Value;
                        //获取工单固化周期（分钟）
                        WorkOrderRule wor = this.WorkOrderRuleDataEngine.Get(new WorkOrderRuleKey()
                        {
                            OrderNumber=lot.OrderNumber,
                            MaterialCode=lot.MaterialCode
                        });
                        //工单没有设置规则。
                        if (wor == null)
                        {
                            result.Code = 2000;
                            result.Message = string.Format("工单（{0}:{1}）规则没有维护，请确认。", lot.OrderNumber, lot.MaterialCode);
                            return result;
                        }
                        
                        double curFixMinutes = (DateTime.Now - dtStartProcessTime).TotalMinutes;
                        if (wor.FixCycle > curFixMinutes)
                        {
                            result.Code = 2001;
                            result.Message = string.Format("批次（{0}）已固化（{1}）分钟，不满足规则要求的（{2}）分钟，请确认。"
                                                            , lotNumber
                                                            , curFixMinutes
                                                            , wor.FixCycle);
                            return result;
                        }
                    }
                }
            }
            return result;
        }
    }
}
