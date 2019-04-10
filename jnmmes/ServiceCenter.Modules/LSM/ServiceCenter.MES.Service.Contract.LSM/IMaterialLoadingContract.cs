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
    /// 定义上料数据服务契约。
    /// </summary>
    [ServiceContract]
    public interface IMaterialLoadingContract
    {
        /// <summary>
        /// 添加上料数据。
        /// </summary>
        /// <param name="obj">上料数据。</param>
        /// <param name="lstDetail">上料明细数据。</param>
        /// <returns><see cref="MethodReturnResult"/></returns>
        [OperationContract]
        MethodReturnResult Add(MaterialLoading obj, IList<MaterialLoadingDetail> lstDetail);
        /// <summary>
        /// 获取上料数据。
        /// </summary>
        /// <param name="key">上料数据标识符.</param>
        /// <returns>MethodReturnResult&lt;MaterialLoading&gt;，上料数据.</returns>
        [OperationContract]
        MethodReturnResult<MaterialLoading> Get(string key);
        /// <summary>
        /// 获取上料数据集合。
        /// </summary>
        /// <param name="cfg">查询上料。</param>
        /// <returns>MethodReturnResult&lt;IList&lt;MaterialLoading&gt;&gt;，上料数据集合。</returns>
        [OperationContract(Name = "GetList")]
        MethodReturnResult<IList<MaterialLoading>> Get(ref PagingConfig cfg);
        /// <summary>
        /// 获取上料明细数据。
        /// </summary>
        /// <param name="key">上料明细数据标识符.</param>
        /// <returns>MethodReturnResult&lt;MaterialLoadingDetail&gt;，上料明细数据.</returns>
        [OperationContract]
        MethodReturnResult<MaterialLoadingDetail> GetDetail(MaterialLoadingDetailKey key);
        /// <summary>
        /// 获取上料明细数据集合。
        /// </summary>
        /// <param name="cfg">查询上料明细。</param>
        /// <returns>MethodReturnResult&lt;IList&lt;MaterialLoadingDetail&gt;&gt;，上料明细数据集合。</returns>
        [OperationContract(Name = "GetDetailList")]
        MethodReturnResult<IList<MaterialLoadingDetail>> GetDetail(ref PagingConfig cfg);
    }
}
