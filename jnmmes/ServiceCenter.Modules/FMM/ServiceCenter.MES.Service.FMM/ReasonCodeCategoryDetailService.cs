using ServiceCenter.MES.DataAccess.Interface.FMM;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Service.Contract.FMM;
using ServiceCenter.MES.Service.FMM.Resources;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.FMM
{
    /// <summary>
    /// 实现代码分组明细管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class ReasonCodeCategoryDetailService : IReasonCodeCategoryDetailContract
    {
        /// <summary>
        /// 代码分组明细数据访问读写。
        /// </summary>
        public IReasonCodeCategoryDetailDataEngine ReasonCodeCategoryDetailDataEngine { get; set; }


        /// <summary>
        /// 添加代码分组明细。
        /// </summary>
        /// <param name="obj">代码分组明细数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(ReasonCodeCategoryDetail obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.ReasonCodeCategoryDetailDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.ReasonCodeCategoryDetailService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.ReasonCodeCategoryDetailDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改代码分组明细。
        /// </summary>
        /// <param name="obj">代码分组明细数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(ReasonCodeCategoryDetail obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.ReasonCodeCategoryDetailDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.ReasonCodeCategoryDetailService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.ReasonCodeCategoryDetailDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除代码分组明细。
        /// </summary>
        /// <param name="key">代码分组明细名称。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(ReasonCodeCategoryDetailKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.ReasonCodeCategoryDetailDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.ReasonCodeCategoryDetailService_IsNotExists, key);
                return result;
            }
            try
            {
                this.ReasonCodeCategoryDetailDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取代码分组明细数据。
        /// </summary>
        /// <param name="key">代码分组明细名称.</param>
        /// <returns><see cref="MethodReturnResult&lt;ReasonCodeCategoryDetail&gt;" />,代码分组明细数据.</returns>
        public MethodReturnResult<ReasonCodeCategoryDetail> Get(ReasonCodeCategoryDetailKey key)
        {
            MethodReturnResult<ReasonCodeCategoryDetail> result = new MethodReturnResult<ReasonCodeCategoryDetail>();
            if (!this.ReasonCodeCategoryDetailDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.ReasonCodeCategoryDetailService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.ReasonCodeCategoryDetailDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取代码分组明细数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;ReasonCodeCategoryDetail&gt;" />,代码分组明细数据集合。</returns>
        public MethodReturnResult<IList<ReasonCodeCategoryDetail>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<ReasonCodeCategoryDetail>> result = new MethodReturnResult<IList<ReasonCodeCategoryDetail>>();
            try
            {
                result.Data = this.ReasonCodeCategoryDetailDataEngine.Get(cfg);
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
