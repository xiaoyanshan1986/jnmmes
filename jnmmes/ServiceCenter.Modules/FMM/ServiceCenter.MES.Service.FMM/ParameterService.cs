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
    /// 实现参数管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class ParameterService : IParameterContract
    {
        /// <summary>
        /// 参数数据访问读写。
        /// </summary>
        public IParameterDataEngine ParameterDataEngine { get; set; }


        /// <summary>
        /// 添加参数。
        /// </summary>
        /// <param name="obj">参数数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(Parameter obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.ParameterDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.ParameterService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.ParameterDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改参数。
        /// </summary>
        /// <param name="obj">参数数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(Parameter obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.ParameterDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.ParameterService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.ParameterDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除参数。
        /// </summary>
        /// <param name="key">参数标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.ParameterDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.ParameterService_IsNotExists, key);
                return result;
            }
            try
            {
                this.ParameterDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取参数数据。
        /// </summary>
        /// <param name="key">参数标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;Parameter&gt;" />,参数数据.</returns>
        public MethodReturnResult<Parameter> Get(string key)
        {
            MethodReturnResult<Parameter> result = new MethodReturnResult<Parameter>();
            if (!this.ParameterDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.ParameterService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.ParameterDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取参数数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;Parameter&gt;" />,参数数据集合。</returns>
        public MethodReturnResult<IList<Parameter>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<Parameter>> result = new MethodReturnResult<IList<Parameter>>();
            try
            {
                result.Data = this.ParameterDataEngine.Get(cfg);
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
