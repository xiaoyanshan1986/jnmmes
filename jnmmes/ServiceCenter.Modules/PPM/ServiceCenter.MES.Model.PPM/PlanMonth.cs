using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;
using System.ComponentModel.DataAnnotations;
using ServiceCenter.MES.Model.PPM.Resources;

namespace ServiceCenter.MES.Model.PPM
{
    /// <summary>
    /// 表示某月生产计划的数据主键。
    /// </summary>
    public struct PlanMonthKey
    {
        /// <summary>
        /// 年份
        /// </summary>
        public string Year { get; set; }
        /// <summary>
        /// 月份
        /// </summary>
        public string Month { get; set; }
        /// <summary>
        /// 车间
        /// </summary>
        public string LocationName { get; set; }
        
        public override string ToString()
        {
            return string.Format("{0}-{1}-{2}", Year, Month, LocationName);
        }
    }
    public class PlanMonth : BaseModel<PlanMonthKey>
    {
        /// <summary>
        /// 计划数量
        /// </summary>
        public virtual string PlanQty { get; set; }
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
