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
    /// 实现日排班计划管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class PlanAttendanceService : IPlanAttendanceContract
    {
        /// <summary>
        /// 日排班计划数据访问读写。
        /// </summary>
        public IPlanAttendanceDataEngine PlanAttendanceDataEngine { get; set; }

        /// <summary>
        /// 添加日排班计划。
        /// </summary>
        /// <param name="obj">日排班计划数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(PlanAttendance obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.PlanAttendanceDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.PlanAttendanceService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.PlanAttendanceDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        
        /// <summary>
        /// 修改日排班计划。
        /// </summary>
        /// <param name="obj">日排班计划数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(PlanAttendance obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                this.PlanAttendanceDataEngine.Modify(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 修改日排班计划(列表方式)
        /// </summary>
        /// <param name="lst">计划列表</param>
        /// <returns></returns>
        public MethodReturnResult Modify(IList<PlanAttendance> lst)
        {
            MethodReturnResult result = new MethodReturnResult();
            ISession db = null;
            ITransaction transaction = null;
            try
            {
                db = this.PlanAttendanceDataEngine.SessionFactory.OpenSession();
                transaction = db.BeginTransaction();         
                foreach (PlanAttendance obj in lst)
                {
                    this.PlanAttendanceDataEngine.Modify(obj,db);
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
        /// 删除日排班计划。
        /// </summary>
        /// <param name="key">日排班计划标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(PlanAttendanceKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.PlanAttendanceDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.PlanAttendanceService_IsNotExists, key);
                return result;
            }
            try
            {
                this.PlanAttendanceDataEngine.Delete(key);
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
        /// <returns><see cref="MethodReturnResult&lt;PlanAttendance&gt;" />,日排班计划数据.</returns>
        public MethodReturnResult<PlanAttendance> Get(PlanAttendanceKey key)
        {
            MethodReturnResult<PlanAttendance> result = new MethodReturnResult<PlanAttendance>();
            if (!this.PlanAttendanceDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.PlanAttendanceService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.PlanAttendanceDataEngine.Get(key);
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
        /// <returns><see cref="MethodReturnResult&lt;PlanAttendance&gt;" />,日排班计划数据集合。</returns>
        public MethodReturnResult<IList<PlanAttendance>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<PlanAttendance>> result = new MethodReturnResult<IList<PlanAttendance>>();
            try
            {
                result.Data = this.PlanAttendanceDataEngine.Get(cfg);
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
