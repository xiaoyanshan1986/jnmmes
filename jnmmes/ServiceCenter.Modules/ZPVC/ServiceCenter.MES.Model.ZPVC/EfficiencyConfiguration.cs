using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;

namespace ServiceCenter.MES.Model.ZPVC
{
    /// <summary>
    /// 表示效率档配置主键。
    /// </summary>
    public struct EfficiencyConfigurationKey
    {
        /// <summary>
        /// 效率档配置组名。
        /// </summary>
        public string Group { get; set; }
        /// <summary>
        /// 效率档配置代码。
        /// </summary>
        public string Code { get; set; }

        public override string ToString()
        {
            return string.Format("{0}:{1}", this.Group, this.Code);
        }
    }
    /// <summary>
    /// 描述效率档配置数据模型
    /// </summary>
    [DataContract]
    public class EfficiencyConfiguration : BaseModel<EfficiencyConfigurationKey>
    {
        /// <summary>
        /// 效率代码。
        /// </summary>
        [DataMember]
        public virtual string Code { get; set; }
        /// <summary>
        /// 效率名称。
        /// </summary>
        [DataMember]
        public virtual string Name { get; set; }
        /// <summary>
        /// 效率最小值。
        /// </summary>
        [DataMember]
        public virtual double? Lower { get; set; }
        /// <summary>
        /// 效率最大值。
        /// </summary>
        [DataMember]
        public virtual double? Upper { get; set; }
        /// <summary>
        /// 等级。
        /// </summary>
        [DataMember]
        public virtual string Grade { get; set; }
        /// <summary>
        /// 花色。
        /// </summary>
        [DataMember]
        public virtual string Color { get; set; }
        /// <summary>
        /// 对应的物料号。
        /// </summary>
        [DataMember]
        public virtual string MaterialCode { get; set; }
        /// <summary>
        /// 描述。
        /// </summary>
        [DataMember]
        public virtual string Description { get; set; }
        /// <summary>
        /// 是否可用。
        /// </summary>
        [DataMember]
        public virtual bool IsUsed { get; set; }
        /// <summary>
        /// 创建人。
        /// </summary>
        [DataMember]
        public virtual  string Creator { get; set; }
        /// <summary>
        /// 创建时间。
        /// </summary>
        [DataMember]
        public virtual  DateTime CreateTime { get; set; }
        /// <summary>
        /// 编辑人。
        /// </summary>
        [DataMember]
        public virtual  string Editor { get; set; }
        /// <summary>
        /// 编辑时间。
        /// </summary>
        [DataMember]
        public virtual  DateTime EditTime { get; set; }

    }
}
