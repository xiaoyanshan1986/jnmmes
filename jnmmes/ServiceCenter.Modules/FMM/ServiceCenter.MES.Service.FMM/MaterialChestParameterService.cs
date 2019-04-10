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
    /// 实现产品编码成柜参数管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class MaterialChestParameterService : IMaterialChestParameterContract
    {
        /// <summary>
        /// 产品编码成柜参数数据访问读写。
        /// </summary>
        public IMaterialChestParameterDataEngine MaterialChestParameterDataEngine { get; set; }


        /// <summary>
        /// 添加产品编码成柜参数。
        /// </summary>
        /// <param name="obj">产品编码成柜参数数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(MaterialChestParameter obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.MaterialChestParameterDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.MaterialChestParameterService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.MaterialChestParameterDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改产品编码成柜参数。
        /// </summary>
        /// <param name="obj">产品编码成柜参数数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(MaterialChestParameter obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.MaterialChestParameterDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.MaterialChestParameterService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.MaterialChestParameterDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除产品编码成柜参数。
        /// </summary>
        /// <param name="key">产品编码成柜参数标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.MaterialChestParameterDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.MaterialChestParameterService_IsNotExists, key);
                return result;
            }
            try
            {
                this.MaterialChestParameterDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取产品编码成柜参数数据。
        /// </summary>
        /// <param name="key">产品编码成柜参数标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;MaterialChestParameter&gt;" />,产品编码成柜参数数据.</returns>
        public MethodReturnResult<MaterialChestParameter> Get(string key)
        {
            MethodReturnResult<MaterialChestParameter> result = new MethodReturnResult<MaterialChestParameter>();
            if (!this.MaterialChestParameterDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.MaterialChestParameterService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.MaterialChestParameterDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取产品编码成柜参数数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;MaterialChestParameter&gt;" />,产品编码成柜参数数据集合。</returns>
        public MethodReturnResult<IList<MaterialChestParameter>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<MaterialChestParameter>> result = new MethodReturnResult<IList<MaterialChestParameter>>();
            try
            {
                result.Data = this.MaterialChestParameterDataEngine.Get(cfg);
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
