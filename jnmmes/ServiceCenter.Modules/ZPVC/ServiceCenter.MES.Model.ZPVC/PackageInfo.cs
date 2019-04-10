using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;
using System.ComponentModel.DataAnnotations;

namespace ServiceCenter.MES.Model.ZPVC
{
    /// <summary>
    /// 描述包装数据的模型类。
    /// </summary>
    [DataContract]
    public class PackageInfo : BaseModel<string>
    {
        /// <summary>
        /// 主键（包装号）
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
        /// 配置组名。
        /// </summary>
        [DataMember]
        public virtual string ConfigGroup { get; set; }
        /// <summary>
        /// 分类编码。
        /// </summary>
        [DataMember]
        public virtual string ConfigCode { get; set; }
        /// <summary>
        /// 效率名称。
        /// </summary>
        [DataMember]
        public virtual string EfficiencyName { get; set; }
        /// <summary>
        /// 效率最小值。
        /// </summary>
        [DataMember]
        public virtual double? EfficiencyLower { get; set; }
        /// <summary>
        /// 效率最大值。
        /// </summary>
        [DataMember]
        public virtual double? EfficiencyUpper { get; set; }
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
        /// 生产线代码。
        /// </summary>
        [DataMember]
        public virtual string LineCode { get; set; }

        /// <summary>
        /// PN类型。
        /// </summary>
        [DataMember]
        public virtual string PNType { get; set; }

        /// <summary>
        /// 产品编号。
        /// </summary>
        [DataMember]
        public virtual string ProductId { get; set; }
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
        /// <summary>
        /// 创建人。
        /// </summary>
        [DataMember]
        public virtual string Creator { get; set; }
        /// <summary>
        /// 创建时间。
        /// </summary>
        [DataMember]
        public virtual DateTime? CreateTime { get; set; }
        


    }
}
