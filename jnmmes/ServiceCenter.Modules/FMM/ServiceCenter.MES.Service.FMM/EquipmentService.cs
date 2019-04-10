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
    /// 实现设备管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class EquipmentService : IEquipmentContract
    {
        /// <summary>
        /// 设备数据访问读写。
        /// </summary>
        public IEquipmentDataEngine EquipmentDataEngine { get; set; }


        /// <summary>
        /// 添加设备。
        /// </summary>
        /// <param name="obj">设备数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(Equipment obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.EquipmentDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.EquipmentService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.EquipmentDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改设备。
        /// </summary>
        /// <param name="obj">设备数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(Equipment obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.EquipmentDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.EquipmentService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.EquipmentDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除设备。
        /// </summary>
        /// <param name="key">设备标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.EquipmentDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.EquipmentService_IsNotExists, key);
                return result;
            }
            try
            {
                this.EquipmentDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取设备数据。
        /// </summary>
        /// <param name="key">设备标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;Equipment&gt;" />,设备数据.</returns>
        public MethodReturnResult<Equipment> Get(string key)
        {
            MethodReturnResult<Equipment> result = new MethodReturnResult<Equipment>();
            if (!this.EquipmentDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.EquipmentService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.EquipmentDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取设备数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;Equipment&gt;" />,设备数据集合。</returns>
        public MethodReturnResult<IList<Equipment>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<Equipment>> result = new MethodReturnResult<IList<Equipment>>();
            try
            {
                result.Data = this.EquipmentDataEngine.Get(cfg);
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
