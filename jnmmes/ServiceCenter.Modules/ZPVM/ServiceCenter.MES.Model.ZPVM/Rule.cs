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
    /// 描述规则数据模型
    /// </summary>
    [DataContract]
    public class Rule : BaseModel<string>
    {
        /// <summary>
        /// 主键，规则代码。
        /// </summary>
        [DataMember]
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
        /// 规则名称。
        /// </summary>
        [DataMember]
        public virtual string Name { get; set; }
        /// <summary>
        /// 分档代码。
        /// </summary>
        [DataMember]
        public virtual string PowersetCode { get; set; }
        /// <summary>
        /// 满包数量。
        /// </summary>
        [DataMember]
        public virtual double FullPackageQty { get; set; }
        /// <summary>
        /// 功率精度。
        /// </summary>
        [DataMember]
        public virtual int PowerDegree { get; set; }
        /// <summary>
        /// 最小功率。
        /// </summary>
        [DataMember]
        public virtual double? MinPower { get; set; }
        /// <summary>
        /// 最大功率。
        /// </summary>
        [DataMember]
        public virtual double? MaxPower { get; set; }

        /// <summary>
        /// 校准板类型。
        /// </summary>
        [DataMember]
        public virtual string CalibrationType { get; set; }
        /// <summary>
        /// 校准周期（分钟数）。
        /// </summary>
        [DataMember]
        public virtual double? CalibrationCycle { get; set; }
        /// <summary>
        /// 固化周期（分钟数）。
        /// </summary>
        [DataMember]
        public virtual double? FixCycle { get; set; }

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
