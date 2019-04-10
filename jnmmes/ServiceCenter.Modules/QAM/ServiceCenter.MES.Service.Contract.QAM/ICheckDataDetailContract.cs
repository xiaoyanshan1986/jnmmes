using ServiceCenter.MES.Model.QAM;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.Contract.QAM
{
    /// <summary>
    /// 定义检验数据明细服务契约。
    /// </summary>
     [ServiceContract]
    public interface ICheckDataDetailContract
    {
         /// <summary>
         /// 添加检验数据明细。
         /// </summary>
         /// <param name="obj">检验数据明细。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(CheckDataDetail obj);
         /// <summary>
         /// 修改检验数据明细。
         /// </summary>
         /// <param name="obj">检验数据明细。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(CheckDataDetail obj);
         /// <summary>
         /// 删除检验数据明细。
         /// </summary>
         /// <param name="key">检验数据明细标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(CheckDataDetailKey key);
         /// <summary>
         /// 获取检验数据明细。
         /// </summary>
         /// <param name="key">检验数据明细标识符.</param>
         /// <returns>MethodReturnResult&lt;CheckDataDetail&gt;，检验数据明细.</returns>
         [OperationContract]
         MethodReturnResult<CheckDataDetail> Get(CheckDataDetailKey key);
         /// <summary>
         /// 获取检验数据明细集合。
         /// </summary>
         /// <param name="cfg">查询检验参数。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;CheckDataDetail&gt;&gt;，检验数据明细集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<CheckDataDetail>> Get(ref PagingConfig cfg);
    }
}
