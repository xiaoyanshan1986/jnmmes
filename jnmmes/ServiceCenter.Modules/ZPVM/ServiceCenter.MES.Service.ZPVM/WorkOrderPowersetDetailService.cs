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
    /// 实现工单子分档设置数据管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class WorkOrderPowersetDetailService : IWorkOrderPowersetDetailContract
    {
        /// <summary>
        /// 工单子分档设置数据数据访问读写。
        /// </summary>
        public IWorkOrderPowersetDetailDataEngine WorkOrderPowersetDetailDataEngine { get; set; }


        /// <summary>
        /// 添加工单子分档设置数据。
        /// </summary>
        /// <param name="obj">工单子分档设置数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(WorkOrderPowersetDetail obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.WorkOrderPowersetDetailDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.WorkOrderPowersetDetailService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.WorkOrderPowersetDetailDataEngine.Insert(obj);
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
        /// 修改工单子分档设置数据。
        /// </summary>
        /// <param name="obj">工单子分档设置数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(WorkOrderPowersetDetail obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.WorkOrderPowersetDetailDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.WorkOrderPowersetDetailService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.WorkOrderPowersetDetailDataEngine.Update(obj);
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
        /// 删除工单子分档设置数据。
        /// </summary>
        /// <param name="key">工单子分档设置数据标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(WorkOrderPowersetDetailKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.WorkOrderPowersetDetailDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.WorkOrderPowersetDetailService_IsNotExists, key);
                return result;
            }
            try
            {
                this.WorkOrderPowersetDetailDataEngine.Delete(key);
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
        /// 获取工单子分档设置数据数据。
        /// </summary>
        /// <param name="key">工单子分档设置数据标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;WorkOrderPowersetDetail&gt;" />,工单子分档设置数据数据.</returns>
        public MethodReturnResult<WorkOrderPowersetDetail> Get(WorkOrderPowersetDetailKey key)
        {
            MethodReturnResult<WorkOrderPowersetDetail> result = new MethodReturnResult<WorkOrderPowersetDetail>();
            if (!this.WorkOrderPowersetDetailDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.WorkOrderPowersetDetailService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.WorkOrderPowersetDetailDataEngine.Get(key);
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
        /// 获取工单子分档设置数据数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;WorkOrderPowersetDetail&gt;" />,工单子分档设置数据数据集合。</returns>
        public MethodReturnResult<IList<WorkOrderPowersetDetail>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<WorkOrderPowersetDetail>> result = new MethodReturnResult<IList<WorkOrderPowersetDetail>>();
            try
            {
                result.Data = this.WorkOrderPowersetDetailDataEngine.Get(cfg);
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
