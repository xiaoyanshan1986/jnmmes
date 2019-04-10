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
    /// 定义采集参数组数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface ICategoryContract
    {
         /// <summary>
         /// 添加采集参数组数据。
         /// </summary>
         /// <param name="obj">采集参数组数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(Category obj);
         /// <summary>
         /// 修改采集参数组数据。
         /// </summary>
         /// <param name="obj">采集参数组数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(Category obj);
         /// <summary>
         /// 删除采集参数组数据。
         /// </summary>
         /// <param name="key">采集参数组数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(string key);
         /// <summary>
         /// 获取采集参数组数据。
         /// </summary>
         /// <param name="key">采集参数组数据标识符.</param>
         /// <returns>MethodReturnResult&lt;Category&gt;，采集参数组数据.</returns>
         [OperationContract]
         MethodReturnResult<Category> Get(string key);
         /// <summary>
         /// 获取采集参数组数据集合。
         /// </summary>
         /// <param name="cfg">查询采集参数组。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;Category&gt;&gt;，采集参数组数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<Category>> Get(ref PagingConfig cfg);
    }
}
