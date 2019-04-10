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
    /// 定义产品产品标签设置数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IMaterialPrintSetContract
    {
         /// <summary>
         /// 添加产品产品标签设置数据。
         /// </summary>
         /// <param name="obj">产品产品标签设置数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(MaterialPrintSet obj);

         /// <summary>
         /// 修改产品产品标签设置数据。
         /// </summary>
         /// <param name="obj">产品产品标签设置数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(MaterialPrintSet obj);

         /// <summary>
         /// 删除产品产品标签设置数据。
         /// </summary>
         /// <param name="key">产品产品标签设置数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(MaterialPrintSetKey key);

         /// <summary>
         /// 获取产品产品标签设置数据。
         /// </summary>
         /// <param name="key">产品产品标签设置数据标识符.</param>
         /// <returns>MethodReturnResult&lt;MaterialPrintSet&gt;，产品产品标签设置数据.</returns>
         [OperationContract]
         MethodReturnResult<MaterialPrintSet> Get(MaterialPrintSetKey key);

         /// <summary>
         /// 获取产品产品标签设置数据集合。
         /// </summary>
         /// <param name="cfg">查询产品产品标签设置。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;MaterialPrintSet&gt;&gt;，产品产品标签设置数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<MaterialPrintSet>> Get(ref PagingConfig cfg);
    }
}
