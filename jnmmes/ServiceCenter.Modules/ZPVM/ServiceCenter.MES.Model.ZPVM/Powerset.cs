using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;
using System.ComponentModel.DataAnnotations;
using ServiceCenter.MES.Model.ZPVM.Resources;

namespace ServiceCenter.MES.Model.ZPVM
{

    /// <summary>
    /// 子分档方式。
    /// </summary>
    public enum EnumPowersetSubWay
    {
        /// <summary>
        /// 无子分档。
        /// </summary>
        [Display(Name = "EnumPowersetSubWay_None", ResourceType = typeof(StringResource))]
        None=0,
        /// <summary>
        /// 电流子分档。
        /// </summary>
        [Display(Name = "EnumPowersetSubWay_ISC", ResourceType = typeof(StringResource))]
        ISC=1,
        /// <summary>
        /// 电压子分档。
        /// </summary>
        [Display(Name = "EnumPowersetSubWay_VOC", ResourceType = typeof(StringResource))]
        VOC=2,
        /// <summary>
        /// 电流子分档。
        /// </summary>
        [Display(Name = "EnumPowersetSubWay_IPM", ResourceType = typeof(StringResource))]
        IPM = 3,
        /// <summary>
        /// 电流子分档。
        /// </summary>
        [Display(Name = "EnumPowersetSubWay_VPM", ResourceType = typeof(StringResource))]
        VPM = 4,
    }
    /// <summary>
    /// 表示分档主键。
    /// </summary>
    public struct PowersetKey
    {
        /// <summary>
        /// 分档代码。
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 项目号。
        /// </summary>
        public int ItemNo { get; set; }

        public override string ToString()
        {
            return string.Format("{0}:{1}", this.Code, this.ItemNo);
        }
    }
    /// <summary>
    /// 描述分档数据模型
    /// </summary>
    [DataContract]
    public class Powerset : BaseModel<PowersetKey>
    {
        /// <summary>
        /// 分档名称。
        /// </summary>
        [DataMember]
        public virtual string Name { get; set; }
        /// <summary>
        /// 最小值。
        /// </summary>
        [DataMember]
        public virtual double? MinValue { get; set; }
        /// <summary>
        /// 最大值。
        /// </summary>
        [DataMember]
        public virtual double? MaxValue { get; set; }
        /// <summary>
        /// 档位名称。
        /// </summary>
        [DataMember]
        public virtual string PowerName { get; set; }
        /// <summary>
        /// 标准功率。
        /// </summary>
        [DataMember]
        public virtual double? StandardPower { get; set; }
        /// <summary>
        /// 标准电流。
        /// </summary>
        [DataMember]
        public virtual double? StandardIsc { get; set; }
        /// <summary>
        /// 标准电压。
        /// </summary>
        [DataMember]
        public virtual double? StandardVoc { get; set; }
        /// <summary>
        /// 标准最大电流。
        /// </summary>
        [DataMember]
        public virtual double? StandardIPM { get; set; }
        /// <summary>
        /// 标准最大电压。
        /// </summary>
        [DataMember]
        public virtual double? StandardVPM { get; set; }
        /// <summary>
        /// 标准填充因子。
        /// </summary>
        [DataMember]
        public virtual double? StandardFuse { get; set; }
        /// <summary>
        /// 分档方式。
        /// </summary>
        [DataMember]
        public virtual string PowerDifference { get; set; }
        /// <summary>
        /// 子分档方式。
        /// </summary>
        [DataMember]
        public virtual EnumPowersetSubWay SubWay { get; set; }
        /// <summary>
        /// ArticleNo。
        /// </summary>
        [DataMember]
        public virtual string ArticleNo { get; set; }
        /// <summary>
        /// 描述。
        /// </summary>
        [DataMember]
        public virtual string Description { get; set; }

        /// <summary>
        /// 是否混颜色。
        /// </summary>
        [DataMember]
        public virtual bool MixColor { get; set; }
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
