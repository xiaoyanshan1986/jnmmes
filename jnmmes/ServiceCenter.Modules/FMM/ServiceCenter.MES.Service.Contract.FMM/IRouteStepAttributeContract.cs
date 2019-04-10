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
    /// 定义工步属性数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IRouteStepAttributeContract
    {
         /// <summary>
         /// 添加工步属性数据。
         /// </summary>
         /// <param name="obj">工步属性数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(RouteStepAttribute obj);
         /// <summary>
         /// 修改工步属性数据。
         /// </summary>
         /// <param name="obj">工步属性数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(RouteStepAttribute obj);
         /// <summary>
         /// 删除工步属性数据。
         /// </summary>
         /// <param name="key">工步属性数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(RouteStepAttributeKey key);
         /// <summary>
         /// 获取工步属性数据。
         /// </summary>
         /// <param name="key">工步属性数据标识符.</param>
         /// <returns>MethodReturnResult&lt;RouteStepAttribute&gt;，工步属性数据.</returns>
         [OperationContract]
         MethodReturnResult<RouteStepAttribute> Get(RouteStepAttributeKey key);
         /// <summary>
         /// 获取工步属性数据集合。
         /// </summary>
         /// <param name="cfg">查询参数。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;RouteStepAttribute&gt;&gt;，工步属性数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<RouteStepAttribute>> Get(ref PagingConfig cfg);
    }
}
