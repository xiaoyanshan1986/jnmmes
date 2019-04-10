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
    /// 定义工艺流程组数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IRouteEnterpriseContract
    {
         /// <summary>
         /// 添加工艺流程组数据。
         /// </summary>
         /// <param name="obj">工艺流程组数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(RouteEnterprise obj);
         /// <summary>
         /// 修改工艺流程组数据。
         /// </summary>
         /// <param name="obj">工艺流程组数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(RouteEnterprise obj);
         /// <summary>
         /// 删除工艺流程组数据。
         /// </summary>
         /// <param name="key">工艺流程组数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(string key);
         /// <summary>
         /// 获取工艺流程组数据。
         /// </summary>
         /// <param name="key">工艺流程组数据标识符.</param>
         /// <returns>MethodReturnResult&lt;RouteEnterprise&gt;，工艺流程组数据.</returns>
         [OperationContract]
         MethodReturnResult<RouteEnterprise> Get(string key);
         /// <summary>
         /// 获取工艺流程组数据集合。
         /// </summary>
         /// <param name="cfg">查询参数。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;RouteEnterprise&gt;&gt;，工艺流程组数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<RouteEnterprise>> Get(ref PagingConfig cfg);
    }
}
