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
    public struct ColorTestDataKey
    {
        /// <summary>
        /// 批次号。
        /// </summary>
        public string LotNumber { get; set; }
       
        public DateTime InspectTime { get; set; }

        public override string ToString()
        {
            return string.Format("{0}{1}", this.LotNumber);
        }
        
    }

    /// <summary>
    /// 描述IV测试数据的模型
    /// </summary>
    [DataContract]
    public class ColorTestData : BaseModel<ColorTestDataKey>
    {

        /// <summary>
        /// 花色值。
        /// </summary>
        [DataMember]
        public virtual string InspctResult { get; set; }

        [DataMember]
        public virtual string LocalIp { get; set; }

        //[DataMember]
        //public virtual string Opetator { get; set; }

        //[DataMember]
        //public virtual string Shift { get; set; }

        //[DataMember]
        //public virtual string RcpName { get; set; }

        //[DataMember]
        //public virtual string DeviceID { get; set; }

        //[DataMember]
        //public virtual DateTime DataTime { get; set; }

        //[DataMember]
        //public virtual string InspectValue { get; set; }

        //[DataMember]
        //public virtual string BlueValue { get; set; }

    }
}
