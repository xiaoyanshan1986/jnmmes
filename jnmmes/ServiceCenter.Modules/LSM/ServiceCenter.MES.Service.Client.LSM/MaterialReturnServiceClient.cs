// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Client.LSM
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 08-08-2014
// ***********************************************************************
// <copyright file="MaterialReturnServiceClient.cs" company="">
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
using ServiceCenter.MES.Service.Contract.LSM;
using ServiceCenter.MES.Model.LSM;
using ServiceCenter.Model;
using System.Data;

/// <summary>
/// The LSM namespace.
/// </summary>
namespace ServiceCenter.MES.Service.Client.LSM
{
    /// <summary>
    /// 定义退料单管理契约调用方式。
    /// </summary>
    public class MaterialReturnServiceClient : ClientBase<IMaterialReturnContract>, IMaterialReturnContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialReturnServiceClient" /> class.
        /// </summary>
        public MaterialReturnServiceClient() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialReturnServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public MaterialReturnServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialReturnServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public MaterialReturnServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialReturnServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public MaterialReturnServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialReturnServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public MaterialReturnServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }

        /// <summary>
        /// 添加退料单。
        /// </summary>
        /// <param name="obj">退料单数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(MaterialReturn obj, IList<MaterialReturnDetail> lstDetail)
        {
            return base.Channel.Add(obj, lstDetail);
        }

        /// <summary>
        /// add as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> AddAsync(MaterialReturn obj, IList<MaterialReturnDetail> lstDetail)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Add(obj, lstDetail);
            });
        }

        /// <summary>
        /// 获取退料单数据。
        /// </summary>
        /// <param name="key">退料单标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;MaterialReturn&gt;" />,退料单数据.</returns>
        public MethodReturnResult<MaterialReturn> Get(string key)
        {
            return base.Channel.Get(key);
        }

        /// <summary>
        /// get as an asynchronous operation.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Task&lt;MethodReturnResult&lt;MaterialReturn&gt;&gt;.</returns>
        public async Task<MethodReturnResult<MaterialReturn>> GetAsync(string key)
        {
            return await Task.Run<MethodReturnResult<MaterialReturn>>(() =>
            {
                return base.Channel.Get(key);
            });
        }

        /// <summary>
        /// 获取退料单数据集合。
        /// </summary>
        /// <param name="cfg">查询退料单.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;MaterialReturn&gt;&gt;，退料单数据集合.</returns>
        public MethodReturnResult<IList<MaterialReturn>> Get(ref ServiceCenter.Model.PagingConfig cfg)
        {
            return base.Channel.Get(ref cfg);
        }


        /// <summary>
        /// 获取退料单明细数据。
        /// </summary>
        /// <param name="key">退料单明细标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;MaterialReturnDetail&gt;" />,退料单明细数据.</returns>
        public MethodReturnResult<MaterialReturnDetail> GetDetail(MaterialReturnDetailKey key)
        {
            return base.Channel.GetDetail(key);
        }

        /// <summary>
        /// get as an asynchronous operation.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Task&lt;MethodReturnResult&lt;MaterialReturnDetail&gt;&gt;.</returns>
        public async Task<MethodReturnResult<MaterialReturnDetail>> GetDetailAsync(MaterialReturnDetailKey key)
        {
            return await Task.Run<MethodReturnResult<MaterialReturnDetail>>(() =>
            {
                return base.Channel.GetDetail(key);
            });
        }

        /// <summary>
        /// 获取退料单明细数据集合。
        /// </summary>
        /// <param name="cfg">查询退料单明细.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;MaterialReturnDetail&gt;&gt;，退料单明细数据集合.</returns>
        public MethodReturnResult<IList<MaterialReturnDetail>> GetDetail(ref ServiceCenter.Model.PagingConfig cfg)
        {
            return base.Channel.GetDetail(ref cfg);
        }

        public MethodReturnResult<DataSet> GetDetailByReturnNo(string key)
        {
            return base.Channel.GetDetailByReturnNo(key);
        }

        public MethodReturnResult WO(MaterialReturnParameter p)
        {
            return base.Channel.WO(p);
        }

        public MethodReturnResult<DataSet> GetStore()
        {
            return base.Channel.GetStore();
        }

        public MethodReturnResult<DataSet> GetStoreName(string Store)
        {
            return base.Channel.GetStoreName(Store);
        }

        public MethodReturnResult<DataSet> GetEffiByMaterialLot(string Code)
        {
            return base.Channel.GetEffiByMaterialLot(Code);
        }

        public MethodReturnResult<DataSet> GetERPMaterialReceiptDetail(string LotNo)
        {
            return base.Channel.GetERPMaterialReceiptDetail(LotNo);
        }
        public MethodReturnResult<DataSet> GetERPWorkStock(string OrderNumber)
        {
            return base.Channel.GetERPWorkStock(OrderNumber);
        }
        //public MethodReturnResult<DataSet> GetERPWorkStockInfo(string BLNumber)
        //{
        //    return base.Channel.GetERPWorkStockInfo(BLNumber);
        //}
        /// <summary>
        /// 查询ERP中的备料计划
        /// </summary>
        /// <param name="BLNumber">工单号</param>
        /// <param name="materiallot">物料编码</param>
        /// <returns>返回相应工单备料计划</returns>
        public MethodReturnResult<DataSet> GetERPWorkStockInfo(string BLNumber, string materiallot)
        {
            return base.Channel.GetERPWorkStockInfo(BLNumber, materiallot);
        }

        public MethodReturnResult<DataSet> GetWOReportFromDB(string ReturnNo)
        {
            return base.Channel.GetWOReportFromDB(ReturnNo);
        }

        public MethodReturnResult<DataSet> GetERPReportCodeById(string strId)
        {
            return base.Channel.GetERPReportCodeById(strId);
        }
        public MethodReturnResult<DataSet> GetReturnReportFromDB(string ReturnNo)
        {
            return base.Channel.GetReturnReportFromDB(ReturnNo);
        }

        public MethodReturnResult<DataSet> GetMaterialInfo(string MaterialCode)
        {
            return base.Channel.GetMaterialInfo(MaterialCode);
        }

    }
}
