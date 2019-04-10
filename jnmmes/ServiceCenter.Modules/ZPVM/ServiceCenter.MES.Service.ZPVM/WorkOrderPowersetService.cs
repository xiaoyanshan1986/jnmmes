using NHibernate;
using ServiceCenter.MES.DataAccess.Interface.ZPVM;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.MES.Service.Contract.ZPVM;
using ServiceCenter.MES.Service.ZPVM.Resources;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace ServiceCenter.MES.Service.ZPVM
{
    /// <summary>
    /// 实现工单分档设置数据管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class WorkOrderPowersetService : IWorkOrderPowersetContract
    {
        /// <summary>
        /// 工单分档设置数据数据访问读写。
        /// </summary>
        public IWorkOrderPowersetDataEngine WorkOrderPowersetDataEngine { get; set; }
        /// <summary>
        /// 工单子分档设置数据数据访问读写。
        /// </summary>
        public IWorkOrderPowersetDetailDataEngine WorkOrderPowersetDetailDataEngine { get; set; }

        /// <summary>
        /// 分档数据数据访问读写。
        /// </summary>
        public IPowersetDataEngine PowersetDataEngine { get; set; }
        /// <summary>
        /// 子分档数据数据访问读写。
        /// </summary>
        public IPowersetDetailDataEngine PowersetDetailDataEngine { get; set; }

        /// <summary>
        /// 添加工单分档设置数据。
        /// </summary>
        /// <param name="obj">工单分档设置数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(WorkOrderPowerset obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.WorkOrderPowersetDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.WorkOrderPowersetService_IsExists, obj.Key);
                return result;
            }
            try
            {
                
                //判断分档是否存在。
                PowersetKey key = new PowersetKey()
                {
                    Code = obj.Key.Code,
                    ItemNo = obj.Key.ItemNo
                };
                Powerset powerset = this.PowersetDataEngine.Get(key);
                if (powerset == null)
                {
                    result.Code = 1001;
                    result.Message = String.Format(StringResource.PowersetService_IsNotExists, obj.Key);
                    return result;
                }

                //using(TransactionScope ts=new TransactionScope())
                ISession session = this.PowersetDataEngine.SessionFactory.OpenSession();
                ITransaction transaction = session.BeginTransaction();
                {
                    //新增工单分档。
                    this.WorkOrderPowersetDataEngine.Insert(obj,session);

                    //新增工单子分档
                    if(obj.SubWay!=EnumPowersetSubWay.None)
                    {
                        PagingConfig cfg = new PagingConfig()
                        {
                            IsPaging = false,
                            Where = string.Format("Key.Code='{0}' AND Key.ItemNo='{1}' AND IsUsed=1",obj.Key.Code,obj.Key.ItemNo)
                        };

                        IList<PowersetDetail> lst = this.PowersetDetailDataEngine.Get(cfg,session);

                        foreach(PowersetDetail item in lst)
                        {
                            WorkOrderPowersetDetail wpd = new WorkOrderPowersetDetail()
                            {
                                CreateTime=obj.CreateTime,
                                Creator=obj.Creator,
                                Editor=obj.Editor,
                                EditTime=obj.EditTime,
                                IsUsed=true,
                                Key = new WorkOrderPowersetDetailKey()
                                {
                                    OrderNumber=obj.Key.OrderNumber,
                                    MaterialCode=obj.Key.MaterialCode,
                                    Code=obj.Key.Code,
                                    ItemNo=obj.Key.ItemNo,
                                    SubCode=item.Key.SubCode
                                },
                                MaxValue=item.MaxValue,
                                MinValue=item.MinValue,
                                SubName=item.SubName,
                                Picture = item.Picture
                            };
                            this.WorkOrderPowersetDetailDataEngine.Modify(wpd,session);
                        }
                    }
                    //ts.Complete();
                    transaction.Commit();
                    session.Close();
                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error,ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }


        /// <summary>
        /// 修改工单分档设置数据。
        /// </summary>
        /// <param name="obj">工单分档设置数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(WorkOrderPowerset obj)
        {
            MethodReturnResult result = new MethodReturnResult();

            if (!this.WorkOrderPowersetDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.WorkOrderPowersetService_IsNotExists, obj.Key);
                return result;
            }

            try
            {
                //using (TransactionScope ts = new TransactionScope())
                ISession session = this.PowersetDataEngine.SessionFactory.OpenSession();
                ITransaction transaction = session.BeginTransaction();
                {
                    this.WorkOrderPowersetDataEngine.Update(obj,session);

                    if (obj.SubWay == EnumPowersetSubWay.None)
                    {
                        string condition = string.Format("Key.Code='{0}' AND Key.ItemNo='{1}' AND Key.OrderNumber='{2}' AND Key.MaterialCode='{3}'"
                                                         , obj.Key.Code
                                                         , obj.Key.ItemNo
                                                         , obj.Key.OrderNumber
                                                         , obj.Key.MaterialCode);
                        this.WorkOrderPowersetDetailDataEngine.DeleteByCondition(condition,session);
                    }
                    //ts.Complete();
                    transaction.Commit();
                    session.Close();
                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }
        /// <summary>
        /// 删除工单分档设置数据。
        /// </summary>
        /// <param name="key">工单分档设置数据标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(WorkOrderPowersetKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.WorkOrderPowersetDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.WorkOrderPowersetService_IsNotExists, key);
                return result;
            }
            try
            {
                //using (TransactionScope ts = new TransactionScope())
                ISession session = this.PowersetDataEngine.SessionFactory.OpenSession();
                ITransaction transaction = session.BeginTransaction();
                {
                    this.WorkOrderPowersetDataEngine.Delete(key,session);

                    string condition = string.Format("Key.Code='{0}' AND Key.ItemNo='{1}' AND Key.OrderNumber='{2}' AND Key.MaterialCode='{3}'"
                                                        , key.Code
                                                        , key.ItemNo
                                                        , key.OrderNumber
                                                        , key.MaterialCode);
                    this.WorkOrderPowersetDetailDataEngine.DeleteByCondition(condition,session);
                    //ts.Complete();
                    transaction.Commit();
                    session.Close();
                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 获取工单分档设置数据数据。
        /// </summary>
        /// <param name="key">工单分档设置数据标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;WorkOrderPowerset&gt;" />,工单分档设置数据数据.</returns>
        public MethodReturnResult<WorkOrderPowerset> Get(WorkOrderPowersetKey key)
        {
            MethodReturnResult<WorkOrderPowerset> result = new MethodReturnResult<WorkOrderPowerset>();
            if (!this.WorkOrderPowersetDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.WorkOrderPowersetService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.WorkOrderPowersetDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 获取工单分档设置数据数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;WorkOrderPowerset&gt;" />,工单分档设置数据数据集合。</returns>
        public MethodReturnResult<IList<WorkOrderPowerset>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<WorkOrderPowerset>> result = new MethodReturnResult<IList<WorkOrderPowerset>>();
            try
            {
                result.Data = this.WorkOrderPowersetDataEngine.Get(cfg);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }
    }
}
