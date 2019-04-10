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
    /// 定义规则-控制参数对象设置数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IRuleControlObjectContract
    {
         /// <summary>
         /// 添加规则-控制参数对象设置数据。
         /// </summary>
         /// <param name="obj">规则-控制参数对象设置数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(RuleControlObject obj);
         /// <summary>
         /// 修改规则-控制参数对象设置数据。
         /// </summary>
         /// <param name="obj">规则-控制参数对象设置数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(RuleControlObject obj);
         /// <summary>
         /// 删除规则-控制参数对象设置数据。
         /// </summary>
         /// <param name="key">规则-控制参数对象设置数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(RuleControlObjectKey key);
         /// <summary>
         /// 获取规则-控制参数对象设置数据。
         /// </summary>
         /// <param name="key">规则-控制参数对象设置数据标识符.</param>
         /// <returns>MethodReturnResult&lt;RuleControlObject&gt;，规则-控制参数对象设置数据.</returns>
         [OperationContract]
         MethodReturnResult<RuleControlObject> Get(RuleControlObjectKey key);
         /// <summary>
         /// 获取规则-控制参数对象设置数据集合。
         /// </summary>
         /// <param name="cfg">查询规则-控制参数对象设置数据。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;RuleControlObject&gt;&gt;，规则-控制参数对象设置数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<RuleControlObject>> Get(ref PagingConfig cfg);
    }
}
