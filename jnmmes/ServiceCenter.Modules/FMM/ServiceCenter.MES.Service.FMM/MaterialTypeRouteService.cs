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
    /// 实现物料类型工艺流程管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class MaterialTypeRouteService : IMaterialTypeRouteContract
    {
        /// <summary>
        /// 物料类型工艺流程数据访问读写。
        /// </summary>
        public IMaterialTypeRouteDataEngine MaterialTypeRouteDataEngine { get; set; }


        /// <summary>
        /// 添加物料类型工艺流程。
        /// </summary>
        /// <param name="obj">物料类型工艺流程数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(MaterialTypeRoute obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.MaterialTypeRouteDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.MaterialTypeRouteService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.MaterialTypeRouteDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改物料类型工艺流程。
        /// </summary>
        /// <param name="obj">物料类型工艺流程数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(MaterialTypeRoute obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.MaterialTypeRouteDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.MaterialTypeRouteService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.MaterialTypeRouteDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除物料类型工艺流程。
        /// </summary>
        /// <param name="key">物料类型工艺流程标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(MaterialTypeRouteKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.MaterialTypeRouteDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.MaterialTypeRouteService_IsNotExists, key);
                return result;
            }
            try
            {
                this.MaterialTypeRouteDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取物料类型工艺流程数据。
        /// </summary>
        /// <param name="key">物料类型工艺流程标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;MaterialTypeRoute&gt;" />,物料类型工艺流程数据.</returns>
        public MethodReturnResult<MaterialTypeRoute> Get(MaterialTypeRouteKey key)
        {
            MethodReturnResult<MaterialTypeRoute> result = new MethodReturnResult<MaterialTypeRoute>();
            if (!this.MaterialTypeRouteDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.MaterialTypeRouteService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.MaterialTypeRouteDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取物料类型工艺流程数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;MaterialTypeRoute&gt;" />,物料类型工艺流程数据集合。</returns>
        public MethodReturnResult<IList<MaterialTypeRoute>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<MaterialTypeRoute>> result = new MethodReturnResult<IList<MaterialTypeRoute>>();
            try
            {
                result.Data = this.MaterialTypeRouteDataEngine.Get(cfg);
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
