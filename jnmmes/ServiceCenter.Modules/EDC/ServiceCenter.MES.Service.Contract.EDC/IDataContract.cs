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
    /// 定义采集数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IDataContract
    {
         /// <summary>
         /// 添加采集数据。
         /// </summary>
         /// <param name="obj">采集数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(Data obj);
         /// <summary>
         /// 修改采集数据。
         /// </summary>
         /// <param name="obj">采集数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(Data obj);
         /// <summary>
         /// 删除采集数据。
         /// </summary>
         /// <param name="key">采集数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(string key);
         /// <summary>
         /// 获取采集数据。
         /// </summary>
         /// <param name="key">采集数据标识符.</param>
         /// <returns>MethodReturnResult&lt;Data&gt;，采集数据.</returns>
         [OperationContract]
         MethodReturnResult<Data> Get(string key);
         /// <summary>
         /// 获取采集数据集合。
         /// </summary>
         /// <param name="cfg">查询采集数据的。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;Data&gt;&gt;，采集数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<Data>> Get(ref PagingConfig cfg);
    }
}
