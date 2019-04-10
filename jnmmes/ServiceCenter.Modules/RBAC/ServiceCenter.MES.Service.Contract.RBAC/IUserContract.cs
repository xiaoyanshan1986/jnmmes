// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Contract.RBAC
// Author           : peter
// Created          : 07-25-2014
//
// Last Modified By : peter
// Last Modified On : 07-30-2014
// ***********************************************************************
// <copyright file="IUserContract.cs" company="">
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
    /// 定义用户管理契约接口。
    /// </summary>
    [ServiceContract]
    public interface IUserContract
    {
        /// <summary>
        /// 添加用户。
        /// </summary>
        /// <param name="obj">用户数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        [OperationContract]
        MethodReturnResult Add(User obj);
        /// <summary>
        /// 修改用户。
        /// </summary>
        /// <param name="obj">用户数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        [OperationContract]
        MethodReturnResult Modify(User obj);

        /// <summary>
        /// 设置用户角色数据。
        /// </summary>
        /// <param name="loginName">用户登录名。</param>
        /// <param name="lst">角色数据集合。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        [OperationContract]
        MethodReturnResult SetUserRole(string loginName,IList<UserInRole> lst);
        /// <summary>
        /// 设置用户资源数据。
        /// </summary>
        /// <param name="loginName">用户登录名。</param>
        /// <param name="lst">用户资源数据集合。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        [OperationContract]
        MethodReturnResult SetUserResource(string loginName,IList<UserOwnResource> lst);
        /// <summary>
        /// 删除用户。
        /// </summary>
        /// <param name="name">登录名。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        [OperationContract]
        MethodReturnResult Delete(string name);
        /// <summary>
        /// 获取用户数据。
        /// </summary>
        /// <param name="name">登录名.</param>
        /// <returns><see cref="MethodReturnResult&lt;User&gt;" />,用户数据.</returns>
        [OperationContract]
        MethodReturnResult<User> Get(string name);
        /// <summary>
        /// 获取用户数据集合。
        /// </summary>
        /// <param name="cfg">查询参数.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;User&gt;&gt;，用户数据集合.</returns>
        [OperationContract(Name = "GetList")]
        MethodReturnResult<IList<User>> Get(ref PagingConfig cfg);
    }
}
