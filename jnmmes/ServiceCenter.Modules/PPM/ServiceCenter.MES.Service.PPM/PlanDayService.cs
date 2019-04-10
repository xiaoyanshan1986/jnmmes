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
    /// 实现日生产计划管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class PlanDayService : IPlanDayContract
    {
        /// <summary>
        /// 日生产计划数据访问读写。
        /// </summary>
        public IPlanDayDataEngine PlanDayDataEngine { get; set; }

        /// <summary>
        /// 添加日生产计划。
        /// </summary>
        /// <param name="obj">日生产计划数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(PlanDay obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.PlanDayDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.PlanDayService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.PlanDayDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        
        /// <summary>
        /// 修改日生产计划。
        /// </summary>
        /// <param name="obj">日生产计划数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(PlanDay obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                this.PlanDayDataEngine.Modify(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 修改日生产计划(列表方式)
        /// </summary>
        /// <param name="lst">计划列表</param>
        /// <returns></returns>
        public MethodReturnResult Modify(IList<PlanDay> lst)
        {
            MethodReturnResult result = new MethodReturnResult();
            ISession db = null;
            ITransaction transaction = null;
            try
            {
                db = this.PlanDayDataEngine.SessionFactory.OpenSession();
                transaction = db.BeginTransaction();         
                foreach (PlanDay obj in lst)
                {
                    this.PlanDayDataEngine.Modify(obj,db);
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
        /// 删除日生产计划。
        /// </summary>
        /// <param name="key">日生产计划标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(PlanDayKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.PlanDayDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.PlanDayService_IsNotExists, key);
                return result;
            }
            try
            {
                this.PlanDayDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取日生产计划数据。
        /// </summary>
        /// <param name="key">日生产计划标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;PlanDay&gt;" />,日生产计划数据.</returns>
        public MethodReturnResult<PlanDay> Get(PlanDayKey key)
        {
            MethodReturnResult<PlanDay> result = new MethodReturnResult<PlanDay>();
            if (!this.PlanDayDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.PlanDayService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.PlanDayDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取日生产计划数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;PlanDay&gt;" />,日生产计划数据集合。</returns>
        public MethodReturnResult<IList<PlanDay>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<PlanDay>> result = new MethodReturnResult<IList<PlanDay>>();
            try
            {
                result.Data = this.PlanDayDataEngine.Get(cfg);
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
