using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;

namespace ServiceCenter.MES.Model.FMM
{
    /// <summary>
    /// 表示设备布局数据模型类。
    /// </summary>
    [DataContract]
    public class EquipmentLayout : BaseModel<string>
    {
        /// <summary>
        /// 主键（设备布局名称）。
        /// </summary>
        [DataMember]
        public override string Key
        {
            get;
            set;
        }
        /// <summary>
        /// 宽度。
        /// </summary>
        [DataMember]
        public virtual int Width { get; set; }
        /// <summary>
        /// 高度。
        /// </summary>
        [DataMember]
        public virtual int Height { get; set; }
        /// <summary>
        /// 背景图片。
        /// </summary>
        [DataMember]
        public virtual byte[] BackgroundImage { get; set; }
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
