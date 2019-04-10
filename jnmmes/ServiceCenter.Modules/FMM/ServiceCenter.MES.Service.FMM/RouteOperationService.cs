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
    /// 实现工序管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class RouteOperationService : IRouteOperationContract
    {
        /// <summary>
        /// 工序数据访问读写。
        /// </summary>
        public IRouteOperationDataEngine RouteOperationDataEngine { get; set; }


        /// <summary>
        /// 添加工序。
        /// </summary>
        /// <param name="obj">工序数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(RouteOperation obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.RouteOperationDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.RouteOperationService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.RouteOperationDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改工序。
        /// </summary>
        /// <param name="obj">工序数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(RouteOperation obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.RouteOperationDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.RouteOperationService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.RouteOperationDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除工序。
        /// </summary>
        /// <param name="key">工序名称。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.RouteOperationDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.RouteOperationService_IsNotExists, key);
                return result;
            }
            try
            {
                this.RouteOperationDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取工序数据。
        /// </summary>
        /// <param name="key">工序名称.</param>
        /// <returns><see cref="MethodReturnResult&lt;RouteOperation&gt;" />,工序数据.</returns>
        public MethodReturnResult<RouteOperation> Get(string key)
        {
            MethodReturnResult<RouteOperation> result = new MethodReturnResult<RouteOperation>();
            if (!this.RouteOperationDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.RouteOperationService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.RouteOperationDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取工序数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;RouteOperation&gt;" />,工序数据集合。</returns>
        public MethodReturnResult<IList<RouteOperation>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<RouteOperation>> result = new MethodReturnResult<IList<RouteOperation>>();
            try
            {
                result.Data = this.RouteOperationDataEngine.Get(cfg);
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
