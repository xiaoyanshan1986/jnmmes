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
    /// 表示日排班计划的数据主键。
    /// </summary>
    public struct ScheduleDayKey
    {
        /// <summary>
        /// 车间名称
        /// </summary>
        public string LocationName { get; set; }
        /// <summary>
        /// 工序名称
        /// </summary>
        public string RouteOperationName { get; set; }
        /// <summary>
        /// 日期。
        /// </summary>
        public DateTime Day { get; set; }
        /// <summary>
        /// 班别名称。
        /// </summary>
        [DataMember]
        public string ShiftName { get; set; }
        public override string ToString()
        {
            return string.Format("{0}{1}({2:yyyy-MM-dd}:{3})", this.LocationName, this.RouteOperationName, this.Day, this.ShiftName);
        }
    }

    /// <summary>
    /// 描述日排班计划的数据模型类。
    /// </summary>
    [DataContract]
    public class ScheduleDay : BaseModel<ScheduleDayKey>
    {
        /// <summary>
        /// 班次。
        /// </summary>
        [DataMember]
        public virtual string ShiftValue { get; set; }
        /// <summary>
        /// 序号。
        /// </summary>
        [DataMember]
        public virtual int SeqNo { get; set; }
        /// <summary>
        /// 班别开始时间。
        /// </summary>
        [DataMember]
        public virtual DateTime StartTime { get; set; }
        /// <summary>
        /// 班别结束时间。
        /// </summary>
        [DataMember]
        public virtual DateTime EndTime { get; set; }

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
