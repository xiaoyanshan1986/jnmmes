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
    public enum EnumReasonCodeType
    {
        /// <summary>
        /// 不良。
        /// </summary>
        [Display(Name = "EnumReasonCodeType_Defect", ResourceType = typeof(StringResource))]
        Defect=0,
        /// <summary>
        /// 报废。
        /// </summary>
        [Display(Name = "EnumReasonCodeType_Scrap", ResourceType = typeof(StringResource))]
        Scrap = 1,
        /// <summary>
        /// 暂停。
        /// </summary>
        [Display(Name = "EnumReasonCodeType_Hold", ResourceType = typeof(StringResource))]
        Hold=2,
        /// <summary>
        /// 结束。
        /// </summary>
        [Display(Name = "EnumReasonCodeType_Terminal", ResourceType = typeof(StringResource))]
        Terminal=3,
        /// <summary>
        /// 补料。
        /// </summary>
        [Display(Name = "EnumReasonCodeType_Patch", ResourceType = typeof(StringResource))]
        Patch = 4,
        /// <summary>
        /// 超时。
        /// </summary>
        [Display(Name = "EnumReasonCodeType_Timeout", ResourceType = typeof(StringResource))]
        Timeout=5,
        /// <summary>
        /// 返修。
        /// </summary>
        [Display(Name = "EnumReasonCodeType_Repair", ResourceType = typeof(StringResource))]
        Repair = 6,
        /// <summary>
        /// 其他。
        /// </summary>
        [Display(Name = "EnumReasonCodeType_Other", ResourceType = typeof(StringResource))]
        Other=9
    }
    /// <summary>
    /// 原因代码数据模型。
    /// </summary>
    [DataContract]
    public class ReasonCode : BaseModel<string>
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
        public virtual EnumReasonCodeType Type { get; set; }
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
