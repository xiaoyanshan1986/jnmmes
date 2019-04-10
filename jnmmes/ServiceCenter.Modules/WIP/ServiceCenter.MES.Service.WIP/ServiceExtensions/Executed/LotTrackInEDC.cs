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
    /// 扩展批次进站，判断批次是否需要进行数据采集，并更新相应数据。
    /// </summary>
    class LotTrackInEDC : ILotTrack
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
        /// 在批次进站后执行数据采集。
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public MethodReturnResult Execute(TrackParameter p)
        {
            const string DependSampStep = "DependSampStep";
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };
            DateTime now=DateTime.Now;

            Lot lotObj = this.LotDataEngine.GetByLotNumber(p.LotNumber);
            RouteStep rs = this.RouteStepDataEngine.Get(new RouteStepKey
            {
                RouteName = lotObj.RouteKey,
                RouteStepName = lotObj.StepKey
            });
            //获取自动过站配置。
            RouteStepAttribute dependSampStepAttr = this.RouteStepAttributeDataEngine
                                                        .Get(new RouteStepAttributeKey()
            {
                RouteStepName = rs.Key.RouteStepName,
                AttributeName =DependSampStep
            });

            string dependSampStep = dependSampStepAttr.Value;
            //获取数据抽检点设置。

            return result;
        }
    }
}
