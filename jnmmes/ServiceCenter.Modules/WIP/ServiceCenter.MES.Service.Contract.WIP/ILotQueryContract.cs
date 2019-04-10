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
    public class RPTLotMateriallistParameter : BaseMethodParameter
    {

        [DataMember]
        public string LotNumber { get; set; }

        [DataMember]
        public string ErrorMsg { get; set; }

        [DataMember]
        public int Activity { get; set; }
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


    }

    public class RPTLotQueryDetailParameter : BaseMethodParameter
    {

        [DataMember]
        public string LotNumber { get; set; }

        [DataMember]
        public string LotNumber1 { get; set; }

        [DataMember]
        public string LocationName { get; set; }

        [DataMember]
        public string OrderNumber { get; set; }

        [DataMember]
        public string MaterialCode { get; set; }

        [DataMember]
        public string LineCode { get; set; }

        [DataMember]
        public string RouteStepName { get; set; }

        [DataMember]
        public string StateFlag { get; set; }

        [DataMember]
        public string PackageNo { get; set; }

        [DataMember]
        public string MapType { get; set; }

        [DataMember]
        public string HoldFlag { get; set; }

        [DataMember]
        public string DeletedFlag { get; set; }

        [DataMember]
        public string StartCreateTime { get; set; }
        //public DateTime? StartCreateTime { get; set; }

        [DataMember]
        public string EndCreateTime { get; set; }

        [DataMember]
        public string AttributeType { get; set; }

        [DataMember]
        public string ErrorMsg { get; set; }

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


    }

    /// <summary>
    /// 批次查询契约接口。
    /// </summary>
    [ServiceContract]
    public interface ILotQueryContract
    {
        /// <summary>
        /// 获取批次数据。。
        /// </summary>
        /// <param name="key">批次标识符.</param>
        /// <returns>MethodReturnResult&lt;Lot&gt;，批次数据.</returns>
        [OperationContract]
        MethodReturnResult<Lot> Get(string key);
        /// <summary>
        /// 获取批次数据集合。
        /// </summary>
        /// <param name="cfg">查询条件。</param>
        /// <returns>MethodReturnResult&lt;IList&lt;Lot&gt;&gt;，批次数据。</returns>
        [OperationContract(Name = "GetList")]
        MethodReturnResult<IList<Lot>> Get(ref PagingConfig cfg);

        [OperationContract]
        MethodReturnResult<DataSet> GetEx(RPTLotQueryDetailParameter p);

        /// <summary>
        /// 获取批次自定义特性数据。
        /// </summary>
        /// <param name="key">批次自定义特性数据标识符.</param>
        /// <returns>MethodReturnResult&lt;LotAttribute&gt;，批次自定义特性数据.</returns>
        [OperationContract]
        MethodReturnResult<LotAttribute> GetAttribute(LotAttributeKey key);
        /// <summary>
        /// 获取批次自定义特性数据集合。
        /// </summary>
        /// <param name="cfg">查询条件。</param>
        /// <returns>MethodReturnResult&lt;IList&lt;LotAttribute&gt;&gt;，批次自定义特性数据集合。</returns>
        [OperationContract(Name = "GetAttributeList")]
        MethodReturnResult<IList<LotAttribute>> GetAttribute(ref PagingConfig cfg);

        [OperationContract]
        MethodReturnResult<DataSet> GetAttributeEx(RPTLotQueryDetailParameter p);

        [OperationContract]
        MethodReturnResult<DataSet> GetLotList(RPTLotQueryDetailParameter p);
        /// <summary>
        /// 获取批次操作数据。
        /// </summary>
        /// <param name="key">批次操作标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;LotTransaction&gt;" />,批次操作数据.</returns>
        [OperationContract]
        MethodReturnResult<LotTransaction> GetTransaction(string key);
         /// <summary>
        /// 获取批次操作数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;LotTransaction&gt;" />,批次操作数据集合。</returns>
        [OperationContract(Name = "GetTransactionList")]
        MethodReturnResult<IList<LotTransaction>> GetTransaction(ref PagingConfig cfg);

        /// <summary>
        /// 获取批次历史数据。
        /// </summary>
        /// <param name="key">批次操作标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;LotTransactionHistory&gt;" />,批次操作数据.</returns>
        [OperationContract]
        MethodReturnResult<LotTransactionHistory> GetLotTransactionHistory(string key);
        /// <summary>
        /// 获取批次历史数据。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;LotTransactionHistory&gt;" />,批次操作数据集合。</returns>
        [OperationContract(Name = "GetLotTransactionHistorynList")]
        MethodReturnResult<IList<LotTransactionHistory>> GetLotTransactionHistory(ref PagingConfig cfg);

        /// <summary>
        /// 获取批次参数数据。
        /// </summary>
        /// <param name="key">批次参数标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;LotTransactionParameter&gt;" />,批次参数数据.</returns>
        [OperationContract]
        MethodReturnResult<LotTransactionParameter> GetTransactionParameter(LotTransactionParameterKey key);
         /// <summary>
        /// 获取批次参数数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;LotTransactionParameter&gt;" />,批次参数数据集合。</returns>
        [OperationContract(Name = "GetTransactionParameterList")]
        MethodReturnResult<IList<LotTransactionParameter>> GetTransactionParameter(ref PagingConfig cfg);
        /// <summary>
        /// 获取批次不良数据。
        /// </summary>
        /// <param name="key">批次不良标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;LotTransactionDefect&gt;" />,批次不良数据.</returns>
        [OperationContract]
        MethodReturnResult<LotTransactionDefect> GetLotTransactionDefect(LotTransactionDefectKey key);
        /// <summary>
        /// 获取批次不良数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;LotTransactionDefect&gt;" />,批次不良数据集合。</returns>
        [OperationContract(Name = "GetLotTransactionDefectList")]
        MethodReturnResult<IList<LotTransactionDefect>> GetLotTransactionDefect(ref PagingConfig cfg);
        /// <summary>
        /// 获取批次报废数据。
        /// </summary>
        /// <param name="key">批次报废标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;LotTransactionScrap&gt;" />,批次报废数据.</returns>
        [OperationContract]
        MethodReturnResult<LotTransactionScrap> GetLotTransactionScrap(LotTransactionScrapKey key);
        /// <summary>
        /// 获取批次报废数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;LotTransactionScrap&gt;" />,批次报废数据集合。</returns>
        [OperationContract(Name = "GetLotTransactionScrapList")]
        MethodReturnResult<IList<LotTransactionScrap>> GetLotTransactionScrap(ref PagingConfig cfg);

        /// <summary>
        /// 获取批次补料数据。
        /// </summary>
        /// <param name="key">批次补料标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;LotTransactionPatch&gt;" />,批次补料数据.</returns>
        [OperationContract]
        MethodReturnResult<LotTransactionPatch> GetLotTransactionPatch(LotTransactionPatchKey key);
        /// <summary>
        /// 获取批次补料数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;LotTransactionPatch&gt;" />,批次补料数据集合。</returns>
        [OperationContract(Name = "GetLotTransactionPatchList")]
        MethodReturnResult<IList<LotTransactionPatch>> GetLotTransactionPatch(ref PagingConfig cfg);

        /// <summary>
        /// 获取批次用料数据。
        /// </summary>
        /// <param name="key">批次用料标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;LotBOM&gt;" />,批次用料数据.</returns>
        [OperationContract]
        MethodReturnResult<LotBOM> GetLotBOM(LotBOMKey key);
        /// <summary>
        /// 获取批次用料数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;LotBOM&gt;" />,批次用料数据集合。</returns>
        [OperationContract(Name = "GetLotBOMList")]
        MethodReturnResult<IList<LotBOM>> GetLotBOM(ref PagingConfig cfg);

        /// <summary>
        /// 获取批次设备加工数据。
        /// </summary>
        /// <param name="key">批次设备加工数据标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;LotTransactionEquipment&gt;" />,批次设备加工数据.</returns>
        [OperationContract]
        MethodReturnResult<LotTransactionEquipment> GetLotTransactionEquipment(string key);
        /// <summary>
        /// 获取批次设备加工数据集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;LotTransactionEquipment&gt;" />,批次设备加工数据集合。</returns>
        [OperationContract(Name = "GetLotTransactionEquipmentList")]
        MethodReturnResult<IList<LotTransactionEquipment>> GetLotTransactionEquipment(ref PagingConfig cfg);

        /// <summary>
        /// 获取批次定时作业数据。
        /// </summary>
        /// <param name="key">批次定时作业数据标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;LotJob&gt;" />,批次定时作业数据.</returns>
        [OperationContract]
        MethodReturnResult<LotJob> GetLotJob(string key);
        /// <summary>
        /// 获取批次定时作业数据。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;LotJob&gt;" />,批次定时作业数据集合。</returns>
        [OperationContract(Name = "GetLotJobList")]
        MethodReturnResult<IList<LotJob>> GetLotJob(ref PagingConfig cfg);
        /// <summary>
        /// 获取批次检验数据。
        /// </summary>
        /// <param name="key">批次检验数据标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;LotTransactionCheck&gt;" />,批次检验数据.</returns>
        [OperationContract]
        MethodReturnResult<LotTransactionCheck> GetLotTransactionCheck(string key);
        /// <summary>
        /// 获取批次检验数据。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns><see cref="MethodReturnResult&lt;LotTransactionCheck&gt;" />,批次检验数据集合。</returns>
        [OperationContract(Name = "GetLotTransactionCheckList")]
        MethodReturnResult<IList<LotTransactionCheck>> GetLotTransactionCheck(ref PagingConfig cfg);
        /// <summary>
        /// 获取批次总数。
        /// </summary>
        /// <param name="key">批次检验数据标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;LotTransactionCheck&gt;" />,批次检验数据.</returns>
        [OperationContract]
        MethodReturnResult<DataSet> GetLotCount(string where);

        [OperationContract]
        MethodReturnResult<DataSet> GetLotColor(string lot);

        [OperationContract]
        MethodReturnResult<DataSet> GetRPTLotMaterialList(ref RPTLotMateriallistParameter p);

        [OperationContract]
        MethodReturnResult<DataSet> GetRPTLotProcessingHistory( ref RPTLotMateriallistParameter p);

        [OperationContract]
        MethodReturnResult<DataSet> GetMapDataQueryDb(ref RPTLotQueryDetailParameter p);
    }
}
