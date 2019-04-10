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
using ServiceCenter.DataAccess;
using System.ServiceModel.Activation;
using ServiceCenter.MES.DataAccess.Interface.PPM;
using ServiceCenter.MES.Model.EMS;
using NHibernate;
using System.Data;
using ServiceCenter.Service.Client;
using ServiceCenter.MES.DataAccess.Interface.ZPVM;
using ServiceCenter.MES.Model.ZPVM;

namespace ServiceCenter.MES.Service.WIP
{
    /// <summary>
    /// 实现批次进站服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public  partial class LotTrackInService : ILotTrackInContract, ILotTrackInCheck, ILotTrackIn
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
        /// IV测试数据访问对象
        /// </summary>
        public IIVTestDataDataEngine IVTestDataDataEngine
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
               
                result = this.OnExecuted(p);
                if (result.Code > 0)
                {
                    return result;
                }
                sbMessage.Append(result.Message);  
                result.Message = sbMessage.ToString();
            }
            catch (Exception ex)
            {
                LogHelper.WriteLogError("TrackInLot>", ex);
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

//                    string sql = string.Format(@"select top 1 t3.ATTR_4, t2.HOLD_DESCRIPTION  from  WIP_TRANSACTION  t1
//                                                    inner join [dbo].[WIP_TRANSACTION_HOLD_RELEASE]  t2 on  t1.TRANSACTION_KEY=t2.TRANSACTION_KEY
//                                                    inner join WIP_LOT t3  on t3.LOT_NUMBER = t1.LOT_NUMBER  
//                                                    where t1.LOT_NUMBER='{0}'
//                                                    order by t2.HOLD_TIME  desc", lotNumber);
//                    DataTable dt = new DataTable();
//                    using (DBServiceClient client = new DBServiceClient())
//                    {
//                        MethodReturnResult<DataTable> dtResult = client.ExecuteQuery(sql);
//                        if (result.Code == 0)
//                        {
//                            dt = dtResult.Data;
//                        }
//                    }

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

                #region //判断工序是否校验最迟进站时间
                PagingConfig cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format(@" Key.RouteName='{0}' and Key.RouteStepName='{1}'"
                     ,lot.RouteName,lot.RouteStepName)

                };
                bool isCheckFixCycleBeforeTrackIn = false;
                
                double dFixCycleTimeForTrackIn = 0;
                IList<RouteStepAttribute> lstRouteStepAttributeValues = this.RouteStepAttributeDataEngine.Get(cfg);
                if (lstRouteStepAttributeValues != null && lstRouteStepAttributeValues.Count > 0)
                {
                    string val = getAttributeValueFromList(lstRouteStepAttributeValues, "IsCheckFixCycleBeforeTrackIn");
                    if(Boolean.TryParse(val, out isCheckFixCycleBeforeTrackIn)==false)
                    {
                        isCheckFixCycleBeforeTrackIn = false;
                    }

                    if(isCheckFixCycleBeforeTrackIn==true)
                    {
                        val = getAttributeValueFromList(lstRouteStepAttributeValues, "FixCycleTimeForTrackIn");
                        if(double.TryParse(val,out dFixCycleTimeForTrackIn)==false)
                        {
                            dFixCycleTimeForTrackIn = 0;
                        }
                        else
                        {
                            dFixCycleTimeForTrackIn = dFixCycleTimeForTrackIn * 60;
                        }
                        //判断上一个出站工序到此次的进站工序的时间
                        //根据批次当前工艺工步获取下一个工步
                        RouteStepKey rsKey = new RouteStepKey()
                        {
                            RouteName = lot.RouteName,
                            RouteStepName = lot.RouteStepName
                        };
                        RouteStep rsObj = this.RouteStepDataEngine.Get(rsKey);
                       
                        cfg = new PagingConfig()
                        {
                            PageNo = 0,
                            PageSize = 1,
                            Where = string.Format(@"Key.RouteName='{0}'
                                            AND SortSeq <='{1}'"
                                                    , rsObj.Key.RouteName
                                                    , rsObj.SortSeq - 1),
                            OrderBy = "SortSeq desc "
                        };
                        string fromRouteStepName = "";
                        IList<RouteStep> lstRouteStep = this.RouteStepDataEngine.Get(cfg);
                        if (lstRouteStep != null && lstRouteStep.Count > 0)
                        {
                            fromRouteStepName = lstRouteStep[0].Key.RouteStepName;
                        }
                        //从WIP_Transaction_lot表获取此工序出站的时间
                        //select top 1  * from WIP_TRANSACTION where ACTIVITY=2 and LOT_NUMBER='' and Route_name='' and ROUTE_STEP_NAME=''
                        cfg = new PagingConfig()
                        {
                            PageNo = 0,
                            PageSize = 1,
                            Where = string.Format(@"Activity='{0}'
                                            AND LotNumber ='{1}'
                                            AND RouteName ='{2}'
                                            AND RouteStepName ='{3}'
                                            AND UndoFlag ='0' "
                                            , Convert.ToInt32(EnumLotActivity.TrackOut)
                                            , lot.Key
                                            , lot.RouteName
                                            , fromRouteStepName                                           
                                            ),
                            OrderBy = "CreateTime desc "
                        };
                        IList<LotTransaction> lstLotTransaction = this.LotTransactionDataEngine.Get(cfg);
                        if(lstLotTransaction!=null && lstLotTransaction.Count>0)
                        {
                            LotTransaction lotTransaction = lstLotTransaction.FirstOrDefault();
                            System.DateTime dFromTime = System.DateTime.Now;
                            if( lotTransaction.CreateTime!=null)
                            {
                                dFromTime = lotTransaction.CreateTime.Value;
                            }
                            if(DateTime.Compare(dFromTime.AddSeconds(dFixCycleTimeForTrackIn),System.DateTime.Now)>0)
                            {
   
                                result.Code = 1009;
                                result.Message = string.Format("批次（{0}）需要到({1:yyyy-MM-dd hh:mm:ss})后才能进此站。"
                                                                , lotNumber
                                                                , dFromTime.AddSeconds(dFixCycleTimeForTrackIn).ToString()
                                                                );
                                return result;
                            }
                        }
                        lstLotTransaction = null;
                    }
                }
                #endregion

            }
            return result;
        }

        private string getAttributeValueFromList(IList<RouteStepAttribute> lstRouteStepAttributeValues,string attributeName)
        {
            string strValue = "";
            RouteStepAttribute obj=null;
            var lng = from stepAttr in lstRouteStepAttributeValues.AsEnumerable()
                      where stepAttr.Key.AttributeName == attributeName
                      select stepAttr;
            if(lng!=null && lng.Count()>0)
            {
                obj = lng.FirstOrDefault();
                strValue = obj.Value;
            }
            return strValue;
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

            #region define List of DataEngine
            List<Lot> lstLotDataEngineForUpdate = new List<Lot>();
            List<LotTransactionEquipment> lstLotTransactionEquipmentDataEngineForInsert = new List<LotTransactionEquipment>();
            List<LotTransaction> lstLotTransactionForInsert = new List<LotTransaction>();
            List<LotTransactionHistory> lstLotTransactionHistoryForInsert = new List<LotTransactionHistory>();
            List<LotTransactionParameter> lstLotTransactionParameterDataEngineForInsert = new List<LotTransactionParameter>();

            List<Equipment> lstEquipmentForUpdate = new List<Equipment>();
            List<EquipmentStateEvent> lstEquipmentStateEventForInsert = new List<EquipmentStateEvent>();

            List<LotJob> lstLotJobsForInsert = new List<LotJob>();
            List<LotJob> lstLotJobsForUpdate = new List<LotJob>();


            List<IVTestData> lstIVTestDataForUpdate = new List<IVTestData>();
            #endregion

            //循环批次。
            foreach (string lotNumber in p.LotNumbers)
            {
                #region //LOT 循环
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
                lstLotDataEngineForUpdate.Add(lotUpdate);
                //this.LotDataEngine.Update(lotUpdate);

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
                    lstLotTransactionEquipmentDataEngineForInsert.Add(transEquipment);
                    //this.LotTransactionEquipmentDataEngine.Insert(transEquipment);
                }

                #region //更新设备状态
               

                bool blEquipmentContinue = false;
                if (string.IsNullOrEmpty(p.EquipmentCode))
                {
                    blEquipmentContinue = false;
                }
                Equipment e = null;
                if (blEquipmentContinue)
                { 
                    //获取设备数据。
                    e = this.EquipmentDataEngine.Get(p.EquipmentCode);
                    if (e == null)
                    {
                        blEquipmentContinue = false;
                    }
                }
                EquipmentState es = null;
                if (blEquipmentContinue)
                { 
                    //获取设备当前状态。
                    es = this.EquipmentStateDataEngine.Get(e.StateName);
                    if (es == null)
                    {
                        blEquipmentContinue = false;
                    }
                }
                

                #region //blEquipmentContinue =true
                EquipmentState runState = null;
                if (blEquipmentContinue)
                {
                    //获取设备RUN的主键
                    runState = this.EquipmentStateDataEngine.Get("RUN");
                
                    //获取设备当前状态->RUN的状态切换数据。
                    EquipmentChangeState ecsToRun = this.EquipmentChangeStateDataEngine.Get(es.Key, runState.Key);
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

                                lstEquipmentForUpdate.Add(epUpdate);
                                //this.EquipmentDataEngine.Update(epUpdate);

                                //新增设备状态事件数据
                                EquipmentStateEvent newStateEvent = new EquipmentStateEvent()
                                {
                                    Key = Guid.NewGuid().ToString(),
                                    CreateTime = now,
                                    Creator = p.Creator,
                                    Description = p.Remark,
                                    Editor = p.Creator,
                                    EditTime = now,
                                    EquipmentChangeStateName = ecsToRun.Key,
                                    EquipmentCode = e.ParentEquipmentCode,
                                    EquipmentFromStateName = es.Key,
                                    EquipmentToStateName = runState.Key,
                                    IsCurrent = true
                                };
                                lstEquipmentStateEventForInsert.Add(newStateEvent);
                                //this.EquipmentStateEventDataEngine.Insert(newStateEvent);
                            }
                        }
                
                        //更新设备状态。
                        Equipment eUpdate = e.Clone() as Equipment;
                        eUpdate.StateName = runState.Key;
                        eUpdate.ChangeStateName = ecsToRun.Key;
                        lstEquipmentForUpdate.Add(eUpdate);
                        //this.EquipmentDataEngine.Update(eUpdate);


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
                        lstEquipmentStateEventForInsert.Add(stateEvent);
                       // this.EquipmentStateEventDataEngine.Insert(stateEvent);
                    }
                }
                #endregion

                #endregion

                #region //判断是否要设置IV数据无效
                bool isAllowIVTestData = false;
                //获取工步设置是否自动出站。
                RouteStepAttribute rsa = this.RouteStepAttributeDataEngine.Get(new RouteStepAttributeKey()
                {
                    RouteName = lot.RouteName,
                    RouteStepName = lot.RouteStepName,
                    AttributeName = "IsAllowIVTestData"
                });
                if (rsa != null)
                {
                    bool.TryParse(rsa.Value, out isAllowIVTestData);
                }
                if(isAllowIVTestData==true)
                {
                    #region  //判断IV测试数据是否存在。
                    PagingConfig cfg = new PagingConfig()
                    {
                        PageNo = 0,
                        IsPaging=false,
                        Where = string.Format("Key.LotNumber='{0}' AND IsDefault=1", lotNumber)
                    };

                    IList<IVTestData> lstTestData = this.IVTestDataDataEngine.Get(cfg);
                    foreach(IVTestData ivTestData in lstTestData)
                    {
                        IVTestData testData = ivTestData.Clone() as IVTestData;
                        testData.IsDefault = false;
                        testData.Editor = p.Creator;
                        testData.EditTime = DateTime.Now;
                        lstIVTestDataForUpdate.Add(testData);
                    }
                    #endregion
                }
                #endregion

                #region //TrackIn For AutoTrackOut
                bool isAutoTrackOut = true;
                
                //批次状态非等待出站 已暂停 已结束 不可用批次不自动出站。
                if (lot.StateFlag != EnumLotState.WaitTrackOut
                    || lot.Status == EnumObjectStatus.Disabled
                    || lot.DeletedFlag == true
                    || lot.HoldFlag == true)
                {
                    isAutoTrackOut = false;
                }
                if(isAutoTrackOut)
                { 
                    //获取工步设置是否自动出站。
                    rsa = this.RouteStepAttributeDataEngine.Get(new RouteStepAttributeKey()
                    {
                        RouteName = lot.RouteName,
                        RouteStepName = lot.RouteStepName,
                        AttributeName = "IsAutoTrackOut"
                    });

                    if (rsa != null)
                    {
                        bool.TryParse(rsa.Value, out isAutoTrackOut);
                    }
                }
                //Lot自动出站。
                if (isAutoTrackOut)
                {
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
                                                   , lotUpdate.LineCode
                                                   , lotUpdate.RouteStepName)
                        };
                        IList<Equipment> lstEquipment = this.EquipmentDataEngine.Get(cfg1);
                        if (lstEquipment.Count > 0)
                        {
                            equipmentCode = lstEquipment[0].Key;
                        }
                    }
                    //获取工步标准时长，以定时出站。
                    double seconds = 1;  //秒数。
                    RouteStep rsObj = this.RouteStepDataEngine.Get(new RouteStepKey()
                    {
                        RouteName = lotUpdate.RouteName,
                        RouteStepName = lotUpdate.RouteStepName
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
                                                , lotUpdate.Key
                                                , lotUpdate.RouteStepName
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
                            JobName = string.Format("{0} {1}", lotUpdate.Key, lotUpdate.StateFlag.GetDisplayName()),
                            Key = Convert.ToString(Guid.NewGuid()),
                            LineCode = lotUpdate.LineCode,
                            LotNumber = lotUpdate.Key,
                            Type = EnumJobType.AutoTrackOut,
                            RouteEnterpriseName = lotUpdate.RouteEnterpriseName,
                            RouteName = lotUpdate.RouteName,
                            RouteStepName = lotUpdate.RouteStepName,
                            RunCount = 0,
                            Status = EnumObjectStatus.Available,
                            NextRunTime = DateTime.Now.AddSeconds(seconds),
                            NotifyMessage = string.Empty,
                            NotifyUser = string.Empty
                        };
                        lstLotJobsForInsert.Add(job);
                        //this.LotJobDataEngine.Insert(job);
                    }
                    else
                    {
                        LotJob jobUpdate = lstJob[0].Clone() as LotJob;
                        jobUpdate.NextRunTime = DateTime.Now.AddSeconds(seconds);
                        jobUpdate.LineCode = lotUpdate.LineCode;
                        jobUpdate.EquipmentCode = equipmentCode;
                        jobUpdate.RunCount = 0;
                        jobUpdate.Editor = p.Creator;
                        jobUpdate.EditTime = DateTime.Now;
                        lstLotJobsForUpdate.Add(jobUpdate);
                        //this.LotJobDataEngine.Update(jobUpdate);
                    }
                }
                #endregion

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
                lstLotTransactionForInsert.Add(transObj);
                //this.LotTransactionDataEngine.Insert(transObj);
                //新增批次历史记录。
                LotTransactionHistory lotHistory = new LotTransactionHistory(transactionKey, lot);
           
                lstLotTransactionHistoryForInsert.Add(lotHistory);
                //this.LotTransactionHistoryDataEngine.Insert(lotHistory);
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
                        lstLotTransactionParameterDataEngineForInsert.Add(lotParamObj);
                        //this.LotTransactionParameterDataEngine.Insert(lotParamObj);
                    }
                }
                #endregion

                #endregion
            }

            #region //开始事物处理
            ISession session = this.LotDataEngine.SessionFactory.OpenSession();
            ITransaction transaction = session.BeginTransaction();
            try
            {
                

                #region 更新批次LOT 的信息
                //更新批次基本信息
                foreach (Lot lot in lstLotDataEngineForUpdate)
                {
                    this.LotDataEngine.Update(lot, session);
                }

                //更新批次LotTransaction信息
                foreach (LotTransaction lotTransaction in lstLotTransactionForInsert)
                {
                    this.LotTransactionDataEngine.Insert(lotTransaction, session);
                }

                //更新批次TransactionHistory信息
                foreach (LotTransactionHistory lotTransactionHistory in lstLotTransactionHistoryForInsert)
                {
                    this.LotTransactionHistoryDataEngine.Insert(lotTransactionHistory, session);
                }

                //LotTransactionParameter
                foreach (LotTransactionParameter lotTransactionParameter in lstLotTransactionParameterDataEngineForInsert)
                {
                    this.LotTransactionParameterDataEngine.Update(lotTransactionParameter, session);
                }
                #endregion

                #region //更新IV数据，IsDefault=0
                foreach (IVTestData ivTestData in lstIVTestDataForUpdate)
                {
                    this.IVTestDataDataEngine.Update(ivTestData, session);
                }
                #endregion

                #region //更新设备基本信息
                //更新设备Transaction信息
                foreach (Equipment equipment in lstEquipmentForUpdate)
                {
                    this.EquipmentDataEngine.Update(equipment, session);
                }

                //Insert设备Transaction信息
                foreach (LotTransactionEquipment lotTransactionEquipment in lstLotTransactionEquipmentDataEngineForInsert)
                {
                    this.LotTransactionEquipmentDataEngine.Insert(lotTransactionEquipment, session);
                }

                //Insert设备Transaction信息
                foreach (EquipmentStateEvent equipmentStateEvent in lstEquipmentStateEventForInsert)
                {
                    this.EquipmentStateEventDataEngine.Insert(equipmentStateEvent, session);
                }
                #endregion

                #region //LOT JOBS
                foreach (LotJob lotJob in lstLotJobsForInsert)
                {
                    this.LotJobDataEngine.Insert(lotJob, session);
                }

                   foreach (LotJob lotJob in lstLotJobsForUpdate)
                {
                    this.LotJobDataEngine.Update(lotJob, session);
                }

                #endregion

                transaction.Commit();
                session.Close();

            }
            catch (Exception err)
            {
                transaction.Rollback();
                session.Close();
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
