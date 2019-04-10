// ***********************************************************************
// Assembly         : ServiceCenter.MES.DataAccess.ZPVM
// Author           : zijing.wu
// Created          : 10-11-2016
//
// Last Modified By : zijing.wu
// Last Modified On : 10-11-2016
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
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.Model;
using ServiceCenter.MES.DataAccess.FMM;
using NHibernate;
using ServiceCenter.MES.DataAccess.Interface.FMM;
using ServiceCenter.Common.DataAccess.NHibernate;

/// <summary>
/// The FMM namespace.
/// </summary>
namespace ServiceCenter.MES.DataAccess.FMM
{
    /// <summary>
    /// 实现规则-控制参数对象设置数据访问类。
    /// </summary>
    public class EquipmentControlObjectDataEngine
        : DatabaseDataEngine<EquipmentControlObject, EquipmentControlObjectKey>
        , IEquipmentControlObjectDataEngine
    {
        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="sf">The sf.</param>
        public EquipmentControlObjectDataEngine(ISessionFactory sf)
            : base(sf)
        {
        }
    }
}
