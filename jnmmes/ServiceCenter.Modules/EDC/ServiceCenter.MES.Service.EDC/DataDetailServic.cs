using ServiceCenter.MES.DataAccess.Interface.EDC;
using ServiceCenter.MES.Model.EDC;
using ServiceCenter.MES.Service.Contract.EDC;
using ServiceCenter.MES.Service.EDC.Resources;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.EDC
{
    /// <summary>
    /// 实现采集数据明细的管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class DataDetailService : IDataDetailContract
    {
        /// <summary>
        /// 采集数据明细数据访问读写。
        /// </summary>
        public IDataDetailDataEngine DataDetailDataEngine { get; set; }


        /// <summary>
        /// 添加采集数据明细。
        /// </summary>
        /// <param name="obj">采集数据明细数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(DataDetail obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.DataDetailDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.DataDetailService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.DataDetailDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改采集数据明细。
        /// </summary>
        /// <param name="obj">采集数据明细数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(DataDetail obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.DataDetailDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.DataDetailService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.DataDetailDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除采集数据明细。
        /// </summary>
        /// <param name="key">采集数据明细标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(DataDetailKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.DataDetailDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.DataDetailService_IsNotExists, key);
                return result;
            }
            try
            {
                this.DataDetailDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取采集数据明细数据。
        /// </summary>
        /// <param name="key">采集数据明细标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;Data&gt;" />,采集数据明细数据.</returns>
        public MethodReturnResult<DataDetail> Get(DataDetailKey key)
        {
            MethodReturnResult<DataDetail> result = new MethodReturnResult<DataDetail>();
            if (!this.DataDetailDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.DataDetailService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.DataDetailDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取采集数据明细数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;Data&gt;" />,采集数据明细数据集合。</returns>
        public MethodReturnResult<IList<DataDetail>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<DataDetail>> result = new MethodReturnResult<IList<DataDetail>>();
            try
            {
                result.Data = this.DataDetailDataEngine.Get(cfg);
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
