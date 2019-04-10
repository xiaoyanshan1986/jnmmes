// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Client.LSM
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 08-08-2014
// ***********************************************************************
// <copyright file="MaterialReceiptServiceClient.cs" company="">
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
    /// 定义领料单管理契约调用方式。
    /// </summary>
    public class MaterialReceiptServiceClient : ClientBase<IMaterialReceiptContract>, IMaterialReceiptContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialReceiptServiceClient" /> class.
        /// </summary>
        public MaterialReceiptServiceClient() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialReceiptServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public MaterialReceiptServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialReceiptServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public MaterialReceiptServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialReceiptServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public MaterialReceiptServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialReceiptServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public MaterialReceiptServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }

        /// <summary>
        /// 添加领料单。
        /// </summary>
        /// <param name="obj">领料单数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(MaterialReceipt obj, IList<MaterialReceiptDetail> lstDetail)
        {
            return base.Channel.Add(obj, lstDetail);
        }

        /// <summary>
        /// add as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> AddAsync(MaterialReceipt obj, IList<MaterialReceiptDetail> lstDetail)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Add(obj, lstDetail);
            });
        }

        /// <summary>
        /// 获取领料单数据。
        /// </summary>
        /// <param name="key">领料单标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;MaterialReceipt&gt;" />,领料单数据.</returns>
        public MethodReturnResult<MaterialReceipt> Get(string key)
        {
            return base.Channel.Get(key);
        }

        /// <summary>
        /// get as an asynchronous operation.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Task&lt;MethodReturnResult&lt;MaterialReceipt&gt;&gt;.</returns>
        public async Task<MethodReturnResult<MaterialReceipt>> GetAsync(string key)
        {
            return await Task.Run<MethodReturnResult<MaterialReceipt>>(() =>
            {
                return base.Channel.Get(key);
            });
        }

        /// <summary>
        /// 获取领料单数据集合。
        /// </summary>
        /// <param name="cfg">查询领料单.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;MaterialReceipt&gt;&gt;，领料单数据集合.</returns>
        public MethodReturnResult<IList<MaterialReceipt>> Get(ref ServiceCenter.Model.PagingConfig cfg)
        {
            return base.Channel.Get(ref cfg);
        }


        /// <summary>
        /// 获取领料单明细数据。
        /// </summary>
        /// <param name="key">领料单明细标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;MaterialReceiptDetail&gt;" />,领料单明细数据.</returns>
        public MethodReturnResult<MaterialReceiptDetail> GetDetail(MaterialReceiptDetailKey key)
        {
            return base.Channel.GetDetail(key);
        }

        /// <summary>
        /// get as an asynchronous operation.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Task&lt;MethodReturnResult&lt;MaterialReceiptDetail&gt;&gt;.</returns>
        public async Task<MethodReturnResult<MaterialReceiptDetail>> GetDetailAsync(MaterialReceiptDetailKey key)
        {
            return await Task.Run<MethodReturnResult<MaterialReceiptDetail>>(() =>
            {
                return base.Channel.GetDetail(key);
            });
        }

        /// <summary>
        /// 获取领料单明细数据集合。
        /// </summary>
        /// <param name="cfg">查询领料单明细.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;MaterialReceiptDetail&gt;&gt;，领料单明细数据集合.</returns>
        public MethodReturnResult<IList<MaterialReceiptDetail>> GetDetail(ref ServiceCenter.Model.PagingConfig cfg)
        {
            return base.Channel.GetDetail(ref cfg);
        }

        public MethodReturnResult<MaterialReceipt> AddMaterialReceipt(MaterialReceipt obj)
        {
            return base.Channel.AddMaterialReceipt(obj);
        }
        public MethodReturnResult AddMaterialReceiptDetail(MaterialReceiptDetailParamter p)
        {
            return base.Channel.AddMaterialReceiptDetail(p);
        }

        public MethodReturnResult DeleteMaterialReceiptDetail(MaterialReceiptDetailParamter p)
        {
            return base.Channel.DeleteMaterialReceiptDetail(p);
        }

        public MethodReturnResult<MaterialReceipt> DeleteMaterialReceipt(string materialReceiptNo)
        {
            return base.Channel.DeleteMaterialReceipt(materialReceiptNo);
        }
        public MethodReturnResult<MaterialReceipt> ModifyMaterialReceipt(MaterialReceipt obj)
        {
            return base.Channel.ModifyMaterialReceipt(obj);
        }
        public MethodReturnResult ApproveMaterialReceipt(MaterialReceiptParamter p)
        {
            return base.Channel.ApproveMaterialReceipt(p);
        }

        public MethodReturnResult<MaterialReceiptDetail> GetBoxDetail(string boxLotNumber)
        {
            return base.Channel.GetBoxDetail(boxLotNumber);
        }

        public MethodReturnResult<DataSet> GetOrderNumberByMaterialLot(string MaterialLot)
        {

            return base.Channel.GetOrderNumberByMaterialLot(MaterialLot);
        }
    }
}
