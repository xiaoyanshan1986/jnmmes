﻿// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Client.PPM
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 08-08-2014
// ***********************************************************************
// <copyright file="WorkOrderAttributeServiceClient.cs" company="">
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
using ServiceCenter.MES.Service.Contract.PPM;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.Model;

/// <summary>
/// The PPM namespace.
/// </summary>
namespace ServiceCenter.MES.Service.Client.PPM
{
    /// <summary>
    /// 定义工单属性管理契约调用方式。
    /// </summary>
    public class WorkOrderAttributeServiceClient : ClientBase<IWorkOrderAttributeContract>, IWorkOrderAttributeContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkOrderAttributeServiceClient" /> class.
        /// </summary>
        public WorkOrderAttributeServiceClient() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkOrderAttributeServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public WorkOrderAttributeServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkOrderAttributeServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public WorkOrderAttributeServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkOrderAttributeServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public WorkOrderAttributeServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkOrderAttributeServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public WorkOrderAttributeServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }

        /// <summary>
        /// 添加工单属性。
        /// </summary>
        /// <param name="obj">工单属性数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(WorkOrderAttribute obj)
        {
            return base.Channel.Add(obj);
        }

        /// <summary>
        /// add as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> AddAsync(WorkOrderAttribute obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Add(obj);
            });
        }
        /// <summary>
        /// 修改工单属性。
        /// </summary>
        /// <param name="obj">工单属性数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public ServiceCenter.Model.MethodReturnResult Modify(WorkOrderAttribute obj)
        {
            return base.Channel.Modify(obj);
        }
        /// <summary>
        /// modify as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> ModifyAsync(WorkOrderAttribute obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Modify(obj);
            });
        }
        /// <summary>
        /// 删除工单属性。
        /// </summary>
        /// <param name="key">工单属性标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(WorkOrderAttributeKey key)
        {
            return base.Channel.Delete(key);
        }

        /// <summary>
        /// delete as an asynchronous operation.
        /// </summary>
        /// <param name="key">工单属性标识符.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> DeleteAsync(WorkOrderAttributeKey key)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Delete(key);
            });
        }

        /// <summary>
        /// 获取工单属性数据。
        /// </summary>
        /// <param name="key">工单属性标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;WorkOrderAttribute&gt;" />,工单属性数据.</returns>
        public MethodReturnResult<WorkOrderAttribute> Get(WorkOrderAttributeKey key)
        {
            return base.Channel.Get(key);
        }

        /// <summary>
        /// get as an asynchronous operation.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Task&lt;MethodReturnResult&lt;WorkOrderAttribute&gt;&gt;.</returns>
        public async Task<MethodReturnResult<WorkOrderAttribute>> GetAsync(WorkOrderAttributeKey key)
        {
            return await Task.Run<MethodReturnResult<WorkOrderAttribute>>(() =>
            {
                return base.Channel.Get(key);
            });
        }

        /// <summary>
        /// 获取工单属性数据集合。
        /// </summary>
        /// <param name="cfg">查询工单属性.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;WorkOrderAttribute&gt;&gt;，工单属性数据集合.</returns>
        public MethodReturnResult<IList<WorkOrderAttribute>> Get(ref ServiceCenter.Model.PagingConfig cfg)
        {
            return base.Channel.Get(ref cfg);
        }
    }
}