using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.Common.Model;
using ServiceCenter.MES.Model.ERP;

namespace ServiceCenter.MES.Service.Contract.ERP
{
    /// <summary>
    /// 工单的参数类。
    /// </summary>
    [DataContract]
    public class ERPWorkOrderParameter : BaseMethodParameter
    {
        /// <summary>
        /// 工单号
        /// </summary>
        [DataMember]
        public string OrderNumber { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        //[DataMember]
        //public string Creator { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        //[DataMember]
        //public EnumWorkOrderPriority enumPriority { get; set; }
    }

    /// <summary>
    /// 领料的参数类
    /// </summary>
    [DataContract]
    public class ERPMaterialReceiptParameter : BaseMethodParameter
    {

        [DataMember]
        public string ReceiptNo { get; set; }

        //[DataMember]
        //public string Creator { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string LineStore { get; set; }
    }
    /// <summary>
    /// 数据回滚的参数类
    /// </summary>
    [DataContract]
    public class REbackdataParameter : BaseMethodParameter
    {

        [DataMember]
        public string PackageNo { get; set; }

        [DataMember]
        public string ErrorMsg { get; set; }
        [DataMember]
        public int ReType { get; set; }
        [DataMember]
        public int IsDelete { get; set; }

      
    }
    /// <summary>
    /// 定义工单数据服务契约。
    /// </summary>
    [ServiceContract]
    public interface IERPContract
    {     
        [OperationContract]
        MethodReturnResult<DataSet> GetERPSaleOut(string SalesNo);

        [OperationContract]
        MethodReturnResult<DataSet> GetERPWorkOrder(string OrderNumber);

        [OperationContract]
        MethodReturnResult<DataSet> GetERPSupplier(string SupplierCode);

        [OperationContract]
        MethodReturnResult<DataSet> GetByNameERPManufacturer(string ManufacturerName);

        [OperationContract]
        MethodReturnResult<DataSet> GetByCodeERPManufacturer(string ManufacturerCode);

        [OperationContract]
        MethodReturnResult<DataSet> GetERPWorkOrderBOM(string OrderNumber);

        [OperationContract]
        MethodReturnResult AddERPWorkOrder(ERPWorkOrderParameter p);

        [OperationContract]
        MethodReturnResult UpdateBaseInfo(ERPWorkOrderParameter p);

        [OperationContract]
        MethodReturnResult<DataSet> GetERPMaterialReceipt(string OrderNumber);

        [OperationContract]
        MethodReturnResult<List<MaterialReceiptReplace>> GetMaterialReceiptReplaceDetail(MethodReturnResult<DataSet> ds_detail, int PageNo, int PageSize);

        [OperationContract]
        MethodReturnResult<DataSet> GetERPMaterialReceiptDetail(string ReceiptNO);

        [OperationContract]
        MethodReturnResult AddERPMaterialReceipt(ERPMaterialReceiptParameter p);

        [OperationContract]
        MethodReturnResult<DataSet> GetERPWR(string WRCode);

        [OperationContract]
        MethodReturnResult<DataSet> GetERPWRDetail(string ObjectNumber, string WRCode,string OrderNumber);

        [OperationContract]
        MethodReturnResult<DataSet> GetERPWRDetailInfo(string WRCode);

        [OperationContract]
        MethodReturnResult<DataSet> GetERPOrderType(string ObjectType);

        [OperationContract]
        MethodReturnResult<DataSet> GetERPINCodeById(string INCode);

        [OperationContract]
        MethodReturnResult AddMaterialTypeFromERP(BaseMethodParameter p);

        [OperationContract]
        MethodReturnResult<DataSet> GetReceiptOrderNumberByPackageNo(string PackageNo);

        [OperationContract]
        MethodReturnResult GetREbackdata(REbackdataParameter p);

        /// <summary>
        /// 根据ERP出库单确认部门
        /// </summary>
        /// <param name="ERPPartCode">部门编码</param>
        /// <param name="ReceiptNO">出库单单据号</param>
        /// <returns></returns>
        [OperationContract]
        MethodReturnResult<DataSet> GetERPStore(string ERPPartCode, string ReceiptNO);
        /// <summary>
        /// 根据ERP出库单确认对应的MES仓库
        /// </summary>
        /// <param name="ReceiptNO">出库单单据号</param>
        /// <returns></returns>
        [OperationContract]
        MethodReturnResult<DataSet> GetERPMaterialReceiptStore(string ReceiptNO);

        /// <summary>
        /// 根据工单类型取得对应的ERP入库单类型
        /// </summary>
        /// <param name="orderType">MES工单类型</param>
        /// <returns></returns>
        [OperationContract]
        string GetERPStockInType(string orderType);

        /// <summary>
        /// 根据工单类型取得对应的ERP入库单类型主键
        /// </summary>
        /// <param name="orderType">MES工单类型</param>
        /// <returns></returns>
        [OperationContract]
        string GetERPStockInTypeKey(string orderType);


    }
}
