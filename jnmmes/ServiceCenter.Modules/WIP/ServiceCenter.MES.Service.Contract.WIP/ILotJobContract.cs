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
    /// 定义批次定时作业数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface ILotJobContract
    {
         /// <summary>
         /// 添加批次定时作业数据。
         /// </summary>
         /// <param name="obj">批次定时作业数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(LotJob obj);
         /// <summary>
         /// 修改批次定时作业数据。
         /// </summary>
         /// <param name="obj">批次定时作业数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(LotJob obj);
         /// <summary>
         /// 删除批次定时作业数据。
         /// </summary>
         /// <param name="key">批次定时作业数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(string key);
         /// <summary>
         /// 获取批次定时作业数据。
         /// </summary>
         /// <param name="key">批次定时作业数据标识符.</param>
         /// <returns>MethodReturnResult&lt;LotJob&gt;，批次定时作业数据.</returns>
         [OperationContract]
         MethodReturnResult<LotJob> Get(string key);
         /// <summary>
         /// 获取批次定时作业数据集合。
         /// </summary>
         /// <param name="cfg">查询批次定时作业。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;LotJob&gt;&gt;，批次定时作业数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<LotJob>> Get(ref PagingConfig cfg);
    }
}
