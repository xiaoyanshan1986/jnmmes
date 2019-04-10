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
    /// 表示排班计划详细数据的主键
    /// </summary>
    public struct ScheduleDetailKey
    {
        /// <summary>
        /// 排版计划名称。
        /// </summary>
        public string ScheduleName { get; set; }
        /// <summary>
        /// 班别名称。
        /// </summary>
        public string ShiftName { get; set; }

        public override string ToString()
        {
            return string.Format("{0}:{1}", this.ScheduleName, this.ShiftName);
        }
    }

    /// <summary>
    /// 描述排班计划详细数据的数据模型类。
    /// </summary>
    [DataContract]
    public class ScheduleDetail : BaseModel<ScheduleDetailKey>
    {
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
