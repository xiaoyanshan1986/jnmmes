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
    /// 定义工单数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IWorkOrderContract
    {
         /// <summary>
         /// 添加工单数据。
         /// </summary>
         /// <param name="obj">工单数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(WorkOrder obj);
         /// <summary>
         /// 修改工单数据。
         /// </summary>
         /// <param name="obj">工单数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(WorkOrder obj);
         /// <summary>
         /// 删除工单数据。
         /// </summary>
         /// <param name="key">工单数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(string key);
         /// <summary>
         /// 获取工单数据。
         /// </summary>
         /// <param name="key">工单数据标识符.</param>
         /// <returns>MethodReturnResult&lt;WorkOrder&gt;，工单数据.</returns>
         [OperationContract]
         MethodReturnResult<WorkOrder> Get(string key);
         /// <summary>
         /// 获取工单数据集合。
         /// </summary>
         /// <param name="cfg">查询工单。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;WorkOrder&gt;&gt;，工单数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<WorkOrder>> Get(ref PagingConfig cfg);
    }
}
