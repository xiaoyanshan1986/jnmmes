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
using ServiceCenter.Common;
using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace ServiceCenter.MES.Service.WIP
{

    /// <summary>
    /// 实现批次不良服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class LotDefectService : ILotDefectContract, ILotDefectCheck, ILotDefect
    {
        protected Database _db;

        public ISessionFactory SessionFactory
        {
            get;
            set;
        }
        public LotDefectService(ISessionFactory sf)
        {
            this._db = DatabaseFactory.CreateDatabase();
            this.SessionFactory = sf;
            this.RegisterCheckInstance(this);
            this.RegisterExecutedInstance(this);
        }

        /// <summary>
        /// 操作前检查事件。
        /// </summary>
        public event Func<DefectParameter, MethodReturnResult> CheckEvent;
        /// <summary>
        /// 执行操作时事件。
        /// </summary>
        public event Func<DefectParameter, MethodReturnResult> ExecutingEvent;
        /// <summary>
        /// 操作执行完成事件。
        /// </summary>
        public event Func<DefectParameter, MethodReturnResult> ExecutedEvent;

        /// <summary>
        /// 自定义操作前检查的清单列表。
        /// </summary>
        private IList<ILotDefectCheck> CheckList { get; set; }
        /// <summary>
        /// 自定义执行中操作的清单列表。
        /// </summary>
        private IList<ILotDefect> ExecutingList { get; set; }
        /// <summary>
        /// 自定义执行后操作的清单列表。
        /// </summary>
        private IList<ILotDefect> ExecutedList { get; set; }


        /// <summary>
        /// 注册自定义检查的操作实例。
        /// </summary>
        /// <param name="obj"></param>
        public void RegisterCheckInstance(ILotDefectCheck obj)
        {
            if (this.CheckList == null)
            {
                this.CheckList = new List<ILotDefectCheck>();
            }
            this.CheckList.Add(obj);
        }
        /// <summary>
        /// 注册执行中的自定义操作实例。
        /// </summary>
        /// <param name="obj"></param>
        public void RegisterExecutingInstance(ILotDefect obj)
        {
            if (this.ExecutingList == null)
            {
                this.ExecutingList = new List<ILotDefect>();
            }
            this.ExecutingList.Add(obj);
        }

        /// <summary>
        /// 注册执行完成后的自定义操作实例。
        /// </summary>
        /// <param name="obj"></param>
        public void RegisterExecutedInstance(ILotDefect obj)
        {
            if (this.ExecutedList == null)
            {
                this.ExecutedList = new List<ILotDefect>();
            }
            this.ExecutedList.Add(obj);
        }


        /// <summary>
        /// 操作前检查。
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        protected virtual MethodReturnResult OnCheck(DefectParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            if (this.CheckEvent != null)
            {
                foreach (Func<DefectParameter, MethodReturnResult> d in this.CheckEvent.GetInvocationList())
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
                foreach (ILotDefectCheck d in this.CheckList)
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
        protected virtual MethodReturnResult OnExecuting(DefectParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            if (this.ExecutingEvent != null)
            {
                foreach (Func<DefectParameter, MethodReturnResult> d in this.ExecutingEvent.GetInvocationList())
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
                foreach (ILotDefect d in this.ExecutingList)
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
        protected virtual MethodReturnResult OnExecuted(DefectParameter p)
        {
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };

            if (this.ExecutedEvent != null)
            {
                foreach (Func<DefectParameter, MethodReturnResult> d in this.ExecutedEvent.GetInvocationList())
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
                foreach (ILotDefect d in this.ExecutedList)
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
        public LotDefectService()
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
        /// 批次不良数据访问类。
        /// </summary>
        public ILotTransactionDefectDataEngine LotTransactionDefectDataEngine
        {
            get;
            set;
        }

        public ILotTransactionDefectPosDataEngine LotTransactionDefectPosDataEngine
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
        /// 批次不良操作。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>
        /// 代码表示：0：成功，其他失败。
        /// </returns>
        MethodReturnResult ILotDefectContract.Defect(DefectParameter p)
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
               
                result = this.OnExecuted(p);
                if (result.Code > 0)
                {
                    return result;
                }                
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = string.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }
        MethodReturnResult<DataSet> ILotDefectContract.GetXY(string key)
        {
            MethodReturnResult<DataSet> result = new MethodReturnResult<DataSet>();

            try
            {
                using (DbConnection con = this._db.CreateConnection())
                {
                    DbCommand cmd = con.CreateCommand();
                    cmd.CommandText = string.Format(@"select  POS_X+'*'+POS_Y  from [dbo].[WIP_TRANSACTION_DEFECT_POS] where TRANSACTION_KEY='{0}'", key);
                    result.Data = _db.ExecuteDataSet(cmd);
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


        /// <summary>
        /// 操作前检查。
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        MethodReturnResult ILotDefectCheck.Check(DefectParameter p)
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
        MethodReturnResult ILotDefect.Execute(DefectParameter p)
        {
            DateTime now = DateTime.Now;
            MethodReturnResult result = new MethodReturnResult()
            {
                Code = 0
            };
            p.TransactionKeys = new Dictionary<string, string>();

            List<Lot> lstLotDataEngineForUpdate = new List<Lot>();
            List<LotTransaction> lstLotTransactionForInsert = new List<LotTransaction>();
            List<LotTransactionHistory> lstLotTransactionHistoryForInsert = new List<LotTransactionHistory>();
            List<LotTransactionParameter> lstLotTransactionParameterDataEngineForInsert = new List<LotTransactionParameter>();
            List<LotTransactionDefect> lstLotTransactionDefectForInsert = new List<LotTransactionDefect>();
            List<LotTransactionDefectPos> lstLotTransactionDefectPosForInsert = new List<LotTransactionDefectPos>();
    
            //循环批次。
            foreach (string lotNumber in p.LotNumbers)
            {
                Lot lot = this.LotDataEngine.Get(lotNumber);
                //生成操作事务主键。
                string transaciontKey = Guid.NewGuid().ToString();
                p.TransactionKeys.Add(lotNumber, transaciontKey);

                //更新批次记录。
                Lot lotUpdate = lot.Clone() as Lot;
                lotUpdate.Grade = p.Grade;
                lotUpdate.OperateComputer = p.OperateComputer;
                lotUpdate.Editor = p.Creator;
                lotUpdate.EditTime = now;
                //this.LotDataEngine.Update(lotUpdate);
                lstLotDataEngineForUpdate.Add(lotUpdate);

                #region//记录操作历史。
                LotTransaction transObj = new LotTransaction()
                {
                    Key = transaciontKey,
                    Activity = EnumLotActivity.Defect,
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
                    RouteEnterpriseName = lot.RouteEnterpriseName,
                    RouteName = lot.RouteName,
                    RouteStepName = lot.RouteStepName,
                    ShiftName = p.ShiftName,
                    UndoFlag = false,
                    UndoTransactionKey = null,
                    Grade=p.Grade,
                    Color = lot.Color,
                    Attr1 = lot.Attr1,
                    Attr2 = lot.Attr2,
                    Attr3 = lot.Attr3,
                    Attr4 = lot.Attr4,
                    Attr5 = lot.Attr5,
                    OriginalOrderNumber = lot.OriginalOrderNumber
                };
                //this.LotTransactionDataEngine.Insert(transObj);
                lstLotTransactionForInsert.Add(transObj);

                //新增批次历史记录。
                LotTransactionHistory lotHistory = new LotTransactionHistory(transaciontKey, lot);
                lotHistory.Grade = p.Grade;
                lstLotTransactionHistoryForInsert.Add(lotHistory);
                //this.LotTransactionHistoryDataEngine.Insert(lotHistory);

                #endregion

                #region //新增批次不良数据
                if (p.ReasonCodes != null && p.ReasonCodes.ContainsKey(lotNumber))
                {
                    foreach (DefectReasonCodeParameter rcp in p.ReasonCodes[lotNumber])
                    {
                        LotTransactionDefect lotDefect = new LotTransactionDefect()
                        {
                            Key = new LotTransactionDefectKey()
                            {
                                TransactionKey = transaciontKey,
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
                        //你懂得
                        foreach (var data in rcp.ListDefectPOSParameter)
                        {
                            LotTransactionDefectPos lotDefectPos = new LotTransactionDefectPos()
                            {
                                Key = new LotTransactionDefectPosKey()
                                {
                                    TransactionKey = transaciontKey,
                                    PosX = data.POS_X,
                                    PosY = data.POS_Y
                                },
                                Editor = p.Creator,
                                EditTime = now,
                            };
                            //this.LotTransactionDefectPosDataEngine.Insert(lotDefectPos);
                            lstLotTransactionDefectPosForInsert.Add(lotDefectPos);
                        }

                    }
                }
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
                                TransactionKey = transaciontKey,
                                ParameterName = tp.Name,
                                ItemNo = tp.Index,
                            },
                            ParameterValue = tp.Value,
                            Editor = p.Creator,
                            EditTime = now
                        };
                        //this.LotTransactionParameterDataEngine.Insert(lotParamObj);
                        lstLotTransactionParameterDataEngineForInsert.Add(lotParamObj);
                    }
                }
                #endregion
            }

            ISession db = this.LotDataEngine.SessionFactory.OpenSession();
            ITransaction transaction = db.BeginTransaction();
            try
            {
                #region //开始事物处理

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
                    this.LotTransactionParameterDataEngine.Insert(lotTransactionParameter, db);
                }

                #region //新增批次不良数据
                foreach (LotTransactionDefect lotTransactionDefect in lstLotTransactionDefectForInsert)
                {
                    LotTransactionDefectDataEngine.Insert(lotTransactionDefect, db);
                }

                foreach (LotTransactionDefectPos obj in lstLotTransactionDefectPosForInsert)
                {
                    LotTransactionDefectPosDataEngine.Insert(obj, db);
                }
                #endregion

                transaction.Commit();
                db.Close();
                #endregion
            }
            catch (Exception err)
            {
                LogHelper.WriteLogError("LotDefect.Execute>", err);
                transaction.Rollback();
                db.Close();
                result.Code = 1000;
                result.Message += string.Format(StringResource.Error, err.Message);
                result.Detail = err.ToString();
                return result;
            }
            return result;
        }


    }
}
