// ***********************************************************************
// Assembly         : ServiceCenter.MES.DataAccess.ZPVM
// Author           : Peter
// Created          : 09-05-2014
//
// Last Modified By : Peter
// Last Modified On : 09-05-2014
// ***********************************************************************
// <copyright file="EfficiencyDataEngine.cs" company="">
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
    /// 实现效率档数据访问类。
    /// </summary>
    public class EfficiencyDataEngine
        : DatabaseDataEngine<Efficiency, EfficiencyKey>
        , IEfficiencyDataEngine
    {
        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="sf">The sf.</param>
        public EfficiencyDataEngine(ISessionFactory sf)
            : base(sf)
        {
        }
    }
}
