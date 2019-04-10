using ServiceCenter.MES.DataAccess.Interface.FMM;
using ServiceCenter.MES.DataAccess.Interface.PPM;
using ServiceCenter.MES.Model.FMM;
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
    /// 实现工单BOM管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class WorkOrderBOMService : IWorkOrderBOMContract
    {
        /// <summary>
        /// 工单BOM数据访问读写。
        /// </summary>
        public IWorkOrderBOMDataEngine WorkOrderBOMDataEngine { get; set; }
        /// <summary>
        /// 物料数据访问读写。
        /// </summary>
        public IMaterialDataEngine MaterialDataEngine { get; set; }

        /// <summary>
        /// 添加工单BOM。
        /// </summary>
        /// <param name="obj">工单BOM数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(WorkOrderBOM obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                if (this.WorkOrderBOMDataEngine.IsExists(obj.Key))
                {
                    result.Code = 1001;
                    result.Message = String.Format(StringResource.WorkOrderBOMService_IsExists, obj.Key);
                    return result;
                }
                Material mat = this.MaterialDataEngine.Get(obj.MaterialCode);
                if (mat==null || mat.IsRaw==false)
                {
                    result.Code = 1003;
                    result.Message = String.Format(StringResource.WorkOrderService_MaterialCodeIsNotExists, obj.MaterialCode);
                    return result;
                }

                this.WorkOrderBOMDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改工单BOM。
        /// </summary>
        /// <param name="obj">工单BOM数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(WorkOrderBOM obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            
            try
            {
                if (!this.WorkOrderBOMDataEngine.IsExists(obj.Key))
                {
                    result.Code = 1002;
                    result.Message = String.Format(StringResource.WorkOrderBOMService_IsNotExists, obj.Key);
                    return result;
                }
                Material mat = this.MaterialDataEngine.Get(obj.MaterialCode);
                if (mat == null || mat.IsRaw == false)
                {
                    result.Code = 1003;
                    result.Message = String.Format(StringResource.WorkOrderService_MaterialCodeIsNotExists, obj.MaterialCode);
                    return result;
                }
                this.WorkOrderBOMDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除工单BOM。
        /// </summary>
        /// <param name="key">工单BOM标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(WorkOrderBOMKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.WorkOrderBOMDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.WorkOrderBOMService_IsNotExists, key);
                return result;
            }
            try
            {
                this.WorkOrderBOMDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取工单BOM数据。
        /// </summary>
        /// <param name="key">工单BOM标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;WorkOrderBOM&gt;" />,工单BOM数据.</returns>
        public MethodReturnResult<WorkOrderBOM> Get(WorkOrderBOMKey key)
        {
            MethodReturnResult<WorkOrderBOM> result = new MethodReturnResult<WorkOrderBOM>();
            if (!this.WorkOrderBOMDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.WorkOrderBOMService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.WorkOrderBOMDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取工单BOM数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;WorkOrderBOM&gt;" />,工单BOM数据集合。</returns>
        public MethodReturnResult<IList<WorkOrderBOM>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<WorkOrderBOM>> result = new MethodReturnResult<IList<WorkOrderBOM>>();
            try
            {
                result.Data = this.WorkOrderBOMDataEngine.Get(cfg);
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
