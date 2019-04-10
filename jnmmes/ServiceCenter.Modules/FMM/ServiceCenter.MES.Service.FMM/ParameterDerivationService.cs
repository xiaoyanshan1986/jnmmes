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
    /// 实现参数推导管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class ParameterDerivationService : IParameterDerivationContract
    {
        /// <summary>
        /// 参数推导数据访问读写。
        /// </summary>
        public IParameterDerivationDataEngine ParameterDerivationDataEngine { get; set; }


        /// <summary>
        /// 添加参数推导。
        /// </summary>
        /// <param name="obj">参数推导数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(ParameterDerivation obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.ParameterDerivationDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.ParameterDerivationService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.ParameterDerivationDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改参数推导。
        /// </summary>
        /// <param name="obj">参数推导数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(ParameterDerivation obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.ParameterDerivationDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.ParameterDerivationService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.ParameterDerivationDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除参数推导。
        /// </summary>
        /// <param name="key">参数推导标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(ParameterDerivationKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.ParameterDerivationDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.ParameterDerivationService_IsNotExists, key);
                return result;
            }
            try
            {
                this.ParameterDerivationDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取参数推导数据。
        /// </summary>
        /// <param name="key">参数推导标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;ParameterDerivation&gt;" />,参数推导数据.</returns>
        public MethodReturnResult<ParameterDerivation> Get(ParameterDerivationKey key)
        {
            MethodReturnResult<ParameterDerivation> result = new MethodReturnResult<ParameterDerivation>();
            if (!this.ParameterDerivationDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.ParameterDerivationService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.ParameterDerivationDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取参数推导数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;ParameterDerivation&gt;" />,参数推导数据集合。</returns>
        public MethodReturnResult<IList<ParameterDerivation>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<ParameterDerivation>> result = new MethodReturnResult<IList<ParameterDerivation>>();
            try
            {
                result.Data = this.ParameterDerivationDataEngine.Get(cfg);
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
