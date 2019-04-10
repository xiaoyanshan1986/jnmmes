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
    /// 描述批次暂停释放操作事务数据的模型类。
    /// </summary>
    [DataContract]
    public class LotTransactionHoldRelease : BaseModel<string>
    {
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
        /// 暂停操作人。
        /// </summary>
        [DataMember]
        public virtual string HoldOperator { get; set; }
        /// <summary>
        /// 暂停时间。
        /// </summary>
        [DataMember]
        public virtual DateTime? HoldTime { get; set; }
        /// <summary>
        /// 暂停描述。
        /// </summary>
        [DataMember]
        public virtual string HoldDescription { get; set; }
        /// <summary>
        /// 暂停密码。
        /// </summary>
        [DataMember]
        public virtual string HoldPassword { get; set; }
        /// <summary>
        /// 是否释放。
        /// </summary>
        [DataMember]
        public virtual bool IsRelease { get; set; }
        /// <summary>
        /// 暂停释放事务操作主键。
        /// </summary>
        [DataMember]
        public virtual string ReleaseTransactionKey { get; set; }
        /// <summary>
        /// 暂停释放操作人。
        /// </summary>
        [DataMember]
        public virtual string ReleaseOperator { get; set; }
        /// <summary>
        /// 暂停释放时间。
        /// </summary>
        [DataMember]
        public virtual DateTime? ReleaseTime { get; set; }
        /// <summary>
        /// 暂停释放描述。
        /// </summary>
        [DataMember]
        public virtual string ReleaseDescription { get; set; }
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
