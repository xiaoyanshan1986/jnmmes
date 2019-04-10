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
    /// 描述批次操作事务下一工步数据的模型类。
    /// </summary>
    [DataContract]
    public class LotTransactionStep : BaseModel<string>
    {
        /// <summary>
        /// 下一工艺流程组名称。
        /// </summary>
        [DataMember]
        public virtual string ToRouteEnterpriseName { get; set; }
        /// <summary>
        /// 下一工艺流程名称。
        /// </summary>
        [DataMember]
        public virtual string ToRouteName { get; set; }
        /// <summary>
        /// 下一工艺流程工步名称。
        /// </summary>
        [DataMember]
        public virtual string ToRouteStepName { get; set; }
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
