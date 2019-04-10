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
    /// 定义客户端配置属性数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IClientConfigAttributeContract
    {
         /// <summary>
         /// 添加客户端配置属性数据。
         /// </summary>
         /// <param name="obj">客户端配置属性数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(ClientConfigAttribute obj);
         /// <summary>
         /// 修改客户端配置属性数据。
         /// </summary>
         /// <param name="obj">客户端配置属性数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(ClientConfigAttribute obj);
         /// <summary>
         /// 删除客户端配置属性数据。
         /// </summary>
         /// <param name="key">客户端配置属性数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(ClientConfigAttributeKey key);
         /// <summary>
         /// 获取客户端配置属性数据。
         /// </summary>
         /// <param name="key">客户端配置属性数据标识符.</param>
         /// <returns>MethodReturnResult&lt;ClientConfigAttribute&gt;，客户端配置属性数据.</returns>
         [OperationContract]
         MethodReturnResult<ClientConfigAttribute> Get(ClientConfigAttributeKey key);
         /// <summary>
         /// 获取客户端配置属性数据集合。
         /// </summary>
         /// <param name="cfg">查询客户端配置属性。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;ClientConfigAttribute&gt;&gt;，客户端配置属性数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<ClientConfigAttribute>> Get(ref PagingConfig cfg);
    }
}
