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
    /// 定义客户端配置数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IClientConfigContract
    {
         /// <summary>
         /// 添加客户端配置数据。
         /// </summary>
         /// <param name="obj">客户端配置数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(ClientConfig obj);
         /// <summary>
         /// 修改客户端配置数据。
         /// </summary>
         /// <param name="obj">客户端配置数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(ClientConfig obj);
         /// <summary>
         /// 删除客户端配置数据。
         /// </summary>
         /// <param name="key">客户端配置数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(string key);
         /// <summary>
         /// 获取客户端配置数据。
         /// </summary>
         /// <param name="key">客户端配置数据标识符.</param>
         /// <returns>MethodReturnResult&lt;ClientConfig&gt;，客户端配置数据.</returns>
         [OperationContract]
         MethodReturnResult<ClientConfig> Get(string key);
         /// <summary>
         /// 获取客户端配置数据集合。
         /// </summary>
         /// <param name="cfg">查询客户端配置。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;ClientConfig&gt;&gt;，客户端配置数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<ClientConfig>> Get(ref PagingConfig cfg);
    }
}
