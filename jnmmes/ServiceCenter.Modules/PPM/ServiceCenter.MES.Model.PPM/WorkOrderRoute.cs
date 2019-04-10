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
    /// 工单工艺主键。
    /// </summary>
    public struct WorkOrderRouteKey
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
    /// 描述工单工艺的模型类。
    /// </summary>
    [DataContract]
    public class WorkOrderRoute : BaseModel<WorkOrderRouteKey>
    {
        /// <summary>
        /// 工艺流程组名称。
        /// </summary>
         [DataMember]
         public virtual string RouteEnterpriseName { get; set; }
        /// <summary>
        /// 工艺流程名称。
        /// </summary>
        [DataMember]
        public virtual string RouteName { get; set; }
        /// <summary>
        /// 工步名称。
        /// </summary>
        [DataMember]
        public virtual string RouteStepName { get; set; }

        /// <summary>
        /// 是否返工/返修工艺流程。
        /// </summary>
        [DataMember]
        public virtual bool IsRework { get; set; }

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
