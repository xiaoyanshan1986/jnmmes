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
    /// 实现规则-控制参数对象设置数据管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class EquipmentControlObjectService : IEquipmentControlObjectContract
    {
        /// <summary>
        /// 规则-控制参数对象设置数据数据访问读写。
        /// </summary>
        public IEquipmentControlObjectDataEngine EquipmentControlObjectDataEngine { get; set; }


        /// <summary>
        /// 添加规则-控制参数对象设置数据。
        /// </summary>
        /// <param name="obj">规则-控制参数对象设置数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(EquipmentControlObject obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.EquipmentControlObjectDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.EquipmentControlObjectService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.EquipmentControlObjectDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error,ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }


        /// <summary>
        /// 修改规则-控制参数对象设置数据。
        /// </summary>
        /// <param name="obj">规则-控制参数对象设置数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(EquipmentControlObject obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.EquipmentControlObjectDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.EquipmentControlObjectService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.EquipmentControlObjectDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }
        /// <summary>
        /// 删除规则-控制参数对象设置数据。
        /// </summary>
        /// <param name="key">规则-控制参数对象设置数据标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(EquipmentControlObjectKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.EquipmentControlObjectDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.EquipmentControlObjectService_IsNotExists, key);
                return result;
            }
            try
            {
                this.EquipmentControlObjectDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 获取规则-控制参数对象设置数据数据。
        /// </summary>
        /// <param name="key">规则-控制参数对象设置数据标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;RuleControlObject&gt;" />,规则-控制参数对象设置数据数据.</returns>
        public MethodReturnResult<EquipmentControlObject> Get(EquipmentControlObjectKey key)
        {
            MethodReturnResult<EquipmentControlObject> result = new MethodReturnResult<EquipmentControlObject>();
            if (!this.EquipmentControlObjectDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.EquipmentControlObjectService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.EquipmentControlObjectDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 获取规则-控制参数对象设置数据数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;RuleControlObject&gt;" />,规则-控制参数对象设置数据数据集合。</returns>
        public MethodReturnResult<IList<EquipmentControlObject>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<EquipmentControlObject>> result = new MethodReturnResult<IList<EquipmentControlObject>>();
            try
            {
                result.Data = this.EquipmentControlObjectDataEngine.Get(cfg);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }
    }
}
