using ServiceCenter.MES.DataAccess.Interface.BaseData;
using ServiceCenter.MES.Model.BaseData;
using ServiceCenter.MES.Service.Contract.BaseData;
using ServiceCenter.MES.Service.BaseData.Resources;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using NHibernate;

/// <summary>
/// The BaseData namespace.
/// </summary>
namespace ServiceCenter.MES.Service.BaseData
{
    /// <summary>
    /// 实现基础属性管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class BaseAttributeValueService : IBaseAttributeValueContract
    {
        /// <summary>
        /// 基础属性数据访问读写。
        /// </summary>
        /// <value>The BaseAttributeValue data engine.</value>
        public IBaseAttributeValueDataEngine BaseAttributeValueDataEngine { get; set; }


        /// <summary>
        /// 添加基础属性。
        /// </summary>
        /// <param name="obj">基础属性数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(BaseAttributeValue obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.BaseAttributeValueDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.BaseAttributeValueService_IsExists,obj.Key);
                return result;
            }
            try
            {
                this.BaseAttributeValueDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.BaseAttributeValueService_OtherError,ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 修改基础属性。
        /// </summary>
        /// <param name="obj">基础属性数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(BaseAttributeValue obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.BaseAttributeValueDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.BaseAttributeValueService_IsNotExists,obj.Key);
                return result;
            }
            try
            {
                this.BaseAttributeValueDataEngine.Modify(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.BaseAttributeValueService_OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除基础属性。
        /// </summary>
        /// <param name="key">基础属性标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(BaseAttributeValueKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.BaseAttributeValueDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.BaseAttributeValueService_IsNotExists, key);
                return result;
            }
            try
            {
                this.BaseAttributeValueDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.BaseAttributeValueService_OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取基础属性数据。
        /// </summary>
        /// <param name="key">基础属性标识符。</param>
        /// <returns><see cref="MethodReturnResult&lt;BaseAttributeValue&gt;" />,基础属性数据.</returns>
        public MethodReturnResult<BaseAttributeValue> Get(BaseAttributeValueKey key)
        {
            MethodReturnResult<BaseAttributeValue> result = new MethodReturnResult<BaseAttributeValue>();
            if (!this.BaseAttributeValueDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.BaseAttributeValueService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.BaseAttributeValueDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.BaseAttributeValueService_OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取基础属性数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;BaseAttributeValue&gt;" />,基础属性数据集合。</returns>
        public MethodReturnResult<IList<BaseAttributeValue>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<BaseAttributeValue>> result = new MethodReturnResult<IList<BaseAttributeValue>>();
            try
            {
                result.Data = this.BaseAttributeValueDataEngine.Get(cfg);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.BaseAttributeValueService_OtherError, ex.Message);
            }
            return result;
        }

        public MethodReturnResult Add(IList<BaseAttributeValue> lst)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                if (lst != null)
                {
                    //using (TransactionScope scope = new TransactionScope())
                    ISession session = this.BaseAttributeValueDataEngine.SessionFactory.OpenSession();
                    ITransaction transaction = session.BeginTransaction();
                    {
                        foreach (BaseAttributeValue obj in lst)
                        {
                            this.BaseAttributeValueDataEngine.Insert(obj,session);
                        }
                        //scope.Complete();
                        transaction.Commit();
                        session.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.BaseAttributeValueService_OtherError, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        public MethodReturnResult Delete(string categoryName, int itemOrder)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                string condition = string.Format("Key.CategoryName='{0}' AND Key.ItemOrder='{1}'",
                                                  categoryName, itemOrder);
                this.BaseAttributeValueDataEngine.DeleteByCondition(condition);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.BaseAttributeValueService_OtherError, ex.Message);
            }
            return result;
        }

        public MethodReturnResult Modify(IList<BaseAttributeValue> lst)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                if (lst != null)
                {
                    //using (TransactionScope scope = new TransactionScope())
                    ISession session = this.BaseAttributeValueDataEngine.SessionFactory.OpenSession();
                    ITransaction transaction = session.BeginTransaction();
                    {
                        foreach (BaseAttributeValue obj in lst)
                        {
                            this.BaseAttributeValueDataEngine.Modify(obj,session);
                        }
                        //scope.Complete();
                        transaction.Commit();
                        session.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.BaseAttributeValueService_OtherError, ex.Message);
            }
            return result;
        }


    }
}
