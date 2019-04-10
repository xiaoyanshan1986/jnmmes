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
    /// 表示某月排班计划的数据主键。
    /// </summary>
    public struct DefectKey
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
        /// 日期
        /// </summary>
        public string Day { get; set; }

        /// <summary>
        /// 车间
        /// </summary>
        public string LocationName { get; set; }

        /// <summary>
        /// 班别
        /// </summary>
        public string ShiftName { get; set; }

        /// <summary>
        /// 不良组
        /// </summary>
        public string ReasonCodeCategoryName { get; set; }

        /// <summary>
        /// 不良原因
        /// </summary>
        public string ReasonCodeName { get; set; }

        public override string ToString()
        {
            return string.Format("{0}-{1}-{2} ,{3}车间, {4}班", Year, Month, Day, LocationName, ShiftName);
        }
    }
    public class Defect : BaseModel<DefectKey>
    {
        /// <summary>
        /// 数量
        /// </summary>
        public virtual double Qty { get; set; }

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
