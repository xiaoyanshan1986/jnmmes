using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.Contract.ZPVM
{
    /// <summary>
    /// 定义工单等级设置数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IWorkOrderGradeContract
    {
         /// <summary>
         /// 添加工单等级设置数据。
         /// </summary>
         /// <param name="obj">工单等级设置数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(WorkOrderGrade obj);
         /// <summary>
         /// 修改工单等级设置数据。
         /// </summary>
         /// <param name="obj">工单等级设置数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(WorkOrderGrade obj);
         /// <summary>
         /// 删除工单等级设置数据。
         /// </summary>
         /// <param name="key">工单等级设置数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(WorkOrderGradeKey key);
         /// <summary>
         /// 获取工单等级设置数据。
         /// </summary>
         /// <param name="key">工单等级设置数据标识符.</param>
         /// <returns>MethodReturnResult&lt;WorkOrderGrade&gt;，工单等级设置数据.</returns>
         [OperationContract]
         MethodReturnResult<WorkOrderGrade> Get(WorkOrderGradeKey key);
         /// <summary>
         /// 获取工单等级设置数据集合。
         /// </summary>
         /// <param name="cfg">查询工单等级设置数据。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;WorkOrderGrade&gt;&gt;，工单等级设置数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<WorkOrderGrade>> Get(ref PagingConfig cfg);
    }
}
