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
    /// 实现工步属性管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class RouteStepAttributeService : IRouteStepAttributeContract
    {
        /// <summary>
        /// 工步属性数据访问读写。
        /// </summary>
        public IRouteStepAttributeDataEngine RouteStepAttributeDataEngine { get; set; }


        /// <summary>
        /// 添加工步属性。
        /// </summary>
        /// <param name="obj">工步属性数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(RouteStepAttribute obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.RouteStepAttributeDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.RouteStepAttributeService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.RouteStepAttributeDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改工步属性。
        /// </summary>
        /// <param name="obj">工步属性数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(RouteStepAttribute obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.RouteStepAttributeDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.RouteStepAttributeService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.RouteStepAttributeDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除工步属性。
        /// </summary>
        /// <param name="key">工步属性名称。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(RouteStepAttributeKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.RouteStepAttributeDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.RouteStepAttributeService_IsNotExists, key);
                return result;
            }
            try
            {
                this.RouteStepAttributeDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取工步属性数据。
        /// </summary>
        /// <param name="key">工步属性名称.</param>
        /// <returns><see cref="MethodReturnResult&lt;RouteStepAttribute&gt;" />,工步属性数据.</returns>
        public MethodReturnResult<RouteStepAttribute> Get(RouteStepAttributeKey key)
        {
            MethodReturnResult<RouteStepAttribute> result = new MethodReturnResult<RouteStepAttribute>();
            if (!this.RouteStepAttributeDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.RouteStepAttributeService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.RouteStepAttributeDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取工步属性数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;RouteStepAttribute&gt;" />,工步属性数据集合。</returns>
        public MethodReturnResult<IList<RouteStepAttribute>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<RouteStepAttribute>> result = new MethodReturnResult<IList<RouteStepAttribute>>();
            try
            {
                result.Data = this.RouteStepAttributeDataEngine.Get(cfg);
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
