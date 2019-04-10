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
    /// 表示规则-打印日志设置主键。
    /// </summary>
    public struct ZwipLablePrintLogKey
    {
        /// <summary>
        /// 批次号。
        /// </summary>
        public string LotNumber { get; set; }
        /// <summary>
        /// 线别。
        /// </summary>
        public string LineCode { get; set; }
        
        public override string ToString()
        {
            return string.Format("{0}:{1}", this.LotNumber,this.LineCode);
        }
    }
    /// <summary>
    /// 描述规则-打印设置的数据模型
    /// </summary>
    [DataContract]
    public class ZwipLablePrintLog : BaseModel<ZwipLablePrintLogKey>
    {
        /// <summary>
        /// 项目号。
        /// </summary>
        [DataMember]
        public virtual int ItemNo { get; set; }  
      
        /// <summary>
        /// 打印时间。
        /// </summary>
        [DataMember]
        public virtual DateTime PrintTime { get; set; }
       
        /// <summary>
        /// 编辑时间。
        /// </summary>
        [DataMember]
        public virtual DateTime EditTime { get; set; }

        /// <summary>
        /// 过站标志位
        /// </summary>
        public virtual int TrackFlag { get; set; }

    }
}
