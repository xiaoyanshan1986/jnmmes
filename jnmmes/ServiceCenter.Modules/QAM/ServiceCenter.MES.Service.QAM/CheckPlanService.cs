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
    /// 实现检验计划管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class CheckPlanService : ICheckPlanContract
    {
        /// <summary>
        /// 检验计划数据访问读写。
        /// </summary>
        public ICheckPlanDataEngine CheckPlanDataEngine { get; set; }


        /// <summary>
        /// 添加检验计划。
        /// </summary>
        /// <param name="obj">检验计划数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(CheckPlan obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.CheckPlanDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.CheckPlanService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.CheckPlanDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改检验计划。
        /// </summary>
        /// <param name="obj">检验计划数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(CheckPlan obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.CheckPlanDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.CheckPlanService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.CheckPlanDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除检验计划。
        /// </summary>
        /// <param name="key">检验计划标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.CheckPlanDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.CheckPlanService_IsNotExists, key);
                return result;
            }
            try
            {
                this.CheckPlanDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取检验计划数据。
        /// </summary>
        /// <param name="key">检验计划标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;CheckPlan&gt;" />,检验计划数据.</returns>
        public MethodReturnResult<CheckPlan> Get(string key)
        {
            MethodReturnResult<CheckPlan> result = new MethodReturnResult<CheckPlan>();
            if (!this.CheckPlanDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.CheckPlanService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.CheckPlanDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取检验计划数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;CheckPlan&gt;" />,检验计划数据集合。</returns>
        public MethodReturnResult<IList<CheckPlan>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<CheckPlan>> result = new MethodReturnResult<IList<CheckPlan>>();
            try
            {
                result.Data = this.CheckPlanDataEngine.Get(cfg);
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
