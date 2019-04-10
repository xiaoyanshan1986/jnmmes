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
    /// 定义物料数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IMaterialContract
    {
         /// <summary>
         /// 添加物料数据。
         /// </summary>
         /// <param name="obj">物料数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(Material obj);
         /// <summary>
         /// 修改物料数据。
         /// </summary>
         /// <param name="obj">物料数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(Material obj);
         /// <summary>
         /// 删除物料数据。
         /// </summary>
         /// <param name="key">物料数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(string key);
         /// <summary>
         /// 获取物料数据。
         /// </summary>
         /// <param name="key">物料数据标识符.</param>
         /// <returns>MethodReturnResult&lt;Material&gt;，物料数据.</returns>
         [OperationContract]
         MethodReturnResult<Material> Get(string key);
         /// <summary>
         /// 获取物料数据集合。
         /// </summary>
         /// <param name="cfg">查询物料。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;Material&gt;&gt;，物料数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<Material>> Get(ref PagingConfig cfg);
    }
}
