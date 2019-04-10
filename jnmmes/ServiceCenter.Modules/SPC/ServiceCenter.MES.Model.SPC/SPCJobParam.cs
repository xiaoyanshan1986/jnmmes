using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using ServiceCenter.MES.Model.SPC.Resources;

namespace ServiceCenter.MES.Model.SPC
{
    public struct SPCJobParamKey
    {
        /// <summary>
        /// ID
        /// </summary>
        [DataMember]
        public  string JobId { set; get; }
        /// <summary>
        /// 参数名称
        /// </summary>
        [DataMember]
        public  string ParamName { set; get; }

        /// <summary>
        /// 参数名称
        /// </summary>
        [DataMember]
        public string ParamType { set; get; }     
    }

    [DataContract]
    public class SPCJobParam : BaseModel<SPCJobParamKey>
    {
        /// <summary>
        /// 上范围值。
        /// </summary>
        [DataMember]
        public virtual double? UpperBoundary { get; set; }
        /// <summary>
        /// 上规格值。
        /// </summary>
        [DataMember]
        public virtual double? UpperSpecification { get; set; }
        /// <summary>
        /// 上控制值。
        /// </summary>
        [DataMember]
        public virtual double? UpperControl { get; set; }
        /// <summary>
        /// 目标值。
        /// </summary>
        [DataMember]
        public virtual double? Target { get; set; }
        /// <summary>
        /// 下控制值。
        /// </summary>
        [DataMember]
        public virtual double? LowerControl { get; set; }
        /// <summary>
        /// 下规格值。
        /// </summary>
        [DataMember]
        public virtual double? LowerSpecification { get; set; }
        /// <summary>
        /// 下范围值。
        /// </summary>
        [DataMember]
        public virtual double? LowerBoundary { get; set; }

        /// <summary>
        /// y轴最大值。
        /// </summary>
        [DataMember]
        public virtual double? LineUpper { get; set; }

        /// <summary>
        /// y轴最小值。
        /// </summary>
        [DataMember]
        public virtual double? LineLower { get; set; }

        /// <summary>
        /// y轴间距。
        /// </summary>
        [DataMember]
        public virtual double? LineYinterval { get; set; }

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
