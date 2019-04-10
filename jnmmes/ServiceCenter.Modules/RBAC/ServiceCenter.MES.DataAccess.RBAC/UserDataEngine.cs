// ***********************************************************************
// Assembly         : ServiceCenter.MES.DataAccess.RBAC
// Author           : peter
// Created          : 07-25-2014
//
// Last Modified By : peter
// Last Modified On : 07-30-2014
// ***********************************************************************
// <copyright file="UserDataEngine.cs" company="">
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
    /// 实现用户数据访问接口。
    /// </summary>
    public class UserDataEngine
         : DatabaseDataEngine<User, string>
         ,IUserDataEngine
    {
        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="sf">The sf.</param>
        public UserDataEngine(ISessionFactory sf):base(sf)
        {
        }

    }

}
