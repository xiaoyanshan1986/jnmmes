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
    /// 定义物料类型工艺流程数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IMaterialTypeRouteContract
    {
         /// <summary>
         /// 添加物料类型工艺流程数据。
         /// </summary>
         /// <param name="obj">物料类型工艺流程数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(MaterialTypeRoute obj);
         /// <summary>
         /// 修改物料类型工艺流程数据。
         /// </summary>
         /// <param name="obj">物料类型工艺流程数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(MaterialTypeRoute obj);
         /// <summary>
         /// 删除物料类型工艺流程数据。
         /// </summary>
         /// <param name="key">物料类型工艺流程数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(MaterialTypeRouteKey key);
         /// <summary>
         /// 获取物料类型工艺流程数据。
         /// </summary>
         /// <param name="key">物料类型工艺流程数据标识符.</param>
         /// <returns>MethodReturnResult&lt;MaterialTypeRoute&gt;，物料类型工艺流程数据.</returns>
         [OperationContract]
         MethodReturnResult<MaterialTypeRoute> Get(MaterialTypeRouteKey key);
         /// <summary>
         /// 获取物料类型工艺流程数据集合。
         /// </summary>
         /// <param name="cfg">查询物料类型工艺流程。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;MaterialTypeRoute&gt;&gt;，物料类型工艺流程数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<MaterialTypeRoute>> Get(ref PagingConfig cfg);
    }
}
