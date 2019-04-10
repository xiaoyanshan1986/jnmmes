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
    /// 定义检验参数数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface ICheckCategoryDetailContract
    {
         /// <summary>
         /// 添加检验参数数据。
         /// </summary>
         /// <param name="obj">检验参数数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(CheckCategoryDetail obj);
         /// <summary>
         /// 修改检验参数数据。
         /// </summary>
         /// <param name="obj">检验参数数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(CheckCategoryDetail obj);
         /// <summary>
         /// 删除检验参数数据。
         /// </summary>
         /// <param name="key">检验参数数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(CheckCategoryDetailKey key);
         /// <summary>
         /// 获取检验参数数据。
         /// </summary>
         /// <param name="key">检验参数数据标识符.</param>
         /// <returns>MethodReturnResult&lt;CheckCategoryDetail&gt;，检验参数数据.</returns>
         [OperationContract]
         MethodReturnResult<CheckCategoryDetail> Get(CheckCategoryDetailKey key);
         /// <summary>
         /// 获取检验参数数据集合。
         /// </summary>
         /// <param name="cfg">查询检验参数。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;CheckCategoryDetail&gt;&gt;，检验参数数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<CheckCategoryDetail>> Get(ref PagingConfig cfg);
    }
}
