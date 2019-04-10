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
    /// 实现检验数据管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class CheckDataService : ICheckDataContract
    {
        /// <summary>
        /// 检验数据数据访问读写。
        /// </summary>
        public ICheckDataDataEngine CheckDataDataEngine { get; set; }
        /// <summary>
        /// 检验数据明细数据访问读写。
        /// </summary>
        public ICheckDataDetailDataEngine CheckDataDetailDataEngine { get; set; }

        /// <summary>
        /// 添加检验数据。
        /// </summary>
        /// <param name="obj">检验数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(CheckData obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.CheckDataDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.CheckDataService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.CheckDataDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改检验数据。
        /// </summary>
        /// <param name="obj">检验数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(CheckData obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.CheckDataDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.CheckDataService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.CheckDataDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除检验数据。
        /// </summary>
        /// <param name="key">检验数据标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.CheckDataDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.CheckDataService_IsNotExists, key);
                return result;
            }
            try
            {
                this.CheckDataDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取检验数据数据。
        /// </summary>
        /// <param name="key">检验数据标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;CheckData&gt;" />,检验数据数据.</returns>
        public MethodReturnResult<CheckData> Get(string key)
        {
            MethodReturnResult<CheckData> result = new MethodReturnResult<CheckData>();
            if (!this.CheckDataDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.CheckDataService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.CheckDataDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取检验数据数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;CheckData&gt;" />,检验数据数据集合。</returns>
        public MethodReturnResult<IList<CheckData>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<CheckData>> result = new MethodReturnResult<IList<CheckData>>();
            try
            {
                result.Data = this.CheckDataDataEngine.Get(cfg);
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
