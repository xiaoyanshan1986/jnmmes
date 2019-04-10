using ServiceCenter.MES.Model.SPC;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.Contract.SPC
{
    [ServiceContract]
    public interface ISPCJobContract
    {
        /// <summary>
        /// 添加SPC规则
        /// </summary>
        /// <param name="obj">SPC规则数据</param>
        /// <returns><see cref="MethodReturnResult"/></returns>
        [OperationContract]
        MethodReturnResult Add(SPCJob obj);
        /// <summary>
        /// 修改SPC规则。
        /// </summary>
        /// <param name="obj">SPC规则数据。</param>
        /// <returns><see cref="MethodReturnResult" /></returns>
        [OperationContract]
        MethodReturnResult Modify(SPCJob obj);

        /// <summary>
        /// 删除SPC规则数据。
        /// </summary>
        /// <param name="key">SPC规则数据标识符.</param>
        /// <returns>MethodReturnResult.</returns>
        [OperationContract]
        MethodReturnResult Delete(string key);
        /// <summary>
        /// 获取SPC规则数据
        /// </summary>
        /// <param name="key">SPC规则数据识符.</param>
        /// <returns>MethodReturnResult&lt;Equipment&gt;，设备数据.</returns>
        [OperationContract]
        MethodReturnResult<SPCJob> Get(string key);

        /// <summary>
        /// 获取分档规则数据。
        /// </summary>
        /// <param name="cfg">查询SPC规则数据。</param>
        /// <returns>MethodReturnResult&lt;IList&lt;BinRule&gt;&gt;，分档规则集合。</returns>
        [OperationContract(Name = "GetList")]
        MethodReturnResult<IList<SPCJob>> Get(ref PagingConfig cfg);

        /// <summary>
        /// 获取JOB的参数信息
        /// </summary>
        /// <param name="JobId">JOBID</param>
        /// <returns></returns>
        [OperationContract]
        MethodReturnResult<IList<SPCJobParam>> GetJobParams(string JobId);
        /// <summary>
        /// 删除jobparam
        /// </summary>
        /// <param name="JobId"></param>
        /// <returns></returns>
        [OperationContract]
        MethodReturnResult DeleteJobParams(SPCJobParamKey key);

        /// <summary>
        /// 修改SPCJob规则。
        /// </summary>
        /// <param name="obj">SPCjob规则数据。</param>
        /// <returns><see cref="MethodReturnResult" /></returns>
        [OperationContract]
        MethodReturnResult ModifyJobParams(SPCJobParam obj);
        /// <summary>
        /// 添加SPCjob规则
        /// </summary>
        /// <param name="obj">SPC规则数据</param>
        /// <returns><see cref="MethodReturnResult"/></returns>
        [OperationContract]
        MethodReturnResult AddJobParams(SPCJobParam obj);
        /// <summary>
        /// 添加SPCjob规则
        /// </summary>
        /// <param name="obj">SPC规则数据</param>
        /// <returns><see cref="MethodReturnResult"/></returns>
        [OperationContract]
        MethodReturnResult<SPCJobParam> GetSPCJobParam(SPCJobParamKey key);
    }
}
