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
    /// 扩展批次进站，进行设备状态切换操作。
    /// </summary>
    class LotTrackInForEquipmentState : ILotTrackIn,ILotTrackInCheck
    {
        /// <summary>
        /// 设备数据访问类。
        /// </summary>
        public IEquipmentDataEngine EquipmentDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 设备状态数据访问类。
        /// </summary>
        public IEquipmentStateDataEngine EquipmentStateDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 设备状态切换数据访问类。
        /// </summary>
        public IEquipmentChangeStateDataEngine EquipmentChangeStateDataEngine
        {
            get;
            set;
        }
        /// <summary>
        /// 设备状态事件数据访问类。
        /// </summary>
        public IEquipmentStateEventDataEngine EquipmentStateEventDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 在批次进站时进行检查。
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public MethodReturnResult Check(TrackInParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };
            if (string.IsNullOrEmpty(p.EquipmentCode))
            {
                return result;
            }
            //获取设备数据
            Equipment e = this.EquipmentDataEngine.Get(p.EquipmentCode??string.Empty);
            if (e == null)
            {
                return result;
            }
            //如果设备状态在不可运行状态，返回错误。
            //获取设备当前状态。
            EquipmentState es = this.EquipmentStateDataEngine.Get(e.StateName);
            if (es == null)
            {
                return result;
            }
            //如果设备状态不是“待产” 和 “运行”
            if (es.Type != EnumEquipmentStateType.Lost 
                && es.Type != EnumEquipmentStateType.Run
                && es.Type != EnumEquipmentStateType.Test)
            {
                result.Code = 1200;
                result.Message = String.Format("设备（{0}）状态（{1}）不可用。", e.Key,e.StateName);
                return result;
            }
            return result;
        }
        /// <summary>
        /// 在批次进站时，更新对应的设备数据。
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
            if (string.IsNullOrEmpty(p.EquipmentCode))
            {
                return result;
            }
            DateTime now=DateTime.Now;
            //获取设备数据。
            Equipment e = this.EquipmentDataEngine.Get(p.EquipmentCode);
            if (e == null)
            {
                return result;
            }
            //获取设备当前状态。
            EquipmentState es = this.EquipmentStateDataEngine.Get(e.StateName);
            if (es == null)
            {
                return result;
            }
            //获取设备RUN的主键
            EquipmentState runState = this.EquipmentStateDataEngine.Get("RUN");
            //获取设备当前状态->RUN的状态切换数据。
            EquipmentChangeState ecsToRun = this.EquipmentChangeStateDataEngine.Get(es.Key,runState.Key);
            if (ecsToRun != null)
            {
                //更新父设备状态。
                if (!string.IsNullOrEmpty(e.ParentEquipmentCode))
                {
                    Equipment ep = this.EquipmentDataEngine.Get(e.ParentEquipmentCode);
                    if (ep != null)
                    {
                        Equipment epUpdate = ep.Clone() as Equipment;
                        //更新设备状态。
                        epUpdate.StateName = runState.Key;
                        epUpdate.ChangeStateName = ecsToRun.Key;
                        this.EquipmentDataEngine.Update(epUpdate);
                       
                        //新增设备状态事件数据
                        EquipmentStateEvent newStateEvent = new EquipmentStateEvent()
                        {
                            Key=Guid.NewGuid().ToString(),
                            CreateTime=now,
                            Creator=p.Creator,
                            Description=p.Remark,
                            Editor=p.Creator,
                            EditTime=now,
                            EquipmentChangeStateName = ecsToRun.Key,
                            EquipmentCode=e.ParentEquipmentCode,
                            EquipmentFromStateName = es.Key,
                            EquipmentToStateName=runState.Key,
                            IsCurrent=true
                        };
                        this.EquipmentStateEventDataEngine.Insert(newStateEvent);
                    }
                }
                //更新设备状态。
                Equipment eUpdate = e.Clone() as Equipment;
                eUpdate.StateName = runState.Key;
                eUpdate.ChangeStateName = ecsToRun.Key;
                this.EquipmentDataEngine.Update(eUpdate);
                //新增设备状态事件数据
                EquipmentStateEvent stateEvent = new EquipmentStateEvent()
                {
                    Key = Guid.NewGuid().ToString(),
                    CreateTime = now,
                    Creator = p.Creator,
                    Description = p.Remark,
                    Editor = p.Creator,
                    EditTime = now,
                    EquipmentChangeStateName = ecsToRun.Key,
                    EquipmentCode = e.Key,
                    EquipmentFromStateName = es.Key,
                    EquipmentToStateName = runState.Key,
                    IsCurrent = true
                };
                this.EquipmentStateEventDataEngine.Insert(stateEvent);
            }*/
            return result;
        }

    }
}
