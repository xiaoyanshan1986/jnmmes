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
    /// 实现批次出站服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class LotTrackOutService : ILotTrackOutContract, ILotTrackOutCheck, ILotTrackOut
    {
        /// <summary>
        /// 操作前检查事件。
        /// </summary>
        public event Func<TrackOutParameter, MethodReturnResult> CheckEvent;
        /// <summary>
        /// 执行操作时事件。
        /// </summary>
        public event Func<TrackOutParameter, MethodReturnResult> ExecutingEvent;
        /// <summary>
        /// 操作执行完成事件。
        /// </summary>
        public event Func<TrackOutParameter, MethodReturnResult> ExecutedEvent;

        /// <summary>
        /// 自定义操作前检查的清单列表。
        /// </summary>
        private IList<ILotTrackOutCheck> CheckList { get; set; }
        /// <summary>
        /// 自定义执行中操作的清单列表。
        /// </summary>
        private IList<ILotTrackOut> ExecutingList { get; set; }
        /// <summary>
        /// 自定义执行后操作的清单列表。
        /// </summary>
        private IList<ILotTrackOut> ExecutedList { get; set; }


        /// <summary>
        /// 注册自定义检查的操作实例。
        /// </summary>
        /// <param name="obj"></param>
        public void RegisterCheckInstance(ILotTrackOutCheck obj)
        {
            if (this.CheckList == null)
            {
                this.CheckList = new List<ILotTrackOutCheck>();
            }
            this.CheckList.Add(obj);
        }
        /// <summary>
        /// 注册执行中的自定义操作实例。
        /// </summary>
        /// <param name="obj"></param>
        public void RegisterExecutingInstance(ILotTrackOut obj)
        {
            if (this.ExecutingList == null)
            {
                this.ExecutingList = new List<ILotTrackOut>();
            }
            this.ExecutingList.Add(obj);
        }

        /// <summary>
        /// 注册执行完成后的自定义操作实例。
        /// </summary>
        /// <param name="obj"></param>
        public void RegisterExecutedInstance(ILotTrackOut obj)
        {
            if (this.ExecutedList == null)
            {
                this.ExecutedList = new List<ILotTrackOut>();
            }
            this.ExecutedList.Add(obj);
        }


        /// <summary>
        /// 操作前检查。
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        protected virtual MethodReturnResult OnCheck(TrackOutParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };
            StringBuilder sbMessage = new StringBuilder();
            if (this.CheckEvent != null)
            {
                foreach (Func<TrackOutParameter, MethodReturnResult> d in this.CheckEvent.GetInvocationList())
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
                foreach (ILotTrackOutCheck d in this.CheckList)
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
        protected virtual MethodReturnResult OnExecuting(TrackOutParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };
            StringBuilder sbMessage = new StringBuilder();
            if (this.ExecutingEvent != null)
            {
                foreach (Func<TrackOutParameter, MethodReturnResult> d in this.ExecutingEvent.GetInvocationList())
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
                foreach (ILotTrackOut d in this.ExecutingList)
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
        protected virtual MethodReturnResult OnExecuted(TrackOutParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };
            StringBuilder sbMessage = new StringBuilder();
            if (this.ExecutedEvent != null)
            {
                foreach (Func<TrackOutParameter, MethodReturnResult> d in this.ExecutedEvent.GetInvocationList())
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
                foreach (ILotTrackOut d in this.ExecutedList)
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
        public LotTrackOutService()
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
        /// 工艺流程组明细数据访问对象。
        /// </summary>
        public IRouteEnterpriseDetailDataEngine RouteEnterpriseDetailDataEngine
        {
            get;
            set;
        }
        /// <summary>
        /// 工艺工步数据访问对象。
        /// </summary>
        public IRouteStepDataEngine RouteStepDataEngine
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
        /// 批次加工设备数据访问对象。
        /// </summary>
        public ILotTransactionEquipmentDataEngine LotTransactionEquipmentDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 批次检验数据访问对象。
        /// </summary>
        public ILotTransactionCheckDataEngine LotTransactionCheckDataEngine
        {
            get;
            set;
        }

        /// <summary>
        /// 批次出站操作。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>
        /// 代码表示：0：成功，其他失败。
        /// </returns>
        MethodReturnResult ILotTrackOutContract.TrackOut(TrackOutParameter p)
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
                result.Message += string.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 操作前检查。
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        MethodReturnResult ILotTrackOutCheck.Check(TrackOutParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

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
                if (lot.StateFlag != EnumLotState.WaitTrackOut)
                {
                    result.Code = 1006;
                    result.Message = string.Format("批次（{0}）目前状态（{1}）,非({2})状态。"
                                                    , lotNumber
                                                    , lot.StateFlag.GetDisplayName()
                                                    , EnumLotState.WaitTrackOut.GetDisplayName());
                    return result;
                }
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
            }
            return result;
        }
        /// <summary>
        /// 执行操作。
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        MethodReturnResult ILotTrackOut.Execute(TrackOutParameter p)
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

                //根据批次当前工艺工步获取下一个工步
                RouteStepKey rsKey=new RouteStepKey()
                {
                     RouteName=lot.RouteName,
                     RouteStepName=lot.RouteStepName
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
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo=0,
                    PageSize=1,
                    Where = string.Format(@"Key.RouteName='{0}'
                                            AND SortSeq>='{1}'"
                                            ,rsObj.Key.RouteName
                                            ,rsObj.SortSeq+1),
                    OrderBy="SortSeq"
                };
                IList<RouteStep> lstRouteStep = this.RouteStepDataEngine.Get(cfg);
                if (lstRouteStep.Count == 0)
                {
                    //获取下一个工艺流程。
                    RouteEnterpriseDetail reDetail = this.RouteEnterpriseDetailDataEngine.Get(new RouteEnterpriseDetailKey()
                    {
                        RouteEnterpriseName=lot.RouteEnterpriseName,
                        RouteName=lot.RouteName
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
                    if(lstRouteEnterpriseDetail.Count>0)
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
                if (lstRouteStep!=null && lstRouteStep.Count > 0)
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

                if (!string.IsNullOrEmpty(p.Color))
                {
                    lotUpdate.Color = p.Color;
                }
                if (!string.IsNullOrEmpty(p.Grade))
                {
                    lotUpdate.Grade = p.Grade;
                }
                lotUpdate.StartProcessTime = null;
                lotUpdate.StateFlag = isFinish?EnumLotState.Finished:EnumLotState.WaitTrackIn;
                lotUpdate.RouteEnterpriseName = toRouteEnterpriseName;
                lotUpdate.RouteName = toRouteName;
                lotUpdate.RouteStepName = toRouteStepName;
                lotUpdate.OperateComputer = p.OperateComputer;
                lotUpdate.PreLineCode = lot.LineCode;
                lotUpdate.LineCode = p.LineCode;
                lotUpdate.Editor = p.Creator;
                lotUpdate.EditTime = now;
                lotUpdate.EquipmentCode = null;
                this.LotDataEngine.Update(lotUpdate);
                
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
                    foreach (LotTransactionEquipment item in lstLotTransactionEquipment)
                    {
                        LotTransactionEquipment itemUpdate = item.Clone() as LotTransactionEquipment;
                        itemUpdate.EndTransactionKey = transactionKey;
                        itemUpdate.EditTime = now;
                        itemUpdate.Editor = p.Creator;
                        itemUpdate.EndTime = now;
                        itemUpdate.State = EnumLotTransactionEquipmentState.End;
                        this.LotTransactionEquipmentDataEngine.Update(itemUpdate);
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
                    this.LotTransactionEquipmentDataEngine.Insert(transEquipment);
                }
                

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
                    LineCode=string.IsNullOrEmpty(p.LineCode)?lot.LineCode:p.LineCode,
                    LocationName=lot.LocationName,
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

                //新增工艺下一步记录。
                LotTransactionStep nextStep = new LotTransactionStep()
                {
                    Key = transactionKey,
                    ToRouteEnterpriseName =toRouteEnterpriseName ,
                    ToRouteName = toRouteName,
                    ToRouteStepName = toRouteStepName,
                    Editor = p.Creator,
                    EditTime = now
                };
                this.LotTransactionStepDataEngine.Insert(nextStep);
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

                #region 新增检验数据
                if (string.IsNullOrEmpty(p.Color)==false
                    || string.IsNullOrEmpty(p.Grade)==false
                    || (p.CheckBarcodes != null && p.CheckBarcodes.ContainsKey(lotNumber)))
                {
                    LotTransactionCheck ltcObj = new LotTransactionCheck()
                    {
                        Key=transactionKey,
                        Editor = p.Creator,
                        EditTime = now
                    };
                    ltcObj.Color = p.Color;
                    ltcObj.Grade = p.Grade;

                    if(p.CheckBarcodes != null && p.CheckBarcodes.ContainsKey(lotNumber))
                    {
                        IList<string> lstBarcode=p.CheckBarcodes[lotNumber];
                        ltcObj.Barcode1 = lstBarcode.Count > 0 ? lstBarcode[0] : null;
                        ltcObj.Barcode2 = lstBarcode.Count > 1 ? lstBarcode[1] : null;
                        ltcObj.Barcode3 = lstBarcode.Count > 2 ? lstBarcode[2] : null;
                        ltcObj.Barcode4 = lstBarcode.Count > 3 ? lstBarcode[3] : null;
                        ltcObj.Barcode5 = lstBarcode.Count > 4 ? lstBarcode[4] : null;
                    }

                    this.LotTransactionCheckDataEngine.Insert(ltcObj);
                }
                #endregion 
            }
            return result;
        }


    }
}
