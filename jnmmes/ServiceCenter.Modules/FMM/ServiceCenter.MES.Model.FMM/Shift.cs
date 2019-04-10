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
    /// 描述班别的数据模型类。
    /// </summary>
    [DataContract]
    public class Shift : BaseModel<string>
    {
        /// <summary>
        /// 主键(班别名称)。
        /// </summary>
        [DataMember]
        public override string Key
        {
            get;
            set;
        }
        /// <summary>
        /// 班别开始时间。08:00
        /// </summary>
        [DataMember]
        public virtual string StartTime { get; set; }
        /// <summary>
        /// 班别结束时间。20:00
        /// </summary>
        [DataMember]
        public virtual string EndTime { get; set; }
        /// <summary>
        /// 是否跨天。
        /// </summary>
        [DataMember]
        public virtual bool IsOverDay { get; set; }
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
    }
}
