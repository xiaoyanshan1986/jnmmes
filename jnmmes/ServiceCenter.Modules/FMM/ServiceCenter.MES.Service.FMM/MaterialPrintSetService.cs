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
    /// 实现产品产品标签设置管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class MaterialPrintSetService : IMaterialPrintSetContract
    {
        /// <summary>
        /// 产品产品标签设置数据访问读写。
        /// </summary>
        public IMaterialPrintSetDataEngine MaterialPrintSetDataEngine { get; set; }
        
        /// <summary>
        /// 添加产品产品标签设置。
        /// </summary>
        /// <param name="obj">产品产品标签设置数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(MaterialPrintSet obj)
        {
            MethodReturnResult result = new MethodReturnResult();

            if (this.MaterialPrintSetDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.MaterialPrintSetService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.MaterialPrintSetDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }
        
        /// <summary>
        /// 修改产品产品标签设置。
        /// </summary>
        /// <param name="obj">产品产品标签设置数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(MaterialPrintSet obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.MaterialPrintSetDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.MaterialPrintSetService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.MaterialPrintSetDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 删除产品产品标签设置。
        /// </summary>
        /// <param name="key">产品产品标签设置标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(MaterialPrintSetKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.MaterialPrintSetDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.MaterialPrintSetService_IsNotExists, key);
                return result;
            }
            try
            {
                this.MaterialPrintSetDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取产品产品标签设置数据。
        /// </summary>
        /// <param name="key">产品产品标签设置标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;MaterialPrintSet&gt;" />,产品产品标签设置数据.</returns>
        public MethodReturnResult<MaterialPrintSet> Get(MaterialPrintSetKey key)
        {
            MethodReturnResult<MaterialPrintSet> result = new MethodReturnResult<MaterialPrintSet>();
            if (!this.MaterialPrintSetDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.MaterialPrintSetService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.MaterialPrintSetDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取产品产品标签设置数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;MaterialPrintSet&gt;" />,产品产品标签设置数据集合。</returns>
        public MethodReturnResult<IList<MaterialPrintSet>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<MaterialPrintSet>> result = new MethodReturnResult<IList<MaterialPrintSet>>();
            try
            {
                result.Data = this.MaterialPrintSetDataEngine.Get(cfg);
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
