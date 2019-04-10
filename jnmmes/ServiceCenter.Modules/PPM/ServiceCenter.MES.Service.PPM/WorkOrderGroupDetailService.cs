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
using System.Transactions;

namespace ServiceCenter.MES.Service.PPM
{
    /// <summary>
    /// 实现混工单组规则数据管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class WorkOrderGroupDetailService : IWorkOrderGroupDetailContract
    {
        /// <summary>
        /// 混工单组规则数据数据访问读写。
        /// </summary>
        public IWorkOrderGroupDetailDataEngine WorkOrderGroupDetailDataEngine { get; set; }       

        /// <summary>
        /// 添加混工单组规则数据。
        /// </summary>
        /// <param name="obj">混工单组规则数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(WorkOrderGroupDetail obj)
        {
            MethodReturnResult result = new MethodReturnResult();

            if (this.WorkOrderGroupDetailDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.WorkOrderGroupDetailService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.WorkOrderGroupDetailDataEngine.Insert(obj);
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
        /// 修改混工单组规则数据。
        /// </summary>
        /// <param name="obj">混工单组规则数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(WorkOrderGroupDetail obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            ISession session = this.WorkOrderGroupDetailDataEngine.SessionFactory.OpenSession();
            ITransaction transaction = session.BeginTransaction();
            if (!this.WorkOrderGroupDetailDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.WorkOrderGroupDetailService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                //查找和同在此工单组的项目
                PagingConfig cfg = new PagingConfig()
                {
                    Where = string.Format(" Key.WorkOrderGroupNo = '{0}' and ItemNo <> {1}", obj.Key.WorkOrderGroupNo,obj.ItemNo)
                };
                MethodReturnResult<IList<WorkOrderGroupDetail>> resultOfOther=new MethodReturnResult<IList<WorkOrderGroupDetail>> ();
                resultOfOther.Data = this.WorkOrderGroupDetailDataEngine.Get(cfg);
                List<WorkOrderGroupDetail> lstWorkOrderGroupDetailDataForUpdate = new List<WorkOrderGroupDetail>();
                if (resultOfOther.Data != null && resultOfOther.Data.Count > 0)
                {
                    foreach (WorkOrderGroupDetail item in resultOfOther.Data)
                    {
                        item.Description = obj.Description;
                        lstWorkOrderGroupDetailDataForUpdate.Add(item);
                    }
                }

                //处理当前修改的工单的混工单组规则
                this.WorkOrderGroupDetailDataEngine.Update(obj, session);

                //处理该混工单组内其他工单的组规则
                if (lstWorkOrderGroupDetailDataForUpdate.Count > 0)
                {
                    foreach (WorkOrderGroupDetail item in lstWorkOrderGroupDetailDataForUpdate)
                    {
                        this.WorkOrderGroupDetailDataEngine.Update(item, session);
                    }
                }
                transaction.Commit();
                session.Close();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                session.Close();
                result.Code = 1000;
                result.Message = String.Format(StringResource.Error, ex.Message);
                result.Detail = ex.ToString();
            }
            return result;
        }
        /// <summary>
        /// 删除混工单组规则数据。
        /// </summary>
        /// <param name="key">混工单组规则主键。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(WorkOrderGroupDetailKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.WorkOrderGroupDetailDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.WorkOrderGroupDetailService_IsNotExists, key);
                return result;
            }
            try
            {
                ISession session = this.WorkOrderGroupDetailDataEngine.SessionFactory.OpenSession();
                ITransaction transaction = session.BeginTransaction();
                {
                    this.WorkOrderGroupDetailDataEngine.Delete(key, session);                    
                    transaction.Commit();
                    session.Close();
                }
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
        /// 获取混工单组规则数据。
        /// </summary>
        /// <param name="key">混工单组规则主键.</param>
        /// <returns><see cref="MethodReturnResult&lt;WorkOrderGroupDetail&gt;" />,混工单组规则数据数据.</returns>
        public MethodReturnResult<WorkOrderGroupDetail> Get(WorkOrderGroupDetailKey key)
        {
            MethodReturnResult<WorkOrderGroupDetail> result = new MethodReturnResult<WorkOrderGroupDetail>();
            if (!this.WorkOrderGroupDetailDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.WorkOrderGroupDetailService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.WorkOrderGroupDetailDataEngine.Get(key);
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
        /// 获取混工单组规则数据数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;WorkOrderGroupDetail&gt;" />,混工单组规则数据数据集合。</returns>
        public MethodReturnResult<IList<WorkOrderGroupDetail>> Gets(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<WorkOrderGroupDetail>> result = new MethodReturnResult<IList<WorkOrderGroupDetail>>();
            try
            {
                result.Data = this.WorkOrderGroupDetailDataEngine.Get(cfg);
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
