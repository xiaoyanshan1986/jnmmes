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
    /// 实现规则数据管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class RuleService : IRuleContract
    {
        /// <summary>
        /// 规则数据数据访问读写。
        /// </summary>
        public IRuleDataEngine RuleDataEngine { get; set; }
        /// <summary>
        /// 分档数据数据访问读写。
        /// </summary>
        public IPowersetDataEngine PowersetDataEngine { get; set; }


        /// <summary>
        /// 添加规则数据。
        /// </summary>
        /// <param name="obj">规则数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(Rule obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.RuleDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.RuleService_IsExists, obj.Key);
                return result;
            }
            try
            {
                //判断分档代码是否存在。
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo=0,
                    PageSize=1,
                    Where=string.Format("Key.Code='{0}'",obj.PowersetCode)
                };

                IList<Powerset> lstPowerset = this.PowersetDataEngine.Get(cfg);
                if (lstPowerset.Count == 0)
                {
                    result.Code = 1001;
                    result.Message = String.Format(StringResource.PowersetService_IsNotExists, obj.PowersetCode);
                    return result;
                }

                this.RuleDataEngine.Insert(obj);
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
        /// 修改规则数据。
        /// </summary>
        /// <param name="obj">规则数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(Rule obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.RuleDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.RuleService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                //判断分档代码是否存在。
                PagingConfig cfg = new PagingConfig()
                {
                    PageNo = 0,
                    PageSize = 1,
                    Where = string.Format("Key.Code='{0}'", obj.PowersetCode)
                };

                IList<Powerset> lstPowerset = this.PowersetDataEngine.Get(cfg);
                if (lstPowerset.Count == 0)
                {
                    result.Code = 1001;
                    result.Message = String.Format(StringResource.PowersetService_IsNotExists, obj.PowersetCode);
                    return result;
                }

                this.RuleDataEngine.Update(obj);
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
        /// 删除规则数据。
        /// </summary>
        /// <param name="key">规则数据标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.RuleDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.RuleService_IsNotExists, key);
                return result;
            }
            try
            {
                this.RuleDataEngine.Delete(key);
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
        /// 获取规则数据数据。
        /// </summary>
        /// <param name="key">规则数据标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;Rule&gt;" />,规则数据数据.</returns>
        public MethodReturnResult<Rule> Get(string key)
        {
            MethodReturnResult<Rule> result = new MethodReturnResult<Rule>();
            if (!this.RuleDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.RuleService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.RuleDataEngine.Get(key);
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
        /// 获取规则数据数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;Rule&gt;" />,规则数据数据集合。</returns>
        public MethodReturnResult<IList<Rule>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<Rule>> result = new MethodReturnResult<IList<Rule>>();
            try
            {
                result.Data = this.RuleDataEngine.Get(cfg);
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
