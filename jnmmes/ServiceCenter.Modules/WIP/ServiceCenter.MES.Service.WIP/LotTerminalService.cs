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
using ServiceCenter.MES.DataAccess.Interface.PPM;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.DataAccess.Interface.LSM;
using ServiceCenter.MES.Model.LSM;
using System.ServiceModel.Activation;
using NHibernate;

namespace ServiceCenter.MES.Service.WIP
{
    /// <summary>
    /// 实现批次结束服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class LotTerminalService : ILotTerminalContract, ILotTerminalCheck, ILotTerminal
    {
        /// <summary>
        /// 操作前检查事件。
        /// </summary>
        public event Func<TerminalParameter, MethodReturnResult> CheckEvent;
        /// <summary>
        /// 执行操作时事件。
        /// </summary>
        public event Func<TerminalParameter, MethodReturnResult> ExecutingEvent;
        /// <summary>
        /// 操作执行完成事件。
        /// </summary>
        public event Func<TerminalParameter, MethodReturnResult> ExecutedEvent;

        /// <summary>
        /// 自定义操作前检查的清单列表。
        /// </summary>
        private IList<ILotTerminalCheck> CheckList { get; set; }
        /// <summary>
        /// 自定义执行中操作的清单列表。
        /// </summary>
        private IList<ILotTerminal> ExecutingList { get; set; }
        /// <summary>
        /// 自定义执行后操作的清单列表。
        /// </summary>
        private IList<ILotTerminal> ExecutedList { get; set; }


        /// <summary>
        /// 注册自定义检查的操作实例。
        /// </summary>
        /// <param name="obj"></param>
        public void RegisterCheckInstance(ILotTerminalCheck obj)
        {
            if (this.CheckList == null)
            {
                this.CheckList = new List<ILotTerminalCheck>();
            }
            this.CheckList.Add(obj);
        }
        /// <summary>
        /// 注册执行中的自定义操作实例。
        /// </summary>
        /// <param name="obj"></param>
        public void RegisterExecutingInstance(ILotTerminal obj)
        {
            if (this.ExecutingList == null)
            {
                this.ExecutingList = new List<ILotTerminal>();
            }
            this.ExecutingList.Add(obj);
        }

        /// <summary>
        /// 注册执行完成后的自定义操作实例。
        /// </summary>
        /// <param name="obj"></param>
        public void RegisterExecutedInstance(ILotTerminal obj)
        {
            if (this.ExecutedList == null)
            {
                this.ExecutedList = new List<ILotTerminal>();
            }
            this.ExecutedList.Add(obj);
        }


        /// <summary>
        /// 操作前检查。
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        protected virtual MethodReturnResult OnCheck(TerminalParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            if (this.CheckEvent != null)
            {
                foreach (Func<TerminalParameter, MethodReturnResult> d in this.CheckEvent.GetInvocationList())
                {
                    result = d(p);
                    if (result.Code > 0)
                    {
                        return result;
                    }
                }
            }
            if (this.CheckList != null)
            {
                foreach (ILotTerminalCheck d in this.CheckList)
                {
                    result = d.Check(p);
                    if (result.Code > 0)
                    {
                        return result;
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// 操作执行中。
        /// </summary>
        protected virtual MethodReturnResult OnExecuting(TerminalParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            if (this.ExecutingEvent != null)
            {
                foreach (Func<TerminalParameter, MethodReturnResult> d in this.ExecutingEvent.GetInvocationList())
                {
                    result = d(p);
                    if (result.Code > 0)
                    {
                        return result;
                    }
                }
            }

            if (this.ExecutingList != null)
            {
                foreach (ILotTerminal d in this.ExecutingList)
                {
                    result = d.Execute(p);
                    if (result.Code > 0)
                    {
                        return result;
                    }
                }
            }

            return result;
        }
        /// <summary>
        /// 执行完成。
        /// </summary>
        protected virtual MethodReturnResult OnExecuted(TerminalParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            if (this.ExecutedEvent != null)
            {
                foreach (Func<TerminalParameter, MethodReturnResult> d in this.ExecutedEvent.GetInvocationList())
                {
                    result = d(p);
                    if (result.Code > 0)
                    {
                        return result;
                    }
                }
            }
            if (this.ExecutedList != null)
            {
                foreach (ILotTerminal d in this.ExecutedList)
                {
                    result = d.Execute(p);
                    if (result.Code > 0)
                    {
                        return result;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 构造函数。
        /// </summary>
        public LotTerminalService()
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
        /// 批次终止数据访问类。
        /// </summary>
        public ILotTransactionTerminalDataEngine LotTransactionTerminalDataEngine
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
        /// 批次加工设备数据访问对象。
        /// </summary>
        public ILotTransactionEquipmentDataEngine LotTransactionEquipmentDataEngine
        {
            get;
            set;
        }
        /// <summary>
        /// 批次结束操作。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>
        /// 代码表示：0：成功，其他失败。
        /// </returns>
        MethodReturnResult ILotTerminalContract.Terminal(TerminalParameter p)
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
                //操作前检查。
                result = this.OnCheck(p);
                if (result.Code > 0)
                {
                    return result;
                }
                //执行操作
                //using (TransactionScope ts = new TransactionScope())
                ISession db = this.LotDataEngine.SessionFactory.OpenSession();
                ITransaction transaction = db.BeginTransaction();
                {
                    result = this.OnExecuting(p);
                    if (result.Code > 0)
                    {
                        return result;
                    }
                    result = this.OnExecuted(p);
                    if (result.Code > 0)
                    {
                        return result;
                    }
                    //ts.Complete();
                    transaction.Commit();
                    db.Close();
                }
            }
            catch(Exception ex)
            {
                result.Code = 1000;
                result.Message = string.Format(StringResource.Error,ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 操作前检查。
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        MethodReturnResult ILotTerminalCheck.Check(TerminalParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };
            foreach (string lotNumber in p.LotNumbers)
            {
                Lot lot = this.LotDataEngine.Get(lotNumber);
                //判定是否存在批次记录。
                if (lot == null)
                {
                    result.Code = 1002;
                    result.Message = string.Format("批次（{0}）不存在。", lotNumber);
                    return result;
                }
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
                //批次已包装
                if (lot.PackageFlag == true)
                {
                    result.Code = 1006;
                    result.Message = string.Format("批次（{0}）已包装。", lotNumber);
                    return result;
                }
                //批次已暂停
                if (lot.HoldFlag == true)
                {
                    result.Code = 1005;
                    result.Message = string.Format("批次（{0}）已暂停。", lotNumber);
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
        MethodReturnResult ILotTerminal.Execute(TerminalParameter p)
        {
            DateTime now = DateTime.Now;
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };
            p.TransactionKeys = new Dictionary<string, string>();
            //循环批次。
            foreach(string lotNumber in p.LotNumbers)
            {
                Lot lot = this.LotDataEngine.Get(lotNumber);

                //生成操作事务主键。
                string transaciontKey = Guid.NewGuid().ToString();
                p.TransactionKeys.Add(lotNumber, transaciontKey);

                #region //更新批次记录。
                Lot lotUpdate = lot.Clone() as Lot;
                lotUpdate.DeletedFlag = true;
                lotUpdate.OperateComputer = p.OperateComputer;
                lotUpdate.Editor = p.Creator;
                lotUpdate.EditTime = now;
                this.LotDataEngine.Update(lotUpdate);
                #endregion

                //更新批次设备加工历史数据。
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("LotNumber='{0}' AND EquipmentCode='{1}' AND State=0",
                                          lotNumber,
                                          lot.EquipmentCode)
                };
                IList<LotTransactionEquipment> lstLotTransactionEquipment = this.LotTransactionEquipmentDataEngine.Get(cfg);
                foreach (LotTransactionEquipment item in lstLotTransactionEquipment)
                {
                    LotTransactionEquipment itemUpdate = item.Clone() as LotTransactionEquipment;
                    itemUpdate.EndTransactionKey = transaciontKey;
                    itemUpdate.EditTime = now;
                    itemUpdate.Editor = p.Creator;
                    itemUpdate.EndTime = now;
                    itemUpdate.State = EnumLotTransactionEquipmentState.End;
                    this.LotTransactionEquipmentDataEngine.Update(itemUpdate);
                }

                #region//记录操作历史。
                LotTransaction transObj = new LotTransaction()
                {
                    Key =  transaciontKey,
                    Activity = EnumLotActivity.Terminal,
                    CreateTime =now,
                    Creator=p.Creator,
                    Description=p.Remark,
                    Editor=p.Creator,
                    EditTime=now,
                    InQuantity = lot.Quantity,
                    LotNumber=lotNumber,
                    LocationName=lot.LocationName,
                    LineCode=lot.LineCode,
                    OperateComputer=p.OperateComputer,
                    OrderNumber=lot.OrderNumber,
                    OutQuantity=lot.Quantity,
                    RouteEnterpriseName=lot.RouteEnterpriseName,
                    RouteName = lot.RouteName,
                    RouteStepName = lot.RouteStepName,
                    ShiftName=p.ShiftName,
                    UndoFlag=false,
                    UndoTransactionKey=null
                };
                this.LotTransactionDataEngine.Insert(transObj);
                //新增批次历史记录。
                LotTransactionHistory lotHistory = new LotTransactionHistory(transaciontKey, lot);
                this.LotTransactionHistoryDataEngine.Insert(lotHistory);
                //新增批次终止数据
                LotTransactionTerminal lotTerminal = new LotTransactionTerminal()
                {
                   Editor=p.Creator,
                   EditTime=now,
                   Key=transaciontKey,
                   ReasonCodeCategoryName=p.ReasonCodeCategoryName,
                   ReasonCodeName=p.ReasonCodeName
                };
                this.LotTransactionTerminalDataEngine.Insert(lotTerminal);
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
                                TransactionKey=transaciontKey,
                                ParameterName=tp.Name,
                                ItemNo=tp.Index,
                            },
                            ParameterValue=tp.Value,
                            Editor=p.Creator,
                            EditTime=now
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
