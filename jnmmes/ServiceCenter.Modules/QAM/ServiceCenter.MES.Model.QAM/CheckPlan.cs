using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;
using System.ComponentModel.DataAnnotations;
using ServiceCenter.MES.Model.QAM.Resources;

namespace ServiceCenter.MES.Model.QAM
{   
    /// <summary>
    /// 检验计划类型。
    /// </summary>
    public enum EnumCheckType
    {
        /// <summary>
        /// 正常检验。
        /// </summary>
        [Display(Name = "EnumCheckType_Normal", ResourceType = typeof(StringResource))]
        Normal=0
    }
    /// <summary>
    /// 检验计划策略。
    /// </summary>
    public enum EnumCheckMode
    {
        /// <summary>
        /// 每批次检验。
        /// </summary>
        [Display(Name = "EnumCheckMode_Lot", ResourceType = typeof(StringResource))]
        Lot = 0,
        /// <summary>
        /// 按数量检验。
        /// </summary>
        [Display(Name = "EnumCheckMode_Qty", ResourceType = typeof(StringResource))]
        Qty = 1,
        /// <summary>
        /// 按分钟间隔检验。
        /// </summary>
        [Display(Name = "EnumCheckMode_Interval", ResourceType = typeof(StringResource))]
        Interval = 2,
        /// <summary>
        /// 按批次数量检验。
        /// </summary>
        [Display(Name = "EnumCheckMode_LotQty", ResourceType = typeof(StringResource))]
        LotQty = 3
    }
    /// <summary>
    /// 描述检验计划的模型类。
    /// </summary>
    [DataContract]
    public class CheckPlan : BaseModel<string>
    {
        /// <summary>
        /// 主键（检验计划名称）。
        /// </summary>
        [DataMember]
        public override string Key
        {
            get;
            set;
        }
        /// <summary>
        ///检验计划类型（0：普通检验）
        /// </summary>
        [DataMember]
        public virtual EnumCheckType Type { get; set; }
        /// <summary>
        /// 检验计划策略。
        /// </summary>
        [DataMember]
        public virtual EnumCheckMode Mode { get; set; }
        /// <summary>
        /// 检验计划大小
        /// </summary>
        [DataMember]
        public virtual double? Size { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [DataMember]
        public virtual string Description { get; set; }
        /// <summary>
        /// 状态 1:可用， 0：不可用。
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
