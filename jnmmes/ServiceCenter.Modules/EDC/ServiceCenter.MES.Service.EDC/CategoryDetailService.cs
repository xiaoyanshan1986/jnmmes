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
    /// 实现采集参数管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class CategoryDetailService : ICategoryDetailContract
    {
        /// <summary>
        /// 采集参数数据访问读写。
        /// </summary>
        public ICategoryDetailDataEngine CategoryDetailDataEngine { get; set; }


        /// <summary>
        /// 添加采集参数。
        /// </summary>
        /// <param name="obj">采集参数数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(CategoryDetail obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.CategoryDetailDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.CategoryDetailService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.CategoryDetailDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改采集参数。
        /// </summary>
        /// <param name="obj">采集参数数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(CategoryDetail obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.CategoryDetailDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.CategoryDetailService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.CategoryDetailDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除采集参数。
        /// </summary>
        /// <param name="key">采集参数标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(CategoryDetailKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.CategoryDetailDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.CategoryDetailService_IsNotExists, key);
                return result;
            }
            try
            {
                this.CategoryDetailDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取采集参数数据。
        /// </summary>
        /// <param name="key">采集参数标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;CategoryDetail&gt;" />,采集参数数据.</returns>
        public MethodReturnResult<CategoryDetail> Get(CategoryDetailKey key)
        {
            MethodReturnResult<CategoryDetail> result = new MethodReturnResult<CategoryDetail>();
            if (!this.CategoryDetailDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.CategoryDetailService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.CategoryDetailDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取采集参数数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;CategoryDetail&gt;" />,采集参数数据集合。</returns>
        public MethodReturnResult<IList<CategoryDetail>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<CategoryDetail>> result = new MethodReturnResult<IList<CategoryDetail>>();
            try
            {
                result.Data = this.CategoryDetailDataEngine.Get(cfg);
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
