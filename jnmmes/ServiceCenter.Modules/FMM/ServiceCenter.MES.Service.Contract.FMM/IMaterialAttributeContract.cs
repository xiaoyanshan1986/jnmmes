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
    /// 定义物料属性数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IMaterialAttributeContract
    {
         /// <summary>
         /// 添加物料属性数据。
         /// </summary>
         /// <param name="obj">物料属性数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(MaterialAttribute obj);
         /// <summary>
         /// 修改物料属性数据。
         /// </summary>
         /// <param name="obj">物料属性数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(MaterialAttribute obj);
         /// <summary>
         /// 删除物料属性数据。
         /// </summary>
         /// <param name="key">物料属性数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(MaterialAttributeKey key);
         /// <summary>
         /// 获取物料属性数据。
         /// </summary>
         /// <param name="key">物料属性数据标识符.</param>
         /// <returns>MethodReturnResult&lt;MaterialAttribute&gt;，物料属性数据.</returns>
         [OperationContract]
         MethodReturnResult<MaterialAttribute> Get(MaterialAttributeKey key);
         /// <summary>
         /// 获取物料属性数据集合。
         /// </summary>
         /// <param name="cfg">查询物料属性。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;MaterialAttribute&gt;&gt;，物料属性数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<MaterialAttribute>> Get(ref PagingConfig cfg);
    }
}
