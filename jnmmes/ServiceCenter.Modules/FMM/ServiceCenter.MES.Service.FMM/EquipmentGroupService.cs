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
    /// 实现设备组管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class EquipmentGroupService : IEquipmentGroupContract
    {
        /// <summary>
        /// 设备组数据访问读写。
        /// </summary>
        public IEquipmentGroupDataEngine EquipmentGroupDataEngine { get; set; }


        /// <summary>
        /// 添加设备组。
        /// </summary>
        /// <param name="obj">设备组数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(EquipmentGroup obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.EquipmentGroupDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.EquipmentGroupService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.EquipmentGroupDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改设备组。
        /// </summary>
        /// <param name="obj">设备组数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(EquipmentGroup obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.EquipmentGroupDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.EquipmentGroupService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.EquipmentGroupDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除设备组。
        /// </summary>
        /// <param name="key">设备组标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.EquipmentGroupDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.EquipmentGroupService_IsNotExists, key);
                return result;
            }
            try
            {
                this.EquipmentGroupDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取设备组数据。
        /// </summary>
        /// <param name="key">设备组标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;EquipmentGroup&gt;" />,设备组数据.</returns>
        public MethodReturnResult<EquipmentGroup> Get(string key)
        {
            MethodReturnResult<EquipmentGroup> result = new MethodReturnResult<EquipmentGroup>();
            if (!this.EquipmentGroupDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.EquipmentGroupService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.EquipmentGroupDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取设备组数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;EquipmentGroup&gt;" />,设备组数据集合。</returns>
        public MethodReturnResult<IList<EquipmentGroup>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<EquipmentGroup>> result = new MethodReturnResult<IList<EquipmentGroup>>();
            try
            {
                result.Data = this.EquipmentGroupDataEngine.Get(cfg);
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
