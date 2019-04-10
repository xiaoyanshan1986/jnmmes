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
    /// 扩展批次进站前检查，用于检查在线时间是否符合进站要求。
    /// </summary>
    class LotTrackInCheckOnlineCycle : ILotTrackCheck
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

        public MethodReturnResult Check(TrackParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };
            DateTime now = DateTime.Now;
            //判断工序是否配置批次最大在线时间周期（分钟）。
            RouteOperationAttribute attr = this.RouteOperationAttributeDataEngine.Get(new RouteOperationAttributeKey()
            {
                RouteOperationName = p.RouteOperationName,
                AttributeName = "CreateLotMaxCycle"
            });
            double cycle = double.MaxValue;
            if (attr != null)
            {
                double.TryParse(attr.Value, out cycle);
            }
            //进行批次在线时间检查。
            if (cycle!=double.MaxValue)
            {
                //获取批次数据
                Lot obj = this.LotDataEngine.GetByLotNumber(p.LotNumber);
                if (obj != null)
                {
                    double onlineMinutes = (now - obj.CreateTime.Value).TotalMinutes;
                    if (onlineMinutes > cycle)
                    {
                        result.Code = 1305;
                        result.Message = String.Format(StringResource.TrackIn_OnlineTimeIsTimeout,
                                                       p.LotNumber,onlineMinutes,cycle);
                    }
                }
            }
            return result;
        }
    }
}
