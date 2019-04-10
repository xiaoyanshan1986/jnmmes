﻿// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Client.FMM
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 08-08-2014
// ***********************************************************************
// <copyright file="RouteEnterpriseServiceClient.cs" company="">
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
/// The FMM namespace.
/// </summary>
namespace ServiceCenter.MES.Service.Client.FMM
{
    /// <summary>
    /// 定义工艺流程组管理契约调用方式。
    /// </summary>
    public class RouteEnterpriseServiceClient : ClientBase<IRouteEnterpriseContract>, IRouteEnterpriseContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RouteEnterpriseServiceClient" /> class.
        /// </summary>
        public RouteEnterpriseServiceClient() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteEnterpriseServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public RouteEnterpriseServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteEnterpriseServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public RouteEnterpriseServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteEnterpriseServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public RouteEnterpriseServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteEnterpriseServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public RouteEnterpriseServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }



        /// <summary>
        /// 添加工艺流程组。
        /// </summary>
        /// <param name="obj">工艺流程组数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(RouteEnterprise obj)
        {
            return base.Channel.Add(obj);
        }

        /// <summary>
        /// add as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> AddAsync(RouteEnterprise obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Add(obj);
            });
        }
        /// <summary>
        /// 修改工艺流程组。
        /// </summary>
        /// <param name="obj">工艺流程组数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public ServiceCenter.Model.MethodReturnResult Modify(RouteEnterprise obj)
        {
            return base.Channel.Modify(obj);
        }
        /// <summary>
        /// modify as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> ModifyAsync(RouteEnterprise obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Modify(obj);
            });
        }
        /// <summary>
        /// 删除工艺流程组。
        /// </summary>
        /// <param name="key">工艺流程组标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(string key)
        {
            return base.Channel.Delete(key);
        }

        /// <summary>
        /// delete as an asynchronous operation.
        /// </summary>
        /// <param name="key">工艺流程组标识符.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> DeleteAsync(string key)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Delete(key);
            });
        }
        /// <summary>
        /// 获取工艺流程组数据。
        /// </summary>
        /// <param name="key">工艺流程组标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;RouteEnterprise&gt;" />,工艺流程组数据.</returns>
        public MethodReturnResult<RouteEnterprise> Get(string key)
        {
            return base.Channel.Get(key);
        }

        /// <summary>
        /// get as an asynchronous operation.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Task&lt;MethodReturnResult&lt;RouteEnterprise&gt;&gt;.</returns>
        public async Task<MethodReturnResult<RouteEnterprise>> GetAsync(string key)
        {
            return await Task.Run<MethodReturnResult<RouteEnterprise>>(() =>
            {
                return base.Channel.Get(key);
            });
        }

        /// <summary>
        /// 获取工艺流程组数据集合。
        /// </summary>
        /// <param name="cfg">查询参数.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;RouteEnterprise&gt;&gt;，工艺流程组数据集合.</returns>
        public MethodReturnResult<IList<RouteEnterprise>> Get(ref ServiceCenter.Model.PagingConfig cfg)
        {
            return base.Channel.Get(ref cfg);
        }
    }
}