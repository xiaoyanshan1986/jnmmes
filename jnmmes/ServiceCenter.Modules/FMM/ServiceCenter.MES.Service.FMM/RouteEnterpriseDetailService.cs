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
    /// 实现工艺流程组明细管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class RouteEnterpriseDetailService : IRouteEnterpriseDetailContract
    {
        /// <summary>
        /// 工艺流程组明细数据访问读写。
        /// </summary>
        public IRouteEnterpriseDetailDataEngine RouteEnterpriseDetailDataEngine { get; set; }


        /// <summary>
        /// 添加工艺流程组明细。
        /// </summary>
        /// <param name="obj">工艺流程组明细数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(RouteEnterpriseDetail obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.RouteEnterpriseDetailDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.RouteEnterpriseDetailService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.RouteEnterpriseDetailDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改工艺流程组明细。
        /// </summary>
        /// <param name="obj">工艺流程组明细数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(RouteEnterpriseDetail obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.RouteEnterpriseDetailDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.RouteEnterpriseDetailService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.RouteEnterpriseDetailDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除工艺流程组明细。
        /// </summary>
        /// <param name="key">工艺流程组明细名称。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(RouteEnterpriseDetailKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.RouteEnterpriseDetailDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.RouteEnterpriseDetailService_IsNotExists, key);
                return result;
            }
            try
            {
                this.RouteEnterpriseDetailDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取工艺流程组明细数据。
        /// </summary>
        /// <param name="key">工艺流程组明细名称.</param>
        /// <returns><see cref="MethodReturnResult&lt;RouteEnterpriseDetail&gt;" />,工艺流程组明细数据.</returns>
        public MethodReturnResult<RouteEnterpriseDetail> Get(RouteEnterpriseDetailKey key)
        {
            MethodReturnResult<RouteEnterpriseDetail> result = new MethodReturnResult<RouteEnterpriseDetail>();
            if (!this.RouteEnterpriseDetailDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.RouteEnterpriseDetailService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.RouteEnterpriseDetailDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取工艺流程组明细数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;RouteEnterpriseDetail&gt;" />,工艺流程组明细数据集合。</returns>
        public MethodReturnResult<IList<RouteEnterpriseDetail>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<RouteEnterpriseDetail>> result = new MethodReturnResult<IList<RouteEnterpriseDetail>>();
            try
            {
                result.Data = this.RouteEnterpriseDetailDataEngine.Get(cfg);
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
