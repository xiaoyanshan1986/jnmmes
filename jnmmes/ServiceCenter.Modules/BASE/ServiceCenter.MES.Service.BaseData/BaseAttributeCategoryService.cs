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
    /// 实现基础属性分类管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class BaseAttributeCategoryService : IBaseAttributeCategoryContract
    {
        /// <summary>
        /// 基础属性分类数据访问读写。
        /// </summary>
        /// <value>The BaseAttributeCategory data engine.</value>
        public IBaseAttributeCategoryDataEngine BaseAttributeCategoryDataEngine { get; set; }


        /// <summary>
        /// 添加基础属性分类。
        /// </summary>
        /// <param name="obj">基础属性分类数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(BaseAttributeCategory obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.BaseAttributeCategoryDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.BaseAttributeCategoryService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.BaseAttributeCategoryDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.BaseAttributeCategoryService_OtherError,ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 修改基础属性分类。
        /// </summary>
        /// <param name="obj">基础属性分类数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(BaseAttributeCategory obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.BaseAttributeCategoryDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.BaseAttributeCategoryService_IsNotExists,obj.Key);
                return result;
            }
            try
            {
                this.BaseAttributeCategoryDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.BaseAttributeCategoryService_OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除基础属性分类。
        /// </summary>
        /// <param name="key">基础属性分类标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.BaseAttributeCategoryDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.BaseAttributeCategoryService_IsNotExists,key);
                return result;
            }
            try
            {
                this.BaseAttributeCategoryDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.BaseAttributeCategoryService_OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取基础属性分类数据。
        /// </summary>
        /// <param name="key">基础属性分类标识符。</param>
        /// <returns><see cref="MethodReturnResult&lt;BaseAttributeCategory&gt;" />,基础属性分类数据.</returns>
        public MethodReturnResult<BaseAttributeCategory> Get(string key)
        {
            MethodReturnResult<BaseAttributeCategory> result = new MethodReturnResult<BaseAttributeCategory>();
            if (!this.BaseAttributeCategoryDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.BaseAttributeCategoryService_IsNotExists,key);
                return result;
            }
            try
            {
                result.Data = this.BaseAttributeCategoryDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.BaseAttributeCategoryService_OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取基础属性分类数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;BaseAttributeCategory&gt;" />,基础属性分类数据集合。</returns>
        public MethodReturnResult<IList<BaseAttributeCategory>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<BaseAttributeCategory>> result = new MethodReturnResult<IList<BaseAttributeCategory>>();
            try
            {
                result.Data = this.BaseAttributeCategoryDataEngine.Get(cfg);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.BaseAttributeCategoryService_OtherError, ex.Message);
            }
            return result;
        }
    }
}
