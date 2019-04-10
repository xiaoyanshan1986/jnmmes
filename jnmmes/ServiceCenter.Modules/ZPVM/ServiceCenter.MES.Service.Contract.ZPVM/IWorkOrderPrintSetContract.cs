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
    /// 定义工单打印设置数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IWorkOrderPrintSetContract
    {
         /// <summary>
         /// 添加工单打印设置数据。
         /// </summary>
         /// <param name="obj">工单打印设置数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(WorkOrderPrintSet obj);
         /// <summary>
         /// 修改工单打印设置数据。
         /// </summary>
         /// <param name="obj">工单打印设置数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(WorkOrderPrintSet obj);
         /// <summary>
         /// 删除工单打印设置数据。
         /// </summary>
         /// <param name="key">工单打印设置数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(WorkOrderPrintSetKey key);
         /// <summary>
         /// 获取工单打印设置数据。
         /// </summary>
         /// <param name="key">工单打印设置数据标识符.</param>
         /// <returns>MethodReturnResult&lt;WorkOrderPrintSet&gt;，工单打印设置数据.</returns>
         [OperationContract]
         MethodReturnResult<WorkOrderPrintSet> Get(WorkOrderPrintSetKey key);
         /// <summary>
         /// 获取工单打印设置数据集合。
         /// </summary>
         /// <param name="cfg">查询工单打印设置数据。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;WorkOrderPrintSet&gt;&gt;，工单打印设置数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<WorkOrderPrintSet>> Get(ref PagingConfig cfg);
    }
}
