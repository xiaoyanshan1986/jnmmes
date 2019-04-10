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
    /// 表示工单产品衰减设置主键。
    /// </summary>
    public struct WorkOrderDecayKey
    {
        /// <summary>
        /// 工单号。
        /// </summary>
        public string OrderNumber { get; set; }
        /// <summary>
        /// 产品料号。
        /// </summary>
        public string MaterialCode { get; set; }
        /// <summary>
        /// 最小功率值。
        /// </summary>
        public double MinPower { get; set; }
        /// <summary>
        /// 最大功率值。
        /// </summary>
        public double MaxPower { get; set; }

        public override string ToString()
        {
            return string.Format("{0}:{1}({2}-{3})", this.OrderNumber, this.MaterialCode, this.MinPower, this.MaxPower);
        }
    }
    /// <summary>
    /// 描述工单产品衰减系数设置的数据模型
    /// </summary>
    [DataContract]
    public class WorkOrderDecay : BaseModel<WorkOrderDecayKey>
    {
        /// <summary>
        /// 衰减系数代码。
        /// </summary>
        [DataMember]
        public virtual string DecayCode { get; set; }
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
