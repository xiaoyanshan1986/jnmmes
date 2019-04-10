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
    /// 批次不良操作事务数据主键。
    /// </summary>
    public struct LotTransactionDefectPosKey
    {
        /// <summary>
        /// 事务操作主键。
        /// </summary>
        public string TransactionKey { get; set; }
        /// <summary>
        /// x。
        /// </summary>
        public string PosX { get; set; }
        /// <summary>
        /// y。
        /// </summary>
        public string PosY { get; set; }
    }
    /// <summary>
    /// 描述批次不良操作事务数据的模型类。
    /// </summary>
    [DataContract]
    public class LotTransactionDefectPos : BaseModel<LotTransactionDefectPosKey>
    {
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
