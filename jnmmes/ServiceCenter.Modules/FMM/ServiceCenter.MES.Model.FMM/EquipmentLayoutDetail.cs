using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;

namespace ServiceCenter.MES.Model.FMM
{
    /// <summary>
    /// 设备布局明细数据主键。
    /// </summary>
    public struct EquipmentLayoutDetailKey
    {
        /// <summary>
        /// 布局名称。
        /// </summary>
        public string LayoutName { get; set; }
        /// <summary>
        /// 设备代码。
        /// </summary>
        public string EquipmentCode { get; set; }

        public override string ToString()
        {
            return string.Format("{0}:{1}", this.LayoutName, this.EquipmentCode);
        }
    }
    /// <summary>
    /// 设备布局明细数据模型类。
    /// </summary>
    [DataContract]
    public class EquipmentLayoutDetail : BaseModel<EquipmentLayoutDetailKey>
    {
        /// <summary>
        /// 位置Left。
        /// </summary>
        [DataMember]
        public virtual int Left { get; set; }
        /// <summary>
        /// 位置Top。
        /// </summary>
        [DataMember]
        public virtual int Top { get; set; }
        /// <summary>
        /// 宽度。
        /// </summary>
        [DataMember]
        public virtual int Width{ get; set; }
        /// <summary>
        /// 高度。
        /// </summary>
        [DataMember]
        public virtual int Height { get; set; }
        /// <summary>
        /// 描述。
        /// </summary>
        [DataMember]
        public virtual string Description { get; set; }
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
