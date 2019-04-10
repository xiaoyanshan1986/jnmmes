using ServiceCenter.MES.Model.PPM;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.Contract.PPM
{
    /// <summary>
    /// 定义工单生产线数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IWorkOrderProductionLineContract
    {
         /// <summary>
         /// 添加工单生产线数据。
         /// </summary>
         /// <param name="obj">工单生产线数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(WorkOrderProductionLine obj);
         /// <summary>
         /// 修改工单生产线数据。
         /// </summary>
         /// <param name="obj">工单生产线数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(WorkOrderProductionLine obj);
         /// <summary>
         /// 删除工单生产线数据。
         /// </summary>
         /// <param name="key">工单生产线数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(WorkOrderProductionLineKey key);
         /// <summary>
         /// 获取工单生产线数据。
         /// </summary>
         /// <param name="key">工单生产线数据标识符.</param>
         /// <returns>MethodReturnResult&lt;WorkOrderProductionLine&gt;，工单生产线数据.</returns>
         [OperationContract]
         MethodReturnResult<WorkOrderProductionLine> Get(WorkOrderProductionLineKey key);
         /// <summary>
         /// 获取工单生产线数据集合。
         /// </summary>
         /// <param name="cfg">查询工单生产线。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;WorkOrderProductionLine&gt;&gt;，工单生产线数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<WorkOrderProductionLine>> Get(ref PagingConfig cfg);
    }
}
