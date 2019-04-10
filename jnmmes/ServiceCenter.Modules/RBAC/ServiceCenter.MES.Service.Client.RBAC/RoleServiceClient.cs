// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Client.FMM
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 07-31-2014
// ***********************************************************************
// <copyright file="RoleServiceClient.cs" company="">
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
    /// 定义角色管理契约调用方式。
    /// </summary>
    public class RoleServiceClient : ClientBase<IRoleContract>, IRoleContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoleServiceClient" /> class.
        /// </summary>
        public RoleServiceClient() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public RoleServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public RoleServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public RoleServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public RoleServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }



        /// <summary>
        /// 添加角色。
        /// </summary>
        /// <param name="obj">角色数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Add(Role obj)
        {
            return base.Channel.Add(obj);
        }

        /// <summary>
        /// add as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> AddAsync(Role obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Add(obj);
            });
        }
        /// <summary>
        /// 修改角色。
        /// </summary>
        /// <param name="obj">角色数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public ServiceCenter.Model.MethodReturnResult Modify(Role obj)
        {
            return base.Channel.Modify(obj);
        }
        /// <summary>
        /// modify as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> ModifyAsync(Role obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Modify(obj);
            });
        }
        /// <summary>
        /// 删除角色。
        /// </summary>
        /// <param name="name">角色名。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public MethodReturnResult Delete(string name)
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
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Delete(name);
            });
        }
        /// <summary>
        /// 获取角色数据。
        /// </summary>
        /// <param name="name">角色名.</param>
        /// <returns><see cref="MethodReturnResult&lt;Role&gt;" />,角色数据.</returns>
        public MethodReturnResult<Role> Get(string name)
        {
            return base.Channel.Get(name);
        }

        /// <summary>
        /// get as an asynchronous operation.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Task&lt;MethodReturnResult&lt;Role&gt;&gt;.</returns>
        public async Task<MethodReturnResult<Role>> GetAsync(string name)
        {
            return await Task.Run<MethodReturnResult<Role>>(() =>
            {
                return base.Channel.Get(name);
            });
        }

        /// <summary>
        /// 获取角色数据集合。
        /// </summary>
        /// <param name="cfg">查询参数.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;Role&gt;&gt;，角色数据集合.</returns>
        public MethodReturnResult<IList<Role>> Get(ref ServiceCenter.Model.PagingConfig cfg)
        {
            return base.Channel.Get(ref cfg);
        }


        public MethodReturnResult SetRoleUser(string roleName, IList<UserInRole> lst)
        {
            return base.Channel.SetRoleUser(roleName, lst);
        }

        public async Task<MethodReturnResult> SetRoleUserAsync(string roleName, IList<UserInRole> lst)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.SetRoleUser(roleName, lst);
            });
        }

        public MethodReturnResult SetRoleResource(string roleName, IList<RoleOwnResource> lst)
        {
            return base.Channel.SetRoleResource(roleName, lst);
        }

        public async Task<MethodReturnResult> SetRoleResourceAsync(string roleName, IList<RoleOwnResource> lst)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.SetRoleResource(roleName, lst);
            });
        }
    }
}
