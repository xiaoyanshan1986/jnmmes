using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.Contract.ZPVM
{
    /// <summary>
    /// 定义IV测试数据打印日志服务契约。
    /// </summary>
     [ServiceContract]
    public interface IIVTestDataPrintLogContract
    {
         /// <summary>
         /// 添加IV测试数据打印日志。
         /// </summary>
         /// <param name="obj">IV测试数据打印日志。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(IVTestDataPrintLog obj);
         /// <summary>
         /// 修改IV测试数据打印日志。
         /// </summary>
         /// <param name="obj">IV测试数据打印日志。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(IVTestDataPrintLog obj);
         /// <summary>
         /// 删除IV测试数据打印日志。
         /// </summary>
         /// <param name="key">IV测试数据打印日志标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(IVTestDataPrintLogKey key);
         /// <summary>
         /// 获取IV测试数据打印日志。
         /// </summary>
         /// <param name="key">IV测试数据打印日志标识符.</param>
         /// <returns>MethodReturnResult&lt;IVTestDataPrintLog&gt;，IV测试数据打印日志.</returns>
         [OperationContract]
         MethodReturnResult<IVTestDataPrintLog> Get(IVTestDataPrintLogKey key);
         /// <summary>
         /// 获取IV测试数据打印日志集合。
         /// </summary>
         /// <param name="cfg">查询IV测试数据打印日志。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;IVTestDataPrintLog&gt;&gt;，IV测试数据打印日志集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<IVTestDataPrintLog>> Get(ref PagingConfig cfg);
    }
}
