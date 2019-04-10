using ServiceCenter.Common.Model;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.Contract.ZPVM
{
    /// <summary>
    /// 定义IV测试数据服务契约。
    /// </summary>
    [ServiceContract]
    public interface IVIRTestDataContract
    {
        /// <summary>
        /// 添加安规测试数据。
        /// </summary>
        /// <param name="obj">安规测试数据。</param>
        /// <returns><see cref="MethodReturnResult"/></returns>
        [OperationContract]
        MethodReturnResult Add(VIRTestData obj);

        /// <summary>
        /// 修改安规测试数据。
        /// </summary>
        /// <param name="obj">安规测试数据。</param>
        /// <returns><see cref="MethodReturnResult" /></returns>
        [OperationContract]
        MethodReturnResult Modify(VIRTestData obj);

        /// <summary>
        /// 删除安规测试数据。
        /// </summary>
        /// <param name="key">安规测试数据标识符.</param>
        /// <returns>MethodReturnResult.</returns>
        [OperationContract]
        MethodReturnResult Delete(VIRTestDataKey key);

        /// <summary>
        /// 获取安规测试数据。
        /// </summary>
        /// <param name="key">安规测试数据标识符.</param>
        /// <returns>MethodReturnResult&lt;安规TestData&gt;，安规测试数据.</returns>
        [OperationContract]
        MethodReturnResult<VIRTestData> Get(VIRTestDataKey key);

        /// <summary>
        /// 获取安规测试数据集合。
        /// </summary>
        /// <param name="cfg">查询安规测试数据。</param>
        /// <returns>MethodReturnResult&lt;IList&lt;安规TestData&gt;&gt;，安规测试数据集合。</returns>
        [OperationContract(Name = "GetList")]
        MethodReturnResult<IList<VIRTestData>> Get(ref PagingConfig cfg);


    }
}
