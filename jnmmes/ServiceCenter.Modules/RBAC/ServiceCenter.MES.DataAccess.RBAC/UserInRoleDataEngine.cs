// ***********************************************************************
// Assembly         : ServiceCenter.MES.DataAccess.RBAC
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 08-07-2014
// ***********************************************************************
// <copyright file="UserInRoleDataEngine.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using ServiceCenter.MES.DataAccess.Interface.RBAC;
using ServiceCenter.MES.Model.RBAC;
using NHibernate;
using ServiceCenter.DataAccess;
using ServiceCenter.Model;
using ServiceCenter.Common.DataAccess.NHibernate;

/// <summary>
/// The RBAC namespace.
/// </summary>
namespace ServiceCenter.MES.DataAccess.RBAC
{
    /// <summary>
    /// 实现用户角色数据访问接口。
    /// </summary>
    public class UserInRoleDataEngine
        : DatabaseDataEngine<UserInRole, UserInRoleKey>
        , IUserInRoleDataEngine
    {
        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="sf">The sf.</param>
        public UserInRoleDataEngine(ISessionFactory sf)
            : base(sf)
        {
        }

        /// <summary>
        /// 根据用户登录名删除对应角色。
        /// </summary>
        /// <param name="loginName">登录名。</param>
        public void DeleteByLoginName(string loginName)
        {
            using (ISession session = this.SessionFactory.OpenSession())
            {
                IQuery qry = session.CreateQuery(@"DELETE FROM UserInRole 
                                                    WHERE Key.LoginName=:loginName")
                                  .SetString("loginName", loginName);
                qry.ExecuteUpdate();
            }
        }

        /// <summary>
        /// 根据用户角色名删除对应用户。
        /// </summary>
        /// <param name="roleName">角色名。</param>
        public void DeleteByRoleName(string roleName)
        {
            using (ISession session = this.SessionFactory.OpenSession())
            {
                IQuery qry = session.CreateQuery(@"DELETE FROM UserInRole 
                                                    WHERE Key.RoleName=:roleName")
                                  .SetString("roleName", roleName);
                qry.ExecuteUpdate();
            }
        }
    }
}
