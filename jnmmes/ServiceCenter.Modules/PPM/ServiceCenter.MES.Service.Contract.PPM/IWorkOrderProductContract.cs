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
    /// 定义工单产品数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IWorkOrderProductContract
    {
         /// <summary>
         /// 添加工单产品数据。
         /// </summary>
         /// <param name="obj">工单产品数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(WorkOrderProduct obj);
         /// <summary>
         /// 修改工单产品数据。
         /// </summary>
         /// <param name="obj">工单产品数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(WorkOrderProduct obj);
         /// <summary>
         /// 删除工单产品数据。
         /// </summary>
         /// <param name="key">工单产品数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(WorkOrderProductKey key);
         /// <summary>
         /// 获取工单产品数据。
         /// </summary>
         /// <param name="key">工单产品数据标识符.</param>
         /// <returns>MethodReturnResult&lt;WorkOrderProduct&gt;，工单产品数据.</returns>
         [OperationContract]
         MethodReturnResult<WorkOrderProduct> Get(WorkOrderProductKey key);
         /// <summary>
         /// 获取工单产品数据集合。
         /// </summary>
         /// <param name="cfg">查询工单产品。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;WorkOrderProduct&gt;&gt;，工单产品数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<WorkOrderProduct>> Get(ref PagingConfig cfg);
    }
}
