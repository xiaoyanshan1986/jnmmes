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
    /// 扩展批次包装，进行出站操作。
    /// </summary>
    class LotPackageForTrackOut : ILotPackage
    {
        /// <summary>
        /// 批次出站操作契约对象。
        /// </summary>
        public ILotTrackOutContract LotTrackOut { get; set; }
        /// <summary>
        /// 批次数据访问类。
        /// </summary>
        public ILotDataEngine LotDataEngine
        {
            get;
            set;
        }
        /// <summary>
        /// 包装明细数据访问类。
        /// </summary>
        public IPackageDetailDataEngine PackageDetailDataEngine { get; set; }

        /// <summary>
        /// 在批次包装时，进行批次出站操作。
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public MethodReturnResult Execute(PackageParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };
            /*
            if (LotTrackOut != null && p.IsFinishPackage==true)
            {
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging=false,
                    Where=string.Format("Key.PackageNo='{0}'",p.PackageNo)
                };
                IList<PackageDetail> lstPackageDetail = this.PackageDetailDataEngine.Get(cfg);

                if (lstPackageDetail!=null && lstPackageDetail.Count > 0)
                {
                    TrackOutParameter top = new TrackOutParameter()
                    {
                        Creator = p.Creator,
                        LotNumbers = new List<string>(),
                        OperateComputer = p.OperateComputer,
                        Operator = p.Operator,
                        LineCode = p.LineCode,
                        Paramters = p.Paramters,
                        RouteOperationName = p.RouteOperationName,
                        Remark = p.Remark,
                        EquipmentCode=p.EquipmentCode,
                        ShiftName = p.ShiftName
                    };

                    foreach (PackageDetail item in lstPackageDetail)
                    {
                        Lot obj = this.LotDataEngine.Get(item.Key.ObjectNumber);
                        if (obj == null)
                        {
                            continue;
                        }
                        //批次处于等待进站状态。
                        if (obj.StateFlag == EnumLotState.WaitTrackOut)
                        {
                            top.LotNumbers.Add(obj.Key);
                        }
                    }

                    if (top.LotNumbers.Count > 0)
                    {
                        result = LotTrackOut.TrackOut(top);
                        if (result.Code > 0)
                        {
                            return result;
                        }
                    }
                }
            }*/
            return result;
        }
    }
}
