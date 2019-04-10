// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Contract.RBAC
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 07-30-2014
// ***********************************************************************
// <copyright file="IUserAuthenticateContract.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using ServiceCenter.MES.Model.RBAC;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// The RBAC namespace.
/// </summary>
namespace ServiceCenter.MES.Service.Contract.RBAC
{
    /// <summary>
    /// 定义用户验证契约接口。
    /// </summary>
    [ServiceContract]
    public interface IUserAuthenticateContract
    {
        /// <summary>
        /// 用户身份验证。
        /// </summary>
        /// <param name="loginName">用户登录名。</param>
        /// <param name="password">用户登录密码。</param>
        /// <returns>验证反馈信息。</returns>
        [OperationContract]
        MethodReturnResult<User> Authenticate(string loginName, string password);
        /// <summary>
        /// 用户资源权限验证。
        /// </summary>
        /// <param name="loginName">用户登录名。</param>
        /// <param name="resourceType">资源类型。</param>
        /// <param name="resourceData">资源数据。</param>
        /// <returns>MethodReturnResult&lt;Resource&gt;.</returns>
        [OperationContract]
        MethodReturnResult AuthenticateResource(string loginName, ResourceType resourceType, string resourceData);
        /// <summary>
        /// Gets the resource list.
        /// </summary>
        /// <param name="loginName">Name of the login.</param>
        /// <param name="resourceType">Type of the resource.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;Resource&gt;&gt;.</returns>
        [OperationContract]
        MethodReturnResult<IList<Resource>> GetResourceList(string loginName, ResourceType resourceType);
        /// <summary>
        /// Gets the resource.
        /// </summary>
        /// <param name="loginName">Name of the login.</param>
        /// <param name="resourceType">Type of the resource.</param>
        /// <param name="resourceCode">The resource code.</param>
        /// <returns>MethodReturnResult&lt;Resource&gt;.</returns>
        [OperationContract]
        MethodReturnResult<Resource> GetResource(string loginName, ResourceType resourceType, string resourceCode);

    }
}
