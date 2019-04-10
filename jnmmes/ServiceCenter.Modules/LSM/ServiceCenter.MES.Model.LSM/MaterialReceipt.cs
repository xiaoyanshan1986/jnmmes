using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;

namespace ServiceCenter.MES.Model.LSM
{
    /// <summary>
    /// 领料单类型。
    /// </summary>
    public enum EnumReceiptType
    {
        /// <summary>
        /// 正常。
        /// </summary>
        Normal=0
    }

    /// <summary>
    /// 领料状态。
    /// </summary>
    public enum EnumReceiptState
    {
        /// <summary>
        /// 已创建。
        /// </summary>
        Created = 0,
        /// <summary>
        /// 已审核。
        /// </summary>
        Approved=10
    }

    /// <summary>
    /// 描述领料单数据的模型类。
    /// </summary>
    [DataContract]
    public class MaterialReceipt : BaseModel<string>
    {
        public MaterialReceipt()
        {
            this.State = EnumReceiptState.Approved;
            this.Type = EnumReceiptType.Normal;
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;
        }
        /// <summary>
        /// 主键（领料单号）。
        /// </summary>
        public override string Key 
        { 
            get; 
            set; 
        }
        /// <summary>
        /// 领料单类型。
        /// </summary>
        [DataMember]
        public virtual EnumReceiptType Type
        {
            get;
            set;
        }
        /// <summary>
        /// 领料日期。
        /// </summary>
        [DataMember]
        public virtual DateTime ReceiptDate
        {
            get;
            set;
        }
        /// <summary>
        /// 领料单状态。
        /// </summary>
        [DataMember]
        public virtual EnumReceiptState State
        {
            get;
            set;
        }
        /// <summary>
        /// 工单号。
        /// </summary>
        [DataMember]
        public virtual string OrderNumber { get; set; }
        /// <summary>
        /// 描述。
        /// </summary>
        [DataMember]
        public virtual string Description { get; set; }
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
        /// 线边仓。
        /// </summary>
        [DataMember]
        public virtual string LineStore { get; set; }
    }
}
