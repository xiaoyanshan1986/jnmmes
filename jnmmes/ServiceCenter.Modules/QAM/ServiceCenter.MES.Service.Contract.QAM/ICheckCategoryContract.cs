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
    /// 定义检验参数组数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface ICheckCategoryContract
    {
         /// <summary>
         /// 添加检验参数组数据。
         /// </summary>
         /// <param name="obj">检验参数组数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(CheckCategory obj);
         /// <summary>
         /// 修改检验参数组数据。
         /// </summary>
         /// <param name="obj">检验参数组数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(CheckCategory obj);
         /// <summary>
         /// 删除检验参数组数据。
         /// </summary>
         /// <param name="key">检验参数组数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(string key);
         /// <summary>
         /// 获取检验参数组数据。
         /// </summary>
         /// <param name="key">检验参数组数据标识符.</param>
         /// <returns>MethodReturnResult&lt;CheckCategory&gt;，检验参数组数据.</returns>
         [OperationContract]
         MethodReturnResult<CheckCategory> Get(string key);
         /// <summary>
         /// 获取检验参数组数据集合。
         /// </summary>
         /// <param name="cfg">查询检验参数组。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;CheckCategory&gt;&gt;，检验参数组数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<CheckCategory>> Get(ref PagingConfig cfg);
    }
}
