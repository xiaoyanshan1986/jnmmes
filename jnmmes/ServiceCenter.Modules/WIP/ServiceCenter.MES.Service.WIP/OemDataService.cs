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
    /// 实现OEM批次数据管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class OemDataService : IOemDataContract
    {
        /// <summary>
        /// OEM批次数据访问读写。
        /// </summary>
        public IOemDataEngine OemDataEngine { get; set; }

        /// <summary>
        /// 添加OEM批次数据。
        /// </summary>
        /// <param name="obj">OEM批次数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(OemData obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.OemDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.OemDataService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.OemDataEngine.Insert(obj);
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
        /// 修改OEM批次数据。如果不存在则新增。
        /// </summary>
        /// <param name="obj">OEM批次数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(OemData obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                this.OemDataEngine.Modify(obj);
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
        /// 删除OEM批次数据。
        /// </summary>
        /// <param name="key">OEM批次数据标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.OemDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.OemDataService_IsNotExists, key);
                return result;
            }
            try
            {
                this.OemDataEngine.Delete(key);
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
        /// 获取OEM批次数据。
        /// </summary>
        /// <param name="key">OEM批次数据标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;OemData&gt;" />,OEM批次数据.</returns>
        public MethodReturnResult<OemData> Get(string key)
        {
            MethodReturnResult<OemData> result = new MethodReturnResult<OemData>();
            if (!this.OemDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.OemDataService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.OemDataEngine.Get(key);
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
        /// 获取OEM批次数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;OemData&gt;" />,OEM批次数据集合。</returns>
        public MethodReturnResult<IList<OemData>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<OemData>> result = new MethodReturnResult<IList<OemData>>();
            try
            {
                result.Data = this.OemDataEngine.Get(cfg);
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
