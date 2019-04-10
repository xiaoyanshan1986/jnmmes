using ServiceCenter.MES.Model.ERP;
using ServiceCenter.MES.Service.Contract.ERP;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.Client.ERP
{
    public class WOReportClient : ClientBase<IWOReportContract>, IWOReportContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkOrderServiceClient" /> class.
        /// </summary>
        public WOReportClient()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkOrderServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public WOReportClient(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkOrderServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public WOReportClient(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkOrderServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public WOReportClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkOrderServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public WOReportClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }


        public MethodReturnResult<WOReport> GetWOReport(string Key)
        {
            return base.Channel.GetWOReport(Key);
        }

        public MethodReturnResult<IList<WOReport>> GetWOReport(ref PagingConfig cfg)
        {
            return base.Channel.GetWOReport(ref cfg);
        }

        public MethodReturnResult AddWOReport(WOReport model)
        {
            return base.Channel.AddWOReport(model);
        }

        public MethodReturnResult EditWOReport(WOReport model)
        {
            return base.Channel.EditWOReport(model);
        }

        public MethodReturnResult DeleteWOReport(WOReport model, string key)
        {
            return base.Channel.DeleteWOReport(model,key);
        }

        public MethodReturnResult<WOReportDetail> GetWOReportDetail(WOReportDetailKey Key)
        {
            return base.Channel.GetWOReportDetail(Key);
        }

        public MethodReturnResult<IList<WOReportDetail>> GetWOReportDetail(ref PagingConfig cfg)
        {
            return base.Channel.GetWOReportDetail(ref cfg);
        }

        public MethodReturnResult AddWOReportDetail(WOReportDetail model, EnumScrapType ScrapType)
        {
            return base.Channel.AddWOReportDetail(model, ScrapType);
        }

        public MethodReturnResult DeleteWOReportDetail(WOReportDetail model, WOReportDetailKey key)
        {
            return base.Channel.DeleteWOReportDetail(model,key);
        }

        public MethodReturnResult GetERPWorkReprotBillCode(string stockInBillCode)
        {
            return base.Channel.GetERPWorkReprotBillCode(stockInBillCode);
        }

        public MethodReturnResult<DataSet> GetPackageInfo(string ObjectNumber)
        {
            return base.Channel.GetPackageInfo(ObjectNumber);
        }
        public MethodReturnResult<DataSet> GetPackageInfoEx(string ObjectNumber, EnumScrapType ScrapType)
        {
            return base.Channel.GetPackageInfoEx(ObjectNumber, ScrapType);
        }
        public MethodReturnResult WO(WOReportParameter p, string ScrapType)
        {
            return base.Channel.WO(p, ScrapType);
        }

        public MethodReturnResult<DataSet> GetCodeByName(string Name,string ListCode)
        {
            return base.Channel.GetCodeByName(Name, ListCode);
        }

        public MethodReturnResult<DataSet> GetERPCodeByBillCode(string BillCode)
        {
            return base.Channel.GetERPCodeByBillCode(BillCode);
        }

        /// <summary>
        /// 根据ERP入库单主键取得ERP入库单号
        /// </summary>
        /// <param name="ERPStockInKey"></param>
        /// <returns></returns>
        public MethodReturnResult GetERPStockInBillCodeByKey(string ERPStockInKey)
        {
            return base.Channel.GetERPStockInBillCodeByKey(ERPStockInKey);
        }
        public MethodReturnResult<DataSet> GetStore()
        {
            return base.Channel.GetStore();
        }
        public MethodReturnResult<DataSet> sGetStore()
        {
            return base.Channel.sGetStore();
        }

        public MethodReturnResult<DataSet> wGetStore()
        {
            return base.Channel.wGetStore();
        }

        public MethodReturnResult<DataSet> GetUnitByMaterialCode(string MaterialCode)
        {
            return base.Channel.GetUnitByMaterialCode(MaterialCode);
        }


        public MethodReturnResult WI(WOReportParameter p)
        {
            return base.Channel.WI(p);
        }

        public MethodReturnResult<DataSet> GetWOReportFromDB(string BillCode)
        {
            return base.Channel.GetWOReportFromDB(BillCode);
        }
        public MethodReturnResult<DataSet> GetERPReportCodeById(string strId)
        {
            return base.Channel.GetERPReportCodeById(strId);
        }

        public MethodReturnResult<DataSet> GetReportDetailByObjectNumber(string BillCode,string Scrap_Type)
        {
            return base.Channel.GetReportDetailByObjectNumber(BillCode, Scrap_Type);
        }
        
        public MethodReturnResult<bool> CheckReportDetail(string BillCode)
        {
            return base.Channel.CheckReportDetail(BillCode);
        }

        /// <summary>
        /// 入库申请单申报或申报撤销
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public MethodReturnResult StockInApply(WOReportParameter p)
        {
            return base.Channel.StockInApply(p);
        }

        /// <summary>
        /// 入库申请单接收或撤销
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public MethodReturnResult StockIn(WOReportParameter p)
        {
            return base.Channel.StockIn(p);
        }

        /// <summary>
        /// 入库接收核对
        /// </summary>
        /// <param name="packageNo"></param>
        /// <param name="billCode"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public MethodReturnResult CheckPackageInWIReportDetail(string packageNo, string billCode, string userName)
        {
            return base.Channel.CheckPackageInWIReportDetail(packageNo,billCode,userName);
        }

        /// <summary>
        /// 入库接收取消核对
        /// </summary>
        /// <param name="packageNo"></param>
        /// <param name="billCode"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public MethodReturnResult UnCheckPackageInWIReportDetail(string packageNo, string billCode, string userName)
        {
            return base.Channel.UnCheckPackageInWIReportDetail(packageNo, billCode, userName);
        }
    }
}
