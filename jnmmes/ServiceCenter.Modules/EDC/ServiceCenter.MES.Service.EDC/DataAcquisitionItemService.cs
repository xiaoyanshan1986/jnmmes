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
    /// 实现采集项目管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class DataAcquisitionItemService : IDataAcquisitionItemContract
    {
        /// <summary>
        /// 采集项目数据访问读写。
        /// </summary>
        public IDataAcquisitionItemDataEngine DataAcquisitionItemDataEngine { get; set; } 


        /// <summary>
        /// 添加采集项目。
        /// </summary>
        /// <param name="obj">采集项目数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(DataAcquisitionItem obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.DataAcquisitionItemDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.DataAcquisitionItemService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.DataAcquisitionItemDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改采集项目。
        /// </summary>
        /// <param name="obj">采集项目数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(DataAcquisitionItem obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.DataAcquisitionItemDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.DataAcquisitionItemService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.DataAcquisitionItemDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除采集项目。
        /// </summary>
        /// <param name="key">采集项目标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.DataAcquisitionItemDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.DataAcquisitionItemService_IsNotExists, key);
                return result;
            }
            try
            {
                this.DataAcquisitionItemDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取采集项目数据。
        /// </summary>
        /// <param name="key">采集项目标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;DataAcquisitionItem&gt;" />,采集项目数据.</returns>
        public MethodReturnResult<DataAcquisitionItem> Get(string key)
        {
            MethodReturnResult<DataAcquisitionItem> result = new MethodReturnResult<DataAcquisitionItem>();
            if (!this.DataAcquisitionItemDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.DataAcquisitionItemService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.DataAcquisitionItemDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取采集项目数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;DataAcquisitionItem&gt;" />,采集项目数据集合。</returns>
        public MethodReturnResult<IList<DataAcquisitionItem>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<DataAcquisitionItem>> result = new MethodReturnResult<IList<DataAcquisitionItem>>();
            try
            {
                result.Data = this.DataAcquisitionItemDataEngine.Get(cfg);
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
