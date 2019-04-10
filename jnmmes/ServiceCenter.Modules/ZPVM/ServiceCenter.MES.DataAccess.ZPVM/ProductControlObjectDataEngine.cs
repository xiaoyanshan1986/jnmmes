// ***********************************************************************
// Assembly         : ServiceCenter.MES.DataAccess.ZPVM
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
    /// 实现产品-控制参数对象设置数据访问类。
    /// </summary>
    public class ProductControlObjectDataEngine
        : DatabaseDataEngine<ProductControlObject, ProductControlObjectKey>
        , IProductControlObjectDataEngine
    {
        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="sf">The sf.</param>
        public ProductControlObjectDataEngine(ISessionFactory sf)
            : base(sf)
        {
        }
    }
}
