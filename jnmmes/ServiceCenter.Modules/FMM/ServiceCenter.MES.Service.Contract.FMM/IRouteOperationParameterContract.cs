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
    /// 定义工序参数数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IRouteOperationParameterContract
    {
         /// <summary>
         /// 添加工序参数数据。
         /// </summary>
         /// <param name="obj">工序参数数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(RouteOperationParameter obj);
         /// <summary>
         /// 修改工序参数数据。
         /// </summary>
         /// <param name="obj">工序参数数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(RouteOperationParameter obj);
         /// <summary>
         /// 删除工序参数数据。
         /// </summary>
         /// <param name="key">工序参数数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(RouteOperationParameterKey key);
         /// <summary>
         /// 获取工序参数数据。
         /// </summary>
         /// <param name="key">工序参数数据标识符.</param>
         /// <returns>MethodReturnResult&lt;RouteOperationParameter&gt;，工序参数数据.</returns>
         [OperationContract]
         MethodReturnResult<RouteOperationParameter> Get(RouteOperationParameterKey key);
         /// <summary>
         /// 获取工序参数数据集合。
         /// </summary>
         /// <param name="cfg">查询参数。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;RouteOperationParameter&gt;&gt;，工序参数数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<RouteOperationParameter>> Get(ref PagingConfig cfg);
    }
}
