// ***********************************************************************
// Assembly         : ServiceCenter.MES.Model.RBAC
// Author           : peter
// Created          : 07-25-2014
//
// Last Modified By : peter
// Last Modified On : 07-30-2014
// ***********************************************************************
// <copyright file="Resource.cs" company="">
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
using System.ComponentModel.DataAnnotations;
using ServiceCenter.MES.Model.RBAC.Resources;

/// <summary>
/// The RBAC namespace.
/// </summary>
namespace ServiceCenter.MES.Model.RBAC
{
    /// <summary>
    /// 资源类型。
    /// </summary>
    public enum ResourceType
    {
        /// <summary>
        /// 导航菜单。
        /// </summary>
        [Display(Name = "ResourceType_Menu", ResourceType = typeof(StringResource))]
        Menu = 10,
        /// <summary>
        /// 导航菜单项。
        /// </summary>
        [Display(Name = "ResourceType_MenuItem", ResourceType = typeof(StringResource))]
        MenuItem = 11,
        /// <summary>
        /// 功能
        /// </summary>
        [Display(Name = "ResourceType_MenuItemFunction", ResourceType = typeof(StringResource))]
        MenuItemFunction=12,
        /// <summary>
        /// 生产线。
        /// </summary>
        [Display(Name = "ResourceType_ProductionLine", ResourceType = typeof(StringResource))]
        ProductionLine=20,
        /// <summary>
        /// 工序。
        /// </summary>
        [Display(Name = "ResourceType_RouteOperation", ResourceType = typeof(StringResource))]
        RouteOperation=30,
        /// <summary>
        /// 线边仓。
        /// </summary>
        [Display(Name = "ResourceType_LineStore", ResourceType = typeof(StringResource))]
        LineStore=40,
        /// <summary>
        /// 线边仓。
        /// </summary>
        [Display(Name = "ResourceType_EquipmentState", ResourceType = typeof(StringResource))]
        EquipmentState=50
    }
    /// <summary>
    /// 表示资源数据主键。
    /// </summary>
    public struct ResourceKey
    {
        /// <summary>
        /// 资源类型。
        /// </summary>
        /// <value>The type.</value>
        public ResourceType Type { get; set; }
        /// <summary>
        /// 资源代码。
        /// </summary>
        /// <value>The code.</value>
        public string Code { get; set; }


    }
    /// <summary>
    /// 描述资源数据的模型类。
    /// </summary>
    [DataContract]
    public class Resource : BaseModel<ResourceKey>
    {
        /// <summary>
        /// 资源名。
        /// </summary>
        /// <value>The name.</value>
        [DataMember]
        public virtual string Name { get; set; }
        /// <summary>
        /// 资源数据。
        /// </summary>
        /// <value>The data.</value>
        [DataMember]
        public virtual string Data { get; set; }
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
