// ***********************************************************************
// Assembly         : ServiceCenter.MES.Model.RBAC
// Author           : peter
// Created          : 07-25-2014
//
// Last Modified By : peter
// Last Modified On : 07-30-2014
// ***********************************************************************
// <copyright file="User.cs" company="">
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
    /// 描述用户数据的模型类。
    /// </summary>
    [DataContract]
    public class User : BaseModel<string>
    {
        /// <summary>
        /// 主键（用户登录名）。
        /// </summary>
        /// <value>The key.</value>
        [DataMember]
        public override string Key
        {
            get
            {

                return base.Key;
            }
            set
            {
                base.Key = value;
            }
        }
        /// <summary>
        /// 用户名。
        /// </summary>
        /// <value>The name of the user.</value>
        [DataMember]
        public virtual string UserName { get; set; }
        /// <summary>
        /// 登陆密码。
        /// </summary>
        /// <value>The password.</value>
        [DataMember]
        public virtual string Password { get; set; }
        /// <summary>
        /// 邮件。
        /// </summary>
        /// <value>The email.</value>
        [DataMember]
        public virtual string Email { get; set; }
        /// <summary>
        /// 办公电话。
        /// </summary>
        /// <value>The office phone.</value>
        [DataMember]
        public virtual string OfficePhone { get; set; }
        /// <summary>
        /// 移动电话。
        /// </summary>
        /// <value>The mobile phone.</value>
        [DataMember]
        public virtual string MobilePhone { get; set; }
        /// <summary>
        /// 是否锁定。
        /// </summary>
        /// <value><c>true</c> if this instance is locked; otherwise, <c>false</c>.</value>
        [DataMember]
        public virtual bool IsLocked { get; set; }
        /// <summary>
        /// 是否激活。
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        [DataMember]
        public virtual bool IsActive { get; set; }
        /// <summary>
        /// 是否审核通过。
        /// </summary>
        /// <value><c>true</c> if this instance is approved; otherwise, <c>false</c>.</value>
        [DataMember]
        public virtual bool IsApproved { get; set; }
        /// <summary>
        /// 最后一次登陆IP地址。
        /// </summary>
        /// <value>The last login ip.</value>
        [DataMember]
        public virtual string LastLoginIP { get; set; }
        /// <summary>
        /// 最后一次登陆时间。
        /// </summary>
        /// <value>The last login time.</value>
        [DataMember]
        public virtual DateTime? LastLoginTime { get; set; }
        /// <summary>
        /// 备注。
        /// </summary>
        /// <value>The description.</value>
        [DataMember]
        public virtual string Description { get; set; }
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
        /// <summary>
        /// 创建人。
        /// </summary>
        /// <value>The creator.</value>
        [DataMember]
        public virtual string Creator { get; set; }
        /// <summary>
        /// 创建时间。
        /// </summary>
        /// <value>The create time.</value>
        [DataMember]
        public virtual DateTime? CreateTime { get; set; }
    }
}
