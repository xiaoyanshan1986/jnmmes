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
    /// 实现衰减数据管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class DecayService : IDecayContract
    {
        /// <summary>
        /// 衰减数据数据访问读写。
        /// </summary>
        public IDecayDataEngine DecayDataEngine { get; set; }


        /// <summary>
        /// 添加衰减数据。
        /// </summary>
        /// <param name="obj">衰减数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(Decay obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.DecayDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.DecayService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.DecayDataEngine.Insert(obj);
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
        /// 修改衰减数据。
        /// </summary>
        /// <param name="obj">衰减数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(Decay obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.DecayDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.DecayService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.DecayDataEngine.Update(obj);
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
        /// 删除衰减数据。
        /// </summary>
        /// <param name="key">衰减数据标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(DecayKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.DecayDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.DecayService_IsNotExists, key);
                return result;
            }
            try
            {
                this.DecayDataEngine.Delete(key);
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
        /// 获取衰减数据数据。
        /// </summary>
        /// <param name="key">衰减数据标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;Decay&gt;" />,衰减数据数据.</returns>
        public MethodReturnResult<Decay> Get(DecayKey key)
        {
            MethodReturnResult<Decay> result = new MethodReturnResult<Decay>();
            if (!this.DecayDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.DecayService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.DecayDataEngine.Get(key);
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
        /// 获取衰减数据数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;Decay&gt;" />,衰减数据数据集合。</returns>
        public MethodReturnResult<IList<Decay>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<Decay>> result = new MethodReturnResult<IList<Decay>>();
            try
            {
                result.Data = this.DecayDataEngine.Get(cfg);
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
