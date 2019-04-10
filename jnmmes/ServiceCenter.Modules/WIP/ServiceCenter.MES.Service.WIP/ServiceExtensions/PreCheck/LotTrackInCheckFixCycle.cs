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
    /// 扩展批次进站前检查，用于检查固化周期符合进站要求。
    /// </summary>
    class LotTrackInCheckFixCycle : ILotTrackCheck
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
        /// 工单产品数据访问类。
        /// </summary>
        public IWorkOrderProductDataEngine WorkOrderProductDataEngine
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

        public MethodReturnResult Check(TrackParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };
            DateTime now = DateTime.Now;
            //判断工序是否配置了进行固化时间检查
            RouteOperationAttribute attr = this.RouteOperationAttributeDataEngine.Get(new RouteOperationAttributeKey()
            {
                RouteOperationName = p.RouteOperationName,
                AttributeName = "IsCheckFixCycle"
            });
            bool isCheck = false;
            if (attr != null)
            {
                bool.TryParse(attr.Value, out isCheck);
            }
            //进行固化时间检查。
            if (isCheck)
            {
                //获取批次数据
                Lot obj = this.LotDataEngine.GetByLotNumber(p.LotNumber);
                if (obj != null && obj.StartWaitTime != null)
                {
                    WorkOrderProduct wopObj = WorkOrderProductDataEngine.Get(obj.OrderNumber, obj.PartNumber);
                    if (wopObj.FixCycle != null)
                    {
                        //当前时间-等待进站时间>=固化周期
                        double curCycle = (now - obj.StartWaitTime.Value).TotalMinutes;
                        if (curCycle < wopObj.FixCycle.Value)
                        {
                            result.Code = 1302;
                            result.Message = String.Format(StringResource.TrackIn_FixCycleIsNotReach,
                                                           p.LotNumber,curCycle, wopObj.FixCycle);
                        }
                    }
                }
            }
            return result;
        }
    }
}
