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
    /// 实现日不良管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class DefectService : IDefectContract
    {
        /// <summary>
        /// 日不良数据访问读写。
        /// </summary>
        public IDefectDataEngine DefectDataEngine { get; set; }

        /// <summary>
        /// 添加日不良。
        /// </summary>
        /// <param name="obj">日不良数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(Defect obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.DefectDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.DefectService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.DefectDataEngine.Insert(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }
        
        /// <summary>
        /// 修改日不良。
        /// </summary>
        /// <param name="obj">日不良数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(Defect obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            try
            {
                this.DefectDataEngine.Modify(obj);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 修改日不良(列表方式)
        /// </summary>
        /// <param name="lst">计划列表</param>
        /// <returns></returns>
        public MethodReturnResult Modify(IList<Defect> lst)
        {
            MethodReturnResult result = new MethodReturnResult();
            ISession db = null;
            ITransaction transaction = null;
            try
            {
                db = this.DefectDataEngine.SessionFactory.OpenSession();
                transaction = db.BeginTransaction();         
                foreach (Defect obj in lst)
                {
                    this.DefectDataEngine.Modify(obj,db);
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
        /// 删除日不良。
        /// </summary>
        /// <param name="key">日不良标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(DefectKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.DefectDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.DefectService_IsNotExists, key);
                return result;
            }
            try
            {
                this.DefectDataEngine.Delete(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取日不良数据。
        /// </summary>
        /// <param name="key">日不良标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;Defect&gt;" />,日不良数据.</returns>
        public MethodReturnResult<Defect> Get(DefectKey key)
        {
            MethodReturnResult<Defect> result = new MethodReturnResult<Defect>();
            if (!this.DefectDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.DefectService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.DefectDataEngine.Get(key);
            }
            catch (Exception ex)
            {
                result.Code = 1000;
                result.Message = String.Format(StringResource.OtherError, ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取日不良数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;Defect&gt;" />,日不良数据集合。</returns>
        public MethodReturnResult<IList<Defect>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<Defect>> result = new MethodReturnResult<IList<Defect>>();
            try
            {
                result.Data = this.DefectDataEngine.Get(cfg);
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
