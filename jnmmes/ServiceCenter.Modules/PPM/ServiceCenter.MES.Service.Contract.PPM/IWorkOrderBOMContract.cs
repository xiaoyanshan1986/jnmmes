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
    /// 定义工单BOM数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IWorkOrderBOMContract
    {
         /// <summary>
         /// 添加工单BOM数据。
         /// </summary>
         /// <param name="obj">工单BOM数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(WorkOrderBOM obj);
         /// <summary>
         /// 修改工单BOM数据。
         /// </summary>
         /// <param name="obj">工单BOM数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(WorkOrderBOM obj);
         /// <summary>
         /// 删除工单BOM数据。
         /// </summary>
         /// <param name="key">工单BOM数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(WorkOrderBOMKey key);
         /// <summary>
         /// 获取工单BOM数据。
         /// </summary>
         /// <param name="key">工单BOM数据标识符.</param>
         /// <returns>MethodReturnResult&lt;WorkOrderBOM&gt;，工单BOM数据.</returns>
         [OperationContract]
         MethodReturnResult<WorkOrderBOM> Get(WorkOrderBOMKey key);
         /// <summary>
         /// 获取工单BOM数据集合。
         /// </summary>
         /// <param name="cfg">查询工单BOM。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;WorkOrderBOM&gt;&gt;，工单BOM数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<WorkOrderBOM>> Get(ref PagingConfig cfg);
    }
}
