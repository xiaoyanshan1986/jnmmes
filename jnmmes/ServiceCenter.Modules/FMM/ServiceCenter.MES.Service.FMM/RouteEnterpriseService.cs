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
    /// 实现工艺流程组管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class RouteEnterpriseService : IRouteEnterpriseContract
    {
        /// <summary>
        /// 工艺流程组数据访问读写。
        /// </summary>
        public IRouteEnterpriseDataEngine RouteEnterpriseDataEngine { get; set; }


        /// <summary>
        /// 添加工艺流程组。
        /// </summary>
        /// <param name="obj">工艺流程组数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(RouteEnterprise obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.RouteEnterpriseDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.RouteEnterpriseService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.RouteEnterpriseDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改工艺流程组。
        /// </summary>
        /// <param name="obj">工艺流程组数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(RouteEnterprise obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.RouteEnterpriseDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.RouteEnterpriseService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.RouteEnterpriseDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除工艺流程组。
        /// </summary>
        /// <param name="key">工艺流程组名称。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.RouteEnterpriseDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.RouteEnterpriseService_IsNotExists, key);
                return result;
            }
            try
            {
                this.RouteEnterpriseDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取工艺流程组数据。
        /// </summary>
        /// <param name="key">工艺流程组名称.</param>
        /// <returns><see cref="MethodReturnResult&lt;RouteEnterprise&gt;" />,工艺流程组数据.</returns>
        public MethodReturnResult<RouteEnterprise> Get(string key)
        {
            MethodReturnResult<RouteEnterprise> result = new MethodReturnResult<RouteEnterprise>();
            if (!this.RouteEnterpriseDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.RouteEnterpriseService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.RouteEnterpriseDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取工艺流程组数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;RouteEnterprise&gt;" />,工艺流程组数据集合。</returns>
        public MethodReturnResult<IList<RouteEnterprise>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<RouteEnterprise>> result = new MethodReturnResult<IList<RouteEnterprise>>();
            try
            {
                result.Data = this.RouteEnterpriseDataEngine.Get(cfg);
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
