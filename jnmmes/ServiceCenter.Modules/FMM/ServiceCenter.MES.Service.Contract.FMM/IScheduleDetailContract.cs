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
    /// 定义排班计划详细数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IScheduleDetailContract
    {
         /// <summary>
         /// 添加排班计划详细数据。
         /// </summary>
         /// <param name="obj">排班计划详细数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(ScheduleDetail obj);
         /// <summary>
         /// 修改排班计划详细数据。
         /// </summary>
         /// <param name="obj">排班计划详细数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(ScheduleDetail obj);
         /// <summary>
         /// 删除排班计划详细数据。
         /// </summary>
         /// <param name="key">排班计划详细数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(ScheduleDetailKey key);
         /// <summary>
         /// 获取排班计划详细数据数据。
         /// </summary>
         /// <param name="key">排班计划详细数据标识符.</param>
         /// <returns>MethodReturnResult&lt;ScheduleDetail&gt;，排班计划详细数据数据.</returns>
         [OperationContract]
         MethodReturnResult<ScheduleDetail> Get(ScheduleDetailKey key);
         /// <summary>
         /// 获取排班计划详细数据数据集合。
         /// </summary>
         /// <param name="cfg">查询参数。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;ScheduleDetail&gt;&gt;，排班计划详细数据数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<ScheduleDetail>> Get(ref PagingConfig cfg);
    }
}
