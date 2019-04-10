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
    /// 实现工序设备管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class RouteOperationEquipmentService : IRouteOperationEquipmentContract
    {
        /// <summary>
        /// 工序设备数据访问读写。
        /// </summary>
        public IRouteOperationEquipmentDataEngine RouteOperationEquipmentDataEngine { get; set; }


        /// <summary>
        /// 添加工序设备。
        /// </summary>
        /// <param name="obj">工序设备数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(RouteOperationEquipment obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.RouteOperationEquipmentDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.RouteOperationEquipmentService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.RouteOperationEquipmentDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改工序设备。
        /// </summary>
        /// <param name="obj">工序设备数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(RouteOperationEquipment obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.RouteOperationEquipmentDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.RouteOperationEquipmentService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.RouteOperationEquipmentDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除工序设备。
        /// </summary>
        /// <param name="key">工序设备名称。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(RouteOperationEquipmentKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.RouteOperationEquipmentDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.RouteOperationEquipmentService_IsNotExists, key);
                return result;
            }
            try
            {
                this.RouteOperationEquipmentDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取工序设备数据。
        /// </summary>
        /// <param name="key">工序设备名称.</param>
        /// <returns><see cref="MethodReturnResult&lt;RouteOperationEquipment&gt;" />,工序设备数据.</returns>
        public MethodReturnResult<RouteOperationEquipment> Get(RouteOperationEquipmentKey key)
        {
            MethodReturnResult<RouteOperationEquipment> result = new MethodReturnResult<RouteOperationEquipment>();
            if (!this.RouteOperationEquipmentDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.RouteOperationEquipmentService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.RouteOperationEquipmentDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取工序设备数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;RouteOperationEquipment&gt;" />,工序设备数据集合。</returns>
        public MethodReturnResult<IList<RouteOperationEquipment>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<RouteOperationEquipment>> result = new MethodReturnResult<IList<RouteOperationEquipment>>();
            try
            {
                result.Data = this.RouteOperationEquipmentDataEngine.Get(cfg);
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
