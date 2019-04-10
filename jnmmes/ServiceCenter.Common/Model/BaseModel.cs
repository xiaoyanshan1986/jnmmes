using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ServiceCenter.Model
{
    /// <summary>
    /// 数据模型基类。
    /// </summary>
    /// <typeparam name="K">数据模型主键类型。</typeparam>
    [DataContract(Name = "BaseModel_{0}")]
    public abstract class BaseModel<K>:IExtensibleDataObject,ICloneable
    {
        ExtensionDataObject IExtensibleDataObject.ExtensionData { get; set; }
        /// <summary>
        /// 主键。
        /// </summary>
        [DataMember]
        public virtual K Key { get; set; }

        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
