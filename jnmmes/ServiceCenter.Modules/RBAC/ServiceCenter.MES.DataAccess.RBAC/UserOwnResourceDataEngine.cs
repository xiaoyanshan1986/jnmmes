// ***********************************************************************
// Assembly         : ServiceCenter.MES.DataAccess.RBAC
// Author           : peter
// Created          : 07-30-2014
//
// Last Modified By : peter
// Last Modified On : 07-30-2014
// ***********************************************************************
// <copyright file="UserOwnResourceDataEngine.cs" company="">
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
    /// 实现用户资源数据访问接口。
    /// </summary>
    public class UserOwnResourceDataEngine
        : DatabaseDataEngine<UserOwnResource, UserOwnResourceKey>
        , IUserOwnResourceDataEngine
    {
        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="sf">The sf.</param>
        public UserOwnResourceDataEngine(ISessionFactory sf)
            : base(sf)
        {
        }

        /// <summary>
        /// 根据用户登录名删除对应用户资源。
        /// </summary>
        /// <param name="loginName">登录名。</param>
        public void DeleteByLoginName(string loginName)
        {
            using (ISession session = this.SessionFactory.OpenSession())
            {
                IQuery qry = session.CreateQuery(@"DELETE FROM UserOwnResource 
                                                    WHERE Key.LoginName=:loginName")
                                    .SetString("loginName", loginName);
                qry.ExecuteUpdate();
            }
        }
        /// <summary>
        /// 根据资源删除对应用户资源数据。
        /// </summary>
        /// <param name="resourceKey">资源主键。</param>
        public void DeleteByResourceKey(ResourceKey resourceKey)
        {
            using (ISession session = this.SessionFactory.OpenSession())
            {
                IQuery qry = session.CreateQuery(@"DELETE FROM UserOwnResource 
                                                    WHERE Key.ResourceType=:type
                                                    AND Key.ResourceCode=:code")
                                    .SetParameter<ResourceType>("type", resourceKey.Type)
                                    .SetString("code",resourceKey.Code);
                qry.ExecuteUpdate();
            }
        }


    }
}
