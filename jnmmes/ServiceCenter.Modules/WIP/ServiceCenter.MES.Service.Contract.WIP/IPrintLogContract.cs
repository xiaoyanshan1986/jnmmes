using ServiceCenter.MES.Model.WIP;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.Contract.WIP
{
    /// <summary>
    /// 定义打印操作日志数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IPrintLogContract
    {
         /// <summary>
         /// 添加打印操作日志数据。
         /// </summary>
         /// <param name="obj">打印操作日志数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(PrintLog obj);

         /// <summary>
         /// 修改打印操作日志数据。
         /// </summary>
         /// <param name="obj">打印操作日志数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(PrintLog obj);

         /// <summary>
         /// 删除打印操作日志数据。
         /// </summary>
         /// <param name="key">打印操作日志数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(string key);

         /// <summary>
         /// 获取打印操作日志数据。
         /// </summary>
         /// <param name="key">打印操作日志数据标识符.</param>
         /// <returns>MethodReturnResult&lt;PrintLog&gt;，打印操作日志数据.</returns>
         [OperationContract]
         MethodReturnResult<PrintLog> Get(string key);

         /// <summary>
         /// 获取打印操作日志数据集合。
         /// </summary>
         /// <param name="cfg">查询打印操作日志。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;PrintLog&gt;&gt;，打印操作日志数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<PrintLog>> Get(ref PagingConfig cfg);
    }
}
