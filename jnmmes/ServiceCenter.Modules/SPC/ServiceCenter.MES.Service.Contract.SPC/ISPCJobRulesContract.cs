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
    public interface ISPCJobRulesContract
    {
        /// <summary>
        /// 添加SPC规则
        /// </summary>
        /// <param name="obj">SPC规则数据</param>
        /// <returns><see cref="MethodReturnResult"/></returns>
        [OperationContract]
        MethodReturnResult Add(SPCJobRules obj);
        /// <summary>
        /// 修改SPC规则。
        /// </summary>
        /// <param name="obj">SPC规则数据。</param>
        /// <returns><see cref="MethodReturnResult" /></returns>
        [OperationContract]
        MethodReturnResult Modify(SPCJobRules obj);

        /// <summary>
        /// 删除SPC规则数据。
        /// </summary>
        /// <param name="key">SPC规则数据标识符.</param>
        /// <returns>MethodReturnResult.</returns>
        [OperationContract]
        MethodReturnResult Delete(SPCJobRulesKey key);
        /// <summary>
        /// 获取SPC规则数据
        /// </summary>
        /// <param name="key">SPC规则数据识符.</param>
        /// <returns>MethodReturnResult&lt;Equipment&gt;，设备数据.</returns>
        [OperationContract]
        MethodReturnResult<SPCJobRules> Get(SPCJobRulesKey key);

        /// <summary>
        /// 获取分档规则数据。
        /// </summary>
        /// <param name="cfg">查询SPC规则数据。</param>
        /// <returns>MethodReturnResult&lt;IList&lt;BinRule&gt;&gt;，分档规则集合。</returns>
        [OperationContract(Name = "GetList")]
        MethodReturnResult<IList<SPCJobRules>> Get(ref PagingConfig cfg);
    }
}
