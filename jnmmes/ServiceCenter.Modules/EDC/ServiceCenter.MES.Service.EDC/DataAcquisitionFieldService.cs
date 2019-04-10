using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.DataAccess.Interface.EDC;
using ServiceCenter.MES.Model.EDC;
using ServiceCenter.MES.Service.Contract.EDC;
using ServiceCenter.MES.Service.EDC.Resources;
using ServiceCenter.Model;
using System.ServiceModel.Activation;

namespace ServiceCenter.MES.Service.EDC
{
    /// <summary>
    /// 实现采集项目字段管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class DataAcquisitionFieldService : IDataAcquisitionFieldContract
    {
        /// <summary>
        /// 采集字段数据访问读写。
        /// </summary>
        /// <value>The DataAcquisitionField data engine.</value>
        public IDataAcquisitionFieldDataEngine DataAcquisitionFieldDataEngine { get; set; }


        /// <summary>
        /// 添加采集字段。
        /// </summary>
        /// <param name="obj">采集字段数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(DataAcquisitionField obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.DataAcquisitionFieldDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.DataAcquisitionFieldService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.DataAcquisitionFieldDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 修改采集字段。
        /// </summary>
        /// <param name="obj">采集字段数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(DataAcquisitionField obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.DataAcquisitionFieldDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.DataAcquisitionFieldService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.DataAcquisitionFieldDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 删除采集字段。
        /// </summary>
        /// <param name="key">采集字段标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(DataAcquisitionFieldKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.DataAcquisitionFieldDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.DataAcquisitionFieldService_IsNotExists, key);
                return result;
            }
            try
            {
                this.DataAcquisitionFieldDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取采集字段数据。
        /// </summary>
        /// <param name="key">采集字段标识符。</param>
        /// <returns><see cref="MethodReturnResult&lt;DataAcquisitionField&gt;" />,采集字段数据.</returns>
        public MethodReturnResult<DataAcquisitionField> Get(DataAcquisitionFieldKey key)
        {
            MethodReturnResult<DataAcquisitionField> result = new MethodReturnResult<DataAcquisitionField>();
            if (!this.DataAcquisitionFieldDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.DataAcquisitionFieldService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.DataAcquisitionFieldDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取采集字段数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;DataAcquisitionField&gt;" />,采集字段数据集合。</returns>
        public MethodReturnResult<IList<DataAcquisitionField>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<DataAcquisitionField>> result = new MethodReturnResult<IList<DataAcquisitionField>>();
            try
            {
                result.Data = this.DataAcquisitionFieldDataEngine.Get(cfg);
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
