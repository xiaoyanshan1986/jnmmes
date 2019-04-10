using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;

namespace ServiceCenter.MES.Model.LSM
{
    /// <summary>
    /// 描述下料数据的模型类。
    /// </summary>
    [DataContract]
    public class MaterialUnloading : BaseModel<string>
    {
        public MaterialUnloading()
        {
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;
        }

        /// <summary>
        /// 主键。
        /// </summary>
        public override string Key 
        { 
            get; 
            set; 
        }
        /// <summary>
        /// 工序名称。
        /// </summary>
        [DataMember]
        public virtual string RouteOperationName
        {
            get;
            set;
        }
        /// <summary>
        /// 线别代码。
        /// </summary>
        [DataMember]
        public virtual string ProductionLineCode
        {
            get;
            set;
        }
        /// <summary>
        /// 设备代码。
        /// </summary>
        [DataMember]
        public virtual string EquipmentCode
        {
            get;
            set;
        }
        /// <summary>
        /// 下料时间。
        /// </summary>
        [DataMember]
        public virtual DateTime UnloadingTime { get; set; }

        /// <summary>
        /// 下料操作人。
        /// </summary>
        [DataMember]
        public virtual string Operator { get; set; }

        /// <summary>
        /// 描述。
        /// </summary>
        [DataMember]
        public virtual string Description { get; set; }
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
