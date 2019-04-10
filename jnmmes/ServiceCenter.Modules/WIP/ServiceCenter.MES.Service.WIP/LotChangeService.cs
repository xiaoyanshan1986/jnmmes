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
    /// 实现批次转工单服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class LotChangeService : ILotChangeContract, ILotChangeCheck, ILotChange
    {


        /// <summary>
        /// 批次数据访问类。
        /// </summary>
        public ILotDataEngine LotDataEngine { get; set; }
        /// <summary>
        /// 批次操作数据访问类。
        /// </summary>
        public ILotTransactionDataEngine LotTransactionDataEngine { get; set; }

        /// <summary>
        /// 批次历史数据访问类。
        /// </summary>
        public ILotTransactionHistoryDataEngine LotTransactionHistoryDataEngine { get; set; }

        /// <summary>
        ///  批次附加参数数据访问类。
        /// </summary>
        public ILotTransactionParameterDataEngine LotTransactionParameterDataEngine { get; set; }
        /// <summary>
        /// 批次加工设备数据访问对象。
        /// </summary>
        public ILotTransactionEquipmentDataEngine LotTransactionEquipmentDataEngine
        {
            get;
            set;
        }


        /// <summary>
        /// 批次数据访问类。
        /// </summary>
        public IPackageDetailDataEngine PackageDetailDataEngine
        {
            get;
            set;
        }
        /// <summary>
        /// 操作前检查事件。
        /// </summary>
        public event Func<ChangeParameter, MethodReturnResult> CheckEvent;
        /// <summary>
        /// 执行操作时事件。
        /// </summary>
        public event Func<ChangeParameter, MethodReturnResult> ExecutingEvent;
        /// <summary>
        /// 操作执行完成事件。
        /// </summary>
        public event Func<ChangeParameter, MethodReturnResult> ExecutedEvent;

        /// <summary>
        /// 构造函数。
        /// </summary>
        public LotChangeService(ISessionFactory sf)
        {
            this.SessionFactory = sf;
            this.RegisterCheckInstance(this);
            this.RegisterExecutedInstance(this);
        }

        /// <summary>
        /// 自定义操作前检查的清单列表。
        /// </summary>
        private IList<ILotChangeCheck> CheckList { get; set; }
        /// <summary>
        /// 自定义执行中操作的清单列表。
        /// </summary>
        private IList<ILotChange> ExecutingList { get; set; }
        /// <summary>
        /// 自定义执行后操作的清单列表。
        /// </summary>
        private IList<ILotChange> ExecutedList { get; set; }

        public ISessionFactory SessionFactory
        {
            get;
            set;
        }

        /// <summary>
        /// 注册自定义检查的操作实例。
        /// </summary>
        /// <param name="obj"></param>
        public void RegisterCheckInstance(ILotChangeCheck obj)
        {
            if (this.CheckList == null)
            {
                this.CheckList = new List<ILotChangeCheck>();
            }
            this.CheckList.Add(obj);
        }
        /// <summary>
        /// 注册执行中的自定义操作实例。
        /// </summary>
        /// <param name="obj"></param>
        public void RegisterExecutingInstance(ILotChange obj)
        {
            if (this.ExecutingList == null)
            {
                this.ExecutingList = new List<ILotChange>();
            }
            this.ExecutingList.Add(obj);
        }

        /// <summary>
        /// 注册执行完成后的自定义操作实例。
        /// </summary>
        /// <param name="obj"></param>
        public void RegisterExecutedInstance(ILotChange obj)
        {
            if (this.ExecutedList == null)
            {
                this.ExecutedList = new List<ILotChange>();
            }
            this.ExecutedList.Add(obj);
        }


        /// <summary>
        /// 操作前检查。
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        protected virtual MethodReturnResult OnCheck(ChangeParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };
            StringBuilder sbMessage = new StringBuilder();
            if (this.CheckEvent != null)
            {
                foreach (Func<ChangeParameter, MethodReturnResult> d in this.CheckEvent.GetInvocationList())
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
                foreach (ILotChangeCheck d in this.CheckList)
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
        protected virtual MethodReturnResult OnExecuting(ChangeParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };
            StringBuilder sbMessage = new StringBuilder();
            if (this.ExecutingEvent != null)
            {
                foreach (Func<ChangeParameter, MethodReturnResult> d in this.ExecutingEvent.GetInvocationList())
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
                foreach (ILotChange d in this.ExecutingList)
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
        protected virtual MethodReturnResult OnExecuted(ChangeParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };
            StringBuilder sbMessage = new StringBuilder();
            if (this.ExecutedEvent != null)
            {
                foreach (Func<ChangeParameter, MethodReturnResult> d in this.ExecutedEvent.GetInvocationList())
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
                foreach (ILotChange d in this.ExecutedList)
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
        /// 批次转工单操作。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>
        /// 代码表示：0：成功，其他失败。
        /// </returns>
        MethodReturnResult ILotChangeContract.Change(ChangeParameter p)
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
                result = this.OnExecuting(p);
                if (result.Code > 0)
                {
                    return result;
                }
                result = this.OnExecuted(p);
                sbMessage.Append(result.Message);
                if (result.Code > 0)
                {
                    return result;
                }
                sbMessage.Append(result.Message);
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
        MethodReturnResult ILotChangeCheck.Check(ChangeParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

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
                ////批次已经完成。
                //if (lot.StateFlag == EnumLotState.Finished)
                //{
                //    result.Code = 1003;
                //    result.Message = string.Format("批次（{0}）已完成。", lotNumber);
                //    return result;
                //}
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

                if (lot.PackageFlag == true)
                {
                    result.Code = 1006;
                    result.Message = string.Format("批次（{0}）已入托（{1}）。", lotNumber, lot.PackageNo);
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
        MethodReturnResult ILotChange.Execute(ChangeParameter p)
        {
            DateTime now = DateTime.Now;
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };
            p.TransactionKeys = new Dictionary<string, string>();
            List<Lot> lstLotDataEngineForUpdate = new List<Lot>();
            List<LotTransactionEquipment> lstLotTransactionEquipmentDataEngineForUpdate = new List<LotTransactionEquipment>();
            List<LotTransaction> lstLotTransactionDataEngineForInsert = new List<LotTransaction>();
            List<LotTransactionHistory> lstLotTransactionHistoryDataEngineForInsert = new List<LotTransactionHistory>();
            List<LotTransactionParameter> lstLotTransactionParameterDataEngineForInsert = new List<LotTransactionParameter>();

            ////获取转工单的工单信息
            //PagingConfig ocfg = new PagingConfig()
            //{
            //    IsPaging = false,
            //    Where = string.Format("Key.OrderNumber='{0}' AND IsRework = false", p.OrderNumber)
            //};
            //IList<WorkOrderRoute> worList = this.WorkOrderRouteDataEngine.Get(ocfg);
            //WorkOrderRoute wor = null;
            //if (worList != null && worList.Count > 0)
            //{
            //    wor = worList.First<WorkOrderRoute>();
            //}
            //else
            //{
            //    result.Message = string.Format("工单{0}的主流程不存在", p.OrderNumber);
            //    return result;
            //}

            //循环批次。
            foreach (string lotNumber in p.LotNumbers)
            {
                Lot lot = this.LotDataEngine.Get(lotNumber);
                //生成操作事务主键。
                string transactionKey = Guid.NewGuid().ToString();
                p.TransactionKeys.Add(lotNumber, transactionKey);

                //更新批次记录。
                Lot lotUpdate = lot.Clone() as Lot;
                lotUpdate.OrderNumber = p.OrderNumber;
                lotUpdate.MaterialCode = p.MaterialCode;
                lotUpdate.RouteEnterpriseName = p.RouteEnterpriseName;
                lotUpdate.RouteName = p.RouteName;
                lotUpdate.RouteStepName = p.RouteStepName;
                lotUpdate.StateFlag = EnumLotState.WaitTrackIn;
                lotUpdate.OperateComputer = p.OperateComputer;
                lotUpdate.Editor = p.Creator;
                lotUpdate.EditTime = now;
                //this.LotDataEngine.Update(lotUpdate);
                lstLotDataEngineForUpdate.Add(lotUpdate);

                //进站时已经确定设备。
                if (!string.IsNullOrEmpty(lot.EquipmentCode))
                {
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
                        itemUpdate.EndTransactionKey = transactionKey;
                        itemUpdate.EditTime = now;
                        itemUpdate.Editor = p.Creator;
                        itemUpdate.EndTime = now;
                        itemUpdate.State = EnumLotTransactionEquipmentState.End;
                        //this.LotTransactionEquipmentDataEngine.Update(itemUpdate);
                        lstLotTransactionEquipmentDataEngineForUpdate.Add(itemUpdate);
                    }
                }
                #region//记录操作历史。
                LotTransaction transObj = new LotTransaction()
                {
                    Key = transactionKey,
                    Activity = EnumLotActivity.Change,
                    CreateTime = now,
                    Creator = p.Creator,
                    Description = p.Remark,
                    Editor = p.Creator,
                    EditTime = now,
                    InQuantity = lot.Quantity,
                    LotNumber = lotNumber,
                    LineCode = lot.LineCode,
                    LocationName = lot.LocationName,
                    OperateComputer = p.OperateComputer,
                    OrderNumber = lot.OrderNumber,
                    OutQuantity = lotUpdate.Quantity,
                    RouteEnterpriseName = p.RouteEnterpriseName,
                    RouteName = p.RouteName,
                    RouteStepName = p.RouteStepName,
                    ShiftName = p.ShiftName,
                    UndoFlag = false,
                    UndoTransactionKey = null
                };
                //this.LotTransactionDataEngine.Insert(transObj);
                lstLotTransactionDataEngineForInsert.Add(transObj);
                //新增批次历史记录。
                LotTransactionHistory lotHistory = new LotTransactionHistory(transactionKey, lot);
                //this.LotTransactionHistoryDataEngine.Insert(lotHistory);
                lstLotTransactionHistoryDataEngineForInsert.Add(lotHistory);
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
                        //lstLotTransactionParameterDataEngineForUpdate.Add(lotParamObj);
                    }
                }
                #endregion
            }

            #region //开始事物处理
            ITransaction transaction = null;
            ISession db = this.SessionFactory.OpenSession();
            transaction = db.BeginTransaction();

            try
            {
                #region 更新批次LOT 的信息
                //更新批次基本信息
                foreach (Lot obj in lstLotDataEngineForUpdate)
                {
                    this.LotDataEngine.Update(obj, db);
                }

                foreach (LotTransactionEquipment obj in lstLotTransactionEquipmentDataEngineForUpdate)
                {
                    this.LotTransactionEquipmentDataEngine.Update(obj, db);
                }

                //更新批次LotTransaction信息
                foreach (LotTransaction obj in lstLotTransactionDataEngineForInsert)
                {
                    this.LotTransactionDataEngine.Insert(obj, db);
                }

                foreach (LotTransactionHistory obj in lstLotTransactionHistoryDataEngineForInsert)
                {
                    this.LotTransactionHistoryDataEngine.Insert(obj, db);
                }

                foreach (LotTransactionParameter obj in lstLotTransactionParameterDataEngineForInsert)
                {
                    this.LotTransactionParameterDataEngine.Insert(obj, db);
                }
                #endregion

                transaction.Commit();
                db.Close();

            }  
            catch (Exception err)
            {
                transaction.Rollback();
                db.Close();
                result.Code = 1000;
                result.Message += string.Format(StringResource.Error, err.Message);
                result.Detail = err.ToString();
                return result;
            }
            #endregion
            return result;
        }



    }
}
