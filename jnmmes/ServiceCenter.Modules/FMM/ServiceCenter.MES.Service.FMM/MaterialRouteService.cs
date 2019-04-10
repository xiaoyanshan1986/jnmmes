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
    /// 实现产品工艺流程管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class MaterialRouteService : IMaterialRouteContract
    {
        /// <summary>
        /// 产品工艺流程数据访问读写。
        /// </summary>
        public IMaterialRouteDataEngine MaterialRouteDataEngine { get; set; }


        /// <summary>
        /// 添加产品工艺流程。
        /// </summary>
        /// <param name="obj">产品工艺流程数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(MaterialRoute obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.MaterialRouteDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.MaterialRouteService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.MaterialRouteDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改产品工艺流程。
        /// </summary>
        /// <param name="obj">产品工艺流程数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(MaterialRoute obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.MaterialRouteDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.MaterialRouteService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.MaterialRouteDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除产品工艺流程。
        /// </summary>
        /// <param name="key">产品工艺流程标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(MaterialRouteKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.MaterialRouteDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.MaterialRouteService_IsNotExists, key);
                return result;
            }
            try
            {
                this.MaterialRouteDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取产品工艺流程数据。
        /// </summary>
        /// <param name="key">产品工艺流程标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;MaterialRoute&gt;" />,产品工艺流程数据.</returns>
        public MethodReturnResult<MaterialRoute> Get(MaterialRouteKey key)
        {
            MethodReturnResult<MaterialRoute> result = new MethodReturnResult<MaterialRoute>();
            if (!this.MaterialRouteDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.MaterialRouteService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.MaterialRouteDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取产品工艺流程数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;MaterialRoute&gt;" />,产品工艺流程数据集合。</returns>
        public MethodReturnResult<IList<MaterialRoute>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<MaterialRoute>> result = new MethodReturnResult<IList<MaterialRoute>>();
            try
            {
                result.Data = this.MaterialRouteDataEngine.Get(cfg);
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
