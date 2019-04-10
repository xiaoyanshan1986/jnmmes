using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;
using System.ComponentModel.DataAnnotations;
using ServiceCenter.MES.Model.PPM.Resources;

namespace ServiceCenter.MES.Model.PPM
{    
    /// <summary>
    /// 表示混工单组规则主键。
    /// </summary>
    public struct WorkOrderGroupDetailKey
    {
        /// <summary>
        /// 混工单组。
        /// </summary>
        public string WorkOrderGroupNo { get; set; }
        /// <summary>
        /// 工单号。
        /// </summary>
        public string OrderNumber { get; set; }
        /// <summary>
        /// 产品编码。
        /// </summary>
        public string ProductCode { get; set; }
                       
        public override string ToString()
        {
            return string.Format("{0}:{1}:{2}", this.WorkOrderGroupNo, this.OrderNumber,this.ProductCode);
        }
    }
    /// <summary>
    /// 描述分档数据模型
    /// </summary>
    [DataContract]
    public class WorkOrderGroupDetail : BaseModel<WorkOrderGroupDetailKey>
    {
        /// <summary>
        /// 项目号。
        /// </summary>
        [DataMember]
        public virtual int ItemNo { get; set; }

        /// <summary>
        /// 号别。
        /// </summary>
        [DataMember]
        public virtual int Nums { get; set; }
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

        /// <summary>
        /// 描述。
        /// </summary>
        [DataMember]
        public virtual string Description { get; set; }
    }
}
