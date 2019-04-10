// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Client.FMM
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 07-30-2014
// ***********************************************************************
// <copyright file="ResourceServiceClient.cs" company="">
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
using ServiceCenter.MES.Service.Contract.RBAC;
using ServiceCenter.MES.Model.RBAC;
using ServiceCenter.Model;

/// <summary>
/// The RBAC namespace.
/// </summary>
namespace ServiceCenter.MES.Service.Client.RBAC
{
    /// <summary>
    /// 定义资源管理契约调用方式。
    /// </summary>
    public class ResourceServiceClient : ClientBase<IResourceContract>, IResourceContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceServiceClient" /> class.
        /// </summary>
        public ResourceServiceClient() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public ResourceServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public ResourceServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public ResourceServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public ResourceServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }



        /// <summary>
        /// 添加资源。
        /// </summary>
        /// <param name="obj">资源数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(Resource obj)
        {
            return base.Channel.Add(obj);
        }

        public async Task<MethodReturnResult> AddAsync(Resource obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Add(obj);
            });
        }

        /// <summary>
        /// 修改资源。
        /// </summary>
        /// <param name="obj">资源数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(Resource obj)
        {
            return base.Channel.Modify(obj);
        }

        public async Task<MethodReturnResult> ModifyAsync(Resource obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Modify(obj);
            });
        }

        /// <summary>
        /// 删除资源。
        /// </summary>
        /// <param name="key">资源标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(ResourceKey key)
        {
            return base.Channel.Delete(key);
        }


        public async Task<MethodReturnResult> DeleteAsync(ResourceKey key)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Delete(key);
            });
        }
        /// <summary>
        /// 获取资源数据。
        /// </summary>
        /// <param name="key">资源标识符。</param>
        /// <returns><see cref="MethodReturnResult&lt;Resource&gt;" />,资源数据.</returns>
        public MethodReturnResult<Resource> Get(ResourceKey key)
        {
            return base.Channel.Get(key);
        }

        public async Task<MethodReturnResult<Resource>> GetAsync(ResourceKey key)
        {
            return await Task.Run<MethodReturnResult<Resource>>(() =>
            {
                return base.Channel.Get(key);
            });
        }
        /// <summary>
        /// 获取资源数据集合。
        /// </summary>
        /// <param name="cfg">查询参数.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;Resource&gt;&gt;，资源数据集合.</returns>
        public ServiceCenter.Model.MethodReturnResult<IList<Resource>> Get(ref ServiceCenter.Model.PagingConfig cfg)
        {
            return base.Channel.Get(ref cfg);
        }

    }
}
