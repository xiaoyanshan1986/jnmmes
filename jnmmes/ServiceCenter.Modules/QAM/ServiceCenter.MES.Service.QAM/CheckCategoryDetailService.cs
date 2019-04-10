using ServiceCenter.MES.DataAccess.Interface.QAM;
using ServiceCenter.MES.Model.QAM;
using ServiceCenter.MES.Service.Contract.QAM;
using ServiceCenter.MES.Service.QAM.Resources;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.QAM
{
    /// <summary>
    /// 实现检验参数管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class CheckCategoryDetailService : ICheckCategoryDetailContract
    {
        /// <summary>
        /// 检验参数数据访问读写。
        /// </summary>
        public ICheckCategoryDetailDataEngine CheckCategoryDetailDataEngine { get; set; }


        /// <summary>
        /// 添加检验参数。
        /// </summary>
        /// <param name="obj">检验参数数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(CheckCategoryDetail obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.CheckCategoryDetailDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.CheckCategoryDetailService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.CheckCategoryDetailDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改检验参数。
        /// </summary>
        /// <param name="obj">检验参数数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(CheckCategoryDetail obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.CheckCategoryDetailDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.CheckCategoryDetailService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.CheckCategoryDetailDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除检验参数。
        /// </summary>
        /// <param name="key">检验参数标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(CheckCategoryDetailKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.CheckCategoryDetailDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.CheckCategoryDetailService_IsNotExists, key);
                return result;
            }
            try
            {
                this.CheckCategoryDetailDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取检验参数数据。
        /// </summary>
        /// <param name="key">检验参数标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;CheckCategoryDetail&gt;" />,检验参数数据.</returns>
        public MethodReturnResult<CheckCategoryDetail> Get(CheckCategoryDetailKey key)
        {
            MethodReturnResult<CheckCategoryDetail> result = new MethodReturnResult<CheckCategoryDetail>();
            if (!this.CheckCategoryDetailDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.CheckCategoryDetailService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.CheckCategoryDetailDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取检验参数数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;CheckCategoryDetail&gt;" />,检验参数数据集合。</returns>
        public MethodReturnResult<IList<CheckCategoryDetail>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<CheckCategoryDetail>> result = new MethodReturnResult<IList<CheckCategoryDetail>>();
            try
            {
                result.Data = this.CheckCategoryDetailDataEngine.Get(cfg);
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
