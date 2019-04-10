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
    /// 工步主键。
    /// </summary>
    public struct RouteStepKey
    {
        /// <summary>
        /// 工艺流程名称。
        /// </summary>
        public string RouteName { get; set; }
        /// <summary>
        /// 工步名称。
        /// </summary>
        public string RouteStepName { get; set; }
        public override string ToString()
        {
            return string.Format("{0}:{1}", this.RouteName, this.RouteStepName);
        }
    }
    /// <summary>
    /// 表示工艺流程中工步数据模型，工序在工艺流程中的具体化。
    /// </summary>
    [DataContract]
    public class RouteStep : BaseModel<RouteStepKey>
    { 
        /// <summary>
        /// 主键。
        /// </summary>
        [DataMember]
        public override RouteStepKey Key
        {
            get;
            set;
        }
        /// <summary>
        /// 排序序号。
        /// </summary>
        [DataMember]
        public virtual int SortSeq { get; set; }
        /// <summary>
        /// 加工时长(分钟数)。
        /// </summary>
        [DataMember]
        public virtual double? Duration { get; set; }
        /// <summary>
        /// 报废原因代码分组名称。
        /// </summary>
        [DataMember]
        public virtual string ScrapReasonCodeCategoryName { get; set; }
        /// <summary>
        /// 不良原因代码分组名称。
        /// </summary>
        [DataMember]
        public virtual string DefectReasonCodeCategoryName { get; set; }
        /// <summary>
        /// 工艺工序名称。
        /// </summary>
        [DataMember]
        public virtual string RouteOperationName { get; set; }
        /// <summary>
        /// 描述。
        /// </summary>
        [DataMember]
        public virtual string Description { get; set; }
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
