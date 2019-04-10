using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;
using System.ComponentModel.DataAnnotations;
using ServiceCenter.MES.Model.PPM.Resources;

namespace ServiceCenter.MES.Model.PPM
{
    /// <summary> 工单优先级 </summary>
    public enum EnumWorkOrderPriority
    {
        /// <summary>
        /// 高
        /// </summary>
        [Display(Name = "EnumWorkOrderPriority_High", ResourceType = typeof(StringResource))]
        High=10,
        /// <summary>
        /// 正常
        /// </summary>
        [Display(Name = "EnumWorkOrderPriority_Normal", ResourceType = typeof(StringResource))]
        Normal=5,
        /// <summary>
        /// 低
        /// </summary>
        [Display(Name = "EnumWorkOrderPriority_Lower", ResourceType = typeof(StringResource))]
        Lower=1
    }

    /// <summary> 工单关闭类型 </summary>
    public enum EnumWorkOrderCloseType
    {
        /// <summary>
        /// 未关闭
        /// </summary>
        [Display(Name = "EnumWorkOrderCloseType_None", ResourceType = typeof(PPM.Resources.StringResource))]
        None = 0,

        /// <summary>
        /// 正常关闭
        /// </summary>
         [Display(Name = "EnumWorkOrderCloseType_Normal", ResourceType = typeof(PPM.Resources.StringResource))]
        Normal = 1,

        /// <summary>
        /// 手动关闭
        /// </summary>
         [Display(Name = "EnumWorkOrderCloseType_Manual", ResourceType = typeof(PPM.Resources.StringResource))]
        Manual = 2,

        /// <summary>
        /// 其他关闭
        /// </summary>
         [Display(Name = "EnumWorkOrderCloseType_Other", ResourceType = typeof(PPM.Resources.StringResource))]
        Other = 3
    }

    /// <summary> 工单创建类型 </summary>
    public enum EnumWorkOrderCreateType
    {
        /// <summary>
        /// MES系统创建
        /// </summary>
        [Display(Name = "EnumWorkOrderCreateType_SYS", ResourceType = typeof(PPM.Resources.StringResource))]
        MES = 0,
        /// <summary>
        /// ERP系统创建
        /// </summary>
        [Display(Name = "EnumWorkOrderCreateType_ERP", ResourceType = typeof(PPM.Resources.StringResource))]
        ERP = 1
    }

    /// <summary> 工单状态 </summary>
    public enum EnumWorkOrderState
    {
        /// <summary>
        /// 正常
        /// </summary>
        [Display(Name = "EnumWorkOrderState_Open", ResourceType = typeof(PPM.Resources.StringResource))]
        Open = 0,

        /// <summary>
        /// 关闭
        /// </summary>
        [Display(Name = "EnumWorkOrderState_Close", ResourceType = typeof(PPM.Resources.StringResource))]
        Close = 1,

        /// <summary>
        /// 暂停
        /// </summary>
        [Display(Name = "EnumWorkOrderState_Hold", ResourceType = typeof(PPM.Resources.StringResource))]
        Hold = 2,

        /// <summary>
        /// 删除
        /// </summary>
        [Display(Name = "EnumWorkOrderState_Delete", ResourceType = typeof(PPM.Resources.StringResource))]
        Delete = -1
    }

    /// <summary> 描述工单模型类 </summary>
    [DataContract]
    public class WorkOrder : BaseModel<string>
    {
        /// <summary> 主键（工单号） </summary>
        [DataMember]
        public override string Key
        {
            get;
            set;
        }

        /// <summary> 物料编码 </summary>
        [DataMember]
        public virtual string MaterialCode { get; set; }

        /// <summary> 工单状态 </summary>
        [DataMember]
        public virtual EnumWorkOrderState OrderState { get; set; }

        /// <summary>
        /// 工单类型。
        /// </summary>
        [DataMember]
        public virtual string OrderType { get; set; }

        /// <summary>
        /// 优先级。
        /// </summary>
        [DataMember]
        public virtual EnumWorkOrderPriority Priority { get; set; }

        /// <summary>
        /// 工单数量。
        /// </summary>
        [DataMember]
        public virtual double OrderQuantity { get; set; }

        /// <summary>
        /// 在制品数量。
        /// </summary>
        [DataMember]
        public virtual double WIPQuantity { get; set; }

        /// <summary>
        /// 报废数量。
        /// </summary>
        [DataMember]
        public virtual double ScrapQuantity { get; set; }

        /// <summary>
        /// 完成数量。
        /// </summary>
        [DataMember]
        public virtual double FinishQuantity { get; set; }

        /// <summary>
        /// 剩余数量。
        /// </summary>
        [DataMember]
        public virtual double LeftQuantity { get; set; }

        /// <summary>
        /// 返工数量。
        /// </summary>
        [DataMember]
        public virtual double ReworkQuantity { get; set; }

        /// <summary>
        /// 返修数量。
        /// </summary>
        [DataMember]
        public virtual double RepairQuantity { get; set; }

        /// <summary>
        /// 计划开始时间。
        /// </summary>
        [DataMember]
        public virtual DateTime PlanStartTime { get; set; }

        /// <summary>
        /// 计划完成时间。
        /// </summary>
        [DataMember]
        public virtual DateTime PlanFinishTime { get; set; }

        /// <summary>
        /// 开始时间。
        /// </summary>
        [DataMember]
        public virtual DateTime StartTime { get; set; }

        /// <summary>
        /// 完成时间。
        /// </summary>
        [DataMember]
        public virtual DateTime FinishTime { get; set; }

        /// <summary>
        /// 工单关闭类型。
        /// </summary>
        [DataMember]
        public virtual EnumWorkOrderCloseType CloseType { get; set; }

        /// <summary>
        /// 保税类型。
        /// </summary>
        [DataMember]
        public virtual string RevenueType { get; set; }

        /// <summary>
        /// 车间名称。
        /// </summary>
        [DataMember]
        public virtual string LocationName { get; set; }

        /// <summary>
        /// 描述。
        /// </summary>
        [DataMember]
        public virtual string Description { get; set; }

        /// <summary>
        /// 工单创建类型 S - MES系统创建 E - ERP系统创建
        /// </summary>
        [DataMember]
        public virtual EnumWorkOrderCreateType CreateType { get; set; }

        /// <summary>
        /// ERP工单主键
        /// </summary>
        [DataMember]
        public virtual string ERPWorkOrderKey { get; set; }

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
