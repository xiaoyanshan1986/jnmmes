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
    /// 描述批次预设暂停数据的模型类。
    /// </summary>
    [DataContract]
    public class LotFuturehold : BaseModel<string>
    {
        /// <summary>
        /// 批次号。
        /// </summary>
        [DataMember]
        public virtual string LotNumber { get; set; }
        /// <summary>
        /// 工单号。
        /// </summary>
        [DataMember]
        public virtual string OrderNumber { get; set; }
        /// <summary>
        /// 暂停密码。
        /// </summary>
        [DataMember]
        public virtual string Password { get; set; }
        /// <summary>
        /// 触发动作名称。
        /// </summary>
        [DataMember]
        public virtual string ActionName { get; set; }
        /// <summary>
        /// 工艺流程组名称。
        /// </summary>
        [DataMember]
        public virtual string RouteEnterpriseName { get; set; }
        /// <summary>
        /// 工艺流程名称。
        /// </summary>
        [DataMember]
        public virtual string RouteName{ get; set; }
        /// <summary>
        /// 工步名称。
        /// </summary>
        [DataMember]
        public virtual string RouteStepName { get; set; }
        /// <summary>
        /// 预设暂停时工艺流程组名称。
        /// </summary>
        [DataMember]
        public virtual string SetRouteEnterpriseName { get; set; }
        /// <summary>
        /// 预设暂停时工艺流程名称。
        /// </summary>
        [DataMember]
        public virtual string SetRouteName { get; set; }
        /// <summary>
        /// 预设暂停时工步名称。
        /// </summary>
        [DataMember]
        public virtual string SetRouteStepName { get; set; }
        /// <summary>
        /// 工序名称。
        /// </summary>
        [DataMember]
        public virtual string RouteOperationName { get; set; }
        /// <summary>
        /// 原因代码组名称。
        /// </summary>
        [DataMember]
        public virtual string ReasonCodeCategoryName { get; set; }
        /// <summary>
        /// 原因代码名称。
        /// </summary>
        [DataMember]
        public virtual string ReasonCodeName { get; set; }
        /// <summary>
        /// 备注。
        /// </summary>
        [DataMember]
        public virtual string Description { get; set; }
        /// <summary>
        /// 状态。
        /// </summary>
        [DataMember]
        public virtual EnumObjectStatus Status { get; set; }
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
        /// 创建人。
        /// </summary>
        [DataMember]
        public virtual string Creator { get; set; }
        /// <summary>
        /// 创建时间。
        /// </summary>
        [DataMember]
        public virtual DateTime? CreateTime { get; set; }
    }
}
