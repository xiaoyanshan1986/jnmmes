using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using ServiceCenter.MES.DataAccess.Interface.FMM;
using ServiceCenter.MES.DataAccess.Interface.RBAC;
using ServiceCenter.MES.DataAccess.Interface.WIP;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Model.RBAC;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Service.Contract.WIP;
using ServiceCenter.MES.Service.WIP.Resources;
using ServiceCenter.Model;
using ServiceCenter.Common;
using System.ServiceModel.Activation;
using ServiceCenter.MES.DataAccess.Interface.PPM;

namespace ServiceCenter.MES.Service.WIP
{
    /// <summary>
    /// 实现批次进站服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class LotTrackInService : ILotTrackInContract, ILotTrackInCheck, ILotTrackIn
    {
        /// <summary>
        /// 操作前检查事件。
        /// </summary>
        public event Func<TrackInParameter, MethodReturnResult> CheckEvent;
        /// <summary>
        /// 执行操作时事件。
        /// </summary>
        public event Func<TrackInParameter, MethodReturnResult> ExecutingEvent;
        /// <summary>
        /// 操作执行完成事件。
        /// </summary>
        public event Func<TrackInParameter, MethodReturnResult> ExecutedEvent;

        /// <summary>
        /// 自定义操作前检查的清单列表。
        /// </summary>
        private IList<ILotTrackInCheck> CheckList { get; set; }
        /// <summary>
        /// 自定义执行中操作的清单列表。
        /// </summary>
        private IList<ILotTrackIn> ExecutingList { get; set; }
        /// <summary>
        /// 自定义执行后操作的清单列表。
        /// </summary>
        private IList<ILotTrackIn> ExecutedList { get; set; }


        /// <summary>
        /// 注册自定义检查的操作实例。
        /// </summary>
        /// <param name="obj"></param>
        public void RegisterCheckInstance(ILotTrackInCheck obj)
        {
            if (this.CheckList == null)
            {
                this.CheckList = new List<ILotTrackInCheck>();
            }
            this.CheckList.Add(obj);
        }
        /// <summary>
        /// 注册执行中的自定义操作实例。
        /// </summary>
        /// <param name="obj"></param>
        public void RegisterExecutingInstance(ILotTrackIn obj)
        {
            if (this.ExecutingList == null)
            {
                this.ExecutingList = new List<ILotTrackIn>();
            }
            this.ExecutingList.Add(obj);
        }

        /// <summary>
        /// 注册执行完成后的自定义操作实例。
        /// </summary>
        /// <param name="obj"></param>
        public void RegisterExecutedInstance(ILotTrackIn obj)
        {
            if (this.ExecutedList == null)
            {
                this.ExecutedList = new List<ILotTrackIn>();
            }
            this.ExecutedList.Add(obj);
        }


        /// <summary>
        /// 操作前检查。
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        protected virtual MethodReturnResult OnCheck(TrackInParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };
            StringBuilder sbMessage = new StringBuilder();
            if (this.CheckEvent != null)
            {
                foreach (Func<TrackInParameter, MethodReturnResult> d in this.CheckEvent.GetInvocationList())
                {
                    result = d(p);
                    if (result.Code > 0)
                    {
                        return result;
                    }
                    if (!string.IsNullOrEmpty(result.Message))
                    {
                        sbMessage.Append(result.Message + "\n");
                    }
                }
            }
            if (this.CheckList != null)
            {
                foreach (ILotTrackInCheck d in this.CheckList)
                {
                    result = d.Check(p);
                    if (result.Code > 0)
                    {
                        return result;
                    }
                    if (!string.IsNullOrEmpty(result.Message))
                    {
                        sbMessage.Append(result.Message + "\n");
                    }
                }
            }
            result.Message = sbMessage.ToString();
            return result;
        }
        /// <summary>
        /// 操作执行中。
        /// </summary>
        protected virtual MethodReturnResult OnExecuting(TrackInParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };
            StringBuilder sbMessage = new StringBuilder();
            if (this.ExecutingEvent != null)
            {
                foreach (Func<TrackInParameter, MethodReturnResult> d in this.ExecutingEvent.GetInvocationList())
                {
                    result = d(p);
                    if (result.Code > 0)
                    {
                        return result;
                    }
                    if (!string.IsNullOrEmpty(result.Message))
                    {
                        sbMessage.Append(result.Message + "\n");
                    }
                }
            }

