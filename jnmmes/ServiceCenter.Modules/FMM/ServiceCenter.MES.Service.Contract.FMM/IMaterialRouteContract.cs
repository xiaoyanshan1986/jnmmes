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
    /// 定义产品工艺流程数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IMaterialRouteContract
    {
         /// <summary>
         /// 添加产品工艺流程数据。
         /// </summary>
         /// <param name="obj">产品工艺流程数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(MaterialRoute obj);
         /// <summary>
         /// 修改产品工艺流程数据。
         /// </summary>
         /// <param name="obj">产品工艺流程数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(MaterialRoute obj);
         /// <summary>
         /// 删除产品工艺流程数据。
         /// </summary>
         /// <param name="key">产品工艺流程数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(MaterialRouteKey key);
         /// <summary>
         /// 获取产品工艺流程数据。
         /// </summary>
         /// <param name="key">产品工艺流程数据标识符.</param>
         /// <returns>MethodReturnResult&lt;MaterialRoute&gt;，产品工艺流程数据.</returns>
         [OperationContract]
         MethodReturnResult<MaterialRoute> Get(MaterialRouteKey key);
         /// <summary>
         /// 获取产品工艺流程数据集合。
         /// </summary>
         /// <param name="cfg">查询产品工艺流程。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;MaterialRoute&gt;&gt;，产品工艺流程数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<MaterialRoute>> Get(ref PagingConfig cfg);
    }
}
