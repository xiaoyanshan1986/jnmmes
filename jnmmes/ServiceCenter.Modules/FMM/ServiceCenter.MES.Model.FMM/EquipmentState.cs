using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;
using ServiceCenter.MES.Model.FMM.Resources;
using System.ComponentModel.DataAnnotations;

namespace ServiceCenter.MES.Model.FMM
{
    /// <summary>
    /// 设备状态类型
    /// </summary>
    public enum EnumEquipmentStateType
    {
        /// <summary>
        /// 生产状态。
        /// </summary>
        [Display(Name = "EnumEquipmentStateType_Run", ResourceType = typeof(StringResource))]
        Run=0,

        /// <summary>
        /// 空闲状态。
        /// </summary>
        [Display(Name = "EnumEquipmentStateType_Lost", ResourceType = typeof(StringResource))]
        Lost=1,

        /// <summary>
        /// 停机状态。
        /// </summary>
        [Display(Name = "EnumEquipmentStateType_Down", ResourceType = typeof(StringResource))]
        Down = 2,

        /// <summary>
        /// 保养状态。
        /// </summary>
        [Display(Name = "EnumEquipmentStateType_PM", ResourceType = typeof(StringResource))]
        PM = 3,

        /// <summary>
        /// 测试状态。
        /// </summary>
        [Display(Name = "EnumEquipmentStateType_Test", ResourceType = typeof(StringResource))]
        Test = 4,

        ///// <summary>
        ///// 设备不启用。
        ///// </summary>
        //[Display(Name = "EnumEquipmentStateType_Off", ResourceType = typeof(StringResource))]
        //OFF = 5,
        /// <summary>
        /// 等待设备工程师。
        /// </summary>
        //[Display(Name = "EnumEquipmentStateType_WEE", ResourceType = typeof(StringResource))]
        //W_EE = 6,
        ///// <summary>
        ///// 等待工艺工程师。
        ///// </summary>
        //[Display(Name = "EnumEquipmentStateType_WPE", ResourceType = typeof(StringResource))]
        //W_PE = 7,
        ///// <summary>
        ///// 等待生产操作人员。
        ///// </summary>
        //[Display(Name = "EnumEquipmentStateType_WMFG", ResourceType = typeof(StringResource))]
        //W_MFG = 8,
        ///// <summary>
        ///// 断气、断电等异常情况。
        ///// </summary>
        //[Display(Name = "EnumEquipmentStateType_Facility", ResourceType = typeof(StringResource))]
        //Facility = 9,

        /// <summary>
        /// 其他状态。
        /// </summary>
        [Display(Name = "EnumEquipmentStateType_Other", ResourceType = typeof(StringResource))]
        Other = 10
    }
    /// <summary>
    /// 设备状态分类
    /// </summary>
    public enum EnumEquipmentStateCategory
    {
        /// <summary>
        /// 运行时间。
        /// </summary>
        [Display(Name = "EnumEquipmentStateCategory_UpTime", ResourceType = typeof(StringResource))]
        UpTime = 0,
        /// <summary>
        /// 停机时间。
        /// </summary>
        [Display(Name = "EnumEquipmentStateCategory_DownTime", ResourceType = typeof(StringResource))]
        DownTime = 1,
        /// <summary>
        /// 其他。
        /// </summary>
        [Display(Name = "EnumEquipmentStateCategory_Other", ResourceType = typeof(StringResource))]
        Other=9
    }
    /// <summary>
    /// 表示设备状态数据模型类。
    /// </summary>
    [DataContract]
    public class EquipmentState : BaseModel<string>
    {
        /// <summary>
        /// 主键（设备状态名称）。
        /// </summary>
        [DataMember]
        public override string Key
        {
            get;
            set;
        }
        /// <summary>
        /// 描述。
        /// </summary>
        [DataMember]
        public virtual  string Description { get; set; }
        /// <summary>
        /// 类型。
        /// </summary>
        [DataMember]
        public virtual EnumEquipmentStateType Type { get; set; }
        /// <summary>
        /// 状态分组。
        /// </summary>
        [DataMember]
        public virtual EnumEquipmentStateCategory Category { get; set; }
        /// <summary>
        /// 状态颜色。
        /// </summary>
        [DataMember]
        public virtual string StateColor { get; set; }
        /// <summary>
        /// 创建人。
        /// </summary>
        [DataMember]
        public virtual  string Creator { get; set; }
        /// <summary>
        /// 创建时间。
        /// </summary>
        [DataMember]
        public virtual  DateTime? CreateTime { get; set; }
        /// <summary>
        /// 编辑人。
        /// </summary>
        [DataMember]
        public virtual  string Editor { get; set; }
        /// <summary>
        /// 编辑时间。
        /// </summary>
        [DataMember]
        public virtual  DateTime? EditTime { get; set; }
    }
}
