using ServiceCenter.MES.Model.LSM;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.Contract.LSM
{
    /// <summary>
    /// 定义下料数据服务契约。
    /// </summary>
    [ServiceContract]
    public interface IMaterialUnloadingContract
    {
        /// <summary>
        /// 添加下料数据。
        /// </summary>
        /// <param name="obj">下料数据。</param>
        /// <param name="lstDetail">下料明细数据。</param>
        /// <returns><see cref="MethodReturnResult"/></returns>
        [OperationContract]
        MethodReturnResult Add(MaterialUnloading obj, IList<MaterialUnloadingDetail> lstDetail);
        /// <summary>
        /// 获取下料数据。
        /// </summary>
        /// <param name="key">下料数据标识符.</param>
        /// <returns>MethodReturnResult&lt;MaterialUnloading&gt;，下料数据.</returns>
        [OperationContract]
        MethodReturnResult<MaterialUnloading> Get(string key);
        /// <summary>
        /// 获取下料数据集合。
        /// </summary>
        /// <param name="cfg">查询下料。</param>
        /// <returns>MethodReturnResult&lt;IList&lt;MaterialUnloading&gt;&gt;，下料数据集合。</returns>
        [OperationContract(Name = "GetList")]
        MethodReturnResult<IList<MaterialUnloading>> Get(ref PagingConfig cfg);
        /// <summary>
        /// 获取下料明细数据。
        /// </summary>
        /// <param name="key">下料明细数据标识符.</param>
        /// <returns>MethodReturnResult&lt;MaterialUnloadingDetail&gt;，下料明细数据.</returns>
        [OperationContract]
        MethodReturnResult<MaterialUnloadingDetail> GetDetail(MaterialUnloadingDetailKey key);
        /// <summary>
        /// 获取下料明细数据集合。
        /// </summary>
        /// <param name="cfg">查询下料明细。</param>
        /// <returns>MethodReturnResult&lt;IList&lt;MaterialUnloadingDetail&gt;&gt;，下料明细数据集合。</returns>
        [OperationContract(Name = "GetDetailList")]
        MethodReturnResult<IList<MaterialUnloadingDetail>> GetDetail(ref PagingConfig cfg);
    }
}
