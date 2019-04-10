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
    /// 定义参数推导数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IParameterDerivationContract
    {
         /// <summary>
         /// 添加参数推导数据。
         /// </summary>
         /// <param name="obj">参数推导数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(ParameterDerivation obj);
         /// <summary>
         /// 修改参数推导数据。
         /// </summary>
         /// <param name="obj">参数推导数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(ParameterDerivation obj);
         /// <summary>
         /// 删除参数推导数据。
         /// </summary>
         /// <param name="key">参数推导数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(ParameterDerivationKey key);
         /// <summary>
         /// 获取参数推导数据。
         /// </summary>
         /// <param name="key">参数推导数据标识符.</param>
         /// <returns>MethodReturnResult&lt;ParameterDerivation&gt;，参数推导数据.</returns>
         [OperationContract]
         MethodReturnResult<ParameterDerivation> Get(ParameterDerivationKey key);
         /// <summary>
         /// 获取参数推导数据集合。
         /// </summary>
         /// <param name="cfg">查询参数。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;ParameterDerivation&gt;&gt;，参数推导数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<ParameterDerivation>> Get(ref PagingConfig cfg);
    }
}
