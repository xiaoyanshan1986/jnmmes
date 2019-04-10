using NHibernate;
using ServiceCenter.MES.DataAccess.Interface.PPM;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.MES.Service.Contract.PPM;
using ServiceCenter.MES.Service.PPM.Resources;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.PPM
{
    /// <summary>
    /// 实现日目标参数管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class TargetParameterService : ITargetParameterContract
    {
        /// <summary>
        /// 日目标参数数据访问读写。
        /// </summary>
        public ITargetParameterDataEngine TargetParameterDataEngine { get; set; }

        /// <summary>
        /// 添加日目标参数。
        /// </summary>
        /// <param name="obj">日目标参数数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(TargetParameter obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.TargetParameterDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.TargetParameterService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.TargetParameterDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        
        /// <summary>
        /// 修改日目标参数。
        /// </summary>
        /// <param name="obj">日目标参数数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(TargetParameter obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                this.TargetParameterDataEngine.Modify(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 修改日目标参数(列表方式)
        /// </summary>
        /// <param name="lst">计划列表</param>
        /// <returns></returns>
        public MethodReturnResult Modify(IList<TargetParameter> lst)
        {
            MethodReturnResult result = new MethodReturnResult();
            ISession db = null;
            ITransaction transaction = null;
            try
            {
                db = this.TargetParameterDataEngine.SessionFactory.OpenSession();
                transaction = db.BeginTransaction();         
                foreach (TargetParameter obj in lst)
                {
                    this.TargetParameterDataEngine.Modify(obj,db);
                }
                transaction.Commit();
                db.Close();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                db.Close();
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 删除日目标参数。
        /// </summary>
        /// <param name="key">日目标参数标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(TargetParameterKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.TargetParameterDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.TargetParameterService_IsNotExists, key);
                return result;
            }
            try
            {
                this.TargetParameterDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取日目标参数数据。
        /// </summary>
        /// <param name="key">日目标参数标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;TargetParameter&gt;" />,日目标参数数据.</returns>
        public MethodReturnResult<TargetParameter> Get(TargetParameterKey key)
        {
            MethodReturnResult<TargetParameter> result = new MethodReturnResult<TargetParameter>();
            if (!this.TargetParameterDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.TargetParameterService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.TargetParameterDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取日目标参数数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;TargetParameter&gt;" />,日目标参数数据集合。</returns>
        public MethodReturnResult<IList<TargetParameter>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<TargetParameter>> result = new MethodReturnResult<IList<TargetParameter>>();
            try
            {
                result.Data = this.TargetParameterDataEngine.Get(cfg);
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
