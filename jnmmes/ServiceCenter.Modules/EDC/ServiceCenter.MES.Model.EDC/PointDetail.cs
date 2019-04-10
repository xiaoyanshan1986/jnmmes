using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;

namespace ServiceCenter.MES.Model.EDC
{
    /// <summary>
    /// 采集点设置明细主键。
    /// </summary>
    public struct PointDetailKey
    {
        /// <summary>
        /// 采集点设置主键。
        /// </summary>
        public string PointKey { get; set; }
        /// <summary>
        /// 参数名称。
        /// </summary>
        public string ParameterName { get; set; }

        public override string ToString()
        {
            return string.Format("{0}:{1}", this.PointKey, this.ParameterName);
        }
    }

    /// <summary>
    /// 描述采集点设置明细的模型类。
    /// </summary>
    [DataContract]
    public class PointDetail : BaseModel<PointDetailKey>
    {
        /// <summary>
        /// 主键。
        /// </summary>
        [DataMember]
        public override PointDetailKey Key
        {
            get;
            set;
        }
        /// <summary>
        /// 项目号。
        /// </summary>
        [DataMember]
        public virtual int ItemNo { get; set; }
        /// <summary>
        /// 参数值。
        /// </summary>
        [DataMember]
        public virtual int ParameterCount{ get; set; }
        /// <summary>
        /// 参数类型。
        /// </summary>
        [DataMember]
        public virtual EnumParameterType ParameterType { get; set; }
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
        /// 是否必输。
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
        /// 上范围值。
        /// </summary>
        [DataMember]
        public virtual double? UpperBoundary { get; set; }
        /// <summary>
        /// 上规格值。
        /// </summary>
        [DataMember]
        public virtual double? UpperSpecification { get; set; }
        /// <summary>
        /// 上控制值。
        /// </summary>
        [DataMember]
        public virtual double? UpperControl { get; set; }
        /// <summary>
        /// 目标值。
        /// </summary>
        [DataMember]
        public virtual double? Target { get; set; }
        /// <summary>
        /// 下控制值。
        /// </summary>
        [DataMember]
        public virtual double? LowerControl { get; set; }
        /// <summary>
        /// 下规格值。
        /// </summary>
        [DataMember]
        public virtual double? LowerSpecification { get; set; }
        /// <summary>
        /// 下范围值。
        /// </summary>
        [DataMember]
        public virtual double? LowerBoundary { get; set; }
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
