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
    /// 定义物料报废数据服务契约。
    /// </summary>
    [ServiceContract]
    public interface IMaterialScrapContract
    {
        /// <summary>
        /// 添加物料报废数据。
        /// </summary>
        /// <param name="obj">物料报废数据。</param>
        /// <param name="lstDetail">物料报废明细数据。</param>
        /// <returns><see cref="MethodReturnResult"/></returns>
        [OperationContract]
        MethodReturnResult Add(MaterialScrap obj, IList<MaterialScrapDetail> lstDetail);
        /// <summary>
        /// 获取物料报废数据。
        /// </summary>
        /// <param name="key">物料报废数据标识符.</param>
        /// <returns>MethodReturnResult&lt;MaterialScrap&gt;，物料报废数据.</returns>
        [OperationContract]
        MethodReturnResult<MaterialScrap> Get(string key);
        /// <summary>
        /// 获取物料报废数据集合。
        /// </summary>
        /// <param name="cfg">查询物料报废。</param>
        /// <returns>MethodReturnResult&lt;IList&lt;MaterialScrap&gt;&gt;，物料报废数据集合。</returns>
        [OperationContract(Name = "GetList")]
        MethodReturnResult<IList<MaterialScrap>> Get(ref PagingConfig cfg);
        /// <summary>
        /// 获取物料报废明细数据。
        /// </summary>
        /// <param name="key">物料报废明细数据标识符.</param>
        /// <returns>MethodReturnResult&lt;MaterialScrapDetail&gt;，物料报废明细数据.</returns>
        [OperationContract]
        MethodReturnResult<MaterialScrapDetail> GetDetail(MaterialScrapDetailKey key);
        /// <summary>
        /// 获取物料报废明细数据集合。
        /// </summary>
        /// <param name="cfg">查询物料报废明细。</param>
        /// <returns>MethodReturnResult&lt;IList&lt;MaterialScrapDetail&gt;&gt;，物料报废明细数据集合。</returns>
        [OperationContract(Name = "GetDetailList")]
        MethodReturnResult<IList<MaterialScrapDetail>> GetDetail(ref PagingConfig cfg);

        MethodReturnResult<MaterialScrap> Delete(string ScrapNo);
    }
}
