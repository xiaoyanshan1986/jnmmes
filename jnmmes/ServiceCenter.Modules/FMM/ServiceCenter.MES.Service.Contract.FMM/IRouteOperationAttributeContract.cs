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
    /// 定义工序属性数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IRouteOperationAttributeContract
    {
         /// <summary>
         /// 添加工序属性数据。
         /// </summary>
         /// <param name="obj">工序属性数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(RouteOperationAttribute obj);
         /// <summary>
         /// 修改工序属性数据。
         /// </summary>
         /// <param name="obj">工序属性数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(RouteOperationAttribute obj);
         /// <summary>
         /// 删除工序属性数据。
         /// </summary>
         /// <param name="key">工序属性数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(RouteOperationAttributeKey key);
         /// <summary>
         /// 获取工序属性数据。
         /// </summary>
         /// <param name="key">工序属性数据标识符.</param>
         /// <returns>MethodReturnResult&lt;RouteOperationAttribute&gt;，工序属性数据.</returns>
         [OperationContract]
         MethodReturnResult<RouteOperationAttribute> Get(RouteOperationAttributeKey key);
         /// <summary>
         /// 获取工序属性数据集合。
         /// </summary>
         /// <param name="cfg">查询参数。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;RouteOperationAttribute&gt;&gt;，工序属性数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<RouteOperationAttribute>> Get(ref PagingConfig cfg);
    }
}
