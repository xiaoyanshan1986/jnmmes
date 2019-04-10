using ServiceCenter.MES.Model.QAM;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.Contract.QAM
{
    /// <summary>
    /// 定义检验计划服务契约。
    /// </summary>
     [ServiceContract]
    public interface ICheckPlanContract
    {
         /// <summary>
         /// 添加检验计划。
         /// </summary>
         /// <param name="obj">检验计划。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(CheckPlan obj);
         /// <summary>
         /// 修改检验计划。
         /// </summary>
         /// <param name="obj">检验计划。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(CheckPlan obj);
         /// <summary>
         /// 删除检验计划。
         /// </summary>
         /// <param name="key">检验计划标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(string key);
         /// <summary>
         /// 获取检验计划。
         /// </summary>
         /// <param name="key">检验计划标识符.</param>
         /// <returns>MethodReturnResult&lt;CheckPlan&gt;，检验计划.</returns>
         [OperationContract]
         MethodReturnResult<CheckPlan> Get(string key);
         /// <summary>
         /// 获取检验计划集合。
         /// </summary>
         /// <param name="cfg">查询采集计划。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;CheckPlan&gt;&gt;，检验计划集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<CheckPlan>> Get(ref PagingConfig cfg);
    }
}
