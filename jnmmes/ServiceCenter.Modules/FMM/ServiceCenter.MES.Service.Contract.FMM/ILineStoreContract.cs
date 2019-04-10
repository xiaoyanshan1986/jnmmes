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
    /// 定义线边仓数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface ILineStoreContract
    {
         /// <summary>
         /// 添加线边仓数据。
         /// </summary>
         /// <param name="obj">线边仓数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(LineStore obj);
         /// <summary>
         /// 修改线边仓数据。
         /// </summary>
         /// <param name="obj">线边仓数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(LineStore obj);
         /// <summary>
         /// 删除线边仓数据。
         /// </summary>
         /// <param name="key">线边仓数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(string key);
         /// <summary>
         /// 获取线边仓数据。
         /// </summary>
         /// <param name="key">线边仓数据标识符.</param>
         /// <returns>MethodReturnResult&lt;LineStore&gt;，线边仓数据.</returns>
         [OperationContract]
         MethodReturnResult<LineStore> Get(string key);
         /// <summary>
         /// 获取线边仓数据集合。
         /// </summary>
         /// <param name="cfg">查询线边仓。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;LineStore&gt;&gt;，线边仓数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<LineStore>> Get(ref PagingConfig cfg);
    }
}
