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
    /// 实现批次定时作业数据管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class LotJobService : ILotJobContract
    {
        /// <summary>
        /// 批次定时作业数据数据访问读写。
        /// </summary>
        public ILotJobDataEngine LotJobDataEngine { get; set; }


        /// <summary>
        /// 添加批次定时作业数据。
        /// </summary>
        /// <param name="obj">批次定时作业数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(LotJob obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.LotJobDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.LotJobService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.LotJobDataEngine.Insert(obj);
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
        /// 修改批次定时作业数据。
        /// </summary>
        /// <param name="obj">批次定时作业数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(LotJob obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.LotJobDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.LotJobService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.LotJobDataEngine.Update(obj);
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
        /// 删除批次定时作业数据。
        /// </summary>
        /// <param name="key">批次定时作业数据标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.LotJobDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.LotJobService_IsNotExists, key);
                return result;
            }
            try
            {
                this.LotJobDataEngine.Delete(key);
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
        /// 获取批次定时作业数据数据。
        /// </summary>
        /// <param name="key">批次定时作业数据标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;LotJob&gt;" />,批次定时作业数据数据.</returns>
        public MethodReturnResult<LotJob> Get(string key)
        {
            MethodReturnResult<LotJob> result = new MethodReturnResult<LotJob>();
            if (!this.LotJobDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.LotJobService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.LotJobDataEngine.Get(key);
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
        /// 获取批次定时作业数据数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;LotJob&gt;" />,批次定时作业数据数据集合。</returns>
        public MethodReturnResult<IList<LotJob>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<LotJob>> result = new MethodReturnResult<IList<LotJob>>();
            try
            {
                result.Data = this.LotJobDataEngine.Get(cfg);
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
