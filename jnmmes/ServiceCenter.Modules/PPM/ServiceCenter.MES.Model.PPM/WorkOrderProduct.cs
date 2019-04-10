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
    /// 工单产品主键。
    /// </summary>
    public struct WorkOrderProductKey
    {
        /// <summary>
        /// 工单号。
        /// </summary>
        public string OrderNumber { get; set; }
        /// <summary>
        /// 产品代码。
        /// </summary>
        public string MaterialCode { get; set; }

        public override string ToString()
        {
            return string.Format("{0}:{1}", this.OrderNumber, this.MaterialCode);
        }
    }

    /// <summary>
    /// 描述工单产品的模型类。
    /// </summary>
    [DataContract]
    public class WorkOrderProduct : BaseModel<WorkOrderProductKey>
    {
        /// <summary>
        /// 产品值。
        /// </summary>
        [DataMember]
        public virtual int ItemNo { get; set; }
        /// <summary>
        /// 是否主产品。
        /// </summary>
        [DataMember]
        public virtual bool IsMain { get; set; }
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
