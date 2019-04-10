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
    /// 定义月排班计划服务契约。
    /// </summary>
     [ServiceContract]
    public interface IScheduleMonthContract
    {
         /// <summary>
         /// 添加月排班计划。
         /// </summary>
         /// <param name="obj">月排班计划数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(ScheduleMonth obj);
         /// <summary>
         /// 修改月排班计划。
         /// </summary>
         /// <param name="obj">月排班计划数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(ScheduleMonth obj);
         /// <summary>
         /// 删除月排班计划。
         /// </summary>
         /// <param name="key">月排班计划标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(ScheduleMonthKey key);
         /// <summary>
         /// 获取月排班计划数据。
         /// </summary>
         /// <param name="key">月排班计划标识符.</param>
         /// <returns>MethodReturnResult&lt;ScheduleMonth&gt;，月排班计划数据.</returns>
         [OperationContract]
         MethodReturnResult<ScheduleMonth> Get(ScheduleMonthKey key);
         /// <summary>
         /// 获取月排班计划数据集合。
         /// </summary>
         /// <param name="cfg">查询参数。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;ScheduleMonth&gt;&gt;，月排班计划数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<ScheduleMonth>> Get(ref PagingConfig cfg);
    }
}
