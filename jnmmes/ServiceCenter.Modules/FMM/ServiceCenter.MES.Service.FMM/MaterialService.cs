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
    /// 实现物料管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class MaterialService : IMaterialContract
    {
        /// <summary>
        /// 物料数据访问读写。
        /// </summary>
        public IMaterialDataEngine MaterialDataEngine { get; set; }


        /// <summary>
        /// 添加物料。
        /// </summary>
        /// <param name="obj">物料数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(Material obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.MaterialDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.MaterialService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.MaterialDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改物料。
        /// </summary>
        /// <param name="obj">物料数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(Material obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.MaterialDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.MaterialService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.MaterialDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除物料。
        /// </summary>
        /// <param name="key">物料标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.MaterialDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.MaterialService_IsNotExists, key);
                return result;
            }
            try
            {
                this.MaterialDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取物料数据。
        /// </summary>
        /// <param name="key">物料标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;Material&gt;" />,物料数据.</returns>
        public MethodReturnResult<Material> Get(string key)
        {
            MethodReturnResult<Material> result = new MethodReturnResult<Material>();
            if (!this.MaterialDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.MaterialService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.MaterialDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取物料数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;Material&gt;" />,物料数据集合。</returns>
        public MethodReturnResult<IList<Material>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<Material>> result = new MethodReturnResult<IList<Material>>();
            try
            {
                result.Data = this.MaterialDataEngine.Get(cfg);
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
