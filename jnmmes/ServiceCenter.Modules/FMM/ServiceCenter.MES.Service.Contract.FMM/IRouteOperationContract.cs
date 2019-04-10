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
    /// 定义工序数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IRouteOperationContract
    {
         /// <summary>
         /// 添加工序数据。
         /// </summary>
         /// <param name="obj">工序数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(RouteOperation obj);
         /// <summary>
         /// 修改工序数据。
         /// </summary>
         /// <param name="obj">工序数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(RouteOperation obj);
         /// <summary>
         /// 删除工序数据。
         /// </summary>
         /// <param name="key">工序数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(string key);
         /// <summary>
         /// 获取工序数据。
         /// </summary>
         /// <param name="key">工序数据标识符.</param>
         /// <returns>MethodReturnResult&lt;RouteOperation&gt;，工序数据.</returns>
         [OperationContract]
         MethodReturnResult<RouteOperation> Get(string key);
         /// <summary>
         /// 获取工序数据集合。
         /// </summary>
         /// <param name="cfg">查询参数。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;RouteOperation&gt;&gt;，工序数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<RouteOperation>> Get(ref PagingConfig cfg);
    }
}
