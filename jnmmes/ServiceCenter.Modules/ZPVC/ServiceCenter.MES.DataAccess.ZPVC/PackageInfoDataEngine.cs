// ***********************************************************************
// Assembly         : ServiceCenter.MES.DataAccess.ZPVC
// Author           : Peter
// Created          : 09-05-2014
//
// Last Modified By : Peter
// Last Modified On : 09-05-2014
// ***********************************************************************
// <copyright file="PackageInfoDataEngine.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.Model.ZPVC;

using ServiceCenter.Model;
using ServiceCenter.MES.DataAccess.ZPVC;
using NHibernate;
using ServiceCenter.MES.DataAccess.Interface.ZPVC;
using ServiceCenter.Common.DataAccess.NHibernate;

/// <summary>
/// The ZPVC namespace.
/// </summary>
namespace ServiceCenter.MES.DataAccess.ZPVC
{
    /// <summary>
    /// 实现包装信息数据访问类。
    /// </summary>
    public class PackageInfoDataEngine
        : DatabaseDataEngine<PackageInfo, string>
        , IPackageInfoDataEngine
    {
        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="sf">The sf.</param>
        public PackageInfoDataEngine(ISessionFactory sf)
            : base(sf)
        {
        }
    }
}
