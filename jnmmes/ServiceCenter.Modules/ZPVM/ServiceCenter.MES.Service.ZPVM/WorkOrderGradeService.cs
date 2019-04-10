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
    /// 实现工单等级设置数据管理服务契约。
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class WorkOrderGradeService : IWorkOrderGradeContract
    {
        /// <summary>
        /// 工单等级设置数据数据访问读写。
        /// </summary>
        public IWorkOrderGradeDataEngine WorkOrderGradeDataEngine { get; set; }


        /// <summary>
        /// 添加工单等级设置数据。
        /// </summary>
        /// <param name="obj">工单等级设置数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(WorkOrderGrade obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (this.WorkOrderGradeDataEngine.IsExists(obj.Key))
            {
                result.Code = 1001;
                result.Message = String.Format(StringResource.WorkOrderGradeService_IsExists, obj.Key);
                return result;
            }
            try
            {
                this.WorkOrderGradeDataEngine.Insert(obj);
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
        /// 修改工单等级设置数据。
        /// </summary>
        /// <param name="obj">工单等级设置数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(WorkOrderGrade obj)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.WorkOrderGradeDataEngine.IsExists(obj.Key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.WorkOrderGradeService_IsNotExists, obj.Key);
                return result;
            }
            try
            {
                this.WorkOrderGradeDataEngine.Update(obj);
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
        /// 删除工单等级设置数据。
        /// </summary>
        /// <param name="key">工单等级设置数据标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(WorkOrderGradeKey key)
        {
            MethodReturnResult result = new MethodReturnResult();
            if (!this.WorkOrderGradeDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.WorkOrderGradeService_IsNotExists, key);
                return result;
            }
            try
            {
                this.WorkOrderGradeDataEngine.Delete(key);
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
        /// 获取工单等级设置数据数据。
        /// </summary>
        /// <param name="key">工单等级设置数据标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;WorkOrderGrade&gt;" />,工单等级设置数据数据.</returns>
        public MethodReturnResult<WorkOrderGrade> Get(WorkOrderGradeKey key)
        {
            MethodReturnResult<WorkOrderGrade> result = new MethodReturnResult<WorkOrderGrade>();
            if (!this.WorkOrderGradeDataEngine.IsExists(key))
            {
                result.Code = 1002;
                result.Message = String.Format(StringResource.WorkOrderGradeService_IsNotExists, key);
                return result;
            }
            try
            {
                result.Data = this.WorkOrderGradeDataEngine.Get(key);
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
        /// 获取工单等级设置数据数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;WorkOrderGrade&gt;" />,工单等级设置数据数据集合。</returns>
        public MethodReturnResult<IList<WorkOrderGrade>> Get(ref PagingConfig cfg)
        {
            MethodReturnResult<IList<WorkOrderGrade>> result = new MethodReturnResult<IList<WorkOrderGrade>>();
            try
            {
                result.Data = this.WorkOrderGradeDataEngine.Get(cfg);
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
