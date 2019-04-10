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
    /// 实现排班计划详细数据管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class ScheduleDetailService : IScheduleDetailContract
    {
        /// <summary>
        /// 排班计划详细数据数据访问读写。
        /// </summary>
        public IScheduleDetailDataEngine ScheduleDetailDataEngine { get; set; }


        /// <summary>
        /// 添加排班计划详细数据。
        /// </summary>
        /// <param name="obj">排班计划详细数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(ScheduleDetail obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.ScheduleDetailDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.ScheduleDetailService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.ScheduleDetailDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改排班计划详细数据。
        /// </summary>
        /// <param name="obj">排班计划详细数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(ScheduleDetail obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.ScheduleDetailDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.ScheduleDetailService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.ScheduleDetailDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除排班计划详细数据。
        /// </summary>
        /// <param name="key">排班计划详细数据名称。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(ScheduleDetailKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.ScheduleDetailDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.ScheduleDetailService_IsNotExists, key);
                return result;
            }
            try
            {
                this.ScheduleDetailDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取排班计划详细数据数据。
        /// </summary>
        /// <param name="key">排班计划详细数据名称.</param>
        /// <returns><see cref="MethodReturnResult&lt;ScheduleDetail&gt;" />,排班计划详细数据数据.</returns>
        public MethodReturnResult<ScheduleDetail> Get(ScheduleDetailKey key)
        {
            MethodReturnResult<ScheduleDetail> result = new MethodReturnResult<ScheduleDetail>();
            if (!this.ScheduleDetailDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.ScheduleDetailService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.ScheduleDetailDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取排班计划详细数据数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;ScheduleDetail&gt;" />,排班计划详细数据数据集合。</returns>
        public MethodReturnResult<IList<ScheduleDetail>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<ScheduleDetail>> result = new MethodReturnResult<IList<ScheduleDetail>>();
            try
            {
                result.Data = this.ScheduleDetailDataEngine.Get(cfg);
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
