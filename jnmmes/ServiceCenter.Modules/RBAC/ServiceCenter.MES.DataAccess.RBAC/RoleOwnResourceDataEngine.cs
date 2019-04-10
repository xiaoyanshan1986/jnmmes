// ***********************************************************************
// Assembly         : ServiceCenter.MES.DataAccess.RBAC
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 07-30-2014
// ***********************************************************************
// <copyright file="RoleOwnResourceDataEngine.cs" company="">
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
    /// 实现角色资源数据访问接口。
    /// </summary>
    public class RoleOwnResourceDataEngine
        : DatabaseDataEngine<RoleOwnResource, RoleOwnResourceKey>
        , IRoleOwnResourceDataEngine
    {
        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="sf">The sf.</param>
        public RoleOwnResourceDataEngine(ISessionFactory sf)
            : base(sf)
        {
        }

        public void DeleteByRoleName(string roleName)
        {
            using (ISession session = this.SessionFactory.OpenSession())
            {
                IQuery qry = session.CreateQuery(@"DELETE FROM RoleOwnResource 
                                                    WHERE Key.RoleName=:roleName")
                                    .SetString("roleName", roleName);
                qry.ExecuteUpdate();
            }
        }

        public void DeleteByResourceKey(ResourceKey resourceKey)
        {
            using (ISession session = this.SessionFactory.OpenSession())
            {
                IQuery qry = session.CreateQuery(@"DELETE FROM RoleOwnResource 
                                                    WHERE Key.ResourceType=:type
                                                    AND Key.ResourceCode=:code")
                                    .SetParameter<ResourceType>("type", resourceKey.Type)
                                    .SetString("code", resourceKey.Code);
                qry.ExecuteUpdate();
            }
        }
    }
}
