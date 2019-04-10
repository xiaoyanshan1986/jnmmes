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
using ServiceCenter.MES.Service.WIP.Resource;
using ServiceCenter.Model;

namespace ServiceCenter.MES.Service.WIP.LotTrackInServiceExtend
{
    /// <summary>
    /// 扩展批次进站前检查，用于检查恒温时间是否符合进站要求。
    /// </summary>
    class LotTrackInCheckConstantTemperatureCycle : ILotTrackCheck
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
        /// 工序属性数据访问类。
        /// </summary>
        public IRouteOperationAttributeDataEngine RouteOperationAttributeDataEngine
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

        public MethodReturnResult Check(TrackParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };
            DateTime now = DateTime.Now;
            //判断工序是否配置了进行恒温时间检查
            RouteOperationAttribute attr = this.RouteOperationAttributeDataEngine.Get(new RouteOperationAttributeKey()
            {
                RouteOperationName = p.RouteOperationName,
                AttributeName = "IsCheckConstantTemperatureCycle"
            });
           
            bool isCheck = false;
            if (attr != null)
            {
                bool.TryParse(attr.Value, out isCheck);
            }
            //进行恒温时间检查。
            if (isCheck)
            {
                //获取批次数据
                Lot obj = this.LotDataEngine.GetByLotNumber(p.LotNumber);
                if (obj != null && obj.StartWaitTime != null)
                {
                    WorkOrderProduct wopObj = WorkOrderProductDataEngine.Get(obj.OrderNumber, obj.PartNumber);
                    if (wopObj.ConstantTemperatureCycle != null)
                    {
                        //当前时间-等待进站时间>=恒温周期
                        double curCycle = (now - obj.StartWaitTime.Value).TotalMinutes;
                        if (curCycle < wopObj.ConstantTemperatureCycle.Value)
                        {
                            result.Code = 1303;
                            result.Message = String.Format(StringResource.TrackIn_ConstantTemperatureCycleIsNotReach,
                                                           p.LotNumber,curCycle, wopObj.FixCycle);
                        }
                    }
                }
            }
            return result;
        }
    }
}
