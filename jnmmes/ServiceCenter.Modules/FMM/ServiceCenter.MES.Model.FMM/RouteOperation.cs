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
    /// 表示工序数据，工艺流程中工步的抽象化。
    /// </summary>
    [DataContract]
    public class RouteOperation : BaseModel<string>
    {
        /// <summary>
        /// 主键(工序名称）。
        /// </summary>
        [DataMember]
        public override string Key
        {
            get;
            set;
        }
        /// <summary>
        /// 加工时长。
        /// </summary>
        [DataMember]
        public virtual double? Duration { get; set; }
        /// <summary>
        /// 状态。
        /// </summary>
        [DataMember]
        public virtual EnumObjectStatus Status { get; set; }
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
        /// 排序序号。
        /// </summary>
        [DataMember]
        public virtual int SortSeq { get; set; }
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
