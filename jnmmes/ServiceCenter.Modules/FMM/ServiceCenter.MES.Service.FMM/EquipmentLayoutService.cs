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
    /// 实现设备布局管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class EquipmentLayoutService : IEquipmentLayoutContract
    {
        /// <summary>
        /// 设备布局数据访问读写。
        /// </summary>
        public IEquipmentLayoutDataEngine EquipmentLayoutDataEngine { get; set; }


        /// <summary>
        /// 添加设备布局。
        /// </summary>
        /// <param name="obj">设备布局数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(EquipmentLayout obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.EquipmentLayoutDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.EquipmentLayoutService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.EquipmentLayoutDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改设备布局。
        /// </summary>
        /// <param name="obj">设备布局数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(EquipmentLayout obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.EquipmentLayoutDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.EquipmentLayoutService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.EquipmentLayoutDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除设备布局。
        /// </summary>
        /// <param name="key">设备布局标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.EquipmentLayoutDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.EquipmentLayoutService_IsNotExists, key);
                return result;
            }
            try
            {
                this.EquipmentLayoutDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取设备布局数据。
        /// </summary>
        /// <param name="key">设备布局标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;EquipmentLayout&gt;" />,设备布局数据.</returns>
        public MethodReturnResult<EquipmentLayout> Get(string key)
        {
            MethodReturnResult<EquipmentLayout> result = new MethodReturnResult<EquipmentLayout>();
            if (!this.EquipmentLayoutDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.EquipmentLayoutService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.EquipmentLayoutDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取设备布局数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;EquipmentLayout&gt;" />,设备布局数据集合。</returns>
        public MethodReturnResult<IList<EquipmentLayout>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<EquipmentLayout>> result = new MethodReturnResult<IList<EquipmentLayout>>();
            try
            {
                result.Data = this.EquipmentLayoutDataEngine.Get(cfg);
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
