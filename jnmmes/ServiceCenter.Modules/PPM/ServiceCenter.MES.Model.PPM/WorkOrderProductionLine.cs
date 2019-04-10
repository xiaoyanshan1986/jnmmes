using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;

namespace ServiceCenter.MES.Model.PPM
{
    /// <summary>
    /// 工单线别主键。
    /// </summary>
    public struct WorkOrderProductionLineKey
    {
        /// <summary>
        /// 工单号。
        /// </summary>
        public string OrderNumber { get; set; }
        /// <summary>
        /// 生产线代码。
        /// </summary>
        public string LineCode { get; set; }

        public override string ToString()
        {
            return string.Format("{0}:{1}", this.OrderNumber, this.LineCode);
        }
    }

    /// <summary>
    /// 描述工单线别的模型类。
    /// </summary>
    [DataContract]
    public class WorkOrderProductionLine : BaseModel<WorkOrderProductionLineKey>
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
