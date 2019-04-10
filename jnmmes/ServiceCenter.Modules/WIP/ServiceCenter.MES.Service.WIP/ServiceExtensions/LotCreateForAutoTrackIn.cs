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
using ServiceCenter.MES.DataAccess.Interface.PPM;
using ServiceCenter.MES.Model.PPM;

namespace ServiceCenter.MES.Service.WIP.ServiceExtensions
{
    /// <summary>
    /// 扩展批次创建，批次创建后自动进站。
    /// </summary>
    class LotCreateForAutoTrackIn : ILotCreate
    {
        /// <summary>
        /// 批次数据访问对象。
        /// </summary>
        public ILotDataEngine LotDataEngine { get; set; }
        /// <summary>
        /// 批次定时作业数据访问对象。
        /// </summary>
        public ILotJobDataEngine LotJobDataEngine { get; set; }
        /// <summary>
        /// 工步属性数据访问类。
        /// </summary>
        public IRouteStepAttributeDataEngine RouteStepAttributeDataEngine
        {
            get;
            set;
        }
        /// <summary>
        /// 工单线别数据访问类。
        /// </summary>
        public IWorkOrderProductionLineDataEngine WorkOrderProductionLineDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 线别数据访问类。
        /// </summary>
        public IProductionLineDataEngine ProductionLineDataEngine 
        {
            get;
            set;
        }
        /// <summary>
        /// 在批次创建时，进行批次定时进站。
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public MethodReturnResult Execute(CreateParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };
            
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
                //根据工单获取线别。
                string lineCode = string.Empty;
                PagingConfig cfg = new PagingConfig()
                {
                    PageSize = 1,
                    PageNo = 0,
                    Where = string.Format(@"Key.OrderNumber='{0}'"
                                            , lot.OrderNumber)
                };
                IList<WorkOrderProductionLine> lstWorkOrderProductionLine = this.WorkOrderProductionLineDataEngine.Get(cfg);
                if (lstWorkOrderProductionLine.Count > 0)
                {
                    lineCode = lstWorkOrderProductionLine[0].Key.LineCode;
                }
                else
                {
                    cfg.Where=string.Format(@"EXISTS (FROM Location as p
                                                      WHERE p.Key=self.LocationName
                                                      AND p.ParentLocationName='{0}'
                                                      AND p.Level='{1}')"
                                             ,lot.LocationName
                                             ,Convert.ToInt32(LocationLevel.Area));
                    IList<ProductionLine> lstProductionLine = this.ProductionLineDataEngine.Get(cfg);
                    if (lstProductionLine.Count > 0)
                    {
                        lineCode = lstProductionLine[0].Key;
                    }
                }
                //不存在批次在指定工步自动进站作业。
                cfg.Where = string.Format(@"LotNumber='{0}' 
                                            AND CloseType=0 
                                            AND Status=1
                                            AND RouteStepName='{1}'
                                            AND Type='{2}'"
                                            , lot.Key
                                            , lot.RouteStepName
                                            , Convert.ToInt32(EnumJobType.AutoTrackIn));
                IList<LotJob> lstJob = this.LotJobDataEngine.Get(cfg);
                if (lstJob.Count == 0)
                {
                    //新增批次自动创建作业。
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
                        LineCode = lineCode,
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
                    jobUpdate.LineCode = lineCode;
                    jobUpdate.EquipmentCode = null;
                    jobUpdate.RunCount = 0;
                    jobUpdate.Editor = p.Creator;
                    jobUpdate.EditTime = DateTime.Now;
                    this.LotJobDataEngine.Update(jobUpdate);
                }
            }
            return result;
        }

    }
}
