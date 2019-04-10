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
    /// 定义班别管理服务契约。
    /// </summary>
     [ServiceContract]
    public interface IShiftContract
    {
         /// <summary>
         /// 添加班别。
         /// </summary>
         /// <param name="obj">班别数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(Shift obj);

         /// <summary>
         /// 修改班别。
         /// </summary>
         /// <param name="obj">班别数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Modify(Shift obj);

         /// <summary>
         /// 删除班别。
         /// </summary>
         /// <param name="key">班别标识符。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Delete(string key);

         /// <summary>
         ///  获取班别数据。
         /// </summary>
         /// <param name="key">班别标识符.</param>
         /// <returns>MethodReturnResult&lt;Shift&gt;，班别数据。</returns>
         [OperationContract]
         MethodReturnResult<Shift> Get(string key);

         /// <summary>
         /// 获取班别数据集合。
         /// </summary>
         /// <param name="cfg">查询参数.</param>
         /// <returns>MethodReturnResult&lt;IList&lt;Shift&gt;&gt;，班别数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<Shift>> Get(ref PagingConfig cfg);
    }
}
