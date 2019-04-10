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
    /// 定义参数数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IParameterContract
    {
         /// <summary>
         /// 添加参数数据。
         /// </summary>
         /// <param name="obj">参数数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(Parameter obj);
         /// <summary>
         /// 修改参数数据。
         /// </summary>
         /// <param name="obj">参数数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(Parameter obj);
         /// <summary>
         /// 删除参数数据。
         /// </summary>
         /// <param name="key">参数数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(string key);
         /// <summary>
         /// 获取参数数据。
         /// </summary>
         /// <param name="key">参数数据标识符.</param>
         /// <returns>MethodReturnResult&lt;Parameter&gt;，参数数据.</returns>
         [OperationContract]
         MethodReturnResult<Parameter> Get(string key);
         /// <summary>
         /// 获取参数数据集合。
         /// </summary>
         /// <param name="cfg">查询参数。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;Parameter&gt;&gt;，参数数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<Parameter>> Get(ref PagingConfig cfg);
    }
}
