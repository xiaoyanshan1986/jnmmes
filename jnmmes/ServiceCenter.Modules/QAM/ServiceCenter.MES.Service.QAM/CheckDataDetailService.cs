using ServiceCenter.MES.DataAccess.Interface.QAM;
using ServiceCenter.MES.Model.QAM;
using ServiceCenter.MES.Service.Contract.QAM;
using ServiceCenter.MES.Service.QAM.Resources;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.QAM
{
    /// <summary>
    /// 实现检验数据明细管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class CheckDataDetailService : ICheckDataDetailContract
    {
        /// <summary>
        /// 检验数据明细数据访问读写。
        /// </summary>
        public ICheckDataDetailDataEngine CheckDataDetailDataEngine { get; set; }


        /// <summary>
        /// 添加检验数据明细。
        /// </summary>
        /// <param name="obj">检验数据明细数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(CheckDataDetail obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.CheckDataDetailDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.CheckDataDetailService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.CheckDataDetailDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改检验数据明细。
        /// </summary>
        /// <param name="obj">检验数据明细数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(CheckDataDetail obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.CheckDataDetailDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.CheckDataDetailService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.CheckDataDetailDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除检验数据明细。
        /// </summary>
        /// <param name="key">检验数据明细标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(CheckDataDetailKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.CheckDataDetailDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.CheckDataDetailService_IsNotExists, key);
                return result;
            }
            try
            {
                this.CheckDataDetailDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取检验数据明细数据。
        /// </summary>
        /// <param name="key">检验数据明细标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;CheckDataDetail&gt;" />,检验数据明细数据.</returns>
        public MethodReturnResult<CheckDataDetail> Get(CheckDataDetailKey key)
        {
            MethodReturnResult<CheckDataDetail> result = new MethodReturnResult<CheckDataDetail>();
            if (!this.CheckDataDetailDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.CheckDataDetailService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.CheckDataDetailDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取检验数据明细数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;CheckDataDetail&gt;" />,检验数据明细数据集合。</returns>
        public MethodReturnResult<IList<CheckDataDetail>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<CheckDataDetail>> result = new MethodReturnResult<IList<CheckDataDetail>>();
            try
            {
                result.Data = this.CheckDataDetailDataEngine.Get(cfg);
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
