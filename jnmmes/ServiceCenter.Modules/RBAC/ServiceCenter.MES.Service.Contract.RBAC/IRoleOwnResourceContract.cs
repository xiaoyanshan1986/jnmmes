// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Contract.RBAC
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 07-30-2014
// ***********************************************************************
// <copyright file="IRoleOwnResourceContract.cs" company="">
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
    /// 定义角色资源管理契约接口。
    /// </summary>
    [ServiceContract]
    public interface IRoleOwnResourceContract
    {
        /// <summary>
        /// 添加角色资源。
        /// </summary>
        /// <param name="obj">角色资源数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        [OperationContract]
        MethodReturnResult Add(RoleOwnResource obj);
        /// <summary>
        /// 修改角色资源。
        /// </summary>
        /// <param name="obj">角色资源数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        [OperationContract]
        MethodReturnResult Modify(RoleOwnResource obj);
        /// <summary>
        /// 删除角色资源。
        /// </summary>
        /// <param name="key">角色资源标识符。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        [OperationContract]
        MethodReturnResult Delete(RoleOwnResourceKey key);
        /// <summary>
        /// 获取角色资源数据。
        /// </summary>
        /// <param name="key">角色资源标识符.</param>
        /// <returns><see cref="MethodReturnResult&lt;RoleOwnResource&gt;" />,角色资源数据.</returns>
        [OperationContract]
        MethodReturnResult<RoleOwnResource> Get(RoleOwnResourceKey key);
        /// <summary>
        /// 获取角色资源数据集合。
        /// </summary>
        /// <param name="cfg">查询参数.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;RoleOwnResource&gt;&gt;，角色资源数据集合.</returns>
        [OperationContract(Name = "GetList")]
        MethodReturnResult<IList<RoleOwnResource>> Get(ref PagingConfig cfg);
    }
}
