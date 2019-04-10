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
    /// 定义供应商管理服务契约。
    /// </summary>
     [ServiceContract]
    public interface ISupplierContract
    {
         /// <summary>
         /// 添加供应商。
         /// </summary>
         /// <param name="obj">供应商数据。</param>
         /// <returns><see cref="MethodReturnResult"/>.</returns>
         [OperationContract]
         MethodReturnResult Add(Supplier obj);
         /// <summary>
         /// 修改供应商。
         /// </summary>
         /// <param name="obj">供应商数据。</param>
         /// <returns><see cref="MethodReturnResult"/>.</returns>
         [OperationContract]
         MethodReturnResult Modify(Supplier obj);
         /// <summary>
         /// 删除供应商。
         /// </summary>
         /// <param name="key">供应商标识符。</param>
         /// <returns><see cref="MethodReturnResult"/>.</returns>
         [OperationContract]
         MethodReturnResult Delete(string key);
         /// <summary>
         /// 获取供应商数据。
         /// </summary>
         /// <param name="key">供应商标识符.</param>
         /// <returns><see cref="MethodReturnResult&lt;Supplier&gt;"/>,供应商数据.</returns>
         [OperationContract]
         MethodReturnResult<Supplier> Get(string key);

         /// <summary>
         ///  获取供应商数据集合。
         /// </summary>
         /// <param name="cfg">查询参数.</param>
         /// <returns>MethodReturnResult&lt;IList&lt;Supplier&gt;&gt;，供应商数据集合.</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<Supplier>> Get(ref PagingConfig cfg);
    }
}
