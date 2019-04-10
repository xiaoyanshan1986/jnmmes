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
    /// 定义规则-等级设置数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IRuleGradeContract
    {
         /// <summary>
         /// 添加规则-等级设置数据。
         /// </summary>
         /// <param name="obj">规则-等级设置数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(RuleGrade obj);
         /// <summary>
         /// 修改规则-等级设置数据。
         /// </summary>
         /// <param name="obj">规则-等级设置数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(RuleGrade obj);
         /// <summary>
         /// 删除规则-等级设置数据。
         /// </summary>
         /// <param name="key">规则-等级设置数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(RuleGradeKey key);
         /// <summary>
         /// 获取规则-等级设置数据。
         /// </summary>
         /// <param name="key">规则-等级设置数据标识符.</param>
         /// <returns>MethodReturnResult&lt;RuleGrade&gt;，规则-等级设置数据.</returns>
         [OperationContract]
         MethodReturnResult<RuleGrade> Get(RuleGradeKey key);
         /// <summary>
         /// 获取规则-等级设置数据集合。
         /// </summary>
         /// <param name="cfg">查询规则-等级设置数据。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;RuleGrade&gt;&gt;，规则-等级设置数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<RuleGrade>> Get(ref PagingConfig cfg);
    }
}
