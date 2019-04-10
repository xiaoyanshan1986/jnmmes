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
    /// 批次补片操作事务数据主键。
    /// </summary>
    public struct LotTransactionPatchKey
    {
        /// <summary>
        /// 批次的事务操作主键。
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
    /// 描述批次补片操作事务数据的模型类。
    /// </summary>
    [DataContract]
    public class LotTransactionPatch : BaseModel<LotTransactionPatchKey>
    {
        /// <summary>
        /// 线边仓名称。
        /// </summary>
        [DataMember]
        public virtual string LineStoreName { get; set; }
        /// <summary>
        /// 生产线别。
        /// </summary>
        [DataMember]
        public virtual string LineCode { get; set; }
        /// <summary>
        /// 物料编码。
        /// </summary>
        [DataMember]
        public virtual string MaterialCode { get; set; }
        /// <summary>
        /// 物料批次号。
        /// </summary>
        [DataMember]
        public virtual string MaterialLot { get; set; }
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
        /// 责任工序。
        /// </summary>
        [DataMember]
        public virtual string RouteOperationName { get; set; }
        /// <summary>
        /// 责任人。
        /// </summary>
        [DataMember]
        public virtual string ResponsiblePerson { get; set; }
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
