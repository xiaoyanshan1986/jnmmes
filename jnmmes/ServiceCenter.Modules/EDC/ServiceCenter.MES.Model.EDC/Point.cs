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
    /// 采集触发动作。
    /// </summary>
    public enum EnumEDCAction
    {
        /// <summary>
        /// 手工采集。
        /// </summary>
        [Display(Name = "EnumEDCAction_None", ResourceType = typeof(StringResource))]
        None = 0,
        /// <summary>
        /// 进站后采集。
        /// </summary>
        [Display(Name = "EnumEDCAction_TrackIn", ResourceType = typeof(StringResource))]
        TrackIn = 1,
    }

    /// <summary>
    /// 描述采集点设置的模型类。
    /// </summary>
    [DataContract]
    public class Point : BaseModel<string>
    {
        /// <summary>
        /// 采集设置组名称。
        /// </summary>
        [DataMember]
        public virtual string GroupName { get; set; }
        /// <summary>
        /// 物料类型。
        /// </summary>
        [DataMember]
        public virtual string MaterialType { get; set; }
        /// <summary>
        /// 物料编码。
        /// </summary>
        [DataMember]
        public virtual string MaterialCode { get; set; }
        /// <summary>
        /// 工艺流程组名称。
        /// </summary>
        [DataMember]
        public virtual string RouteEnterpriseName { get; set; }
        /// <summary>
        /// 工艺流程名称。
        /// </summary>
        [DataMember]
        public virtual string RouteName { get; set; }
        /// <summary>
        /// 工步名称。
        /// </summary>
        [DataMember]
        public virtual string RouteStepName { get; set; }
        /// <summary>
        /// 工序名称。
        /// </summary>
        [DataMember]
        public virtual string RouteOperationName { get; set; }
        /// <summary>
        /// 生产线代码。
        /// </summary>
        [DataMember]
        public virtual string ProductionLineCode { get; set; }
        /// <summary>
        /// 设备代码。
        /// </summary>
        [DataMember]
        public virtual string EquipmentCode { get; set; }
        /// <summary>
        /// 触发采集的动作。
        /// </summary>
        [DataMember]
        public virtual EnumEDCAction ActionName { get; set; }
        /// <summary>
        /// 采集参数组名称。
        /// </summary>
        [DataMember]
        public virtual string CategoryName { get; set; }
        /// <summary>
        /// 采集计划名称。
        /// </summary>
        [DataMember]
        public virtual string SamplingPlanName { get; set; }
        /// <summary>
        /// 状态 1：可用 0：不可用。
        /// </summary>
        [DataMember]
        public virtual EnumObjectStatus Status { get; set; }
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
