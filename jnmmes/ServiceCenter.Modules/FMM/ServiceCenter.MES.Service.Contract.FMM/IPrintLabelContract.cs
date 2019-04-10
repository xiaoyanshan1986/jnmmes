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
    /// 定义打印标签数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IPrintLabelContract
    {
         /// <summary>
         /// 添加打印标签数据。
         /// </summary>
         /// <param name="obj">打印标签数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(PrintLabel obj);
         /// <summary>
         /// 修改打印标签数据。
         /// </summary>
         /// <param name="obj">打印标签数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(PrintLabel obj);
         /// <summary>
         /// 删除打印标签数据。
         /// </summary>
         /// <param name="key">打印标签数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(string key);
         /// <summary>
         /// 获取打印标签数据。
         /// </summary>
         /// <param name="key">打印标签数据标识符.</param>
         /// <returns>MethodReturnResult&lt;PrintLabel&gt;，打印标签数据.</returns>
         [OperationContract]
         MethodReturnResult<PrintLabel> Get(string key);
         /// <summary>
         /// 获取打印标签数据集合。
         /// </summary>
         /// <param name="cfg">查询打印标签。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;PrintLabel&gt;&gt;，打印标签数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<PrintLabel>> Get(ref PagingConfig cfg);
    }
}
