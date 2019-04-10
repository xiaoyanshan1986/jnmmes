using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;
using System.ComponentModel.DataAnnotations;
using ServiceCenter.MES.Model.FMM.Resources;

namespace ServiceCenter.MES.Model.FMM
{
    /// <summary>
    /// //设备异常类型耗时管理主键。
    /// </summary>
    public struct EquipmentConsumingKey
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
        /// 班别
        /// </summary>
        public string ShiftName { get; set; }

        /// <summary>
        /// 车间
        /// </summary>
        public string LocationName { get; set; }

        /// <summary>
        /// 线别
        /// </summary>
        public string LineCode { get; set; }

       

        /// <summary>
        /// 设备代码
        /// </summary>
        public string EquipmentCode { get; set; }

        /// <summary>
        /// 原因代码
        /// </summary>
        public string ReasonCodeName { get; set; }


        /// <summary>
        /// 工序
        /// </summary>
        public  string RouteStepName { get; set; }
       
       

        public override string ToString()
        {
            return string.Format("{0}-{1}-{2} ,{3}, {4},{5},{6},{7},{8}", Year, Month, Day, ShiftName, LocationName, LineCode, EquipmentCode, ReasonCodeName, RouteStepName);
        }
    }
    public class EquipmentConsuming : BaseModel<EquipmentConsumingKey>
    {
        /// <summary>
        /// 耗时
        /// </summary>
        public virtual int Consuming { get; set; }

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
