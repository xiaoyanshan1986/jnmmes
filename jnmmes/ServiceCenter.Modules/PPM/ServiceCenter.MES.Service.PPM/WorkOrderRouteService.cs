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
    /// 实现工单工艺管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class WorkOrderRouteService : IWorkOrderRouteContract
    {
        /// <summary>
        /// 工单工艺数据访问读写。
        /// </summary>
        public IWorkOrderRouteDataEngine WorkOrderRouteDataEngine { get; set; }


        /// <summary>
        /// 添加工单工艺。
        /// </summary>
        /// <param name="obj">工单工艺数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(WorkOrderRoute obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.WorkOrderRouteDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.WorkOrderRouteService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.WorkOrderRouteDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改工单工艺。
        /// </summary>
        /// <param name="obj">工单工艺数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(WorkOrderRoute obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.WorkOrderRouteDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.WorkOrderRouteService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.WorkOrderRouteDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除工单工艺。
        /// </summary>
        /// <param name="key">工单工艺标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(WorkOrderRouteKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.WorkOrderRouteDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.WorkOrderRouteService_IsNotExists, key);
                return result;
            }
            try
            {
                this.WorkOrderRouteDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取工单工艺数据。
        /// </summary>
        /// <param name="key">工单工艺标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;WorkOrderRoute&gt;" />,工单工艺数据.</returns>
        public MethodReturnResult<WorkOrderRoute> Get(WorkOrderRouteKey key)
        {
            MethodReturnResult<WorkOrderRoute> result = new MethodReturnResult<WorkOrderRoute>();
            if (!this.WorkOrderRouteDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.WorkOrderRouteService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.WorkOrderRouteDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取工单工艺数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;WorkOrderRoute&gt;" />,工单工艺数据集合。</returns>
        public MethodReturnResult<IList<WorkOrderRoute>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<WorkOrderRoute>> result = new MethodReturnResult<IList<WorkOrderRoute>>();
            try
            {
                result.Data = this.WorkOrderRouteDataEngine.Get(cfg);
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
