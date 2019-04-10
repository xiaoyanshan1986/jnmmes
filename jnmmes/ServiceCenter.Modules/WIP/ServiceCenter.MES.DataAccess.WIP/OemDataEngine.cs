// ***********************************************************************
// Assembly         : ServiceCenter.MES.DataAccess.WIP
// Author           : junhai
// Created          : 07-13-2018
//
// Last Modified By : junhai
// Last Modified On : 07-13-2018
// ***********************************************************************
// <copyright file="OemDataEngine.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.Model.WIP;

using ServiceCenter.Model;
using ServiceCenter.MES.DataAccess.WIP;
using NHibernate;
using ServiceCenter.MES.DataAccess.Interface.WIP;
using ServiceCenter.Common.DataAccess.NHibernate;

/// <summary>
/// The WIP namespace.
/// </summary>
namespace ServiceCenter.MES.DataAccess.WIP
{
    /// <summary>
    /// Oem代加工数据访问类。
    /// </summary>
    public class OemDataEngine
        : DatabaseDataEngine<OemData, string>
        , IOemDataEngine
    {
        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="sf">The sf.</param>
        public OemDataEngine(ISessionFactory sf):base(sf)
        {
        }
    }
}
