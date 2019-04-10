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
    /// 实现月排班计划管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class ScheduleMonthService : IScheduleMonthContract
    {
        /// <summary>
        /// 月排班计划数据访问读写。
        /// </summary>
        public IScheduleMonthDataEngine ScheduleMonthDataEngine { get; set; }


        /// <summary>
        /// 添加月排班计划。
        /// </summary>
        /// <param name="obj">月排班计划数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(ScheduleMonth obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.ScheduleMonthDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.ScheduleMonthService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.ScheduleMonthDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改月排班计划。
        /// </summary>
        /// <param name="obj">月排班计划数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(ScheduleMonth obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.ScheduleMonthDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.ScheduleMonthService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.ScheduleMonthDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除月排班计划。
        /// </summary>
        /// <param name="key">月排班计划名称。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(ScheduleMonthKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.ScheduleMonthDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.ScheduleMonthService_IsNotExists, key);
                return result;
            }
            try
            {
                this.ScheduleMonthDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取月排班计划数据。
        /// </summary>
        /// <param name="key">月排班计划名称.</param>
        /// <returns><see cref="MethodReturnResult&lt;ScheduleMonth&gt;" />,月排班计划数据.</returns>
        public MethodReturnResult<ScheduleMonth> Get(ScheduleMonthKey key)
        {
            MethodReturnResult<ScheduleMonth> result = new MethodReturnResult<ScheduleMonth>();
            if (!this.ScheduleMonthDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.ScheduleMonthService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.ScheduleMonthDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取月排班计划数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;ScheduleMonth&gt;" />,月排班计划数据集合。</returns>
        public MethodReturnResult<IList<ScheduleMonth>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<ScheduleMonth>> result = new MethodReturnResult<IList<ScheduleMonth>>();
            try
            {
                result.Data = this.ScheduleMonthDataEngine.Get(cfg);
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
