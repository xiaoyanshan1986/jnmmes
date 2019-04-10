﻿// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Client.ZPVC
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 08-08-2014
// ***********************************************************************
// <copyright file="EfficiencyConfigurationServiceClient.cs" company="">
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
using ServiceCenter.MES.Service.Contract.ZPVC;
using ServiceCenter.MES.Model.ZPVC;
using ServiceCenter.Model;

/// <summary>
/// The ZPVC namespace.
/// </summary>
namespace ServiceCenter.MES.Service.Client.ZPVC
{
    /// <summary>
    /// 定义效率档配置数据管理契约调用方式。
    /// </summary>
    public class EfficiencyConfigurationServiceClient : ClientBase<IEfficiencyConfigurationContract>, IEfficiencyConfigurationContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EfficiencyConfigurationServiceClient" /> class.
        /// </summary>
        public EfficiencyConfigurationServiceClient() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EfficiencyConfigurationServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public EfficiencyConfigurationServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EfficiencyConfigurationServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public EfficiencyConfigurationServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EfficiencyConfigurationServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public EfficiencyConfigurationServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EfficiencyConfigurationServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public EfficiencyConfigurationServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }

        /// <summary>
        /// 添加效率档配置数据。
        /// </summary>
        /// <param name="obj">效率档配置数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(EfficiencyConfiguration obj)
        {
            return base.Channel.Add(obj);
        }

        /// <summary>
        /// add as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> AddAsync(EfficiencyConfiguration obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Add(obj);
            });
        }
        /// <summary>
        /// 修改效率档配置数据。
        /// </summary>
        /// <param name="obj">效率档配置数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public ServiceCenter.Model.MethodReturnResult Modify(EfficiencyConfiguration obj)
        {
            return base.Channel.Modify(obj);
        }
        /// <summary>
        /// modify as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> ModifyAsync(EfficiencyConfiguration obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Modify(obj);
            });
        }
        /// <summary>
        /// 删除效率档配置数据。
        /// </summary>
        /// <param name="key">效率档配置数据标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(EfficiencyConfigurationKey key)
        {
            return base.Channel.Delete(key);
        }

        /// <summary>
        /// delete as an asynchronous operation.
        /// </summary>
        /// <param name="key">效率档配置数据标识符.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> DeleteAsync(EfficiencyConfigurationKey key)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Delete(key);
            });
        }

        /// <summary>
        /// 获取效率档配置数据数据。
        /// </summary>
        /// <param name="key">效率档配置数据标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;EfficiencyConfiguration&gt;" />,效率档配置数据数据.</returns>
        public MethodReturnResult<EfficiencyConfiguration> Get(EfficiencyConfigurationKey key)
        {
            return base.Channel.Get(key);
        }

        /// <summary>
        /// get as an asynchronous operation.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Task&lt;MethodReturnResult&lt;EfficiencyConfiguration&gt;&gt;.</returns>
        public async Task<MethodReturnResult<EfficiencyConfiguration>> GetAsync(EfficiencyConfigurationKey key)
        {
            return await Task.Run<MethodReturnResult<EfficiencyConfiguration>>(() =>
            {
                return base.Channel.Get(key);
            });
        }

        /// <summary>
        /// 获取效率档配置数据数据集合。
        /// </summary>
        /// <param name="cfg">查询效率档配置数据.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;EfficiencyConfiguration&gt;&gt;，效率档配置数据数据集合.</returns>
        public MethodReturnResult<IList<EfficiencyConfiguration>> Get(ref ServiceCenter.Model.PagingConfig cfg)
        {
            return base.Channel.Get(ref cfg);
        }
    }
}