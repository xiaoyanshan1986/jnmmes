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
    /// 实现效率档数据管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class EfficiencyService : IEfficiencyContract
    {
        /// <summary>
        /// 效率档数据数据访问读写。
        /// </summary>
        public IEfficiencyDataEngine EfficiencyDataEngine { get; set; }


        /// <summary>
        /// 添加效率档数据。
        /// </summary>
        /// <param name="obj">效率档数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(Efficiency obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.EfficiencyDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.EfficiencyService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.EfficiencyDataEngine.Insert(obj);
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
        /// 修改效率档数据。
        /// </summary>
        /// <param name="obj">效率档数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(Efficiency obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.EfficiencyDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.EfficiencyService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.EfficiencyDataEngine.Update(obj);
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
        /// 删除效率档数据。
        /// </summary>
        /// <param name="key">效率档数据标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(EfficiencyKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.EfficiencyDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.EfficiencyService_IsNotExists, key);
                return result;
            }
            try
            {
                this.EfficiencyDataEngine.Delete(key);
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
        /// 获取效率档数据数据。
        /// </summary>
        /// <param name="key">效率档数据标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;Efficiency&gt;" />,效率档数据数据.</returns>
        public MethodReturnResult<Efficiency> Get(EfficiencyKey key)
        {
            MethodReturnResult<Efficiency> result = new MethodReturnResult<Efficiency>();
            if (!this.EfficiencyDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.EfficiencyService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.EfficiencyDataEngine.Get(key);
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
        /// 获取效率档数据数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;Efficiency&gt;" />,效率档数据数据集合。</returns>
        public MethodReturnResult<IList<Efficiency>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<Efficiency>> result = new MethodReturnResult<IList<Efficiency>>();
            try
            {
                result.Data = this.EfficiencyDataEngine.Get(cfg);
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
