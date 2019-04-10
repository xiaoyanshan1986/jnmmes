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
using ServiceCenter.Common;
using ServiceCenter.MES.DataAccess.Interface.EMS;
using ServiceCenter.MES.Model.EMS;

namespace ServiceCenter.MES.Service.WIP.ServiceExtensions
{
    /// <summary>
    /// 扩展批次进站，批次进站后自动出站。
    /// </summary>
    class LotTrackInForAutoTrackOut : ILotTrackIn
    {
        /// <summary>
        /// 批次定时作业数据访问对象。
        /// </summary>
        public ILotJobDataEngine LotJobDataEngine { get; set; }
        /// <summary>
        /// 批次数据访问对象。
        /// </summary>
        public ILotDataEngine LotDataEngine { get; set; }
        /// <summary>
        /// 工步属性数据访问类。
        /// </summary>
        public IRouteStepAttributeDataEngine RouteStepAttributeDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 设备数据访问对象。
        /// </summary>
        public IEquipmentDataEngine EquipmentDataEngine { get; set; }

        /// <summary>
        /// 工步数据访问对象。
        /// </summary>
        public IRouteStepDataEngine RouteStepDataEngine { get; set; }

        /// <summary>
        /// 在批次进站时，进行批次定时出站。
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public MethodReturnResult Execute(TrackInParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };
            /*
             //循环批次。
            foreach (string lotNumber in p.LotNumbers)
            {
                Lot lot = this.LotDataEngine.Get(lotNumber);
                //批次状态非等待出站 已暂停 已结束 不可用批次不自动出站。
                if (lot.StateFlag!=EnumLotState.WaitTrackOut
                    || lot.Status==EnumObjectStatus.Disabled
                    || lot.DeletedFlag==true
                    || lot.HoldFlag==true)
                {
                    return result;
                }
                //获取工步设置是否自动出站。
                RouteStepAttribute rsa = this.RouteStepAttributeDataEngine.Get(new RouteStepAttributeKey()
                {
                    RouteName=lot.RouteName,
                    RouteStepName=lot.RouteStepName,
                    AttributeName="IsAutoTrackOut"
                });
                bool isAutoTrackOut = false;
                if (rsa != null)
                {
                    bool.TryParse(rsa.Value, out isAutoTrackOut);
                }
                //不自动出站。
                if (isAutoTrackOut == false)
                {
                    return result;
                }
                //如果进站时设备为空，则选择一个默认设备出站。
                string equipmentCode = p.EquipmentCode;
                if (string.IsNullOrEmpty(equipmentCode))
                {
                    //根据工序和线别获取第一个设备。
                    PagingConfig cfg1 = new PagingConfig()
                    {
                        PageSize = 1,
                        PageNo = 0,
                        Where = string.Format(@"LineCode='{0}' 
                                                AND EXISTS ( FROM RouteOperationEquipment as p
                                                             WHERE p.Key.EquipmentCode=self.Key
                                                             AND p.Key.RouteOperationName='{1}')"
                                               ,lot.LineCode
                                               ,lot.RouteStepName)
                    };
                    IList<Equipment> lstEquipment = this.EquipmentDataEngine.Get(cfg1);
                    if (lstEquipment.Count > 0)
                    {
                        equipmentCode = lstEquipment[0].Key;
                    }
                }
                //获取工步标准时长，以定时出站。
                double seconds = 1;  //秒数。
                RouteStep rsObj=this.RouteStepDataEngine.Get(new RouteStepKey()
                {
                    RouteName = lot.RouteName,
                    RouteStepName = lot.RouteStepName
                });
                if (rsObj != null && rsObj.Duration != null)
                {
                    seconds = rsObj.Duration.Value * 60;
                }
                //不存在批次在指定工步自动出站作业。
                PagingConfig cfg = new PagingConfig()
                {
                    PageSize = 1,
                    PageNo = 0,
                    Where = string.Format(@"LotNumber='{0}' 
                                            AND CloseType=0 
                                            AND Status=1
                                            AND RouteStepName='{1}'
                                            AND Type='{2}'"
                                            , lot.Key
                                            , lot.RouteStepName
                                            , Convert.ToInt32(EnumJobType.AutoTrackOut))
                };
                IList<LotJob> lstJob = this.LotJobDataEngine.Get(cfg);
                if (lstJob.Count == 0)
                {
                    //新增批次自动出站作业。
                    LotJob job = new LotJob()
                    {
                        CloseType = EnumCloseType.None,
                        CreateTime = DateTime.Now,
                        Creator = p.Creator,
                        Editor = p.Creator,
                        EditTime = DateTime.Now,
                        EquipmentCode = equipmentCode,
                        JobName = string.Format("{0} {1}", lot.Key, lot.StateFlag.GetDisplayName()),
                        Key = Convert.ToString(Guid.NewGuid()),
                        LineCode = lot.LineCode,
                        LotNumber = lot.Key,
                        Type = EnumJobType.AutoTrackOut,
                        RouteEnterpriseName = lot.RouteEnterpriseName,
                        RouteName = lot.RouteName,
                        RouteStepName = lot.RouteStepName,
                        RunCount = 0,
                        Status = EnumObjectStatus.Available,
                        NextRunTime = DateTime.Now.AddSeconds(seconds),
                        NotifyMessage = string.Empty,
                        NotifyUser = string.Empty
                    };
                    this.LotJobDataEngine.Insert(job);
                }
                else
                {
                    LotJob jobUpdate = lstJob[0].Clone() as LotJob;
                    jobUpdate.NextRunTime = DateTime.Now.AddSeconds(seconds);
                    jobUpdate.LineCode = lot.LineCode;
                    jobUpdate.EquipmentCode = equipmentCode;
                    jobUpdate.RunCount = 0;
                    jobUpdate.Editor = p.Creator;
                    jobUpdate.EditTime = DateTime.Now;
                    this.LotJobDataEngine.Update(jobUpdate);
                }
            }
            */
            return result;
        }

    }
}
