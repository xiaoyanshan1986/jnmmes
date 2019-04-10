using ServiceCenter.MES.Model.FMM;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.Contract.FMM
{
    /// <summary>
    /// 定义Bin规则属性数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IBinRuleContract
    {
         /// <summary>
         /// 添加Bin规则属性数据。
         /// </summary>
         /// <param name="obj">Bin规则属性数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(BinRule obj);
         /// <summary>
         /// 修改Bin规则属性数据。
         /// </summary>
         /// <param name="obj">Bin规则属性数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(BinRule obj);
         /// <summary>
         /// 删除Bin规则属性数据。
         /// </summary>
         /// <param name="key">Bin规则属性数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(BinRuleKey key);
         /// <summary>
         /// 获取Bin规则属性数据。
         /// </summary>
         /// <param name="key">Bin规则属性数据标识符.</param>
         /// <returns>MethodReturnResult&lt;BinRule&gt;，Bin规则属性数据.</returns>
         [OperationContract]
         MethodReturnResult<BinRule> Get(BinRuleKey key);
         /// <summary>
         /// 获取Bin规则属性数据集合。
         /// </summary>
         /// <param name="cfg">查询Bin规则属性。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;BinRule&gt;&gt;，Bin规则属性数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<BinRule>> Get(ref PagingConfig cfg);
    }
}
