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
    /// 实现物料属性管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class MaterialAttributeService : IMaterialAttributeContract
    {
        /// <summary>
        /// 物料属性数据访问读写。
        /// </summary>
        public IMaterialAttributeDataEngine MaterialAttributeDataEngine { get; set; }


        /// <summary>
        /// 添加物料属性。
        /// </summary>
        /// <param name="obj">物料属性数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(MaterialAttribute obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.MaterialAttributeDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.MaterialAttributeService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.MaterialAttributeDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改物料属性。
        /// </summary>
        /// <param name="obj">物料属性数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(MaterialAttribute obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.MaterialAttributeDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.MaterialAttributeService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.MaterialAttributeDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除物料属性。
        /// </summary>
        /// <param name="key">物料属性标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(MaterialAttributeKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.MaterialAttributeDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.MaterialAttributeService_IsNotExists, key);
                return result;
            }
            try
            {
                this.MaterialAttributeDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取物料属性数据。
        /// </summary>
        /// <param name="key">物料属性标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;MaterialAttribute&gt;" />,物料属性数据.</returns>
        public MethodReturnResult<MaterialAttribute> Get(MaterialAttributeKey key)
        {
            MethodReturnResult<MaterialAttribute> result = new MethodReturnResult<MaterialAttribute>();
            if (!this.MaterialAttributeDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.MaterialAttributeService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.MaterialAttributeDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取物料属性数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;MaterialAttribute&gt;" />,物料属性数据集合。</returns>
        public MethodReturnResult<IList<MaterialAttribute>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<MaterialAttribute>> result = new MethodReturnResult<IList<MaterialAttribute>>();
            try
            {
                result.Data = this.MaterialAttributeDataEngine.Get(cfg);
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
