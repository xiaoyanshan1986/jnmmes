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
    /// 实现工单产品管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class WorkOrderProductService : IWorkOrderProductContract
    {
        /// <summary>
        /// 工单产品数据访问读写。
        /// </summary>
        public IWorkOrderProductDataEngine WorkOrderProductDataEngine { get; set; }

        /// <summary>
        /// 物料数据访问读写。
        /// </summary>
        public IMaterialDataEngine MaterialDataEngine { get; set; }

        /// <summary>
        /// 添加工单产品。
        /// </summary>
        /// <param name="obj">工单产品数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(WorkOrderProduct obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            
            try
            {
                if (this.WorkOrderProductDataEngine.IsExists(obj.Key))
                {
                    result.Code = 1001;
                    result.Message = String.Format(StringResource.WorkOrderProductService_IsExists, obj.Key);
                    return result;
                }
                Material mat = this.MaterialDataEngine.Get(obj.Key.MaterialCode);
                if (mat == null || mat.IsProduct == false)
                {
                    result.Code = 1003;
                    result.Message = String.Format(StringResource.WorkOrderService_MaterialCodeIsNotExists, obj.Key.MaterialCode);
                    return result;
                }

                this.WorkOrderProductDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改工单产品。
        /// </summary>
        /// <param name="obj">工单产品数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(WorkOrderProduct obj)
        {
            MethodReturnResult result = new MethodReturnResult();
           
            try
            {
                if (!this.WorkOrderProductDataEngine.IsExists(obj.Key))
                {
                    result.Code = 1002;
                    result.Message = String.Format(StringResource.WorkOrderProductService_IsNotExists, obj.Key);
                    return result;
                }

                Material mat = this.MaterialDataEngine.Get(obj.Key.MaterialCode);
                if (mat == null || mat.IsProduct == false)
                {
                    result.Code = 1003;
                    result.Message = String.Format(StringResource.WorkOrderService_MaterialCodeIsNotExists, obj.Key.MaterialCode);
                    return result;
                }
                this.WorkOrderProductDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除工单产品。
        /// </summary>
        /// <param name="key">工单产品标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(WorkOrderProductKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            
            try
            {
                if (!this.WorkOrderProductDataEngine.IsExists(key))
                {
                    result.Code = 1002;
                    result.Message = String.Format(StringResource.WorkOrderProductService_IsNotExists, key);
                    return result;
                }

                this.WorkOrderProductDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取工单产品数据。
        /// </summary>
        /// <param name="key">工单产品标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;WorkOrderProduct&gt;" />,工单产品数据.</returns>
        public MethodReturnResult<WorkOrderProduct> Get(WorkOrderProductKey key)
        {
            MethodReturnResult<WorkOrderProduct> result = new MethodReturnResult<WorkOrderProduct>();
            
            try
            {
                if (!this.WorkOrderProductDataEngine.IsExists(key))
                {
                    result.Code = 1002;
                    result.Message = String.Format(StringResource.WorkOrderProductService_IsNotExists, key);
                    return result;
                }
                result.Data = this.WorkOrderProductDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取工单产品数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;WorkOrderProduct&gt;" />,工单产品数据集合。</returns>
        public MethodReturnResult<IList<WorkOrderProduct>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<WorkOrderProduct>> result = new MethodReturnResult<IList<WorkOrderProduct>>();
            try
            {
                result.Data = this.WorkOrderProductDataEngine.Get(cfg);
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
