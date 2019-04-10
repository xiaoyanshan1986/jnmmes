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
using ServiceCenter.Service.Client;

namespace ServiceCenter.MES.Service.WIP
{
    /// <summary>
    /// 实现批次出站服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public partial class LotTrackOutService : ILotTrackOutContract, ILotTrackOutCheck, ILotTrackOut
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

        public IRouteStepAttributeDataEngine RouteStepAttributeDataEngine
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
        /// 校准板线别信息
        /// </summary>
        public ICalibrationPlateLineDataEngine CalibrationPlateLineDataEngine
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
                LogHelper.WriteLogError("TrackOutLot>", ex);
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
            if (p.LotNumbers == null || p.LotNumbers.Count == 0)
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
                    result.Message = string.Format("批次（{0}）已暂停", lotNumber);
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
            List<EquipmentStateEvent> lstEquipmentStateEventtForInsert = new List<EquipmentStateEvent>();

            List<IVTestData> lstIVTestDataForUpdate = new List<IVTestData>();

            List<LotJob> lstLotJobsForInsert = new List<LotJob>();
            List<LotJob> lstLotJobsForUpdate = new List<LotJob>();

            List<LotBOM> lstLotBOMForInsert = new List<LotBOM>();
            List<MaterialLoadingDetail> lstMaterialLoadingDetailForUpdate = new List<MaterialLoadingDetail>();
            List<LineStoreMaterialDetail> lstLineStoreMaterialDetailForUpdate = new List<LineStoreMaterialDetail>();
            #endregion

            //PagingConfig cfg = new PagingConfig()
            //{
            //    IsPaging = false,
            //    Where = string.Format(@" Key.CategoryName='{0}' and Key.AttributeName='{1}'"
            //     , "SystemConfigAttribute", "IsCreateLotForConsumedCell")

            //};
            //bool isCreateLotForConsumedCell = true;
            //IList<BaseAttributeValue> lstBaseAttributeValues = this.BaseAttributeValueDataEngine.Get(cfg);
            //if (lstBaseAttributeValues != null && lstBaseAttributeValues.Count > 0)
            //{
            //    BaseAttributeValue obj = lstBaseAttributeValues.FirstOrDefault();
            //    if (String.IsNullOrEmpty(obj.Value) == false)
            //    {
            //        Boolean.TryParse(obj.Value, out isCreateLotForConsumedCell);
            //    }
            //}


