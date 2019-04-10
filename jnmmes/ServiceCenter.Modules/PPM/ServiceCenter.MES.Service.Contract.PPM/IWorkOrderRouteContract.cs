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
    /// 定义工单工艺数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IWorkOrderRouteContract
    {
         /// <summary>
         /// 添加工单工艺数据。
         /// </summary>
         /// <param name="obj">工单工艺数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(WorkOrderRoute obj);
         /// <summary>
         /// 修改工单工艺数据。
         /// </summary>
         /// <param name="obj">工单工艺数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(WorkOrderRoute obj);
         /// <summary>
         /// 删除工单工艺数据。
         /// </summary>
         /// <param name="key">工单工艺数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(WorkOrderRouteKey key);
         /// <summary>
         /// 获取工单工艺数据。
         /// </summary>
         /// <param name="key">工单工艺数据标识符.</param>
         /// <returns>MethodReturnResult&lt;WorkOrderRoute&gt;，工单工艺数据.</returns>
         [OperationContract]
         MethodReturnResult<WorkOrderRoute> Get(WorkOrderRouteKey key);
         /// <summary>
         /// 获取工单工艺数据集合。
         /// </summary>
         /// <param name="cfg">查询工单工艺。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;WorkOrderRoute&gt;&gt;，工单工艺数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<WorkOrderRoute>> Get(ref PagingConfig cfg);
    }
}
