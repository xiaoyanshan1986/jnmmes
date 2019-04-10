using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using ServiceCenter.MES.Model.EDC.Resources;

namespace ServiceCenter.MES.Model.EDC
{
    /// <summary>
    /// 采集数据状态
    /// </summary>
    public enum EnumAcquisitionDataState
    {
        /// <summary>
        /// 新增
        /// </summary>
        [Display(Name = "AcquisitionDataState_New", ResourceType = typeof(StringResource))]
        New = 0,
        /// <summary>
        /// 已审核
        /// </summary>
        [Display(Name = "AcquisitionDataState_Reviewed", ResourceType = typeof(StringResource))]
        Reviewed = 1
    }

    /// <summary>
    /// 表示项目数据主键
    /// </summary>
    public struct DataAcquisitionDataKey
    {
        /// <summary>
        /// 采集时间
        /// </summary>
        [DataMember]
        public DateTime EDCTime { get; set; }

        /// <summary>
        /// 采集项目代码
        /// </summary>
        public string ItemCode { get; set; }

        /// <summary>
        /// 采集字段
        /// </summary>
        public string FieldCode { get; set; }

        /// <summary>
        /// 车间
        /// </summary>
        public string LocationName { get; set; }

        /// <summary>
        /// 线别
        /// </summary>
        public string LineCode { get; set; }

        /// <summary>
        /// 设备代码
        /// </summary>
        public string EquipmentCode { get; set; }

        public override string ToString()
        {
            return string.Format("{0}:{1}:{2}:{3}:{4}:{5}", this.EDCTime, this.ItemCode, this.FieldCode, this.LocationName, this.LineCode, this.EquipmentCode);
        }
    }

    /// <summary>
    /// 表示采集数据数据
    /// </summary>
    [DataContract]
    public class DataAcquisitionData : BaseModel<DataAcquisitionDataKey>
    {      
        /// <summary>
        /// 数据
        /// </summary>
        [DataMember]
        public virtual string DataValue { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        public virtual EnumAcquisitionDataState DataState { get; set; }
        
        /// <summary>
        /// 创建人
        /// </summary>
        [DataMember]
        public virtual string Creator { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMember]
        public virtual DateTime? CreateTime { get; set; }

        /// <summary>
        /// 审核人
        /// </summary>
        [DataMember]
        public virtual string Auditor { get; set; }

        /// <summary>
        /// 审核时间
        /// </summary>
        [DataMember]
        public virtual DateTime? AuditTime { get; set; }

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
