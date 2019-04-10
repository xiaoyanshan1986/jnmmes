// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Client.FMM
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 07-30-2014
// ***********************************************************************
// <copyright file="RoleOwnResourceServiceClient.cs" company="">
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
    /// 定义角色资源管理契约调用方式。
    /// </summary>
    public class RoleOwnResourceServiceClient : ClientBase<IRoleOwnResourceContract>, IRoleOwnResourceContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoleOwnResourceServiceClient" /> class.
        /// </summary>
        public RoleOwnResourceServiceClient() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleOwnResourceServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public RoleOwnResourceServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleOwnResourceServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public RoleOwnResourceServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleOwnResourceServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public RoleOwnResourceServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleOwnResourceServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public RoleOwnResourceServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }



        /// <summary>
        /// 添加角色资源。
        /// </summary>
        /// <param name="obj">角色资源数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(RoleOwnResource obj)
        {
            return base.Channel.Add(obj);
        }

        public async Task<MethodReturnResult> AddAsync(RoleOwnResource obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Add(obj);
            });
        }

        /// <summary>
        /// 修改角色资源。
        /// </summary>
        /// <param name="obj">角色资源数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Modify(RoleOwnResource obj)
        {
            return base.Channel.Modify(obj);
        }

        public async Task<MethodReturnResult> ModifyAsync(RoleOwnResource obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Modify(obj);
            });
        }
        /// <summary>
        /// 删除角色资源。
        /// </summary>
        /// <param name="key">角色资源标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(RoleOwnResourceKey key)
        {
            return base.Channel.Delete(key);
        }

        public async Task<MethodReturnResult> DeleteAsync(RoleOwnResourceKey key)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Delete(key);
            });
        }
        /// <summary>
        /// 获取角色资源数据。
        /// </summary>
        /// <param name="key">角色资源标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;RoleOwnResource&gt;" />,角色资源数据.</returns>
        public MethodReturnResult<RoleOwnResource> Get(RoleOwnResourceKey key)
        {
            return base.Channel.Get(key);
        }

        public async Task<MethodReturnResult<RoleOwnResource>> GetAsync(RoleOwnResourceKey key)
        {
            return await Task.Run<MethodReturnResult<RoleOwnResource>>(() =>
            {
                return base.Channel.Get(key);
            });
        }

        /// <summary>
        /// 获取角色资源数据集合。
        /// </summary>
        /// <param name="cfg">查询参数.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;RoleOwnResource&gt;&gt;，角色资源数据集合.</returns>
        public MethodReturnResult<IList<RoleOwnResource>> Get(ref ServiceCenter.Model.PagingConfig cfg)
        {
            return base.Channel.Get(ref cfg);
        }
    }
}
