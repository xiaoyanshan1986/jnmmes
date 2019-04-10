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
    /// 表示某月排班计划的数据主键。
    /// </summary>
    public struct ScheduleMonthKey
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
        /// 年份。
        /// </summary>
        public string Year { get; set; }
        /// <summary>
        /// 月份。
        /// </summary>
        public string Month { get; set; }


        public override string ToString()
        {
            return string.Format("{0}{1}({2}-{3})",this.LocationName,this.RouteOperationName, Year, Month);
        }
    }

    /// <summary>
    /// 描述某月排班计划的数据模型类。
    /// </summary>
    [DataContract]
    public class ScheduleMonth : BaseModel<ScheduleMonthKey>
    {
        /// <summary>
        /// 排班计划名称。
        /// </summary>
        [DataMember]
        public virtual string ScheduleName{ get; set; }
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
