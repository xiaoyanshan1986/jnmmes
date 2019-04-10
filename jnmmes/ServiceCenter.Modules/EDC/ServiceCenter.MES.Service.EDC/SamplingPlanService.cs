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
    /// 实现采样计划管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class SamplingPlanService : ISamplingPlanContract
    {
        /// <summary>
        /// 采样计划数据访问读写。
        /// </summary>
        public ISamplingPlanDataEngine SamplingPlanDataEngine { get; set; }


        /// <summary>
        /// 添加采样计划。
        /// </summary>
        /// <param name="obj">采样计划数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(SamplingPlan obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.SamplingPlanDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.SamplingPlanService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.SamplingPlanDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改采样计划。
        /// </summary>
        /// <param name="obj">采样计划数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(SamplingPlan obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.SamplingPlanDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.SamplingPlanService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.SamplingPlanDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除采样计划。
        /// </summary>
        /// <param name="key">采样计划标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.SamplingPlanDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.SamplingPlanService_IsNotExists, key);
                return result;
            }
            try
            {
                this.SamplingPlanDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取采样计划数据。
        /// </summary>
        /// <param name="key">采样计划标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;SamplingPlan&gt;" />,采样计划数据.</returns>
        public MethodReturnResult<SamplingPlan> Get(string key)
        {
            MethodReturnResult<SamplingPlan> result = new MethodReturnResult<SamplingPlan>();
            if (!this.SamplingPlanDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.SamplingPlanService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.SamplingPlanDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取采样计划数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;SamplingPlan&gt;" />,采样计划数据集合。</returns>
        public MethodReturnResult<IList<SamplingPlan>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<SamplingPlan>> result = new MethodReturnResult<IList<SamplingPlan>>();
            try
            {
                result.Data = this.SamplingPlanDataEngine.Get(cfg);
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
