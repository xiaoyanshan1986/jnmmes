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
    /// 原因代码分组明细主键。
    /// </summary>
    public struct ReasonCodeCategoryDetailKey
    {
        /// <summary>
        /// 原因代码组名称。
        /// </summary>
        public string ReasonCodeCategoryName { get; set; }
        /// <summary>
        /// 原因代码名称。
        /// </summary>
        public string ReasonCodeName { get; set; }

        public override string ToString()
        {
            return string.Format("{0}:{1}", this.ReasonCodeCategoryName, this.ReasonCodeName);
        }
    }
    /// <summary>
    /// 原因代码分组数据模型。
    /// </summary>
    [DataContract]
    public class ReasonCodeCategoryDetail : BaseModel<ReasonCodeCategoryDetailKey>
    {
        /// <summary>
        /// 项目号。
        /// </summary>
        [DataMember]
        public virtual int ItemNo { get; set; }

        /// <summary>
        /// 创建人。
        /// </summary>
        [DataMember]
        public virtual string Creator { get; set; }
        /// <summary>
        /// 创建时间。
        /// </summary>
        [DataMember]
        public virtual DateTime CreateTime { get; set; }
        /// <summary>
        /// 编辑人。
        /// </summary>
        [DataMember]
        public virtual string Editor { get; set; }
        /// <summary>
        /// 编辑时间。
        /// </summary>
        [DataMember]
        public virtual DateTime EditTime { get; set; }
    }
}
