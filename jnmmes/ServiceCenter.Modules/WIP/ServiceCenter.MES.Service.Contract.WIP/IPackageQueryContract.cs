using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.Model;
using System.Data;
using System.Runtime.Serialization;
using ServiceCenter.Common.Model;
namespace ServiceCenter.MES.Service.Contract.WIP
{

     [DataContract]
    public class RPTpackagelistParameter : BaseMethodParameter
    {

        [DataMember]
        public string PackageNo { get; set; }

        [DataMember]
        public string ErrorMsg { get; set; }

        /// <summary>
        /// 页号。
        /// </summary>
        [DataMember]
        public int PageNo { get; set; }
        /// <summary>
        /// 每页大小。
        /// </summary>
        [DataMember]
        public int PageSize { get; set; }
        /// <summary>
        /// 总记录数。
        /// </summary>
        [DataMember]
        public int TotalRecords { get; set; }

        /// <summary>
        /// 批次号。
        /// </summary>
        [DataMember]
        public string LotNumber { get; set; }

    }
    /// <summary>
    /// 包装查询契约接口。
    /// </summary>
    [ServiceContract]
    public interface IPackageQueryContract
    {
        /// <summary>
        /// 获取包装数据。。
        /// </summary>
        /// <param name="key">包装标识符.</param>
        /// <returns>MethodReturnResult&lt;Package&gt;，包装数据.</returns>
        [OperationContract]
        MethodReturnResult<Package> Get(string key);
        /// <summary>
        /// 获取包装数据集合。
        /// </summary>
        /// <param name="cfg">查询条件。</param>
        /// <returns>MethodReturnResult&lt;IList&lt;Package&gt;&gt;，包装数据。</returns>
        [OperationContract(Name = "GetList")]
        MethodReturnResult<IList<Package>> Get(ref PagingConfig cfg);
        /// <summary>
        /// 获取包装明细数据。
        /// </summary>
        /// <param name="key">包装明细数据标识符.</param>
        /// <returns>MethodReturnResult&lt;PackageDetail&gt;，包装明细数据.</returns>
        [OperationContract]
        MethodReturnResult<PackageDetail> GetDetail(PackageDetailKey key);
        /// <summary>
        /// 获取包装明细数据集合。
        /// </summary>
        /// <param name="cfg">查询条件。</param>
        /// <returns>MethodReturnResult&lt;IList&lt;PackageDetail&gt;&gt;，包装明细数据集合。</returns>
        [OperationContract(Name = "GetDetailList")]
        MethodReturnResult<IList<PackageDetail>> GetDetail(ref PagingConfig cfg);

        [OperationContract]
        MethodReturnResult<DataSet> GetPackageTransaction(string p);
        [OperationContract]
        MethodReturnResult CleanBin(string lineCode, string binNo);
        [OperationContract]
        MethodReturnResult<DataSet> GetRPTpackagelist(RPTpackagelistParameter p);

        [OperationContract]
        MethodReturnResult<DataSet> GetOEMpackagelist(RPTpackagelistParameter p);

        [OperationContract]
        MethodReturnResult<DataSet> GetRPTpackagelistQueryDb(ref RPTpackagelistParameter p);

        /// <summary> 存储过程获取包装历史记录数据查询 </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [OperationContract]
        MethodReturnResult<DataSet> GetPackageTransactionQueryDb(ref RPTpackagelistParameter p);


        /// <summary>
        /// 获取历史包装号数据
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [OperationContract]
        MethodReturnResult<DataSet> GetRPTPackageNoInfo(RPTpackagelistParameter p);

        /// <summary>
        /// 添加包装号描述
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [OperationContract]
        MethodReturnResult UpdateAdd(Package obj, string action);
      
    }
}
