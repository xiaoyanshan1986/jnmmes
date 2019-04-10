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
    /// 实现工步参数管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class RouteStepParameterService : IRouteStepParameterContract
    {
        /// <summary>
        /// 工步参数数据访问读写。
        /// </summary>
        public IRouteStepParameterDataEngine RouteStepParameterDataEngine { get; set; }


        /// <summary>
        /// 添加工步参数。
        /// </summary>
        /// <param name="obj">工步参数数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(RouteStepParameter obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.RouteStepParameterDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.RouteStepParameterService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.RouteStepParameterDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改工步参数。
        /// </summary>
        /// <param name="obj">工步参数数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(RouteStepParameter obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.RouteStepParameterDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.RouteStepParameterService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.RouteStepParameterDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除工步参数。
        /// </summary>
        /// <param name="key">工步参数名称。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(RouteStepParameterKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.RouteStepParameterDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.RouteStepParameterService_IsNotExists, key);
                return result;
            }
            try
            {
                this.RouteStepParameterDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取工步参数数据。
        /// </summary>
        /// <param name="key">工步参数名称.</param>
        /// <returns><see cref="MethodReturnResult&lt;RouteStepParameter&gt;" />,工步参数数据.</returns>
        public MethodReturnResult<RouteStepParameter> Get(RouteStepParameterKey key)
        {
            MethodReturnResult<RouteStepParameter> result = new MethodReturnResult<RouteStepParameter>();
            if (!this.RouteStepParameterDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.RouteStepParameterService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.RouteStepParameterDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取工步参数数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;RouteStepParameter&gt;" />,工步参数数据集合。</returns>
        public MethodReturnResult<IList<RouteStepParameter>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<RouteStepParameter>> result = new MethodReturnResult<IList<RouteStepParameter>>();
            try
            {
                result.Data = this.RouteStepParameterDataEngine.Get(cfg);
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
