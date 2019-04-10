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
    /// 实现工单打印标签设置数据管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class WorkOrderPrintSetService : IWorkOrderPrintSetContract
    {
        /// <summary>
        /// 工单打印标签设置数据数据访问读写。
        /// </summary>
        public IWorkOrderPrintSetDataEngine WorkOrderPrintSetDataEngine { get; set; }


        /// <summary>
        /// 添加工单打印标签设置数据。
        /// </summary>
        /// <param name="obj">工单打印标签设置数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(WorkOrderPrintSet obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.WorkOrderPrintSetDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.WorkOrderPrintSetService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.WorkOrderPrintSetDataEngine.Insert(obj);
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
        /// 修改工单打印标签设置数据。
        /// </summary>
        /// <param name="obj">工单打印标签设置数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(WorkOrderPrintSet obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.WorkOrderPrintSetDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.WorkOrderPrintSetService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.WorkOrderPrintSetDataEngine.Update(obj);
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
        /// 删除工单打印标签设置数据。
        /// </summary>
        /// <param name="key">工单打印标签设置数据标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(WorkOrderPrintSetKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.WorkOrderPrintSetDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.WorkOrderPrintSetService_IsNotExists, key);
                return result;
            }
            try
            {
                this.WorkOrderPrintSetDataEngine.Delete(key);
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
        /// 获取工单打印标签设置数据数据。
        /// </summary>
        /// <param name="key">工单打印标签设置数据标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;WorkOrderPrintSet&gt;" />,工单打印标签设置数据数据.</returns>
        public MethodReturnResult<WorkOrderPrintSet> Get(WorkOrderPrintSetKey key)
        {
            MethodReturnResult<WorkOrderPrintSet> result = new MethodReturnResult<WorkOrderPrintSet>();
            if (!this.WorkOrderPrintSetDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.WorkOrderPrintSetService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.WorkOrderPrintSetDataEngine.Get(key);
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
        /// 获取工单打印标签设置数据数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;WorkOrderPrintSet&gt;" />,工单打印标签设置数据数据集合。</returns>
        public MethodReturnResult<IList<WorkOrderPrintSet>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<WorkOrderPrintSet>> result = new MethodReturnResult<IList<WorkOrderPrintSet>>();
            try
            {
                result.Data = this.WorkOrderPrintSetDataEngine.Get(cfg);
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
