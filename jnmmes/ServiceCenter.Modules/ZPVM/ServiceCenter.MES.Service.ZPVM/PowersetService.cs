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
    /// 实现分档数据管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class PowersetService : IPowersetContract
    {
        /// <summary>
        /// 分档数据数据访问读写。
        /// </summary>
        public IPowersetDataEngine PowersetDataEngine { get; set; }
        /// <summary>
        /// 子分档数据数据访问读写。
        /// </summary>
        public IPowersetDetailDataEngine PowersetDetailDataEngine { get; set; }

        /// <summary>
        /// 添加分档数据。
        /// </summary>
        /// <param name="obj">分档数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(Powerset obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.PowersetDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.PowersetService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.PowersetDataEngine.Insert(obj);
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
        /// 修改分档数据。
        /// </summary>
        /// <param name="obj">分档数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(Powerset obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.PowersetDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.PowersetService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                //using(TransactionScope ts=new TransactionScope())
                ISession session = this.PowersetDataEngine.SessionFactory.OpenSession();
                ITransaction transaction = session.BeginTransaction();
                {
                    if (obj.SubWay == EnumPowersetSubWay.None)
                    {
                        string condition = string.Format("Key.Code='{0}' AND Key.ItemNo='{1}'", obj.Key.Code, obj.Key.ItemNo);
                        this.PowersetDetailDataEngine.DeleteByCondition(condition,session);
                    }

                    this.PowersetDataEngine.Update(obj,session);
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
        /// 删除分档数据。
        /// </summary>
        /// <param name="key">分档数据标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(PowersetKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.PowersetDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.PowersetService_IsNotExists, key);
                return result;
            }
            try
            {
                //using (TransactionScope ts = new TransactionScope())
                ISession session = this.PowersetDataEngine.SessionFactory.OpenSession();
                ITransaction transaction = session.BeginTransaction();
                {
                    this.PowersetDataEngine.Delete(key,session);
                    string condition = string.Format("Key.Code='{0}' AND Key.ItemNo='{1}'", key.Code, key.ItemNo);
                    this.PowersetDetailDataEngine.DeleteByCondition(condition,session);
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
        /// 获取分档数据数据。
        /// </summary>
        /// <param name="key">分档数据标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;Powerset&gt;" />,分档数据数据.</returns>
        public MethodReturnResult<Powerset> Get(PowersetKey key)
        {
            MethodReturnResult<Powerset> result = new MethodReturnResult<Powerset>();
            if (!this.PowersetDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.PowersetService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.PowersetDataEngine.Get(key);
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
        /// 获取分档数据数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;Powerset&gt;" />,分档数据数据集合。</returns>
        public MethodReturnResult<IList<Powerset>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<Powerset>> result = new MethodReturnResult<IList<Powerset>>();
            try
            {
                result.Data = this.PowersetDataEngine.Get(cfg);
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
