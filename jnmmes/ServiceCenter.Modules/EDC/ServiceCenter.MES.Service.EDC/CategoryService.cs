using ServiceCenter.MES.DataAccess.Interface.EDC;
using ServiceCenter.MES.Model.EDC;
using ServiceCenter.MES.Service.Contract.EDC;
using ServiceCenter.MES.Service.EDC.Resources;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.EDC
{
    /// <summary>
    /// 实现采集参数组管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class CategoryService : ICategoryContract
    {
        /// <summary>
        /// 采集参数组数据访问读写。
        /// </summary>
        public ICategoryDataEngine CategoryDataEngine { get; set; }


        /// <summary>
        /// 添加采集参数组。
        /// </summary>
        /// <param name="obj">采集参数组数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(Category obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.CategoryDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.CategoryService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.CategoryDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改采集参数组。
        /// </summary>
        /// <param name="obj">采集参数组数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(Category obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.CategoryDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.CategoryService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.CategoryDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除采集参数组。
        /// </summary>
        /// <param name="key">采集参数组标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.CategoryDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.CategoryService_IsNotExists, key);
                return result;
            }
            try
            {
                this.CategoryDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取采集参数组数据。
        /// </summary>
        /// <param name="key">采集参数组标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;Category&gt;" />,采集参数组数据.</returns>
        public MethodReturnResult<Category> Get(string key)
        {
            MethodReturnResult<Category> result = new MethodReturnResult<Category>();
            if (!this.CategoryDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.CategoryService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.CategoryDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取采集参数组数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;Category&gt;" />,采集参数组数据集合。</returns>
        public MethodReturnResult<IList<Category>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<Category>> result = new MethodReturnResult<IList<Category>>();
            try
            {
                result.Data = this.CategoryDataEngine.Get(cfg);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
    }
}
