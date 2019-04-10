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
    /// 表示基础编码规则主键。
    /// </summary>
    public struct BaseCodeRuleKey
    {
        /// <summary>
        /// 基础编码类型。
        /// </summary>
        public string CodeType { get; set; }

        /// <summary>
        /// 基础编码代码。
        /// </summary>
        public string CodeCode { get; set; }

        /// <summary>
        /// 基础序号。
        /// </summary>
        public int ItemNo { get; set; }

        public override string ToString()
        {
            return string.Format("{0}:{1}:{2}", this.CodeType, this.CodeCode, this.ItemNo);
        }
    }
    /// <summary>
    /// 表示基础编码内容数据。
    /// </summary>
    [DataContract]
    public class BaseCodeRule : BaseModel<BaseCodeRuleKey>
    {
        /// <summary>
        /// 编码名称。
        /// </summary>
        [DataMember]
        public virtual string CodeName { get; set; }

        /// <summary>
        /// 分段编码。
        /// </summary>
        [DataMember]
        public virtual string PartCode { get; set; }

        /// <summary>
        /// 分段编码值。
        /// </summary>
        [DataMember]
        public virtual string PartValue { get; set; }

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
