using ServiceCenter.MES.DataAccess.Interface.ZPVM;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.MES.Service.Contract.ZPVM;
using ServiceCenter.MES.Service.ZPVM.Resources;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.ZPVM
{
    /// <summary>
    /// 实现工单衰减设置数据管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class WorkOrderDecayService : IWorkOrderDecayContract
    {
        /// <summary>
        /// 工单衰减设置数据数据访问读写。
        /// </summary>
        public IWorkOrderDecayDataEngine WorkOrderDecayDataEngine { get; set; }


        /// <summary>
        /// 添加工单衰减设置数据。
        /// </summary>
        /// <param name="obj">工单衰减设置数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(WorkOrderDecay obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.WorkOrderDecayDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.WorkOrderDecayService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.WorkOrderDecayDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error,ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }


        /// <summary>
        /// 修改工单衰减设置数据。
        /// </summary>
        /// <param name="obj">工单衰减设置数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(WorkOrderDecay obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.WorkOrderDecayDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.WorkOrderDecayService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.WorkOrderDecayDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }
        /// <summary>
        /// 删除工单衰减设置数据。
        /// </summary>
        /// <param name="key">工单衰减设置数据标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(WorkOrderDecayKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.WorkOrderDecayDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.WorkOrderDecayService_IsNotExists, key);
                return result;
            }
            try
            {
                this.WorkOrderDecayDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 获取工单衰减设置数据数据。
        /// </summary>
        /// <param name="key">工单衰减设置数据标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;WorkOrderDecay&gt;" />,工单衰减设置数据数据.</returns>
        public MethodReturnResult<WorkOrderDecay> Get(WorkOrderDecayKey key)
        {
            MethodReturnResult<WorkOrderDecay> result = new MethodReturnResult<WorkOrderDecay>();
            if (!this.WorkOrderDecayDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.WorkOrderDecayService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.WorkOrderDecayDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 获取工单衰减设置数据数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;WorkOrderDecay&gt;" />,工单衰减设置数据数据集合。</returns>
        public MethodReturnResult<IList<WorkOrderDecay>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<WorkOrderDecay>> result = new MethodReturnResult<IList<WorkOrderDecay>>();
            try
            {
                result.Data = this.WorkOrderDecayDataEngine.Get(cfg);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }
    }
}
