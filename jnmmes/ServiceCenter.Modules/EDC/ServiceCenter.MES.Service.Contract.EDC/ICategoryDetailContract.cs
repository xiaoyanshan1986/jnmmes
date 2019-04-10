using ServiceCenter.MES.Model.EDC;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.Contract.EDC
{
    /// <summary>
    /// 定义采集参数数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface ICategoryDetailContract
    {
         /// <summary>
         /// 添加采集参数数据。
         /// </summary>
         /// <param name="obj">采集参数数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(CategoryDetail obj);
         /// <summary>
         /// 修改采集参数数据。
         /// </summary>
         /// <param name="obj">采集参数数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(CategoryDetail obj);
         /// <summary>
         /// 删除采集参数数据。
         /// </summary>
         /// <param name="key">采集参数数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(CategoryDetailKey key);
         /// <summary>
         /// 获取采集参数数据。
         /// </summary>
         /// <param name="key">采集参数数据标识符.</param>
         /// <returns>MethodReturnResult&lt;CategoryDetail&gt;，采集参数数据.</returns>
         [OperationContract]
         MethodReturnResult<CategoryDetail> Get(CategoryDetailKey key);
         /// <summary>
         /// 获取采集参数数据集合。
         /// </summary>
         /// <param name="cfg">查询采集参数。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;CategoryDetail&gt;&gt;，采集参数数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<CategoryDetail>> Get(ref PagingConfig cfg);
    }
}
