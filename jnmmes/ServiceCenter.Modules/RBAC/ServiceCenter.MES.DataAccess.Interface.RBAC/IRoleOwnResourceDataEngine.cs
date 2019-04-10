// ***********************************************************************
// Assembly         : ServiceCenter.MES.DataAccess.Interface.RBAC
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 07-30-2014
// ***********************************************************************
// <copyright file="IRoleOwnResourceDataEngine.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.Model.RBAC;
using ServiceCenter.DataAccess;
using ServiceCenter.Model;

/// <summary>
/// The RBAC namespace.
/// </summary>
namespace ServiceCenter.MES.DataAccess.Interface.RBAC
{
    /// <summary>
    /// 定义角色资源数据访问接口。
    /// </summary>
    public interface IRoleOwnResourceDataEngine
        : IDatabaseDataEngine<RoleOwnResource, RoleOwnResourceKey>
    {
        /// <summary>
        /// 根据角色名删除对应角色资源数据。
        /// </summary>
        /// <param name="roleName">登录名。</param>
        void DeleteByRoleName(string roleName);

        /// <summary>
        /// 根据资源删除对应角色资源数据。
        /// </summary>
        /// <param name="resourceKey">资源主键。</param>
        void DeleteByResourceKey(ResourceKey resourceKey);
    }
}
