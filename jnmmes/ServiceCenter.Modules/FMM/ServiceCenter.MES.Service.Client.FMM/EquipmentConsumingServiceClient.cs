// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Client.PPM
// Author           : 武子靖
// Created          : 10-08-2016
//
// Last Modified By : 
// Last Modified On : 
// ***********************************************************************
// <copyright file="EquipmentConsumingServiceClient.cs" company="">
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

/// <summary>
/// The PPM namespace.
/// </summary>
namespace ServiceCenter.MES.Service.Client.FMM
{
    /// <summary>
    /// 设备异常类型耗时管理契约调用方式。
    /// </summary>
    public class EquipmentConsumingServiceClient : ClientBase<IEquipmentConsumingContract>, IEquipmentConsumingContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlanAttendanceServiceClient" /> class.
        /// </summary>
        public EquipmentConsumingServiceClient()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlanAttendanceServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public EquipmentConsumingServiceClient(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlanAttendanceServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public EquipmentConsumingServiceClient(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlanAttendanceServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public EquipmentConsumingServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlanAttendanceServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public EquipmentConsumingServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }
        
        /// <summary>
        /// 添加设备异常类型耗时。
        /// </summary>
        /// <param name="obj">设备异常类型耗时管理数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(EquipmentConsuming obj)
        {
            return base.Channel.Add(obj);
        }

        /// <summary>
        /// add as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> AddAsync(EquipmentConsuming obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Add(obj);
            });
        }

        /// <summary>
        /// 修改设备异常类型耗时管理。
        /// </summary>
        /// <param name="obj">设备异常类型耗时管理数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public ServiceCenter.Model.MethodReturnResult Modify(EquipmentConsuming obj)
        {
            return base.Channel.Modify(obj);
        }

        public ServiceCenter.Model.MethodReturnResult Modify(IList<EquipmentConsuming> lst)
        {
            return base.Channel.Modify(lst);
        }
        /// <summary>
        /// modify as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> ModifyAsync(EquipmentConsuming obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Modify(obj);
            });
        }

        public async Task<MethodReturnResult> ModifyAsync(IList<EquipmentConsuming> lst)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Modify(lst);
            });
        }

        /// <summary>
        /// 删除设备异常类型耗时管理。
        /// </summary>
        /// <param name="key">设备异常类型耗时管理标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(EquipmentConsumingKey key)
        {
            return base.Channel.Delete(key);
        }

        /// <summary>
        /// delete as an asynchronous operation.
        /// </summary>
        /// <param name="key">设备异常类型耗时管理标识符.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> DeleteAsync(EquipmentConsumingKey key)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Delete(key);
            });
        }

        /// <summary>
        /// 获取设备异常类型耗时管理。
        /// </summary>
        /// <param name="key">设备异常类型耗时管理标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;PlanAttendance&gt;" />,设备异常类型耗时管理数据.</returns>
        public MethodReturnResult<EquipmentConsuming> Get(EquipmentConsumingKey key)
        {
            return base.Channel.Get(key);
        }

        /// <summary>
        /// get as an asynchronous operation.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Task&lt;MethodReturnResult&lt;PlanAttendance&gt;&gt;.</returns>
        public async Task<MethodReturnResult<EquipmentConsuming>> GetAsync(EquipmentConsumingKey key)
        {
            return await Task.Run<MethodReturnResult<EquipmentConsuming>>(() =>
            {
                return base.Channel.Get(key);
            });
        }

        /// <summary>
        /// 设备异常类型耗时管理集合。
        /// </summary>
        /// <param name="cfg">查询参数.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;PlanAttendance&gt;&gt;，设备异常类型耗时管理集合.</returns>
        public MethodReturnResult<IList<EquipmentConsuming>> Get(ref ServiceCenter.Model.PagingConfig cfg)
        {
            return base.Channel.Get(ref cfg);
        }
    }
}
