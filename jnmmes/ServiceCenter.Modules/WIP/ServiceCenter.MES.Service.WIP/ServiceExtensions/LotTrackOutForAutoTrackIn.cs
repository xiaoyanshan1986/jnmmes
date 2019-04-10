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
    /// 扩展批次出站，批次出站后自动进站。
    /// </summary>
    class LotTrackOutForAutoTrackIn : ILotTrackOut
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
        /// 在批次出站时，进行批次定时进站。
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
             //循环批次。
            foreach (string lotNumber in p.LotNumbers)
            {
                Lot lot = this.LotDataEngine.Get(lotNumber);
                //批次状态非等待进站 已暂停 已结束 不可用批次不自动进站。
                if (lot.StateFlag != EnumLotState.WaitTrackIn
                    || lot.Status==EnumObjectStatus.Disabled
                    || lot.DeletedFlag==true
                    || lot.HoldFlag==true)
                {
                    return result;
                }
                //获取工步设置是否自动进站。
                RouteStepAttribute rsa = this.RouteStepAttributeDataEngine.Get(new RouteStepAttributeKey()
                {
                    RouteName=lot.RouteName,
                    RouteStepName=lot.RouteStepName,
                    AttributeName="IsAutoTrackIn"
                });
               
                bool isAutoTrackIn = false;
                if (rsa != null)
                {
                    bool.TryParse(rsa.Value, out isAutoTrackIn);
                }
                //不自动进站。
                if (isAutoTrackIn==false)
                {
                    return result;
                }

                //不存在批次在指定工步自动进站作业。
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
                                            , Convert.ToInt32(EnumJobType.AutoTrackIn))
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
                        EquipmentCode = null,
                        JobName = string.Format("{0} {1}", lot.Key, lot.StateFlag.GetDisplayName()),
                        Key = Convert.ToString(Guid.NewGuid()),
                        LineCode = lot.LineCode,
                        LotNumber = lot.Key,
                        Type = EnumJobType.AutoTrackIn,
                        RouteEnterpriseName = lot.RouteEnterpriseName,
                        RouteName = lot.RouteName,
                        RouteStepName = lot.RouteStepName,
                        RunCount = 0,
                        Status = EnumObjectStatus.Available,
                        NextRunTime = DateTime.Now.AddSeconds(1),
                        NotifyMessage = string.Empty,
                        NotifyUser = string.Empty
                    };
                    this.LotJobDataEngine.Insert(job);
                }
                else
                {
                    LotJob jobUpdate = lstJob[0].Clone() as LotJob;
                    jobUpdate.NextRunTime = DateTime.Now.AddSeconds(1);
                    jobUpdate.LineCode = lot.LineCode;
                    jobUpdate.EquipmentCode = null;
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
