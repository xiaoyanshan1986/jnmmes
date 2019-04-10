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
using ServiceCenter.MES.DataAccess.Interface.LSM;
using ServiceCenter.MES.Model.LSM;
using NHibernate;
using ServiceCenter.MES.Model.PPM;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;



namespace ServiceCenter.MES.Service.WIP
{
    /// <summary>
    /// 实现批次操作撤销服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class LotUndoService : ILotUndoContract, ILotUndoCheck, ILotUndo
    {
        
        public ISessionFactory SessionFactory
        {
            get;
            set;
        }
        protected Database query_db;      

        /// <summary>
        /// 操作前检查事件。
        /// </summary>
        public event Func<UndoParameter, MethodReturnResult> CheckEvent;
        /// <summary>
        /// 执行操作时事件。
        /// </summary>
        public event Func<UndoParameter, MethodReturnResult> ExecutingEvent;
        /// <summary>
        /// 操作执行完成事件。
        /// </summary>
        public event Func<UndoParameter, MethodReturnResult> ExecutedEvent;

        /// <summary>
        /// 自定义操作前检查的清单列表。
        /// </summary>
        private IList<ILotUndoCheck> CheckList { get; set; }
        /// <summary>
        /// 自定义执行中操作的清单列表。
        /// </summary>
        private IList<ILotUndo> ExecutingList { get; set; }
        /// <summary>
        /// 自定义执行后操作的清单列表。
        /// </summary>
        private IList<ILotUndo> ExecutedList { get; set; }


        /// <summary>
        /// 注册自定义检查的操作实例。
        /// </summary>
        /// <param name="obj"></param>
        public void RegisterCheckInstance(ILotUndoCheck obj)
        {
            if (this.CheckList == null)
            {
                this.CheckList = new List<ILotUndoCheck>();
            }
            this.CheckList.Add(obj);
        }
        /// <summary>
        /// 注册执行中的自定义操作实例。
        /// </summary>
        /// <param name="obj"></param>
        public void RegisterExecutingInstance(ILotUndo obj)
        {
            if (this.ExecutingList == null)
            {
                this.ExecutingList = new List<ILotUndo>();
            }
            this.ExecutingList.Add(obj);
        }

        /// <summary>
        /// 注册执行完成后的自定义操作实例。
        /// </summary>
        /// <param name="obj"></param>
        public void RegisterExecutedInstance(ILotUndo obj)
        {
            if (this.ExecutedList == null)
            {
                this.ExecutedList = new List<ILotUndo>();
            }
            this.ExecutedList.Add(obj);
        }


        /// <summary>
        /// 操作前检查。
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        protected virtual MethodReturnResult OnCheck(UndoParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };
            StringBuilder sbMessage = new StringBuilder();
            if (this.CheckEvent != null)
            {
                foreach (Func<UndoParameter, MethodReturnResult> d in this.CheckEvent.GetInvocationList())
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
                foreach (ILotUndoCheck d in this.CheckList)
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
        protected virtual MethodReturnResult OnExecuting(UndoParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };
            StringBuilder sbMessage = new StringBuilder();

            if (this.ExecutingEvent != null)
            {
                foreach (Func<UndoParameter, MethodReturnResult> d in this.ExecutingEvent.GetInvocationList())
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
                foreach (ILotUndo d in this.ExecutingList)
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
        protected virtual MethodReturnResult OnExecuted(UndoParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };
            StringBuilder sbMessage = new StringBuilder();
            if (this.ExecutedEvent != null)
            {
                foreach (Func<UndoParameter, MethodReturnResult> d in this.ExecutedEvent.GetInvocationList())
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
                foreach (ILotUndo d in this.ExecutedList)
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
        public LotUndoService(ISessionFactory sf)
        {
            this.query_db = DatabaseFactory.CreateDatabase("QUERYDATA");
            this.SessionFactory = sf;

            this.RegisterCheckInstance(this);
            this.RegisterExecutedInstance(this);
        }
        //public LotUndoService()
        //{
        //    this.RegisterCheckInstance(this);
        //    this.RegisterExecutedInstance(this);
        //}

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
        /// 包装数据访问对象。
        /// </summary>
        public IPackageDataEngine PackageDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 包装明细数据访问对象。
        /// </summary>
        public IPackageDetailDataEngine PackageDetailDataEngine
        {
            get;
            set;
        }
        /// <summary>
        ///  批次包装操作数据访问类。
        /// </summary>
        public ILotTransactionPackageDataEngine LotTransactionPackageDataEngine
        {
            get;
            set;
        }

        /// <summary>
        ///  批次物料数据访问类。
        /// </summary>
        public ILotBOMDataEngine LotBOMDataEngine
        {
            get;
            set;
        }
        /// <summary>
        /// 物料上料明细数据访问类。
        /// </summary>
        public IMaterialLoadingDetailDataEngine MaterialLoadingDetailDataEngine
        {
            get;
            set;
        }
        /// <summary>
        /// 线边仓物料明细数据访问类。
        /// </summary>
        public ILineStoreMaterialDetailDataEngine LineStoreMaterialDetailDataEngine
        {
            get;
            set;
        }
        /// <summary>
        /// 批次设备数据访问类。
        /// </summary>
        public ILotTransactionEquipmentDataEngine LotTransactionEquipmentDataEngine
        {
            get;
            set;
        }

        public IRouteStepDataEngine RouteStepDataEngine
        {
            get;
            set;
        }

        /// <summary> 工单数据访问类 </summary>
        public IWorkOrderDataEngine WorkOrderDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 批次操作撤销操作。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>
        /// 代码表示：0：成功，其他失败。
        /// </returns>
        MethodReturnResult ILotUndoContract.Undo(UndoParameter p)
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
                //using (TransactionScope ts = new TransactionScope())
                ISession db = this.LotDataEngine.SessionFactory.OpenSession();
                ITransaction transaction = db.BeginTransaction();
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
                        transaction.Rollback();
                        db.Close();

                        return result;
                    }

                    sbMessage.Append(result.Message);
                    //ts.Complete();
                    transaction.Commit();
                    db.Close();
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
        MethodReturnResult ILotUndoCheck.Check(UndoParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };
            
            if (p.LotNumbers == null || p.LotNumbers.Count==0)
            {
                result.Code = 1001;
                result.Message = string.Format("{0} {1}"
                                                , "批次号"
                                                , StringResource.ParameterIsNull);
                return result;
            }

            if (p.UndoTransactionKeys == null || p.UndoTransactionKeys.Count == 0)
            {
                result.Code = 1002;
                result.Message = string.Format("{0} {1}"
                                                , "被撤销操作"
                                                , StringResource.ParameterIsNull);
                return result;
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
            }
            return result;
        }

        /// <summary>
        /// 执行操作。
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        MethodReturnResult ILotUndo.Execute(UndoParameter p)
        {
            MethodReturnResult result = new MethodReturnResult();
            MethodReturnResult resultLotLock = new MethodReturnResult();

            bool isLotStateLock = false;            //批次状态锁定
            List<Lot> lockLots = new List<Lot>();

            try
            {
                List<LotTransaction> lstUndoLotTransUpdate = new List<LotTransaction>();                    //批次撤销事物列表
                List<Lot> lstLotForUpdate = new List<Lot>();
                List<Lot> lstLotForDelete = new List<Lot>();
                List<LotTransaction> lstLotTransInsert = new List<LotTransaction>();                        //批次新增事物列表                
                List<LotTransactionHistory> lstLotTransHisInsert = new List<LotTransactionHistory>();       //批次历史事物列表                
                List<LotTransactionParameter> lstLotTransParamInsert = new List<LotTransactionParameter>(); //批次属性列表
                List<MaterialLoadingDetail> lstMaterialLoadingDetailUpdate = new List<MaterialLoadingDetail>();          //上料数据更新列表
                List<LotBOM> lstLotBOMDelete = new List<LotBOM>();                                          //批次扣料数据删除列表
                List<WorkOrder> lstWorkOrderUpdate = new List<WorkOrder>();                                 //工单属性更新对象列表
                
                DateTime now = DateTime.Now;
                string undoTransactionKey = "";
                string transaciontKey = "";
                PagingConfig cfg = null;
                IList<LotBOM> lstLotBom = null;

                //循环处理批次
                //foreach (string lotNumber in p.LotNumbers)
                //{
                for (int i = 0; i < p.LotNumbers.Count; i++)
                {
                    string lotNumber = p.LotNumbers[i];
                    #region 1.取得撤销事物
                    //确定需要撤销事物主键
                    undoTransactionKey = p.UndoTransactionKeys[lotNumber][0];
                    //undoTransactionKey = p.UndoTransactionKeys[lotNumber][0];

                    //更新操作记录。
                    LotTransaction undotrans = this.LotTransactionDataEngine.Get(undoTransactionKey);

                    if (undotrans == null)
                    {
                        result.Code = 2001;
                        result.Message = string.Format("批次（{0}）事物主键（{1}）提取失败！"
                                                        , lotNumber
                                                        , undoTransactionKey);
                        return result;
                    }

                    //不进行拆包操作
                    if (undotrans.Activity == EnumLotActivity.Package)
                    {
                        result.Code = 2002;
                        result.Message = string.Format("批次（{0}）在（{1}）不能被撤销。"
                                                        , lotNumber
                                                        , undotrans.RouteStepName + ":" + undotrans.Activity.GetDisplayName());
                        return result;
                    }

                    //获取比需要撤销操作时间更小的操作记录
                    cfg = new PagingConfig()
                    {
                        PageNo = 0,
                        PageSize = 1,
                        Where = string.Format(@"LotNumber = '{0}' 
                                                AND CreateTime > '{1:yyyy-MM-dd HH:mm:ss.fff}' 
                                                AND UndoFlag = 0
                                                AND Activity <> -1"
                                            , lotNumber
                                            , undotrans.CreateTime)
                    };

                    IList<LotTransaction> lstTrans = this.LotTransactionDataEngine.Get(cfg);

                    if (lstTrans.Count > 0)
                    {
                        result.Code = 2002;
                        result.Message = string.Format("批次（{0}）进行了新的操作，指定操作（{1}）不能被撤销。"
                                                        , lotNumber
                                                        , undotrans.RouteStepName + ":" + undotrans.Activity.GetDisplayName());
                        return result;
                    }

                    transaciontKey = Guid.NewGuid().ToString();

                    //设置属性
                    undotrans.UndoTransactionKey = transaciontKey;  //撤销事物主键
                    undotrans.UndoFlag = true;                      //撤销标记

                    //加入事物处理列表
                    lstUndoLotTransUpdate.Add(undotrans);
                    #endregion

                    #region 2.设置撤销批后次对象属性
                    //取得当前批次对象
                    Lot lot = this.LotDataEngine.Get(lotNumber);

                    #region 检验并锁定批次
                    //批次在处理中
                    if (lot.LotState == 1)
                    {
                        result.Code = 1003;
                        result.Message = string.Format("批次（{0}）正在处理锁定中！", lotNumber);

                        return result;
                    }

                    lockLots.Add(lot.Clone() as Lot);
                    if (lot.LotState != 1 && isLotStateLock == false) 
                    {
                        result = SetLotStateForLock(lockLots, true);
                    }
                  

                    if (result.Code > 0)
                    {
                        return result;
                    }

                    isLotStateLock = true;
                    #endregion

                    if (undotrans.Activity == EnumLotActivity.Create)
                    {
                        lstLotForDelete.Add(lot);

                        #region 更新工单信息

                        WorkOrder wo = this.WorkOrderDataEngine.Get(lot.OrderNumber);
                        if (lstWorkOrderUpdate.Count > 0)
                        {
                            foreach (WorkOrder items in lstWorkOrderUpdate)
                            {
                                //如果工单列表中已存在该工单信息
                                if (items.Key == wo.Key)
                                {
                                    items.WIPQuantity -= 1;
                                    items.LeftQuantity += 1;
                                    wo.Editor = p.Creator;   //编辑人
                                    wo.EditTime = now;       //编辑时间
                                }
                             
                            }
                        }
                        else 
                        {

                            //如果工单列表中不存在该工单信息，更新工单记录
                            wo.WIPQuantity -= 1;     //工单WIP数量
                            wo.LeftQuantity += 1;    //未投批数量
                            wo.Editor = p.Creator;   //编辑人
                            wo.EditTime = now;       //编辑时间
                            //更新工单信息
                            lstWorkOrderUpdate.Add(wo);
                        }

                     

                      
                        #endregion
                    }
                    else
                    {
                        //取得撤销批次事物对象
                        LotTransactionHistory undoLotHistory = this.LotTransactionHistoryDataEngine.Get(undoTransactionKey);

                        //根据撤销批次事物对象创建撤销后批次信息
                        Lot undoLot = new Lot(undoLotHistory);

                        if (lot.StateFlag == EnumLotState.WaitTrackOut) 
                        {
                            lot.StateFlag = EnumLotState.WaitTrackIn;//如果批次为等待出站状态，进行批次撤销操作后，批次仍为原工序，批次状态为等待进站状态
                        
                        }
                        else if (lot.StateFlag == EnumLotState.WaitTrackIn)//如果批次为等待进站状态，进行批次撤销操作后，批次仍为上一工序，批次状态为等待出站状态
                        {
                             PagingConfig cfg1 = new PagingConfig()
                            {
                                IsPaging = false,
                                OrderBy = "SortSeq",
                                Where = string.Format("Key.RouteName='{0}' AND Key.RouteStepName='{1}'"
                                                        , lot.RouteName
                                                        , lot.RouteStepName
                                                        )
                            };

                            IList<RouteStep> lstRouteStep = this.RouteStepDataEngine.Get(cfg1);
                            if (lstRouteStep.Count > 0) 
                            {
                                PagingConfig cfg2 = new PagingConfig()
                                {
                                    IsPaging = false,         
                                    Where = string.Format("SortSeq='{0}' AND Key.RouteName='{1}'"
                                                            , lstRouteStep[0].SortSeq - 1
                                                            , lot.RouteName
                                                            )
                                };

                                IList<RouteStep> lstRouteStep1 = this.RouteStepDataEngine.Get(cfg2);
                                if (lstRouteStep1.Count > 0)
                                {
                                    lot.RouteStepName = lstRouteStep1[0].Key.RouteStepName;
                                    lot.StateFlag = EnumLotState.WaitTrackOut;

                                }
                                else
                                {
                                    result.Code = 2006;
                                    result.Message = string.Format("批次（{0}）在（{1}）工序上，无法进行批次撤销"
                                                                    , lotNumber
                                                                    , lot.RouteStepName);
                                    return result;

                                }
                            
                            }

                        }
                        //修改属性
                        undoLot.Editor = p.Creator;
                        undoLot.EditTime = now;

                        //加入批次事物列表
                        lstLotForUpdate.Add(undoLot);
                    }
                    #endregion                    

                    #region 3.生成操作事务对象
                    LotTransaction transObj = new LotTransaction()
                    {
                        Key = transaciontKey,
                        Activity = EnumLotActivity.Undo,
                        CreateTime = now,
                        Creator = p.Creator,
                        Description = p.Remark,
                        Editor = p.Creator,
                        EditTime = now,
                        InQuantity = undotrans.InQuantity,
                        LotNumber = lotNumber,
                        LocationName = undotrans.LocationName,
                        LineCode = undotrans.LineCode,
                        OperateComputer = p.OperateComputer,
                        OrderNumber = undotrans.OrderNumber,
                        OutQuantity = undotrans.OutQuantity,
                        RouteEnterpriseName = undotrans.RouteEnterpriseName,
                        RouteName = undotrans.RouteName,
                        RouteStepName = undotrans.RouteStepName,
                        ShiftName = p.ShiftName,
                        UndoFlag = false,
                        UndoTransactionKey = null
                    };

                    //加入事物新增列表
                    lstLotTransInsert.Add(transObj);
                    
                    //新增批次历史记录。
                    LotTransactionHistory lotHistory = new LotTransactionHistory(transaciontKey, lot);

                    //加入批次状态新增列表
                    lstLotTransHisInsert.Add(lotHistory);
                    #endregion

                    #region 4.有附加参数记录附加参数数据
                    if (p.Paramters != null && p.Paramters.ContainsKey(lotNumber))
                    {
                        foreach (TransactionParameter tp in p.Paramters[lotNumber])
                        {
                            LotTransactionParameter lotParamObj = new LotTransactionParameter()
                            {
                                Key = new LotTransactionParameterKey()
                                {
                                    TransactionKey = transaciontKey,
                                    ParameterName = tp.Name,
                                    ItemNo = tp.Index,
                                },
                                ParameterValue = tp.Value,
                                Editor = p.Creator,
                                EditTime = now
                            };

                            lstLotTransParamInsert.Add(lotParamObj);
                        }
                    }
                    #endregion

                    #region 5.处理上料、扣料记录
                    //获取批次过站物料数据
                    cfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format("TransactionKey = '{0}'", undoTransactionKey)
                    };

                    lstLotBom = this.LotBOMDataEngine.Get(cfg);

                    //循环处理上料、扣料数据
                    foreach (LotBOM item in lstLotBom)
                    {
                        if (item.MaterialFrom == EnumMaterialFrom.Loading && item.LoadingItemNo != null)
                        {                                                             
                            foreach (MaterialLoadingDetail items in lstMaterialLoadingDetailUpdate)
                            {
                                //如果上料更新列表已存在该上料号及序号
                                if (items.Key.LoadingKey == item.LoadingKey && items.Key.ItemNo == item.LoadingItemNo.Value)
                                {
                                    items.CurrentQty += item.Qty;
                                }                              
                            }

                            //如果上料更新列表不存在该上料号及序号
                            List<string> lstKey = new List<string>();
                            foreach (MaterialLoadingDetail items in lstMaterialLoadingDetailUpdate)
                            {
                                lstKey.Add(items.Key.LoadingKey);
                            }
                            if (!lstKey.Contains(item.LoadingKey))
                            {
                                #region 5.1 处理上料记录
                                //更新对应的上料数据
                                MaterialLoadingDetail mld = this.MaterialLoadingDetailDataEngine.Get(new MaterialLoadingDetailKey()
                                {
                                    LoadingKey = item.LoadingKey,
                                    ItemNo = item.LoadingItemNo.Value
                                });

                                if (mld == null)
                                {
                                    result.Code = 2003;
                                    result.Message = string.Format("批次[{0}]对应上料记录[{1}]提取失败！"
                                                                    , lotNumber
                                                                    , item.LoadingKey + ":" + item.LoadingItemNo.Value);
                                    return result;
                                }

                                mld.CurrentQty += item.Qty;

                                lstMaterialLoadingDetailUpdate.Add(mld);

                                #endregion  
                            }                                                                   
                        }

                        //物料扣料删除记录
                        lstLotBOMDelete.Add(item);
                    }

                    #endregion
                }

                #region 6.事物处理
                ISession session = this.LotDataEngine.SessionFactory.OpenSession();
                ITransaction transaction = session.BeginTransaction();

                try
                {
                    //6.1更新批次撤销事物
                    foreach (LotTransaction lotTran in lstUndoLotTransUpdate)
                    {
                        this.LotTransactionDataEngine.Update(lotTran, session);
                    }
                    
                    //6.2更新批次
                    foreach (Lot lot in lstLotForUpdate)
                    {
                        this.LotDataEngine.Update(lot, session);
                    }

                    //6.3删除批次
                    foreach (Lot lot in lstLotForDelete)
                    {
                        this.LotDataEngine.Delete(lot.Key, session);
                    }

                    //6.4批次新增事物
                    foreach (LotTransaction lotTran in lstLotTransInsert)
                    {
                        this.LotTransactionDataEngine.Insert(lotTran, session);
                    }

                    //6.5批次历史事物
                    foreach (LotTransactionHistory lotTransHis in lstLotTransHisInsert)
                    {
                        this.LotTransactionHistoryDataEngine.Insert(lotTransHis, session);
                    }

                    //6.6批次属性
                    foreach (LotTransactionParameter lotTransParam in lstLotTransParamInsert)
                    {
                        this.LotTransactionParameterDataEngine.Insert(lotTransParam, session);
                    }

                    //6.7上料数据更新
                    foreach (MaterialLoadingDetail mld in lstMaterialLoadingDetailUpdate)
                    {
                        this.MaterialLoadingDetailDataEngine.Update(mld, session);
                    }                    

                    //6.8扣料数据删除
                    foreach (LotBOM lotBom in lstLotBOMDelete)
                    {
                        this.LotBOMDataEngine.Delete(lotBom.Key, session);
                    }

                    //6.9工单属性更新
                    foreach (WorkOrder wo in lstWorkOrderUpdate)
                    {
                        this.WorkOrderDataEngine.Update(wo, session);
                    }
                    
                    //提交事务
                    //transaction.Rollback();
                    transaction.Commit();
                    session.Close();

                    //取消批次锁定
                    if (isLotStateLock)
                    {
                        resultLotLock = SetLotStateForLock(lockLots, false);

                        if (resultLotLock.Code != 0)
                        {
                            result.Message = result.Message + resultLotLock.Message;
                        }
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    session.Close();

                    result.Code = 2000;
                    result.Message = string.Format(StringResource.Error, ex.Message);

                    //取消批次锁定
                    if (isLotStateLock)
                    {
                        resultLotLock = SetLotStateForLock(lockLots, false);

                        if (resultLotLock.Code != 0)
                        {
                            result.Message = result.Message + resultLotLock.Message;
                        }
                    }
                }
                #endregion
            }
            catch (Exception err)
            {
                result.Code = 1000;

                result.Message += string.Format(StringResource.Error, err.Message) + err.StackTrace;
                result.Detail = err.ToString();

                //取消批次锁定
                if (isLotStateLock)
                {
                    resultLotLock = SetLotStateForLock(lockLots, false);

                    if (resultLotLock.Code != 0)
                    {
                        result.Message = result.Message + resultLotLock.Message;
                    }
                }
            }

            return result;
        }

//        MethodReturnResult ILotUndo.Execute(UndoParameter p)
//        {
//            DateTime now = DateTime.Now;
//            MethodReturnResult result = new MethodReturnResult()
//            {
//                Code = 0
//            };

//            p.TransactionKeys = new Dictionary<string, string>();

//            try
//            {
//                //循环批次。
//                foreach (string lotNumber in p.LotNumbers)
//                {
//                    if (!p.UndoTransactionKeys.ContainsKey(lotNumber))
//                    {
//                        continue;
//                    }

//                    //取得当前事物对象

//                    //生成操作事务主键。
//                    string transaciontKey = Guid.NewGuid().ToString();
//                    p.TransactionKeys.Add(lotNumber, transaciontKey);

//                    Lot lot = this.LotDataEngine.Get(lotNumber);

//                    #region//记录操作历史。
//                    LotTransaction transObj = new LotTransaction()
//                    {
//                        Key = transaciontKey,
//                        Activity = EnumLotActivity.Undo,
//                        CreateTime = now,
//                        Creator = p.Creator,
//                        Description = p.Remark,
//                        Editor = p.Creator,
//                        EditTime = now,
//                        InQuantity = lot.Quantity,
//                        LotNumber = lotNumber,
//                        LocationName = lot.LocationName,
//                        LineCode = lot.LineCode,
//                        OperateComputer = p.OperateComputer,
//                        OrderNumber = lot.OrderNumber,
//                        OutQuantity = lot.Quantity,
//                        RouteEnterpriseName = lot.RouteEnterpriseName,
//                        RouteName = lot.RouteName,
//                        RouteStepName = lot.RouteStepName,
//                        ShiftName = p.ShiftName,
//                        UndoFlag = false,
//                        UndoTransactionKey = null
//                    };
//                    this.LotTransactionDataEngine.Insert(transObj);

//                    //新增批次历史记录。
//                    LotTransactionHistory lotHistory = new LotTransactionHistory(transaciontKey, lot);
//                    this.LotTransactionHistoryDataEngine.Insert(lotHistory);
//                    #endregion

//                    #region //有附加参数记录附加参数数据。
//                    if (p.Paramters != null && p.Paramters.ContainsKey(lotNumber))
//                    {
//                        foreach (TransactionParameter tp in p.Paramters[lotNumber])
//                        {
//                            LotTransactionParameter lotParamObj = new LotTransactionParameter()
//                            {
//                                Key = new LotTransactionParameterKey()
//                                {
//                                    TransactionKey = transaciontKey,
//                                    ParameterName = tp.Name,
//                                    ItemNo = tp.Index,
//                                },
//                                ParameterValue = tp.Value,
//                                Editor = p.Creator,
//                                EditTime = now
//                            };
//                            this.LotTransactionParameterDataEngine.Insert(lotParamObj);
//                        }
//                    }
//                    #endregion

//                    foreach (string undoTransactionKey in p.UndoTransactionKeys[lotNumber])
//                    {
//                        //更新操作记录。
//                        LotTransaction trans = this.LotTransactionDataEngine.Get(undoTransactionKey);
//                        //获取比需要撤销操作时间更小的操作记录
//                        PagingConfig cfg1 = new PagingConfig()
//                        {
//                            PageNo = 0,
//                            PageSize = 1,
//                            Where = string.Format(@"LotNumber='{0}' 
//                                                AND CreateTime>'{1:yyyy-MM-dd HH:mm:ss.fffffff}' 
//                                                AND UndoFlag=0
//                                                AND Activity>-1"
//                                                , lotNumber
//                                                , trans.CreateTime)
//                        };
//                        IList<LotTransaction> lstTrans = this.LotTransactionDataEngine.Get(cfg1);
//                        if (lstTrans.Count > 0)
//                        {
//                            result.Code = 1002;
//                            result.Message = string.Format("批次（{0}）进行了新的操作，指定操作（{1}）不能被撤销。"
//                                                            , lotNumber
//                                                            , trans.RouteStepName + ":" + trans.Activity.GetDisplayName());
//                            return result;
//                        }

//                        LotTransaction transUpdate = trans.Clone() as LotTransaction;
//                        transUpdate.UndoFlag = true;
//                        transUpdate.UndoTransactionKey = transaciontKey;
//                        this.LotTransactionDataEngine.Update(transUpdate);

//                        //获取批次操作历史记录
//                        LotTransactionHistory hisLot = this.LotTransactionHistoryDataEngine.Get(undoTransactionKey);

//                        #region //如果是包装操作。
//                        if (trans.Activity == EnumLotActivity.Package)
//                        {
//                            //获取操作明细记录
//                            LotTransactionPackage transPackageObj = this.LotTransactionPackageDataEngine.Get(undoTransactionKey);
//                            if (transPackageObj == null)
//                            {
//                                continue;
//                            }
//                            //获取包装对象。
//                            Package packageObj = this.PackageDataEngine.Get(transPackageObj.PackageNo);
//                            //更新包装记录。
//                            Package packageObjUpdate = packageObj.Clone() as Package;
//                            packageObjUpdate.Quantity -= hisLot.Quantity;
//                            if (packageObjUpdate.Quantity <= 0)
//                            {
//                                packageObjUpdate.Quantity = 0;
//                                packageObjUpdate.PackageState = EnumPackageState.Packaging;
//                                packageObjUpdate.OrderNumber = null;
//                                packageObjUpdate.MaterialCode = null;
//                            }
//                            packageObjUpdate.Editor = p.Creator;
//                            packageObjUpdate.EditTime = now;
//                            this.PackageDataEngine.Update(packageObjUpdate);
//                            //删除批次包装明细记录。
//                            this.PackageDetailDataEngine.Delete(new PackageDetailKey()
//                            {
//                                PackageNo = transPackageObj.PackageNo,
//                                ObjectNumber = hisLot.LotNumber,
//                                ObjectType = EnumPackageObjectType.Lot
//                            });
//                            //更新包装明细的项目号。
//                            PagingConfig cfg = new PagingConfig()
//                            {
//                                IsPaging = false,
//                                Where = string.Format("Key.PackageNo='{0}'", transPackageObj.PackageNo),
//                                OrderBy = "ItemNo"
//                            };
//                            IList<PackageDetail> lstPackageDetail = this.PackageDetailDataEngine.Get(cfg);
//                            int itemNo = 0;
//                            foreach (PackageDetail packageDetailObj in lstPackageDetail)
//                            {
//                                itemNo++;
//                                if (packageDetailObj.ItemNo == itemNo)
//                                {
//                                    continue;
//                                }
//                                PackageDetail packageDetailObjUpdate = packageDetailObj.Clone() as PackageDetail;
//                                packageDetailObjUpdate.ItemNo = itemNo;
//                                this.PackageDetailDataEngine.Update(packageDetailObjUpdate);
//                            }
//                        }
//                        #endregion

//                        //撤销批次进站。
//                        else if (trans.Activity == EnumLotActivity.TrackIn)
//                        {
//                            LotTransactionEquipment lteObj = this.LotTransactionEquipmentDataEngine.Get(trans.Key);
//                            if (lteObj != null)
//                            {
//                                LotTransactionEquipment lteObjUpdate = lteObj.Clone() as LotTransactionEquipment;
//                                lteObjUpdate.State = EnumLotTransactionEquipmentState.Deleted;
//                                this.LotTransactionEquipmentDataEngine.Update(lteObjUpdate);
//                            }
//                        }

//                        //撤销批次出站。
//                        else if (trans.Activity == EnumLotActivity.TrackOut)
//                        {
//                            PagingConfig cfg = new PagingConfig()
//                            {
//                                IsPaging = false,
//                                Where = string.Format("EndTransactionKey='{0}'", trans.Key)
//                            };
//                            IList<LotTransactionEquipment> lst = this.LotTransactionEquipmentDataEngine.Get(cfg);
//                            foreach (LotTransactionEquipment lteObj in lst)
//                            {
//                                LotTransactionEquipment lteObjUpdate = lteObj.Clone() as LotTransactionEquipment;
//                                lteObjUpdate.State = EnumLotTransactionEquipmentState.Start;
//                                //如果开始操作主键和结束主键一致，则设置为删除标志。
//                                if (lteObjUpdate.Key == lteObjUpdate.EndTransactionKey)
//                                {
//                                    lteObjUpdate.State = EnumLotTransactionEquipmentState.Deleted;
//                                }
//                                lteObjUpdate.EndTime = null;
//                                lteObjUpdate.EndTransactionKey = null;
//                                lteObjUpdate.Editor = hisLot.Editor;
//                                lteObjUpdate.EditTime = hisLot.EditTime;
//                                this.LotTransactionEquipmentDataEngine.Update(lteObjUpdate);
//                            }
//                        }
//                        else if (trans.Activity == EnumLotActivity.Rework)
//                        {
//                            Package packageObj = this.PackageDataEngine.Get(hisLot.PackageNo);
//                            if (packageObj != null)
//                            {
//                                //更新包装记录。
//                                Package packageObjUpadte = packageObj.Clone() as Package;
//                                packageObjUpadte.Quantity += hisLot.Quantity;
//                                if (packageObjUpadte.PackageState == EnumPackageState.Packaging)
//                                {
//                                    EnumPackageState packageState = EnumPackageState.Packaged;
//                                    if (hisLot.ShippedFlag)
//                                    {
//                                        packageState = EnumPackageState.Shipped;
//                                    }
//                                    else if (hisLot.StateFlag == EnumLotState.ToWarehouse)
//                                    {
//                                        packageState = EnumPackageState.ToWarehouse;
//                                    }
//                                    packageObjUpadte.PackageState = packageState;
//                                }
//                                this.PackageDataEngine.Update(packageObjUpadte);

//                                //新增包装明细数据。
//                                int itemNo = 1;
//                                int.TryParse(hisLot.Description, out itemNo);

//                                PackageDetail pdObj = new PackageDetail()
//                                {
//                                    Key = new PackageDetailKey()
//                                    {
//                                        PackageNo = hisLot.PackageNo,
//                                        ObjectType = EnumPackageObjectType.Lot,
//                                        ObjectNumber = hisLot.LotNumber
//                                    },
//                                    CreateTime = hisLot.EditTime,
//                                    Creator = hisLot.Editor,
//                                    MaterialCode = hisLot.MaterialCode,
//                                    OrderNumber = hisLot.OrderNumber,
//                                    ItemNo = itemNo
//                                };
//                                this.PackageDetailDataEngine.Insert(pdObj);
//                            }
//                        }

//                        //获取批次创建时物料数据
//                        IList<LotBOM> lstLotBom = new List<LotBOM>();
//                        cfg1 = new PagingConfig()
//                        {
//                            IsPaging = false,
//                            Where = string.Format("TransactionKey='{0}'", undoTransactionKey)
//                        };
//                        lstLotBom = this.LotBOMDataEngine.Get(cfg1);

//                        foreach (LotBOM item in lstLotBom)
//                        {
//                            if (item.MaterialFrom == EnumMaterialFrom.Loading && item.LoadingItemNo != null)
//                            {
//                                //更新对应的上料数据
//                                MaterialLoadingDetail mld = this.MaterialLoadingDetailDataEngine.Get(new MaterialLoadingDetailKey()
//                                {
//                                    LoadingKey = item.LoadingKey,
//                                    ItemNo = item.LoadingItemNo.Value
//                                });
//                                if (mld != null)
//                                {
//                                    MaterialLoadingDetail mldUpdate = mld.Clone() as MaterialLoadingDetail;
//                                    mldUpdate.CurrentQty += item.Qty;
//                                    this.MaterialLoadingDetailDataEngine.Update(mldUpdate);
//                                }
//                            }
//                            else
//                            {
//                                //更新对应线边仓
//                                LineStoreMaterialDetail lsmd = this.LineStoreMaterialDetailDataEngine.Get(new LineStoreMaterialDetailKey()
//                                {
//                                    LineStoreName = item.LineStoreName,
//                                    OrderNumber = hisLot != null ? hisLot.OrderNumber : lot.OrderNumber,
//                                    MaterialCode = item.MaterialCode,
//                                    MaterialLot = item.Key.MaterialLot
//                                });

//                                if (lsmd != null)
//                                {
//                                    LineStoreMaterialDetail lsmdUpdate = lsmd.Clone() as LineStoreMaterialDetail;
//                                    lsmdUpdate.CurrentQty += item.Qty;
//                                    this.LineStoreMaterialDetailDataEngine.Update(lsmdUpdate);
//                                }
//                            }
//                            this.LotBOMDataEngine.Delete(item.Key);
//                        }

//                        //更新批次记录。
//                        if (hisLot != null)
//                        {
//                            Lot lotUpdate = new Lot(hisLot);
//                            this.LotDataEngine.Update(lotUpdate);

//                            if (trans.Activity == EnumLotActivity.Create)
//                            {
//                                this.LotDataEngine.Delete(lotNumber);

//                                #region 工单信息

//                                WorkOrder wo = this.WorkOrderDataEngine.Get(lot.OrderNumber);

//                                //更新工单记录
//                                //判断创建批次数量是否超出工单未建批次数量
//                                wo.WIPQuantity -= 1;     //工单WIP数量
//                                wo.LeftQuantity += 1;    //未投批数量
//                                wo.Editor = p.Creator;   //编辑人
//                                wo.EditTime = now;       //编辑时间

//                                //更新工单信息
//                                this.WorkOrderDataEngine.Update(wo);

//                                #endregion
//                            }
//                        }
//                        else
//                        {
//                            this.LotDataEngine.Delete(lotNumber);

//                            #region 工单信息

//                            WorkOrder wo = this.WorkOrderDataEngine.Get(lot.OrderNumber);

//                            //更新工单记录
//                            //判断创建批次数量是否超出工单未建批次数量
//                            wo.WIPQuantity -= 1;     //工单WIP数量
//                            wo.LeftQuantity += 1;    //未投批数量
//                            wo.Editor = p.Creator;   //编辑人
//                            wo.EditTime = now;       //编辑时间

//                            //更新工单信息
//                            this.WorkOrderDataEngine.Update(wo);

//                            #endregion
//                        }
//                    }
//                }
//            }
//            catch (Exception err)
//            {
//                result.Code = 1000;

//                result.Message += string.Format(StringResource.Error, err.Message) + err.StackTrace;
//                result.Detail = err.ToString();
//            }

//            return result;
//        }

        /// <summary>
        /// 立即将批次状态置为锁定状态
        /// </summary>
        /// <param name="lots">批次对象列表</param>
        /// <param name="bLock">锁定标识</param>
        /// <returns></returns>
        public MethodReturnResult SetLotStateForLock(List<Lot> lots, bool bLock)
        {
            MethodReturnResult result = new MethodReturnResult();
            ITransaction transactioneqp = null;
            ISession session = null;

            try
            {
                session = this.SessionFactory.OpenSession();
                transactioneqp = session.BeginTransaction();
                
                //设置状态
                foreach (Lot lot in lots)
                {
                    //重新提取批次对象
                    Lot lotcur = this.LotDataEngine.Get(lot.Key);

                    if (bLock)
                    {
                        if (lotcur.LotState == 1)
                        {
                            result.Code = 3000;
                            result.Message = string.Format("批次（{0}）在加工中，请稍后操作。"
                                                            , lot.Key);

                            return result;
                        }

                        lotcur.LotState = 1;           //临时使用该标识锁定批次
                    }
                    else
                    {
                        lotcur.LotState = 0;           //临时使用该标识锁定批次
                    }

                    //更新设备Transaction信息
                    this.LotDataEngine.Update(lotcur, session);
                }


                transactioneqp.Commit();
                session.Close();

                result.Code = 0;
            }
            catch (Exception ex)
            {
                transactioneqp.Rollback();
                session.Close();

                result.Code = 1000;
                result.Message = string.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }

            return result;
        }


        /// <summary>获取最新的一条加工历史数据（从存储过程获取）</summary>
        /// <param name="p"></param>
        /// <returns></returns>
        /// 
        public MethodReturnResult<DataSet> GetLotProcessing(ref LotProcessingParameter p)
        {
            string strErrorMessage = string.Empty;
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();
            try
            {
                using (DbConnection con = this.query_db.CreateConnection())
                {
                    //调用存储过程
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "sp_RPT_LotProcessing";

                    //存储过程传递的参数
                    this.query_db.AddInParameter(cmd, "@LotList", DbType.String, p.Lotlist);
                    this.query_db.AddInParameter(cmd, "@PageNo", DbType.Int32, p.PageNo + 1);
                    this.query_db.AddInParameter(cmd, "@PageSize", DbType.Int32, p.PageSize);
                    //返回总记录数
                    this.query_db.AddOutParameter(cmd, "@Records", DbType.Int32, int.MaxValue);
                    cmd.Parameters["@Records"].Direction = ParameterDirection.Output;
                    //设置返回错误信息
                    cmd.Parameters.Add(new SqlParameter("@ErrorMsg", SqlDbType.NVarChar, 500));
                    cmd.Parameters["@ErrorMsg"].Direction = ParameterDirection.Output;

                    //设置返回值
                    SqlParameter parReturn = new SqlParameter("@return", SqlDbType.Int);
                    parReturn.Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add(parReturn);
                    cmd.CommandTimeout = 120;

                    //执行存储过程
                    result.Data = this.query_db.ExecuteDataSet(cmd);

                    //返回总记录数
                    //int a = Convert.ToInt32(cmd.Parameters["@Records"].Value);
                    //p.TotalRecords = Convert.ToInt32(cmd.Parameters["@Records"].Value);

                    //取得返回值
                    int i = (int)cmd.Parameters["@return"].Value;

                    //调用失败返回错误信息
                    if (i == -1)
                    {
                        strErrorMessage = cmd.Parameters["@ErrorMsg"].Value.ToString();
                        result.Code = 1000;
                        result.Message = strErrorMessage;
                        result.Detail = strErrorMessage;
                    }
                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = ex.Message;
                result.Detail = ex.ToString();
            }
            return result;
        }
    }
}
