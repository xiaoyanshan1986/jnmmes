// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Client.FMM
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 07-31-2014
// ***********************************************************************
// <copyright file="UserOwnResourceServiceClient.cs" company="">
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
    /// 定义用户资源管理契约调用方式。
    /// </summary>
    public class UserOwnResourceServiceClient : ClientBase<IUserOwnResourceContract>, IUserOwnResourceContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserOwnResourceServiceClient" /> class.
        /// </summary>
        public UserOwnResourceServiceClient() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserOwnResourceServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public UserOwnResourceServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserOwnResourceServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public UserOwnResourceServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserOwnResourceServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public UserOwnResourceServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserOwnResourceServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public UserOwnResourceServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }



        /// <summary>
        /// 添加用户资源。
        /// </summary>
        /// <param name="obj">用户资源数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public ServiceCenter.Model.MethodReturnResult Add(UserOwnResource obj)
        {
            return base.Channel.Add(obj);
        }

        /// <summary>
        /// add as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> AddAsync(UserOwnResource obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Add(obj);
            });
        }

        /// <summary>
        /// 修改用户资源。
        /// </summary>
        /// <param name="obj">用户资源数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public ServiceCenter.Model.MethodReturnResult Modify(UserOwnResource obj)
        {
            return base.Channel.Modify(obj);
        }

        /// <summary>
        /// modify as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> ModifyAsync(UserOwnResource obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Modify(obj);
            });
        }
        /// <summary>
        /// 删除用户资源。
        /// </summary>
        /// <param name="key">用户资源标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public ServiceCenter.Model.MethodReturnResult Delete(UserOwnResourceKey key)
        {
            return base.Channel.Delete(key);
        }


        /// <summary>
        /// delete as an asynchronous operation.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> DeleteAsync(UserOwnResourceKey key)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Delete(key);
            });
        }
        /// <summary>
        /// 获取用户资源数据。
        /// </summary>
        /// <param name="key">用户资源标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;UserOwnResource&gt;" />,用户资源数据.</returns>
        public ServiceCenter.Model.MethodReturnResult<UserOwnResource> Get(UserOwnResourceKey key)
        {
            return base.Channel.Get(key);
        }

        /// <summary>
        /// get as an asynchronous operation.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Task&lt;MethodReturnResult&lt;UserOwnResource&gt;&gt;.</returns>
        public async Task<MethodReturnResult<UserOwnResource>> GetAsync(UserOwnResourceKey key)
        {
            return await Task.Run<MethodReturnResult<UserOwnResource>>(() =>
            {
                return base.Channel.Get(key);
            });
        }
        /// <summary>
        /// 获取用户资源数据集合。
        /// </summary>
        /// <param name="cfg">查询参数.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;UserOwnResource&gt;&gt;，用户资源数据集合.</returns>
        public ServiceCenter.Model.MethodReturnResult<IList<UserOwnResource>> Get(ref ServiceCenter.Model.PagingConfig cfg)
        {
            return base.Channel.Get(ref cfg);
        }
    }
}
