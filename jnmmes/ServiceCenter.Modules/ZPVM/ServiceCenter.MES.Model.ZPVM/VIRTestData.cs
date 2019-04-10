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
    /// 表示安规测试数据主键。
    /// </summary>
    public struct VIRTestDataKey
    {
        /// <summary>
        /// 设备代码。
        /// </summary>
        public string EquipmentCode { get; set; }

        /// <summary>
        /// 测试时间
        /// </summary>
        public DateTime TestTime { get; set; }
        /// <summary>
        /// 批次号。
        /// </summary>
        public string LotNumber { get; set; }

        /// <summary>
        /// 测试类型。
        /// </summary>
        public string TestType { get; set; }

    }
    /// <summary>
    /// 描述安规测试数据的模型
    /// </summary>
    [DataContract]
    public class VIRTestData : BaseModel<VIRTestDataKey>
    {

        /// <summary>
        /// 测试结果。
        /// </summary>
        [DataMember]
        public virtual string TestResult { get; set; }
        /// <summary>
        /// 测试标示。
        /// </summary>
        [DataMember]
        public virtual bool TestFlag { get; set; }
        /// <summary>
        /// 步骤。
        /// </summary>
        [DataMember]
        public virtual int TestStepSeq { get; set; }

        /// <summary>
        /// 测试步骤结果。
        /// </summary>
        [DataMember]
        public virtual string TestStepResult { get; set; }
        /// <summary>
        /// 测试参数1。
        /// </summary>
        [DataMember]
        public virtual string TestParam1 { get; set; }
        /// <summary>
        /// 测试参数2。
        /// </summary>
        [DataMember]
        public virtual string TestParam2 { get; set; }
        /// <summary>
        /// 输出电压
        /// </summary>
        [DataMember]
        public virtual string Voltage { get; set; }
        /// <summary>
        /// 频率
        /// </summary>
        [DataMember]
        public virtual string Frequency { get; set; }
        /// <summary>
        /// 加载电流
        /// </summary>
        [DataMember]
        public virtual string Ecurren { get; set; }
        /// <summary>
        /// 范围上限。
        /// </summary>
        [DataMember]
        public virtual string Hilimit { get; set; }
        /// <summary>
        /// 范围下限。
        /// </summary>
        [DataMember]
        public virtual string Lolimit { get; set; }
        /// <summary>
        /// 缓升时间。
        /// </summary>
        [DataMember]
        public virtual string Rampup { get; set; }

        /// <summary>
        /// 延时时间。
        /// </summary>
        [DataMember]
        public virtual string Dwelltime { get; set; }
        /// <summary>
        /// 实际填充因子。
        /// </summary>
        [DataMember]
        public virtual string Delaytime { get; set; }
        /// <summary>
        /// 缓升完毕。
        /// </summary>
        [DataMember]
        public virtual string Ramphi { get; set; }

        /// <summary>
        /// 最低电荷。
        /// </summary>
        [DataMember]
        public virtual string Chargelo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public virtual string Offset { get; set; }
        /// <summary>
        /// 灵敏度
        /// </summary>
        [DataMember]
        public virtual string Arcsense { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public virtual string Arcfail { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public virtual string Scanner { get; set; }

    }
}
