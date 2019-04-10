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
    /// 实现工序参数管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class RouteOperationParameterService : IRouteOperationParameterContract
    {
        /// <summary>
        /// 工序参数数据访问读写。
        /// </summary>
        public IRouteOperationParameterDataEngine RouteOperationParameterDataEngine { get; set; }


        /// <summary>
        /// 添加工序参数。
        /// </summary>
        /// <param name="obj">工序参数数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(RouteOperationParameter obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.RouteOperationParameterDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.RouteOperationParameterService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.RouteOperationParameterDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改工序参数。
        /// </summary>
        /// <param name="obj">工序参数数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(RouteOperationParameter obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.RouteOperationParameterDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.RouteOperationParameterService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.RouteOperationParameterDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除工序参数。
        /// </summary>
        /// <param name="key">工序参数名称。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(RouteOperationParameterKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.RouteOperationParameterDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.RouteOperationParameterService_IsNotExists, key);
                return result;
            }
            try
            {
                this.RouteOperationParameterDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取工序参数数据。
        /// </summary>
        /// <param name="key">工序参数名称.</param>
        /// <returns><see cref="MethodReturnResult&lt;RouteOperationParameter&gt;" />,工序参数数据.</returns>
        public MethodReturnResult<RouteOperationParameter> Get(RouteOperationParameterKey key)
        {
            MethodReturnResult<RouteOperationParameter> result = new MethodReturnResult<RouteOperationParameter>();
            if (!this.RouteOperationParameterDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.RouteOperationParameterService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.RouteOperationParameterDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取工序参数数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;RouteOperationParameter&gt;" />,工序参数数据集合。</returns>
        public MethodReturnResult<IList<RouteOperationParameter>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<RouteOperationParameter>> result = new MethodReturnResult<IList<RouteOperationParameter>>();
            try
            {
                result.Data = this.RouteOperationParameterDataEngine.Get(cfg);
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
