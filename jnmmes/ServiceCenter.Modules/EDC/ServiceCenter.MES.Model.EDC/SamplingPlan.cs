using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;
using System.ComponentModel.DataAnnotations;
using ServiceCenter.MES.Model.EDC.Resources;

namespace ServiceCenter.MES.Model.EDC
{   
    /// <summary>
    /// 采集计划类型。
    /// </summary>
    public enum EnumSamplingPlanType
    {
        /// <summary>
        /// 正常采集。
        /// </summary>
        [Display(Name = "EnumSamplingPlanType_Normal", ResourceType = typeof(StringResource))]
        Normal=0
    }
    /// <summary>
    /// 采集计划策略。
    /// </summary>
    public enum EnumSamplingPlanMode
    {
        /// <summary>
        /// 每批次采集。
        /// </summary>
        [Display(Name = "EnumSamplingPlanMode_Lot", ResourceType = typeof(StringResource))]
        Lot=0,
        /// <summary>
        /// 按数量采集。
        /// </summary>
        [Display(Name = "EnumSamplingPlanMode_Qty", ResourceType = typeof(StringResource))]
        Qty=1,
        /// <summary>
        /// 按分钟间隔采集。
        /// </summary>
        [Display(Name = "EnumSamplingPlanMode_Interval", ResourceType = typeof(StringResource))]
        Interval = 2,
        /// <summary>
        /// 按批次数量采集。
        /// </summary>
        [Display(Name = "EnumSamplingPlanMode_LotQty", ResourceType = typeof(StringResource))]
        LotQty=3
    }
    /// <summary>
    /// 描述采集计划的模型类。
    /// </summary>
    [DataContract]
    public class SamplingPlan : BaseModel<string>
    {
        /// <summary>
        /// 主键（采集计划名称）。
        /// </summary>
        [DataMember]
        public override string Key
        {
            get;
            set;
        }
        /// <summary>
        ///采集计划类型（0：普通采集）
        /// </summary>
        [DataMember]
        public virtual EnumSamplingPlanType Type { get; set; }
        /// <summary>
        /// 采集计划策略
        /// </summary>
        [DataMember]
        public virtual EnumSamplingPlanMode Mode { get; set; }
        /// <summary>
        /// 采集计划大小
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
