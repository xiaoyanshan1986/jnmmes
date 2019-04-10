// ***********************************************************************
// Assembly         : ServiceCenter.MES.DataAccess.Interface.RBAC
// Author           : peter
// Created          : 07-25-2014
//
// Last Modified By : peter
// Last Modified On : 07-30-2014
// ***********************************************************************
// <copyright file="IRoleDataEngine.cs" company="">
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
using ServiceCenter.Model;

/// <summary>
/// The RBAC namespace.
/// </summary>
namespace ServiceCenter.MES.DataAccess.Interface.RBAC
{
    /// <summary>
    /// 定义角色数据访问接口。
    /// </summary>
    public interface IRoleDataEngine : IDatabaseDataEngine<Role, string>
    {
    }
}
