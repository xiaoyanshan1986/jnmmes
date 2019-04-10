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
    /// 实现代码管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class EquipmentReasonCodeService : IEquipmentReasonCodeContract
    {
        /// <summary>
        /// 代码数据访问读写。
        /// </summary>
        public IEquipmentReasonCodeDataEngine EquipmentReasonCodeDataEngine { get; set; }


        /// <summary>
        /// 添加代码。
        /// </summary>
        /// <param name="obj">代码数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(EquipmentReasonCode obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.EquipmentReasonCodeDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.ReasonCodeService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.EquipmentReasonCodeDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改代码。
        /// </summary>
        /// <param name="obj">代码数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(EquipmentReasonCode obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.EquipmentReasonCodeDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.ReasonCodeService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.EquipmentReasonCodeDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除代码。
        /// </summary>
        /// <param name="key">代码名称。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.EquipmentReasonCodeDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.ReasonCodeService_IsNotExists, key);
                return result;
            }
            try
            {
                this.EquipmentReasonCodeDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取代码数据。
        /// </summary>
        /// <param name="key">代码名称.</param>
        /// <returns><see cref="MethodReturnResult&lt;ReasonCode&gt;" />,代码数据.</returns>
        public MethodReturnResult<EquipmentReasonCode> Get(string key)
        {
            MethodReturnResult<EquipmentReasonCode> result = new MethodReturnResult<EquipmentReasonCode>();
            if (!this.EquipmentReasonCodeDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.ReasonCodeService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.EquipmentReasonCodeDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取代码数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;ReasonCode&gt;" />,代码数据集合。</returns>
        public MethodReturnResult<IList<EquipmentReasonCode>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<EquipmentReasonCode>> result = new MethodReturnResult<IList<EquipmentReasonCode>>();
            try
            {
                result.Data = this.EquipmentReasonCodeDataEngine.Get(cfg);
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
