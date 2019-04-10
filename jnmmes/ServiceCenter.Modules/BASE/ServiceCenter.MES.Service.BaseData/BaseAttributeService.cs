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

/// <summary>
/// The BaseData namespace.
/// </summary>
namespace ServiceCenter.MES.Service.BaseData
{
    /// <summary>
    /// 实现基础属性管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class BaseAttributeService : IBaseAttributeContract
    {
        /// <summary>
        /// 基础属性数据访问读写。
        /// </summary>
        /// <value>The BaseAttribute data engine.</value>
        public IBaseAttributeDataEngine BaseAttributeDataEngine { get; set; }


        /// <summary>
        /// 添加基础属性。
        /// </summary>
        /// <param name="obj">基础属性数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(BaseAttribute obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.BaseAttributeDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.BaseAttributeService_IsExists,obj.Key);
                return result;
            }
            try
            {
                this.BaseAttributeDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.BaseAttributeService_OtherError,ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 修改基础属性。
        /// </summary>
        /// <param name="obj">基础属性数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(BaseAttribute obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.BaseAttributeDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.BaseAttributeService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.BaseAttributeDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.BaseAttributeService_OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除基础属性。
        /// </summary>
        /// <param name="key">基础属性标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(BaseAttributeKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.BaseAttributeDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.BaseAttributeService_IsNotExists, key);
                return result;
            }
            try
            {
                this.BaseAttributeDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.BaseAttributeService_OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取基础属性数据。
        /// </summary>
        /// <param name="key">基础属性标识符。</param>
        /// <returns><see cref="MethodReturnResult&lt;BaseAttribute&gt;" />,基础属性数据.</returns>
        public MethodReturnResult<BaseAttribute> Get(BaseAttributeKey key)
        {
            MethodReturnResult<BaseAttribute> result = new MethodReturnResult<BaseAttribute>();
            if (!this.BaseAttributeDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.BaseAttributeService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.BaseAttributeDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.BaseAttributeService_OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取基础属性数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;BaseAttribute&gt;" />,基础属性数据集合。</returns>
        public MethodReturnResult<IList<BaseAttribute>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<BaseAttribute>> result = new MethodReturnResult<IList<BaseAttribute>>();
            try
            {
                result.Data = this.BaseAttributeDataEngine.Get(cfg);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.BaseAttributeService_OtherError, ex.Message);
            }
            return result;
        }
    }
}
