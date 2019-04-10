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
    /// 批次合并操作事务数据主键。
    /// </summary>
    public struct LotTransactionMergeKey
    {
        /// <summary>
        /// 合并批次的事务操作主键。
        /// </summary>
        public string TransactionKey { get; set; }
        /// <summary>
        /// 被合并批次的事务操作主键。
        /// </summary>
        public string ChildTransactionKey { get; set; }
    }
    /// <summary>
    /// 描述批次合并操作事务数据的模型类。
    /// </summary>
    [DataContract]
    public class LotTransactionMerge : BaseModel<LotTransactionMergeKey>
    {
        /// <summary>
        /// 批次号。
        /// </summary>
        [DataMember]
        public virtual string MainLotNumber { get; set; }
        /// <summary>
        /// 被合并批次号。
        /// </summary>
        [DataMember]
        public virtual string ChildLotNumber { get; set; }
        /// <summary>
        /// 数量。
        /// </summary>
        [DataMember]
        public virtual double Quantity { get; set; }
       
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
