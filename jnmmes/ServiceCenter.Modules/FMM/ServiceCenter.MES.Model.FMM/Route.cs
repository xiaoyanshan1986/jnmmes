using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;
using System.ComponentModel.DataAnnotations;
using ServiceCenter.MES.Model.FMM.Resources;

namespace ServiceCenter.MES.Model.FMM
{
    /// <summary>
    /// 工艺流程类型
    /// </summary>
    public enum EnumRouteType
    {
        /// <summary>
        /// 生产主流程
        /// </summary>
        [Display(Name = "RouteType_MainFlow", ResourceType = typeof(StringResource))]
        MainFlow = 0,

        /// <summary>
        /// 返修流程
        /// </summary>
        [Display(Name = "RouteType_Repair", ResourceType = typeof(StringResource))]
        Repair = 1
    }

    /// <summary>
    /// 表示工艺流程数据模型
    /// </summary>
    [DataContract]
    public class Route : BaseModel<string>
    {
        /// <summary>
        /// 主键（工艺流程名称）。
        /// </summary>
        [DataMember]
        public override string Key
        {
            get;
            set;
        }

        /// <summary>
        /// 状态。
        /// </summary>
        [DataMember]
        public virtual EnumObjectStatus Status { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        [DataMember]
        public virtual EnumRouteType RouteType { get; set; }

        /// <summary>
        /// 描述。
        /// </summary>
        [DataMember]
        public virtual string Description { get; set; }

        /// <summary>
        /// 创建人。
        /// </summary>
        [DataMember]
        public virtual string Creator { get; set; }

        /// <summary>
        /// 创建时间。
        /// </summary>
        [DataMember]
        public virtual DateTime? CreateTime { get; set; }

        /// <summary>
        /// 编辑人。
        /// </summary>
        [DataMember]
        public virtual string Editor { get; set; }

        /// <summary>
        /// 编辑时间。
        /// </summary>
        [DataMember]
        public virtual DateTime? EditTime { get; set; }
    }
}
