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
    /// 定义工单属性数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IWorkOrderAttributeContract
    {
         /// <summary>
         /// 添加工单属性数据。
         /// </summary>
         /// <param name="obj">工单属性数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(WorkOrderAttribute obj);
         /// <summary>
         /// 修改工单属性数据。
         /// </summary>
         /// <param name="obj">工单属性数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(WorkOrderAttribute obj);
         /// <summary>
         /// 删除工单属性数据。
         /// </summary>
         /// <param name="key">工单属性数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(WorkOrderAttributeKey key);
         /// <summary>
         /// 获取工单属性数据。
         /// </summary>
         /// <param name="key">工单属性数据标识符.</param>
         /// <returns>MethodReturnResult&lt;WorkOrderAttribute&gt;，工单属性数据.</returns>
         [OperationContract]
         MethodReturnResult<WorkOrderAttribute> Get(WorkOrderAttributeKey key);
         /// <summary>
         /// 获取工单属性数据集合。
         /// </summary>
         /// <param name="cfg">查询工单属性。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;WorkOrderAttribute&gt;&gt;，工单属性数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<WorkOrderAttribute>> Get(ref PagingConfig cfg);
    }
}
