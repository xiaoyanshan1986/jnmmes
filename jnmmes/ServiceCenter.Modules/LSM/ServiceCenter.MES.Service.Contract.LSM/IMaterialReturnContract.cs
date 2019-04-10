using ServiceCenter.MES.Model.LSM;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using ServiceCenter.Common.Model;
using System.Runtime.Serialization;

namespace ServiceCenter.MES.Service.Contract.LSM
{

    public class MaterialReturnParameter : BaseMethodParameter
    {
        [DataMember]
        public string ReturnNo { get; set; }

        [DataMember]
        public string Editor { get; set; }

        [DataMember]
        public string Store { get; set; }

        [DataMember]
        public string ErpCode { get; set; }
        [DataMember]
        public EnumReturnState State { get; set; }
    }

    /// <summary>
    /// 定义退料数据服务契约。
    /// </summary>
    [ServiceContract]
    public interface IMaterialReturnContract
    {
        /// <summary>
        /// 添加退料数据。
        /// </summary>
        /// <param name="obj">退料数据。</param>
        /// <param name="lstDetail">退料明细数据。</param>
        /// <returns><see cref="MethodReturnResult"/></returns>
        [OperationContract]
        MethodReturnResult Add(MaterialReturn obj, IList<MaterialReturnDetail> lstDetail);
        /// <summary>
        /// 获取退料数据。
        /// </summary>
        /// <param name="key">退料数据标识符.</param>
        /// <returns>MethodReturnResult&lt;MaterialReturn&gt;，退料数据.</returns>
        [OperationContract]
        MethodReturnResult<MaterialReturn> Get(string key);
        /// <summary>
        /// 获取退料数据集合。
        /// </summary>
        /// <param name="cfg">查询退料。</param>
        /// <returns>MethodReturnResult&lt;IList&lt;MaterialReturn&gt;&gt;，退料数据集合。</returns>
        [OperationContract(Name = "GetList")]
        MethodReturnResult<IList<MaterialReturn>> Get(ref PagingConfig cfg);
        /// <summary>
        /// 获取退料明细数据。
        /// </summary>
        /// <param name="key">退料明细数据标识符.</param>
        /// <returns>MethodReturnResult&lt;MaterialReturnDetail&gt;，退料明细数据.</returns>
        [OperationContract]
        MethodReturnResult<MaterialReturnDetail> GetDetail(MaterialReturnDetailKey key);
        /// <summary>
        /// 获取退料明细数据集合。
        /// </summary>
        /// <param name="cfg">查询退料明细。</param>
        /// <returns>MethodReturnResult&lt;IList&lt;MaterialReturnDetail&gt;&gt;，退料明细数据集合。</returns>
        [OperationContract(Name = "GetDetailList")]
        MethodReturnResult<IList<MaterialReturnDetail>> GetDetail(ref PagingConfig cfg);
        [OperationContract]
        MethodReturnResult<DataSet> GetDetailByReturnNo(string key);

        [OperationContract]
        MethodReturnResult WO(MaterialReturnParameter p);

        [OperationContract]
        MethodReturnResult<DataSet> GetStore();

        [OperationContract]
        MethodReturnResult<DataSet> GetStoreName(string Store);
        [OperationContract]
        MethodReturnResult<DataSet> GetEffiByMaterialLot(string Code);

        [OperationContract]
        MethodReturnResult<DataSet> GetERPMaterialReceiptDetail(string LotNo);

        [OperationContract]
        MethodReturnResult<DataSet> GetERPWorkStock(string OrderNumber);

        //[OperationContract]
        //MethodReturnResult<DataSet> GetERPWorkStockInfo(string BLNumber);

        /// <summary>
        /// 查询ERP中的备料计划
        /// </summary>
        /// <param name="BLNumber">工单号</param>
        /// <param name="materiallot">物料编码</param>
        /// <returns></returns>
        [OperationContract]
        MethodReturnResult<DataSet> GetERPWorkStockInfo(string BLNumber, string materiallot);

        [OperationContract]
        MethodReturnResult<DataSet> GetWOReportFromDB(string ReturnNo);

        [OperationContract]
        MethodReturnResult<DataSet> GetERPReportCodeById(string strId);

        [OperationContract]
        MethodReturnResult<DataSet> GetReturnReportFromDB(string ReturnNo);

        [OperationContract]
        MethodReturnResult<DataSet> GetMaterialInfo(string MaterialCode);


    }
}
