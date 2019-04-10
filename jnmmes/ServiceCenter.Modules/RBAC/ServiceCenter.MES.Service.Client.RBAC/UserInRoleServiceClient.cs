// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Client.FMM
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 07-31-2014
// ***********************************************************************
// <copyright file="UserInRoleServiceClient.cs" company="">
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
    /// 定义用户角色管理契约调用方式。
    /// </summary>
    public class UserInRoleServiceClient : ClientBase<IUserInRoleContract>, IUserInRoleContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserInRoleServiceClient" /> class.
        /// </summary>
        public UserInRoleServiceClient() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserInRoleServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public UserInRoleServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserInRoleServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public UserInRoleServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserInRoleServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public UserInRoleServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserInRoleServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public UserInRoleServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }



        /// <summary>
        /// 添加用户角色。
        /// </summary>
        /// <param name="obj">用户角色数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public ServiceCenter.Model.MethodReturnResult Add(UserInRole obj)
        {
            return base.Channel.Add(obj);
        }

        /// <summary>
        /// add as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> AddAsync(UserInRole obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Add(obj);
            });
        }
        /// <summary>
        /// 修改用户角色。
        /// </summary>
        /// <param name="obj">用户角色数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public ServiceCenter.Model.MethodReturnResult Modify(UserInRole obj)
        {
            return base.Channel.Modify(obj);
        }


        /// <summary>
        /// modify as an asynchronous operation.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> ModifyAsync(UserInRole obj)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Modify(obj);
            });
        }
        /// <summary>
        /// 删除用户角色。
        /// </summary>
        /// <param name="key">用户角色标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        public ServiceCenter.Model.MethodReturnResult Delete(UserInRoleKey key)
        {
            return base.Channel.Delete(key);
        }


        /// <summary>
        /// delete as an asynchronous operation.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Task&lt;MethodReturnResult&gt;.</returns>
        public async Task<MethodReturnResult> DeleteAsync(UserInRoleKey key)
        {
            return await Task.Run<MethodReturnResult>(() =>
            {
                return base.Channel.Delete(key);
            });
        }
        /// <summary>
        /// 获取用户角色数据。
        /// </summary>
        /// <param name="key">用户角色标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;UserInRole&gt;" />,用户角色数据.</returns>
        public ServiceCenter.Model.MethodReturnResult<UserInRole> Get(UserInRoleKey key)
        {
            return base.Channel.Get(key);
        }

        /// <summary>
        /// get as an asynchronous operation.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Task&lt;MethodReturnResult&lt;UserInRole&gt;&gt;.</returns>
        public async Task<MethodReturnResult<UserInRole>> GetAsync(UserInRoleKey key)
        {
            return await Task.Run<MethodReturnResult<UserInRole>>(() =>
            {
                return base.Channel.Get(key);
            });
        }

        /// <summary>
        /// 获取用户角色数据集合。
        /// </summary>
        /// <param name="cfg">查询参数.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;UserInRole&gt;&gt;，用户角色数据集合.</returns>
        public ServiceCenter.Model.MethodReturnResult<IList<UserInRole>> Get(ref ServiceCenter.Model.PagingConfig cfg)
        {
            return base.Channel.Get(ref cfg);
        }
    }
}
