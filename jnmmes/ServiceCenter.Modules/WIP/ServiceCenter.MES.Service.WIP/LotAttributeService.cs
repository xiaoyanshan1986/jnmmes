using ServiceCenter.MES.DataAccess.Interface.WIP;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Service.Contract.WIP;
using ServiceCenter.MES.Service.WIP.Resources;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.WIP
{
    /// <summary>
    /// 实现批次属性数据管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class LotAttributeService : ILotAttributeContract
    {
        /// <summary>
        /// 批次属性数据访问读写。
        /// </summary>
        public ILotAttributeDataEngine LotAttributeDataEngine { get; set; }

        /// <summary>
        /// 添加批次属性数据。
        /// </summary>
        /// <param name="obj">批次属性数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(LotAttribute obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.LotAttributeDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.LotAttributeService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.LotAttributeDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error,ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }


        /// <summary>
        /// 修改批次属性数据。如果不存在则新增。
        /// </summary>
        /// <param name="obj">批次属性数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(LotAttribute obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                this.LotAttributeDataEngine.Modify(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }
        /// <summary>
        /// 删除批次属性数据。
        /// </summary>
        /// <param name="key">批次属性数据标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(LotAttributeKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.LotAttributeDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.LotAttributeService_IsNotExists, key);
                return result;
            }
            try
            {
                this.LotAttributeDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 获取批次属性数据。
        /// </summary>
        /// <param name="key">批次属性数据标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;LotAttribute&gt;" />,批次属性数据.</returns>
        public MethodReturnResult<LotAttribute> Get(LotAttributeKey key)
        {
            MethodReturnResult<LotAttribute> result = new MethodReturnResult<LotAttribute>();
            if (!this.LotAttributeDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.LotAttributeService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.LotAttributeDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 获取批次属性数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;LotAttribute&gt;" />,批次属性数据集合。</returns>
        public MethodReturnResult<IList<LotAttribute>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<LotAttribute>> result = new MethodReturnResult<IList<LotAttribute>>();
            try
            {
                result.Data = this.LotAttributeDataEngine.Get(cfg);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }
    }
}
