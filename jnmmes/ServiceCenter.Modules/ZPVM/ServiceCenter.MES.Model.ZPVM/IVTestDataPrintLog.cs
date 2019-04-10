using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;
using System.ComponentModel.DataAnnotations;
using ServiceCenter.MES.Model.ZPVM.Resources;

namespace ServiceCenter.MES.Model.ZPVM
{
    /// <summary>
    /// 表示IV测试数据打印日志主键。
    /// </summary>
    public struct IVTestDataPrintLogKey
    {
        /// <summary>
        /// 批次号。
        /// </summary>
        public string LotNumber { get; set; }
        /// <summary>
        /// 测试时间。
        /// </summary>
        public DateTime TestTime { get; set; }
        /// <summary>
        /// 设备代码。
        /// </summary>
        public string  EquipmentCode { get; set; }
        /// <summary>
        /// 项目号。
        /// </summary>
        public int ItemNo { get; set; }
    }
    /// <summary>
    /// 描述IV测试数据打印日志的模型
    /// </summary>
    [DataContract]
    public class IVTestDataPrintLog : BaseModel<IVTestDataPrintLogKey>
    {
        /// <summary>
        /// 实际功率。
        /// </summary>
        [DataMember]
        public virtual double CoefPM { get; set; }
        /// <summary>
        /// 实际电流。
        /// </summary>
        [DataMember]
        public virtual double CoefISC { get; set; }
        /// <summary>
        /// 实际最大电流值。
        /// </summary>
        [DataMember]
        public virtual double CoefIPM { get; set; }
        /// <summary>
        /// 实际电压。
        /// </summary>
        [DataMember]
        public virtual double CoefVOC { get; set; }
        /// <summary>
        /// 实际最大电压。
        /// </summary>
        [DataMember]
        public virtual double CoefVPM { get; set; }
        /// <summary>
        /// 实际填充因子。
        /// </summary>
        [DataMember]
        public virtual double CoefFF { get; set; }
        /// <summary>
        /// CTM。
        /// </summary>
        [DataMember]
        public virtual double CTM { get; set; }
        /// <summary>
        /// 分档代码。
        /// </summary>
        [DataMember]
        public virtual string PowersetCode { get; set; }
        /// <summary>
        /// 分档项目号。
        /// </summary>
        [DataMember]
        public virtual int PowersetItemNo { get; set; }
        /// <summary>
        /// 子分档代码。
        /// </summary>
        [DataMember]
        public virtual string PowersetSubCode { get; set; }
        /// <summary>
        /// 打印时间。
        /// </summary>
        [DataMember]
        public virtual DateTime PrintTime { get; set; }
        /// <summary>
        /// 标签代码。
        /// </summary>
        [DataMember]
        public virtual string LabelCode { get; set; }
        /// <summary>
        /// 创建人。
        /// </summary>
        [DataMember]
        public virtual  string Creator { get; set; }
        /// <summary>
        /// 创建时间。
        /// </summary>
        [DataMember]
        public virtual  DateTime CreateTime { get; set; }

    }
}
