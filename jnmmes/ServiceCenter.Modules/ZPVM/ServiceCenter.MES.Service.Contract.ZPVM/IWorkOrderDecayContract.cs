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
    /// 定义工单衰减设置数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IWorkOrderDecayContract
    {
         /// <summary>
         /// 添加工单衰减设置数据。
         /// </summary>
         /// <param name="obj">工单衰减设置数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(WorkOrderDecay obj);
         /// <summary>
         /// 修改工单衰减设置数据。
         /// </summary>
         /// <param name="obj">工单衰减设置数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(WorkOrderDecay obj);
         /// <summary>
         /// 删除工单衰减设置数据。
         /// </summary>
         /// <param name="key">工单衰减设置数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(WorkOrderDecayKey key);
         /// <summary>
         /// 获取工单衰减设置数据。
         /// </summary>
         /// <param name="key">工单衰减设置数据标识符.</param>
         /// <returns>MethodReturnResult&lt;WorkOrderDecay&gt;，工单衰减设置数据.</returns>
         [OperationContract]
         MethodReturnResult<WorkOrderDecay> Get(WorkOrderDecayKey key);
         /// <summary>
         /// 获取工单衰减设置数据集合。
         /// </summary>
         /// <param name="cfg">查询工单衰减设置数据。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;WorkOrderDecay&gt;&gt;，工单衰减设置数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<WorkOrderDecay>> Get(ref PagingConfig cfg);
    }
}
