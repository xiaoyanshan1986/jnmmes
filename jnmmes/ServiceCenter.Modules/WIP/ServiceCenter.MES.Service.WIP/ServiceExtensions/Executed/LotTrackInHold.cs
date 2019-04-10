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
    /// 扩展批次进站，进行暂停批次操作。
    /// </summary>
    class LotTrackInHold : ILotTrack
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
        /// 批次预设暂停数据访问类。
        /// </summary>
        public ILotFutureholdDataEngine LotFutureholdDataEngine
        {
            get;
            set;
        }


        
        /// <summary>
        /// 在批次进站后执行暂停。
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public MethodReturnResult Execute(TrackParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };
            DateTime now=DateTime.Now;
            //获取批次数据
            Lot obj = this.LotDataEngine.GetByLotNumber(p.LotNumber);

            LotFuturehold futureHold = this.LotFutureholdDataEngine
                                           .Get(obj.Key, obj.StepKey, "TRACKIN");
            if (futureHold != null)
            {
                //暂停批次。

                //更新预设暂停
                futureHold.Status = 0;
                futureHold.Editor = p.Operator;
                futureHold.EditTime=now;
                this.LotFutureholdDataEngine.Update(futureHold);
            }
            return result;
        }
    }
}
