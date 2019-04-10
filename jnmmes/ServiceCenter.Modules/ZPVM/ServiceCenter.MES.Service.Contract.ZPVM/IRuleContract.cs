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
    /// 定义规则数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IRuleContract
    {
         /// <summary>
         /// 添加规则数据。
         /// </summary>
         /// <param name="obj">规则数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(Rule obj);
         /// <summary>
         /// 修改规则数据。
         /// </summary>
         /// <param name="obj">规则数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(Rule obj);
         /// <summary>
         /// 删除规则数据。
         /// </summary>
         /// <param name="key">规则数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(string key);
         /// <summary>
         /// 获取规则数据。
         /// </summary>
         /// <param name="key">规则数据标识符.</param>
         /// <returns>MethodReturnResult&lt;Rule&gt;，规则数据.</returns>
         [OperationContract]
         MethodReturnResult<Rule> Get(string key);
         /// <summary>
         /// 获取规则数据集合。
         /// </summary>
         /// <param name="cfg">查询规则数据。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;Rule&gt;&gt;，规则数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<Rule>> Get(ref PagingConfig cfg);
    }
}
