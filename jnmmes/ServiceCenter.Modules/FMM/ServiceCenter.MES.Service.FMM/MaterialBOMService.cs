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
    /// 实现物料BOM管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class MaterialBOMService : IMaterialBOMContract
    {
        /// <summary>
        /// 物料BOM数据访问读写。
        /// </summary>
        public IMaterialBOMDataEngine MaterialBOMDataEngine { get; set; }
        /// <summary>
        /// 物料数据访问读写。
        /// </summary>
        public IMaterialDataEngine MaterialDataEngine { get; set; }

        /// <summary>
        /// 添加物料BOM。
        /// </summary>
        /// <param name="obj">物料BOM数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(MaterialBOM obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                if (this.MaterialBOMDataEngine.IsExists(obj.Key))
                {
                    result.Code = 1001;
                    result.Message = String.Format(StringResource.MaterialBOMService_IsExists, obj.Key);
                    return result;
                }
                Material mat = this.MaterialDataEngine.Get(obj.RawMaterialCode);
                if (mat==null || mat.IsRaw==false)
                {
                    result.Code = 1003;
                    result.Message = String.Format(StringResource.MaterialService_IsNotExists, obj.RawMaterialCode);
                    return result;
                }

                this.MaterialBOMDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改物料BOM。
        /// </summary>
        /// <param name="obj">物料BOM数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(MaterialBOM obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            
            try
            {
                if (!this.MaterialBOMDataEngine.IsExists(obj.Key))
                {
                    result.Code = 1002;
                    result.Message = String.Format(StringResource.MaterialBOMService_IsNotExists, obj.Key);
                    return result;
                }
                Material mat = this.MaterialDataEngine.Get(obj.RawMaterialCode);
                if (mat == null || mat.IsRaw == false)
                {
                    result.Code = 1003;
                    result.Message = String.Format(StringResource.MaterialService_IsNotExists, obj.RawMaterialCode);
                    return result;
                }
                this.MaterialBOMDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除物料BOM。
        /// </summary>
        /// <param name="key">物料BOM标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(MaterialBOMKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.MaterialBOMDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.MaterialBOMService_IsNotExists, key);
                return result;
            }
            try
            {
                this.MaterialBOMDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取物料BOM数据。
        /// </summary>
        /// <param name="key">物料BOM标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;MaterialBOM&gt;" />,物料BOM数据.</returns>
        public MethodReturnResult<MaterialBOM> Get(MaterialBOMKey key)
        {
            MethodReturnResult<MaterialBOM> result = new MethodReturnResult<MaterialBOM>();
            if (!this.MaterialBOMDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.MaterialBOMService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.MaterialBOMDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取物料BOM数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;MaterialBOM&gt;" />,物料BOM数据集合。</returns>
        public MethodReturnResult<IList<MaterialBOM>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<MaterialBOM>> result = new MethodReturnResult<IList<MaterialBOM>>();
            try
            {
                result.Data = this.MaterialBOMDataEngine.Get(cfg);
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
