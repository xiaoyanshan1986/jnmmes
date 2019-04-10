using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;

namespace ServiceCenter.MES.Model.EMS
{
    /// <summary>
    /// 描述设备状态事件的模型类
    /// </summary>
    [DataContract]
    public class EquipmentStateEvent : BaseModel<string>
    {
        /// <summary>
        /// 主键
        /// </summary>
        [DataMember]
        public override string Key
        {
            get;
            set;
        }

        /// <summary>
        /// 设备代码
        /// </summary>
        [DataMember]
        public virtual string EquipmentCode { get; set; }

        /// <summary>
        /// 设备状态切换名称
        /// </summary>
        [DataMember]
        public virtual string EquipmentChangeStateName { get; set; }

        /// <summary>
        /// 设备切换前状态名称。
        /// </summary>
        [DataMember]
        public virtual string EquipmentFromStateName { get; set; }

        /// <summary>
        /// 设备切换到状态名称
        /// </summary>
        [DataMember]
        public virtual string EquipmentToStateName { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [DataMember]
        public virtual string Description { get; set; }

        /// <summary>
        /// 是否当前状态
        /// </summary>
        [DataMember]
        public virtual bool IsCurrent { get; set; }
        
        /// <summary>
        /// 描述
        /// </summary>
        [DataMember]
        public virtual string EndEventKey { get; set; }
        
         /// <summary>
        ///事件开始时间
        /// </summary>
        [DataMember]
        public virtual DateTime? StartTime { get; set; }
        
        /// <summary>
        /// 事件结束时间
        /// </summary>
        [DataMember]
        public virtual DateTime? EndTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [DataMember]
        public virtual string Creator { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMember]
        public virtual DateTime? CreateTime { get; set; }

        /// <summary>
        /// 编辑人
        /// </summary>
        [DataMember]
        public virtual string Editor { get; set; }

        /// <summary>
        /// 编辑时间
        /// </summary>
        [DataMember]
        public virtual DateTime? EditTime { get; set; }

        /// <summary>
        /// 原因代码组
        /// </summary>
         [DataMember]
        public virtual string ReasonCodeCategoryName { get; set; }

        /// <summary>
        /// 原因代码
        /// </summary>
        [DataMember]
        public virtual string ReasonCodeName { get; set; }
    }
}
