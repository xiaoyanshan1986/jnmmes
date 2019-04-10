using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;

namespace ServiceCenter.MES.Model.FMM
{
    /// <summary>
    /// 表示原因代码组数据。
    /// </summary>
    [DataContract]
    public class EquipmentReasonCodeCategory : BaseModel<string>
    {
        /// <summary>
        /// 主键（原因代码分组名称）。
        /// </summary>
        [DataMember]
        public override string Key
        {
            get;
            set;
        }
        /// <summary>
        /// 类型。 
        /// </summary>
        [DataMember]
        public virtual EnumEquipmentReasonCodeType Type { get; set; }

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
        public virtual DateTime CreateTime { get; set; }

        /// <summary>
        /// 编辑人。
        /// </summary>
        [DataMember]
        public virtual string Editor { get; set; }

        /// <summary>
        /// 编辑时间。
        /// </summary>
        [DataMember]
        public virtual DateTime EditTime { get; set; }

    }
}
