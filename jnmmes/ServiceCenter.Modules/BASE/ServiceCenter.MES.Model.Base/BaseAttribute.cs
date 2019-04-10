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
    /// 表示基础属性主键。
    /// </summary>
    public struct BaseAttributeKey
    {
        /// <summary>
        /// 基础属性分类名称。
        /// </summary>
        public string CategoryName { get; set; }
        /// <summary>
        /// 基础属性名称。
        /// </summary>
        public string AttributeName { get; set; }

        public override string ToString()
        {
            return string.Format("{0}：{1}", this.CategoryName, this.AttributeName);
        }
    }
    /// <summary>
    /// 表示基础属性数据。
    /// </summary>
    [DataContract]
    public class BaseAttribute : BaseModel<BaseAttributeKey>
    {
        /// <summary>
        /// 描述。
        /// </summary>
        [DataMember]
        public virtual string Description { get; set; }
        /// <summary>
        /// 数据类型。
        /// </summary>
        [DataMember]
        public virtual EnumDataType DataType { get; set; }
        /// <summary>
        /// 排序序号。
        /// </summary>
        [DataMember]
        public virtual int Order { get; set; }
        /// <summary>
        /// 是否为主键。
        /// </summary>
        [DataMember]
        public virtual bool IsPrimaryKey { get; set; }
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
