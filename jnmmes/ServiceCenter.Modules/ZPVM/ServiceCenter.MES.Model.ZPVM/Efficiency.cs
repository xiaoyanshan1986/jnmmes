using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;
using System.ComponentModel.DataAnnotations;
using ServiceCenter.MES.Model.ZPVM.Resources;

namespace ServiceCenter.MES.Model.ZPVM
{
    /// <summary>
    /// 表示效率档系数主键。
    /// </summary>
    public struct EfficiencyKey
    {
        /// <summary>
        /// 效率档组名。
        /// </summary>
        public string Group { get; set; }
        /// <summary>
        /// 效率档代码。
        /// </summary>
        public string Code { get; set; }

        public override string ToString()
        {
            return string.Format("{0}:{1}", this.Group, this.Code);
        }
    }
    /// <summary>
    /// 描述效率档系数数据模型
    /// </summary>
    [DataContract]
    public class Efficiency : BaseModel<EfficiencyKey>
    {
        /// <summary>
        /// 效率档名称。
        /// </summary>
        [DataMember]
        public virtual string Name { get; set; }
        /// <summary>
        /// 最小值。
        /// </summary>
        [DataMember]
        public virtual double? Lower { get; set; }
        /// <summary>
        /// 最大值。
        /// </summary>
        [DataMember]
        public virtual double? Upper { get; set; }
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
