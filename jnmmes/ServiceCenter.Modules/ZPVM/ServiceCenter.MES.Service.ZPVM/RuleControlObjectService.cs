using ServiceCenter.MES.DataAccess.Interface.ZPVM;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.MES.Service.Contract.ZPVM;
using ServiceCenter.MES.Service.ZPVM.Resources;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.ZPVM
{
    /// <summary>
    /// 实现规则-控制参数对象设置数据管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class RuleControlObjectService : IRuleControlObjectContract
    {
        /// <summary>
        /// 规则-控制参数对象设置数据数据访问读写。
        /// </summary>
        public IRuleControlObjectDataEngine RuleControlObjectDataEngine { get; set; }


        /// <summary>
        /// 添加规则-控制参数对象设置数据。
        /// </summary>
        /// <param name="obj">规则-控制参数对象设置数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(RuleControlObject obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.RuleControlObjectDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.RuleControlObjectService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.RuleControlObjectDataEngine.Insert(obj);
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
        public MethodReturnResult Modify(RuleControlObject obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.RuleControlObjectDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.RuleControlObjectService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.RuleControlObjectDataEngine.Update(obj);
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
        public MethodReturnResult Delete(RuleControlObjectKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.RuleControlObjectDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.RuleControlObjectService_IsNotExists, key);
                return result;
            }
            try
            {
                this.RuleControlObjectDataEngine.Delete(key);
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
        public MethodReturnResult<RuleControlObject> Get(RuleControlObjectKey key)
        {
            MethodReturnResult<RuleControlObject> result = new MethodReturnResult<RuleControlObject>();
            if (!this.RuleControlObjectDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.RuleControlObjectService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.RuleControlObjectDataEngine.Get(key);
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
        public MethodReturnResult<IList<RuleControlObject>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<RuleControlObject>> result = new MethodReturnResult<IList<RuleControlObject>>();
            try
            {
                result.Data = this.RuleControlObjectDataEngine.Get(cfg);
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
