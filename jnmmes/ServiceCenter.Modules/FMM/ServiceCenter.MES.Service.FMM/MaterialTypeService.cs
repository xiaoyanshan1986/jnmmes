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
    /// 实现物料类型管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class MaterialTypeService : IMaterialTypeContract
    {
        /// <summary>
        /// 物料类型数据访问读写。
        /// </summary>
        public IMaterialTypeDataEngine MaterialTypeDataEngine { get; set; }


        /// <summary>
        /// 添加物料类型。
        /// </summary>
        /// <param name="obj">物料类型数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(MaterialType obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.MaterialTypeDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.MaterialTypeService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.MaterialTypeDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改物料类型。
        /// </summary>
        /// <param name="obj">物料类型数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(MaterialType obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.MaterialTypeDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.MaterialTypeService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.MaterialTypeDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除物料类型。
        /// </summary>
        /// <param name="key">物料类型标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.MaterialTypeDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.MaterialTypeService_IsNotExists, key);
                return result;
            }
            try
            {
                this.MaterialTypeDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取物料类型数据。
        /// </summary>
        /// <param name="key">物料类型标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;MaterialType&gt;" />,物料类型数据.</returns>
        public MethodReturnResult<MaterialType> Get(string key)
        {
            MethodReturnResult<MaterialType> result = new MethodReturnResult<MaterialType>();
            if (!this.MaterialTypeDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.MaterialTypeService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.MaterialTypeDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取物料类型数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;MaterialType&gt;" />,物料类型数据集合。</returns>
        public MethodReturnResult<IList<MaterialType>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<MaterialType>> result = new MethodReturnResult<IList<MaterialType>>();
            try
            {
                result.Data = this.MaterialTypeDataEngine.Get(cfg);
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
