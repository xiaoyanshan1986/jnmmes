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
    /// 描述批次终止事务数据的模型类。
    /// </summary>
    [DataContract]
    public class LotTransactionTerminal : BaseModel<string>
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
