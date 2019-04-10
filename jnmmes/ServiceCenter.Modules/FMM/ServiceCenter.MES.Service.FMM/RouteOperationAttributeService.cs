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
    /// 实现工序属性管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class RouteOperationAttributeService : IRouteOperationAttributeContract
    {
        /// <summary>
        /// 工序属性数据访问读写。
        /// </summary>
        public IRouteOperationAttributeDataEngine RouteOperationAttributeDataEngine { get; set; }


        /// <summary>
        /// 添加工序属性。
        /// </summary>
        /// <param name="obj">工序属性数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(RouteOperationAttribute obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.RouteOperationAttributeDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.RouteOperationAttributeService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.RouteOperationAttributeDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改工序属性。
        /// </summary>
        /// <param name="obj">工序属性数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(RouteOperationAttribute obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.RouteOperationAttributeDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.RouteOperationAttributeService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.RouteOperationAttributeDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除工序属性。
        /// </summary>
        /// <param name="key">工序属性名称。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(RouteOperationAttributeKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.RouteOperationAttributeDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.RouteOperationAttributeService_IsNotExists, key);
                return result;
            }
            try
            {
                this.RouteOperationAttributeDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取工序属性数据。
        /// </summary>
        /// <param name="key">工序属性名称.</param>
        /// <returns><see cref="MethodReturnResult&lt;RouteOperationAttribute&gt;" />,工序属性数据.</returns>
        public MethodReturnResult<RouteOperationAttribute> Get(RouteOperationAttributeKey key)
        {
            MethodReturnResult<RouteOperationAttribute> result = new MethodReturnResult<RouteOperationAttribute>();
            if (!this.RouteOperationAttributeDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.RouteOperationAttributeService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.RouteOperationAttributeDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取工序属性数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;RouteOperationAttribute&gt;" />,工序属性数据集合。</returns>
        public MethodReturnResult<IList<RouteOperationAttribute>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<RouteOperationAttribute>> result = new MethodReturnResult<IList<RouteOperationAttribute>>();
            try
            {
                result.Data = this.RouteOperationAttributeDataEngine.Get(cfg);
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
