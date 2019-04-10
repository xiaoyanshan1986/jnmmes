using ServiceCenter.MES.DataAccess.Interface.FMM;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.MES.Service.Contract.FMM;
using ServiceCenter.MES.Service.FMM.Resources;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.FMM
{
    /// <summary>
    /// 实现线边仓管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class LineStoreService : ILineStoreContract
    {
        /// <summary>
        /// 线边仓数据访问读写。
        /// </summary>
        public ILineStoreDataEngine LineStoreDataEngine { get; set; }


        /// <summary>
        /// 添加线边仓。
        /// </summary>
        /// <param name="obj">线边仓数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(LineStore obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.LineStoreDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.LineStoreService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.LineStoreDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改线边仓。
        /// </summary>
        /// <param name="obj">线边仓数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(LineStore obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.LineStoreDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.LineStoreService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.LineStoreDataEngine.Update(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除线边仓。
        /// </summary>
        /// <param name="key">线边仓标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(string key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.LineStoreDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.LineStoreService_IsNotExists, key);
                return result;
            }
            try
            {
                this.LineStoreDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取线边仓数据。
        /// </summary>
        /// <param name="key">线边仓标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;LineStore&gt;" />,线边仓数据.</returns>
        public MethodReturnResult<LineStore> Get(string key)
        {
            MethodReturnResult<LineStore> result = new MethodReturnResult<LineStore>();
            if (!this.LineStoreDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.LineStoreService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.LineStoreDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取线边仓数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;LineStore&gt;" />,线边仓数据集合。</returns>
        public MethodReturnResult<IList<LineStore>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<LineStore>> result = new MethodReturnResult<IList<LineStore>>();
            try
            {
                result.Data = this.LineStoreDataEngine.Get(cfg);
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
