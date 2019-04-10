using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;
using System.ComponentModel.DataAnnotations;
using ServiceCenter.MES.Model.FMM.Resources;

namespace ServiceCenter.MES.Model.FMM
{
    
    /// <summary>
    /// 描述参数的数据模型类。
    /// </summary>
    [DataContract]
    public class Parameter : BaseModel<string>
    {
        public Parameter()
        {
            this.Status = EnumObjectStatus.Available;
        }
        /// <summary>
        /// 主键(参数名称）。
        /// </summary>
        [DataMember]
        public override string Key
        {
            get;
            set;
        }
        /// <summary>
        /// 类别。
        /// </summary>
        [DataMember]
        public virtual EnumParameterType Type { get; set; }
        /// <summary>
        /// 数据类型。
        /// </summary>
        [DataMember]
        public virtual EnumDataType DataType { get; set; }
        /// <summary>
        /// 设备类型。
        /// </summary>
        [DataMember]
        public virtual EnumDeviceType DeviceType { get; set; }
        /// <summary>
        /// 必须输入。
        /// </summary>
        [DataMember]
        public virtual bool Mandatory { get; set; }
        /// <summary>
        /// 是否是衍生推导参数。
        /// </summary>
        [DataMember]
        public virtual bool IsDerived { get; set; }
        /// <summary>
        /// 衍生推导公式。
        /// </summary>
        [DataMember]
        public virtual string DerivedFormula { get; set; }
        /// <summary>
        /// 描述。
        /// </summary>
        [DataMember]
        public virtual string Description { get; set; }
        /// <summary>
        /// 状态。
        /// </summary>
        [DataMember]
        public virtual EnumObjectStatus Status { get; set; }

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
