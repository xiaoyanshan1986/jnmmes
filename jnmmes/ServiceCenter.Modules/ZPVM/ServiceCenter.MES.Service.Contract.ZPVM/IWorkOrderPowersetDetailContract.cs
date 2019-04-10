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
    /// 定义工单子分档设置数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IWorkOrderPowersetDetailContract
    {
         /// <summary>
         /// 添加工单子分档设置数据。
         /// </summary>
         /// <param name="obj">工单子分档设置数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(WorkOrderPowersetDetail obj);
         /// <summary>
         /// 修改工单子分档设置数据。
         /// </summary>
         /// <param name="obj">工单子分档设置数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(WorkOrderPowersetDetail obj);
         /// <summary>
         /// 删除工单子分档设置数据。
         /// </summary>
         /// <param name="key">工单子分档设置数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(WorkOrderPowersetDetailKey key);
         /// <summary>
         /// 获取工单子分档设置数据。
         /// </summary>
         /// <param name="key">工单子分档设置数据标识符.</param>
         /// <returns>MethodReturnResult&lt;WorkOrderPowersetDetail&gt;，工单子分档设置数据.</returns>
         [OperationContract]
         MethodReturnResult<WorkOrderPowersetDetail> Get(WorkOrderPowersetDetailKey key);
         /// <summary>
         /// 获取工单子分档设置数据集合。
         /// </summary>
         /// <param name="cfg">查询工单子分档设置数据。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;WorkOrderPowersetDetail&gt;&gt;，工单子分档设置数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<WorkOrderPowersetDetail>> Get(ref PagingConfig cfg);
    }
}
