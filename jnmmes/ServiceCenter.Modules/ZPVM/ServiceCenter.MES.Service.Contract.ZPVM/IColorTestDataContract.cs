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
    /// 定义花色测试数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IColorTestDataContract
    {
         /// <summary>
        /// 添加花色测试数据。
         /// </summary>
        /// <param name="obj">花色测试数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(ColorTestData obj);
         /// <summary>
         /// 修改花色测试数据。
         /// </summary>
         /// <param name="obj">花色测试数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(ColorTestData obj);
         /// <summary>
         /// 删除花色测试数据。
         /// </summary>
         /// <param name="key">花色测试数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(ColorTestDataKey key);
         /// <summary>
         /// 获取IV测试数据。
         /// </summary>
         /// <param name="key">花色测试数据标识符.</param>
         /// <returns>MethodReturnResult&lt;CoroTestData&gt;，花色测试数据.</returns>
         [OperationContract]
         MethodReturnResult<ColorTestData> Get(ColorTestDataKey key);
         /// <summary>
         /// 获取IV测试数据集合。
         /// </summary>
         /// <param name="cfg">查询IV测试数据。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;CoroTestData&gt;&gt;，花色测试数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<ColorTestData>> Get(ref PagingConfig cfg);
    }
}
