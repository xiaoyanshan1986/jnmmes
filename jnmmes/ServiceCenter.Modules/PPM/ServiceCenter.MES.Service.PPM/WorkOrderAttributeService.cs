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
    /// 实现工单属性管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class WorkOrderAttributeService : IWorkOrderAttributeContract
    {
        /// <summary>
        /// 工单属性数据访问读写。
        /// </summary>
        public IWorkOrderAttributeDataEngine WorkOrderAttributeDataEngine { get; set; }


        /// <summary>
        /// 添加工单属性。
        /// </summary>
        /// <param name="obj">工单属性数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(WorkOrderAttribute obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.WorkOrderAttributeDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.WorkOrderAttributeService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.WorkOrderAttributeDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改工单属性。
        /// </summary>
        /// <param name="obj">工单属性数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(WorkOrderAttribute obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.WorkOrderAttributeDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.WorkOrderAttributeService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.WorkOrderAttributeDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除工单属性。
        /// </summary>
        /// <param name="key">工单属性标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(WorkOrderAttributeKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.WorkOrderAttributeDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.WorkOrderAttributeService_IsNotExists, key);
                return result;
            }
            try
            {
                this.WorkOrderAttributeDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取工单属性数据。
        /// </summary>
        /// <param name="key">工单属性标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;WorkOrderAttribute&gt;" />,工单属性数据.</returns>
        public MethodReturnResult<WorkOrderAttribute> Get(WorkOrderAttributeKey key)
        {
            MethodReturnResult<WorkOrderAttribute> result = new MethodReturnResult<WorkOrderAttribute>();
            if (!this.WorkOrderAttributeDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.WorkOrderAttributeService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.WorkOrderAttributeDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取工单属性数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;WorkOrderAttribute&gt;" />,工单属性数据集合。</returns>
        public MethodReturnResult<IList<WorkOrderAttribute>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<WorkOrderAttribute>> result = new MethodReturnResult<IList<WorkOrderAttribute>>();
            try
            {
                result.Data = this.WorkOrderAttributeDataEngine.Get(cfg);
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
