// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Client.FMM
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 07-30-2014
// ***********************************************************************
// <copyright file="UserServiceClient.cs" company="">
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
    /// 定义用户管理契约调用方式。
    /// </summary>
    public class UserServiceClient : ClientBase<IUserContract>, IUserContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserServiceClient" /> class.
        /// </summary>
        public UserServiceClient() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public UserServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public UserServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public UserServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public UserServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }



        /// <summary>
        /// 添加用户。
        /// </summary>
        /// <param name="obj">用户数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(User obj)
        {
            return base.Channel.Add(obj);
        }

        /// <summary>
        /// add as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> AddAsync(User obj)
        {
            return await Task.Run<MethodReturnResult>(() => { return base.Channel.Add(obj); });
        }

        /// <summary>
        /// 修改用户。
        /// </summary>
        /// <param name="obj">用户数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public ServiceCenter.Model.MethodReturnResult Modify(User obj)
        {
            return base.Channel.Modify(obj);
        }


        /// <summary>
        /// modify as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> ModifyAsync(User obj)
        {
            return await Task.Run<MethodReturnResult>(() => { return base.Channel.Modify(obj); });
        }

        /// <summary>
        /// 删除用户。
        /// </summary>
        /// <param name="name">登录名。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public ServiceCenter.Model.MethodReturnResult Delete(string name)
        {
            return base.Channel.Delete(name);
        }

        /// <summary>
        /// delete as an asynchronous operation.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> DeleteAsync(string name)
        {
            return await Task.Run<MethodReturnResult>(() => { return base.Channel.Delete(name); });
        }

        /// <summary>
        /// 获取用户数据。
        /// </summary>
        /// <param name="name">登录名.</param>
        /// <returns><see cref="MethodReturnResult&lt;User&gt;" />,用户数据.</returns>
        public ServiceCenter.Model.MethodReturnResult<User> Get(string name)
        {
            return base.Channel.Get(name);
        }

        /// <summary>
        /// get as an asynchronous operation.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Task&lt;MethodReturnResult&lt;User&gt;&gt;.</returns>
        public async Task<MethodReturnResult<User>> GetAsync(string name)
        {
            return await Task.Run<MethodReturnResult<User>>(() => { return base.Channel.Get(name); });
        }
        
        /// <summary>
        /// 获取用户数据集合。
        /// </summary>
        /// <param name="cfg">查询参数.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;User&gt;&gt;，用户数据集合.</returns>
        public ServiceCenter.Model.MethodReturnResult<IList<User>> Get(ref PagingConfig cfg)
        {
            return base.Channel.Get(ref cfg);
        }



        public MethodReturnResult SetUserRole(string loginName, IList<UserInRole> lst)
        {
            return base.Channel.SetUserRole(loginName,lst);
        }

        public async Task<MethodReturnResult> SetUserRoleAsync(string loginName, IList<UserInRole> lst)
        {
            return await Task.Run<MethodReturnResult>(() => {
                return base.Channel.SetUserRole(loginName,lst);
            });
        }

        public MethodReturnResult SetUserResource(string loginName, IList<UserOwnResource> lst)
        {
            return base.Channel.SetUserResource(loginName, lst);
        }

        public async Task<MethodReturnResult> SetUserResourceAsync(string loginName, IList<UserOwnResource> lst)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.SetUserResource(loginName, lst);
            });
        }
    }
}
