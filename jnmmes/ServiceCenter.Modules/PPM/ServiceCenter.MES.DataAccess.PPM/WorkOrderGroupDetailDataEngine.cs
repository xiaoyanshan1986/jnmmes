// ***********************************************************************
// Assembly         : ServiceCenter.MES.DataAccess.PPM
// Author           : junhai
// Created          : 11-06-2017
//
// Last Modified By : junhai
// Last Modified On : 11-06-2017
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
using ServiceCenter.MES.Model.PPM;

using ServiceCenter.Model;
using ServiceCenter.MES.DataAccess.PPM;
using NHibernate;
using ServiceCenter.MES.DataAccess.Interface.PPM;
using ServiceCenter.Common.DataAccess.NHibernate;

/// <summary>
/// The PPM namespace.
/// </summary>
namespace ServiceCenter.MES.DataAccess.PPM
{
    /// <summary>
    /// 实现混工单组规则数据访问类。
    /// </summary>
    public class WorkOrderGroupDetailDataEngine
        : DatabaseDataEngine<WorkOrderGroupDetail, WorkOrderGroupDetailKey>
        , IWorkOrderGroupDetailDataEngine
    {
        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="sf">The sf.</param>
        public WorkOrderGroupDetailDataEngine(ISessionFactory sf)
            : base(sf)
        {
        }
    }
}
