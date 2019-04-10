using ServiceCenter.MES.DataAccess.Interface.ZPVM;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.MES.Service.Contract.ZPVM;
using ServiceCenter.MES.Service.ZPVM.Resources;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.ZPVM
{
    /// <summary>
    /// 实现子分档数据管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class PowersetDetailService : IPowersetDetailContract
    {
        /// <summary>
        /// 子分档数据数据访问读写。
        /// </summary>
        public IPowersetDetailDataEngine PowersetDetailDataEngine { get; set; }


        /// <summary>
        /// 添加子分档数据。
        /// </summary>
        /// <param name="obj">子分档数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(PowersetDetail obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.PowersetDetailDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.PowersetDetailService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.PowersetDetailDataEngine.Insert(obj);
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
        /// 修改子分档数据。
        /// </summary>
        /// <param name="obj">子分档数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(PowersetDetail obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.PowersetDetailDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.PowersetDetailService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.PowersetDetailDataEngine.Update(obj);
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
        /// 删除子分档数据。
        /// </summary>
        /// <param name="key">子分档数据标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(PowersetDetailKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.PowersetDetailDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.PowersetDetailService_IsNotExists, key);
                return result;
            }
            try
            {
                this.PowersetDetailDataEngine.Delete(key);
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
        /// 获取子分档数据数据。
        /// </summary>
        /// <param name="key">子分档数据标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;PowersetDetail&gt;" />,子分档数据数据.</returns>
        public MethodReturnResult<PowersetDetail> Get(PowersetDetailKey key)
        {
            MethodReturnResult<PowersetDetail> result = new MethodReturnResult<PowersetDetail>();
            if (!this.PowersetDetailDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.PowersetDetailService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.PowersetDetailDataEngine.Get(key);
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
        /// 获取子分档数据数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;PowersetDetail&gt;" />,子分档数据数据集合。</returns>
        public MethodReturnResult<IList<PowersetDetail>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<PowersetDetail>> result = new MethodReturnResult<IList<PowersetDetail>>();
            try
            {
                result.Data = this.PowersetDetailDataEngine.Get(cfg);
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
