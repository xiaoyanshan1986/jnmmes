// ***********************************************************************
// Assembly         : ServiceCenter.MES.Model.RBAC
// Author           : peter
// Created          : 07-25-2014
//
// Last Modified By : peter
// Last Modified On : 07-30-2014
// ***********************************************************************
// <copyright file="Role.cs" company="">
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
    /// 描述角色数据的模型类。
    /// </summary>
    [DataContract]
    public class Role : BaseModel<string>
    {
        /// <summary>
        /// 主键（角色名称）。
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
        /// 描述。
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
