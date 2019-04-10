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
    /// 基础属性数据值主键。
    /// </summary>
    public struct BaseAttributeValueKey
    {
        /// <summary>
        /// 基础属性分类名称。
        /// </summary>
        public string CategoryName { get; set; }
        /// <summary>
        /// 基础属性名称。
        /// </summary>
        public string AttributeName { get; set; }
        /// <summary>
        /// 基础属性数据值行号。
        /// </summary>
        public int ItemOrder { get; set; }

        public override string ToString()
        {
            return string.Format("{0}:{1}:{2}", this.CategoryName, this.AttributeName, this.ItemOrder);
        }
    }
    /// <summary>
    /// 表示基础属性数据值。
    /// </summary>
    [DataContract]
    public class BaseAttributeValue : BaseModel<BaseAttributeValueKey>
    {
        /// <summary>
        /// 基础属性数据值。
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
