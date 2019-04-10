using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;

namespace ServiceCenter.MES.Model.WIP
{
    /// <summary>
    /// 批次加工设备状态。
    /// </summary>
    public enum EnumLotTransactionEquipmentState
    {
        /// <summary>
        /// 已删除。
        /// </summary>
        Deleted=-1,
        /// <summary>
        /// 开始。
        /// </summary>
        Start=0,
        /// <summary>
        /// 结束。
        /// </summary>
        End=1
    }
    /// <summary>
    /// 描述批次加工设备数据的模型类。
    /// </summary>
    [DataContract]
    public class LotTransactionEquipment : BaseModel<string>
    {
        /// <summary>
        /// 设备加工结束事务操作主键。
        /// </summary>
        [DataMember]
        public virtual string EndTransactionKey { get; set; }
        /// <summary>
        /// 批次号。
        /// </summary>
        [DataMember]
        public virtual string LotNumber { get; set; }
        /// <summary>
        /// 设备代码。
        /// </summary>
        [DataMember]
        public virtual string EquipmentCode { get; set; }
        /// <summary>
        /// 加工开始时间。
        /// </summary>
        [DataMember]
        public virtual DateTime? StartTime { get; set; }
        /// <summary>
        /// 加工结束时间。
        /// </summary>
        [DataMember]
        public virtual DateTime? EndTime { get; set; }
        /// <summary>
        /// 数量。
        /// </summary>
         [DataMember]
        public virtual double Quantity { get; set; }
        /// <summary>
        /// 批次设备加工状态。
        /// </summary>
        [DataMember]
         public virtual EnumLotTransactionEquipmentState State { get; set; }
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
