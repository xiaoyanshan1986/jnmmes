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
    /// 设备类型。
    /// </summary>
    public enum EnumEquipmentType
    {
        /// <summary>
        /// 物理设备。
        /// </summary>
        [Display(Name = "EquipmentType_Physical", ResourceType = typeof(StringResource))]
        Physical=0,
        /// <summary>
        /// 虚拟设备。
        /// </summary>
        [Display(Name = "EquipmentType_Virtual", ResourceType = typeof(StringResource))]
        Virtual=1
    }
    /// <summary>
    /// 设备数据模型类。
    /// </summary>
    [DataContract]
    public class Equipment : BaseModel<string>
    {
        /// <summary>
        /// 主键（设备代码）。
        /// </summary>
        [DataMember]
        public override string Key
        {
            get;
            set;
        }
        /// <summary>
        /// 名称。
        /// </summary>
        [DataMember]
        public virtual string Name { get; set; }
        /// <summary>
        /// 设备编号。
        /// </summary>
        [DataMember]
        public virtual string No { get; set; }
        /// <summary>
        /// 类型。
        /// </summary>
        [DataMember]
        public virtual EnumEquipmentType Type { get; set; }
        /// <summary>
        /// 每小时加工量
        /// </summary>
        [DataMember]
        public virtual double? WPH { get; set; }
        /// <summary>
        /// 设备平均时间
        /// </summary>
        [DataMember]
        public virtual double? AvTime { get; set; }
        /// <summary>
        /// 设备TactTime
        /// </summary>
        [DataMember]
        public virtual double? TactTime { get; set; }
        /// <summary>
        /// 资产编号。
        /// </summary>
        [DataMember]
        public virtual string AssetsNo { get; set; }
        /// <summary>
        /// 物理设备代码。
        /// </summary>
        [DataMember]
        public virtual string RealEquipmentCode { get; set; }
        /// <summary>
        /// 是否多腔体设备。 
        /// </summary>
        [DataMember]
        public virtual bool IsMultiChamber { get; set; }
        /// <summary>
        /// 是否批处理设备。 
        /// </summary>
        [DataMember]
        public virtual bool IsBatch { get; set; }
        /// <summary>
        /// 是否是设备腔体。 
        /// </summary>
        [DataMember]
        public virtual bool IsChamber { get; set; }
        /// <summary>
        /// 设备腔体数量。
        /// </summary>
        [DataMember]
        public virtual int? TotalChamber { get; set; }
        /// <summary>
        /// 腔体索引序号。
        /// </summary>
        [DataMember]
        public virtual int? ChamberIndex { get; set; }
        /// <summary>
        /// 设备运行速率
        /// </summary>
        [DataMember]
        public virtual double? RunRate { get; set; }
        /// <summary>
        /// 描述。
        /// </summary>
        [DataMember]
        public virtual string Description { get; set; }
        /// <summary>
        /// 最小加工数量。
        /// </summary>
        [DataMember]
        public virtual double? MinQuantity { get; set; }
        /// <summary>
        /// 最大加工数量。
        /// </summary>
        [DataMember]
        public virtual double? MaxQuantity { get; set; }

        /// <summary>
        /// 设备状态名称。
        /// </summary>
        [DataMember]
        public virtual string StateName { get; set; }
        /// <summary>
        /// 设备状态切换名称。
        /// </summary>
        [DataMember]
        public virtual string ChangeStateName { get; set; }
        /// <summary>
        /// 设备组名称。
        /// </summary>
        [DataMember]
        public virtual string GroupName { get; set; }
        /// <summary>
        /// 区域名称。
        /// </summary>
        [DataMember]
        public virtual string LocationName { get; set; }
        /// <summary>
        /// 产线代码。
        /// </summary>
        [DataMember]
        public virtual string LineCode { get; set; }
        /// <summary>
        /// 父设备代码。
        /// </summary>
        [DataMember]
        public virtual string ParentEquipmentCode { get; set; }
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
