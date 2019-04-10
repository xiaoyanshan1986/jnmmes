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
    /// 实现采集数据管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class DataService : IDataContract
    {
        /// <summary>
        /// 采集数据数据访问读写。
        /// </summary>
        public IDataDataEngine DataDataEngine { get; set; }
        /// <summary>
        /// 采集数据明细数据访问读写。
        /// </summary>
        public IDataDetailDataEngine DataDetailDataEngine { get; set; }

        /// <summary>
        /// 添加采集数据。
        /// </summary>
        /// <param name="obj">采集数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(Data obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.DataDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.DataService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.DataDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改采集数据。
        /// </summary>
        /// <param name="obj">采集数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(Data obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.DataDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.DataService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.DataDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除采集数据。
        /// </summary>
        /// <param name="key">采集数据标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.DataDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.DataService_IsNotExists, key);
                return result;
            }
            try
            {
                this.DataDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取采集数据数据。
        /// </summary>
        /// <param name="key">采集数据标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;Data&gt;" />,采集数据数据.</returns>
        public MethodReturnResult<Data> Get(string key)
        {
            MethodReturnResult<Data> result = new MethodReturnResult<Data>();
            if (!this.DataDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.DataService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.DataDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取采集数据数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;Data&gt;" />,采集数据数据集合。</returns>
        public MethodReturnResult<IList<Data>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<Data>> result = new MethodReturnResult<IList<Data>>();
            try
            {
                result.Data = this.DataDataEngine.Get(cfg);
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
