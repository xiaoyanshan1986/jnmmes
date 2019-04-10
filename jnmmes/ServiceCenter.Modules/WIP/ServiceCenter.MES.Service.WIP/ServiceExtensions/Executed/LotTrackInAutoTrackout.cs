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
    /// 扩展批次进站，记录用于批次自动出站的JOB数据，以将批次自动出站。
    /// </summary>
    class LotTrackInAutoTrackout : ILotTrack
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
        /// 工步流程工步数据访问类。
        /// </summary>
        public IRouteStepDataEngine RouteStepDataEngine
        {
            get;
            set;
        }
        /// <summary>
        /// 工步流程工步属性数据访问类。
        /// </summary>
        public IRouteStepAttributeDataEngine RouteStepAttributeDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 批次作业数据访问类。
        /// </summary>
        public ILotJobDataEngine LotJobDataEngine
        {
            get;
            set;
        }
        /// <summary>
        /// 在批次进站时，进行自动过站。
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public MethodReturnResult Execute(TrackParameter p)
        {
            const string AUTOTRACKOUT="AutoTrackOut";
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };
            DateTime now=DateTime.Now;
            Lot lotObj=this.LotDataEngine.GetByLotNumber(p.LotNumber);
            RouteStep rs = this.RouteStepDataEngine.Get(new RouteStepKey
            {
                 RouteName=lotObj.RouteKey,
                 RouteStepName=lotObj.StepKey
            });
            //获取自动过站配置。
            RouteStepAttribute attr = this.RouteStepAttributeDataEngine.Get(new RouteStepAttributeKey()
            {
                RouteName=rs.Key.RouteName,
                RouteStepName = rs.Key.RouteStepName,
                AttributeName = AUTOTRACKOUT
            });
            bool isAutoTrackout = false;
            bool.TryParse(attr.Value, out isAutoTrackout);
            //允许自动过站
            if (isAutoTrackout)
            {
                //根据工序标准时间计算过站时间。
                DateTime nextRunTime = now.AddMinutes(rs.Duration);
                //新增过站JOB
                LotJob job = new LotJob()
                {
                    Key=Guid.NewGuid().ToString(),
                    LotKey=lotObj.Key,
                    LotNumber=lotObj.LotNumber,
                    WorkOrderNumber=lotObj.OrderNumber,
                    EquipmentKey=p.EquipmentKey,
                    CreateTime=now,
                    Editor=p.Operator,
                    EditTime=now,
                    LineName=p.LineName,
                    RunCount=0,
                    Status = "0",
                    RouteEnterpriseKey=lotObj.RouteEnterpriseKey,
                    RouteProcessKey=lotObj.RouteKey,
                    RouteStepKey=lotObj.StepKey,
                    NextRunTime = nextRunTime,
                    Type = "TRACKIN"
                };
                this.LotJobDataEngine.Insert(job);
            }
            return result;
        }
    }
}
