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
    /// 物料属性主键类型。
    /// </summary>
    public struct MaterialAttributeKey
    {
        /// <summary>
        /// 物料编码。
        /// </summary>
        public string MaterialCode { get; set; }
        /// <summary>
        /// 属性名称。
        /// </summary>
        public string AttributeName{ get; set; }

        public override string ToString()
        {
            return string.Format("{0}：{1}", this.MaterialCode, this.AttributeName);
        }
    }
    /// <summary>
    /// 物料属性数据。
    /// </summary>
    [DataContract]
    public class MaterialAttribute : BaseModel<MaterialAttributeKey>
    {
        /// <summary>
        /// 属性值。
        /// </summary>
        [DataMember]
        public virtual string Value { get; set; }
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
