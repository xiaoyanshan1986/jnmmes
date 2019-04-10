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
    /// 批次退料操作事务数据主键。
    /// </summary>
    public struct LotTransactionReturnKey
    {
        /// <summary>
        /// 事务操作主键。
        /// </summary>
        public string TransactionKey { get; set; }
        /// <summary>
        /// 代码组名称。
        /// </summary>
        public string ReasonCodeCategoryName { get; set; }
        /// <summary>
        /// 代码名称。
        /// </summary>
        public string ReasonCodeName { get; set; }
    }
    /// <summary>
    /// 描述批次退料操作事务数据的模型类。
    /// </summary>
    [DataContract]
    public class LotTransactionReturn : BaseModel<LotTransactionReturnKey>
    {
        /// <summary>
        /// 数量。
        /// </summary>
        [DataMember]
        public virtual double Quantity { get; set; }
        /// <summary>
        /// 描述。
        /// </summary>
        [DataMember]
        public virtual string Description { get; set; }
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
