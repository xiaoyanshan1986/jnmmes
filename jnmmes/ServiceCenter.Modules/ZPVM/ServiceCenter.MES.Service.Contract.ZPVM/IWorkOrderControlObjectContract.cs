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
    /// 定义工单控制对象设置数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IWorkOrderControlObjectContract
    {
         /// <summary>
         /// 添加工单控制对象设置数据。
         /// </summary>
         /// <param name="obj">工单控制对象设置数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(WorkOrderControlObject obj);
         /// <summary>
         /// 修改工单控制对象设置数据。
         /// </summary>
         /// <param name="obj">工单控制对象设置数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(WorkOrderControlObject obj);
         /// <summary>
         /// 删除工单控制对象设置数据。
         /// </summary>
         /// <param name="key">工单控制对象设置数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(WorkOrderControlObjectKey key);
         /// <summary>
         /// 获取工单控制对象设置数据。
         /// </summary>
         /// <param name="key">工单控制对象设置数据标识符.</param>
         /// <returns>MethodReturnResult&lt;WorkOrderControlObject&gt;，工单控制对象设置数据.</returns>
         [OperationContract]
         MethodReturnResult<WorkOrderControlObject> Get(WorkOrderControlObjectKey key);
         /// <summary>
         /// 获取工单控制对象设置数据集合。
         /// </summary>
         /// <param name="cfg">查询工单控制对象设置数据。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;WorkOrderControlObject&gt;&gt;，工单控制对象设置数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<WorkOrderControlObject>> Get(ref PagingConfig cfg);
    }
}
