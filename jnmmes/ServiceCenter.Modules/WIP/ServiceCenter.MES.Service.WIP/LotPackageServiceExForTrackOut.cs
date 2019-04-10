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
using NHibernate;


namespace ServiceCenter.MES.Service.WIP
{
    /// <summary>
    /// 扩展批次包装，进行批次进站操作。
    /// </summary>
    public partial class LotPackageService
    {
        /// <summary>
        /// 工艺工步数据访问对象。
        /// </summary>
        public IRouteStepDataEngine RouteStepDataEngine
        {
            get;
            set;
        }

        public IRouteEnterpriseDetailDataEngine RouteEnterpriseDetailDataEngine
        {
            get;
            set;
        }

        public ILotTransactionEquipmentDataEngine LotTransactionEquipmentDataEngine
        {
            get;
            set;
        }

        public ILotTransactionStepDataEngine LotTransactionStepDataEngine
        {
            get;
            set;
        }

        public ILotTransactionCheckDataEngine LotTransactionCheckDataEngine
        {
            get;
            set;
        }

        MethodReturnResult ExecuteTrackOutPackage(PackageParameter p)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (p == null)
            {
                result.Code = 1001;
                result.Message = StringResource.ParameterIsNull;
                return result;
            }
            DateTime now = DateTime.Now;
            Package packageObj = null;
            p.TransactionKeys = new Dictionary<string, string>();
            
            packageObj = this.PackageDataEngine.Get(p.PackageNo);
            if (packageObj.PackageState != EnumPackageState.Packaged)
            {
                result.Code = 1001;
                result.Message = string.Format("包{0}还没有满包", p.PackageNo);
                return result;
            }
            PagingConfig cfg = new PagingConfig()
            {
                IsPaging = false,
                Where = string.Format(@"Key.PackageNo='{0}'
                                        and Exists(
                                                select Key from Lot as p
                                                where p.Key = self.Key.ObjectNumber
                                                and p.PackageNo ='{0}'
                                                and p.StateFlag ='{1}'
                                                )", 
                                                p.PackageNo,
                                                Convert.ToInt32(EnumLotState.WaitTrackOut)
                                       )
            };
            IList<PackageDetail> lstPackageDetail = this.PackageDetailDataEngine.Get(cfg);
            if(lstPackageDetail==null || lstPackageDetail.Count==0)
            {
                result.Code = 1001;
                result.Message = string.Format("包{0}不存在待出站的批次号。", p.PackageNo);
                return result;
            }
   
            #region define List of DataEngine
            List<Lot> lstLotDataEngineForUpdate = new List<Lot>();
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
            List<EquipmentStateEvent> lstEquipmentStateEventForInsert = new List<EquipmentStateEvent>();

            #endregion

