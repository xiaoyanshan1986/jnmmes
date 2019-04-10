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
    /// 实现设备可切换状态管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class EquipmentChangeStateService : IEquipmentChangeStateContract
    {
        /// <summary>
        /// 设备可切换状态数据访问读写。
        /// </summary>
        public IEquipmentChangeStateDataEngine EquipmentChangeStateDataEngine { get; set; }


        /// <summary>
        /// 添加设备可切换状态。
        /// </summary>
        /// <param name="obj">设备可切换状态数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(EquipmentChangeState obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.EquipmentChangeStateDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.EquipmentChangeStateService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.EquipmentChangeStateDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改设备可切换状态。
        /// </summary>
        /// <param name="obj">设备可切换状态数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(EquipmentChangeState obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                this.EquipmentChangeStateDataEngine.Modify(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除设备可切换状态。
        /// </summary>
        /// <param name="key">设备可切换状态标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.EquipmentChangeStateDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.EquipmentChangeStateService_IsNotExists, key);
                return result;
            }
            try
            {
                this.EquipmentChangeStateDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取设备可切换状态数据。
        /// </summary>
        /// <param name="key">设备可切换状态标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;EquipmentChangeState&gt;" />,设备可切换状态数据.</returns>
        public MethodReturnResult<EquipmentChangeState> Get(string key)
        {
            MethodReturnResult<EquipmentChangeState> result = new MethodReturnResult<EquipmentChangeState>();
            if (!this.EquipmentChangeStateDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.EquipmentChangeStateService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.EquipmentChangeStateDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取设备可切换状态数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;EquipmentChangeState&gt;" />,设备可切换状态数据集合。</returns>
        public MethodReturnResult<IList<EquipmentChangeState>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<EquipmentChangeState>> result = new MethodReturnResult<IList<EquipmentChangeState>>();
            try
            {
                result.Data = this.EquipmentChangeStateDataEngine.Get(cfg);
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
