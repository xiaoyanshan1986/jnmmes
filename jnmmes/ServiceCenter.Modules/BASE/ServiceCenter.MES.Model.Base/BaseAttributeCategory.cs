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
    /// 表示基础属性分类数据。
    /// </summary>
    [DataContract]
    public class BaseAttributeCategory : BaseModel<string>
    {
        /// <summary>
        /// 主键（分类名称）
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
