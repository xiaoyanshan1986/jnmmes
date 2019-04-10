using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;

namespace ServiceCenter.MES.Model.QAM
{
    /// <summary>
    /// 检验设置点明细主键。
    /// </summary>
    public struct CheckSettingPointDetailKey
    {
        /// <summary>
        /// 检验设置主键。
        /// </summary>
        public string CheckSettingKey { get; set; }
        /// <summary>
        /// 项目号。
        /// </summary>
        public int ItemNo { get; set; }
        /// <summary>
        /// 参数名称。
        /// </summary>
        public string ParameterName { get; set; }

        public override string ToString()
        {
            return string.Format("{0}:{1}", this.ItemNo, this.ParameterName);
        }
    }

    /// <summary>
    /// 描述检验设置点明细的模型类。
    /// </summary>
    [DataContract]
    public class CheckSettingPointDetail : BaseModel<CheckSettingPointDetailKey>
    {
        /// <summary>
        /// 参数项目号。
        /// </summary>
        [DataMember]
        public virtual int ParameterItemNo { get; set; }
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
