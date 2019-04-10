using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;
using System.ComponentModel.DataAnnotations;
using ServiceCenter.MES.Model.WIP.Resources;

namespace ServiceCenter.MES.Model.WIP
{


    /// <summary>
    /// 包装明细主键。
    /// </summary>
    public struct PrintLabelLogKey
    {
        /// <summary>
        /// 包装号。
        /// </summary>
        [DataMember]
        public string LotNumber { get; set; }
        /// <summary>
        /// 线别
        /// </summary>
        [DataMember]
        public string LineCode { get; set; }


        public override string ToString()
        {
            return string.Format("{0}：{1}", this.LineCode, this.LotNumber);
        }
    }
    /// <summary>
    /// 描述打印操作日志数据的模型类。
    /// </summary>
    [DataContract]
    public class PrintLabelLog : BaseModel<PrintLabelLogKey>
    {      
        /// <summary>
        /// 打印次数
        /// </summary>
        [DataMember]
        public virtual string ItemNo { get; set; }

        /// <summary>
        /// 打印时间
        /// </summary>
        [DataMember]
        public virtual DateTime? PrintTime { get; set; }

        /// <summary>
        /// 编辑时间
        /// </summary>
        [DataMember]
        public virtual DateTime? EditTime { get; set; }

        /// <summary>
        /// 出入栈标志
        /// </summary>
        [DataMember]
        public virtual int TrackFlag { get; set; }


    }
}
