using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;

namespace ServiceCenter.MES.Model.BaseData
{
    
    /// <summary>
    /// 表示基础分段编码数据。
    /// </summary>
    [DataContract]
    public class BasePartCodeRule : BaseModel<string>
    {
        /// <summary>
        /// 主键（分段编码）
        /// </summary>
        public override string Key
        {
            get
            {
                return base.Key;
            }
            set
            {
                base.Key = value;
            }
        }

        /// <summary>
        /// 分段编码名称。
        /// </summary>
        [DataMember]
        public virtual string PartCodeName { get; set; }

        /// <summary>
        /// 分段编码值类型。
        /// </summary>
        [DataMember]
        public virtual string PartCodeValueType { get; set; }

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
