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
    /// 扩展批次进站，进行批次报废操作。
    /// </summary>
    class LotTrackInScrap : ILotTrack
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
        /// 在批次进站后执行批次报废操作。
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
            Lot obj = this.LotDataEngine.Get(p.LotNumber);

            if (obj != null)
            {
                //批次报废。

                //更新预设暂停
            }
            return result;
        }
    }
}
