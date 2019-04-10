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
    /// 定义日目标参数服务契约。
    /// </summary>
    [ServiceContract]
    public interface ITargetParameterContract
    {
        /// <summary>
        /// 添加日目标参数。
        /// </summary>
        /// <param name="obj">日目标参数。</param>
        /// <returns><see cref="MethodReturnResult"/></returns>
        [OperationContract]
        MethodReturnResult Add(TargetParameter obj);

        /// <summary>
        /// 修改日目标参数。
        /// </summary>
        /// <param name="obj">日目标参数。</param>
        /// <returns><see cref="MethodReturnResult" /></returns>
        [OperationContract]
        MethodReturnResult Modify(TargetParameter obj);

        /// <summary>
        /// 修改日目标参数。
        /// </summary>
        /// <param name="lst">日目标参数集合。</param>
        /// <returns><see cref="MethodReturnResult" /></returns>
        [OperationContract(Name = "ModifyList")]
        MethodReturnResult Modify(IList<TargetParameter> lst);

        /// <summary>
        /// 删除日目标参数数据。
        /// </summary>
        /// <param name="key">日目标参数标识符.</param>
        /// <returns>MethodReturnResult.</returns>
        [OperationContract]
        MethodReturnResult Delete(TargetParameterKey key);

        /// <summary>
        /// 获取日目标参数数据。
        /// </summary>
        /// <param name="key">日目标参数标识符.</param>
        /// <returns>MethodReturnResult&lt;TargetParameter&gt;，日目标参数数据.</returns>
        [OperationContract]
        MethodReturnResult<TargetParameter> Get(TargetParameterKey key);

        /// <summary>
        /// 获取日目标参数数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns>MethodReturnResult&lt;IList&lt;TargetParameter&gt;&gt;，日目标参数数据集合。</returns>
        [OperationContract(Name = "GetList")]
        MethodReturnResult<IList<TargetParameter>> Get(ref PagingConfig cfg);
    }
}
