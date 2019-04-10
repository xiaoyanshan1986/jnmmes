// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Client.FMM
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 08-08-2014
// ***********************************************************************
// <copyright file="EquipmentLayoutDetailServiceClient.cs" company="">
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
using ServiceCenter.MES.Service.Contract.FMM;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.Model;
using System.Data;

/// <summary>
/// The FMM namespace.
/// </summary>
namespace ServiceCenter.MES.Service.Client.FMM
{
    /// <summary>
    /// 定义设备布局明细管理契约调用方式。
    /// </summary>
    public class EquipmentLayoutDetailServiceClient : ClientBase<IEquipmentLayoutDetailContract>, IEquipmentLayoutDetailContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EquipmentLayoutDetailServiceClient" /> class.
        /// </summary>
        public EquipmentLayoutDetailServiceClient() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EquipmentLayoutDetailServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public EquipmentLayoutDetailServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EquipmentLayoutDetailServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public EquipmentLayoutDetailServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EquipmentLayoutDetailServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public EquipmentLayoutDetailServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EquipmentLayoutDetailServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public EquipmentLayoutDetailServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }

        /// <summary>
        /// 添加设备布局明细。
        /// </summary>
        /// <param name="obj">设备布局明细数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(EquipmentLayoutDetail obj)
        {
            return base.Channel.Add(obj);
        }

        /// <summary>
        /// add as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> AddAsync(EquipmentLayoutDetail obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Add(obj);
            });
        }
        /// <summary>
        /// 修改设备布局明细。
        /// </summary>
        /// <param name="obj">设备布局明细数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public ServiceCenter.Model.MethodReturnResult Modify(EquipmentLayoutDetail obj)
        {
            return base.Channel.Modify(obj);
        }
        /// <summary>
        /// modify as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> ModifyAsync(EquipmentLayoutDetail obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Modify(obj);
            });
        }
        /// <summary>
        /// 删除设备布局明细。
        /// </summary>
        /// <param name="key">设备布局明细标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(EquipmentLayoutDetailKey key)
        {
            return base.Channel.Delete(key);
        }

        /// <summary>
        /// delete as an asynchronous operation.
        /// </summary>
        /// <param name="key">设备布局明细标识符.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> DeleteAsync(EquipmentLayoutDetailKey key)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Delete(key);
            });
        }

        /// <summary>
        /// 获取设备布局明细数据。
        /// </summary>
        /// <param name="key">设备布局明细标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;EquipmentLayoutDetail&gt;" />,设备布局明细数据.</returns>
        public MethodReturnResult<EquipmentLayoutDetail> Get(EquipmentLayoutDetailKey key)
        {
            return base.Channel.Get(key);
        }

        /// <summary>
        /// get as an asynchronous operation.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Task&lt;MethodReturnResult&lt;EquipmentLayoutDetail&gt;&gt;.</returns>
        public async Task<MethodReturnResult<EquipmentLayoutDetail>> GetAsync(EquipmentLayoutDetailKey key)
        {
            return await Task.Run<MethodReturnResult<EquipmentLayoutDetail>>(() =>
            {
                return base.Channel.Get(key);
            });
        }

        /// <summary>
        /// 获取设备布局明细数据集合。
        /// </summary>
        /// <param name="cfg">查询设备布局明细.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;EquipmentLayoutDetail&gt;&gt;，设备布局明细数据集合.</returns>
        public MethodReturnResult<IList<EquipmentLayoutDetail>> Get(ref ServiceCenter.Model.PagingConfig cfg)
        {
            return base.Channel.Get(ref cfg);
        }

        public MethodReturnResult<DataTable> GetEQPInfo(string LayoutName)
        {
            return base.Channel.GetEQPInfo(LayoutName);
        }
        public MethodReturnResult<DataTable> GetParameByEqpCode(string EqpCode)
        {
            return base.Channel.GetParameByEqpCode(EqpCode);
        }

        
        public MethodReturnResult<DataTable> GetEquipmentInfo(string EqpCode)
        {
            return base.Channel.GetEquipmentInfo(EqpCode);
        }
    }
}
