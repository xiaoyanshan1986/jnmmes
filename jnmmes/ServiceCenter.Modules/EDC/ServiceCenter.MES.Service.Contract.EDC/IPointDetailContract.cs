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
    /// 定义采集点设置明细数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IPointDetailContract
    {
         /// <summary>
         /// 添加采集点设置明细数据。
         /// </summary>
         /// <param name="obj">采集点设置明细数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(PointDetail obj);
         /// <summary>
         /// 修改采集点设置明细数据。
         /// </summary>
         /// <param name="obj">采集点设置明细数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(PointDetail obj);
         /// <summary>
         /// 删除采集点设置明细数据。
         /// </summary>
         /// <param name="key">采集点设置明细数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(PointDetailKey key);
         /// <summary>
         /// 获取采集点设置明细数据。
         /// </summary>
         /// <param name="key">采集点设置明细数据标识符.</param>
         /// <returns>MethodReturnResult&lt;PointDetail&gt;，采集点设置明细数据.</returns>
         [OperationContract]
         MethodReturnResult<PointDetail> Get(PointDetailKey key);
         /// <summary>
         /// 获取采集点设置明细数据集合。
         /// </summary>
         /// <param name="cfg">查询采集点设置明细。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;PointDetail&gt;&gt;，采集点设置明细数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<PointDetail>> Get(ref PagingConfig cfg);
    }
}
