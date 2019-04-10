using ServiceCenter.MES.Model.LSM;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using ServiceCenter.Common.Model;
using System.Data;

namespace ServiceCenter.MES.Service.Contract.LSM
{

    /// <summary>
    /// 表示操作时需要采集的附加的参数数据。
    /// </summary>
    [DataContract]
    public class MaterialReceiptDetailParamter:BaseMethodParameter
    {
        /// <summary>
        /// 领料单编号
        /// </summary>
        [DataMember]
        public string ReceiptNo { get; set; }
        
        /// <summary>
        /// 领料单对应的工单号
        /// </summary>
        [DataMember]
        public string OrderNumber { get; set; }

        /// <summary>
        /// 原材料批次号
        /// </summary>
        [DataMember]
        public string MaterialLotNumber { get; set; }

        /// <summary>
        /// 线边仓
        /// </summary>
        [DataMember]
        public string LineStore { get; set; }

        /// <summary>
        /// 原材料批次号
        /// </summary>
        [DataMember]
        public bool IsReceiptOfCell { get; set; }
    }


    [DataContract]
    public class MaterialReceiptParamter : BaseMethodParameter
    {
        /// <summary>
        /// 领料单编号
        /// </summary>
        [DataMember]
        public string ReceiptNo { get; set; }
    }
    /// <summary>
    /// 定义领料数据服务契约。
    /// </summary>
    [ServiceContract]
    public interface IMaterialReceiptContract
    {
        /// <summary>
        /// 添加领料数据。
        /// </summary>
        /// <param name="obj">领料数据。</param>
        /// <param name="lstDetail">领料明细数据。</param>
        /// <returns><see cref="MethodReturnResult"/></returns>
        [OperationContract]
        MethodReturnResult Add(MaterialReceipt obj, IList<MaterialReceiptDetail> lstDetail);
        /// <summary>
        /// 获取领料数据。
        /// </summary>
        /// <param name="key">领料数据标识符.</param>
        /// <returns>MethodReturnResult&lt;MaterialReceipt&gt;，领料数据.</returns>
        [OperationContract]
        MethodReturnResult<MaterialReceipt> Get(string key);
        /// <summary>
        /// 获取领料数据集合。
        /// </summary>
        /// <param name="cfg">查询领料。</param>
        /// <returns>MethodReturnResult&lt;IList&lt;MaterialReceipt&gt;&gt;，领料数据集合。</returns>
        [OperationContract(Name = "GetList")]
        MethodReturnResult<IList<MaterialReceipt>> Get(ref PagingConfig cfg);
        /// <summary>
        /// 获取领料明细数据。
        /// </summary>
        /// <param name="key">领料明细数据标识符.</param>
        /// <returns>MethodReturnResult&lt;MaterialReceiptDetail&gt;，领料明细数据.</returns>
        [OperationContract]
        MethodReturnResult<MaterialReceiptDetail> GetDetail(MaterialReceiptDetailKey key);
        /// <summary>
        /// 获取领料明细数据集合。
        /// </summary>
        /// <param name="cfg">查询领料明细。</param>
        /// <returns>MethodReturnResult&lt;IList&lt;MaterialReceiptDetail&gt;&gt;，领料明细数据集合。</returns>
        [OperationContract(Name = "GetDetailList")]
        MethodReturnResult<IList<MaterialReceiptDetail>> GetDetail(ref PagingConfig cfg);

        /// <summary>
        /// 添加领料单表头
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [OperationContract]
        MethodReturnResult<MaterialReceipt> AddMaterialReceipt(MaterialReceipt obj);

        /// <summary>
        /// 添加领料单明细
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [OperationContract]
        MethodReturnResult AddMaterialReceiptDetail(MaterialReceiptDetailParamter p);

        /// <summary>
        /// 删除领料单明细
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [OperationContract]
        MethodReturnResult DeleteMaterialReceiptDetail(MaterialReceiptDetailParamter p);
        /// <summary>
        /// 删除领料单
        /// </summary>
        /// <param name="materialReceiptNo"></param>
        /// <returns></returns>
        [OperationContract]
        MethodReturnResult<MaterialReceipt> DeleteMaterialReceipt(string materialReceiptNo);

        [OperationContract]
        MethodReturnResult<MaterialReceipt> ModifyMaterialReceipt(MaterialReceipt obj);
        [OperationContract]
        MethodReturnResult ApproveMaterialReceipt(MaterialReceiptParamter p);
        [OperationContract]
        MethodReturnResult<MaterialReceiptDetail> GetBoxDetail(string boxLotNumber);
        [OperationContract]
        MethodReturnResult<DataSet> GetOrderNumberByMaterialLot(string MaterialLot);
    }
}
