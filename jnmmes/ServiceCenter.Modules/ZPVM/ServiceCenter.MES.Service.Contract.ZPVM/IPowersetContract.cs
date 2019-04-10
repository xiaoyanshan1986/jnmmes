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
    /// 定义分档数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IPowersetContract
    {
         /// <summary>
         /// 添加分档数据。
         /// </summary>
         /// <param name="obj">分档数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(Powerset obj);
         /// <summary>
         /// 修改分档数据。
         /// </summary>
         /// <param name="obj">分档数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(Powerset obj);
         /// <summary>
         /// 删除分档数据。
         /// </summary>
         /// <param name="key">分档数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(PowersetKey key);
         /// <summary>
         /// 获取分档数据。
         /// </summary>
         /// <param name="key">分档数据标识符.</param>
         /// <returns>MethodReturnResult&lt;Powerset&gt;，分档数据.</returns>
         [OperationContract]
         MethodReturnResult<Powerset> Get(PowersetKey key);
         /// <summary>
         /// 获取分档数据集合。
         /// </summary>
         /// <param name="cfg">查询分档数据。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;Powerset&gt;&gt;，分档数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<Powerset>> Get(ref PagingConfig cfg);
    }
}
