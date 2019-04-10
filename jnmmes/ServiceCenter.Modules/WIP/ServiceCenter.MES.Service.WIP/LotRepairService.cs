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
    /// 实现批次返修服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class LotRepairService : ILotRepairContract, ILotRepairCheck, ILotRepair
    {
        /// <summary>
        /// 操作前检查事件。
        /// </summary>
        public event Func<RepairParameter, MethodReturnResult> CheckEvent;
        /// <summary>
        /// 执行操作时事件。
        /// </summary>
        public event Func<RepairParameter, MethodReturnResult> ExecutingEvent;
        /// <summary>
        /// 操作执行完成事件。
        /// </summary>
        public event Func<RepairParameter, MethodReturnResult> ExecutedEvent;

        /// <summary>
        /// 自定义操作前检查的清单列表。
        /// </summary>
        private IList<ILotRepairCheck> CheckList { get; set; }
        /// <summary>
        /// 自定义执行中操作的清单列表。
        /// </summary>
        private IList<ILotRepair> ExecutingList { get; set; }
        /// <summary>
        /// 自定义执行后操作的清单列表。
        /// </summary>
        private IList<ILotRepair> ExecutedList { get; set; }


        /// <summary>
        /// 注册自定义检查的操作实例。
        /// </summary>
        /// <param name="obj"></param>
        public void RegisterCheckInstance(ILotRepairCheck obj)
        {
            if (this.CheckList == null)
            {
                this.CheckList = new List<ILotRepairCheck>();
            }
            this.CheckList.Add(obj);
        }
        /// <summary>
        /// 注册执行中的自定义操作实例。
        /// </summary>
        /// <param name="obj"></param>
        public void RegisterExecutingInstance(ILotRepair obj)
        {
            if (this.ExecutingList == null)
            {
                this.ExecutingList = new List<ILotRepair>();
            }
            this.ExecutingList.Add(obj);
        }

        /// <summary>
        /// 注册执行完成后的自定义操作实例。
        /// </summary>
        /// <param name="obj"></param>
        public void RegisterExecutedInstance(ILotRepair obj)
        {
            if (this.ExecutedList == null)
            {
                this.ExecutedList = new List<ILotRepair>();
            }
            this.ExecutedList.Add(obj);
        }


        /// <summary>
        /// 操作前检查。
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        protected virtual MethodReturnResult OnCheck(RepairParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            if (this.CheckEvent != null)
            {
                foreach (Func<RepairParameter, MethodReturnResult> d in this.CheckEvent.GetInvocationList())
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
                foreach (ILotRepairCheck d in this.CheckList)
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
        protected virtual MethodReturnResult OnExecuting(RepairParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            if (this.ExecutingEvent != null)
            {
                foreach (Func<RepairParameter, MethodReturnResult> d in this.ExecutingEvent.GetInvocationList())
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
                foreach (ILotRepair d in this.ExecutingList)
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
        protected virtual MethodReturnResult OnExecuted(RepairParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            if (this.ExecutedEvent != null)
            {
                foreach (Func<RepairParameter, MethodReturnResult> d in this.ExecutedEvent.GetInvocationList())
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
                foreach (ILotRepair d in this.ExecutedList)
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
        public LotRepairService()
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
        ///  批次下一工艺工步数据访问类。
        /// </summary>
        public ILotTransactionStepDataEngine LotTransactionStepDataEngine
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
        /// 工艺流程工序数据访问对象
        /// </summary>
        public IRouteStepDataEngine RouteStepDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 工艺流程工序属性数据访问对象
        /// </summary>
        public IRouteStepAttributeDataEngine RouteStepAttributeDataEngine
        {
            get;
            set;
        }

        public ILotAttributeDataEngine LotAttributeDataEngine
        {
            get;
            set;
        }
        

        /// <summary>
        /// 批次返修操作。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>
        /// 代码表示：0：成功，其他失败。
        /// </returns>
        MethodReturnResult ILotRepairContract.Repair(RepairParameter p)
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
               // using (TransactionScope ts = new TransactionScope())
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
        MethodReturnResult ILotRepairCheck.Check(RepairParameter p)
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
        MethodReturnResult ILotRepair.Execute(RepairParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };
            try
            {
                List<Lot> lstLotDataEngineForUpdate = new List<Lot>();
                List<LotTransactionEquipment> lstLotTransactionEquipmentForUpdate = new List<LotTransactionEquipment>();
                List<LotTransaction> lstLotTransInsert = new List<LotTransaction>();                    //批次事物列表
                List<LotTransactionHistory> lstLotTransHisInsert = new List<LotTransactionHistory>();   //批次历史事物列表
                List<LotTransactionStep> lstLotTransactionStepForInsert = new List<LotTransactionStep>();
                List<LotTransactionParameter> lstLotTransactionParameterForInsert = new List<LotTransactionParameter>();
                List<LotAttribute> lstLotAttributeForDelete = new List<LotAttribute>();

                DateTime now = DateTime.Now;

                p.TransactionKeys = new Dictionary<string, string>();
                //循环批次。
                foreach (string lotNumber in p.LotNumbers)
                {
                    Lot lot = this.LotDataEngine.Get(lotNumber);

                    //生成操作事务主键。
                    string transactionKey = Guid.NewGuid().ToString();
                    p.TransactionKeys.Add(lotNumber, transactionKey);

                    #region //更新批次记录。
                    Lot lotUpdate = lot.Clone() as Lot;
                    lotUpdate.RouteEnterpriseName = p.RouteEnterpriseName;
                    lotUpdate.RouteName = p.RouteName;
                    lotUpdate.RouteStepName = p.RouteStepName;
                    lotUpdate.StartWaitTime = now;
                    lotUpdate.StartProcessTime = null;
                    lotUpdate.RepairFlag += 1;
                    lotUpdate.StateFlag = EnumLotState.WaitTrackIn;
                    lotUpdate.EquipmentCode = null;
                    lotUpdate.PreLineCode = lot.LineCode;
                    lotUpdate.LineCode = lot.LineCode;
                    lotUpdate.OperateComputer = p.OperateComputer;
                    lotUpdate.Editor = p.Creator;
                    lotUpdate.EditTime = now;
                    lotUpdate.LotState = 0;
                    //this.LotDataEngine.Update(lotUpdate);
                    lstLotDataEngineForUpdate.Add(lotUpdate);
                    #endregion

                    #region //更新批次设备加工历史数据
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
                            lstLotTransactionEquipmentForUpdate.Add(itemUpdate);
                        }
                    }
                    #endregion

                    #region//记录操作历史。
                    LotTransaction transObj = new LotTransaction()
                    {
                        Key = transactionKey,
                        Activity = EnumLotActivity.Repair,
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
                        OutQuantity = lot.Quantity,
                        RouteEnterpriseName = lot.RouteEnterpriseName,
                        RouteName = lot.RouteName,
                        RouteStepName = lot.RouteStepName,
                        ShiftName = p.ShiftName,
                        UndoFlag = false,
                        UndoTransactionKey = null
                    };
                    //this.LotTransactionDataEngine.Insert(transObj);
                    lstLotTransInsert.Add(transObj);

                    //新增批次历史记录。
                    LotTransactionHistory lotHistory = new LotTransactionHistory(transactionKey, lot);
                    //this.LotTransactionHistoryDataEngine.Insert(lotHistory);
                    lstLotTransHisInsert.Add(lotHistory);

                    //新增工艺下一步记录。
                    LotTransactionStep nextStep = new LotTransactionStep()
                    {
                        Key = transactionKey,
                        ToRouteEnterpriseName = p.RouteEnterpriseName,
                        ToRouteName = p.RouteName,
                        ToRouteStepName = p.RouteStepName,
                        Editor = p.Creator,
                        EditTime = now
                    };
                    //this.LotTransactionStepDataEngine.Insert(nextStep);
                    lstLotTransactionStepForInsert.Add(nextStep);
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
                            //this.LotTransactionParameterDataEngine.Insert(lotParamObj);
                            lstLotTransactionParameterForInsert.Add(lotParamObj);
                        }
                    }
                    #endregion

                    #region //清除批次IV图片属性或层压机台号属性
                    //1.判断返修工艺流程中工序过站是否需要层压机台号
                    PagingConfig cfgOfStepAttr = new PagingConfig()
                    {
                        Where = string.Format("Key.RouteName = '{0}' AND Key.AttributeName = 'isGetLayerEquipment'", p.RouteName)
                    };
                    IList<RouteStepAttribute> lstRouteStepAttribute = RouteStepAttributeDataEngine.Get(cfgOfStepAttr);
                    if (lstRouteStepAttribute != null && lstRouteStepAttribute.Count > 0)
                    {
                        #region 清除批次层压机台号属性
                        //获取批次层压机台号
                        LotAttributeKey lotAttributeKey = new LotAttributeKey()
                        {
                            LotNumber = lotNumber,
                            AttributeName = "LayerEquipmentNo"
                        };
                        LotAttribute lotAttribute = this.LotAttributeDataEngine.Get(lotAttributeKey);
                        if (lotAttribute != null)
                        {
                            //this.LotAttributeDataEngine.Delete(lotAttributeKey);
                            lstLotAttributeForDelete.Add(lotAttribute);
                        }
                        #endregion
                    }
                    //2.获取返修工艺流程工序并判断是否包含功率测试站
                    RouteStepKey routeStepKey = new RouteStepKey()
                    {
                        RouteName = p.RouteName,
                        RouteStepName = "功率测试"
                    };
                    RouteStep routeStepOfIV = this.RouteStepDataEngine.Get(routeStepKey);
                    if (routeStepOfIV != null)
                    {
                        #region 清除批次IV图片属性
                        //获取批次IV图片属性
                        LotAttributeKey lotAttributeKey = new LotAttributeKey()
                        {
                            LotNumber = lotNumber,
                            AttributeName = "IVImagePath"
                        };
                        LotAttribute lotAttribute = this.LotAttributeDataEngine.Get(lotAttributeKey);
                        if (lotAttribute != null)
                        {
                            //this.LotAttributeDataEngine.Delete(lotAttributeKey);
                            lstLotAttributeForDelete.Add(lotAttribute);
                        }
                        #endregion
                    }
                    #endregion
                }

                #region //事物处理
                ISession session = this.LotDataEngine.SessionFactory.OpenSession();
                ITransaction transaction = session.BeginTransaction();

                try
                {
                    //1.更新批次基本信息
                    foreach (Lot lot in lstLotDataEngineForUpdate)
                    {
                        this.LotDataEngine.Update(lot, session);
                    }
                    //2.更新批次LotTransaction信息
                    foreach (LotTransaction lotTransaction in lstLotTransInsert)
                    {
                        this.LotTransactionDataEngine.Insert(lotTransaction, session);
                    }

                    //3.更新批次LotTransactionHistory信息
                    foreach (LotTransactionHistory lotTransactionHistory in lstLotTransHisInsert)
                    {
                        this.LotTransactionHistoryDataEngine.Insert(lotTransactionHistory, session);
                    }

                    //4.更新LotTransactionEquipment信息
                    foreach (LotTransactionEquipment lotTransactionEquipment in lstLotTransactionEquipmentForUpdate)
                    {
                        this.LotTransactionEquipmentDataEngine.Update(lotTransactionEquipment, session);
                    }

                    //5.更新LotTransactionStep信息
                    foreach (LotTransactionStep lotTransactionStep in lstLotTransactionStepForInsert)
                    {
                        this.LotTransactionStepDataEngine.Insert(lotTransactionStep, session);
                    }

                    //6.更新LotTransactionParameter信息
                    foreach (LotTransactionParameter lotTransactionParameter in lstLotTransactionParameterForInsert)
                    {
                        this.LotTransactionParameterDataEngine.Insert(lotTransactionParameter, session);
                    }

                    //7.更新LotAttribute信息
                    foreach (LotAttribute lotAttribute in lstLotAttributeForDelete)
                    {
                        this.LotAttributeDataEngine.Delete(lotAttribute.Key, session);
                    }

                    //提交事务
                    transaction.Commit();
                    session.Close();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    session.Close();

                    result.Code = 2000;
                    result.Message = string.Format(StringResource.Error, ex.Message);
                }
                #endregion
            }
            catch (Exception ex)
            {
                result.Code = 2000;
                result.Message = string.Format(StringResource.Error, ex.Message);
            }
            
            return result;
        }       
    }
}
