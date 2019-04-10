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
    /// 表示规则-衰减设置主键。
    /// </summary>
    public struct RuleDecayKey
    {
        /// <summary>
        /// 规则代码。
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 最小功率值。
        /// </summary>
        public double MinPower { get; set; }
        /// <summary>
        /// 最大功率值。
        /// </summary>
        public double MaxPower { get; set; }

        public override string ToString()
        {
            return string.Format("{0}({1}-{2})", this.Code, this.MinPower, this.MaxPower);
        }
    }
    /// <summary>
    /// 描述规则-衰减系数设置的数据模型
    /// </summary>
    [DataContract]
    public class RuleDecay : BaseModel<RuleDecayKey>
    {
        /// <summary>
        /// 衰减系数代码。
        /// </summary>
        [DataMember]
        public virtual string DecayCode { get; set; }
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
