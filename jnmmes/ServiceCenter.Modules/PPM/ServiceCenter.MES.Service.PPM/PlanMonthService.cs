using ServiceCenter.MES.DataAccess.Interface.PPM;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Service.Contract.PPM;
using ServiceCenter.MES.Service.PPM.Resources;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.PPM
{
    /// <summary>
    /// 实现月生产计划管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class PlanMonthService : IPlanMonthContract
    {
        /// <summary>
        /// 月生产计划数据访问读写。
        /// </summary>
        public IPlanMonthDataEngine PlanMonthDataEngine { get; set; }


        /// <summary>
        /// 添加月生产计划。
        /// </summary>
        /// <param name="obj">月生产计划数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(PlanMonth obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.PlanMonthDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.PlanMonthService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.PlanMonthDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }
        
        /// <summary>
        /// 修改月生产计划。
        /// </summary>
        /// <param name="obj">月生产计划数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(PlanMonth obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.PlanMonthDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.PlanMonthService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.PlanMonthDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除月生产计划。
        /// </summary>
        /// <param name="key">月生产计划名称。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(PlanMonthKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.PlanMonthDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.PlanMonthService_IsExists, key);
                return result;
            }
            try
            {
                this.PlanMonthDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取月生产计划数据。
        /// </summary>
        /// <param name="key">月生产计划名称.</param>
        /// <returns><see cref="MethodReturnResult&lt;PlanMonth&gt;" />,月生产计划数据.</returns>
        public MethodReturnResult<PlanMonth> Get(PlanMonthKey key)
        {
            MethodReturnResult<PlanMonth> result = new MethodReturnResult<PlanMonth>();
            if (!this.PlanMonthDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.PlanMonthService_IsExists, key);
                return result;
            }
            try
            {
                result.Data = this.PlanMonthDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取月生产计划数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;PlanMonth&gt;" />,月生产计划数据集合。</returns>
        public MethodReturnResult<IList<PlanMonth>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<PlanMonth>> result = new MethodReturnResult<IList<PlanMonth>>();
            try
            {
                result.Data = this.PlanMonthDataEngine.Get(cfg);
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
