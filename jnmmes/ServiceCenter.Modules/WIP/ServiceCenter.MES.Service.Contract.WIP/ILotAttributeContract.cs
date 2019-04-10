using ServiceCenter.MES.Model.WIP;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.Contract.WIP
{
    /// <summary>
    /// 定义批次属性数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface ILotAttributeContract
    {
         /// <summary>
         /// 添加批次属性数据。
         /// </summary>
         /// <param name="obj">批次属性数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(LotAttribute obj);
         /// <summary>
         /// 修改批次属性数据。
         /// </summary>
         /// <param name="obj">批次属性数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(LotAttribute obj);
         /// <summary>
         /// 删除批次属性数据。
         /// </summary>
         /// <param name="key">批次属性数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(LotAttributeKey key);
         /// <summary>
         /// 获取批次属性数据。
         /// </summary>
         /// <param name="key">批次属性数据标识符.</param>
         /// <returns>MethodReturnResult&lt;LotAttribute&gt;，批次属性数据.</returns>
         [OperationContract]
         MethodReturnResult<LotAttribute> Get(LotAttributeKey key);
         /// <summary>
         /// 获取批次属性数据集合。
         /// </summary>
         /// <param name="cfg">查询批次属性。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;LotAttribute&gt;&gt;，批次属性数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<LotAttribute>> Get(ref PagingConfig cfg);
    }
}
