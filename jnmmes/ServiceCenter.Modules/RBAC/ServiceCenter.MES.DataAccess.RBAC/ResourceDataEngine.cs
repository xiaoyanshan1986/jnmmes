// ***********************************************************************
// Assembly         : ServiceCenter.MES.DataAccess.RBAC
// Author           : peter
// Created          : 07-25-2014
//
// Last Modified By : peter
// Last Modified On : 07-30-2014
// ***********************************************************************
// <copyright file="ResourceDataEngine.cs" company="">
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
using ServiceCenter.MES.DataAccess.Interface.RBAC;
using ServiceCenter.MES.Model.RBAC;
using NHibernate;
using ServiceCenter.DataAccess;
using ServiceCenter.Common.DataAccess.NHibernate;

/// <summary>
/// The RBAC namespace.
/// </summary>
namespace ServiceCenter.MES.DataAccess.RBAC
{
    /// <summary>
    /// 实现功能资源数据访问接口。
    /// </summary>
    public class ResourceDataEngine
        : DatabaseDataEngine<Resource, ResourceKey>
        , IResourceDataEngine
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceDataEngine" /> class.
        /// </summary>
        /// <param name="sf">The sf.</param>
        public ResourceDataEngine(ISessionFactory sf):base(sf)
        {
        }
    }
}
