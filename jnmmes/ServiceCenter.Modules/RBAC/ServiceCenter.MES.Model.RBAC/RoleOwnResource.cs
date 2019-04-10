// ***********************************************************************
// Assembly         : ServiceCenter.MES.Model.RBAC
// Author           : peter
// Created          : 07-29-2014
//
// Last Modified By : peter
// Last Modified On : 07-30-2014
// ***********************************************************************
// <copyright file="RoleOwnResource.cs" company="">
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
    /// 表示角色资源数据的主键。
    /// </summary>
    public struct RoleOwnResourceKey
    {
        /// <summary>
        /// 角色名称。
        /// </summary>
        /// <value>The name of the role.</value>
        public string RoleName { get; set; }
        /// <summary>
        /// 资源类型。
        /// </summary>
        /// <value>The type of the resource.</value>
        public ResourceType ResourceType { get; set; }
        /// <summary>
        /// 资源代码。
        /// </summary>
        /// <value>The resource code.</value>
        public string ResourceCode { get; set; }
    }
    /// <summary>
    /// 描述角色拥有的资源。
    /// </summary>
    [DataContract]
    public class RoleOwnResource : BaseModel<RoleOwnResourceKey>
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
