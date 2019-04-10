// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Client.EMS
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 08-08-2014
// ***********************************************************************
// <copyright file="EquipmentStateEventServiceClient.cs" company="">
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
using ServiceCenter.MES.Service.Contract.EMS;
using ServiceCenter.MES.Model.EMS;
using ServiceCenter.Model;

/// <summary>
/// The EMS namespace.
/// </summary>
namespace ServiceCenter.MES.Service.Client.EMS
{
    /// <summary>
    /// 定义设备状态事件管理契约调用方式。
    /// </summary>
    public class EquipmentStateEventServiceClient : ClientBase<IEquipmentStateEventContract>, IEquipmentStateEventContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EquipmentStateEventServiceClient" /> class.
        /// </summary>
        public EquipmentStateEventServiceClient() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EquipmentStateEventServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public EquipmentStateEventServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EquipmentStateEventServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public EquipmentStateEventServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EquipmentStateEventServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public EquipmentStateEventServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EquipmentStateEventServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public EquipmentStateEventServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }

        /// <summary>
        /// 添加设备状态事件。
        /// </summary>
        /// <param name="obj">设备状态事件数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(EquipmentStateEvent obj)
        {
            return base.Channel.Add(obj);
        }

        /// <summary>
        /// add as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> AddAsync(EquipmentStateEvent obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Add(obj);
            });
        }
        /// <summary>
        /// 修改设备状态事件。
        /// </summary>
        /// <param name="obj">设备状态事件数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public ServiceCenter.Model.MethodReturnResult Modify(EquipmentStateEvent obj)
        {
            return base.Channel.Modify(obj);
        }
        /// <summary>
        /// modify as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> ModifyAsync(EquipmentStateEvent obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Modify(obj);
            });
        }
        /// <summary>
        /// 删除设备状态事件。
        /// </summary>
        /// <param name="key">设备状态事件标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(string key)
        {
            return base.Channel.Delete(key);
        }

        /// <summary>
        /// delete as an asynchronous operation.
        /// </summary>
        /// <param name="key">设备状态事件标识符.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> DeleteAsync(string key)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Delete(key);
            });
        }

        /// <summary>
        /// 获取设备状态事件数据。
        /// </summary>
        /// <param name="key">设备状态事件标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;EquipmentStateEvent&gt;" />,设备状态事件数据.</returns>
        public MethodReturnResult<EquipmentStateEvent> Get(string key)
        {
            return base.Channel.Get(key);
        }

        /// <summary>
        /// get as an asynchronous operation.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Task&lt;MethodReturnResult&lt;EquipmentStateEvent&gt;&gt;.</returns>
        public async Task<MethodReturnResult<EquipmentStateEvent>> GetAsync(string key)
        {
            return await Task.Run<MethodReturnResult<EquipmentStateEvent>>(() =>
            {
                return base.Channel.Get(key);
            });
        }

        /// <summary>
        /// 获取设备状态事件数据集合。
        /// </summary>
        /// <param name="cfg">查询设备状态事件.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;EquipmentStateEvent&gt;&gt;，设备状态事件数据集合.</returns>
        public MethodReturnResult<IList<EquipmentStateEvent>> Get(ref ServiceCenter.Model.PagingConfig cfg)
        {
            return base.Channel.Get(ref cfg);
        }
    }
}
