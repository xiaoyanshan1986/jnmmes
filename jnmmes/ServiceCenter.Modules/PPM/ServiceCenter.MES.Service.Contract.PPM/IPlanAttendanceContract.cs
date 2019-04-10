using ServiceCenter.MES.Model.PPM;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.Contract.PPM
{
    /// <summary>
    /// 定义日排班计划服务契约。
    /// </summary>
    [ServiceContract]
    public interface IPlanAttendanceContract
    {
        /// <summary>
        /// 添加日排班计划。
        /// </summary>
        /// <param name="obj">日排班计划。</param>
        /// <returns><see cref="MethodReturnResult"/></returns>
        [OperationContract]
        MethodReturnResult Add(PlanAttendance obj);

        /// <summary>
        /// 修改日排班计划。
        /// </summary>
        /// <param name="obj">日排班计划。</param>
        /// <returns><see cref="MethodReturnResult" /></returns>
        [OperationContract]
        MethodReturnResult Modify(PlanAttendance obj);

        /// <summary>
        /// 修改日排班计划。
        /// </summary>
        /// <param name="lst">日排班计划集合。</param>
        /// <returns><see cref="MethodReturnResult" /></returns>
        [OperationContract(Name = "ModifyList")]
        MethodReturnResult Modify(IList<PlanAttendance> lst);

        /// <summary>
        /// 删除日排班计划数据。
        /// </summary>
        /// <param name="key">日排班计划标识符.</param>
        /// <returns>MethodReturnResult.</returns>
        [OperationContract]
        MethodReturnResult Delete(PlanAttendanceKey key);

        /// <summary>
        /// 获取日排班计划数据。
        /// </summary>
        /// <param name="key">日排班计划标识符.</param>
        /// <returns>MethodReturnResult&lt;PlanAttendance&gt;，日排班计划数据.</returns>
        [OperationContract]
        MethodReturnResult<PlanAttendance> Get(PlanAttendanceKey key);

        /// <summary>
        /// 获取日排班计划数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns>MethodReturnResult&lt;IList&lt;PlanAttendance&gt;&gt;，日排班计划数据集合。</returns>
        [OperationContract(Name = "GetList")]
        MethodReturnResult<IList<PlanAttendance>> Get(ref PagingConfig cfg);
    }
}
