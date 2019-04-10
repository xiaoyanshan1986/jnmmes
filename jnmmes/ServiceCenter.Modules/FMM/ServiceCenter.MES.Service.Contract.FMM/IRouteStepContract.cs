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
    /// 定义工步数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IRouteStepContract
    {
         /// <summary>
         /// 添加工步数据。
         /// </summary>
         /// <param name="obj">工步数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(RouteStep obj);
         /// <summary>
         /// 修改工步数据。
         /// </summary>
         /// <param name="obj">工步数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(RouteStep obj);
         /// <summary>
         /// 删除工步数据。
         /// </summary>
         /// <param name="key">工步数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(RouteStepKey key);
         /// <summary>
         /// 获取工步数据。
         /// </summary>
         /// <param name="key">工步数据标识符.</param>
         /// <returns>MethodReturnResult&lt;RouteStep&gt;，工步数据.</returns>
         [OperationContract]
         MethodReturnResult<RouteStep> Get(RouteStepKey key);
         /// <summary>
         /// 获取工步数据集合。
         /// </summary>
         /// <param name="cfg">查询参数。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;RouteStep&gt;&gt;，工步数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<RouteStep>> Get(ref PagingConfig cfg);
    }
}
