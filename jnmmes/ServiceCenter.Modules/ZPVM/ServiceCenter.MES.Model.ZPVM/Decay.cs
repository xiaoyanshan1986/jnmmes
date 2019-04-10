using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;
using System.ComponentModel.DataAnnotations;
using ServiceCenter.MES.Model.ZPVM.Resources;
using ServiceCenter.Common;

namespace ServiceCenter.MES.Model.ZPVM
{
    /// <summary>
    /// 光伏组件测试数据对象。
    /// </summary>
    public enum EnumPVMTestDataType
    {
        /// <summary>
        /// 功率。
        /// </summary>
        [Display(Name = "PVMTestDataType_PM", ResourceType = typeof(StringResource))]
        PM = 0,
        /// <summary>
        /// 电流。
        /// </summary>
        [Display(Name = "PVMTestDataType_ISC", ResourceType = typeof(StringResource))]
        ISC = 1,
        /// <summary>
        /// 电压。
        /// </summary>
        [Display(Name = "PVMTestDataType_VOC", ResourceType = typeof(StringResource))]
        VOC = 2,
        /// <summary>
        /// 最大电流
        /// </summary>
        [Display(Name = "PVMTestDataType_IPM", ResourceType = typeof(StringResource))]
        IPM = 3,
        /// <summary>
        /// 最大电压。
        /// </summary>
        [Display(Name = "PVMTestDataType_VPM", ResourceType = typeof(StringResource))]
        VPM = 4,
        /// <summary>
        /// 填充因子。
        /// </summary>
        [Display(Name = "PVMTestDataType_FF", ResourceType = typeof(StringResource))]
        FF = 5,
        /// <summary>
        /// CTM
        /// </summary>
        [Display(Name = "PVMTestDataType_CTM", ResourceType = typeof(StringResource))]
        CTM = 6,
        /// <summary>
        /// 测试温度
        /// </summary>
        [Display(Name = "PVMTestDataType_AmbientTemperature", ResourceType = typeof(StringResource))]
        AmbientTemperature = 7,
        
        /// <summary>
        /// RSH
        /// </summary>
        [Display(Name = "RSH")]
        RSH = 8
    }

    /// <summary>
    /// 衰减类型。
    /// </summary>
    public enum EnumDecayType
    {
        /// <summary>
        /// 比例衰减。
        /// </summary>
        [Display(Name = "EnumDecayType_Rate", ResourceType = typeof(StringResource))]
        Rate=0,
        /// <summary>
        /// 目标值衰减。
        /// </summary>
        [Display(Name = "EnumDecayType_Aim", ResourceType = typeof(StringResource))]
        Aim=1
    }

    /// <summary>
    /// 表示衰减系数主键。
    /// </summary>
    public struct DecayKey
    {
        /// <summary>
        /// 衰减代码。
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 衰减对象。
        /// </summary>
        public EnumPVMTestDataType Object { get; set; }

        public override string ToString()
        {
            return string.Format("{0}:{1}", this.Code, this.Object.GetDisplayName());
        }
    }
    /// <summary>
    /// 描述衰减系数数据模型
    /// </summary>
    [DataContract]
    public class Decay : BaseModel<DecayKey>
    {
        /// <summary>
        /// 衰减值。
        /// </summary>
        [DataMember]
        public virtual double Value { get; set; }
        /// <summary>
        /// 衰减类型。
        /// </summary>
        [DataMember]
        public virtual EnumDecayType Type { get; set; }
        /// <summary>
        /// 描述。
        /// </summary>
        [DataMember]
        public virtual string Description { get; set; }
        /// <summary>
        /// 是否可用。
        /// </summary>
        [DataMember]
        public virtual bool IsUsed { get; set; }
        /// <summary>
        /// 创建人。
        /// </summary>
        [DataMember]
        public virtual  string Creator { get; set; }
        /// <summary>
        /// 创建时间。
        /// </summary>
        [DataMember]
        public virtual  DateTime CreateTime { get; set; }
        /// <summary>
        /// 编辑人。
        /// </summary>
        [DataMember]
        public virtual  string Editor { get; set; }
        /// <summary>
        /// 编辑时间。
        /// </summary>
        [DataMember]
        public virtual  DateTime EditTime { get; set; }

    }
}
