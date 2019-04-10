using ServiceCenter.Common.Model;
using ServiceCenter.MES.Model.ERP;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.Contract.ERP
{
    [DataContract]
    public class WOReportParameter : BaseMethodParameter
    {
        [DataMember]
        public string BillCode { get; set; }

        [DataMember]
        public string Editor { get; set; }

        [DataMember]
        public string Store { get; set; }

        /// <summary>
        /// ERP报工单号
        /// </summary>
        [DataMember]
        public string ERPWorkReportCode { get; set; }

        /// <summary>
        /// ERP报工单主键
        /// </summary>
        [DataMember]
        public string ERPWorkReportKey { get; set; }

        /// <summary>
        /// 单据状态
        /// </summary>
        [DataMember]
        public EnumBillState BillState { get; set; }

        /// <summary>
        /// 操作类型（0 - 新增 -1 - 撤销）
        /// </summary>
        [DataMember]
        public int OperationType { get; set; }

        /// <summary>
        /// ERP入库单号
        /// </summary>
        [DataMember]
        public IDictionary<string, string> ERPStockInCodes { get; set; }
        
        /// <summary>
        /// 记录工单对应的入库单主键
        /// </summary>
        [DataMember]
        public IDictionary<string, string> ERPStockInKeys { get; set; }
    }


    [ServiceContract]
    public interface IWOReportContract
    {
        [OperationContract]
        MethodReturnResult<WOReport> GetWOReport(string Key);

        [OperationContract(Name = "GetList")]
        MethodReturnResult<IList<WOReport>> GetWOReport(ref PagingConfig cfg);

        [OperationContract]
        MethodReturnResult AddWOReport(WOReport model);

        [OperationContract]
        MethodReturnResult EditWOReport(WOReport model);

        [OperationContract]
        MethodReturnResult DeleteWOReport(WOReport model, string key);

        [OperationContract]
        MethodReturnResult<WOReportDetail> GetWOReportDetail(WOReportDetailKey Key);

        [OperationContract(Name = "GetDetailList")]
        MethodReturnResult<IList<WOReportDetail>> GetWOReportDetail(ref PagingConfig cfg);

        [OperationContract]
        MethodReturnResult AddWOReportDetail(WOReportDetail model, EnumScrapType ScrapType);

        [OperationContract]
        MethodReturnResult DeleteWOReportDetail(WOReportDetail model, WOReportDetailKey key);

        [OperationContract]
        MethodReturnResult GetERPWorkReprotBillCode(string stockInBillCode);

        [OperationContract]
        MethodReturnResult<DataSet> GetPackageInfo(string ObjectNumber);

        [OperationContract]
        MethodReturnResult<DataSet> GetPackageInfoEx(string ObjectNumber, EnumScrapType ScrapType);

        [OperationContract]
        MethodReturnResult WO(WOReportParameter p, string ScrapType);

        [OperationContract]
        MethodReturnResult<DataSet> GetCodeByName(string Name,string ListCode);

        [OperationContract]
        MethodReturnResult<DataSet> GetERPCodeByBillCode(string BillCode);

        [OperationContract]
        MethodReturnResult GetERPStockInBillCodeByKey(string ERPStockInKey);

        [OperationContract]
        MethodReturnResult<DataSet> GetStore();

        [OperationContract]
        MethodReturnResult<DataSet> sGetStore();

        [OperationContract]
        MethodReturnResult<DataSet> wGetStore();

        [OperationContract]
        MethodReturnResult<DataSet> GetUnitByMaterialCode(string MaterialCode);

        [OperationContract]
        MethodReturnResult WI(WOReportParameter p);

        [OperationContract]
        MethodReturnResult<DataSet> GetWOReportFromDB(string BillCode);

        /// <summary>
        /// 获取ERP报工单号，入库申请单号
        /// </summary>
        /// <param name="strId"></param>
        /// <returns></returns>
        [OperationContract]
        MethodReturnResult<DataSet> GetERPReportCodeById(string strId);

        [OperationContract]
        MethodReturnResult<DataSet> GetReportDetailByObjectNumber(string BillCode,string Scrap_Type);
        
        [OperationContract]
        MethodReturnResult<bool> CheckReportDetail(string BillCode);

        [OperationContract]
        MethodReturnResult StockInApply(WOReportParameter p);

        [OperationContract]
        MethodReturnResult StockIn(WOReportParameter p);

        [OperationContract]
        MethodReturnResult CheckPackageInWIReportDetail(string packageNo, string billCode, string userName);

        [OperationContract]
        MethodReturnResult UnCheckPackageInWIReportDetail(string packageNo, string billCode, string userName);
    }
}
