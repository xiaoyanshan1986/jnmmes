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
    /// 批次参数主键。
    /// </summary>
    public struct LotTransactionParameterKey
    {
        /// <summary>
        /// 操作事务主键。
        /// </summary>
        public string TransactionKey { get; set; }
        /// <summary>
        /// 参数名称。
        /// </summary>
        public string ParameterName { get; set; }
        /// <summary>
        /// 项目号。
        /// </summary>
        public int ItemNo { get; set; }
    }
    /// <summary>
    /// 描述批次参数数据的模型类。
    /// </summary>
    [DataContract]
    public class LotTransactionParameter : BaseModel<LotTransactionParameterKey>
    {
        /// <summary>
        /// 参数值。
        /// </summary>
        [DataMember]
        public virtual string ParameterValue { get; set; }
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
