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
    /// 工单BOM主键。
    /// </summary>
    public struct WorkOrderBOMKey
    {
        /// <summary>
        /// 工单号。
        /// </summary>
        public string OrderNumber { get; set; }
        /// <summary>
        /// 项目号。
        /// </summary>
        public int ItemNo { get; set; }

        public override string ToString()
        {
            return string.Format("{0}:{1}", this.OrderNumber, this.ItemNo);
        }
    }

    /// <summary>
    /// 描述工单BOM的模型类。
    /// </summary>
    [DataContract]
    public class WorkOrderBOM : BaseModel<WorkOrderBOMKey>
    {
        /// <summary>
        /// 物料编码。
        /// </summary>
        [DataMember]
        public virtual string MaterialCode { get; set; }

        /// <summary>
        /// 数量。
        /// </summary>
        [DataMember]
        public virtual decimal Qty { get; set; }

        /// <summary>
        /// 单位。
        /// </summary>
        [DataMember]
        public virtual string MaterialUnit { get; set; }

        /// <summary>
        /// 最小单元
        /// </summary>
        [DataMember]
        public virtual decimal MinUnit { get; set; }

        /// <summary>
        /// 可替代物料
        /// </summary>
        [DataMember]
        public virtual string ReplaceMaterial { get; set; }

        /// <summary>
        /// 工作中心。
        /// </summary>
        [DataMember]
        public virtual string WorkCenter { get; set; }

        /// <summary>
        /// 存储位置。
        /// </summary>
        [DataMember]
        public virtual string StoreLocation { get; set; }

        /// <summary>
        /// 描述。
        /// </summary>
        [DataMember]
        public virtual string Description { get; set; }

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
