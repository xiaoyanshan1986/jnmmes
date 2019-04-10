using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;
using System.ComponentModel.DataAnnotations;
using ServiceCenter.MES.Model.WIP.Resources;

namespace ServiceCenter.MES.Model.WIP
{
    /// <summary>
    /// 作业类型。
    /// </summary>
    public enum EnumJobType
    {
        /// <summary>
        /// 自动进站。
        /// </summary>
        [Display(Name = "EnumJobType_AutoTrackIn", ResourceType = typeof(StringResource))]
        AutoTrackIn=0,
        /// <summary>
        /// 自动出站。
        /// </summary>
        [Display(Name = "EnumJobType_AutoTrackOut", ResourceType = typeof(StringResource))]
        AutoTrackOut=1
    }

    /// <summary>
    /// 作业关闭类型。
    /// </summary>
    public enum EnumCloseType
    {
        /// <summary>
        /// 未关闭。
        /// </summary>
        [Display(Name = "EnumCloseType_None", ResourceType = typeof(StringResource))]
        None = 0,
        /// <summary>
        /// 正常关闭。
        /// </summary>
        [Display(Name = "EnumCloseType_Normal", ResourceType = typeof(StringResource))]
        Normal = 1,
        /// <summary>
        /// 手动关闭。
        /// </summary>
        [Display(Name = "EnumCloseType_Manual", ResourceType = typeof(StringResource))]
        Manual=2
    }

    /// <summary>
    /// 描述批次定时作业数据的模型类。
    /// </summary>
    [DataContract]
    public class LotJob : BaseModel<string>
    {

        /// <summary>
        /// 作业名称。
        /// </summary>
        [DataMember]
        public virtual string JobName { get; set; }
        /// <summary>
        /// 批次号。
        /// </summary>
        [DataMember]
        public virtual string LotNumber { get; set; }
        /// <summary>
        /// 生产线代码。
        /// </summary>
        [DataMember]
        public virtual string LineCode { get; set; }
        /// <summary>
        /// 设备代码。
        /// </summary>
        [DataMember]
        public virtual string EquipmentCode { get; set; }
        /// <summary>
        /// 工艺流程组名称。
        /// </summary>
        [DataMember]
        public virtual string RouteEnterpriseName{ get; set; }
        /// <summary>
        /// 工艺流程名称。
        /// </summary>
        [DataMember]
        public virtual string RouteName{ get; set; }
        /// <summary>
        /// 工艺流程工步名称。
        /// </summary>
        [DataMember]
        public virtual string RouteStepName { get; set; }
        /// <summary>
        /// 作业类型。
        /// </summary>
        [DataMember]
        public virtual EnumJobType Type { get; set; }
        /// <summary>
        /// 作业状态。
        /// </summary>
        [DataMember]
        public virtual EnumObjectStatus Status { get; set; }
        /// <summary>
        /// 作业关闭类型。
        /// </summary>
        [DataMember]
        public virtual EnumCloseType CloseType { get; set; }
        /// <summary>
        /// 作业运行次数。
        /// </summary>
        [DataMember]
        public virtual int RunCount { get; set; }
        /// <summary>
        /// 下次运行时间。
        /// </summary>
        [DataMember]
        public virtual DateTime NextRunTime { get; set; }
        /// <summary>
        /// 需要通知的用户。
        /// </summary>
        [DataMember]
        public virtual string NotifyUser { get; set; }
        /// <summary>
        /// 通知消息。
        /// </summary>
        [DataMember]
        public virtual string NotifyMessage { get; set; }
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
        public virtual DateTime? EditTime { get; set; }
    }
}
