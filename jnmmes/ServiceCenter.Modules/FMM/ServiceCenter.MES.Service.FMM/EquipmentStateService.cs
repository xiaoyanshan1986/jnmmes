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
    /// 实现设备状态管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class EquipmentStateService : IEquipmentStateContract
    {
        /// <summary> 设备状态数据访问读写 </summary>
        public IEquipmentStateDataEngine EquipmentStateDataEngine { get; set; }
        
        /// <summary> 添加设备状态 </summary>
        /// <param name="obj">设备状态数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(EquipmentState obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.EquipmentStateDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.EquipmentStateService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.EquipmentStateDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }
        
        /// <summary> 修改设备状态 </summary>
        /// <param name="obj">设备状态数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(EquipmentState obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.EquipmentStateDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.EquipmentStateService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.EquipmentStateDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary> 删除设备状态 </summary>
        /// <param name="key">设备状态标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
                        
            try
            {
                //判断设备状态是否存在
                if (!this.EquipmentStateDataEngine.IsExists(key))
                {
                    result.Code = 1002;
                    result.Message = String.Format(StringResource.EquipmentStateService_IsNotExists, key);
                    return result;
                }

                //删除设备状态
                this.EquipmentStateDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }

            return result;
        }

        /// <summary> 获取设备状态数据 </summary>
        /// <param name="key">设备状态标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;EquipmentState&gt;" />,设备状态数据.</returns>
        public MethodReturnResult<EquipmentState> Get(string key)
        {
            MethodReturnResult<EquipmentState> result = new MethodReturnResult<EquipmentState>();
            if (!this.EquipmentStateDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.EquipmentStateService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.EquipmentStateDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary> 获取设备状态数据集合 </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;EquipmentState&gt;" />,设备状态数据集合。</returns>
        public MethodReturnResult<IList<EquipmentState>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<EquipmentState>> result = new MethodReturnResult<IList<EquipmentState>>();
            try
            {
                result.Data = this.EquipmentStateDataEngine.Get(cfg);
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
