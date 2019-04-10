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
    /// 实现客户端配置管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class ClientConfigService : IClientConfigContract
    {
        /// <summary>
        /// 客户端配置数据访问读写。
        /// </summary>
        public IClientConfigDataEngine ClientConfigDataEngine { get; set; }


        /// <summary>
        /// 添加客户端配置。
        /// </summary>
        /// <param name="obj">客户端配置数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(ClientConfig obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.ClientConfigDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.ClientConfigService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.ClientConfigDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改客户端配置。
        /// </summary>
        /// <param name="obj">客户端配置数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(ClientConfig obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.ClientConfigDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.ClientConfigService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.ClientConfigDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除客户端配置。
        /// </summary>
        /// <param name="key">客户端配置标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.ClientConfigDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.ClientConfigService_IsNotExists, key);
                return result;
            }
            try
            {
                this.ClientConfigDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取客户端配置数据。
        /// </summary>
        /// <param name="key">客户端配置标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;ClientConfig&gt;" />,客户端配置数据.</returns>
        public MethodReturnResult<ClientConfig> Get(string key)
        {
            MethodReturnResult<ClientConfig> result = new MethodReturnResult<ClientConfig>();
            if (!this.ClientConfigDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.ClientConfigService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.ClientConfigDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取客户端配置数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;ClientConfig&gt;" />,客户端配置数据集合。</returns>
        public MethodReturnResult<IList<ClientConfig>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<ClientConfig>> result = new MethodReturnResult<IList<ClientConfig>>();
            try
            {
                result.Data = this.ClientConfigDataEngine.Get(cfg);
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
