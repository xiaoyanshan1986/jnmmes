using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.Model;
using System.Data;

namespace ServiceCenter.MES.Service.Contract.WIP
{
    /// <summary>
    /// 包装查询契约接口。
    /// </summary>
    [ServiceContract]
    public interface IPackageOemQueryContract
    {
        /// <summary>
        /// 获取包装数据。。
        /// </summary>
        /// <param name="key">包装标识符.</param>
        /// <returns>MethodReturnResult&lt;Package&gt;，包装数据.</returns>
        [OperationContract]
        MethodReturnResult<PackageOemDetail> Get(PackageOemDetailKey key);
        /// <summary>
        /// 获取包装数据集合。
        /// </summary>
        /// <param name="cfg">查询条件。</param>
        /// <returns>MethodReturnResult&lt;IList&lt;Package&gt;&gt;，包装数据。</returns>
        [OperationContract(Name = "GetList")]
        MethodReturnResult<IList<PackageOemDetail>> Get(ref PagingConfig cfg);
        /// <summary>
        /// 获取包装明细数据。
        /// </summary>
        /// <param name="key">包装明细数据标识符.</param>
        /// <returns>MethodReturnResult&lt;PackageDetail&gt;，包装明细数据.</returns>
        [OperationContract]
        MethodReturnResult<PackageOemDetail> GetDetail(PackageOemDetailKey key);
        /// <summary>
        /// 获取包装明细数据集合。
        /// </summary>
        /// <param name="cfg">查询条件。</param>
        /// <returns>MethodReturnResult&lt;IList&lt;PackageDetail&gt;&gt;，包装明细数据集合。</returns>
        [OperationContract(Name = "GetDetailList")]
        MethodReturnResult<IList<PackageOemDetail>> GetDetail(ref PagingConfig cfg);

        [OperationContract]
        MethodReturnResult<DataSet> GetPackageTransaction(string p);

        [OperationContract]
        MethodReturnResult CleanBin(string lineCode, string binNo);
        
    }
}
