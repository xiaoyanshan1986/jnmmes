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
    /// 表示工序设备明细数据主键。
    /// </summary>
    public struct RouteOperationEquipmentKey
    {
        /// <summary>
        /// 工序名称。
        /// </summary>
        public string RouteOperationName { get; set; }
        /// <summary>
        /// 设备代码。
        /// </summary>
        public string EquipmentCode { get; set; }

        public override string ToString()
        {
            return string.Format("{0}:{1}", this.RouteOperationName, this.EquipmentCode);
        }
    }
    /// <summary>
    /// 表示工序设备明细数据。
    /// </summary>
    [DataContract]
    public class RouteOperationEquipment : BaseModel<RouteOperationEquipmentKey>
    {
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
