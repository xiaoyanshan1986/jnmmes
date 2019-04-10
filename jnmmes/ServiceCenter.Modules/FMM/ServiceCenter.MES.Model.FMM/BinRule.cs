using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;

namespace ServiceCenter.MES.Model.FMM
{

    /// <summary>
    /// 包装明细主键。
    /// </summary>
    public struct BinRuleKey
    {
        /// <summary>
        /// Bin号
        /// </summary>
        [DataMember]
        public string BinNo { get; set; }
       
        /// <summary>
        /// 线别代码
        /// </summary>
        [DataMember]
        public string PackageLine { get; set; }

        /// <summary>
        /// 工单代码
        /// </summary>
        [DataMember]
        public string WorkOrderNumber { get; set; }
        
        /// <summary>
        /// 等级
        /// </summary>
        [DataMember]
        public string Grade { get; set; }

        /// <summary>
        /// 分档组
        /// </summary>
        [DataMember]
        public string PsCode { get; set; }

        /// <summary>
        /// 分档项目号
        /// </summary>
        [DataMember]
        public int PsItemNo { get; set; }
        
        /// <summary>
        /// 子分档代码
        /// </summary>
        [DataMember]
        public string PsSubCode { get; set; }

        /// <summary>
        /// 颜色
        /// </summary>
        [DataMember]
        public string Color { get; set; }

        public override string ToString()
        {
            return string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}", this.BinNo, this.PackageLine,this.WorkOrderNumber,this.Grade,
                this.PsCode,this.PsItemNo,this.PsSubCode,this.Color);
        }
    }

    /// <summary>
    /// 描述班别的数据模型类。
    /// </summary>
    [DataContract]
    public class BinRule : BaseModel<BinRuleKey>
    {       
        /// <summary>
        /// 车间代码
        /// </summary>
        [DataMember]
        public virtual string LocationName { get; set; }       
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
