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
    /// 实现规则-标签打印设置数据管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class RulePrintSetService : IRulePrintSetContract
    {
        /// <summary>
        /// 规则-标签打印设置数据数据访问读写。
        /// </summary>
        public IRulePrintSetDataEngine RulePrintSetDataEngine { get; set; }


        /// <summary>
        /// 添加规则-标签打印设置数据。
        /// </summary>
        /// <param name="obj">规则-标签打印设置数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(RulePrintSet obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.RulePrintSetDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.RulePrintSetService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.RulePrintSetDataEngine.Insert(obj);
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
        /// 修改规则-标签打印设置数据。
        /// </summary>
        /// <param name="obj">规则-标签打印设置数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(RulePrintSet obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.RulePrintSetDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.RulePrintSetService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.RulePrintSetDataEngine.Update(obj);
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
        /// 删除规则-标签打印设置数据。
        /// </summary>
        /// <param name="key">规则-标签打印设置数据标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(RulePrintSetKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.RulePrintSetDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.RulePrintSetService_IsNotExists, key);
                return result;
            }
            try
            {
                this.RulePrintSetDataEngine.Delete(key);
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
        /// 获取规则-标签打印设置数据数据。
        /// </summary>
        /// <param name="key">规则-标签打印设置数据标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;RulePrintSet&gt;" />,规则-标签打印设置数据数据.</returns>
        public MethodReturnResult<RulePrintSet> Get(RulePrintSetKey key)
        {
            MethodReturnResult<RulePrintSet> result = new MethodReturnResult<RulePrintSet>();
            if (!this.RulePrintSetDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.RulePrintSetService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.RulePrintSetDataEngine.Get(key);
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
        /// 获取规则-标签打印设置数据数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;RulePrintSet&gt;" />,规则-标签打印设置数据数据集合。</returns>
        public MethodReturnResult<IList<RulePrintSet>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<RulePrintSet>> result = new MethodReturnResult<IList<RulePrintSet>>();
            try
            {
                result.Data = this.RulePrintSetDataEngine.Get(cfg);
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
