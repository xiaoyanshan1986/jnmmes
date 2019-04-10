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
    /// 定义效率档数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IEfficiencyContract
    {
         /// <summary>
         /// 添加效率档数据。
         /// </summary>
         /// <param name="obj">效率档数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(Efficiency obj);
         /// <summary>
         /// 修改效率档数据。
         /// </summary>
         /// <param name="obj">效率档数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(Efficiency obj);
         /// <summary>
         /// 删除效率档数据。
         /// </summary>
         /// <param name="key">效率档数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(EfficiencyKey key);
         /// <summary>
         /// 获取效率档数据。
         /// </summary>
         /// <param name="key">效率档数据标识符.</param>
         /// <returns>MethodReturnResult&lt;Efficiency&gt;，效率档数据.</returns>
         [OperationContract]
         MethodReturnResult<Efficiency> Get(EfficiencyKey key);
         /// <summary>
         /// 获取效率档数据集合。
         /// </summary>
         /// <param name="cfg">查询效率档。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;Efficiency&gt;&gt;，效率档数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<Efficiency>> Get(ref PagingConfig cfg);
    }
}
