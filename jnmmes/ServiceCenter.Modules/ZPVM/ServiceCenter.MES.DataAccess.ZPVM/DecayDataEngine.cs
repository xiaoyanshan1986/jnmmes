// ***********************************************************************
// Assembly         : ServiceCenter.MES.DataAccess.ZPVM
// Author           : Peter
// Created          : 09-05-2014
//
// Last Modified By : Peter
// Last Modified On : 09-05-2014
// ***********************************************************************
// <copyright file="DecayDataEngine.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.Model.ZPVM;

using ServiceCenter.Model;
using ServiceCenter.MES.DataAccess.ZPVM;
using NHibernate;
using ServiceCenter.MES.DataAccess.Interface.ZPVM;
using ServiceCenter.Common.DataAccess.NHibernate;

/// <summary>
/// The ZPVM namespace.
/// </summary>
namespace ServiceCenter.MES.DataAccess.ZPVM
{
    /// <summary>
    /// 实现衰减数据访问类。
    /// </summary>
    public class DecayDataEngine
        : DatabaseDataEngine<Decay, DecayKey>
        , IDecayDataEngine
    {
        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="sf">The sf.</param>
        public DecayDataEngine(ISessionFactory sf)
            : base(sf)
        {
        }
    }
}
