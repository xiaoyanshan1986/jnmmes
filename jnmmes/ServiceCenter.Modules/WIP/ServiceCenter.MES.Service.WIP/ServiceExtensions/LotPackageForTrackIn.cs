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
    /// 扩展批次包装，进行批次进站操作。
    /// </summary>
    class LotPackageForTrackIn : ILotPackage
    {
        /// <summary>
        /// 批次进站操作契约对象。
        /// </summary>
        public ILotTrackInContract LotTrackIn { get; set; }
        /// <summary>
        /// 批次数据访问类。
        /// </summary>
        public ILotDataEngine LotDataEngine
        {
            get;
            set;
        }
        /// <summary>
        /// 在批次包装时，进行批次进站操作。
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public MethodReturnResult Execute(PackageParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };
            //if (LotTrackIn != null)
            //{
            //    foreach(string lotNumber in p.LotNumbers)
            //    {
            //        Lot obj = this.LotDataEngine.Get(lotNumber);
            //        if (obj == null)
            //        {
            //            continue;
            //        }
            //        //批次处于等待进站状态。
            //        if(obj.StateFlag==EnumLotState.WaitTrackIn)
            //        {
            //            TrackInParameter tip = new TrackInParameter()
            //            {
            //                Creator = p.Creator,
            //                LotNumbers = new List<string>(),
            //                OperateComputer = p.OperateComputer,
            //                Operator = p.Operator,
            //                EquipmentCode = p.EquipmentCode,
            //                LineCode = p.LineCode,
            //                Paramters = p.Paramters,
            //                RouteOperationName = p.RouteOperationName,
            //                Remark = p.Remark,
            //                ShiftName = p.ShiftName
            //            };
            //            tip.LotNumbers.Add(lotNumber);
            //            result = LotTrackIn.TrackIn(tip);
            //            if (result.Code > 0)
            //            {
            //                return result;
            //            }
            //        }
            //    }
            //}
            return result;
        }

    }
}
