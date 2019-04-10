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
    /// 表示某月目标参数的数据主键。
    /// </summary>
    public struct TargetParameterKey
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
        /// 项目类型
        /// </summary>
        public string ItemType { get; set; }

        /// <summary>
        /// 项目
        /// </summary>
        public string ItemCode { get; set; }

        public override string ToString()
        {
            return string.Format("{0}-{1}-{2} ,{3}车间, {4}项目", Year, Month, Day, LocationName, ItemCode);
        }
    }
    public class TargetParameter : BaseModel<TargetParameterKey>
    {        
        /// <summary>
        /// 目标值
        /// </summary>
        public virtual double ValueData { get; set; }

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
