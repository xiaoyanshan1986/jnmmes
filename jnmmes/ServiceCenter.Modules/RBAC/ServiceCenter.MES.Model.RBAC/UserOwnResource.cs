// ***********************************************************************
// Assembly         : ServiceCenter.MES.Model.RBAC
// Author           : peter
// Created          : 07-29-2014
//
// Last Modified By : peter
// Last Modified On : 07-30-2014
// ***********************************************************************
// <copyright file="UserOwnResource.cs" company="">
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
    /// 表示用户资源数据的主键。
    /// </summary>
    public struct UserOwnResourceKey
    {
        /// <summary>
        /// 用户登录名。
        /// </summary>
        /// <value>The name of the login.</value>
        public string LoginName { get; set; }
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
    /// 描述用户拥有的资源。
    /// </summary>
    [DataContract]
    public class UserOwnResource : BaseModel<UserOwnResourceKey>
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
