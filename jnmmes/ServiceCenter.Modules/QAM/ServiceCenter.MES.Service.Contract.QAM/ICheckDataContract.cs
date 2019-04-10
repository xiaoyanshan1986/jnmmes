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
    /// 定义检验数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface ICheckDataContract
    {
         /// <summary>
         /// 添加检验数据。
         /// </summary>
         /// <param name="obj">检验数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(CheckData obj);
         /// <summary>
         /// 修改检验数据。
         /// </summary>
         /// <param name="obj">检验数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(CheckData obj);
         /// <summary>
         /// 删除检验数据。
         /// </summary>
         /// <param name="key">检验数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(string key);
         /// <summary>
         /// 获取检验数据。
         /// </summary>
         /// <param name="key">检验数据标识符.</param>
         /// <returns>MethodReturnResult&lt;CheckData&gt;，检验数据.</returns>
         [OperationContract]
         MethodReturnResult<CheckData> Get(string key);
         /// <summary>
         /// 获取检验数据集合。
         /// </summary>
         /// <param name="cfg">查询检验参数组。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;CheckData&gt;&gt;，检验数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<CheckData>> Get(ref PagingConfig cfg);
    }
}
