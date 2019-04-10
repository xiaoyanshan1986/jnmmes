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
    /// 实现代码分组管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class ReasonCodeCategoryService : IReasonCodeCategoryContract
    {
        /// <summary>
        /// 代码分组数据访问读写。
        /// </summary>
        public IReasonCodeCategoryDataEngine ReasonCodeCategoryDataEngine { get; set; }


        /// <summary>
        /// 添加代码分组。
        /// </summary>
        /// <param name="obj">代码分组数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(ReasonCodeCategory obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.ReasonCodeCategoryDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.ReasonCodeCategoryService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.ReasonCodeCategoryDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改代码分组。
        /// </summary>
        /// <param name="obj">代码分组数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(ReasonCodeCategory obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.ReasonCodeCategoryDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.ReasonCodeCategoryService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.ReasonCodeCategoryDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除代码分组。
        /// </summary>
        /// <param name="key">代码分组名称。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.ReasonCodeCategoryDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.ReasonCodeCategoryService_IsNotExists, key);
                return result;
            }
            try
            {
                this.ReasonCodeCategoryDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取代码分组数据。
        /// </summary>
        /// <param name="key">代码分组名称.</param>
        /// <returns><see cref="MethodReturnResult&lt;ReasonCodeCategory&gt;" />,代码分组数据.</returns>
        public MethodReturnResult<ReasonCodeCategory> Get(string key)
        {
            MethodReturnResult<ReasonCodeCategory> result = new MethodReturnResult<ReasonCodeCategory>();
            if (!this.ReasonCodeCategoryDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.ReasonCodeCategoryService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.ReasonCodeCategoryDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取代码分组数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;ReasonCodeCategory&gt;" />,代码分组数据集合。</returns>
        public MethodReturnResult<IList<ReasonCodeCategory>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<ReasonCodeCategory>> result = new MethodReturnResult<IList<ReasonCodeCategory>>();
            try
            {
                result.Data = this.ReasonCodeCategoryDataEngine.Get(cfg);
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
