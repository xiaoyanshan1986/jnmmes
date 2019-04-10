// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Client.ERP
// Author           : 
// Created          : 
//
// Last Modified By : 方军
// Last Modified On : 2016-12-13 17:59:43
// ***********************************************************************
// <copyright file="WorkOrderServiceClient.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.Service.Contract.ERP;
using ServiceCenter.Model;
using System.Data;
using ServiceCenter.Common.Model;
using ServiceCenter.MES.Model.ERP;

/// <summary>
/// The PPM namespace.
/// </summary>
namespace ServiceCenter.MES.Service.Client.ERP
{
    /// <summary>
    /// 定义工单管理契约调用方式。
    /// </summary>
    public class ERPClient : ClientBase<IERPContract>, IERPContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkOrderServiceClient" /> class.
        /// </summary>
        public ERPClient()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkOrderServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public ERPClient(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkOrderServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public ERPClient(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkOrderServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public ERPClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkOrderServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public ERPClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }

        /// <summary>
        /// 查询ERP销售单号
        /// </summary>
        /// <param name="SalesNo">销售单号</param>
        /// <returns></returns>  
        public MethodReturnResult<DataSet> GetERPSaleOut(string SalesNo)
        {
            return base.Channel.GetERPSaleOut(SalesNo);
        }

        /// <summary>
        /// 获取ERP工单
        /// </summary>
        /// <param name="OrderNumber">工单号</param>
        /// <returns></returns>
        public MethodReturnResult<DataSet> GetERPWorkOrder(string OrderNumber)
        {
            return base.Channel.GetERPWorkOrder(OrderNumber);
        }

        /// <summary>
        /// 获取ERP工单的BOM信息
        /// </summary>
        /// <param name="OrderNumber">工单号</param>
        /// <returns></returns>
        public MethodReturnResult<DataSet> GetERPWorkOrderBOM(string OrderNumber)
        {
            return base.Channel.GetERPWorkOrderBOM(OrderNumber);
        }

        /// <summary>
        /// 获取ERP供应商
        /// </summary>
        /// <param name="OrderNumber">工单号</param>
        /// <returns></returns>
        public MethodReturnResult<DataSet> GetERPSupplier(string SupplierCode)
        {
            return base.Channel.GetERPSupplier(SupplierCode);
        }

        // 获取ERP生产厂商
        public MethodReturnResult<DataSet> GetByNameERPManufacturer(string ManufacturerName)
        {
            return base.Channel.GetByNameERPManufacturer(ManufacturerName);
        }

        // 获取ERP生产厂商
        public MethodReturnResult<DataSet> GetByCodeERPManufacturer(string ManufacturerCode)
        {
            return base.Channel.GetByCodeERPManufacturer(ManufacturerCode);
        }

        public MethodReturnResult AddERPWorkOrder(ERPWorkOrderParameter p)
        {
            return base.Channel.AddERPWorkOrder(p);
        }

        public MethodReturnResult UpdateBaseInfo(ERPWorkOrderParameter p)
        {
            return base.Channel.UpdateBaseInfo(p);
        }

        public MethodReturnResult<DataSet> GetERPMaterialReceipt(string OrderNumber)
        {
            return base.Channel.GetERPMaterialReceipt(OrderNumber);
        }

        public MethodReturnResult<DataSet> GetERPMaterialReceiptDetail(string ReceiptNO)
        {
            return base.Channel.GetERPMaterialReceiptDetail(ReceiptNO);
        }

        public MethodReturnResult<List<MaterialReceiptReplace>> GetMaterialReceiptReplaceDetail(MethodReturnResult<DataSet> ds_detail, int PageNo, int PageSize)
        {
            return base.Channel.GetMaterialReceiptReplaceDetail(ds_detail, PageNo, PageSize);
        }

        public MethodReturnResult AddERPMaterialReceipt(ERPMaterialReceiptParameter p)
        {
            return base.Channel.AddERPMaterialReceipt(p);
        }

        public MethodReturnResult<DataSet> GetERPWR(string WRCode)
        {
            return base.Channel.GetERPWR(WRCode);
        }

        public MethodReturnResult<DataSet> GetERPWRDetail(string ObjectNumber, string WRCode,string OrderNumber)
        {
            return base.Channel.GetERPWRDetail(ObjectNumber, WRCode, OrderNumber);
        }

        public MethodReturnResult<DataSet> GetERPWRDetailInfo(string WRCode)
        {
            return base.Channel.GetERPWRDetailInfo(WRCode);
        }
        public MethodReturnResult<DataSet> GetERPOrderType(string ObjectType)
        {
            return base.Channel.GetERPOrderType(ObjectType);
        }
        public MethodReturnResult<DataSet> GetERPINCodeById(string INCode)
        {
            return base.Channel.GetERPINCodeById(INCode);
        }

        public MethodReturnResult AddMaterialTypeFromERP(BaseMethodParameter p)
        {
            return base.Channel.AddMaterialTypeFromERP(p);
        }

        public MethodReturnResult<DataSet> GetReceiptOrderNumberByPackageNo(string PackageNo)
        {
            return base.Channel.GetReceiptOrderNumberByPackageNo(PackageNo);
        }
        public MethodReturnResult GetREbackdata(REbackdataParameter p)
        {
            return base.Channel.GetREbackdata(p);
        }

        /// <summary>
        /// 根据ERP出库单确认部门
        /// </summary>
        /// <param name="ERPPartCode">部门编码</param>
        /// <param name="ReceiptNO">出库单单据号</param>
        /// <returns></returns>
        public MethodReturnResult<DataSet> GetERPStore(string ERPPartCode, string ReceiptNO)
        {
            return base.Channel.GetERPStore(ERPPartCode, ReceiptNO);
        }
        /// <summary>
        /// 根据ERP出库单确认对应的MES仓库
        /// </summary>
        /// <param name="ReceiptNO">出库单单据号</param>
        /// <returns></returns>
        public MethodReturnResult<DataSet> GetERPMaterialReceiptStore(string ReceiptNO)
        {
            return base.Channel.GetERPMaterialReceiptStore(ReceiptNO);
        }

        /// <summary>
        /// 根据工单类型取得对应的ERP入库单类型
        /// </summary>
        /// <param name="ReceiptNO">出库单单据号</param>
        /// <returns></returns>
        public string GetERPStockInType(string orderType)
        {
            return base.Channel.GetERPStockInType(orderType);
        }

        /// <summary>
        /// 根据工单类型取得对应的ERP入库单类型主键
        /// </summary>
        /// <param name="ReceiptNO">出库单单据号</param>
        /// <returns></returns>
        public string GetERPStockInTypeKey(string orderType)
        {
            return base.Channel.GetERPStockInTypeKey(orderType);
        }
    }
}
