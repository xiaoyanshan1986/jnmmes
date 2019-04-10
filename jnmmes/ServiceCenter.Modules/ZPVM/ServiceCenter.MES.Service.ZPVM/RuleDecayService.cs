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
    /// 实现规则-衰减设置数据管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class RuleDecayService : IRuleDecayContract
    {
        /// <summary>
        /// 规则-衰减设置数据数据访问读写。
        /// </summary>
        public IRuleDecayDataEngine RuleDecayDataEngine { get; set; }

        /// <summary>
        /// 衰减数据访问读写。
        /// </summary>
        public IDecayDataEngine DecayDataEngine { get; set; }

        /// <summary>
        /// 添加规则-衰减设置数据。
        /// </summary>
        /// <param name="obj">规则-衰减设置数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(RuleDecay obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.RuleDecayDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.RuleDecayService_IsExists, obj.Key);
                return result;
            }
            try
            {
                //判断衰减代码是否存在。
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key.Code='{0}'", obj.DecayCode)
                };

                IList<Decay> lstPowerset = this.DecayDataEngine.Get(cfg);
                if (lstPowerset.Count == 0)
                {
                    result.Code = 1001;
                    result.Message = String.Format(StringResource.DecayService_IsNotExists, obj.DecayCode);
                    return result;
                }

                this.RuleDecayDataEngine.Insert(obj);
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
        /// 修改规则-衰减设置数据。
        /// </summary>
        /// <param name="obj">规则-衰减设置数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(RuleDecay obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.RuleDecayDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.RuleDecayService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.RuleDecayDataEngine.Update(obj);
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
        /// 删除规则-衰减设置数据。
        /// </summary>
        /// <param name="key">规则-衰减设置数据标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(RuleDecayKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.RuleDecayDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.RuleDecayService_IsNotExists, key);
                return result;
            }
            try
            {
                this.RuleDecayDataEngine.Delete(key);
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
        /// 获取规则-衰减设置数据数据。
        /// </summary>
        /// <param name="key">规则-衰减设置数据标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;RuleDecay&gt;" />,规则-衰减设置数据数据.</returns>
        public MethodReturnResult<RuleDecay> Get(RuleDecayKey key)
        {
            MethodReturnResult<RuleDecay> result = new MethodReturnResult<RuleDecay>();
            if (!this.RuleDecayDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.RuleDecayService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.RuleDecayDataEngine.Get(key);
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
        /// 获取规则-衰减设置数据数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;RuleDecay&gt;" />,规则-衰减设置数据数据集合。</returns>
        public MethodReturnResult<IList<RuleDecay>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<RuleDecay>> result = new MethodReturnResult<IList<RuleDecay>>();
            try
            {
                result.Data = this.RuleDecayDataEngine.Get(cfg);
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
