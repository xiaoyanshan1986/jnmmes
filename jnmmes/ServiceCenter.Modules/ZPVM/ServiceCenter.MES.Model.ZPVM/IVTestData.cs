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
    /// 表示IV测试数据主键。
    /// </summary>
    public struct IVTestDataKey
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
        public string EquipmentCode { get; set; }
    }
    /// <summary>
    /// 描述IV测试数据的模型
    /// </summary>
    [DataContract]
    public class IVTestData : BaseModel<IVTestDataKey>
    {
        /// <summary>
        /// 功率值。
        /// </summary>
        [DataMember]
        public virtual double PM { get; set; }
        /// <summary>
        /// 短路电流值。
        /// </summary>
        [DataMember]
        public virtual double ISC { get; set; }
        /// <summary>
        /// 最大电流值。
        /// </summary>
        [DataMember]
        public virtual double IPM { get; set; }
        /// <summary>
        /// 开路电压。
        /// </summary>
        [DataMember]
        public virtual double VOC { get; set; }
        /// <summary>
        /// 最大电压。
        /// </summary>
        [DataMember]
        public virtual double VPM { get; set; }
        /// <summary>
        /// 填充因子。
        /// </summary>
        [DataMember]
        public virtual double FF { get; set; }
        /// <summary>
        /// 转换效率。
        /// </summary>
        [DataMember]
        public virtual double EFF { get; set; }
        /// <summary>
        /// 串联电阻。
        /// </summary>
        [DataMember]
        public virtual double RS { get; set; }
        /// <summary>
        /// 并联电阻。
        /// </summary>
        [DataMember]
        public virtual double RSH { get; set; }
        /// <summary>
        /// 环境温度。（应为背板温度）
        /// </summary>
        [DataMember]
        public virtual double AmbientTemperature { get; set; }
        /// <summary>
        /// 传感器温度。（应为环境温度）
        /// </summary>
        [DataMember]
        public virtual double  SensorTemperature { get; set; }
        /// <summary>
        /// 光强。
        /// </summary>
        [DataMember]
        public virtual double Intensity { get; set; }
        /// <summary>
        /// 实际功率。
        /// </summary>
        [DataMember]
        public virtual double CoefPM { get; set; }
        /// <summary>
        /// 实际短路电流。
        /// </summary>
        [DataMember]
        public virtual double CoefISC { get; set; }
        /// <summary>
        /// 实际最大电流值。
        /// </summary>
        [DataMember]
        public virtual double CoefIPM { get; set; }
        /// <summary>
        /// 实际开路电压。
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
        public virtual int? PowersetItemNo { get; set; }
        /// <summary>
        /// 子分档代码。
        /// </summary>
        [DataMember]
        public virtual string PowersetSubCode { get; set; }
        /// <summary>
        /// 是否默认值。
        /// </summary>
        [DataMember]
        public virtual bool IsDefault { get; set; }
        /// <summary>
        /// 是否打印标签。
        /// </summary>
        [DataMember]
        public virtual bool IsPrint{ get; set; }
        /// <summary>
        /// 打印时间。
        /// </summary>
        [DataMember]
        public virtual DateTime? PrintTime { get; set; }
        /// <summary>
        /// 打印次数。
        /// </summary>
        [DataMember]
        public virtual int PrintCount { get; set; }
        /// <summary>
        /// 校准时间。
        /// </summary>
        [DataMember]
        public virtual DateTime? CalibrateTime { get; set; }
        /// <summary>
        /// 校准板编号。
        /// </summary>
        [DataMember]
        public virtual string CalibrationNo { get; set; }
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
        /// <summary>
        /// 编辑人。
        /// </summary>
        [DataMember]
        public virtual  string Editor { get; set; }
        /// <summary>
        /// 编辑时间。
        /// </summary>
        [DataMember]
        public virtual  DateTime EditTime { get; set; }

        /// <summary>
        /// 标准电流1。
        /// </summary>
        [DataMember]
        public virtual double StdIsc1 { get; set; }

        /// <summary>
        /// 标准电流2。
        /// </summary>
        [DataMember]
        public virtual double StdIsc2 { get; set; }
        /// <summary>
        /// 标准光强1。
        /// </summary>
        [DataMember]
        public virtual double Stdsun1 { get; set; }
        /// <summary>
        /// 标准光强2。
        /// </summary>
        [DataMember]
        public virtual double Stdsun2 { get; set; }

    }
}
