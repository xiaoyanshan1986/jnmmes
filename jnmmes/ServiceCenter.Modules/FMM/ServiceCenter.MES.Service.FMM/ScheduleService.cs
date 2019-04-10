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
    /// 实现排班计划管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class ScheduleService : IScheduleContract
    {
        /// <summary>
        /// 排班计划数据访问读写。
        /// </summary>
        public IScheduleDataEngine ScheduleDataEngine { get; set; }


        /// <summary>
        /// 添加排班计划。
        /// </summary>
        /// <param name="obj">排班计划数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(Schedule obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.ScheduleDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.ScheduleService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.ScheduleDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改排班计划。
        /// </summary>
        /// <param name="obj">排班计划数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(Schedule obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.ScheduleDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.ScheduleService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.ScheduleDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除排班计划。
        /// </summary>
        /// <param name="key">排班计划名称。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.ScheduleDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.ScheduleService_IsNotExists, key);
                return result;
            }
            try
            {
                this.ScheduleDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取排班计划数据。
        /// </summary>
        /// <param name="key">排班计划名称.</param>
        /// <returns><see cref="MethodReturnResult&lt;Schedule&gt;" />,排班计划数据.</returns>
        public MethodReturnResult<Schedule> Get(string key)
        {
            MethodReturnResult<Schedule> result = new MethodReturnResult<Schedule>();
            if (!this.ScheduleDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.ScheduleService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.ScheduleDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取排班计划数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;Schedule&gt;" />,排班计划数据集合。</returns>
        public MethodReturnResult<IList<Schedule>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<Schedule>> result = new MethodReturnResult<IList<Schedule>>();
            try
            {
                result.Data = this.ScheduleDataEngine.Get(cfg);
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
