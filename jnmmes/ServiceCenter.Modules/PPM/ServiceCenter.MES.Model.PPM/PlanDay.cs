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
    public struct PlanDayKey
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
        /// 产品类型
        /// </summary>
        public string ProductCode { get; set; }

        public override string ToString()
        {
            return string.Format("{0}-{1}-{2} ,{3}车间,产品:{4}", Year, Month, Day, LocationName, ProductCode);
        }
    }
    public class PlanDay : BaseModel<PlanDayKey>
    {
        /// <summary>
        /// 计划产出量
        /// </summary>
        public virtual string PlanQty { get; set; }

        /// <summary>
        /// 计划产出瓦数MW
        /// </summary>
        public virtual decimal PlanWatt { get; set; }

        /// <summary>
        /// 计划投入量
        /// </summary>
        public virtual decimal PlanInQty { get; set; }

        /// <summary>
        /// 目标碎片率
        /// </summary>
        public virtual decimal TargetDebrisRate { get; set; }

        /// <summary>
        /// 人均产出
        /// </summary>
        public virtual decimal PerCapitaEfficiency { get; set; }

        /// <summary>
        /// 层前合格率
        /// </summary>
        public virtual decimal BeforePressQRate { get; set; }

        /// <summary>
        /// 半成品A级品率
        /// </summary>
        public virtual decimal HProductARate { get; set; }

        /// <summary>
        /// A级品率
        /// </summary>
        public virtual decimal ProductARate { get; set; }

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
