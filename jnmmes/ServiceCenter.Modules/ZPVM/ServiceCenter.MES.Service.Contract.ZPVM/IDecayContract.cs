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
    /// 定义衰减数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IDecayContract
    {
         /// <summary>
         /// 添加衰减数据。
         /// </summary>
         /// <param name="obj">衰减数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(Decay obj);
         /// <summary>
         /// 修改衰减数据。
         /// </summary>
         /// <param name="obj">衰减数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(Decay obj);
         /// <summary>
         /// 删除衰减数据。
         /// </summary>
         /// <param name="key">衰减数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(DecayKey key);
         /// <summary>
         /// 获取衰减数据。
         /// </summary>
         /// <param name="key">衰减数据标识符.</param>
         /// <returns>MethodReturnResult&lt;Decay&gt;，衰减数据.</returns>
         [OperationContract]
         MethodReturnResult<Decay> Get(DecayKey key);
         /// <summary>
         /// 获取衰减数据集合。
         /// </summary>
         /// <param name="cfg">查询衰减。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;Decay&gt;&gt;，衰减数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<Decay>> Get(ref PagingConfig cfg);
    }
}
