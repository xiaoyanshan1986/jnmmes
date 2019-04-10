// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Client.FMM
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 07-30-2014
// ***********************************************************************
// <copyright file="UserAuthenticateServiceClient.cs" company="">
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
    /// 定义用验证服务契约调用方式。
    /// </summary>
    public class UserAuthenticateServiceClient : ClientBase<IUserAuthenticateContract>, IUserAuthenticateContract, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserAuthenticateServiceClient" /> class.
        /// </summary>
        public UserAuthenticateServiceClient() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAuthenticateServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public UserAuthenticateServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAuthenticateServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public UserAuthenticateServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAuthenticateServiceClient" /> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public UserAuthenticateServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAuthenticateServiceClient" /> class.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="remoteAddress">The remote address.</param>
        public UserAuthenticateServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }

        /// <summary>
        /// 用户身份验证。
        /// </summary>
        /// <param name="loginName">用户登录名。</param>
        /// <param name="password">用户登录密码。</param>
        /// <returns>验证反馈信息。</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public MethodReturnResult<User> Authenticate(string loginName, string password)
        {
            return base.Channel.Authenticate(loginName, password);
        }

        /// <summary>
        /// authenticate as an asynchronous operation.
        /// </summary>
        /// <param name="loginName">Name of the login.</param>
        /// <param name="password">The password.</param>
        /// <returns>Task&lt;MethodReturnResult&lt;User&gt;&gt;.</returns>
        public async Task<MethodReturnResult<User>> AuthenticateAsync(string loginName, string password)
        {
            return await Task.Run<MethodReturnResult<User>>
                        (
                           () =>
                           {
                               return base.Channel.Authenticate(loginName, password);
                           }
                        );
        }
        /// <summary>
        /// Gets the resource list.
        /// </summary>
        /// <param name="loginName">Name of the login.</param>
        /// <param name="resourceType">Type of the resource.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;Resource&gt;&gt;.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public MethodReturnResult<IList<Resource>> GetResourceList(string loginName, ResourceType resourceType)
        {
            return base.Channel.GetResourceList(loginName, resourceType);
        }

        /// <summary>
        /// get resource list as an asynchronous operation.
        /// </summary>
        /// <param name="loginName">Name of the login.</param>
        /// <param name="resourceType">Type of the resource.</param>
        /// <returns>Task&lt;MethodReturnResult&lt;IList&lt;Resource&gt;&gt;&gt;.</returns>
        public async Task<MethodReturnResult<IList<Resource>>> GetResourceListAsync(string loginName, ResourceType resourceType)
        {
            return await Task.Run<MethodReturnResult<IList<Resource>>>
                        (
                           () =>
                           {
                               return base.Channel.GetResourceList(loginName, resourceType);
                           }
                        );
        }
        /// <summary>
        /// Gets the resource.
        /// </summary>
        /// <param name="loginName">Name of the login.</param>
        /// <param name="resourceType">Type of the resource.</param>
        /// <param name="resourceCode">The resource code.</param>
        /// <returns>MethodReturnResult&lt;Resource&gt;.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public MethodReturnResult<Resource> GetResource(string loginName, ResourceType resourceType, string resourceCode)
        {
            return base.Channel.GetResource(loginName, resourceType,resourceCode);
        }

        /// <summary>
        /// get resource as an asynchronous operation.
        /// </summary>
        /// <param name="loginName">Name of the login.</param>
        /// <param name="resourceType">Type of the resource.</param>
        /// <param name="resourceCode">The resource code.</param>
        /// <returns>Task&lt;MethodReturnResult&lt;Resource&gt;&gt;.</returns>
        public async Task<MethodReturnResult<Resource>> GetResourceAsync(string loginName, ResourceType resourceType, string resourceCode)
        {
            return await Task.Run<MethodReturnResult<Resource>>
                         (
                            ()=>{
                                return base.Channel.GetResource(loginName, resourceType, resourceCode);
                            }
                         );
        }


        public MethodReturnResult AuthenticateResource(string loginName, ResourceType resourceType, string resourceData)
        {
            return base.Channel.AuthenticateResource(loginName, resourceType, resourceData);
        }

        public async Task<MethodReturnResult> AuthenticateResourceAsync(string loginName, ResourceType resourceType, string resourceData)
        {
            return await Task.Run<MethodReturnResult>
             (
                () =>
                {
                    return base.Channel.AuthenticateResource(loginName, resourceType, resourceData);
                }
             );
        }
    }
}
