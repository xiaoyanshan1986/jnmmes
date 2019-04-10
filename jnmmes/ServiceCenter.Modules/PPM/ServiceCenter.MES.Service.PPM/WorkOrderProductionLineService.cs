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
    /// 实现工单生产线管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class WorkOrderProductionLineService : IWorkOrderProductionLineContract
    {
        /// <summary>
        /// 工单生产线数据访问读写。
        /// </summary>
        public IWorkOrderProductionLineDataEngine WorkOrderProductionLineDataEngine { get; set; }


        /// <summary>
        /// 添加工单生产线。
        /// </summary>
        /// <param name="obj">工单生产线数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(WorkOrderProductionLine obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.WorkOrderProductionLineDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.WorkOrderProductionLineService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.WorkOrderProductionLineDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改工单生产线。
        /// </summary>
        /// <param name="obj">工单生产线数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(WorkOrderProductionLine obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.WorkOrderProductionLineDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.WorkOrderProductionLineService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.WorkOrderProductionLineDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除工单生产线。
        /// </summary>
        /// <param name="key">工单生产线标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(WorkOrderProductionLineKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.WorkOrderProductionLineDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.WorkOrderProductionLineService_IsNotExists, key);
                return result;
            }
            try
            {
                this.WorkOrderProductionLineDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取工单生产线数据。
        /// </summary>
        /// <param name="key">工单生产线标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;WorkOrderProductionLine&gt;" />,工单生产线数据.</returns>
        public MethodReturnResult<WorkOrderProductionLine> Get(WorkOrderProductionLineKey key)
        {
            MethodReturnResult<WorkOrderProductionLine> result = new MethodReturnResult<WorkOrderProductionLine>();
            if (!this.WorkOrderProductionLineDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.WorkOrderProductionLineService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.WorkOrderProductionLineDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取工单生产线数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;WorkOrderProductionLine&gt;" />,工单生产线数据集合。</returns>
        public MethodReturnResult<IList<WorkOrderProductionLine>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<WorkOrderProductionLine>> result = new MethodReturnResult<IList<WorkOrderProductionLine>>();
            try
            {
                result.Data = this.WorkOrderProductionLineDataEngine.Get(cfg);
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
