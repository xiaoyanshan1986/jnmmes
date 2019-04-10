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
    /// 定义月生产计划服务契约。
    /// </summary>
     [ServiceContract]
    public interface IPlanMonthContract
    {
         /// <summary>
         /// 添加月生产计划。
         /// </summary>
         /// <param name="obj">月生产计划数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(PlanMonth obj);
         /// <summary>
         /// 修改月生产计划。
         /// </summary>
         /// <param name="obj">月生产计划数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(PlanMonth obj);
         /// <summary>
         /// 删除月生产计划。
         /// </summary>
         /// <param name="key">月生产计划标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(PlanMonthKey key);
         /// <summary>
         /// 获取月生产计划数据。
         /// </summary>
         /// <param name="key">月生产计划标识符.</param>
         /// <returns>MethodReturnResult&lt;PlanMonth&gt;，月生产计划数据.</returns>
         [OperationContract]
         MethodReturnResult<PlanMonth> Get(PlanMonthKey key);
         /// <summary>
         /// 获取月生产计划数据集合。
         /// </summary>
         /// <param name="cfg">查询参数。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;PlanMonth&gt;&gt;，月生产计划数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<PlanMonth>> Get(ref PagingConfig cfg);
    }
}
