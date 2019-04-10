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
    /// 定义采集数据明细数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IDataDetailContract
    {
         /// <summary>
         /// 添加采集数据明细数据。
         /// </summary>
         /// <param name="obj">采集数据明细数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(DataDetail obj);
         /// <summary>
         /// 修改采集数据明细数据。
         /// </summary>
         /// <param name="obj">采集数据明细数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(DataDetail obj);
         /// <summary>
         /// 删除采集数据明细数据。
         /// </summary>
         /// <param name="key">采集数据明细数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(DataDetailKey key);
         /// <summary>
         /// 获取采集数据明细数据。
         /// </summary>
         /// <param name="key">采集数据明细数据标识符.</param>
         /// <returns>MethodReturnResult&lt;DataDetail&gt;，采集数据明细数据.</returns>
         [OperationContract]
         MethodReturnResult<DataDetail> Get(DataDetailKey key);
         /// <summary>
         /// 获取采集数据明细数据集合。
         /// </summary>
         /// <param name="cfg">查询采集数据明细。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;DataDetail&gt;&gt;，采集数据明细数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<DataDetail>> Get(ref PagingConfig cfg);
    }
}
