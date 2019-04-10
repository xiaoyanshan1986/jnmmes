using ServiceCenter.MES.DataAccess.Interface.ZPVC;
using ServiceCenter.MES.Model.ZPVC;
using ServiceCenter.MES.Service.Contract.ZPVC;
using ServiceCenter.MES.Service.ZPVC.Resources;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.ZPVC
{
    /// <summary>
    /// 实现效率档配置数据管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class EfficiencyConfigurationService : IEfficiencyConfigurationContract
    {
        /// <summary>
        /// 效率档配置数据数据访问读写。
        /// </summary>
        public IEfficiencyConfigurationDataEngine EfficiencyConfigurationDataEngine { get; set; }


        /// <summary>
        /// 添加效率档配置数据。
        /// </summary>
        /// <param name="obj">效率档配置数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(EfficiencyConfiguration obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.EfficiencyConfigurationDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.EfficiencyConfigurationService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.EfficiencyConfigurationDataEngine.Insert(obj);
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
        /// 修改效率档配置数据。
        /// </summary>
        /// <param name="obj">效率档配置数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(EfficiencyConfiguration obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.EfficiencyConfigurationDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.EfficiencyConfigurationService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.EfficiencyConfigurationDataEngine.Update(obj);
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
        /// 删除效率档配置数据。
        /// </summary>
        /// <param name="key">效率档配置数据标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(EfficiencyConfigurationKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.EfficiencyConfigurationDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.EfficiencyConfigurationService_IsNotExists, key);
                return result;
            }
            try
            {
                this.EfficiencyConfigurationDataEngine.Delete(key);
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
        /// 获取效率档配置数据数据。
        /// </summary>
        /// <param name="key">效率档配置数据标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;EfficiencyConfiguration&gt;" />,效率档配置数据数据.</returns>
        public MethodReturnResult<EfficiencyConfiguration> Get(EfficiencyConfigurationKey key)
        {
            MethodReturnResult<EfficiencyConfiguration> result = new MethodReturnResult<EfficiencyConfiguration>();
            if (!this.EfficiencyConfigurationDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.EfficiencyConfigurationService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.EfficiencyConfigurationDataEngine.Get(key);
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
        /// 获取效率档配置数据数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;EfficiencyConfiguration&gt;" />,效率档配置数据数据集合。</returns>
        public MethodReturnResult<IList<EfficiencyConfiguration>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<EfficiencyConfiguration>> result = new MethodReturnResult<IList<EfficiencyConfiguration>>();
            try
            {
                result.Data = this.EfficiencyConfigurationDataEngine.Get(cfg);
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
