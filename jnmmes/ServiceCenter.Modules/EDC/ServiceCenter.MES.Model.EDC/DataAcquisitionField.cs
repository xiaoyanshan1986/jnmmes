using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;
using System.Runtime.Serialization;

namespace ServiceCenter.MES.Model.EDC
{   
    /// <summary>
    /// 表示项目字段主键
    /// </summary>
    public struct DataAcquisitionFieldKey
    {
        /// <summary>
        /// 采集项目代码
        /// </summary>
        public string ItemCode { get; set; }

        /// <summary>
        /// 采集字段
        /// </summary>
        public string FieldCode { get; set; }

        public override string ToString()
        {
            return string.Format("{0}：{1}", this.ItemCode, this.FieldCode);
        }
    }

    /// <summary>
    /// 表示采集字段数据
    /// </summary>
    [DataContract]
    public class DataAcquisitionField : BaseModel<DataAcquisitionFieldKey>
    {      
        /// <summary>
        /// 字段说明
        /// </summary>
        [DataMember]
        public virtual string FieldName { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        [DataMember]
        public virtual int SerialNumber { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        [DataMember]
        public virtual EnumDataType DataType { get; set; }

        /// <summary>
        /// 是否主键
        /// </summary>
        [DataMember]
        public virtual bool IsKEY { get; set; }

        /// <summary>
        /// 范围控制
        /// </summary>
        [DataMember]
        public virtual bool IsControl { get; set; }

        /// <summary>
        /// 控制上限
        /// </summary>
        [DataMember]
        public virtual decimal MaxLine { get; set; }

        /// <summary>
        /// 控制下限
        /// </summary>
        [DataMember]
        public virtual decimal MinLine { get; set; }

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
