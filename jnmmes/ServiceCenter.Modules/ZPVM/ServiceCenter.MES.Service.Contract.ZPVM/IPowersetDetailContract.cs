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
    /// 定义子分档数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IPowersetDetailContract
    {
         /// <summary>
         /// 添加子分档数据。
         /// </summary>
         /// <param name="obj">子分档数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(PowersetDetail obj);
         /// <summary>
         /// 修改子分档数据。
         /// </summary>
         /// <param name="obj">子分档数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(PowersetDetail obj);
         /// <summary>
         /// 删除子分档数据。
         /// </summary>
         /// <param name="key">子分档数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(PowersetDetailKey key);
         /// <summary>
         /// 获取子分档数据。
         /// </summary>
         /// <param name="key">子分档数据标识符.</param>
         /// <returns>MethodReturnResult&lt;PowersetDetail&gt;，子分档数据.</returns>
         [OperationContract]
         MethodReturnResult<PowersetDetail> Get(PowersetDetailKey key);
         /// <summary>
         /// 获取子分档数据集合。
         /// </summary>
         /// <param name="cfg">查询子分档数据。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;PowersetDetail&gt;&gt;，子分档数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<PowersetDetail>> Get(ref PagingConfig cfg);
    }
}
