// ***********************************************************************
// Assembly         : ServiceCenter.MES.DataAccess.WIP
// Author           : peter
// Created          : 07-25-2014
//
// Last Modified By : peter
// Last Modified On : 07-29-2014
// ***********************************************************************
// <copyright file="LotDataEngine.cs" company="">
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
    /// 在制品批次数据访问类。
    /// </summary>
    public class LotDataEngine
        : DatabaseDataEngine<Lot, string>
        , ILotDataEngine
    {
        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="sf">The sf.</param>
        public LotDataEngine(ISessionFactory sf):base(sf)
        {
        }
    }
}
