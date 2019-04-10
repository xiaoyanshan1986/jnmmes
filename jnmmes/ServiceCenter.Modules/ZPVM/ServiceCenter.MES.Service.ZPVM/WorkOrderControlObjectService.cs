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
    /// 实现工单控制参数设置数据管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class WorkOrderControlObjectService : IWorkOrderControlObjectContract
    {
        /// <summary>
        /// 工单控制参数设置数据数据访问读写。
        /// </summary>
        public IWorkOrderControlObjectDataEngine WorkOrderControlObjectDataEngine { get; set; }


        /// <summary>
        /// 添加工单控制参数设置数据。
        /// </summary>
        /// <param name="obj">工单控制参数设置数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(WorkOrderControlObject obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.WorkOrderControlObjectDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.WorkOrderControlObjectService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.WorkOrderControlObjectDataEngine.Insert(obj);
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
        /// 修改工单控制参数设置数据。
        /// </summary>
        /// <param name="obj">工单控制参数设置数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(WorkOrderControlObject obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.WorkOrderControlObjectDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.WorkOrderControlObjectService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.WorkOrderControlObjectDataEngine.Update(obj);
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
        /// 删除工单控制参数设置数据。
        /// </summary>
        /// <param name="key">工单控制参数设置数据标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(WorkOrderControlObjectKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.WorkOrderControlObjectDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.WorkOrderControlObjectService_IsNotExists, key);
                return result;
            }
            try
            {
                this.WorkOrderControlObjectDataEngine.Delete(key);
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
        /// 获取工单控制参数设置数据数据。
        /// </summary>
        /// <param name="key">工单控制参数设置数据标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;WorkOrderControlObject&gt;" />,工单控制参数设置数据数据.</returns>
        public MethodReturnResult<WorkOrderControlObject> Get(WorkOrderControlObjectKey key)
        {
            MethodReturnResult<WorkOrderControlObject> result = new MethodReturnResult<WorkOrderControlObject>();
            if (!this.WorkOrderControlObjectDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.WorkOrderControlObjectService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.WorkOrderControlObjectDataEngine.Get(key);
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
        /// 获取工单控制参数设置数据数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;WorkOrderControlObject&gt;" />,工单控制参数设置数据数据集合。</returns>
        public MethodReturnResult<IList<WorkOrderControlObject>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<WorkOrderControlObject>> result = new MethodReturnResult<IList<WorkOrderControlObject>>();
            try
            {
                result.Data = this.WorkOrderControlObjectDataEngine.Get(cfg);
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
