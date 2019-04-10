// ***********************************************************************
// Assembly         : ServiceCenter.MES.Service.Contract.RBAC
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 07-30-2014
// ***********************************************************************
// <copyright file="IRoleContract.cs" company="">
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
    /// 定义角色管理契约接口。
    /// </summary>
    [ServiceContract]
    public interface IRoleContract
    {
        /// <summary>
        /// 添加角色。
        /// </summary>
        /// <param name="obj">角色数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        [OperationContract]
        MethodReturnResult Add(Role obj);
        /// <summary>
        /// 修改角色。
        /// </summary>
        /// <param name="obj">角色数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        [OperationContract]
        MethodReturnResult Modify(Role obj);
        /// <summary>
        /// 设置用户角色数据。
        /// </summary>
        /// <param name="roleName">角色名。</param>
        /// <param name="lst">用户角色数据集合。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        [OperationContract]
        MethodReturnResult SetRoleUser(string roleName, IList<UserInRole> lst);
        /// <summary>
        /// 设置角色资源数据。
        /// </summary>
        /// <param name="roleName">角色名。</param>
        /// <param name="lst">角色资源数据集合。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        [OperationContract]
        MethodReturnResult SetRoleResource(string roleName, IList<RoleOwnResource> lst);
        /// <summary>
        /// 删除角色。
        /// </summary>
        /// <param name="name">角色名。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        [OperationContract]
        MethodReturnResult Delete(string name);
        /// <summary>
        /// 获取角色数据。
        /// </summary>
        /// <param name="name">角色名.</param>
        /// <returns><see cref="MethodReturnResult&lt;Role&gt;" />,角色数据.</returns>
        [OperationContract]
        MethodReturnResult<Role> Get(string name);
        /// <summary>
        /// 获取角色数据集合。
        /// </summary>
        /// <param name="cfg">查询参数.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;Role&gt;&gt;，角色数据集合.</returns>
        [OperationContract(Name = "GetList")]
        MethodReturnResult<IList<Role>> Get(ref PagingConfig cfg);
    }
}
