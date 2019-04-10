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
    /// 定义日生产计划服务契约。
    /// </summary>
    [ServiceContract]
    public interface IPlanDayContract
    {
        /// <summary>
        /// 添加日生产计划。
        /// </summary>
        /// <param name="obj">日生产计划。</param>
        /// <returns><see cref="MethodReturnResult"/></returns>
        [OperationContract]
        MethodReturnResult Add(PlanDay obj);

        /// <summary>
        /// 修改日生产计划。
        /// </summary>
        /// <param name="obj">日生产计划。</param>
        /// <returns><see cref="MethodReturnResult" /></returns>
        [OperationContract]
        MethodReturnResult Modify(PlanDay obj);

        /// <summary>
        /// 修改日生产计划。
        /// </summary>
        /// <param name="lst">日生产计划集合。</param>
        /// <returns><see cref="MethodReturnResult" /></returns>
        [OperationContract(Name = "ModifyList")]
        MethodReturnResult Modify(IList<PlanDay> lst);

        /// <summary>
        /// 删除日生产计划数据。
        /// </summary>
        /// <param name="key">日生产计划标识符.</param>
        /// <returns>MethodReturnResult.</returns>
        [OperationContract]
        MethodReturnResult Delete(PlanDayKey key);

        /// <summary>
        /// 获取日生产计划数据。
        /// </summary>
        /// <param name="key">日生产计划标识符.</param>
        /// <returns>MethodReturnResult&lt;PlanDay&gt;，日生产计划数据.</returns>
        [OperationContract]
        MethodReturnResult<PlanDay> Get(PlanDayKey key);

        /// <summary>
        /// 获取日生产计划数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns>MethodReturnResult&lt;IList&lt;PlanDay&gt;&gt;，日生产计划数据集合。</returns>
        [OperationContract(Name = "GetList")]
        MethodReturnResult<IList<PlanDay>> Get(ref PagingConfig cfg);
    }
}
