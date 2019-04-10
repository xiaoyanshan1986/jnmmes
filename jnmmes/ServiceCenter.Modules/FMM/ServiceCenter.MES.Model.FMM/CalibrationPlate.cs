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
    /// 校准办类型
    /// </summary>
    public enum EnumPlateType
    {
        /// <summary>
        /// 60组件。
        /// </summary>
        [Display(Name = "EnumPlateType_P60", ResourceType = typeof(StringResource))]
        P60 = 0,
        /// <summary>
        /// 72组件。
        /// </summary>
        [Display(Name = "EnumPlateType_P72", ResourceType = typeof(StringResource))]
        P72 = 1,
        /// <summary>
        /// 双玻。
        /// </summary>
        [Display(Name = "EnumPlateType_DoubleGlass", ResourceType = typeof(StringResource))]
        DoubleGlass = 2,
    }
    /// <summary>
    /// 描述校准板的数据模型类。
    /// </summary>
    [DataContract]
    public class CalibrationPlate : BaseModel<string>
    {
        /// <summary>
        /// 主键(校准板编号)。
        /// </summary>
        [DataMember]
        public override string Key
        {
            get;
            set;
        }
        /// <summary>
        /// 校准板类型
        /// </summary>
        [DataMember]
        public virtual int CalibrationPlateType { get; set; }
        /// <summary>
        /// 校准板名称
        /// </summary>
        [DataMember]
        public virtual string CalibrationPlateName { get; set; }
        /// <summary>
        /// <summary>
        /// 功率值。
        /// </summary>
        [DataMember]
        public virtual double PM { get; set; }
        /// <summary>
        /// 短路电流值。
        /// </summary>
        [DataMember]
        public virtual double ISC { get; set; }
        /// <summary>
        /// 开路电压。
        /// </summary>
        [DataMember]
        public virtual double VOC { get; set; }
        /// <summary>
        /// 最大功率。
        /// </summary>
        [DataMember]
        public virtual double MaxPM { get; set; }
        /// <summary>
        /// 最小功率。
        /// </summary>
        [DataMember]
        public virtual double MinPM { get; set; }
        /// <summary>
        /// 最大电流。
        /// </summary>
        [DataMember]
        public virtual double MaxISC { get; set; }
        /// <summary>
        /// 最小电流。
        /// </summary>
        [DataMember]
        public virtual double MinISC { get; set; }
        /// <summary>
        /// 最大电压。
        /// </summary>
        [DataMember]
        public virtual double MaxVOC { get; set; }
        /// <summary>
        /// 最小电压。
        /// </summary>
        [DataMember]
        public virtual double MinVOC { get; set; }
        /// <summary>
        /// 描述。
        /// </summary>
        [DataMember]
        public virtual string Explain{ get; set; }
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

        /// <summary>
        /// 标准电流1。
        /// </summary>
        [DataMember]
        public virtual double StdIsc1 { get; set; }

        /// <summary>
        /// 标准电流2。
        /// </summary>
        [DataMember]
        public virtual double StdIsc2 { get; set; }
        /// <summary>
        /// 标准光强1。
        /// </summary>
        [DataMember]
        public virtual double Stdsun1 { get; set; }
        /// <summary>
        /// 标准光强2。
        /// </summary>
        [DataMember]
        public virtual double Stdsun2 { get; set; }
    }
}
