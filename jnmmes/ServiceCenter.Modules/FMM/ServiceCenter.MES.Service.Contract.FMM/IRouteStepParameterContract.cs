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
    /// 定义工步参数数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IRouteStepParameterContract
    {
         /// <summary>
         /// 添加工步参数数据。
         /// </summary>
         /// <param name="obj">工步参数数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(RouteStepParameter obj);
         /// <summary>
         /// 修改工步参数数据。
         /// </summary>
         /// <param name="obj">工步参数数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(RouteStepParameter obj);
         /// <summary>
         /// 删除工步参数数据。
         /// </summary>
         /// <param name="key">工步参数数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(RouteStepParameterKey key);
         /// <summary>
         /// 获取工步参数数据。
         /// </summary>
         /// <param name="key">工步参数数据标识符.</param>
         /// <returns>MethodReturnResult&lt;RouteStepParameter&gt;，工步参数数据.</returns>
         [OperationContract]
         MethodReturnResult<RouteStepParameter> Get(RouteStepParameterKey key);
         /// <summary>
         /// 获取工步参数数据集合。
         /// </summary>
         /// <param name="cfg">查询参数。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;RouteStepParameter&gt;&gt;，工步参数数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<RouteStepParameter>> Get(ref PagingConfig cfg);
    }
}
