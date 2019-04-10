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
    /// 原因代码类型。
    /// </summary>
    public enum EnumEquipmentReasonCodeType
    {
        /// <summary>
        /// 设备故障
        /// </summary>
        [Display(Name = "EnumEquipmentReasonCodeType_Failure", ResourceType = typeof(StringResource))]
        Failure = 0,
        /// <summary>
        /// 设备维护
        /// </summary>
        [Display(Name = "EnumEquipmentReasonCodeType_Maintenance", ResourceType = typeof(StringResource))]
        Maintenance = 1,
        /// <summary>
        /// 工艺调试
        /// </summary>
        [Display(Name = "EnumEquipmentReasonCodeType_Debug", ResourceType = typeof(StringResource))]
        Debug = 2,
        /// <summary>
        /// 其他。
        /// </summary>
        [Display(Name = "EnumEquipmentReasonCodeType_Other", ResourceType = typeof(StringResource))]
        Other = 9
    }
    /// <summary>
    /// 原因代码数据模型。
    /// </summary>
    [DataContract]
    public class EquipmentReasonCode : BaseModel<string>
    {
        /// <summary>
        /// 主键（原因代码名称）。
        /// </summary>
        [DataMember]
        public override string Key
        {
            get;
            set;
        }
        /// <summary>
        /// 类型。
        /// </summary>
        [DataMember]
        public virtual EnumEquipmentReasonCodeType Type { get; set; }
        /// <summary>
        /// 分类名称。
        /// </summary>
        [DataMember]
        public virtual string Class { get; set; }
        /// <summary>
        /// 描述。
        /// </summary>
        [DataMember]
        public virtual string Description { get; set; }
        /// <summary>
        /// 状态。
        /// </summary>
        [DataMember]
        public virtual EnumObjectStatus Status { get; set; }
        /// <summary>
        /// 创建人。
        /// </summary>
        [DataMember]
        public virtual string Creator { get; set; }
        /// <summary>
        /// 创建时间。
        /// </summary>
        [DataMember]
        public virtual DateTime CreateTime { get; set; }
        /// <summary>
        /// 编辑人。
        /// </summary>
        [DataMember]
        public virtual string Editor { get; set; }
        /// <summary>
        /// 编辑时间。
        /// </summary>
        [DataMember]
        public virtual DateTime EditTime { get; set; }

    }
}
