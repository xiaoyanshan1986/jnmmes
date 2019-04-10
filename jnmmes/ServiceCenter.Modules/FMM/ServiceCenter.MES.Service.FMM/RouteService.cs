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
    /// 实现工艺流程管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class RouteService : IRouteContract
    {
        /// <summary>
        /// 工艺流程数据访问读写。
        /// </summary>
        public IRouteDataEngine RouteDataEngine { get; set; }


        /// <summary>
        /// 添加工艺流程。
        /// </summary>
        /// <param name="obj">工艺流程数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(Route obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.RouteDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.RouteService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.RouteDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改工艺流程。
        /// </summary>
        /// <param name="obj">工艺流程数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(Route obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.RouteDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.RouteService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.RouteDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除工艺流程。
        /// </summary>
        /// <param name="key">工艺流程名称。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.RouteDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.RouteService_IsNotExists, key);
                return result;
            }
            try
            {
                this.RouteDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取工艺流程数据。
        /// </summary>
        /// <param name="key">工艺流程名称.</param>
        /// <returns><see cref="MethodReturnResult&lt;Route&gt;" />,工艺流程数据.</returns>
        public MethodReturnResult<Route> Get(string key)
        {
            MethodReturnResult<Route> result = new MethodReturnResult<Route>();
            if (!this.RouteDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.RouteService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.RouteDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取工艺流程数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;Route&gt;" />,工艺流程数据集合。</returns>
        public MethodReturnResult<IList<Route>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<Route>> result = new MethodReturnResult<IList<Route>>();
            try
            {
                result.Data = this.RouteDataEngine.Get(cfg);
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
