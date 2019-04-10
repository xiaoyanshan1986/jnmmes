// ***********************************************************************
// Assembly         : ServiceCenter.MES.Model.RBAC
// Author           : peter
// Created          : 07-29-2014
//
// Last Modified By : peter
// Last Modified On : 07-29-2014
// ***********************************************************************
// <copyright file="UserInRole.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;

/// <summary>
/// The RBAC namespace.
/// </summary>
namespace ServiceCenter.MES.Model.RBAC
{
    /// <summary>
    /// 表示用户角色数据的主键。
    /// </summary>
    public struct UserInRoleKey
    {
        /// <summary>
        /// 资源类型。
        /// </summary>
        /// <value>The name of the login.</value>
        public string LoginName { get; set; }
        /// <summary>
        /// 资源代码。
        /// </summary>
        /// <value>The name of the role.</value>
        public string RoleName { get; set; }
    }
    /// <summary>
    /// 表示用户所属角色。
    /// </summary>
    [DataContract]
    public class UserInRole : BaseModel<UserInRoleKey>
    {
        /// <summary>
        /// 编辑人。
        /// </summary>
        /// <value>The editor.</value>
        [DataMember]
        public virtual string Editor { get; set; }
        /// <summary>
        /// 编辑时间。
        /// </summary>
        /// <value>The edit time.</value>
        [DataMember]
        public virtual DateTime? EditTime { get; set; }
    }
}
