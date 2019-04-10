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
using ServiceCenter.MES.Service.WIP.Resources;
using ServiceCenter.Model;
using ServiceCenter.MES.DataAccess.Interface.EMS;
using ServiceCenter.MES.Model.EMS;

namespace ServiceCenter.MES.Service.WIP.ServiceExtensions
{
    /// <summary>
    /// 扩展批次出站，进行报废记录操作。
    /// </summary>
    class LotTrackOutForScrap : ILotTrackOut
    {
        /// <summary>
        /// 批次报废记录操作契约对象。
        /// </summary>
        public ILotScrapContract LotScrap { get; set; }


        /// <summary>
        /// 在批次出站时，进行报废记录操作。
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
            if (LotScrap != null && p.ScrapReasonCodes!=null && p.ScrapReasonCodes.Count>0)
            {
                ScrapParameter dp = new ScrapParameter()
                {
                    Creator = p.Creator,
                    LotNumbers = p.LotNumbers,
                    OperateComputer = p.OperateComputer,
                    Operator = p.Operator,
                    ReasonCodes = p.ScrapReasonCodes,
                    Remark = p.Remark,
                    ShiftName = p.ShiftName
                };
                result = this.LotScrap.Scrap(dp);
            }*/

            return result;
        }

    }
}