            if (lstPackageDetail != null && lstPackageDetail.Count > 0)
            {
                foreach (PackageDetail item in lstPackageDetail)
                {
                    Lot lot = this.LotDataEngine.Get(item.Key.ObjectNumber);
                    string lotNumber = lot.Key;
                    string transactionKey = Guid.NewGuid().ToString();
                    if (lot == null)
                    {
                        continue;
                    }
                    //批次处于等待出站状态。
                    if (lot.StateFlag == EnumLotState.WaitTrackOut)
                    {
                        #region //获取 Lot信息，下一道工序
                        //生成操作事务主键。                
                        p.TransactionKeys.Add(lotNumber, transactionKey);

                        //根据批次当前工艺工步获取下一个工步
                        RouteStepKey rsKey = new RouteStepKey()
                        {
                            RouteName = lot.RouteName,
                            RouteStepName = lot.RouteStepName
                        };
                        RouteStep rsObj = this.RouteStepDataEngine.Get(rsKey);
                        if (rsObj == null)
                        {
                            result.Code = 2001;
                            result.Message = string.Format("批次（{0}）所在工艺流程（{1}）不存在。"
                                                            , lotNumber
                                                            , rsKey);
                            return result;
                        }
                        cfg = new PagingConfig()
                        {
                            PageNo = 0,
                            PageSize = 1,
                            Where = string.Format(@"Key.RouteName='{0}'
                                        AND SortSeq>='{1}'"
                                                    , rsObj.Key.RouteName
                                                    , rsObj.SortSeq + 1),
                            OrderBy = "SortSeq"
                        };
                        IList<RouteStep> lstRouteStep = this.RouteStepDataEngine.Get(cfg);
                        if (lstRouteStep.Count == 0)
                        {
                            //获取下一个工艺流程。
                            RouteEnterpriseDetail reDetail = this.RouteEnterpriseDetailDataEngine.Get(new RouteEnterpriseDetailKey()
                            {
                                RouteEnterpriseName = lot.RouteEnterpriseName,
                                RouteName = lot.RouteName
                            });

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
                            IList<RouteEnterpriseDetail> lstRouteEnterpriseDetail = this.RouteEnterpriseDetailDataEngine.Get(cfg);
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
                                lstRouteStep = this.RouteStepDataEngine.Get(cfg);
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
                            lotUpdate.StartWaitTime = null;
                        }
                        else
                        {
                            lotUpdate.StartWaitTime = now;
                        }

                        lotUpdate.StartProcessTime = null;
                        lotUpdate.StateFlag = EnumLotState.WaitTrackIn;
                        //lotUpdate.StateFlag = isFinish ? EnumLotState.Finished : EnumLotState.WaitTrackIn;
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

                        #region //更新设备信息 , 设备的Event ,设备的Transaction

                        //bool blLogEquipment = true;
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
                            IList<LotTransactionEquipment> lstLotTransactionEquipment = this.LotTransactionEquipmentDataEngine.Get(cfg);
                            foreach (LotTransactionEquipment lotTransactionEquipment in lstLotTransactionEquipment)
                            {
                                LotTransactionEquipment itemUpdate = lotTransactionEquipment.Clone() as LotTransactionEquipment;
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

                        #region 设备状态信息更新
                        /*
                        //如果出站也没有选择设备，直接返回。                       
                        if (string.IsNullOrEmpty(equipmentCode) == false)
                        {
                            //获取设备数据
                            Equipment e = this.EquipmentDataEngine.Get(equipmentCode ?? string.Empty);
                            if (e != null)
                            {
                                //获取设备当前状态。
                                EquipmentState es = this.EquipmentStateDataEngine.Get(e.StateName ?? string.Empty);
                                if (es != null)
                                {
                                    //获取设备LOST的主键
                                    EquipmentState lostState = this.EquipmentStateDataEngine.Get("LOST");
                                    //获取设备当前状态->LOST的状态切换数据。
                                    EquipmentChangeState ecsToLost = this.EquipmentChangeStateDataEngine.Get(es.Key, lostState.Key);

                                    if (ecsToLost != null)
                                    {
                                        //根据设备编码获取当前加工批次数据。
                                        cfg = new PagingConfig()
                                        {
                                            PageSize = 1,
                                            PageNo = 0,
                                            Where = string.Format("EquipmentCode='{0}' AND STATE='{1}'"
                                                                    , equipmentCode
                                                                    , Convert.ToInt32(EnumLotTransactionEquipmentState.Start))
                                        };
                                        IList<LotTransactionEquipment> lst = this.LotTransactionEquipmentDataEngine.Get(cfg);
                                        if (lst.Count == 0)//设备当前加工批次>0，则直接返回。
                                        {
                                            #region Change EqupmentState
                                            //更新父设备状态。
                                            if (!string.IsNullOrEmpty(e.ParentEquipmentCode))
                                            {
                                                Equipment ep = this.EquipmentDataEngine.Get(e.ParentEquipmentCode);
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
                                                    lstEquipmentStateEventForInsert.Add(newStateEvent);
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
                                            lstEquipmentStateEventForInsert.Add(stateEvent);
                                            #endregion
                                        }
                                    }
                                }
                            }
                        }*/
                        #endregion

                        #endregion

                        #region//记录操作历史。
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
                        //this.LotTransactionDataEngine.Insert(transObj, session);

                        //新增批次历史记录。
                        LotTransactionHistory lotHistory = new LotTransactionHistory(transactionKey, lot);
                        lstLotTransactionHistoryForInsert.Add(lotHistory);
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
                        #endregion

                        lstLotDataEngineForUpdate.Add(lotUpdate);   
                    }
                }
            }

            try
            {
                ISession session = this.LotDataEngine.SessionFactory.OpenSession();
                ITransaction transaction = session.BeginTransaction();
            
                #region //开始事物处理

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
               /*
               //LotTransactionParameter
               foreach (LotTransactionParameter lotTransactionParameter in lstLotTransactionParameterDataEngineForInsert)
               {
                   this.LotTransactionParameterDataEngine.Insert(lotTransactionParameter, session);
               }

               //更新批次LotTransactionStepData信息
               foreach (LotTransactionStep lotTransactionStep in lstLotTransactionStepDataEngineForInsert)
               {
                   this.LotTransactionStepDataEngine.Insert(lotTransactionStep, session);
               }

               //更新批次LotTransactionCheckDataEngine信息
               foreach (LotTransactionCheck lotTransactionCheck in lstLotTransactionCheckForInsert)
               {
                   this.LotTransactionCheckDataEngine.Insert(lotTransactionCheck, session);
               }

               #endregion

               #region //更新设备信息 , 设备的Event ,设备的Transaction
               //LotTransactionEquipment ,Equipment ,EquipmentStateEvent
               foreach (LotTransactionEquipment lotTransactionEquipment in lstTransactionEquipmentForUpdate)// = new List<LotTransactionDefect>();
               {
                   LotTransactionEquipmentDataEngine.Update(lotTransactionEquipment, session);
               }

               foreach (LotTransactionEquipment lotTransactionEquipment in lstTransactionEquipmentForInsert)// = new List<LotTransactionDefect>();
               {
                   LotTransactionEquipmentDataEngine.Insert(lotTransactionEquipment, session);
               }

               foreach (Equipment equipment in lstEquipmentForUpdate)
               {
                   this.EquipmentDataEngine.Update(equipment, session);
               }

               foreach (EquipmentStateEvent equipmentStateEvent in lstEquipmentStateEventForInsert)
               {
                   this.EquipmentStateEventDataEngine.Insert(equipmentStateEvent, session);
               }
               */
                #endregion

                transaction.Commit();
                session.Close();
                #endregion
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = string.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }
    }
}
