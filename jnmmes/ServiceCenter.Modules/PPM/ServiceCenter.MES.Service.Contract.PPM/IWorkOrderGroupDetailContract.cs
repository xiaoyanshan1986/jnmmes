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
    /// 定义混工单组规则数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IWorkOrderGroupDetailContract 
    {
         /// <summary>
         /// 添加混工单组规则。
         /// </summary>
         /// <param name="obj">混工单组规则数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(WorkOrderGroupDetail obj);
         /// <summary>
         /// 修改混工单组规则。
         /// </summary>
         /// <param name="obj">混工单组规则数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(WorkOrderGroupDetail obj);
         /// <summary>
         /// 删除混工单组规则。
         /// </summary>
         /// <param name="key">混工单组规则主键.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(WorkOrderGroupDetailKey key);
         /// <summary>
         /// 获取混工单组规则。
         /// </summary>
         /// <param name="key">混工单组规则主键.</param>
         /// <returns>MethodReturnResult&lt;WorkOrderGroupDetail&gt;，混工单组规则数据.</returns>
         [OperationContract]
         MethodReturnResult<WorkOrderGroupDetail> Get(WorkOrderGroupDetailKey key);
         /// <summary>
         /// 获取混工单组规则。
         /// </summary>
         /// <param name="cfg">查询混工单组规则数据。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;WorkOrderGroupDetail&gt;&gt;，混工单组规则数据集合。</returns>
         [OperationContract]
         MethodReturnResult<IList<WorkOrderGroupDetail>> Gets(ref PagingConfig cfg);
    }
}
