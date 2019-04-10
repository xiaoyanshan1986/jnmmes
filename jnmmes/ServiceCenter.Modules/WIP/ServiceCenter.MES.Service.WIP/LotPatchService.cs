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
    /// 实现批次补料服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class LotPatchService : ILotPatchContract, ILotPatchCheck, ILotPatch
    {
        /// <summary>
        /// 操作前检查事件。
        /// </summary>
        public event Func<PatchParameter, MethodReturnResult> CheckEvent;
        /// <summary>
        /// 执行操作时事件。
        /// </summary>
        public event Func<PatchParameter, MethodReturnResult> ExecutingEvent;
        /// <summary>
        /// 操作执行完成事件。
        /// </summary>
        public event Func<PatchParameter, MethodReturnResult> ExecutedEvent;

        /// <summary>
        /// 自定义操作前检查的清单列表。
        /// </summary>
        private IList<ILotPatchCheck> CheckList { get; set; }
        /// <summary>
        /// 自定义执行中操作的清单列表。
        /// </summary>
        private IList<ILotPatch> ExecutingList { get; set; }
        /// <summary>
        /// 自定义执行后操作的清单列表。
        /// </summary>
        private IList<ILotPatch> ExecutedList { get; set; }


        /// <summary>
        /// 注册自定义检查的操作实例。
        /// </summary>
        /// <param name="obj"></param>
        public void RegisterCheckInstance(ILotPatchCheck obj)
        {
            if (this.CheckList == null)
            {
                this.CheckList = new List<ILotPatchCheck>();
            }
            this.CheckList.Add(obj);
        }
        /// <summary>
        /// 注册执行中的自定义操作实例。
        /// </summary>
        /// <param name="obj"></param>
        public void RegisterExecutingInstance(ILotPatch obj)
        {
            if (this.ExecutingList == null)
            {
                this.ExecutingList = new List<ILotPatch>();
            }
            this.ExecutingList.Add(obj);
        }

        /// <summary>
        /// 注册执行完成后的自定义操作实例。
        /// </summary>
        /// <param name="obj"></param>
        public void RegisterExecutedInstance(ILotPatch obj)
        {
            if (this.ExecutedList == null)
            {
                this.ExecutedList = new List<ILotPatch>();
            }
            this.ExecutedList.Add(obj);
        }


        /// <summary>
        /// 操作前检查。
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        protected virtual MethodReturnResult OnCheck(PatchParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            if (this.CheckEvent != null)
            {
                foreach (Func<PatchParameter, MethodReturnResult> d in this.CheckEvent.GetInvocationList())
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
                foreach (ILotPatchCheck d in this.CheckList)
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
        protected virtual MethodReturnResult OnExecuting(PatchParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            if (this.ExecutingEvent != null)
            {
                foreach (Func<PatchParameter, MethodReturnResult> d in this.ExecutingEvent.GetInvocationList())
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
                foreach (ILotPatch d in this.ExecutingList)
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
        protected virtual MethodReturnResult OnExecuted(PatchParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            if (this.ExecutedEvent != null)
            {
                foreach (Func<PatchParameter, MethodReturnResult> d in this.ExecutedEvent.GetInvocationList())
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
                foreach (ILotPatch d in this.ExecutedList)
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
        public LotPatchService()
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
        /// 批次BOM数据访问类。
        /// </summary>
        public ILotBOMDataEngine LotBOMDataEngine
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
        /// 批次补料数据访问类。
        /// </summary>
        public ILotTransactionPatchDataEngine LotTransactionPatchDataEngine
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
        /// 工单领料明细数据访问类。
        /// </summary>
        public IMaterialReceiptDetailDataEngine MaterialReceiptDetailDataEngine
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
        /// 批次补料操作。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>
        /// 代码表示：0：成功，其他失败。
        /// </returns>
        MethodReturnResult ILotPatchContract.Patch(PatchParameter p)
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
        MethodReturnResult ILotPatchCheck.Check(PatchParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };
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
            }
            return result;
        }
        /// <summary>
        /// 执行操作。
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        MethodReturnResult ILotPatch.Execute(PatchParameter p)
        {
            DateTime now = DateTime.Now;
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };
            
            double totalQuantity = 0;

            p.TransactionKeys = new Dictionary<string, string>();
            //循环批次。
            foreach(string lotNumber in p.LotNumbers)
            {
                Lot lot = this.LotDataEngine.Get(lotNumber);

                //获取原材料库存记录。
                LineStoreMaterialDetail lsmd = this.LineStoreMaterialDetailDataEngine.Get(new LineStoreMaterialDetailKey()
                {
                    LineStoreName = p.LineStoreName,
                    OrderNumber=lot.OrderNumber,
                    MaterialCode = p.RawMaterialCode,
                    MaterialLot = p.RawMaterialLot
                });
                if (lsmd == null)
                {
                    result.Code = 1003;
                    result.Message = string.Format("线边仓({2})物料（{0}:{1}）不存在。"
                                                   , p.RawMaterialCode
                                                   , p.RawMaterialLot
                                                   , p.LineStoreName);
                    return result;
                }
                //检查工单是否领料指定物料批次号到指定线边仓。
                PagingConfig cfg = new PagingConfig()
                {
                    PageSize = 1,
                    PageNo = 0,
                    Where = string.Format(@"MaterialCode='{0}' 
                                         AND MaterialLot='{1}' 
                                         AND LineStoreName='{2}'
                                         AND EXISTS (SELECT p.Key FROM MaterialReceipt as p 
                                                    WHERE p.Key=self.Key.ReceiptNo AND p.OrderNumber='{3}' )"
                                         , p.RawMaterialCode
                                         , p.RawMaterialLot
                                         , p.LineStoreName
                                         , lot.OrderNumber)
                };
                IList<MaterialReceiptDetail> lstReceipt = this.MaterialReceiptDetailDataEngine.Get(cfg);
                if (lstReceipt.Count == 0)
                {
                    result.Code = 2003;
                    result.Message = string.Format("批次（{0}）对应工单（{1}）在线边仓（{4}）无（{2}:{3}）领料记录。"
                                                   , lotNumber
                                                   , lot.OrderNumber
                                                   , p.RawMaterialCode
                                                   , p.RawMaterialLot
                                                   , p.LineStoreName);
                    return result;
                }
                //生成操作事务主键。
                string transaciontKey = Guid.NewGuid().ToString();
                p.TransactionKeys.Add(lotNumber, transaciontKey);

                //更新批次记录。
                Lot lotUpdate = lot.Clone() as Lot;
                lotUpdate.OperateComputer = p.OperateComputer;
                lotUpdate.Editor = p.Creator;
                lotUpdate.EditTime = now;
                this.LotDataEngine.Update(lotUpdate);
                
                #region//记录操作历史。
                LotTransaction transObj = new LotTransaction()
                {
                    Key = transaciontKey,
                    Activity = EnumLotActivity.Patch,
                    CreateTime = now,
                    Creator = p.Creator,
                    Description = p.Remark,
                    Editor = p.Creator,
                    EditTime = now,
                    InQuantity = lot.Quantity,
                    LotNumber = lotNumber,
                    LocationName=lot.LocationName,
                    LineCode=p.LineCode,
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
                LotTransactionHistory lotHistory = new LotTransactionHistory(transaciontKey, lot);
                this.LotTransactionHistoryDataEngine.Insert(lotHistory);
                #endregion


                double lotTotalQuantity = 0;

                #region //新增批次补料数据
                if (p.ReasonCodes != null && p.ReasonCodes.ContainsKey(lotNumber))
                {
                    foreach (PatchReasonCodeParameter rcp in p.ReasonCodes[lotNumber])
                    {
                        LotTransactionPatch lotPatch = new LotTransactionPatch()
                        {
                            Key = new LotTransactionPatchKey()
                            {
                                TransactionKey = transaciontKey,
                                ReasonCodeCategoryName = rcp.ReasonCodeCategoryName,
                                ReasonCodeName = rcp.ReasonCodeName
                            },
                            LineCode=p.LineCode,
                            LineStoreName=p.LineStoreName,
                            MaterialCode=p.RawMaterialCode,
                            MaterialLot=p.RawMaterialLot,
                            Quantity = rcp.Quantity,
                            ResponsiblePerson=rcp.ResponsiblePerson,
                            RouteOperationName=rcp.RouteOperationName,
                            Description = rcp.Description,
                            Editor = p.Creator,
                            EditTime = now
                        };
                        lotTotalQuantity += rcp.Quantity;
                        totalQuantity += rcp.Quantity;
                        if (lsmd.CurrentQty < totalQuantity)
                        {
                            result.Code = 1004;
                            result.Message = string.Format("物料数量（{0}）不足。"
                                                           , lsmd.CurrentQty);
                            return result;
                        }
                        this.LotTransactionPatchDataEngine.Insert(lotPatch);
                    }
                }
                #endregion

                #region//新增批次BOM
                cfg = new PagingConfig()
                {
                    PageSize=1,
                    PageNo=0,
                    Where = string.Format("Key.LotNumber='{0}'"
                                           ,lotNumber
                                           ,p.RawMaterialLot),
                    OrderBy="Key.ItemNo DESC"
                };
                int itemNo=2;
                IList<LotBOM> lst = this.LotBOMDataEngine.Get(cfg);
                if(lst.Count>0)
                {
                    itemNo = lst[0].Key.ItemNo + 1;
                }

                LotBOM lotbomObj = new LotBOM()
                {
                    Key = new LotBOMKey()
                    {
                        LotNumber = lotNumber,
                        MaterialLot = p.RawMaterialLot,
                        ItemNo = itemNo
                    },
                    TransactionKey=transaciontKey,
                    LineStoreName = p.LineStoreName,
                    Qty = lotTotalQuantity,
                    MaterialCode = p.RawMaterialCode,
                    RouteEnterpriseName = lot.RouteEnterpriseName,
                    RouteName = lot.RouteName,
                    RouteStepName = lot.RouteStepName,
                    EquipmentCode = lot.EquipmentCode,
                    LineCode = lot.LineCode,
                    MaterialFrom = EnumMaterialFrom.LineStore,
                    Creator = p.Creator,
                    CreateTime = now,
                    Editor = p.Creator,
                    EditTime = now
                };
                this.LotBOMDataEngine.Insert(lotbomObj);
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


                //更新原材料。
                LineStoreMaterialDetail lsmdUpdate = lsmd.Clone() as LineStoreMaterialDetail;
                lsmdUpdate.CurrentQty -= totalQuantity;
                lsmdUpdate.Editor = p.Creator;
                lsmdUpdate.EditTime = now;
                this.LineStoreMaterialDetailDataEngine.Update(lsmdUpdate);
            }
            return result;
        }
        

    }
}
