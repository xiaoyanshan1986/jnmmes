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
using NHibernate;
using ServiceCenter.MES.DataAccess.Interface.ZPVM;
using ServiceCenter.MES.Model.EMS;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Model.LSM;
using ServiceCenter.MES.Model.BaseData;
using System.Data;
using System.Data.Common;

namespace ServiceCenter.MES.Service.WIP
{

    public partial class WipEngineerService
    {
        public MethodReturnResult TrackInLot(TrackInParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            if (string.IsNullOrEmpty(p.LineCode))
            {
                result.Code = 1001;
                result.Message = string.Format("{0} {1}"
                                                , "线别代码"
                                                , StringResource.ParameterIsNull);
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

            if (p.LotNumbers == null || p.LotNumbers.Count == 0)
            {
                result.Code = 1001;
                result.Message = string.Format("{0} {1}"
                                                , "批次号"
                                                , StringResource.ParameterIsNull);
                return result;
            }
            string lotNumber = p.LotNumbers[0];

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

            #region //判断工序是否校验最迟进站时间
            PagingConfig cfg = new PagingConfig()
            {
                IsPaging = false,
                Where = string.Format(@" Key.RouteName='{0}' and Key.RouteStepName='{1}'"
                    , lot.RouteName, lot.RouteStepName)

            };

            bool isCheckFixCycleBeforeTrackIn = false;
            double dFixCycleTimeForTrackIn = 0;
            IList<RouteStepAttribute> lstRouteStepAttributeValues = this.RouteStepAttributeDataEngine.Get(cfg);

            if (lstRouteStepAttributeValues != null && lstRouteStepAttributeValues.Count > 0)
            {
                string val = getAttributeValueFromList(lstRouteStepAttributeValues, "IsCheckFixCycleBeforeTrackIn");
                if (Boolean.TryParse(val, out isCheckFixCycleBeforeTrackIn) == false)
                {
                    isCheckFixCycleBeforeTrackIn = false;
                }

                if (isCheckFixCycleBeforeTrackIn == true)
                {
                    val = getAttributeValueFromList(lstRouteStepAttributeValues, "FixCycleTimeForTrackIn");
                    if (double.TryParse(val, out dFixCycleTimeForTrackIn) == false)
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
                    if (lstLotTransaction != null && lstLotTransaction.Count > 0)
                    {
                        LotTransaction lotTransaction = lstLotTransaction.FirstOrDefault();
                        System.DateTime dFromTime = System.DateTime.Now;
                        if (lotTransaction.CreateTime != null)
                        {
                            dFromTime = lotTransaction.CreateTime.Value;
                        }
                        if (DateTime.Compare(dFromTime.AddSeconds(dFixCycleTimeForTrackIn), System.DateTime.Now) > 0)
                        {

                            result.Code = 1009;
                            result.Message = string.Format("批次（{0}）未到固话时间，需要到({1:yyyy-MM-dd hh:mm:ss})后才能进此站。"
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

            if (string.IsNullOrEmpty(p.EquipmentCode) == false)
            {
                #region //判断设备状态
                //获取设备数据
                Equipment e = this.EquipmentDataEngine.Get(p.EquipmentCode ?? string.Empty);
                if (e != null)
                {
                    //如果设备状态在不可运行状态，返回错误。
                    //获取设备当前状态。
                    EquipmentState es = this.EquipmentStateDataEngine.Get(e.StateName);
                    if (es != null)
                    {
                        //如果设备状态不是“待产” 和 “运行”
                        if (es.Type != EnumEquipmentStateType.Lost
                            && es.Type != EnumEquipmentStateType.Run
                            && es.Type != EnumEquipmentStateType.Test)
                        {
                            result.Code = 1200;
                            result.Message = String.Format("设备（{0}）状态（{1}）不可用。", e.Key, e.StateName);
                            return result;
                        }
                    }
                }
                #endregion
            }

            try
            {
                result = ExecuteTrackInLot(p, null, false);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLogError("TrackInLot>", ex);
                result.Code = 1000;
                result.Message += string.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }

            return result;
        }
     
        
        public MethodReturnResult ExecuteTrackInLot(TrackInParameter p, ISession db, bool executedWithTransaction)
        {
            Equipment e = null;
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
                #region 更新设备状态
                bool blEquipmentContinue = true;
                if (string.IsNullOrEmpty(p.EquipmentCode))
                {
                    blEquipmentContinue = false;
                }
                

                if (blEquipmentContinue)
                {
                    //获取设备数据。
                    e = this.EquipmentDataEngine.Get(p.EquipmentCode, db);
                    if (e == null)
                    {
                        blEquipmentContinue = false;
                    }
                }

                EquipmentState es = null;
                if (blEquipmentContinue)
                {
                    //获取设备当前状态。
                    es = this.EquipmentStateDataEngine.Get(e.StateName, db);
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
                    runState = this.EquipmentStateDataEngine.Get("RUN", db);

                    //获取设备当前状态->RUN的状态切换数据。
                    EquipmentChangeState ecsToRun = this.EquipmentChangeStateDataEngine.Get(es.Key, runState.Key, db);
                    if (ecsToRun != null)
                    {
                        //更新父设备状态。
                        if (!string.IsNullOrEmpty(e.ParentEquipmentCode))
                        {
                            Equipment ep = this.EquipmentDataEngine.Get(e.ParentEquipmentCode, db);
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
                        //lstEquipmentForUpdate.Add(eUpdate);
                        //this.EquipmentDataEngine.Update(eUpdate);

                        //设备事物处理
                        #region
                        ITransaction transactioneqp = null;

                        db = this.SessionFactory.OpenSession();
                        transactioneqp = db.BeginTransaction();
                        
                        try
                        {                            
                            //更新设备Transaction信息
                            this.EquipmentDataEngine.Update(eUpdate, db);
                            
                            transactioneqp.Commit();
                            db.Close();                            
                        }
                        catch (Exception err)
                        {                            
                            transactioneqp.Rollback();
                            db.Close();
                            
                            result.Code = 1000;
                            result.Message += string.Format(StringResource.Error, err.Message);
                            result.Detail = err.ToString();
                            return result;
                        }
                        #endregion
                        #endregion

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
                
                #region LOT 循环
                Lot lot = this.LotDataEngine.Get(lotNumber, db);

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
                
                #region
                //#region //更新设备状态


                //bool blEquipmentContinue = true;
                //if (string.IsNullOrEmpty(p.EquipmentCode))
                //{
                //    blEquipmentContinue = false;
                //}
                //Equipment e = null;
                //if (blEquipmentContinue)
                //{
                //    //获取设备数据。
                //    e = this.EquipmentDataEngine.Get(p.EquipmentCode, db);
                //    if (e == null)
                //    {
                //        blEquipmentContinue = false;
                //    }
                //}
                //EquipmentState es = null;
                //if (blEquipmentContinue)
                //{
                //    //获取设备当前状态。
                //    es = this.EquipmentStateDataEngine.Get(e.StateName, db);
                //    if (es == null)
                //    {
                //        blEquipmentContinue = false;
                //    }
                //}


                //#region //blEquipmentContinue =true
                //EquipmentState runState = null;
                //if (blEquipmentContinue)
                //{
                //    //获取设备RUN的主键
                //    runState = this.EquipmentStateDataEngine.Get("RUN", db);

                //    //获取设备当前状态->RUN的状态切换数据。
                //    EquipmentChangeState ecsToRun = this.EquipmentChangeStateDataEngine.Get(es.Key, runState.Key, db);
                //    if (ecsToRun != null)
                //    {
                //        //更新父设备状态。
                //        if (!string.IsNullOrEmpty(e.ParentEquipmentCode))
                //        {
                //            Equipment ep = this.EquipmentDataEngine.Get(e.ParentEquipmentCode, db);
                //            if (ep != null)
                //            {
                //                Equipment epUpdate = ep.Clone() as Equipment;
                //                //更新设备状态。
                //                epUpdate.StateName = runState.Key;
                //                epUpdate.ChangeStateName = ecsToRun.Key;

                //                lstEquipmentForUpdate.Add(epUpdate);
                //                //this.EquipmentDataEngine.Update(epUpdate);

                //                //新增设备状态事件数据
                //                EquipmentStateEvent newStateEvent = new EquipmentStateEvent()
                //                {
                //                    Key = Guid.NewGuid().ToString(),
                //                    CreateTime = now,
                //                    Creator = p.Creator,
                //                    Description = p.Remark,
                //                    Editor = p.Creator,
                //                    EditTime = now,
                //                    EquipmentChangeStateName = ecsToRun.Key,
                //                    EquipmentCode = e.ParentEquipmentCode,
                //                    EquipmentFromStateName = es.Key,
                //                    EquipmentToStateName = runState.Key,
                //                    IsCurrent = true
                //                };
                //                lstEquipmentStateEventForInsert.Add(newStateEvent);
                //                //this.EquipmentStateEventDataEngine.Insert(newStateEvent);
                //            }
                //        }

                //        //更新设备状态。
                //        Equipment eUpdate = e.Clone() as Equipment;
                //        eUpdate.StateName = runState.Key;
                //        eUpdate.ChangeStateName = ecsToRun.Key;
                //        lstEquipmentForUpdate.Add(eUpdate);
                //        //this.EquipmentDataEngine.Update(eUpdate);


                //        //新增设备状态事件数据
                //        EquipmentStateEvent stateEvent = new EquipmentStateEvent()
                //        {
                //            Key = Guid.NewGuid().ToString(),
                //            CreateTime = now,
                //            Creator = p.Creator,
                //            Description = p.Remark,
                //            Editor = p.Creator,
                //            EditTime = now,
                //            EquipmentChangeStateName = ecsToRun.Key,
                //            EquipmentCode = e.Key,
                //            EquipmentFromStateName = es.Key,
                //            EquipmentToStateName = runState.Key,
                //            IsCurrent = true
                //        };
                //        lstEquipmentStateEventForInsert.Add(stateEvent);
                //        // this.EquipmentStateEventDataEngine.Insert(stateEvent);
                //    }
                //}
                //#endregion

                //#endregion
                #endregion

                #region 判断是否要设置IV数据无效
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
                if (isAllowIVTestData == true)
                {
                    #region  //判断IV测试数据是否存在。
                    PagingConfig cfg = new PagingConfig()
                    {
                        PageNo = 0,
                        IsPaging = false,
                        Where = string.Format("Key.LotNumber='{0}' AND IsDefault=1", lotNumber)
                    };

                    IList<IVTestData> lstTestData = this.IVTestDataDataEngine.Get(cfg);
                    foreach (IVTestData ivTestData in lstTestData)
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
                    LocationName = lot.LocationName,
                    LineCode = lotUpdate.LineCode,
                    OperateComputer = p.OperateComputer,
                    OrderNumber = lot.OrderNumber,
                    OutQuantity = lotUpdate.Quantity,
                    RouteEnterpriseName = lot.RouteEnterpriseName,
                    RouteName = lot.RouteName,
                    RouteStepName = lot.RouteStepName,
                    ShiftName = p.ShiftName,
                    UndoFlag = false,
                    UndoTransactionKey = null,
                    Grade = lot.Grade,
                    Color = lot.Color,
                    Attr1 = lot.Attr1,
                    Attr2 = lot.Attr2,
                    Attr3 = lot.Attr3,
                    Attr4 = lot.Attr4,
                    Attr5 = lot.Attr5,
                    OriginalOrderNumber = lot.OriginalOrderNumber
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

                #region 判断是否自动出站
                bool isAutoTrackOut = false;

                //获取工步设置是否自动出站。
                rsa = this.RouteStepAttributeDataEngine.Get(new RouteStepAttributeKey()
                {
                    RouteName = lot.RouteName,
                    RouteStepName = lot.RouteStepName,
                    AttributeName = "IsAutoTrackOut"
                }, db);

                if (rsa != null)
                {
                    bool.TryParse(rsa.Value, out isAutoTrackOut);
                }

                //自动出站处理
                if (isAutoTrackOut == true)
                {
                    //判断批次状态是否符合出站条件（非等待出站 已暂停 已结束 不可用批次不自动出站）
                    if ((lot.Status == EnumObjectStatus.Disabled
                        || lot.DeletedFlag == true
                        || lot.HoldFlag == true
                        || lot.StateFlag != EnumLotState.WaitTrackIn) == false)
                    {
                        //初始化参数
                        TrackOutParameter TrackOutParameters = new TrackOutParameter();

                        TrackOutParameters.IsFinished = true;   //工序已结束
                        TrackOutParameters.RouteOperationName = lot.RouteStepName;
                        TrackOutParameters.LineCode = lot.LineCode;
                        TrackOutParameters.EquipmentCode = p.EquipmentCode;
                        TrackOutParameters.LotNumbers = p.LotNumbers;
                        TrackOutParameters.LineCode = p.LineCode;
                        

                        //调用出站操作
                        result = this.TrackOutLot(TrackOutParameters, db, false);
                        

                        if (result != null) 
                        {
                            if (result.Code > 0)
                            {
                                return result;
                            }                            
                        }
                    }
                }
                #endregion

              
                #endregion
            }

            #region //开始事物处理
            ITransaction transaction = null;
            if (executedWithTransaction == true)
            {
                db = this.SessionFactory.OpenSession();
                transaction = db.BeginTransaction();
            }
            try
            {
                #region 更新批次LOT 的信息
                //更新批次基本信息
                foreach (Lot lot in lstLotDataEngineForUpdate)
                {
                    this.LotDataEngine.Update(lot, db);
                }

                //更新批次LotTransaction信息
                foreach (LotTransaction lotTransaction in lstLotTransactionForInsert)
                {
                    this.LotTransactionDataEngine.Insert(lotTransaction, db);
                }

                //更新批次TransactionHistory信息
                foreach (LotTransactionHistory lotTransactionHistory in lstLotTransactionHistoryForInsert)
                {
                    this.LotTransactionHistoryDataEngine.Insert(lotTransactionHistory, db);
                }

                //LotTransactionParameter
                foreach (LotTransactionParameter lotTransactionParameter in lstLotTransactionParameterDataEngineForInsert)
                {
                    this.LotTransactionParameterDataEngine.Update(lotTransactionParameter, db);
                }
                #endregion

                #region //更新设备基本信息
                //更新设备Transaction信息
                foreach (Equipment equipment in lstEquipmentForUpdate)
                {
                    this.EquipmentDataEngine.Update(equipment, db);
                }

                //Insert设备Transaction信息
                foreach (LotTransactionEquipment lotTransactionEquipment in lstLotTransactionEquipmentDataEngineForInsert)
                {
                    this.LotTransactionEquipmentDataEngine.Insert(lotTransactionEquipment, db);
                }

                //Insert设备Transaction信息
                foreach (EquipmentStateEvent equipmentStateEvent in lstEquipmentStateEventForInsert)
                {
                    //this.EquipmentStateEventDataEngine.Insert(equipmentStateEvent, db);
                    this.ExecuteAddEquipmentStateEvent(equipmentStateEvent, db, true);
                }
                #endregion

                #region //更新IV数据，IsDefault=0
                foreach (IVTestData ivTestData in lstIVTestDataForUpdate)
                {
                    this.IVTestDataDataEngine.Update(ivTestData, db);
                }
                #endregion

                #region //LOT JOBS
                foreach (LotJob lotJob in lstLotJobsForInsert)
                {
                    this.LotJobDataEngine.Insert(lotJob, db);
                }

                foreach (LotJob lotJob in lstLotJobsForUpdate)
                {
                    this.LotJobDataEngine.Update(lotJob, db);
                }

                #endregion

                if (executedWithTransaction == false)
                {
                    transaction.Commit();
                    db.Close();
                }
                else
                {
                    db.Flush();
                }
            }
            catch (Exception err)
            {
                if (executedWithTransaction == false)
                {
                    transaction.Rollback();
                    db.Close();

                    #region 设备状态回滚处理
                    ITransaction transactioneqp = null;

                    db = this.SessionFactory.OpenSession();
                    transactioneqp = db.BeginTransaction();

                    try
                    {
                        //更新设备Transaction信息
                        this.EquipmentDataEngine.Update(e, db);

                        transactioneqp.Commit();
                        db.Close();
                    }
                    catch (Exception errep)
                    {
                        transactioneqp.Rollback();
                        db.Close();

                        result.Code = 1000;
                        result.Message += string.Format(StringResource.Error, errep.Message);
                        result.Detail = err.ToString();
                        return result;
                    }
                    #endregion
                }
                result.Code = 1000;
                result.Message += string.Format(StringResource.Error, err.Message);
                result.Detail = err.ToString();
                return result;
            }
            #endregion

            return result;
        }

        /// <summary>
        /// 批次出站
        /// </summary>
        /// <param name="p">出站参数</param>
        /// <param name="db">事物对象</param>
        /// <param name="executedWithTransaction">执行时是否进行事物处理 true - 处理事物 false - 不进行事物处理</param>
        /// <returns></returns>
        public MethodReturnResult TrackOutLot(TrackOutParameter p, ISession db, bool executedWithTransaction)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            #region 出站基础信息校验
            if (string.IsNullOrEmpty(p.RouteOperationName))
            {
                result.Code = 1001;
                result.Message = string.Format("{0} {1}"
                                                , "工序名称"
                                                , StringResource.ParameterIsNull);
                return result;
            }
            if (string.IsNullOrEmpty(p.LineCode))
            {
                result.Code = 1001;
                result.Message = string.Format("{0} {1}"
                                                , "线别代码"
                                                , StringResource.ParameterIsNull);
                return result;
            }
            if (p.LotNumbers == null || p.LotNumbers.Count == 0)
            {
                result.Code = 1001;
                result.Message = string.Format("{0} {1}"
                                                , "批次号"
                                                , StringResource.ParameterIsNull);
                return result;
            }
            string lotNumber = p.LotNumbers[0];
            //获取线别车间。
            string locationName = string.Empty;

            ProductionLine pl = this.ProductionLineDataEngine.Get(p.LineCode);
            if (pl != null)
            {
                //根据线别所在区域，获取车间名称。
                Location loc = this.LocationDataEngine.Get(pl.LocationName);
                locationName = loc.ParentLocationName ?? string.Empty;
            }

            Lot lot = this.LotDataEngine.Get(lotNumber);
            //判定是否存在批次记录。
            if (lot == null || lot.Status == EnumObjectStatus.Disabled)
            {
                result.Code = 1002;
                result.Message = string.Format("批次（{0}）不存在。", lotNumber);
                return result;
            }
            //若进站时未选择设备，判断设备是否选择
            if (string.IsNullOrEmpty(lot.EquipmentCode) && string.IsNullOrEmpty(p.EquipmentCode))
            {
                result.Code = 1009;
                result.Message = string.Format("请选择批次（{0}）加工设备。", lotNumber);
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
            //批次目前非等待出站状态。
            //if (lot.StateFlag != EnumLotState.WaitTrackOut)
            //{
            //    result.Code = 1006;
            //    result.Message = string.Format("批次（{0}）目前状态（{1}）,非({2})状态。"
            //                                    , lotNumber
            //                                    , lot.StateFlag.GetDisplayName()
            //                                    , EnumLotState.WaitTrackOut.GetDisplayName());
            //    return result;
            //}
            //检查批次所在工序是否处于指定工序
            if (lot.RouteStepName != p.RouteOperationName)
            {
                result.Code = 1007;
                result.Message = string.Format("批次（{0}）目前在（{1}）工序，不能在({2})工序操作。"
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
            #endregion

            #region //判断工序是否校验最迟进站时间
            PagingConfig cfg = new PagingConfig()
            {
                IsPaging = false,
                Where = string.Format(@" Key.RouteName='{0}' and Key.RouteStepName='{1}'"
                    , lot.RouteName, lot.RouteStepName)

            };
            bool isCheckFixCycleBeforeTrackOut = false;
            double dFixCycleTimeForTrackOut = 0;
            IList<RouteStepAttribute> lstRouteStepAttributeValues = this.RouteStepAttributeDataEngine.Get(cfg);
            if (lstRouteStepAttributeValues != null && lstRouteStepAttributeValues.Count > 0)
            {
                string val = getAttributeValueFromList(lstRouteStepAttributeValues, "IsCheckFixCycleBeforeTrackOut");
                if (Boolean.TryParse(val, out isCheckFixCycleBeforeTrackOut) == false)
                {
                    isCheckFixCycleBeforeTrackOut = false;
                }

                if (isCheckFixCycleBeforeTrackOut == true)
                {
                    val = getAttributeValueFromList(lstRouteStepAttributeValues, "FixCycleTimeForTrackOut");
                    if (double.TryParse(val, out dFixCycleTimeForTrackOut) == false)
                    {
                        dFixCycleTimeForTrackOut = 0;
                    }
                    else
                    {
                        dFixCycleTimeForTrackOut = dFixCycleTimeForTrackOut * 60;
                    }
                    //判断上一个出站工序到此次的出站工序的时间
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
                    if (lstLotTransaction != null && lstLotTransaction.Count > 0)
                    {
                        LotTransaction lotTransaction = lstLotTransaction.FirstOrDefault();
                        System.DateTime dFromTime = System.DateTime.Now;
                        if (lotTransaction.CreateTime != null)
                        {
                            dFromTime = lotTransaction.CreateTime.Value;
                        }
                        if (DateTime.Compare(dFromTime.AddSeconds(dFixCycleTimeForTrackOut), System.DateTime.Now) > 0)
                        {
                            double curFixMinutes = Math.Round((DateTime.Now - dFromTime).TotalMinutes, 2);
                            result.Code = 1009;
                            result.Message = string.Format("批次（{0}）已固化（{2}）分钟,需要到({1:yyyy-MM-dd hh:mm:ss})后才能出此站。"
                                                            , lotNumber
                                                            , dFromTime.AddSeconds(dFixCycleTimeForTrackOut).ToString(), curFixMinutes
                                                            );
                            return result;
                        }
                    }
                    lstLotTransaction = null;
                }
            }
            #endregion

            #region //检查固化时间校验
            //获取工步属性数据。
            RouteStepAttribute rs = this.RouteStepAttributeDataEngine.Get(new RouteStepAttributeKey()
            {
                RouteName = lot.RouteName,
                RouteStepName = lot.RouteStepName,
                AttributeName = "IsCheckFixCycle"
            });
            //如果设置固化周期检查。
            if (rs != null)
            {
                bool isCheckFixCycle = false;
                bool.TryParse(rs.Value, out isCheckFixCycle);
                //需要检查固化周期。
                if (isCheckFixCycle)
                {
                    DateTime dtStartProcessTime = lot.StartProcessTime.Value;
                    //获取工单固化周期（分钟）
                    WorkOrderRule wor = this.WorkOrderRuleDataEngine.Get(new WorkOrderRuleKey()
                    {
                        OrderNumber = lot.OrderNumber,
                        MaterialCode = lot.MaterialCode
                    });
                    //工单没有设置规则。
                    if (wor == null)
                    {
                        result.Code = 2000;
                        result.Message = string.Format("工单（{0}:{1}）规则没有维护，请确认。", lot.OrderNumber, lot.MaterialCode);
                        return result;
                    }

                    double curFixMinutes = (DateTime.Now - dtStartProcessTime).TotalMinutes;
                    if (wor.FixCycle > curFixMinutes)
                    {
                        result.Code = 2001;
                        result.Message = string.Format("批次（{0}）已固化（{1}）分钟，不满足规则要求的（{2}）分钟，请确认。"
                                                        , lotNumber
                                                        , curFixMinutes
                                                        , wor.FixCycle);
                        return result;
                    }
                }
            }
            #endregion

            #region //判断IV测试数据是否存在。
            rs = this.RouteStepAttributeDataEngine.Get(new RouteStepAttributeKey()
           {
               RouteName = lot.RouteName,
               RouteStepName = lot.RouteStepName,
               AttributeName = "IsCheckIVTestData"
           });
            IVTestData testData = null;
            bool isCheck = false;

            //需要检查IV测试数据。
            if (rs != null
                && bool.TryParse(rs.Value, out isCheck)
                && isCheck)
            {
                #region  //判断IV测试数据是否存在。
                cfg = new PagingConfig()
               {
                   PageNo = 0,
                   PageSize = 1,
                   Where = string.Format("Key.LotNumber='{0}' AND IsDefault=1", lotNumber),
                   OrderBy = "Key.TestTime Desc"
               };
                IList<IVTestData> lstTestData = this.IVTestDataDataEngine.Get(cfg);
                if (lstTestData.Count == 0)
                {
                    result.Code = 2000;
                    result.Message = string.Format("批次（{0}）IV测试数据不存在，请确认。", lot.Key);
                    return result;
                }

                IVTestData firstIVTestData = (from item in lstTestData
                                              where item.Key.LotNumber.ToUpper() == lotNumber.ToUpper()
                                              select item).FirstOrDefault();
                if (firstIVTestData == null)
                {
                    result.Code = 2000;
                    result.Message = string.Format("批次（{0}）在IV测试数据中为全角字符，请修改输入法格式，重新测试。", lot.Key);
                    return result;
                }
                testData = lstTestData[0];
                #endregion

                WorkOrderRule wor = null;

                #region //检查校准板类型。
                rs = this.RouteStepAttributeDataEngine.Get(new RouteStepAttributeKey()
                {
                    RouteName = lot.RouteName,
                    RouteStepName = lot.RouteStepName,
                    AttributeName = "IsCheckCalibrationType"
                });
                if (rs != null
                   && bool.TryParse(rs.Value, out isCheck)
                   && isCheck)
                {
                    //获取工单规则。
                    wor = this.WorkOrderRuleDataEngine.Get(new WorkOrderRuleKey()
                    {
                        OrderNumber = lot.OrderNumber,
                        MaterialCode = lot.MaterialCode
                    });
                    //工单没有设置规则。
                    if (wor == null)
                    {
                        result.Code = 2001;
                        result.Message = string.Format("工单（{0}:{1}）规则没有维护，请确认。", lot.OrderNumber, lot.MaterialCode);
                        return result;
                    }
                    //IV测试数据使用的校准板类型是否正确。
                    if (testData.CalibrationNo == null
                        || !testData.CalibrationNo.StartsWith(wor.CalibrationType))
                    {
                        result.Code = 2002;
                        result.Message = string.Format("批次（{0}）使用的校准板（{1}）和工单规则设置的校准板类型（{2}）不匹配，请确认。"
                                                        , lotNumber
                                                        , testData.CalibrationNo
                                                        , wor.CalibrationType);
                        return result;
                    }
                    //IV测试数据使用的校准板线别是否正确
                    string calibrationNo = "";

                    if (testData.CalibrationNo.Trim().ToUpper().Length >= 11)
                    {
                        calibrationNo = testData.CalibrationNo.Substring(0, 11);
                    }
                    else
                    {
                        calibrationNo = testData.CalibrationNo;
                    }
                    PagingConfig CalibrationLinecfg = new PagingConfig()
                    {
                        IsPaging = false,
                        Where = string.Format("Key.CalibrationPlateID = '{0}'", calibrationNo)
                    };
                    IList<CalibrationPlateLine> calibrationPlateLineList = this.CalibrationPlateLineDataEngine.Get(CalibrationLinecfg);
                    List<string> lineList = new List<string>();
                    if (calibrationPlateLineList != null && calibrationPlateLineList.Count > 0)
                    {
                        foreach (var item in calibrationPlateLineList)
                        {
                            lineList.Add(item.Key.LineCode);
                        }
                        if (!lineList.Contains(p.LineCode))
                        {
                            result.Code = 2002;
                            result.Message = string.Format("批次（{0}）使用的校准板（{1}）的线别与工艺设置的线别（{2}）不匹配，请确认后重新测试IV数据。"
                                                            , lotNumber
                                                            , calibrationNo
                                                            , lineList.ToString());
                            return result;
                        }
                    }
                    else
                    {
                        result.Code = 2002;
                        result.Message = string.Format("批次（{0}）使用的校准板（{1}）与工艺设置的校准板不匹配，请确认后重新测试IV数据。"
                                                        , lotNumber
                                                        , calibrationNo
                                                        );
                        return result;

                    }

                }

                rs = this.RouteStepAttributeDataEngine.Get(new RouteStepAttributeKey()
                {
                    RouteName = lot.RouteName,
                    RouteStepName = lot.RouteStepName,
                    AttributeName = "IsCheckCalibrationCycle"
                });
                #endregion

                #region //检查校准周期。
                if (rs != null
                    && bool.TryParse(rs.Value, out isCheck)
                    && isCheck)
                {
                    if (wor == null)
                    {
                        //获取工单规则。
                        wor = this.WorkOrderRuleDataEngine.Get(new WorkOrderRuleKey()
                        {
                            OrderNumber = lot.OrderNumber,
                            MaterialCode = lot.MaterialCode
                        });
                        //工单没有设置规则。
                        if (wor == null)
                        {
                            result.Code = 2001;
                            result.Message = string.Format("工单（{0}:{1}）规则没有维护，请确认。", lot.OrderNumber, lot.MaterialCode);
                            return result;
                        }
                    }
                    //IV测试数据的校准时间是否满足校准周期要求。
                    DateTime calibrateTime = testData.CalibrateTime ?? DateTime.MinValue;
                    double calibrateCycle = (testData.Key.TestTime - calibrateTime).TotalMinutes;
                    if (calibrateCycle > wor.CalibrationCycle)
                    {
                        result.Code = 2002;
                        result.Message = string.Format("批次（{0}）校准时间（{1:yyyy-MM-dd HH:mm:ss}）超过工单规则设置的校准周期（{2}）分钟，请确认。"
                                                        , lotNumber
                                                        , testData.CalibrateTime
                                                        , wor.CalibrationCycle);
                        return result;
                    }
                }
                #endregion

            }
            #endregion

            //在函数中进行事物提交
            ITransaction transaction = null;
            if (executedWithTransaction == true)
            {
                db = this.SessionFactory.OpenSession();
                transaction = db.BeginTransaction();
            }

            try
            {                
                //处理出站作业
                result = ExecuteTrackOutLot(p, db);
                if (result.Code > 0)
                {
                    if (executedWithTransaction == true)
                    {
                        transaction.Rollback();
                        db.Close();
                    }
                    
                    return result;
                }

                if (p.IsFinished == false)
                {
                    #region //检查是否自动进站

                    bool blIsAutoTrackIn = false;
                    Lot lotOfNew = this.LotDataEngine.Get(lotNumber, db);
                    cfg = new PagingConfig()
                   {
                       IsPaging = false,
                       Where = string.Format(@"Key.RouteName='{0}'
                                                    AND Key.RouteStepName ='{1}'"
                                               , lotOfNew.RouteName
                                               , lotOfNew.RouteStepName)
                   };
                    IList<RouteStepAttribute> lstRouteStepAttributes = this.RouteStepAttributeDataEngine.Get(cfg, db);
                    blIsAutoTrackIn = this.getAttributeValueOfBoolean(lstRouteStepAttributes, "IsAutoTrackIn");
                    if (blIsAutoTrackIn == true)
                    {
                        string strLineCode = "";
                        string strEquipmentCode = "";

                        ProductionLine prodLine = this.ProductionLineDataEngine.Get(lotOfNew.LineCode, db);
                        //根据线别及工序获取设备名称
                        cfg = new PagingConfig()
                        {
                            IsPaging = false,
                            Where = string.Format(@"   EXISTS(  FROM RouteOperationEquipment as p 
                                                        WHERE p.Key.EquipmentCode=self.Key 
                                                        AND p.Key.RouteOperationName='{0}'
                                                    )
                                                    AND self.LineCode ='{1}'"
                                             , lotOfNew.RouteStepName
                                                , prodLine.Key)
                        };
                        Equipment equipment = null;
                        IList<Equipment> lstEquipments = this.EquipmentDataEngine.Get(cfg, db);
                        if (lstEquipments != null && lstEquipments.Count > 0)
                        {
                            equipment = lstEquipments.FirstOrDefault();
                            strLineCode = equipment.LineCode;
                            strEquipmentCode = equipment.Key;
                        }
                        List<string> lstLotNumbers = new List<string>();
                        lstLotNumbers.Add(lotNumber);
                        TrackInParameter trackInParameter = new TrackInParameter()
                        {
                            Creator = p.Creator,
                            EquipmentCode = strEquipmentCode,
                            LineCode = strLineCode,
                            LotNumbers = lstLotNumbers,
                            OperateComputer = p.OperateComputer,
                            Operator = p.Operator,
                            Paramters = null,
                            Remark = "",
                            RouteOperationName = lotOfNew.RouteStepName
                        };
                        result = ExecuteTrackInLot(trackInParameter, db, true);
                        if (result.Code > 0)
                        {
                            transaction.Rollback();
                            db.Close();
                            return result;
                        }
                    }
                    #endregion
                }

                transaction.Commit();
                db.Close();
            }
            catch (Exception ex)
            {
                LogHelper.WriteLogError("TrackOutLot>", ex);
                transaction.Rollback();
                db.Close();
                result.Code = 1000;
                result.Message += string.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        public MethodReturnResult ExecuteTrackOutLot(TrackOutParameter p, ISession db)
        {
            DateTime now = DateTime.Now;
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            p.TransactionKeys = new Dictionary<string, string>();

            #region define List of DataEngine
            List<Lot> lstLotDataEngineForUpdate = new List<Lot>();
            List<LotAttribute> lstLotAttributeForInsert = new List<LotAttribute>();
            List<LotTransaction> lstLotTransactionForInsert = new List<LotTransaction>();
            List<LotTransactionHistory> lstLotTransactionHistoryForInsert = new List<LotTransactionHistory>();
            List<LotTransactionParameter> lstLotTransactionParameterDataEngineForInsert = new List<LotTransactionParameter>();
            List<LotTransactionStep> lstLotTransactionStepDataEngineForInsert = new List<LotTransactionStep>();

            List<LotTransactionDefect> lstLotTransactionDefectForInsert = new List<LotTransactionDefect>();
            List<LotTransactionScrap> lstLotTransactionScrapForInsert = new List<LotTransactionScrap>();
            List<LotTransactionCheck> lstLotTransactionCheckForInsert = new List<LotTransactionCheck>();

            //LotTransactionEquipment ,Equipment ,EquipmentStateEvent
            List<LotTransactionEquipment> lstTransactionEquipmentForUpdate = new List<LotTransactionEquipment>();
            List<LotTransactionEquipment> lstTransactionEquipmentForInsert = new List<LotTransactionEquipment>();


            List<Equipment> lstEquipmentForUpdate = new List<Equipment>();
            List<EquipmentStateEvent> lstEquipmentStateEventtForInsert = new List<EquipmentStateEvent>();

            List<IVTestData> lstIVTestDataForUpdate = new List<IVTestData>();

            List<LotJob> lstLotJobsForInsert = new List<LotJob>();
            List<LotJob> lstLotJobsForUpdate = new List<LotJob>();

            List<LotBOM> lstLotBOMForInsert = new List<LotBOM>();
            List<MaterialLoadingDetail> lstMaterialLoadingDetailForUpdate = new List<MaterialLoadingDetail>();
            List<LineStoreMaterialDetail> lstLineStoreMaterialDetailForUpdate = new List<LineStoreMaterialDetail>();
            #endregion

            //ITransaction transaction = null;
            //if (executedWithTransaction == false)
            //{
            //    db = this.LotDataEngine.SessionFactory.OpenSession();
            //    transaction = db.BeginTransaction();
            //}


            string lotNumber = p.LotNumbers[0];

            Lot lot = this.LotDataEngine.Get(lotNumber, db);
            string transactionKey = Guid.NewGuid().ToString();

            #region //获取 Lot信息，下一道工序
            //生成操作事务主键。                
            p.TransactionKeys.Add(lotNumber, transactionKey);

            //根据批次当前工艺工步获取下一个工步
            RouteStepKey rsKey = new RouteStepKey()
            {
                RouteName = lot.RouteName,
                RouteStepName = lot.RouteStepName
            };
            RouteStep rsObj = this.RouteStepDataEngine.Get(rsKey, db);
            if (rsObj == null)
            {
                result.Code = 2001;
                result.Message = string.Format("批次（{0}）所在工艺流程（{1}）不存在。"
                                                , lotNumber
                                                , rsKey);
                return result;
            }
            PagingConfig cfg = new PagingConfig()
            {
                PageNo = 0,
                PageSize = 1,
                Where = string.Format(@"Key.RouteName='{0}'
                                        AND SortSeq>='{1}'"
                                        , rsObj.Key.RouteName
                                        , rsObj.SortSeq + 1),
                OrderBy = "SortSeq"
            };
            IList<RouteStep> lstRouteStep = this.RouteStepDataEngine.Get(cfg, db);
            if (lstRouteStep.Count == 0)
            {
                //获取下一个工艺流程。
                RouteEnterpriseDetail reDetail = this.RouteEnterpriseDetailDataEngine.Get(new RouteEnterpriseDetailKey()
                {
                    RouteEnterpriseName = lot.RouteEnterpriseName,
                    RouteName = lot.RouteName
                }, db);

                cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format(@"Key.RouteEnterpriseName='{0}'
                                            AND ItemNo='{1}'"
                                            , reDetail.Key.RouteEnterpriseName
                                            , reDetail.ItemNo + 1),
                    OrderBy = "ItemNo"
                };
                IList<RouteEnterpriseDetail> lstRouteEnterpriseDetail = this.RouteEnterpriseDetailDataEngine.Get(cfg, db);
                if (lstRouteEnterpriseDetail.Count > 0)
                {
                    //获取下一工艺流程的第一个工艺工步。
                    reDetail = lstRouteEnterpriseDetail[0];
                    cfg = new PagingConfig()
                    {
                        PageNo = 0,
                        PageSize = 1,
                        Where = string.Format(@"Key.RouteName='{0}'"
                                                , reDetail.Key.RouteName),
                        OrderBy = "SortSeq"
                    };
                    lstRouteStep = this.RouteStepDataEngine.Get(cfg, db);
                }
            }
            bool isFinish = true;
            string toRouteEnterpriseName = lot.RouteEnterpriseName;
            string toRouteName = lot.RouteName;
            string toRouteStepName = lot.RouteStepName;
            if (lstRouteStep != null && lstRouteStep.Count > 0)
            {
                isFinish = false;
                toRouteName = lstRouteStep[0].Key.RouteName;
                toRouteStepName = lstRouteStep[0].Key.RouteStepName;
            }

            //更新批次记录。
            Lot lotUpdate = lot.Clone() as Lot;
            if (isFinish)
            {
                p.IsFinished = true;
                lotUpdate.StartWaitTime = null;
            }
            else
            {
                p.IsFinished = false;
                lotUpdate.StartWaitTime = now;
            }

            if (!string.IsNullOrEmpty(p.Color))
            {
                lotUpdate.Color = p.Color;
            }
            if (!string.IsNullOrEmpty(p.Grade))
            {
                lotUpdate.Grade = p.Grade;
            }

            lotUpdate.StartProcessTime = null;
            //lotUpdate.StateFlag = isFinish ? EnumLotState.Finished : EnumLotState.WaitTrackIn;
            lotUpdate.StateFlag = EnumLotState.WaitTrackIn;
            lotUpdate.RouteEnterpriseName = toRouteEnterpriseName;
            lotUpdate.RouteName = toRouteName;
            lotUpdate.RouteStepName = toRouteStepName;
            lotUpdate.OperateComputer = p.OperateComputer;
            lotUpdate.PreLineCode = lot.LineCode;
            lotUpdate.LineCode = p.LineCode;
            lotUpdate.Editor = p.Creator;
            lotUpdate.EditTime = now;
            lotUpdate.EquipmentCode = null;

            #endregion

            #region //新增批次不良数据

            if (p.DefectReasonCodes != null && p.DefectReasonCodes.ContainsKey(lotNumber))
            {
                foreach (DefectReasonCodeParameter rcp in p.DefectReasonCodes[lotNumber])
                {
                    LotTransactionDefect lotDefect = new LotTransactionDefect()
                    {
                        Key = new LotTransactionDefectKey()
                        {
                            TransactionKey = transactionKey,
                            ReasonCodeCategoryName = rcp.ReasonCodeCategoryName,
                            ReasonCodeName = rcp.ReasonCodeName
                        },
                        Quantity = rcp.Quantity,
                        ResponsiblePerson = rcp.ResponsiblePerson,
                        RouteOperationName = rcp.RouteOperationName,
                        Description = rcp.Description,
                        Editor = p.Creator,
                        EditTime = now,
                    };
                    //this.LotTransactionDefectDataEngine.Insert(lotDefect);
                    lstLotTransactionDefectForInsert.Add(lotDefect);
                }
            }
            #endregion

            #region //新增批次报废数据

            if (p.ScrapReasonCodes != null && p.ScrapReasonCodes.ContainsKey(lotNumber))
            {
                foreach (ScrapReasonCodeParameter rcp in p.ScrapReasonCodes[lotNumber])
                {
                    if (lotUpdate.Quantity < rcp.Quantity)
                    {
                        result.Code = 1006;
                        result.Message = string.Format("批次（{0}）数量（{1}）不满足报废数量。"
                                                        , lotNumber
                                                        , lot.Quantity);
                        return result;
                    }

                    LotTransactionScrap lotScrap = new LotTransactionScrap()
                    {
                        Key = new LotTransactionScrapKey()
                        {
                            TransactionKey = transactionKey,
                            ReasonCodeCategoryName = rcp.ReasonCodeCategoryName,
                            ReasonCodeName = rcp.ReasonCodeName
                        },
                        Quantity = rcp.Quantity,
                        ResponsiblePerson = rcp.ResponsiblePerson,
                        RouteOperationName = rcp.RouteOperationName,
                        Description = rcp.Description,
                        Editor = p.Creator,
                        EditTime = now,
                    };
                    lotUpdate.Quantity -= rcp.Quantity;
                    if (lotUpdate.Quantity < 0)
                    {
                        lotUpdate.Quantity = 0;
                    }
                    //this.LotTransactionScrapDataEngine.Insert(lotScrap);
                    lstLotTransactionScrapForInsert.Add(lotScrap);
                }
            }
            //更新批次记录。
            lotUpdate.DeletedFlag = lotUpdate.Quantity == 0;
            #endregion

            #region //更新设备信息 , 设备的Event ,设备的Transaction

            bool blLogEquipment = true;
            string equipmentCode = lot.EquipmentCode;
            //进站时没有选择设备，则设置出站时批次为当前设备。
            if (string.IsNullOrEmpty(equipmentCode))
            {
                equipmentCode = p.EquipmentCode;
            }

            //进站时已经确定设备。              
            if (!string.IsNullOrEmpty(lot.EquipmentCode))
            {
                //更新批次设备加工历史数据。
                cfg = new PagingConfig()
                {
                    IsPaging = false,
                    Where = string.Format("LotNumber='{0}' AND EquipmentCode='{1}' AND State=0",
                                            lotNumber,
                                            lot.EquipmentCode)
                };
                IList<LotTransactionEquipment> lstLotTransactionEquipment = this.LotTransactionEquipmentDataEngine.Get(cfg, db);
                foreach (LotTransactionEquipment item in lstLotTransactionEquipment)
                {
                    LotTransactionEquipment itemUpdate = item.Clone() as LotTransactionEquipment;
                    itemUpdate.EndTransactionKey = transactionKey;
                    itemUpdate.EditTime = now;
                    itemUpdate.Editor = p.Creator;
                    itemUpdate.EndTime = now;
                    itemUpdate.State = EnumLotTransactionEquipmentState.End;
                    lstTransactionEquipmentForUpdate.Add(itemUpdate);
                    //this.LotTransactionEquipmentDataEngine.Update(itemUpdate, session);
                }
            }
            else
            {//记录批次设备加工历史数据
                LotTransactionEquipment transEquipment = new LotTransactionEquipment()
                {
                    Key = transactionKey,
                    EndTransactionKey = transactionKey,
                    CreateTime = now,
                    Creator = p.Creator,
                    Editor = p.Creator,
                    EditTime = now,
                    EndTime = now,
                    EquipmentCode = p.EquipmentCode,
                    LotNumber = lotNumber,
                    Quantity = lot.Quantity,
                    StartTime = lot.StartProcessTime,
                    State = EnumLotTransactionEquipmentState.End
                };
                lstTransactionEquipmentForInsert.Add(transEquipment);
                //this.LotTransactionEquipmentDataEngine.Insert(transEquipment, session);
            }

            //如果出站也没有选择设备，直接返回。
            if (string.IsNullOrEmpty(equipmentCode) == false)
            {
                //获取设备数据
                Equipment e = this.EquipmentDataEngine.Get(equipmentCode ?? string.Empty, db);
                if (e != null)
                {
                    //获取设备当前状态。
                    EquipmentState es = this.EquipmentStateDataEngine.Get(e.StateName ?? string.Empty, db);
                    if (es != null)
                    {
                        //获取设备LOST的主键
                        EquipmentState lostState = this.EquipmentStateDataEngine.Get("LOST", db);
                        //获取设备当前状态->LOST的状态切换数据。
                        EquipmentChangeState ecsToLost = this.EquipmentChangeStateDataEngine.Get(es.Key, lostState.Key, db);

                        if (ecsToLost != null)
                        {
                            //根据设备编码获取当前加工批次数据。
                            cfg = new PagingConfig()
                            {
                                PageSize = 1,
                                PageNo = 0,
                                OrderBy = "#*#",
                                Where = string.Format("EquipmentCode='{0}' AND STATE='{1}' AND LotNumber<>'{2}' "
                                                        , equipmentCode
                                                        , Convert.ToInt32(EnumLotTransactionEquipmentState.Start)
                                                        , lotNumber
                                                        )
                            };
                            IList<LotTransactionEquipment> lst = this.LotTransactionEquipmentDataEngine.Get(cfg, db);
                            if (lst.Count == 0)//设备当前加工批次>0，则直接返回。
                            {
                                #region Change EqupmentState
                                //更新父设备状态。
                                if (!string.IsNullOrEmpty(e.ParentEquipmentCode))
                                {
                                    Equipment ep = this.EquipmentDataEngine.Get(e.ParentEquipmentCode, db);
                                    if (ep != null)
                                    {
                                        Equipment epUpdate = ep.Clone() as Equipment;
                                        //更新设备状态。
                                        epUpdate.StateName = lostState.Key;
                                        epUpdate.ChangeStateName = ecsToLost.Key;
                                        //this.EquipmentDataEngine.Update(epUpdate);
                                        lstEquipmentForUpdate.Add(epUpdate);
                                        //新增设备状态事件数据
                                        EquipmentStateEvent newStateEvent = new EquipmentStateEvent()
                                        {
                                            Key = Guid.NewGuid().ToString(),
                                            CreateTime = now,
                                            Creator = p.Creator,
                                            Description = p.Remark,
                                            Editor = p.Creator,
                                            EditTime = now,
                                            EquipmentChangeStateName = ecsToLost.Key,
                                            EquipmentCode = e.ParentEquipmentCode,
                                            EquipmentFromStateName = es.Key,
                                            EquipmentToStateName = lostState.Key,
                                            IsCurrent = true
                                        };
                                        //this.EquipmentStateEventDataEngine.Insert(newStateEvent);
                                        lstEquipmentStateEventtForInsert.Add(newStateEvent);
                                    }
                                }
                                //更新设备状态。
                                Equipment eUpdate = e.Clone() as Equipment;
                                eUpdate.StateName = lostState.Key;
                                eUpdate.ChangeStateName = ecsToLost.Key;
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
                                    EquipmentChangeStateName = ecsToLost.Key,
                                    EquipmentCode = e.Key,
                                    EquipmentFromStateName = es.Key,
                                    EquipmentToStateName = lostState.Key,
                                    IsCurrent = true
                                };
                                //this.EquipmentStateEventDataEngine.Insert(stateEvent);
                                lstEquipmentStateEventtForInsert.Add(stateEvent);
                                #endregion
                            }
                        }
                    }
                }
            }
            #endregion

            #region 更新IV测试数据 及 批次基本信息

            //获取工步属性数据。
            RouteStepAttributeKey key = new RouteStepAttributeKey()
            {
                RouteName = lot.RouteName,
                RouteStepName = lot.RouteStepName,
                AttributeName = "IsExecutePowerset"
            };
            RouteStepAttribute rsa = this.RouteStepAttributeDataEngine.Get(key, db);
            bool isExecute = false;
            //需要进行分档。
            if (rsa != null
                && bool.TryParse(rsa.Value, out isExecute)
                && isExecute)
            {
                #region  //判断IV测试数据是否存在。
                cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key.LotNumber='{0}' AND IsDefault=1", lotNumber),
                    OrderBy = "Key.TestTime Desc"
                };
                IList<IVTestData> lstTestData = this.IVTestDataDataEngine.Get(cfg, db);
                if (lstTestData.Count == 0)
                {
                    result.Code = 2000;
                    result.Message = string.Format("批次（{0}）IV测试数据不存在，请确认。", lotNumber);
                    return result;
                }
                #endregion

                IVTestData testData = lstTestData[0].Clone() as IVTestData;
                //获取工单产品设置。
                cfg.Where = string.Format(@"Key.OrderNumber='{0}'"
                                            , lot.OrderNumber);
                cfg.OrderBy = "ItemNo";
                IList<WorkOrderProduct> lstWorkOrderProduct = this.WorkOrderProductDataEngine.Get(cfg, db);
                StringBuilder sbMessage = new StringBuilder();
                bool bSuccess = false;

                for (int i = 0; i < lstWorkOrderProduct.Count; i++)
                {
                    #region //foreach WorkOrderProduct
                    lotUpdate.MaterialCode = lstWorkOrderProduct[i].Key.MaterialCode;

                    sbMessage.AppendFormat("检查批次（{0}）工单（{1}:{2}）分档规则要求。\n"
                                                    , lotUpdate.Key
                                                    , lotUpdate.OrderNumber
                                                    , lotUpdate.MaterialCode);

                    //取得产品代码                 
                    if (lotUpdate.MaterialCode == "1202020012")
                    {
                        //实测功率在315.5---315.9999的组件对其Pmax、Voc、Vpm都乘以0.9985
                        if (testData.CoefPM > 315.5 && testData.CoefPM < 315.9999)
                        {
                            double rate = 0.9985;
                            testData.CoefPM = Math.Round(testData.PM * rate, 4);
                            testData.CoefVOC = Math.Round(testData.VOC * rate, 4);
                            testData.CoefVPM = Math.Round(testData.VPM * rate, 4);


                        }
                        //实测功率在316----319.99999的组件对其Pmax、Voc、Vpm都乘以0.997
                        else
                        {
                            if (testData.CoefPM > 316 && testData.CoefPM < 319.99999)
                            {
                                double rate = 0.997;
                                testData.CoefPM = Math.Round(testData.PM * rate, 4);
                                testData.CoefVOC = Math.Round(testData.VOC * rate, 4);
                                testData.CoefVPM = Math.Round(testData.VPM * rate, 4);

                            }
                            //实测功率在320.5----320.9999的组件对其Pmax、Voc、Vpm都乘以0.9985
                            else
                            {
                                if (testData.CoefPM > 320.5 && testData.CoefPM < 320.9999)
                                {
                                    double rate = 0.9985;
                                    testData.CoefPM = Math.Round(testData.PM * rate, 4);
                                    testData.CoefVOC = Math.Round(testData.VOC * rate, 4);
                                    testData.CoefVPM = Math.Round(testData.VPM * rate, 4);

                                }
                                //实测功率在321----325.99999的组件对其Pmax、Voc、Vpm都乘以0.997
                                else
                                {
                                    if (testData.CoefPM > 321 && testData.CoefPM < 325.99999)
                                    {
                                        double rate = 0.997;
                                        testData.CoefPM = Math.Round(testData.PM * rate, 4);
                                        testData.CoefVOC = Math.Round(testData.VOC * rate, 4);
                                        testData.CoefVPM = Math.Round(testData.VPM * rate, 4);

                                    }
                                }
                            }
                        }
                    }
                    if (lotUpdate.MaterialCode == "1202020020")
                    {
                        //实测功率在320.5---321的组件对其Pmax、Voc、Vpm都乘以0.9985
                        if (testData.CoefPM > 320.5 && testData.CoefPM < 321)
                        {
                            double rate = 0.9985;
                            testData.CoefPM = Math.Round(testData.PM * rate, 4);
                            testData.CoefVOC = Math.Round(testData.VOC * rate, 4);
                            testData.CoefVPM = Math.Round(testData.VPM * rate, 4);


                        }
                        //实测功率在321----325的组件对其Pmax、Voc、Vpm都乘以0.997
                        else
                        {
                            if (testData.CoefPM > 321 && testData.CoefPM < 325)
                            {
                                double rate = 0.997;
                                testData.CoefPM = Math.Round(testData.PM * rate, 4);
                                testData.CoefVOC = Math.Round(testData.VOC * rate, 4);
                                testData.CoefVPM = Math.Round(testData.VPM * rate, 4);

                            }
                            //实测功率在325----327的组件对其Pmax、Voc、Vpm都乘以0.994
                            else
                            {
                                if (testData.CoefPM > 325 && testData.CoefPM < 327)
                                {
                                    double rate = 0.994;
                                    testData.CoefPM = Math.Round(testData.PM * rate, 4);
                                    testData.CoefVOC = Math.Round(testData.VOC * rate, 4);
                                    testData.CoefVPM = Math.Round(testData.VPM * rate, 4);

                                }
                            }

                        }
                    }
                    if (lotUpdate.MaterialCode == "1202020013")
                    {
                        //实测功率在270.5---271的组件对其Pmax、Voc、Vpm都乘以0.9985
                        if (testData.CoefPM > 270.5 && testData.CoefPM < 271)
                        {
                            double rate = 0.9985;
                            testData.CoefPM = Math.Round(testData.PM * rate, 4);
                            testData.CoefVOC = Math.Round(testData.VOC * rate, 4);
                            testData.CoefVPM = Math.Round(testData.VPM * rate, 4);


                        }
                        //实测功率在271----274的组件对其Pmax、Voc、Vpm都乘以0.997
                        else
                        {
                            if (testData.CoefPM > 271 && testData.CoefPM < 274)
                            {
                                double rate = 0.997;
                                testData.CoefPM = Math.Round(testData.PM * rate, 4);
                                testData.CoefVOC = Math.Round(testData.VOC * rate, 4);
                                testData.CoefVPM = Math.Round(testData.VPM * rate, 4);

                            }

                        }
                    }

                    if (lotUpdate.MaterialCode == "1202020021")
                    {
                        //实测功率在320.5---321的组件对其Pmax、Voc、Vpm都乘以0.9985
                        if (testData.CoefPM > 320.5 && testData.CoefPM < 321)
                        {
                            double rate = 0.9985;
                            testData.CoefPM = Math.Round(testData.PM * rate, 4);
                            testData.CoefVOC = Math.Round(testData.VOC * rate, 4);
                            testData.CoefVPM = Math.Round(testData.VPM * rate, 4);


                        }
                        //实测功率在321----325的组件对其Pmax、Voc、Vpm都乘以0.997
                        else
                        {
                            if (testData.CoefPM > 321 && testData.CoefPM < 325)
                            {
                                double rate = 0.997;
                                testData.CoefPM = Math.Round(testData.PM * rate, 4);
                                testData.CoefVOC = Math.Round(testData.VOC * rate, 4);
                                testData.CoefVPM = Math.Round(testData.VPM * rate, 4);

                            }

                        }
                    }

                    //                    #region //进行衰减。
                    //                    //获取工单衰减规则。
                    //                    cfg.Where = string.Format(@"Key.OrderNumber='{0}' 
                    //                                                AND Key.MaterialCode='{1}' 
                    //                                                AND Key.MinPower<='{2}'
                    //                                                AND Key.MaxPower>='{2}'
                    //                                                AND IsUsed=1"
                    //                                                , lotUpdate.OrderNumber
                    //                                                , lotUpdate.MaterialCode
                    //                                                , testData.PM
                    //                                                , testData.PM);
                    //                    cfg.OrderBy = "Key.MinPower";
                    //                    //进行衰减。
                    //                    IList<WorkOrderDecay> lstWorkOrderDecay = this.WorkOrderDecayDataEngine.Get(cfg, db);
                    //                    if (lstWorkOrderDecay.Count > 0)
                    //                    {
                    //                        cfg.IsPaging = false;
                    //                        cfg.Where = string.Format("Key.Code='{0}' AND IsUsed=1", lstWorkOrderDecay[0].DecayCode);
                    //                        cfg.OrderBy = "Key";
                    //                        IList<Decay> lstDecay = this.DecayDataEngine.Get(cfg, db);
                    //                        foreach (Decay item in lstDecay)
                    //                        {
                    //                            //根据功率计算出衰减系数。
                    //                            double rate = 1;
                    //                            if (item.Type == EnumDecayType.Aim)
                    //                            {
                    //                                rate = item.Value / testData.PM;
                    //                            }
                    //                            else
                    //                            {
                    //                                rate = item.Value;
                    //                            }
                    //                            //根据衰减系数计算实际功率值
                    //                            switch (item.Key.Object)
                    //                            {
                    //                                case EnumPVMTestDataType.PM:
                    //                                    testData.CoefPM = testData.PM * rate;
                    //                                    break;
                    //                                case EnumPVMTestDataType.FF:
                    //                                    testData.CoefFF = testData.FF * rate;
                    //                                    break;
                    //                                case EnumPVMTestDataType.IPM:
                    //                                    testData.CoefIPM = testData.IPM * rate;
                    //                                    break;
                    //                                case EnumPVMTestDataType.ISC:
                    //                                    testData.CoefISC = testData.ISC * rate;
                    //                                    break;
                    //                                case EnumPVMTestDataType.VOC:
                    //                                    testData.CoefVOC = testData.VOC * rate;
                    //                                    break;
                    //                                case EnumPVMTestDataType.VPM:
                    //                                    testData.CoefVPM = testData.VPM * rate;
                    //                                    break;
                    //                                default:
                    //                                    break;
                    //                            }
                    //                        }
                    //                    }
                    //                    #endregion

                    #region //判断功率是否符合工单功率范围要求。
                    //获取工单规则。
                    WorkOrderRule wor = this.WorkOrderRuleDataEngine.Get(new WorkOrderRuleKey()
                    {
                        OrderNumber = lotUpdate.OrderNumber,
                        MaterialCode = lotUpdate.MaterialCode
                    }, db);
                    if (wor != null)
                    {
                        testData.CoefPM = Math.Round(testData.CoefPM, wor.PowerDegree, MidpointRounding.AwayFromZero);
                    }
                    if (wor != null
                        && (testData.CoefPM < wor.MinPower || testData.CoefPM > wor.MaxPower))
                    {
                        sbMessage.AppendFormat("批次（{0}）功率（{1}）不符合工单（{2}:{3}）功率范围（{4}-{5}）要求。\n"
                                                , lotUpdate.Key
                                                , testData.CoefPM
                                                , lotUpdate.OrderNumber
                                                , lotUpdate.MaterialCode
                                                , wor.MinPower
                                                , wor.MaxPower);
                        continue;
                    }
                    #endregion

                    #region //判断是否设置并符合控制参数要求。
                    cfg.IsPaging = false;
                    cfg.Where = string.Format(@"Key.OrderNumber='{0}' 
                                                AND Key.MaterialCode='{1}'
                                                AND IsUsed=1"
                                                , lotUpdate.OrderNumber
                                                , lotUpdate.MaterialCode);
                    cfg.OrderBy = "Key";
                    IList<WorkOrderControlObject> lstWorkOrderControlObject = this.WorkOrderControlObjectDataEngine.Get(cfg, db);
                    bool bCheckControlObject = true;
                    foreach (WorkOrderControlObject item in lstWorkOrderControlObject)
                    {
                        double value = double.MinValue;
                        switch (item.Key.Object)
                        {
                            case EnumPVMTestDataType.PM:
                                value = testData.CoefPM;
                                break;
                            case EnumPVMTestDataType.FF:
                                value = testData.CoefFF;
                                break;
                            case EnumPVMTestDataType.IPM:
                                value = testData.CoefIPM;
                                break;
                            case EnumPVMTestDataType.ISC:
                                value = testData.CoefISC;
                                break;
                            case EnumPVMTestDataType.VOC:
                                value = testData.CoefVOC;
                                break;
                            case EnumPVMTestDataType.VPM:
                                value = testData.CoefVPM;
                                break;
                            case EnumPVMTestDataType.CTM:
                                value = testData.CTM;
                                break;
                            default:
                                break;
                        }
                        //控制参数检查。
                        if (value != double.MinValue
                            && CheckControlObject(item.Key.Type, value, item.Value) == false)
                        {
                            sbMessage.AppendFormat("批次（{0}）{1} ({4})不符合工单（{5}:{6}）控制对象（{4}{2}{3}）要求。\n"
                                                    , lotUpdate.Key
                                                    , item.Key.Object.GetDisplayName()
                                                    , item.Key.Type
                                                    , item.Value
                                                    , value
                                                    , lotUpdate.OrderNumber
                                                    , lotUpdate.MaterialCode);
                            bCheckControlObject = false;
                            break;
                        }
                    }
                    if (bCheckControlObject == false)
                    {
                        continue;
                    }
                    #endregion



                    #region //判断是否设置并符合产品控制参数要求。
                    cfg.IsPaging = false;
                    cfg.Where = string.Format(@"Key.LotNumber='{0}' 
                                                AND MaterialCode like'11%'"
                                                , lotUpdate.Key
                                                );
                    cfg.OrderBy = "Key";
                    IList<LotBOM> lotBomList = this.LotBOMDataEngine.Get(cfg, db);
                    LotBOM lotBOM = null;
                    if (lotBomList != null && lotBomList.Count > 0)
                    {
                        lotBOM = (LotBOM)lotBomList.First().Clone();
                    }
                    else
                    {
                        result.Code = 2000;
                        result.Message = string.Format("批次{0}的电池片用料不存在请核实", lotUpdate.Key);
                        return result;
                    }

                    cfg.IsPaging = false;
                    cfg.Where = string.Format(@"Key.ProductCode='{0}' 
                                                AND Key.CellEff='{1}' 
                                                AND Key.SupplierCode='{2}' 
                                                AND IsUsed=1"
                                                , lotUpdate.MaterialCode
                                                , lotUpdate.Attr1
                                                , lotBOM.SupplierCode
                                                );
                    cfg.OrderBy = "Key";

                    IList<ProductControlObject> lstProductControlObject = this.ProductControlObjectDataEngine.Get(cfg, db);
                    foreach (ProductControlObject item in lstProductControlObject)
                    {
                        double value = double.MinValue;
                        switch (item.Key.Object)
                        {
                            case EnumPVMTestDataType.PM:
                                value = testData.CoefPM;
                                break;
                            case EnumPVMTestDataType.FF:
                                value = testData.CoefFF;
                                break;
                            case EnumPVMTestDataType.IPM:
                                value = testData.CoefIPM;
                                break;
                            case EnumPVMTestDataType.ISC:
                                value = testData.CoefISC;
                                break;
                            case EnumPVMTestDataType.VOC:
                                value = testData.CoefVOC;
                                break;
                            case EnumPVMTestDataType.VPM:
                                value = testData.CoefVPM;
                                break;
                            case EnumPVMTestDataType.CTM:
                                value = testData.CTM;
                                break;
                            default:
                                break;
                        }
                        //控制参数检查。
                        if (value != double.MinValue
                            && CheckControlObject(item.Key.Type, value, item.Value) == false)
                        {
                            sbMessage.AppendFormat("批次（{0}）{1} ({4})不符合产品（{5}:{6}）控制对象（{4}{2}{3}）要求。\n"
                                                    , lotUpdate.Key
                                                    , item.Key.Object.GetDisplayName()
                                                    , item.Key.Type
                                                    , item.Value
                                                    , value
                                                    , lotUpdate.OrderNumber
                                                    , lotUpdate.MaterialCode);
                            bCheckControlObject = false;
                            break;
                        }
                    }
                    if (bCheckControlObject == false)
                    {
                        continue;
                    }
                    #endregion

                    #region //进行分档。
                    cfg.IsPaging = true;
                    cfg.Where = string.Format(@"Key.OrderNumber='{0}' 
                                            AND Key.MaterialCode='{1}'
                                            AND MinValue<='{2}'
                                            AND MaxValue>'{2}'
                                            AND IsUsed=1"
                                            , lotUpdate.OrderNumber
                                            , lotUpdate.MaterialCode
                                            , testData.CoefPM);
                    cfg.OrderBy = "Key";
                    IList<WorkOrderPowerset> lstWorkOrderPowerset = this.WorkOrderPowersetDataEngine.Get(cfg, db);
                    if (lstWorkOrderPowerset == null || lstWorkOrderPowerset.Count == 0)
                    {
                        sbMessage.AppendFormat("批次（{0}）功率({1})不符合工单({2}：{3})分档规则要求。\n"
                                                , lotUpdate.Key
                                                , testData.CoefPM
                                                , lotUpdate.OrderNumber
                                                , lotUpdate.MaterialCode);
                        continue;
                    }
                    WorkOrderPowerset ps = lstWorkOrderPowerset[0];
                    testData.PowersetCode = ps.Key.Code;
                    testData.PowersetItemNo = ps.Key.ItemNo;

                    //需要进行子分档
                    if (ps.SubWay != EnumPowersetSubWay.None)
                    {
                        double value = double.MinValue;
                        //电流子分档。
                        if (ps.SubWay == EnumPowersetSubWay.ISC)
                        {
                            value = testData.CoefISC;
                        }
                        else if (ps.SubWay == EnumPowersetSubWay.VOC)
                        {
                            value = testData.CoefVOC;
                        }
                        else if (ps.SubWay == EnumPowersetSubWay.IPM)
                        {
                            value = testData.CoefIPM;
                        }
                        else if (ps.SubWay == EnumPowersetSubWay.VPM)
                        {
                            value = testData.CoefVPM;
                        }
                        cfg.Where = string.Format(@"Key.OrderNumber='{0}' 
                                            AND Key.MaterialCode='{1}'
                                            AND Key.Code='{3}'
                                            AND Key.ItemNo='{4}'
                                            AND MinValue<='{2}'
                                            AND MaxValue>'{2}'
                                            AND IsUsed=1"
                                            , lotUpdate.OrderNumber
                                            , lotUpdate.MaterialCode
                                            , value
                                            , ps.Key.Code
                                            , ps.Key.ItemNo);
                        cfg.OrderBy = "Key";
                        IList<WorkOrderPowersetDetail> lstWorkOrderPowersetDetail = this.WorkOrderPowersetDetailDataEngine.Get(cfg, db);
                        if (lstWorkOrderPowersetDetail.Count > 0)
                        {
                            testData.PowersetSubCode = lstWorkOrderPowersetDetail[0].Key.SubCode;
                        }
                    }

                    #endregion
                    if (ps.SubWay == EnumPowersetSubWay.None && string.IsNullOrEmpty(testData.PowersetSubCode))
                    {
                        sbMessage.AppendFormat("批次（{0}）符合工单（{1}:{2}）分档规则<font size='20' color='red'>无子分档</font>要求。 <font size='20' color='red'>电池片效率为：{3}</font>"
                        , lotUpdate.Key
                        , lotUpdate.OrderNumber
                        , lotUpdate.MaterialCode
                        , lotUpdate.Attr1
                        );
                        /*
                        sbMessage.AppendFormat("<img src='/ZPVM/WorkOrderPowersetDetail/ShowPicture?OrderNumber={0}&MaterialCode={1}&Code={2}&ItemNo={3}&SubCode={4}&TimeStamp={5}' width='150px'/>"
                                        , lotUpdate.OrderNumber
                                        , lotUpdate.MaterialCode
                                        , testData.PowersetCode
                                        , testData.PowersetItemNo
                                        , testData.PowersetSubCode
                                        , DateTime.Now.Ticks);
                        */

                        bSuccess = true;
                    }
                    else if (ps.SubWay != EnumPowersetSubWay.None && !string.IsNullOrEmpty(testData.PowersetSubCode))
                    {
                        sbMessage.AppendFormat("批次（{0}）符合工单（{1}:{2}）分档规则<font size='20' color='red'>({3}-{4})</font>要求。<font size='20' color='red'>电池片效率为：{5}</font>"
                            , lotUpdate.Key
                            , lotUpdate.OrderNumber
                            , lotUpdate.MaterialCode
                            , ps.PowerName
                            , testData.PowersetSubCode
                            , lotUpdate.Attr1);
                        /*
                        sbMessage.AppendFormat("<img src='/ZPVM/WorkOrderPowersetDetail/ShowPicture?OrderNumber={0}&MaterialCode={1}&Code={2}&ItemNo={3}&SubCode={4}&TimeStamp={5}' width='150px'/>"
                                        , lotUpdate.OrderNumber
                                        , lotUpdate.MaterialCode
                                        , testData.PowersetCode
                                        , testData.PowersetItemNo
                                        , testData.PowersetSubCode
                                        , DateTime.Now.Ticks);
                        */

                        bSuccess = true;
                    }
                    else
                    {
                        sbMessage.AppendFormat("工单（{1}:{2}）需要进行子分档，但是未设置子分档规则<font size='20' color='red'>({3}-{4})</font>要求。<font size='20' color='red'>电池片效率为：{5}</font>"
                            , lotUpdate.Key
                            , lotUpdate.OrderNumber
                            , lotUpdate.MaterialCode
                            , ps.PowerName
                            , testData.PowersetSubCode
                            , lotUpdate.Attr1);
                        bSuccess = false;
                    }
                    break;

                    #endregion //foreach workorder
                }
                result.Message = sbMessage.ToString();
                //没有找到符合要求的工单规则。
                if (bSuccess == false)
                {
                    result.Code = 2000;
                    return result;
                }
                //更新批次数据
                lotUpdate.Editor = p.Creator;
                lotUpdate.EditTime = DateTime.Now;
                if (lotUpdate.Attr5 != null && lotUpdate.Attr5 != testData.PowersetItemNo.ToString())
                {
                    lotUpdate.HoldFlag = true;
                    lotUpdate.Attr4 = "批次跳档，请联系工艺人员进行释放！";
                }
                lotUpdate.Attr5 = testData.PowersetItemNo.ToString();
                //this.LotDataEngine.Update(lot);
                //更新测试数据。
                testData.Editor = p.Creator;
                testData.EditTime = DateTime.Now;
                //this.IVTestDataDataEngine.Update(testData);
                lstIVTestDataForUpdate.Add(testData);
            }
            #endregion

            #region 判断是否自动进站
            /*
            //获取工步设置是否自动进站。
            rsa = this.RouteStepAttributeDataEngine.Get(new RouteStepAttributeKey()
            {
                RouteName = lot.RouteName,
                RouteStepName = lot.RouteStepName,
                AttributeName = "IsAutoTrackIn"
            },db);

            bool isAutoTrackIn = false;
            if (rsa != null)
            {
                bool.TryParse(rsa.Value, out isAutoTrackIn);
            }

            //自动进站。
            if (isAutoTrackIn == true)
            {
                //不存在批次在指定工步自动进站作业。
                cfg = new PagingConfig()
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
                IList<LotJob> lstJob = this.LotJobDataEngine.Get(cfg,db);
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
                        JobName = string.Format("{0} {1}", lotUpdate.Key, lotUpdate.StateFlag.GetDisplayName()),
                        Key = Convert.ToString(Guid.NewGuid()),
                        LineCode = lotUpdate.LineCode,
                        LotNumber = lotUpdate.Key,
                        Type = EnumJobType.AutoTrackIn,
                        RouteEnterpriseName = lotUpdate.RouteEnterpriseName,
                        RouteName = lotUpdate.RouteName,
                        RouteStepName = lotUpdate.RouteStepName,
                        RunCount = 0,
                        Status = EnumObjectStatus.Available,
                        NextRunTime = DateTime.Now.AddSeconds(1),
                        NotifyMessage = string.Empty,
                        NotifyUser = string.Empty
                    };
                    //this.LotJobDataEngine.Insert(job);
                    lstLotJobsForInsert.Add(job);
                }
                else
                {
                    LotJob jobUpdate = lstJob[0].Clone() as LotJob;
                    jobUpdate.NextRunTime = DateTime.Now.AddSeconds(1);
                    jobUpdate.LineCode = lotUpdate.LineCode;
                    jobUpdate.EquipmentCode = null;
                    jobUpdate.RunCount = 0;
                    jobUpdate.Editor = p.Creator;
                    jobUpdate.EditTime = DateTime.Now;
                    lstLotJobsForUpdate.Add(jobUpdate);
                    //this.LotJobDataEngine.Update(jobUpdate);
                }
            }
            */
            #endregion

            #region //物料批次管理
            string strAttr1 = "";
            string strAttr2 = "";
            cfg = new PagingConfig()
            {
                IsPaging = false,
                Where = string.Format(@"Key.RouteName='{0}' 
                                        AND Key.RouteStepName='{1}'
                                        AND IsDeleted=0
                                        AND DCType='{2}'"
                                        , lot.RouteName
                                        , lot.RouteStepName
                                        , Convert.ToInt32(EnumDataCollectionAction.TrackOut)),
                OrderBy = "ParamIndex"
            };
            IList<RouteStepParameter> lstRouteStepParameter = this.RouteStepParameterDataEngine.Get(cfg, db);
            if (lstRouteStepParameter.Count > 0 && p.Paramters != null && p.Paramters.Count > 0)
            {
                //检验物料批号。
                foreach (TransactionParameter tp in p.Paramters[lotNumber])
                {
                    RouteStepParameter item = lstRouteStepParameter
                                                    .FirstOrDefault(w => w.Key.ParameterName == tp.Name);

                    if (item == null || item.ValidateRule == EnumValidateRule.None)
                    {
                        continue;
                    }

                    //匹配工单可用物料批号（根据领料记录）。
                    if (item.ValidateRule == EnumValidateRule.FullyWorkOrderMaterialLot)
                    {
                        #region //验证工单领料批号。
                        cfg = new PagingConfig()
                        {
                            PageNo = 0,
                            PageSize = 10,
                            Where = string.Format(@"MaterialLot='{0}'
                                                    AND MaterialCode LIKE '{2}%'
                                                    AND EXISTS(SELECT Key
                                                                FROM MaterialReceipt as p
                                                                WHERE p.OrderNumber='{1}'
                                                                AND p.Key=self.Key.ReceiptNo)"
                                                    , tp.Value
                                                    , lot.OrderNumber
                                                    , item.MaterialType),
                            OrderBy = "CreateTime DESC"
                        };

                        IList<MaterialReceiptDetail> lstMaterialReceiptDetail = this.MaterialReceiptDetailDataEngine.Get(cfg, db);
                        if (lstMaterialReceiptDetail == null || lstMaterialReceiptDetail.Count == 0)
                        {
                            string message = item.ValidateFailedMessage ?? string.Empty;
                            if (string.IsNullOrEmpty(message.Trim()))
                            {
                                message = "参数 （{0}） 对应物料类型（{3}）,其值 {1} 非工单（{2}）的领料批号。";
                            }
                            result.Code = 2004;
                            result.Message = string.Format(message
                                                            , item.Key.ParameterName
                                                            , tp.Value
                                                            , lot.OrderNumber
                                                            , item.MaterialType);
                            return result;
                        }
                        #endregion

                        equipmentCode = lot.EquipmentCode;
                        if (string.IsNullOrEmpty(equipmentCode))
                        {
                            equipmentCode = p.EquipmentCode;
                        }

                        #region 更新线边仓物料记录
                        IDictionary<string, double> dicLineStoreMaterialDetail = new Dictionary<string, double>();
                        //遍历领料记录
                        foreach (MaterialReceiptDetail mrdItem in lstMaterialReceiptDetail)
                        {

                            LineStoreMaterialDetail lsmd = this.LineStoreMaterialDetailDataEngine.Get(new LineStoreMaterialDetailKey()
                            {
                                LineStoreName = mrdItem.LineStoreName,
                                OrderNumber = lot.OrderNumber,
                                MaterialCode = mrdItem.MaterialCode,
                                MaterialLot = mrdItem.MaterialLot
                            }, db);

                            if (lsmd == null
                                || lsmd.CurrentQty == 0)
                            {
                                continue;
                            }


                            //获取物料
                            Material m = this.MaterialDataEngine.Get(lsmd.Key.MaterialCode ?? string.Empty, db);
                            //获取供应商
                            Supplier s = this.SupplierDataEngine.Get(lsmd.SupplierCode ?? string.Empty, db);

                            //获取工单BOM
                            if (!dicLineStoreMaterialDetail.ContainsKey(mrdItem.MaterialCode))
                            {
                                cfg = new PagingConfig()
                                {
                                    PageNo = 0,
                                    PageSize = 1,
                                    Where = string.Format(@"Key.OrderNumber='{0}'
                                                            AND MaterialCode='{1}'"
                                                        , lot.OrderNumber
                                                        , mrdItem.MaterialCode)
                                };
                                IList<WorkOrderBOM> lstWorkOrderBOM = this.WorkOrderBOMDataEngine.Get(cfg, db);
                                if (lstWorkOrderBOM == null || lstWorkOrderBOM.Count == 0)
                                {
                                    continue;
                                }
                                dicLineStoreMaterialDetail.Add(mrdItem.MaterialCode, lstWorkOrderBOM[0].Qty * lot.Quantity);
                            }
                            //更新线边仓物料记录。

                            double qty = dicLineStoreMaterialDetail[mrdItem.MaterialCode];
                            LineStoreMaterialDetail lsmdUpdate = lsmd.Clone() as LineStoreMaterialDetail;
                            double leftQty = lsmdUpdate.CurrentQty - qty;
                            if (leftQty < 0)
                            {
                                dicLineStoreMaterialDetail[mrdItem.MaterialCode] = Math.Abs(leftQty);
                                lsmdUpdate.CurrentQty = 0;
                                string message = item.ValidateFailedMessage ?? string.Empty;
                                if (string.IsNullOrEmpty(message.Trim()))
                                {
                                    message = "参数 （{0}） 值 {1} 在工单（{4}）工序（{2}）设备（{3}）物料不足，请下料。";
                                }
                                result.Code = 2004;
                                result.Message = string.Format(message
                                                                , item.Key.ParameterName
                                                                , tp.Value
                                                                , lot.RouteStepName
                                                                , equipmentCode
                                                                , lot.OrderNumber);
                                return result;
                            }
                            else
                            {
                                dicLineStoreMaterialDetail[mrdItem.MaterialCode] = 0;//设置数量为0
                                lsmdUpdate.CurrentQty = leftQty;
                            }
                            lstLineStoreMaterialDetailForUpdate.Add(lsmdUpdate);
                            //this.LineStoreMaterialDetailDataEngine.Update(lsmdUpdate);

                            //新增批次用料记录。
                            int lotbomItemNo = 1;
                            cfg = new PagingConfig()
                            {
                                PageNo = 0,
                                PageSize = 1,
                                Where = string.Format("Key.LotNumber='{0}' AND Key.MaterialLot='{1}'"
                                                        , lot.Key
                                                        , mrdItem.MaterialLot),
                                OrderBy = "Key.ItemNo Desc"
                            };

                            IList<LotBOM> lstLotBom = this.LotBOMDataEngine.Get(cfg, db);
                            if (lstLotBom.Count > 0)
                            {
                                lotbomItemNo = lstLotBom[0].Key.ItemNo + 1;
                            }
                            LotBOM lotbomObj = new LotBOM()
                            {
                                CreateTime = DateTime.Now,
                                Creator = p.Creator,
                                Editor = p.Creator,
                                EditTime = DateTime.Now,
                                EquipmentCode = equipmentCode,
                                LineCode = lot.LineCode,
                                LineStoreName = mrdItem.LineStoreName,
                                MaterialCode = mrdItem.MaterialCode,
                                MaterialName = m != null ? m.Name : string.Empty,
                                SupplierCode = lsmd.SupplierCode,
                                SupplierName = s != null ? s.Name : string.Empty,
                                RouteEnterpriseName = lot.RouteEnterpriseName,
                                RouteName = lot.RouteName,
                                RouteStepName = lot.RouteStepName,
                                TransactionKey = p.TransactionKeys[lotNumber],
                                MaterialFrom = EnumMaterialFrom.LineStore,
                                Qty = leftQty >= 0 ? qty : qty + leftQty,
                                Key = new LotBOMKey()
                                {
                                    LotNumber = lot.Key,
                                    MaterialLot = mrdItem.MaterialLot,
                                    ItemNo = lotbomItemNo
                                }
                            };
                            lstLotBOMForInsert.Add(lotbomObj);
                            //this.LotBOMDataEngine.Insert(lotbomObj);

                            //如果数量满足，跳出。
                            if (dicLineStoreMaterialDetail[mrdItem.MaterialCode] == 0)
                            {
                                break;
                            }
                        }
                        //线边仓物料数量不足。
                        var lnq = from d in dicLineStoreMaterialDetail
                                  where d.Value > 0
                                  select d;
                        if (lnq.Count() > 0)
                        {
                            string message = item.ValidateFailedMessage ?? string.Empty;
                            if (string.IsNullOrEmpty(message.Trim()))
                            {
                                message = "参数 （{0}） 值 {1} 对应物料不足。";
                            }
                            result.Code = 2004;
                            result.Message = string.Format(message
                                                            , item.Key.ParameterName
                                                            , tp.Value);
                            return result;
                        }
                        #endregion
                    }
                    //匹配设备上料批号（根据上料记录）。
                    else if (item.ValidateRule == EnumValidateRule.FullyLoadingMaterialLot)
                    {
                        equipmentCode = lot.EquipmentCode;
                        if (string.IsNullOrEmpty(equipmentCode))
                        {
                            equipmentCode = p.EquipmentCode;
                        }

                        #region //验证设备上料批号。
                        //                                                cfg = new PagingConfig()
                        //                                                {
                        //                                                    IsPaging = false,
                        //                                                    Where = string.Format(@"MaterialLot='{0}'
                        //                                                                                                    AND OrderNumber='{4}'
                        //                                                                                                    AND MaterialCode LIKE '{3}%'
                        //                                                                                                    AND CurrentQty>0
                        //                                                                                                    AND EXISTS(SELECT Key
                        //                                                                                                                FROM MaterialLoading as p
                        //                                                                                                                WHERE p.RouteOperationName='{1}'
                        //                                                                                                                AND p.EquipmentCode='{2}'
                        //                                                                                                                AND p.Key=self.Key.LoadingKey)"
                        //                                                                            , tp.Value
                        //                                                                            , lot.RouteStepName
                        //                                                                            , equipmentCode
                        //                                                                            , item.MaterialType
                        //                                                                            , lot.OrderNumber)
                        //                                                };

                        //                        //获取上料记录。
                        //                         IList<MaterialLoadingDetail> lstMaterialLoadingDetail = this.MaterialLoadingDetailDataEngine.Get(cfg, db);

                        IList<MaterialLoadingDetail> lstMaterialLoadingDetail = GetMaterialLoadingDetailEx(item.MaterialType, lot.RouteStepName, equipmentCode, tp.Value, lot.OrderNumber);

                        if (lstMaterialLoadingDetail == null || lstMaterialLoadingDetail.Count == 0)
                        {
                            string message = item.ValidateFailedMessage ?? string.Empty;
                            if (string.IsNullOrEmpty(message.Trim()))
                            {
                                message = "参数 （{0}） 值 {1} 非工单（{4}）工序（{2}）设备（{3}）上料批号。";
                            }
                            result.Code = 2004;
                            result.Message = string.Format(message
                                                            , item.Key.ParameterName
                                                            , tp.Value
                                                            , lot.RouteStepName
                                                            , equipmentCode
                                                            , lot.OrderNumber);
                            return result;
                        }
                        #endregion

                        #region 更新上料记录

                        IDictionary<string, double> dicMaterialLoadingDetail = new Dictionary<string, double>();

                        string loadingMaterialCode = lstMaterialLoadingDetail[0].MaterialCode;

                        //获取工单BOM
                        if (!dicMaterialLoadingDetail.ContainsKey(loadingMaterialCode))
                        {
                            cfg = new PagingConfig()
                            {
                                PageNo = 0,
                                PageSize = 1,
                                Where = string.Format(@"Key.OrderNumber='{0}'
                                                        AND MaterialCode='{1}'"
                                                    , lot.OrderNumber
                                                    , loadingMaterialCode)
                            };
                            IList<WorkOrderBOM> lstWorkOrderBOM = this.WorkOrderBOMDataEngine.Get(cfg, db);
                            if (lstWorkOrderBOM == null || lstWorkOrderBOM.Count == 0)
                            {
                                string message = item.ValidateFailedMessage ?? string.Empty;
                                if (string.IsNullOrEmpty(message.Trim()))
                                {
                                    message = "物料" + loadingMaterialCode + "在工单" + lot.OrderNumber + "BOM中不存在。";
                                }
                                result.Code = 2004;
                                result.Message = message;
                                return result;
                            }
                            dicMaterialLoadingDetail.Add(loadingMaterialCode, lstWorkOrderBOM[0].Qty * lot.Quantity);
                        }

                        Dictionary<LotBOMKey, int> dicLotBomKey = new Dictionary<LotBOMKey, int>();

                        //遍历上料记录
                        foreach (MaterialLoadingDetail mldItem in lstMaterialLoadingDetail)
                        {
                            //更新上料记录。
                            double qty = dicMaterialLoadingDetail[loadingMaterialCode];
                            MaterialLoadingDetail mldItemUpdate = mldItem.Clone() as MaterialLoadingDetail;
                            double leftQty = mldItemUpdate.CurrentQty - qty;
                            if (leftQty < 0)
                            {
                                if (mldItemUpdate.MaterialCode.StartsWith("11"))
                                {
                                    dicMaterialLoadingDetail[loadingMaterialCode] = Math.Abs(leftQty);
                                    mldItemUpdate.CurrentQty = 0;
                                }
                                else
                                {
                                    mldItemUpdate.CurrentQty = 0;
                                    lstMaterialLoadingDetailForUpdate.Add(mldItemUpdate);
                                    continue;

                                }

                            }

                            else
                            {
                                dicMaterialLoadingDetail[loadingMaterialCode] = 0;//设置数量为0
                                mldItemUpdate.CurrentQty = leftQty;
                            }

                            lstMaterialLoadingDetailForUpdate.Add(mldItemUpdate);

                            //this.MaterialLoadingDetailDataEngine.Update(mldItemUpdate);

                            LineStoreMaterialDetail lsmd = this.LineStoreMaterialDetailDataEngine.Get(new LineStoreMaterialDetailKey()
                            {
                                LineStoreName = mldItem.LineStoreName,
                                OrderNumber = lot.OrderNumber,
                                MaterialCode = mldItem.MaterialCode,
                                MaterialLot = mldItem.MaterialLot
                            }, db);

                            Material m = null;
                            Supplier s = null;

                            if (lsmd != null)
                            {
                                //获取物料
                                m = this.MaterialDataEngine.Get(lsmd.Key.MaterialCode ?? string.Empty, db);
                                //获取供应商
                                s = this.SupplierDataEngine.Get(lsmd.SupplierCode ?? string.Empty, db);
                            }

                            //新增批次用料记录。
                            int lotbomItemNo = 1;
                            cfg = new PagingConfig()
                            {
                                PageNo = 0,
                                PageSize = 1,
                                Where = string.Format("Key.LotNumber='{0}' AND Key.MaterialLot='{1}'"
                                                        , lot.Key
                                                        , mldItem.MaterialLot),
                                OrderBy = "Key.ItemNo Desc"
                            };

                            IList<LotBOM> lstLotBom = this.LotBOMDataEngine.Get(cfg, db);
                            if (lstLotBom.Count > 0)
                            {
                                lotbomItemNo = lstLotBom[0].Key.ItemNo + 1;
                            }

                            LotBOMKey lotBomKey = new LotBOMKey
                            {
                                LotNumber = lot.Key,
                                MaterialLot = mldItem.MaterialLot,
                                ItemNo = lotbomItemNo
                            };

                            if (dicLotBomKey.ContainsKey(lotBomKey))
                            {
                                int itemNo = 0;
                                dicLotBomKey.TryGetValue(lotBomKey, out itemNo);
                                dicLotBomKey.Remove(lotBomKey);
                                lotBomKey.ItemNo = itemNo + 1;
                                dicLotBomKey.Add(lotBomKey, lotBomKey.ItemNo);
                            }
                            else
                            {
                                dicLotBomKey.Add(lotBomKey, lotBomKey.ItemNo);
                            }
                            LotBOM lotbomObj = new LotBOM()
                            {
                                CreateTime = DateTime.Now,
                                Creator = p.Creator,
                                Editor = p.Creator,
                                EditTime = DateTime.Now,
                                EquipmentCode = equipmentCode,
                                LineCode = lot.LineCode,
                                LineStoreName = mldItem.LineStoreName,
                                MaterialCode = mldItem.MaterialCode,
                                MaterialName = m != null ? m.Name : string.Empty,
                                SupplierCode = lsmd != null ? lsmd.SupplierCode : string.Empty,
                                SupplierName = s != null ? s.Name : string.Empty,
                                RouteEnterpriseName = lot.RouteEnterpriseName,
                                RouteName = lot.RouteName,
                                RouteStepName = lot.RouteStepName,
                                TransactionKey = p.TransactionKeys[lotNumber],
                                MaterialFrom = EnumMaterialFrom.Loading,
                                LoadingItemNo = mldItem.Key.ItemNo,
                                LoadingKey = mldItem.Key.LoadingKey,
                                Qty = leftQty >= 0 ? qty : qty + leftQty,
                                Key = lotBomKey
                            };
                            lstLotBOMForInsert.Add(lotbomObj);
                            //this.LotBOMDataEngine.Insert(lotbomObj);
                            //如果数量满足，跳出。
                            if (dicMaterialLoadingDetail[loadingMaterialCode] == 0)
                            {
                                break;
                            }
                        }


                        //上料数量不足。
                        var lnq = from d in dicMaterialLoadingDetail
                                  where d.Value > 0
                                  select d;
                        if (lnq.Count() > 0)
                        {

                            string message = item.ValidateFailedMessage ?? string.Empty;
                            if (string.IsNullOrEmpty(message.Trim()))
                            {
                                message = "参数 （{0}） 值 {1} 在工序（{2}）设备（{3}）上料不足。";
                            }
                            result.Code = 2004;
                            result.Message = string.Format(message
                                                            , item.Key.ParameterName
                                                            , tp.Value
                                                            , lot.RouteStepName
                                                            , equipmentCode);
                            return result;
                        }
                        #endregion
                    }
                    else
                    {
                        #region //验证工单BOM
                        cfg = new PagingConfig()
                        {
                            PageNo = 0,
                            PageSize = 1,
                            Where = string.Format(@"Key.OrderNumber='{0}'"
                                                , lot.OrderNumber)
                        };

                        if (item.ValidateRule == EnumValidateRule.FullyWorkorderBOM)
                        {
                            cfg.Where += string.Format(" AND  MaterialCode='{0}'", tp.Value);
                        }
                        else if (item.ValidateRule == EnumValidateRule.PrefixWorkorderBOM)
                        {
                            cfg.Where += string.Format(" AND  MaterialCode LIKE '{0}%'", tp.Value);
                        }
                        else if (item.ValidateRule == EnumValidateRule.SuffixWorkorderBOM)
                        {
                            cfg.Where += string.Format(" AND  MaterialCode LIKE '%{0}'", tp.Value);
                        }
                        else
                        {
                            cfg.Where += string.Format(" AND  MaterialCode LIKE '%{0}%'", tp.Value);
                        }

                        IList<WorkOrderBOM> lstWorkOrderBOM = this.WorkOrderBOMDataEngine.Get(cfg, db);
                        if (lstWorkOrderBOM == null || lstWorkOrderBOM.Count == 0)
                        {
                            result.Code = 2005;
                            string message = item.ValidateFailedMessage ?? string.Empty;
                            if (string.IsNullOrEmpty(message.Trim()))
                            {
                                message = "参数 （{0}） 值 {1} 在工单（{2}）BOM中不存在。";
                            }
                            result.Message = string.Format(message
                                                            , item.Key.ParameterName
                                                            , tp.Value
                                                            , lot.OrderNumber);
                            return result;
                        }
                        #endregion
                    }

                    //检查是否需要获取电池片批次信息
                    if (string.Compare(tp.Name, "电池片批号") == 0 || string.Compare(tp.Name, "电池片小包装号") == 0)
                    {
                        MaterialReceiptDetail mReceiptDetail = this.getMaterialReceiptDetail(tp.Value, lot.OrderNumber, db);
                        if (mReceiptDetail != null)
                        {
                            lotUpdate.Attr1 = mReceiptDetail.Attr1;
                            lotUpdate.Attr2 = mReceiptDetail.Attr2;
                            lotUpdate.Color = mReceiptDetail.Attr2;
                        }
                    }
                }
            }

            #endregion

            //#region//记录操作历史。
            LotTransaction transObj = new LotTransaction()
            {
                Key = transactionKey,
                Activity = EnumLotActivity.TrackOut,
                CreateTime = now,
                Creator = p.Creator,
                Description = p.Remark,
                Editor = p.Creator,
                EditTime = now,
                InQuantity = lot.Quantity,
                LotNumber = lotNumber,
                LocationName = lot.LocationName,
                LineCode = lot.LineCode,
                OperateComputer = p.OperateComputer,
                OrderNumber = lot.OrderNumber,
                OutQuantity = lotUpdate.Quantity,
                RouteEnterpriseName = lot.RouteEnterpriseName,
                RouteName = lot.RouteName,
                RouteStepName = lot.RouteStepName,
                ShiftName = p.ShiftName,
                UndoFlag = false,
                UndoTransactionKey = null,
                Grade = lot.Grade,
                Color = lot.Color,
                Attr1 = lot.Attr1,
                Attr2 = lot.Attr2,
                Attr3 = lot.Attr3,
                Attr4 = lot.Attr4,
                Attr5 = lot.Attr5,
                OriginalOrderNumber = lot.OriginalOrderNumber
            };
            lstLotTransactionForInsert.Add(transObj);
            //this.LotTransactionDataEngine.Insert(transObj, session);

            //新增批次历史记录。
            LotTransactionHistory lotHistory = new LotTransactionHistory(transactionKey, lot);
            lstLotTransactionHistoryForInsert.Add(lotHistory);

            //#region //1、判断功率测试到终检的过站时间是否大于48个小时

            if (transObj.RouteStepName == "终检" && transObj.Activity == EnumLotActivity.TrackOut)
            {
                MethodReturnResult<DataSet> res = new MethodReturnResult<DataSet>();
                MethodReturnResult<DataSet> re = new MethodReturnResult<DataSet>();
                DbConnection con = this._db.CreateConnection();
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandText = string.Format(@"SELECT TOP (1)EDIT_TIME FROM dbo.WIP_TRANSACTION WHERE ROUTE_STEP_NAME='功率测试' AND ACTIVITY=2 AND LOT_NUMBER ='{0}' ORDER BY  EDIT_TIME DESC 
                                                                            ", lotNumber);
                       res.Data = _db.ExecuteDataSet(cmd);
                        DateTime d1 = DateTime.Parse(transObj.EditTime.ToString());
                        DateTime d2 = DateTime.Parse(res.Data.Tables[0].Rows[0]["EDIT_TIME"].ToString()).AddDays(2);
                        //2、判断功率测试时间到终检出站时间是否大于48个小时，如果是则进行批次暂停
                        if (d1 > d2)
                        {
                            cmd.CommandText = string.Format(@"SELECT TOP (1)EDIT_TIME,ACTIVITY FROM dbo.WIP_TRANSACTION WHERE ROUTE_STEP_NAME='终检' AND ACTIVITY=4 AND LOT_NUMBER ='{0}' AND UNDO_FLAG=0 ORDER BY  EDIT_TIME DESC 
                                                       ", lotNumber);
                            re.Data = _db.ExecuteDataSet(cmd);
                            //4、判断是否有查找到释放数据
                            if (re.Data.Tables[0].Rows.Count >0)
                            {
                                DateTime d4 = DateTime.Parse(re.Data.Tables[0].Rows[0]["EDIT_TIME"].ToString()).AddDays(2);
                                //3、判断批次释放后的时间与现终检出站的时间间隔是否大于48个小时，如果是则进行批次暂停
                                if (d1 > d4)
                                {
                                    transObj.Activity = EnumLotActivity.Hold;
                                    lotUpdate.HoldFlag = true;
                                    lotUpdate.RouteStepName = lot.RouteStepName;
                                    res.Data = _db.ExecuteDataSet(cmd);
                                    lstLotDataEngineForUpdate.Add(lotUpdate);
                                    result.Code = -1;
                                    result.Message += string.Format("功率测试到终检间隔时间大于48个小时，已暂停，请找工艺人员确认！");
                                }
                                //3、判断批次释放后的时间与现终检出站的时间间隔是否大于48个小时，否则进行批次暂停
                                else
                                {

                                    //this.LotTransactionHistoryDataEngine.Insert(lotHistory, session);

                                    //新增工艺下一步记录。
                                    LotTransactionStep nextStep = new LotTransactionStep()
                                    {
                                        Key = transactionKey,
                                        ToRouteEnterpriseName = toRouteEnterpriseName,
                                        ToRouteName = toRouteName,
                                        ToRouteStepName = toRouteStepName,
                                        Editor = p.Creator,
                                        EditTime = now
                                    };
                                    lstLotTransactionStepDataEngineForInsert.Add(nextStep);
                                    //this.LotTransactionStepDataEngine.Insert(nextStep, session);



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
                                            //this.LotTransactionParameterDataEngine.Insert(lotParamObj, session);
                                        }
                                    }
                                    #endregion

                                    #region //批次属性
                                    if (p.Attributes != null && p.Attributes.ContainsKey(lotNumber))
                                    {
                                        foreach (TransactionParameter tp in p.Attributes[lotNumber])
                                        {
                                            LotAttribute lotParamObj = new LotAttribute()
                                            {
                                                Key = new LotAttributeKey()
                                                {
                                                    AttributeName = tp.Name,
                                                    LotNumber = lotNumber,
                                                },
                                                AttributeValue = tp.Value,
                                                Editor = p.Creator,
                                                EditTime = now
                                            };
                                            lstLotAttributeForInsert.Add(lotParamObj);
                                        }
                                    }
                                    #endregion

                                    #region 新增检验数据
                                    if (string.IsNullOrEmpty(p.Color) == false
                                        || string.IsNullOrEmpty(p.Grade) == false
                                        || (p.CheckBarcodes != null && p.CheckBarcodes.ContainsKey(lotNumber)))
                                    {
                                        LotTransactionCheck ltcObj = new LotTransactionCheck()
                                        {
                                            Key = transactionKey,
                                            Editor = p.Creator,
                                            EditTime = now
                                        };
                                        ltcObj.Color = p.Color;
                                        ltcObj.Grade = p.Grade;

                                        if (p.CheckBarcodes != null && p.CheckBarcodes.ContainsKey(lotNumber))
                                        {
                                            IList<string> lstBarcode = p.CheckBarcodes[lotNumber];
                                            ltcObj.Barcode1 = lstBarcode.Count > 0 ? lstBarcode[0] : null;
                                            ltcObj.Barcode2 = lstBarcode.Count > 1 ? lstBarcode[1] : null;
                                            ltcObj.Barcode3 = lstBarcode.Count > 2 ? lstBarcode[2] : null;
                                            ltcObj.Barcode4 = lstBarcode.Count > 3 ? lstBarcode[3] : null;
                                            ltcObj.Barcode5 = lstBarcode.Count > 4 ? lstBarcode[4] : null;
                                        }
                                        lstLotTransactionCheckForInsert.Add(ltcObj);
                                        //this.LotTransactionCheckDataEngine.Insert(ltcObj);
                                    }
                                    #endregion

                                    lstLotDataEngineForUpdate.Add(lotUpdate);
                                    //}

                                }
                            }
                           //4、没有查找到释放数据，判断是否功率到终检时间间隔是否大于48个小时
                            else
                            {
                                //5、如果时间大于48个小时，则进行批次暂停
                                if (d1 > d2)
                                {

                                    transObj.Activity = EnumLotActivity.Hold;
                                    lotUpdate.HoldFlag = true;
                                    lotUpdate.RouteStepName = lot.RouteStepName;
                                    res.Data = _db.ExecuteDataSet(cmd);
                                    lstLotDataEngineForUpdate.Add(lotUpdate);
                                    result.Code = -1;
                                    result.Message += string.Format("功率测试到终检间隔时间大于48个小时，已暂停，请找工艺人员确认！");

                                }
                                 //5、如果功率测试到终检的时间小于48个小时，则进行批次暂停
                                else
                                {
                                    //this.LotTransactionHistoryDataEngine.Insert(lotHistory, session);

                                    //新增工艺下一步记录。
                                    LotTransactionStep nextStep = new LotTransactionStep()
                                    {
                                        Key = transactionKey,
                                        ToRouteEnterpriseName = toRouteEnterpriseName,
                                        ToRouteName = toRouteName,
                                        ToRouteStepName = toRouteStepName,
                                        Editor = p.Creator,
                                        EditTime = now
                                    };
                                    lstLotTransactionStepDataEngineForInsert.Add(nextStep);
                                    //this.LotTransactionStepDataEngine.Insert(nextStep, session);

            

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
                                            //this.LotTransactionParameterDataEngine.Insert(lotParamObj, session);
                                        }
                                    }
                                    #endregion

                                    #region //批次属性
                                    if (p.Attributes != null && p.Attributes.ContainsKey(lotNumber))
                                    {
                                        foreach (TransactionParameter tp in p.Attributes[lotNumber])
                                        {
                                            LotAttribute lotParamObj = new LotAttribute()
                                            {
                                                Key = new LotAttributeKey()
                                                {
                                                    AttributeName = tp.Name,
                                                    LotNumber = lotNumber,
                                                },
                                                AttributeValue = tp.Value,
                                                Editor = p.Creator,
                                                EditTime = now
                                            };
                                            lstLotAttributeForInsert.Add(lotParamObj);
                                        }
                                    }
                                    #endregion

                                    #region 新增检验数据
                                    if (string.IsNullOrEmpty(p.Color) == false
                                        || string.IsNullOrEmpty(p.Grade) == false
                                        || (p.CheckBarcodes != null && p.CheckBarcodes.ContainsKey(lotNumber)))
                                    {
                                        LotTransactionCheck ltcObj = new LotTransactionCheck()
                                        {
                                            Key = transactionKey,
                                            Editor = p.Creator,
                                            EditTime = now
                                        };
                                        ltcObj.Color = p.Color;
                                        ltcObj.Grade = p.Grade;

                                        if (p.CheckBarcodes != null && p.CheckBarcodes.ContainsKey(lotNumber))
                                        {
                                            IList<string> lstBarcode = p.CheckBarcodes[lotNumber];
                                            ltcObj.Barcode1 = lstBarcode.Count > 0 ? lstBarcode[0] : null;
                                            ltcObj.Barcode2 = lstBarcode.Count > 1 ? lstBarcode[1] : null;
                                            ltcObj.Barcode3 = lstBarcode.Count > 2 ? lstBarcode[2] : null;
                                            ltcObj.Barcode4 = lstBarcode.Count > 3 ? lstBarcode[3] : null;
                                            ltcObj.Barcode5 = lstBarcode.Count > 4 ? lstBarcode[4] : null;
                                        }
                                        lstLotTransactionCheckForInsert.Add(ltcObj);
                                        //this.LotTransactionCheckDataEngine.Insert(ltcObj);
                                    }
                                    #endregion

                                    lstLotDataEngineForUpdate.Add(lotUpdate);
                                    //}
                                }

                            }
                        }
                        //2、判断功率测试时间到终检出站时间是否大于48个小时，否则进行批次暂停
                        else
                        {

                            //this.LotTransactionHistoryDataEngine.Insert(lotHistory, session);

                            //新增工艺下一步记录。
                            LotTransactionStep nextStep = new LotTransactionStep()
                            {
                                Key = transactionKey,
                                ToRouteEnterpriseName = toRouteEnterpriseName,
                                ToRouteName = toRouteName,
                                ToRouteStepName = toRouteStepName,
                                Editor = p.Creator,
                                EditTime = now
                            };
                            lstLotTransactionStepDataEngineForInsert.Add(nextStep);
                            //this.LotTransactionStepDataEngine.Insert(nextStep, session);



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
                                    //this.LotTransactionParameterDataEngine.Insert(lotParamObj, session);
                                }
                            }
                            #endregion

                            #region //批次属性
                            if (p.Attributes != null && p.Attributes.ContainsKey(lotNumber))
                            {
                                foreach (TransactionParameter tp in p.Attributes[lotNumber])
                                {
                                    LotAttribute lotParamObj = new LotAttribute()
                                    {
                                        Key = new LotAttributeKey()
                                        {
                                            AttributeName = tp.Name,
                                            LotNumber = lotNumber,
                                        },
                                        AttributeValue = tp.Value,
                                        Editor = p.Creator,
                                        EditTime = now
                                    };
                                    lstLotAttributeForInsert.Add(lotParamObj);
                                }
                            }
                            #endregion

                            #region 新增检验数据
                            if (string.IsNullOrEmpty(p.Color) == false
                                || string.IsNullOrEmpty(p.Grade) == false
                                || (p.CheckBarcodes != null && p.CheckBarcodes.ContainsKey(lotNumber)))
                            {
                                LotTransactionCheck ltcObj = new LotTransactionCheck()
                                {
                                    Key = transactionKey,
                                    Editor = p.Creator,
                                    EditTime = now
                                };
                                ltcObj.Color = p.Color;
                                ltcObj.Grade = p.Grade;

                                if (p.CheckBarcodes != null && p.CheckBarcodes.ContainsKey(lotNumber))
                                {
                                    IList<string> lstBarcode = p.CheckBarcodes[lotNumber];
                                    ltcObj.Barcode1 = lstBarcode.Count > 0 ? lstBarcode[0] : null;
                                    ltcObj.Barcode2 = lstBarcode.Count > 1 ? lstBarcode[1] : null;
                                    ltcObj.Barcode3 = lstBarcode.Count > 2 ? lstBarcode[2] : null;
                                    ltcObj.Barcode4 = lstBarcode.Count > 3 ? lstBarcode[3] : null;
                                    ltcObj.Barcode5 = lstBarcode.Count > 4 ? lstBarcode[4] : null;
                                }
                                lstLotTransactionCheckForInsert.Add(ltcObj);
                                //this.LotTransactionCheckDataEngine.Insert(ltcObj);
                            }
                            #endregion

                            lstLotDataEngineForUpdate.Add(lotUpdate);
                            //}

                        }
                }
            }
            //#endregion
            //1、判断功率测试到终检的过站时间是否大于48个小时，否则新增下条数据
            else
            {
                //this.LotTransactionHistoryDataEngine.Insert(lotHistory, session);

                //新增工艺下一步记录。
                LotTransactionStep nextStep = new LotTransactionStep()
                {
                    Key = transactionKey,
                    ToRouteEnterpriseName = toRouteEnterpriseName,
                    ToRouteName = toRouteName,
                    ToRouteStepName = toRouteStepName,
                    Editor = p.Creator,
                    EditTime = now
                };
                lstLotTransactionStepDataEngineForInsert.Add(nextStep);
                //this.LotTransactionStepDataEngine.Insert(nextStep, session);
                
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
                        //this.LotTransactionParameterDataEngine.Insert(lotParamObj, session);
                    }
                }
                #endregion

                #region //批次属性
                if (p.Attributes != null && p.Attributes.ContainsKey(lotNumber))
                {
                    foreach (TransactionParameter tp in p.Attributes[lotNumber])
                    {
                        LotAttribute lotParamObj = new LotAttribute()
                        {
                            Key = new LotAttributeKey()
                            {
                                AttributeName = tp.Name,
                                LotNumber = lotNumber,
                            },
                            AttributeValue = tp.Value,
                            Editor = p.Creator,
                            EditTime = now
                        };
                        lstLotAttributeForInsert.Add(lotParamObj);
                    }
                }
                #endregion

                #region 新增检验数据
                if (string.IsNullOrEmpty(p.Color) == false
                    || string.IsNullOrEmpty(p.Grade) == false
                    || (p.CheckBarcodes != null && p.CheckBarcodes.ContainsKey(lotNumber)))
                {
                    LotTransactionCheck ltcObj = new LotTransactionCheck()
                    {
                        Key = transactionKey,
                        Editor = p.Creator,
                        EditTime = now
                    };
                    ltcObj.Color = p.Color;
                    ltcObj.Grade = p.Grade;

                    if (p.CheckBarcodes != null && p.CheckBarcodes.ContainsKey(lotNumber))
                    {
                        IList<string> lstBarcode = p.CheckBarcodes[lotNumber];
                        ltcObj.Barcode1 = lstBarcode.Count > 0 ? lstBarcode[0] : null;
                        ltcObj.Barcode2 = lstBarcode.Count > 1 ? lstBarcode[1] : null;
                        ltcObj.Barcode3 = lstBarcode.Count > 2 ? lstBarcode[2] : null;
                        ltcObj.Barcode4 = lstBarcode.Count > 3 ? lstBarcode[3] : null;
                        ltcObj.Barcode5 = lstBarcode.Count > 4 ? lstBarcode[4] : null;
                    }
                    lstLotTransactionCheckForInsert.Add(ltcObj);
                    //this.LotTransactionCheckDataEngine.Insert(ltcObj);
                }
              

                lstLotDataEngineForUpdate.Add(lotUpdate);
                //}
            }
 #endregion

            try
            {
                #region //开始事物处理

                #region 更新批次LOT 的信息
                //更新批次基本信息
                foreach (Lot obj in lstLotDataEngineForUpdate)
                {
                    this.LotDataEngine.Update(obj, db);
                }

                //更新批次Lot Attribute 信息
                foreach (LotAttribute obj in lstLotAttributeForInsert)
                {
                    this.LotAttributeDataEngine.Insert(obj, db);
                }

                //更新批次LotTransaction信息
                foreach (LotTransaction lotTransaction in lstLotTransactionForInsert)
                {
                    this.LotTransactionDataEngine.Insert(lotTransaction, db);
                }
                foreach (LotTransactionHistory lotTransactionHistory in lstLotTransactionHistoryForInsert)
                {
                    this.LotTransactionHistoryDataEngine.Insert(lotTransactionHistory, db);
                }

                //if (executedWithTransaction == false)
                //{
                //    transaction.Commit();
                //    db.Close();
                //}
                //else
                //{
                    //db.Flush();
                //}

                #endregion
            }
            catch (Exception err)
            {
                LogHelper.WriteLogError("TrackOutLot>", err);
                //if (executedWithTransaction == false)
                //{
                //    transaction.Rollback();
                //    db.Close();
                //}
                result.Code = 1000;
                result.Message += string.Format(StringResource.Error, err.Message);
                result.Detail = err.ToString();
                return result;
            }

            //if (executedWithTransaction == false)
            //{
            //    db = this.SessionFactory.OpenSession();
            //    transaction = db.BeginTransaction();
            //}

            try
            {
                //更新批次TransactionHistory信息


                //LotTransactionParameter
                foreach (LotTransactionParameter lotTransactionParameter in lstLotTransactionParameterDataEngineForInsert)
                {
                    this.LotTransactionParameterDataEngine.Insert(lotTransactionParameter, db);
                }

                //更新批次LotTransactionStepData信息
                foreach (LotTransactionStep lotTransactionStep in lstLotTransactionStepDataEngineForInsert)
                {
                    this.LotTransactionStepDataEngine.Insert(lotTransactionStep, db);
                }

                //更新批次LotTransactionCheckDataEngine信息
                foreach (LotTransactionCheck lotTransactionCheck in lstLotTransactionCheckForInsert)
                {
                    this.LotTransactionCheckDataEngine.Insert(lotTransactionCheck, db);
                }

                #endregion

                #region //新增批次不良数据
                foreach (LotTransactionDefect lotTransactionDefect in lstLotTransactionDefectForInsert)// = new List<LotTransactionDefect>();
                {
                    LotTransactionDefectDataEngine.Update(lotTransactionDefect, db);
                }
                #endregion

                #region //新增批次报废数据
                foreach (LotTransactionScrap lotTransactionScrap in lstLotTransactionScrapForInsert)// = new List<LotTransactionDefect>();
                {
                    LotTransactionScrapDataEngine.Update(lotTransactionScrap, db);
                }
                #endregion

                #region //更新设备信息 , 设备的Event ,设备的Transaction
                //LotTransactionEquipment ,Equipment ,EquipmentStateEvent
                foreach (LotTransactionEquipment lotTransactionEquipment in lstTransactionEquipmentForUpdate)// = new List<LotTransactionDefect>();
                {
                    LotTransactionEquipmentDataEngine.Update(lotTransactionEquipment, db);
                }

                foreach (LotTransactionEquipment lotTransactionEquipment in lstTransactionEquipmentForInsert)// = new List<LotTransactionDefect>();
                {
                    LotTransactionEquipmentDataEngine.Insert(lotTransactionEquipment, db);
                }

                foreach (Equipment equipment in lstEquipmentForUpdate)
                {
                    this.EquipmentDataEngine.Update(equipment, db);
                }

                foreach (EquipmentStateEvent equipmentStateEvent in lstEquipmentStateEventtForInsert)
                {
                    //this.EquipmentStateEventDataEngine.Insert(equipmentStateEvent, db);
                    this.ExecuteAddEquipmentStateEvent(equipmentStateEvent, db, true);
                }
                #endregion


                //更新IV测试数据 及 批次基本信息
                foreach (IVTestData iVTestData in lstIVTestDataForUpdate)
                {
                    this.IVTestDataDataEngine.Update(iVTestData, db);
                }

                #region//判断是否自动进站
                foreach (LotJob lotJob in lstLotJobsForUpdate)
                {
                    this.LotJobDataEngine.Update(lotJob, db);
                }

                foreach (LotJob lotJob in lstLotJobsForInsert)
                {
                    this.LotJobDataEngine.Insert(lotJob, db);
                }
                #endregion

                #region //物料批次管理


                foreach (MaterialLoadingDetail materialLoadingDetail in lstMaterialLoadingDetailForUpdate)
                {
                    this.MaterialLoadingDetailDataEngine.Update(materialLoadingDetail, db);
                }

                foreach (LineStoreMaterialDetail lineStoreMaterialDetail in lstLineStoreMaterialDetailForUpdate)
                {
                    this.LineStoreMaterialDetailDataEngine.Update(lineStoreMaterialDetail, db);
                }

                foreach (LotBOM lotBOM in lstLotBOMForInsert)
                {
                    this.LotBOMDataEngine.Insert(lotBOM, db);
                }
                #endregion
                                
                //db.Flush();
            }
            catch (Exception err)
            {
                LogHelper.WriteLogError("TrackOutLot>", err);
                
                result.Code = 1000;
                result.Message += string.Format(StringResource.Error, err.Message);
                result.Detail = err.ToString();
                return result;
            }
            return result;
        }


        public MethodReturnResult TrackOutLot(TrackOutParameter p) 
        {
            return this.TrackOutLot(p, null, true);
        }

        private string getAttributeValueFromList(IList<RouteStepAttribute> lstRouteStepAttributeValues, string attributeName)
        {
            string strValue = "";
            RouteStepAttribute obj = null;
            var lng = from stepAttr in lstRouteStepAttributeValues.AsEnumerable()
                      where stepAttr.Key.AttributeName == attributeName
                      select stepAttr;
            if (lng != null && lng.Count() > 0)
            {
                obj = lng.FirstOrDefault();
                strValue = obj.Value;
            }
            return strValue;
        }

        //        public MethodReturnResult ExecuteTrackOutLotForConsumeMaterials(TrackOutParameter p, Lot lot, ISession db, bool executedWithTransaction)
        //        {
        //            DateTime now = DateTime.Now;
        //            string lotNumber = lot.Key;
        //            string equipmentCode = "";
        //            MethodReturnResult result = new MethodReturnResult()
        //            {
        //                Code = 0
        //            };

        //            #region //物料批次管理
        //            string strAttr1 = "";
        //            string strAttr2 = "";
        //            PagingConfig cfg = new PagingConfig()
        //            {
        //                IsPaging = false,
        //                Where = string.Format(@"Key.RouteName='{0}' 
        //                                        AND Key.RouteStepName='{1}'
        //                                        AND IsDeleted=0
        //                                        AND DCType='{2}'"
        //                                        , lot.RouteName
        //                                        , lot.RouteStepName
        //                                        , Convert.ToInt32(EnumDataCollectionAction.TrackOut)),
        //                OrderBy = "ParamIndex"
        //            };
        //            IList<RouteStepParameter> lstRouteStepParameter = this.RouteStepParameterDataEngine.Get(cfg, db);
        //            if (lstRouteStepParameter.Count > 0 && p.Paramters != null && p.Paramters.Count > 0)
        //            {
        //                //检验物料批号。
        //                foreach (TransactionParameter tp in p.Paramters[lotNumber])
        //                {
        //                    RouteStepParameter item = lstRouteStepParameter
        //                                                    .FirstOrDefault(w => w.Key.ParameterName == tp.Name);

        //                    if (item == null || item.ValidateRule == EnumValidateRule.None)
        //                    {
        //                        continue;
        //                    }

        //                    //匹配工单可用物料批号（根据领料记录）。
        //                    if (item.ValidateRule == EnumValidateRule.FullyWorkOrderMaterialLot)
        //                    {
        //                        #region //验证工单领料批号。
        //                        cfg = new PagingConfig()
        //                        {
        //                            PageNo = 0,
        //                            PageSize = 10,
        //                            Where = string.Format(@"MaterialLot='{0}'
        //                                                    AND MaterialCode LIKE '{2}%'
        //                                                    AND EXISTS(SELECT Key
        //                                                                FROM MaterialReceipt as p
        //                                                                WHERE p.OrderNumber='{1}'
        //                                                                AND p.Key=self.Key.ReceiptNo)"
        //                                                    , tp.Value
        //                                                    , lot.OrderNumber
        //                                                    , item.MaterialType),
        //                            OrderBy = "CreateTime DESC"
        //                        };

        //                        IList<MaterialReceiptDetail> lstMaterialReceiptDetail = this.MaterialReceiptDetailDataEngine.Get(cfg, db);
        //                        if (lstMaterialReceiptDetail == null || lstMaterialReceiptDetail.Count == 0)
        //                        {
        //                            string message = item.ValidateFailedMessage ?? string.Empty;
        //                            if (string.IsNullOrEmpty(message.Trim()))
        //                            {
        //                                message = "参数 （{0}） 对应物料类型（{3}）,其值 {1} 非工单（{2}）的领料批号。";
        //                            }
        //                            result.Code = 2004;
        //                            result.Message = string.Format(message
        //                                                            , item.Key.ParameterName
        //                                                            , tp.Value
        //                                                            , lot.OrderNumber
        //                                                            , item.MaterialType);
        //                            return result;
        //                        }
        //                        #endregion

        //                        equipmentCode = lot.EquipmentCode;
        //                        if (string.IsNullOrEmpty(equipmentCode))
        //                        {
        //                            equipmentCode = p.EquipmentCode;
        //                        }

        //                        #region 更新线边仓物料记录
        //                        IDictionary<string, double> dicLineStoreMaterialDetail = new Dictionary<string, double>();
        //                        //遍历领料记录
        //                        foreach (MaterialReceiptDetail mrdItem in lstMaterialReceiptDetail)
        //                        {
        //                            LineStoreMaterialDetail lsmd = this.LineStoreMaterialDetailDataEngine.Get(new LineStoreMaterialDetailKey()
        //                            {
        //                                LineStoreName = mrdItem.LineStoreName,
        //                                OrderNumber = lot.OrderNumber,
        //                                MaterialCode = mrdItem.MaterialCode,
        //                                MaterialLot = mrdItem.MaterialLot
        //                            }, db);

        //                            if (lsmd == null
        //                                || lsmd.CurrentQty == 0)
        //                            {
        //                                continue;
        //                            }


        //                            //获取物料
        //                            Material m = this.MaterialDataEngine.Get(lsmd.Key.MaterialCode ?? string.Empty, db);
        //                            //获取供应商
        //                            Supplier s = this.SupplierDataEngine.Get(lsmd.SupplierCode ?? string.Empty, db);

        //                            //获取工单BOM
        //                            if (!dicLineStoreMaterialDetail.ContainsKey(mrdItem.MaterialCode))
        //                            {
        //                                cfg = new PagingConfig()
        //                                {
        //                                    PageNo = 0,
        //                                    PageSize = 1,
        //                                    Where = string.Format(@"Key.OrderNumber='{0}'
        //                                                            AND MaterialCode='{1}'"
        //                                                        , lot.OrderNumber
        //                                                        , mrdItem.MaterialCode)
        //                                };
        //                                IList<WorkOrderBOM> lstWorkOrderBOM = this.WorkOrderBOMDataEngine.Get(cfg, db);
        //                                if (lstWorkOrderBOM == null || lstWorkOrderBOM.Count == 0)
        //                                {
        //                                    continue;
        //                                }
        //                                dicLineStoreMaterialDetail.Add(mrdItem.MaterialCode, lstWorkOrderBOM[0].Qty * lot.Quantity);
        //                            }
        //                            //更新线边仓物料记录。
        //                            double qty = dicLineStoreMaterialDetail[mrdItem.MaterialCode];

        //                            LineStoreMaterialDetail lsmdUpdate = lsmd.Clone() as LineStoreMaterialDetail;
        //                            double leftQty = lsmdUpdate.CurrentQty - qty;
        //                            if (leftQty < 0)
        //                            {
        //                                dicLineStoreMaterialDetail[mrdItem.MaterialCode] = Math.Abs(leftQty);
        //                                lsmdUpdate.CurrentQty = 0;
        //                            }
        //                            else
        //                            {
        //                                dicLineStoreMaterialDetail[mrdItem.MaterialCode] = 0;//设置数量为0
        //                                lsmdUpdate.CurrentQty = leftQty;
        //                            }
        //                            lstLineStoreMaterialDetailForUpdate.Add(lsmdUpdate);
        //                            //this.LineStoreMaterialDetailDataEngine.Update(lsmdUpdate);

        //                            //新增批次用料记录。
        //                            int lotbomItemNo = 1;
        //                            cfg = new PagingConfig()
        //                            {
        //                                PageNo = 0,
        //                                PageSize = 1,
        //                                Where = string.Format("Key.LotNumber='{0}' AND Key.MaterialLot='{1}'"
        //                                                        , lot.Key
        //                                                        , mrdItem.MaterialLot),
        //                                OrderBy = "Key.ItemNo Desc"
        //                            };

        //                            IList<LotBOM> lstLotBom = this.LotBOMDataEngine.Get(cfg, db);
        //                            if (lstLotBom.Count > 0)
        //                            {
        //                                lotbomItemNo = lstLotBom[0].Key.ItemNo + 1;
        //                            }
        //                            LotBOM lotbomObj = new LotBOM()
        //                            {
        //                                CreateTime = DateTime.Now,
        //                                Creator = p.Creator,
        //                                Editor = p.Creator,
        //                                EditTime = DateTime.Now,
        //                                EquipmentCode = equipmentCode,
        //                                LineCode = lot.LineCode,
        //                                LineStoreName = mrdItem.LineStoreName,
        //                                MaterialCode = mrdItem.MaterialCode,
        //                                MaterialName = m != null ? m.Name : string.Empty,
        //                                SupplierCode = lsmd.SupplierCode,
        //                                SupplierName = s != null ? s.Name : string.Empty,
        //                                RouteEnterpriseName = lot.RouteEnterpriseName,
        //                                RouteName = lot.RouteName,
        //                                RouteStepName = lot.RouteStepName,
        //                                TransactionKey = p.TransactionKeys[lotNumber],
        //                                MaterialFrom = EnumMaterialFrom.LineStore,
        //                                Qty = leftQty >= 0 ? qty : qty + leftQty,
        //                                Key = new LotBOMKey()
        //                                {
        //                                    LotNumber = lot.Key,
        //                                    MaterialLot = mrdItem.MaterialLot,
        //                                    ItemNo = lotbomItemNo
        //                                }
        //                            };
        //                            lstLotBOMForInsert.Add(lotbomObj);
        //                            //this.LotBOMDataEngine.Insert(lotbomObj);

        //                            //如果数量满足，跳出。
        //                            if (dicLineStoreMaterialDetail[mrdItem.MaterialCode] == 0)
        //                            {
        //                                break;
        //                            }
        //                        }
        //                        //线边仓物料数量不足。
        //                        var lnq = from d in dicLineStoreMaterialDetail
        //                                  where d.Value > 0
        //                                  select d;
        //                        if (lnq.Count() > 0)
        //                        {
        //                            string message = item.ValidateFailedMessage ?? string.Empty;
        //                            if (string.IsNullOrEmpty(message.Trim()))
        //                            {
        //                                message = "参数 （{0}） 值 {1} 对应物料不足。";
        //                            }
        //                            result.Code = 2004;
        //                            result.Message = string.Format(message
        //                                                            , item.Key.ParameterName
        //                                                            , tp.Value);
        //                            return result;
        //                        }
        //                        #endregion
        //                    }
        //                    //匹配设备上料批号（根据上料记录）。
        //                    else if (item.ValidateRule == EnumValidateRule.FullyLoadingMaterialLot)
        //                    {
        //                        equipmentCode = lot.EquipmentCode;
        //                        if (string.IsNullOrEmpty(equipmentCode))
        //                        {
        //                            equipmentCode = p.EquipmentCode;
        //                        }

        //                        #region //验证设备上料批号。
        //                        cfg = new PagingConfig()
        //                        {
        //                            IsPaging = false,
        //                            Where = string.Format(@"MaterialLot='{0}'
        //                                                    AND OrderNumber='{4}'
        //                                                    AND MaterialCode LIKE '{3}%'
        //                                                    AND CurrentQty>0
        //                                                    AND EXISTS(SELECT Key
        //                                                                FROM MaterialLoading as p
        //                                                                WHERE p.RouteOperationName='{1}'
        //                                                                AND p.EquipmentCode='{2}'
        //                                                                AND p.Key=self.Key.LoadingKey)"
        //                                                    , tp.Value
        //                                                    , lot.RouteStepName
        //                                                    , equipmentCode
        //                                                    , item.MaterialType
        //                                                    , lot.OrderNumber)
        //                        };
        //                        //获取上料记录。
        //                        IList<MaterialLoadingDetail> lstMaterialLoadingDetail = this.MaterialLoadingDetailDataEngine.Get(cfg, db);
        //                        if (lstMaterialLoadingDetail == null || lstMaterialLoadingDetail.Count == 0)
        //                        {
        //                            string message = item.ValidateFailedMessage ?? string.Empty;
        //                            if (string.IsNullOrEmpty(message.Trim()))
        //                            {
        //                                message = "参数 （{0}） 值 {1} 非工单（{4}）工序（{2}）设备（{3}）上料批号。";
        //                            }
        //                            result.Code = 2004;
        //                            result.Message = string.Format(message
        //                                                            , item.Key.ParameterName
        //                                                            , tp.Value
        //                                                            , lot.RouteStepName
        //                                                            , equipmentCode
        //                                                            , lot.OrderNumber);
        //                            return result;
        //                        }
        //                        #endregion

        //                        #region 更新上料记录
        //                        IDictionary<string, double> dicMaterialLoadingDetail = new Dictionary<string, double>();
        //                        string loadingMaterialCode = lstMaterialLoadingDetail[0].MaterialCode;
        //                        //获取工单BOM
        //                        if (!dicMaterialLoadingDetail.ContainsKey(loadingMaterialCode))
        //                        {
        //                            cfg = new PagingConfig()
        //                            {
        //                                PageNo = 0,
        //                                PageSize = 1,
        //                                Where = string.Format(@"Key.OrderNumber='{0}'
        //                                                        AND MaterialCode='{1}'"
        //                                                    , lot.OrderNumber
        //                                                    , loadingMaterialCode)
        //                            };
        //                            IList<WorkOrderBOM> lstWorkOrderBOM = this.WorkOrderBOMDataEngine.Get(cfg, db);
        //                            if (lstWorkOrderBOM == null || lstWorkOrderBOM.Count == 0)
        //                            {
        //                                continue;
        //                            }
        //                            dicMaterialLoadingDetail.Add(loadingMaterialCode, lstWorkOrderBOM[0].Qty * lot.Quantity);
        //                        }

        //                        Dictionary<LotBOMKey, int> dicLotBomKey = new Dictionary<LotBOMKey, int>();
        //                        //遍历上料记录
        //                        foreach (MaterialLoadingDetail mldItem in lstMaterialLoadingDetail)
        //                        {
        //                            //更新上料记录。
        //                            double qty = dicMaterialLoadingDetail[loadingMaterialCode];
        //                            MaterialLoadingDetail mldItemUpdate = mldItem.Clone() as MaterialLoadingDetail;
        //                            double leftQty = mldItemUpdate.CurrentQty - qty;
        //                            if (leftQty < 0)
        //                            {
        //                                dicMaterialLoadingDetail[loadingMaterialCode] = Math.Abs(leftQty);
        //                                mldItemUpdate.CurrentQty = 0;
        //                            }
        //                            else
        //                            {
        //                                dicMaterialLoadingDetail[loadingMaterialCode] = 0;//设置数量为0
        //                                mldItemUpdate.CurrentQty = leftQty;
        //                            }
        //                            lstMaterialLoadingDetailForUpdate.Add(mldItemUpdate);
        //                            //this.MaterialLoadingDetailDataEngine.Update(mldItemUpdate);

        //                            LineStoreMaterialDetail lsmd = this.LineStoreMaterialDetailDataEngine.Get(new LineStoreMaterialDetailKey()
        //                            {
        //                                LineStoreName = mldItem.LineStoreName,
        //                                OrderNumber = lot.OrderNumber,
        //                                MaterialCode = mldItem.MaterialCode,
        //                                MaterialLot = mldItem.MaterialLot
        //                            }, db);

        //                            Material m = null;
        //                            Supplier s = null;
        //                            if (lsmd != null)
        //                            {
        //                                //获取物料
        //                                m = this.MaterialDataEngine.Get(lsmd.Key.MaterialCode ?? string.Empty, db);
        //                                //获取供应商
        //                                s = this.SupplierDataEngine.Get(lsmd.SupplierCode ?? string.Empty, db);
        //                            }

        //                            //新增批次用料记录。
        //                            int lotbomItemNo = 1;
        //                            cfg = new PagingConfig()
        //                            {
        //                                PageNo = 0,
        //                                PageSize = 1,
        //                                Where = string.Format("Key.LotNumber='{0}' AND Key.MaterialLot='{1}'"
        //                                                        , lot.Key
        //                                                        , mldItem.MaterialLot),
        //                                OrderBy = "Key.ItemNo Desc"
        //                            };

        //                            IList<LotBOM> lstLotBom = this.LotBOMDataEngine.Get(cfg, db);
        //                            if (lstLotBom.Count > 0)
        //                            {
        //                                lotbomItemNo = lstLotBom[0].Key.ItemNo + 1;
        //                            }

        //                            LotBOMKey lotBomKey = new LotBOMKey
        //                            {
        //                                LotNumber = lot.Key,
        //                                MaterialLot = mldItem.MaterialLot,
        //                                ItemNo = lotbomItemNo
        //                            };

        //                            if (dicLotBomKey.ContainsKey(lotBomKey))
        //                            {
        //                                int itemNo = 0;
        //                                dicLotBomKey.TryGetValue(lotBomKey, out itemNo);
        //                                dicLotBomKey.Remove(lotBomKey);
        //                                lotBomKey.ItemNo = itemNo + 1;
        //                                dicLotBomKey.Add(lotBomKey, lotBomKey.ItemNo);
        //                            }
        //                            else
        //                            {
        //                                dicLotBomKey.Add(lotBomKey, lotBomKey.ItemNo);
        //                            }
        //                            LotBOM lotbomObj = new LotBOM()
        //                            {
        //                                CreateTime = DateTime.Now,
        //                                Creator = p.Creator,
        //                                Editor = p.Creator,
        //                                EditTime = DateTime.Now,
        //                                EquipmentCode = equipmentCode,
        //                                LineCode = lot.LineCode,
        //                                LineStoreName = mldItem.LineStoreName,
        //                                MaterialCode = mldItem.MaterialCode,
        //                                MaterialName = m != null ? m.Name : string.Empty,
        //                                SupplierCode = lsmd != null ? lsmd.SupplierCode : string.Empty,
        //                                SupplierName = s != null ? s.Name : string.Empty,
        //                                RouteEnterpriseName = lot.RouteEnterpriseName,
        //                                RouteName = lot.RouteName,
        //                                RouteStepName = lot.RouteStepName,
        //                                TransactionKey = p.TransactionKeys[lotNumber],
        //                                MaterialFrom = EnumMaterialFrom.Loading,
        //                                LoadingItemNo = mldItem.Key.ItemNo,
        //                                LoadingKey = mldItem.Key.LoadingKey,
        //                                Qty = leftQty >= 0 ? qty : qty + leftQty,
        //                                Key = lotBomKey
        //                            };
        //                            lstLotBOMForInsert.Add(lotbomObj);
        //                            //this.LotBOMDataEngine.Insert(lotbomObj);
        //                            //如果数量满足，跳出。
        //                            if (dicMaterialLoadingDetail[loadingMaterialCode] == 0)
        //                            {
        //                                break;
        //                            }
        //                            MethodReturnResult<MaterialLoading> lstMaterialLoading = new MethodReturnResult<MaterialLoading>();
        //                            lstMaterialLoading.Data = this.MaterialLoadingDataEngine.Get(mldItem.Key.LoadingKey);
        //                            string loadingdate = lstMaterialLoading.Data.LoadingTime.ToString();
        //                            BorrowMaterials(lot.MaterialCode, lot.EquipmentCode, loadingdate);
        //                        }
        //                        //上料数量不足。
        //                        var lnq = from d in dicMaterialLoadingDetail
        //                                  where d.Value > 0
        //                                  select d;
        //                        if (lnq.Count() > 0)
        //                        {
        //                            string message = item.ValidateFailedMessage ?? string.Empty;
        //                            if (string.IsNullOrEmpty(message.Trim()))
        //                            {
        //                                message = "参数 （{0}） 值 {1} 在工序（{2}）设备（{3}）上料不足。";
        //                            }
        //                            result.Code = 2004;
        //                            result.Message = string.Format(message
        //                                                            , item.Key.ParameterName
        //                                                            , tp.Value
        //                                                            , lot.RouteStepName
        //                                                            , equipmentCode);
        //                            return result;
        //                        }
        //                        #endregion
        //                    }
        //                    else
        //                    {
        //                        #region //验证工单BOM
        //                        cfg = new PagingConfig()
        //                        {
        //                            PageNo = 0,
        //                            PageSize = 1,
        //                            Where = string.Format(@"Key.OrderNumber='{0}'"
        //                                                , lot.OrderNumber)
        //                        };

        //                        if (item.ValidateRule == EnumValidateRule.FullyWorkorderBOM)
        //                        {
        //                            cfg.Where += string.Format(" AND  MaterialCode='{0}'", tp.Value);
        //                        }
        //                        else if (item.ValidateRule == EnumValidateRule.PrefixWorkorderBOM)
        //                        {
        //                            cfg.Where += string.Format(" AND  MaterialCode LIKE '{0}%'", tp.Value);
        //                        }
        //                        else if (item.ValidateRule == EnumValidateRule.SuffixWorkorderBOM)
        //                        {
        //                            cfg.Where += string.Format(" AND  MaterialCode LIKE '%{0}'", tp.Value);
        //                        }
        //                        else
        //                        {
        //                            cfg.Where += string.Format(" AND  MaterialCode LIKE '%{0}%'", tp.Value);
        //                        }

        //                        IList<WorkOrderBOM> lstWorkOrderBOM = this.WorkOrderBOMDataEngine.Get(cfg, db);
        //                        if (lstWorkOrderBOM == null || lstWorkOrderBOM.Count == 0)
        //                        {
        //                            result.Code = 2005;
        //                            string message = item.ValidateFailedMessage ?? string.Empty;
        //                            if (string.IsNullOrEmpty(message.Trim()))
        //                            {
        //                                message = "参数 （{0}） 值 {1} 在工单（{2}）BOM中不存在。";
        //                            }
        //                            result.Message = string.Format(message
        //                                                            , item.Key.ParameterName
        //                                                            , tp.Value
        //                                                            , lot.OrderNumber);
        //                            return result;
        //                        }
        //                        #endregion
        //                    }

        //                    //检查是否需要获取电池片批次信息
        //                    if (string.Compare(tp.Name, "电池片批号") == 0)
        //                    {
        //                        MaterialReceiptDetail mReceiptDetail = this.getMaterialReceiptDetail(tp.Value, lot.OrderNumber, db);
        //                        if (mReceiptDetail != null)
        //                        {
        //                            lotUpdate.Attr1 = mReceiptDetail.Attr1;
        //                            lotUpdate.Attr2 = mReceiptDetail.Attr2;
        //                        }
        //                    }
        //                }
        //            }

        //            #endregion
        //            return result;
        //        }

        //public IList<MaterialLoadingDetail> BorrowMaterials(Lot lot, string LoadingTime)
        //{
        //    IList<MaterialLoadingDetail> lst = null;
        //    lst = this.GetMaterialLoading(lot.MaterialCode, lot.EquipmentCode, LoadingTime);
        //    return lst;
        //}

    }

}
           