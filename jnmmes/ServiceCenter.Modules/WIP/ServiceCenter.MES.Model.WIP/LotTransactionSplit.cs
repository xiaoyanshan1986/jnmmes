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
    /// 批次拆分操作事务数据主键。
    /// </summary>
    public struct LotTransactionSplitKey
    {
        /// <summary>
        /// 拆分批次的事务操作主键。
        /// </summary>
        public string TransactionKey { get; set; }
        /// <summary>
        /// 子批次的事务操作主键。
        /// </summary>
        public string ChildTransactionKey { get; set; }
    }
    /// <summary>
    /// 描述批次拆分操作事务数据的模型类。
    /// </summary>
    [DataContract]
    public class LotTransactionSplit : BaseModel<LotTransactionSplitKey>
    {
        /// <summary>
        /// 批次号。
        /// </summary>
        [DataMember]
        public virtual string MainLotNumber { get; set; }
        /// <summary>
        /// 子批次号。
        /// </summary>
        [DataMember]
        public virtual string ChildLotNumber { get; set; }
        /// <summary>
        /// 拆分数量。
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
