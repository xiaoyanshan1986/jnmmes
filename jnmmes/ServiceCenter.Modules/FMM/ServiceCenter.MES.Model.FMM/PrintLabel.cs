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
    public enum EnumPrintLabelType
    {
        /// <summary>
        /// 批次标签
        /// </summary>
        [Display(Name = "EnumPrintLabelType_Lot", ResourceType = typeof(StringResource))]
        Lot =0,
        /// <summary>
        /// 包装标签
        /// </summary>
        [Display(Name = "EnumPrintLabelType_Package", ResourceType = typeof(StringResource))]
        Package = 1,
        /// <summary>
        /// 箱标签
        /// </summary>
        [Display(Name = "EnumPrintLabelType_Box", ResourceType = typeof(StringResource))]
        Box = 2,
        /// <summary>
        /// 铭牌
        /// </summary>
        [Display(Name = "EnumPrintLabelType_Nameplate", ResourceType = typeof(StringResource))]
        Nameplate=10
    }
    /// <summary>
    /// 打印标签数据。
    /// </summary>
    [DataContract]
    public class PrintLabel : BaseModel<string>
    {
        public PrintLabel()
        {
        }
        /// <summary>
        /// 主键（标签代码）。
        /// </summary>
        [DataMember]
        public override string Key
        {
            get;
            set;
        }
        /// <summary>
        /// 标签名称。
        /// </summary>
        [DataMember]
        public virtual string Name { get; set; }

        /// <summary>
        /// 标签类型。
        /// </summary>
        [DataMember]
        public virtual EnumPrintLabelType Type { get; set; }
        /// <summary>
        /// 标签内容。
        /// </summary>
        [DataMember]
        public virtual string Content { get; set; }
        /// <summary>
        /// 是否可用。
        /// </summary>
        [DataMember]
        public virtual bool IsUsed { get; set; }
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
