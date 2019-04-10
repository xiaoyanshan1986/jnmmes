// ***********************************************************************
// Assembly         : ServiceCenter.MES.DataAccess.Interface.RBAC
// Author           : peter
// Created          : 07-25-2014
//
// Last Modified By : peter
// Last Modified On : 07-30-2014
// ***********************************************************************
// <copyright file="IResourceDataEngine.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.Model.RBAC;
using ServiceCenter.DataAccess;

/// <summary>
/// The RBAC namespace.
/// </summary>
namespace ServiceCenter.MES.DataAccess.Interface.RBAC
{
    /// <summary>
    /// 定义功能资源数据访问接口。
    /// </summary>
    public interface IResourceDataEngine
        :  IDatabaseDataEngine<Resource, ResourceKey>
    {

    }
}