            if (this.ExecutingList != null)
            {
                foreach (ILotTrackIn d in this.ExecutingList)
                {
                    result = d.Execute(p);
                    if (result.Code > 0)
                    {
                        return result;
                    }
                    if (!string.IsNullOrEmpty(result.Message))
                    {
                        sbMessage.Append(result.Message + "\n");
                    }
                }
            }
            result.Message = sbMessage.ToString();
            return result;
        }
        /// <summary>
        /// 执行完成。
        /// </summary>
        protected virtual MethodReturnResult OnExecuted(TrackInParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };
            StringBuilder sbMessage = new StringBuilder();
            if (this.ExecutedEvent != null)
            {
                foreach (Func<TrackInParameter, MethodReturnResult> d in this.ExecutedEvent.GetInvocationList())
                {
                    result = d(p);
                    if (result.Code > 0)
                    {
                        return result;
                    }
                    if (!string.IsNullOrEmpty(result.Message))
                    {
                        sbMessage.Append(result.Message + "\n");
                    }
                }
            }
            if (this.ExecutedList != null)
            {
                foreach (ILotTrackIn d in this.ExecutedList)
                {
                    result = d.Execute(p);
                    if (result.Code > 0)
                    {
                        return result;
                    }
                    if (!string.IsNullOrEmpty(result.Message))
                    {
                        sbMessage.Append(result.Message + "\n");
                    }
                }
            }
            result.Message = sbMessage.ToString();
            return result;
        }

        /// <summary>
        /// 构造函数。
        /// </summary>
        public LotTrackInService()
        {
            this.RegisterCheckInstance(this);
            this.RegisterExecutedInstance(this);

        }
        /// <summary>
        /// 批次数据访问类。
        /// </summary>
        public ILotDataEngine LotDataEngine
        {
            get;
            set;
        }
        /// <summary>
        /// 批次操作数据访问类。
        /// </summary>
        public ILotTransactionDataEngine LotTransactionDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 批次历史数据访问类。
        /// </summary>
        public ILotTransactionHistoryDataEngine LotTransactionHistoryDataEngine
        {
            get;
            set;
        }

        /// <summary>
        ///  批次附加参数数据访问类。
        /// </summary>
        public ILotTransactionParameterDataEngine LotTransactionParameterDataEngine
        {
            get;
            set;
        }
        /// <summary>
        /// 生产线数据访问对象。
        /// </summary>
        public IProductionLineDataEngine ProductionLineDataEngine
        {
            get;
            set;
        }
        /// <summary>
        /// 区域数据访问对象。
        /// </summary>
        public ILocationDataEngine LocationDataEngine
        {
            get;
            set;
        }
        /// <summary>
        /// 批次加工设备数据访问对象。
        /// </summary>
        public ILotTransactionEquipmentDataEngine LotTransactionEquipmentDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 批次进站操作。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>
        /// 代码表示：0：成功，其他失败。
        /// </returns>
        MethodReturnResult ILotTrackInContract.TrackIn(TrackInParameter p)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (p == null)
            {
                result.Code = 1001;
                result.Message = StringResource.ParameterIsNull;
                return result;
            }
            try
            {
                StringBuilder sbMessage = new StringBuilder();
                //操作前检查。
                result = this.OnCheck(p);
                if (result.Code > 0)
                {
                    return result;
                }
                sbMessage.Append(result.Message);
                //执行操作
                using (TransactionScope ts = new TransactionScope())
                {
                    result = this.OnExecuting(p);
                    if (result.Code > 0)
                    {
                        return result;
                    }
                    sbMessage.Append(result.Message);
                    result = this.OnExecuted(p);
                    if (result.Code > 0)
                    {
                        return result;
                    }
                    sbMessage.Append(result.Message);
                    ts.Complete();
                }
                result.Message = sbMessage.ToString();
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = string.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 操作前检查。
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        MethodReturnResult ILotTrackInCheck.Check(TrackInParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            if (string.IsNullOrEmpty(p.LineCode))
            {
                result.Code = 1001;
                result.Message =string.Format("{0} {1}"
                                                ,"线别代码"
                                                ,StringResource.ParameterIsNull);
                return result;
            }

            if (string.IsNullOrEmpty(p.RouteOperationName))
            {
                result.Code = 1001;
                result.Message = string.Format("{0} {1}"
                                                , "工序名称"
                                                , StringResource.ParameterIsNull);
                return result;
            }

            if (p.LotNumbers == null || p.LotNumbers.Count==0)
            {
                result.Code = 1001;
                result.Message = string.Format("{0} {1}"
                                                , "批次号"
                                                , StringResource.ParameterIsNull);
                return result;
            }
            //获取线别车间。
            string locationName = string.Empty;
            ProductionLine pl = this.ProductionLineDataEngine.Get(p.LineCode);
            if (pl != null)
            {
                //根据线别所在区域，获取车间名称。
                Location loc = this.LocationDataEngine.Get(pl.LocationName);
                locationName = loc.ParentLocationName ?? string.Empty;
            }

            //循环批次。
            foreach (string lotNumber in p.LotNumbers)
            {
                Lot lot = this.LotDataEngine.Get(lotNumber);
                //判定是否存在批次记录。
                if (lot == null || lot.Status == EnumObjectStatus.Disabled)
                {
                    result.Code = 1002;
                    result.Message = string.Format("批次（{0}）不存在。", lotNumber);
                    return result;
                }
                //批次已经完成。
                if (lot.StateFlag == EnumLotState.Finished)
                {
                    result.Code = 1003;
                    result.Message = string.Format("批次（{0}）已完成。", lotNumber);
                    return result;
                }
                //批次已结束
                if (lot.DeletedFlag == true)
                {
                    result.Code = 1004;
                    result.Message = string.Format("批次（{0}）已结束。", lotNumber);
                    return result;
                }
                //批次已暂停
                if (lot.HoldFlag == true)
                {
                    result.Code = 1005;
                    result.Message = string.Format("批次（{0}）已暂停。", lotNumber);
                    return result;
                }
                //批次目前非等待进站状态。
                if (lot.StateFlag != EnumLotState.WaitTrackIn)
                {
                    result.Code = 1006;
                    result.Message = string.Format("批次（{0}）目前状态（{1}）,非({2})状态。"
                                                          , lotNumber
                                                          , lot.StateFlag.GetDisplayName()
                                                          , EnumLotState.WaitTrackIn.GetDisplayName());
                    return result;
                }
                //检查批次所在工序是否处于指定工序
                if (lot.RouteStepName != p.RouteOperationName)
                {
                    result.Code = 1007;
                    result.Message = string.Format("批次（{0}）目前在（{1}）工序，不能在({2})操作。"
                                                    , lotNumber
                                                    , lot.RouteStepName
                                                    , p.RouteOperationName);
                    return result;
                }
                //检查批次车间和线别车间是否匹配。
                if (lot.LocationName != locationName)
                {
                    result.Code = 1008;
                    result.Message = string.Format("批次（{0}）属于({1})车间，不能在({2})车间线别上操作。"
                                                    , lotNumber
                                                    , lot.LocationName
                                                    , locationName);
                    return result;
                }
            }
            return result;
        }
        /// <summary>
        /// 执行操作。
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        MethodReturnResult ILotTrackIn.Execute(TrackInParameter p)
        {
            DateTime now = DateTime.Now;
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };
           

            p.TransactionKeys = new Dictionary<string, string>();
            //循环批次。
            foreach (string lotNumber in p.LotNumbers)
            {
                Lot lot = this.LotDataEngine.Get(lotNumber);
                //生成操作事务主键。
                string transactionKey = Guid.NewGuid().ToString();
                p.TransactionKeys.Add(lotNumber, transactionKey);

                //更新批次记录。
                Lot lotUpdate = lot.Clone() as Lot;
                lotUpdate.StartProcessTime = now;
                lotUpdate.EquipmentCode = p.EquipmentCode;
                lotUpdate.LineCode = p.LineCode;
                lotUpdate.OperateComputer = p.OperateComputer;
                lotUpdate.PreLineCode = lot.LineCode;
                lotUpdate.Editor = p.Creator;
                lotUpdate.StateFlag = EnumLotState.WaitTrackOut;
                lotUpdate.EditTime = now;
                this.LotDataEngine.Update(lotUpdate);

                //记录批次设备加工历史数据
                if (!string.IsNullOrEmpty(p.EquipmentCode))
                {
                    LotTransactionEquipment transEquipment = new LotTransactionEquipment()
                    {
                        Key = transactionKey,
                        EndTransactionKey = null,
                        CreateTime = now,
                        Creator = p.Creator,
                        Editor = p.Creator,
                        EditTime = now,
                        EndTime = null,
                        EquipmentCode = p.EquipmentCode,
                        LotNumber = lotNumber,
                        Quantity = lot.Quantity,
                        StartTime = now,
                        State = EnumLotTransactionEquipmentState.Start
                    };
                    this.LotTransactionEquipmentDataEngine.Insert(transEquipment);
                }
                #region//记录操作历史。
                LotTransaction transObj = new LotTransaction()
                {
                    Key = transactionKey,
                    Activity = EnumLotActivity.TrackIn,
                    CreateTime = now,
                    Creator = p.Creator,
                    Description = p.Remark,
                    Editor = p.Creator,
                    EditTime = now,
                    InQuantity = lot.Quantity,
                    LotNumber = lotNumber,
                    LocationName=lot.LocationName,
                    LineCode=string.IsNullOrEmpty(p.LineCode)?lot.LineCode:p.LineCode,
                    OperateComputer = p.OperateComputer,
                    OrderNumber = lot.OrderNumber,
                    OutQuantity = lotUpdate.Quantity,
                    RouteEnterpriseName = lot.RouteEnterpriseName,
                    RouteName = lot.RouteName,
                    RouteStepName = lot.RouteStepName,
                    ShiftName = p.ShiftName,
                    UndoFlag = false,
                    UndoTransactionKey = null
                };
                this.LotTransactionDataEngine.Insert(transObj);
                //新增批次历史记录。
                LotTransactionHistory lotHistory = new LotTransactionHistory(transactionKey, lot);
                this.LotTransactionHistoryDataEngine.Insert(lotHistory);
                #endregion
                
                #region //有附加参数记录附加参数数据。
                if (p.Paramters != null && p.Paramters.ContainsKey(lotNumber))
                {
                    foreach (TransactionParameter tp in p.Paramters[lotNumber])
                    {
                        LotTransactionParameter lotParamObj = new LotTransactionParameter()
                        {
                            Key = new LotTransactionParameterKey()
                            {
                                TransactionKey = transactionKey,
                                ParameterName = tp.Name,
                                ItemNo = tp.Index,
                            },
                            ParameterValue = tp.Value,
                            Editor = p.Creator,
                            EditTime = now
                        };
                        this.LotTransactionParameterDataEngine.Insert(lotParamObj);
                    }
                }
                #endregion
            }
            return result;
        }


    }
}
