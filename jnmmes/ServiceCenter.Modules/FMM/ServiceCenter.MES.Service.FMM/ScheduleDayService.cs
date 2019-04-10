using NHibernate;
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
    /// 实现日排班计划管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class ScheduleDayService : IScheduleDayContract
    {
        /// <summary>
        /// 日排班计划数据访问读写。
        /// </summary>
        public IScheduleDayDataEngine ScheduleDayDataEngine { get; set; }


        /// <summary>
        /// 添加日排班计划。
        /// </summary>
        /// <param name="obj">日排班计划数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(ScheduleDay obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.ScheduleDayDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.ScheduleDayService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.ScheduleDayDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError,ex.Message);
            }
            return result;
        }


        /// <summary>
        /// 修改日排班计划。
        /// </summary>
        /// <param name="obj">日排班计划数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(ScheduleDay obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                this.ScheduleDayDataEngine.Modify(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        public MethodReturnResult Modify(IList<ScheduleDay> lst)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                //using(System.Transactions.TransactionScope ts=new System.Transactions.TransactionScope())
                ISession session = this.ScheduleDayDataEngine.SessionFactory.OpenSession();
                ITransaction transaction = session.BeginTransaction();
                {
                    foreach(ScheduleDay obj in lst)
                    {
                        this.ScheduleDayDataEngine.Modify(obj);
                    }
                    //ts.Complete();
                    transaction.Commit();
                    session.Close();
                }
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 删除日排班计划。
        /// </summary>
        /// <param name="key">日排班计划标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(ScheduleDayKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.ScheduleDayDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.ScheduleDayService_IsNotExists, key);
                return result;
            }
            try
            {
                this.ScheduleDayDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取日排班计划数据。
        /// </summary>
        /// <param name="key">日排班计划标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;ScheduleDay&gt;" />,日排班计划数据.</returns>
        public MethodReturnResult<ScheduleDay> Get(ScheduleDayKey key)
        {
            MethodReturnResult<ScheduleDay> result = new MethodReturnResult<ScheduleDay>();
            if (!this.ScheduleDayDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.ScheduleDayService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.ScheduleDayDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取日排班计划数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;ScheduleDay&gt;" />,日排班计划数据集合。</returns>
        public MethodReturnResult<IList<ScheduleDay>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<ScheduleDay>> result = new MethodReturnResult<IList<ScheduleDay>>();
            try
            {
                result.Data = this.ScheduleDayDataEngine.Get(cfg);
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
