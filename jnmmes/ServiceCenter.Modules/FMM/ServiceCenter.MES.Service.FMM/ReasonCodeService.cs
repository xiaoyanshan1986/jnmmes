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
    /// 实现代码管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class ReasonCodeService : IReasonCodeContract
    {
        /// <summary>
        /// 代码数据访问读写。
        /// </summary>
        public IReasonCodeDataEngine ReasonCodeDataEngine { get; set; }


        /// <summary>
        /// 添加代码。
        /// </summary>
        /// <param name="obj">代码数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(ReasonCode obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.ReasonCodeDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.ReasonCodeService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.ReasonCodeDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改代码。
        /// </summary>
        /// <param name="obj">代码数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(ReasonCode obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.ReasonCodeDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.ReasonCodeService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.ReasonCodeDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除代码。
        /// </summary>
        /// <param name="key">代码名称。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.ReasonCodeDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.ReasonCodeService_IsNotExists, key);
                return result;
            }
            try
            {
                this.ReasonCodeDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取代码数据。
        /// </summary>
        /// <param name="key">代码名称.</param>
        /// <returns><see cref="MethodReturnResult&lt;ReasonCode&gt;" />,代码数据.</returns>
        public MethodReturnResult<ReasonCode> Get(string key)
        {
            MethodReturnResult<ReasonCode> result = new MethodReturnResult<ReasonCode>();
            if (!this.ReasonCodeDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.ReasonCodeService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.ReasonCodeDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取代码数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;ReasonCode&gt;" />,代码数据集合。</returns>
        public MethodReturnResult<IList<ReasonCode>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<ReasonCode>> result = new MethodReturnResult<IList<ReasonCode>>();
            try
            {
                result.Data = this.ReasonCodeDataEngine.Get(cfg);
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
