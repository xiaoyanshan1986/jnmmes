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
    /// 工单属性主键。
    /// </summary>
    public struct WorkOrderAttributeKey
    {
        /// <summary>
        /// 工单号。
        /// </summary>
        public string OrderNumber { get; set; }
        /// <summary>
        /// 属性名称。
        /// </summary>
        public string AttributeName { get; set; }

        public override string ToString()
        {
            return string.Format("{0}:{1}", this.OrderNumber, this.AttributeName);
        }
    }

    /// <summary>
    /// 描述工单属性的模型类。
    /// </summary>
    [DataContract]
    public class WorkOrderAttribute : BaseModel<WorkOrderAttributeKey>
    {
        /// <summary>
        /// 属性值。
        /// </summary>
        [DataMember]
        public virtual string AttributeValue { get; set; }
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
