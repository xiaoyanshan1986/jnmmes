using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.DataAccess.Interface.WIP;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Service.Contract.WIP;
using ServiceCenter.MES.Service.ZPVM.Resources;
using ServiceCenter.Model;
using ServiceCenter.Common;
using ServiceCenter.MES.DataAccess.Interface.FMM;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.DataAccess.Interface.ZPVM;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.MES.DataAccess.Interface.PPM;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Service.Contract.ZPVM;

namespace ServiceCenter.MES.Service.ZPVM.ServiceExtensions
{
    /// <summary>
    /// 扩展IV测试数据，进行批次JOB数据记录，以自动出站。
    /// </summary>
    class IVTestDataTransferForTrackOut : IIVTestDataTransfer
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
        /// 批次作业数据访问类。
        /// </summary>
        public ILotJobDataEngine LotJobDataEngine
        {
            get;
            set;
        }
        /// <summary>
        /// 设备数据访问类。
        /// </summary>
        public IEquipmentDataEngine EquipmentDataEngine
        {
            get;
            set;
        }
        /// <summary>
        /// 工序设备数据访问类。
        /// </summary>
        public IRouteOperationEquipmentDataEngine RouteOperationEquipmentDataEngine 
        {
            get;
            set;
        }
        /// <summary>
        /// 区域数据访问类。
        /// </summary>
        public ILocationDataEngine LocationDataEngine
        {
            get;
            set;
        }
        /// <summary>
        ///扩展IV测试数据移转。
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public MethodReturnResult Execute(IVTestDataTransferParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };
            /*
            if (p == null || p.List == null || p.List.Count == 0)
            {
                return result;
            }

            foreach(IVTestData item in p.List)
            {
                try
                {
                    //IV数据是有效数据
                    if (item.IsDefault)
                    {
                        Job(item);
                    }
                }
                catch (Exception ex)
                {
                    result.Message += ex.Message+"\n";
                }
            }*/
            return result;
        }

        private void Job(IVTestData item)
        {
            //获取批次数据。
            Lot lot = this.LotDataEngine.Get(item.Key.LotNumber);
            if (lot == null  //批次数据不为NULL
                || lot.StateFlag != EnumLotState.WaitTrackOut //批次等待出站。
                || this.RouteOperationEquipmentDataEngine.IsExists(new RouteOperationEquipmentKey()
                   {
                       RouteOperationName = lot.RouteStepName,
                       EquipmentCode = item.Key.EquipmentCode
                   })==false //批次当前工序是否和设备所在工序匹配，如果匹配才进行过站。
            )
            {
                return;
            }
            //获取设备数据。
            Equipment eq = this.EquipmentDataEngine.Get(item.Key.EquipmentCode);
            if (eq == null)
            {
                return;
            }

            Location location = this.LocationDataEngine.Get(eq.LocationName);
            //设备所在车间和批次车间匹配。
            if (location == null || location.ParentLocationName != lot.LocationName)
            {
                return;
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
                    Creator = "system",
                    Editor = "system",
                    EditTime = DateTime.Now,
                    EquipmentCode = item.Key.EquipmentCode,
                    JobName = string.Format("{0} {1}", lot.Key, lot.StateFlag.GetDisplayName()),
                    Key = Convert.ToString(Guid.NewGuid()),
                    LineCode = eq.LineCode,
                    LotNumber = lot.Key,
                    Type = EnumJobType.AutoTrackOut,
                    RouteEnterpriseName = lot.RouteEnterpriseName,
                    RouteName = lot.RouteName,
                    RouteStepName = lot.RouteStepName,
                    RunCount = 0,
                    Status = EnumObjectStatus.Available,
                    NextRunTime = DateTime.Now,
                    NotifyMessage = string.Empty,
                    NotifyUser = string.Empty
                };
                this.LotJobDataEngine.Insert(job);
            }
            else
            {
                LotJob jobUpdate = lstJob[0].Clone() as LotJob;
                jobUpdate.NextRunTime = DateTime.Now;
                jobUpdate.LineCode = eq.LineCode;
                jobUpdate.EquipmentCode = eq.Key;
                this.LotJobDataEngine.Update(jobUpdate);
            }
        }
    }
}