            foreach (string lotNumber in p.LotNumbers)
            {
                #region For each Normal LOT TrackOut
                //循环批次。
                Lot lot = this.LotDataEngine.Get(lotNumber);
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

                if (!string.IsNullOrEmpty(p.Color))
                {
                    lotUpdate.Color = p.Color;
                }
                if (!string.IsNullOrEmpty(p.Grade))
                {
                    lotUpdate.Grade = p.Grade;
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
                if (string.IsNullOrEmpty(equipmentCode)==false)
                {
                    //获取设备数据
                    Equipment e = this.EquipmentDataEngine.Get(equipmentCode ?? string.Empty);
                    if(e!=null)
                    {
                        //获取设备当前状态。
                        EquipmentState es = this.EquipmentStateDataEngine.Get(e.StateName ?? string.Empty);
                        if (es!=null)
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
                                    OrderBy = "#*#",
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
                RouteStepAttribute rsa = this.RouteStepAttributeDataEngine.Get(key);
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
                    IList<IVTestData> lstTestData = this.IVTestDataDataEngine.Get(cfg);
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
                    IList<WorkOrderProduct> lstWorkOrderProduct = this.WorkOrderProductDataEngine.Get(cfg);
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

                        #region //进行衰减。
                        //获取工单衰减规则。
                        cfg.Where = string.Format(@"Key.OrderNumber='{0}' 
                                                    AND Key.MaterialCode='{1}' 
                                                    AND Key.MinPower<='{2}'
                                                    AND Key.MaxPower>='{2}'
                                                    AND IsUsed=1"
                                                   , lotUpdate.OrderNumber
                                                   , lotUpdate.MaterialCode
                                                   , testData.PM
                                                   , testData.PM);
                        cfg.OrderBy = "Key.MinPower";
                        //进行衰减。
                        IList<WorkOrderDecay> lstWorkOrderDecay = this.WorkOrderDecayDataEngine.Get(cfg);
                        if (lstWorkOrderDecay.Count > 0)
                        {
                            cfg.IsPaging = false;
                            cfg.Where = string.Format("Key.Code='{0}' AND IsUsed=1", lstWorkOrderDecay[0].DecayCode);
                            cfg.OrderBy = "Key";
                            IList<Decay> lstDecay = this.DecayDataEngine.Get(cfg);
                            foreach (Decay item in lstDecay)
                            {
                                //根据功率计算出衰减系数。
                                double rate = 1;
                                if (item.Type == EnumDecayType.Aim)
                                {
                                    rate = item.Value / testData.PM;
                                }
                                else
                                {
                                    rate = item.Value;
                                }
                                //根据衰减系数计算实际功率值
                                switch (item.Key.Object)
                                {
                                    case EnumPVMTestDataType.PM:
                                        testData.CoefPM = testData.PM * rate;
                                        break;
                                    case EnumPVMTestDataType.FF:
                                        testData.CoefFF = testData.FF * rate;
                                        break;
                                    case EnumPVMTestDataType.IPM:
                                        testData.CoefIPM = testData.IPM * rate;
                                        break;
                                    case EnumPVMTestDataType.ISC:
                                        testData.CoefISC = testData.ISC * rate;
                                        break;
                                    case EnumPVMTestDataType.VOC:
                                        testData.CoefVOC = testData.VOC * rate;
                                        break;
                                    case EnumPVMTestDataType.VPM:
                                        testData.CoefVPM = testData.VPM * rate;
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                        #endregion

                        #region //判断功率是否符合工单功率范围要求。
                        //获取工单规则。
                        WorkOrderRule wor = this.WorkOrderRuleDataEngine.Get(new WorkOrderRuleKey()
                        {
                            OrderNumber = lotUpdate.OrderNumber,
                            MaterialCode = lotUpdate.MaterialCode
                        });
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
                        IList<WorkOrderControlObject> lstWorkOrderControlObject = this.WorkOrderControlObjectDataEngine.Get(cfg);
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
                        IList<WorkOrderPowerset> lstWorkOrderPowerset = this.WorkOrderPowersetDataEngine.Get(cfg);
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
                            IList<WorkOrderPowersetDetail> lstWorkOrderPowersetDetail = this.WorkOrderPowersetDetailDataEngine.Get(cfg);
                            if (lstWorkOrderPowersetDetail.Count > 0)
                            {
                                testData.PowersetSubCode = lstWorkOrderPowersetDetail[0].Key.SubCode;
                            }
                        }
                        #endregion

                        sbMessage.AppendFormat("批次（{0}）符合工单（{1}:{2}）分档规则<font size='20' color='red'>({3}-{4})</font>要求。"
                                                , lotUpdate.Key
                                                , lotUpdate.OrderNumber
                                                , lotUpdate.MaterialCode
                                                , ps.PowerName
                                                , testData.PowersetSubCode);
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
                        lotUpdate.Attr4 = "批次跳档，请联系工艺与质量人员进行释放！";
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
              
                //获取工步设置是否自动进站。
                rsa = this.RouteStepAttributeDataEngine.Get(new RouteStepAttributeKey()
                {
                    RouteName = lot.RouteName,
                    RouteStepName = lot.RouteStepName,
                    AttributeName = "IsAutoTrackIn"
                });

                bool isAutoTrackIn = false;
                if (rsa != null)
                {
                    bool.TryParse(rsa.Value, out isAutoTrackIn);
                }

                //自动进站。
                if (isAutoTrackIn == true )
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

                #endregion

                #region //物料批次管理
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
                IList<RouteStepParameter> lstRouteStepParameter = this.RouteStepParameterDataEngine.Get(cfg);
                if (lstRouteStepParameter.Count > 0 && p.Paramters!=null && p.Paramters.Count>0)
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

                            IList<MaterialReceiptDetail> lstMaterialReceiptDetail = this.MaterialReceiptDetailDataEngine.Get(cfg);
                            if (lstMaterialReceiptDetail == null || lstMaterialReceiptDetail.Count == 0)
                            {
                                string message = item.ValidateFailedMessage ?? string.Empty;
                                if (string.IsNullOrEmpty(message))
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
                                });

                                if (lsmd == null
                                    || lsmd.CurrentQty == 0)
                                {
                                    continue;
                                }


                                //获取物料
                                Material m = this.MaterialDataEngine.Get(lsmd.Key.MaterialCode ?? string.Empty);
                                //获取供应商
                                Supplier s = this.SupplierDataEngine.Get(lsmd.SupplierCode ?? string.Empty);

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
                                    IList<WorkOrderBOM> lstWorkOrderBOM = this.WorkOrderBOMDataEngine.Get(cfg);
                                    if (lstWorkOrderBOM == null || lstWorkOrderBOM.Count == 0)
                                    {
                                        continue;
                                    }
                                    dicLineStoreMaterialDetail.Add(mrdItem.MaterialCode, Convert.ToDouble(lstWorkOrderBOM[0].Qty) * lot.Quantity);
                                }
                                //更新线边仓物料记录。
                                double qty = dicLineStoreMaterialDetail[mrdItem.MaterialCode];

                                LineStoreMaterialDetail lsmdUpdate = lsmd.Clone() as LineStoreMaterialDetail;
                                double leftQty = lsmdUpdate.CurrentQty - qty;
                                if (leftQty < 0)
                                {
                                    dicLineStoreMaterialDetail[mrdItem.MaterialCode] = Math.Abs(leftQty);
                                    lsmdUpdate.CurrentQty = 0;
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

                                IList<LotBOM> lstLotBom = this.LotBOMDataEngine.Get(cfg);
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
                                if (string.IsNullOrEmpty(message))
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
                            cfg = new PagingConfig()
                            {
                                IsPaging = false,
                                Where = string.Format(@"MaterialLot='{0}'
                                                        AND OrderNumber='{4}'
                                                        AND MaterialCode LIKE '{3}%'
                                                        AND CurrentQty>0
                                                        AND EXISTS(SELECT Key
                                                                   FROM MaterialLoading as p
                                                                   WHERE p.RouteOperationName='{1}'
                                                                   AND p.EquipmentCode='{2}'
                                                                   AND p.Key=self.Key.LoadingKey)"
                                                        , tp.Value
                                                        , lot.RouteStepName
                                                        , equipmentCode
                                                        , item.MaterialType
                                                        , lot.OrderNumber)
                            };
                            //获取上料记录。
                            IList<MaterialLoadingDetail> lstMaterialLoadingDetail = this.MaterialLoadingDetailDataEngine.Get(cfg);
                            if (lstMaterialLoadingDetail == null || lstMaterialLoadingDetail.Count == 0)
                            {
                                string message = item.ValidateFailedMessage ?? string.Empty;
                                if (string.IsNullOrEmpty(message))
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
                                IList<WorkOrderBOM> lstWorkOrderBOM = this.WorkOrderBOMDataEngine.Get(cfg);
                                if (lstWorkOrderBOM == null || lstWorkOrderBOM.Count == 0)
                                {
                                    continue;
                                }
                                dicMaterialLoadingDetail.Add(loadingMaterialCode, Convert.ToDouble(lstWorkOrderBOM[0].Qty) * lot.Quantity);
                            }

                            Dictionary<LotBOMKey,int> dicLotBomKey = new Dictionary<LotBOMKey,int>();
                            //遍历上料记录
                            foreach (MaterialLoadingDetail mldItem in lstMaterialLoadingDetail)
                            {
                                //更新上料记录。
                                double qty = dicMaterialLoadingDetail[loadingMaterialCode];
                                MaterialLoadingDetail mldItemUpdate = mldItem.Clone() as MaterialLoadingDetail;
                                double leftQty = mldItemUpdate.CurrentQty - qty;
                                if (leftQty < 0)
                                {
                                    dicMaterialLoadingDetail[loadingMaterialCode] = Math.Abs(leftQty);
                                    mldItemUpdate.CurrentQty = 0;
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
                                });

                                Material m = null;
                                Supplier s = null;
                                if (lsmd != null)
                                {
                                    //获取物料
                                    m = this.MaterialDataEngine.Get(lsmd.Key.MaterialCode ?? string.Empty);
                                    //获取供应商
                                    s = this.SupplierDataEngine.Get(lsmd.SupplierCode ?? string.Empty);
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

                                IList<LotBOM> lstLotBom = this.LotBOMDataEngine.Get(cfg);
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
                                
                                if(dicLotBomKey.ContainsKey(lotBomKey))
                                {
                                    int itemNo=0;
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
                                if (string.IsNullOrEmpty(message))
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

                            IList<WorkOrderBOM> lstWorkOrderBOM = this.WorkOrderBOMDataEngine.Get(cfg);
                            if (lstWorkOrderBOM == null || lstWorkOrderBOM.Count == 0)
                            {
                                result.Code = 2005;
                                string message = item.ValidateFailedMessage ?? string.Empty;
                                if (string.IsNullOrEmpty(message))
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
                    }
                }
                
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
                #endregion ForLot
            }

            ISession session = this.LotDataEngine.SessionFactory.OpenSession();
            ITransaction transaction = session.BeginTransaction();
            try
            {
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

                #region //新增批次不良数据
                foreach (LotTransactionDefect lotTransactionDefect in lstLotTransactionDefectForInsert)// = new List<LotTransactionDefect>();
                {
                    LotTransactionDefectDataEngine.Update(lotTransactionDefect, session);
                }
                #endregion

                #region //新增批次报废数据
                foreach (LotTransactionScrap lotTransactionScrap in lstLotTransactionScrapForInsert)// = new List<LotTransactionDefect>();
                {
                    LotTransactionScrapDataEngine.Update(lotTransactionScrap, session);
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

                foreach (EquipmentStateEvent equipmentStateEvent in lstEquipmentStateEventtForInsert)
                {
                    this.EquipmentStateEventDataEngine.Insert(equipmentStateEvent, session);
                }
                #endregion


                //更新IV测试数据 及 批次基本信息
                foreach (IVTestData iVTestData in lstIVTestDataForUpdate)
                {
                    this.IVTestDataDataEngine.Update(iVTestData, session);
                }

                #region//判断是否自动进站
                foreach (LotJob lotJob in lstLotJobsForUpdate)
                {
                    this.LotJobDataEngine.Update(lotJob, session);
                }

                foreach (LotJob lotJob in lstLotJobsForInsert)
                {
                    this.LotJobDataEngine.Insert(lotJob, session);
                }
                #endregion

                #region //物料批次管理
                foreach (MaterialLoadingDetail materialLoadingDetail in lstMaterialLoadingDetailForUpdate)
                {
                    this.MaterialLoadingDetailDataEngine.Update(materialLoadingDetail, session);
                }

                foreach (LineStoreMaterialDetail lineStoreMaterialDetail in lstLineStoreMaterialDetailForUpdate)
                {
                    this.LineStoreMaterialDetailDataEngine.Update(lineStoreMaterialDetail, session);
                }

                foreach (LotBOM lotBOM in lstLotBOMForInsert)
                {
                    this.LotBOMDataEngine.Insert(lotBOM, session);
                }
                #endregion

                transaction.Commit();
                session.Close();
                #endregion
            }
            catch (Exception err)
            {
                LogHelper.WriteLogError("TrackOutLot>", err);
                transaction.Rollback();
                session.Close();
                result.Code = 1000;
                result.Message += string.Format(StringResource.Error, err.Message);
                result.Detail = err.ToString();
                return result;
            }
            return result;

           
        }


        public MethodReturnResult ModifyIVDataForLot(LotIVDataParameter p)
        {
            DateTime now = DateTime.Now;
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            #region define List of DataEngine
            List<Lot> lstLotDataEngineForUpdate = new List<Lot>();
            List<LotTransaction> lstLotTransactionForInsert = new List<LotTransaction>();
            List<LotTransactionHistory> lstLotTransactionHistoryForInsert = new List<LotTransactionHistory>();

            List<IVTestData> lstIVTestDataForUpdate = new List<IVTestData>();

            List<LotJob> lstLotJobsForInsert = new List<LotJob>();
            List<LotJob> lstLotJobsForUpdate = new List<LotJob>();

            #endregion
            string lotNumber = p.LotNumber;
            Lot lot = this.LotDataEngine.Get(lotNumber);
            
            if (lot==null)
            {
                result.Code = 2000;
                result.Message = string.Format("批次（{0}）不存在，请确认。", lotNumber);
                return result;
            }

            if (lot.PackageFlag==true)
            {
                result.Code = 2001;
                result.Message = string.Format("批次（{0}）已入托({1})，不允许修改。", lotNumber,lot.PackageNo);
                return result;
            }


            string transactionKey = Guid.NewGuid().ToString();

            #region //获取 Lot信息           

            Lot lotUpdate = lot.Clone() as Lot;
            
            #region 更新IV测试数据 及 批次基本信息
            PagingConfig cfg = null;

            //需要进行分档。
            if (lotUpdate != null)
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
                    result.Message = string.Format("批次（{0}）IV测试数据不存在，请确认。", lotNumber);
                    return result;
                }
                #endregion

                IVTestData testData = lstTestData[0].Clone() as IVTestData;


                //获取工单产品设置。
                cfg.Where = string.Format(@"Key.OrderNumber='{0}'"
                                            , lot.OrderNumber);
                cfg.OrderBy = "ItemNo";
                IList<WorkOrderProduct> lstWorkOrderProduct = this.WorkOrderProductDataEngine.Get(cfg);
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

                    #region //进行衰减。
                    //获取工单衰减规则。
                    cfg.Where = string.Format(@"Key.OrderNumber='{0}' 
                                                AND Key.MaterialCode='{1}' 
                                                AND Key.MinPower<='{2}'
                                                AND Key.MaxPower>='{2}'
                                                AND IsUsed=1"
                                                , lotUpdate.OrderNumber
                                                , lotUpdate.MaterialCode
                                                , testData.PM
                                                , testData.PM);
                    cfg.OrderBy = "Key.MinPower";
                    //进行衰减。
                    IList<WorkOrderDecay> lstWorkOrderDecay = this.WorkOrderDecayDataEngine.Get(cfg);
                    if (lstWorkOrderDecay.Count > 0)
                    {
                        cfg.IsPaging = false;
                        cfg.Where = string.Format("Key.Code='{0}' AND IsUsed=1", lstWorkOrderDecay[0].DecayCode);
                        cfg.OrderBy = "Key";
                        IList<Decay> lstDecay = this.DecayDataEngine.Get(cfg);
                        foreach (Decay item in lstDecay)
                        {
                            //根据功率计算出衰减系数。
                            double rate = 1;
                            if (item.Type == EnumDecayType.Aim)
                            {
                                rate = item.Value / testData.PM;
                            }
                            else
                            {
                                rate = item.Value;
                            }
                            //根据衰减系数计算实际功率值
                            switch (item.Key.Object)
                            {
                                case EnumPVMTestDataType.PM:
                                    testData.CoefPM = testData.PM * rate;
                                    break;
                                case EnumPVMTestDataType.FF:
                                    testData.CoefFF = testData.FF * rate;
                                    break;
                                case EnumPVMTestDataType.IPM:
                                    testData.CoefIPM = testData.IPM * rate;
                                    break;
                                case EnumPVMTestDataType.ISC:
                                    testData.CoefISC = testData.ISC * rate;
                                    break;
                                case EnumPVMTestDataType.VOC:
                                    testData.CoefVOC = testData.VOC * rate;
                                    break;
                                case EnumPVMTestDataType.VPM:
                                    testData.CoefVPM = testData.VPM * rate;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    #endregion

                    #region //判断功率是否符合工单功率范围要求。
                    //获取工单规则。
                    WorkOrderRule wor = this.WorkOrderRuleDataEngine.Get(new WorkOrderRuleKey()
                    {
                        OrderNumber = lotUpdate.OrderNumber,
                        MaterialCode = lotUpdate.MaterialCode
                    });
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
                    IList<WorkOrderControlObject> lstWorkOrderControlObject = this.WorkOrderControlObjectDataEngine.Get(cfg);
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
                    IList<WorkOrderPowerset> lstWorkOrderPowerset = this.WorkOrderPowersetDataEngine.Get(cfg);
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
                        IList<WorkOrderPowersetDetail> lstWorkOrderPowersetDetail = this.WorkOrderPowersetDetailDataEngine.Get(cfg);
                        if (lstWorkOrderPowersetDetail.Count > 0)
                        {
                            testData.PowersetSubCode = lstWorkOrderPowersetDetail[0].Key.SubCode;
                        }
                    }
                    else
                    {
                        testData.PowersetSubCode = "";
                    }
                    #endregion

                    sbMessage.AppendFormat("批次（{0}）符合工单（{1}:{2}）分档规则<font size='20' color='red'>({3}-{4})</font>要求。"
                                            , lotUpdate.Key
                                            , lotUpdate.OrderNumber
                                            , lotUpdate.MaterialCode
                                            , ps.PowerName
                                            , testData.PowersetSubCode);

                    bSuccess = true;
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
                //lotUpdate.Editor = p.Creator;
                lotUpdate.EditTime = DateTime.Now;
                //保存分档档位
                lotUpdate.Attr5 = testData.PowersetCode;
                //this.LotDataEngine.Update(lot);
                //更新测试数据。
                //testData.Editor = p.Creator;
                testData.IsPrint = false;                
                testData.EditTime = DateTime.Now;
                //this.IVTestDataDataEngine.Update(testData);
                lstIVTestDataForUpdate.Add(testData);
            }
            #endregion

            #region//记录操作历史。
            LotTransaction transObj = new LotTransaction()
            {
                Key = transactionKey,
                Activity = EnumLotActivity.ModifyIVData,
                CreateTime = now,
                Creator = p.Creator,
                Description = "",
                Editor = p.Creator,
                EditTime = now,
                InQuantity = lot.Quantity,
                LotNumber = p.LotNumber,
                OperateComputer = lot.OperateComputer,
                OrderNumber = lot.OrderNumber,
                OutQuantity = lotUpdate.Quantity,
                RouteEnterpriseName = lot.RouteEnterpriseName,
                RouteName = lot.RouteName,
                RouteStepName = lot.RouteStepName,
                ShiftName = p.ShiftName,
                LocationName = lot.LocationName,
                LineCode = lot.LineCode,
                UndoFlag = false,
                UndoTransactionKey = null
            };
            lstLotTransactionForInsert.Add(transObj);
            //this.LotTransactionDataEngine.Insert(transObj, session);

            //新增批次历史记录。
            LotTransactionHistory lotHistory = new LotTransactionHistory(transactionKey, lot);
            lstLotTransactionHistoryForInsert.Add(lotHistory);
            #endregion

            lstLotDataEngineForUpdate.Add(lotUpdate);
            #endregion ForLot
            
            ISession session = this.LotDataEngine.SessionFactory.OpenSession();
            ITransaction transaction = session.BeginTransaction();
            try
            {
                #region //开始事物处理

                #region 更新批次LOT 的信息
                //更新批次基本信息
                foreach (Lot obj in lstLotDataEngineForUpdate)
                {
                    this.LotDataEngine.Update(obj, session);
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
                #endregion

                //更新IV测试数据 及 批次基本信息
                foreach (IVTestData iVTestData in lstIVTestDataForUpdate)
                {
                    this.IVTestDataDataEngine.Update(iVTestData, session);
                }
                transaction.Commit();
                session.Close();
                #endregion
            }
            catch (Exception err)
            {
                LogHelper.WriteLogError("ModifyIVDataForLot>", err);
                transaction.Rollback();
                session.Close();
                result.Code = 1000;
                result.Message += string.Format(StringResource.Error, err.Message);
                result.Detail = err.ToString();
                return result;
            }
            return result;
        }
    }
}
