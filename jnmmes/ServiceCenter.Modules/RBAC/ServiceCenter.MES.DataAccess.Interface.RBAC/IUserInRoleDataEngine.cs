// ***********************************************************************
// Assembly         : ServiceCenter.MES.DataAccess.Interface.RBAC
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 07-30-2014
// ***********************************************************************
// <copyright file="IUserInRoleDataEngine.cs" company="">
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
    /// 定义用户角色数据访问接口。
    /// </summary>
    public interface IUserInRoleDataEngine
        : IDatabaseDataEngine<UserInRole, UserInRoleKey>
    {
        /// <summary>
        /// 根据用户登录名删除对应角色。
        /// </summary>
        /// <param name="loginName">登录名。</param>
        void DeleteByLoginName(string loginName);
        /// <summary>
        /// 根据用户角色名删除对应用户。
        /// </summary>
        /// <param name="roleName">角色名。</param>
        void DeleteByRoleName(string roleName);
    }
}
