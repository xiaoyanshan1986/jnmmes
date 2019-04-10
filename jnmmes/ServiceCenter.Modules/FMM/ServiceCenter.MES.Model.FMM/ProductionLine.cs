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
    /// 表示生产线别数据。
    /// </summary>
    [DataContract]
    public class ProductionLine : BaseModel<string>
    {
        /// <summary>
        ///主键（生产线别代码）。
        /// </summary>
        [DataMember]
        public override string Key { get; set; }
        /// <summary>
        /// 线别名称。
        /// </summary>
        [DataMember]
        public virtual string Name { get; set; }
        /// <summary>
        /// 区域名称。
        /// </summary>
        [DataMember]
        public virtual string LocationName { get; set; }
        /// <summary>
        /// 线别描述。
        /// </summary>
        [DataMember]
        public virtual string Description { get; set; }
        /// <summary>
        /// 属性1
        /// </summary>
        [DataMember]
        public virtual string Attr1 { get; set; }
        /// <summary>
        /// 属性2
        /// </summary>
        [DataMember]
        public virtual string Attr2 { get; set; }
        /// <summary>
        /// 属性3
        /// </summary>
        [DataMember]
        public virtual string Attr3 { get; set; }
        /// <summary>
        /// 属性4
        /// </summary>
        [DataMember]
        public virtual string Attr4 { get; set; }
        /// <summary>
        /// 属性5
        /// </summary>
        [DataMember]
        public virtual string Attr5 { get; set; }
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
