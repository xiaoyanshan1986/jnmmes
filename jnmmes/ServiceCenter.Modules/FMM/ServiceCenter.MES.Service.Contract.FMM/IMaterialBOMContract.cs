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
    /// 定义物料BOM数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IMaterialBOMContract
    {
         /// <summary>
         /// 添加物料BOM数据。
         /// </summary>
         /// <param name="obj">物料BOM数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(MaterialBOM obj);
         /// <summary>
         /// 修改物料BOM数据。
         /// </summary>
         /// <param name="obj">物料BOM数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(MaterialBOM obj);
         /// <summary>
         /// 删除物料BOM数据。
         /// </summary>
         /// <param name="key">物料BOM数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(MaterialBOMKey key);
         /// <summary>
         /// 获取物料BOM数据。
         /// </summary>
         /// <param name="key">物料BOM数据标识符.</param>
         /// <returns>MethodReturnResult&lt;MaterialBOM&gt;，物料BOM数据.</returns>
         [OperationContract]
         MethodReturnResult<MaterialBOM> Get(MaterialBOMKey key);
         /// <summary>
         /// 获取物料BOM数据集合。
         /// </summary>
         /// <param name="cfg">查询物料BOM。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;MaterialBOM&gt;&gt;，物料BOM数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<MaterialBOM>> Get(ref PagingConfig cfg);
    }
}
