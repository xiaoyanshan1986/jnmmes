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
    /// 退料单类型。
    /// </summary>
    public enum EnumReturnType
    {
        /// <summary>
        /// 正常。
        /// </summary>
        Normal=0
    }

    /// <summary>
    /// 退料单状态。
    /// </summary>
    public enum EnumReturnState
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
    /// 描述退料单数据的模型类。
    /// </summary>
    [DataContract]
    public class MaterialReturn : BaseModel<string>
    {
        public MaterialReturn()
        {
            this.State = EnumReturnState.Created;
            this.Type = EnumReturnType.Normal;
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;
        }
        /// <summary>
        /// 主键（退料单号）。
        /// </summary>
        public override string Key 
        { 
            get; 
            set; 
        }
        /// <summary>
        /// 退料单类型。
        /// </summary>
        [DataMember]
        public virtual EnumReturnType Type
        {
            get;
            set;
        }
        /// <summary>
        /// 退料单日期。
        /// </summary>z
        [DataMember]
        public virtual DateTime ReturnDate
        {
            get;
            set;
        }
        /// <summary>
        /// 退料单状态。
        /// </summary>
        [DataMember]
        public virtual EnumReturnState State
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
        /// ERP回填单号
        /// </summary>
        [DataMember]
        public virtual string ErpCode { get; set; }
        /// <summary>
        /// 仓库
        /// </summary>
        [DataMember]
        public virtual string Store { get; set; }
    }
}
