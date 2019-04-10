using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;

namespace ServiceCenter.MES.Model.EDC
{
    /// <summary>
    /// 采集参数主键。
    /// </summary>
    public struct CategoryDetailKey
    {
        /// <summary>
        /// 采集参数组名称。
        /// </summary>
        public string CategoryName { get; set; }
        /// <summary>
        /// 参数名称。
        /// </summary>
        public string ParameterName { get; set; }

        public override string ToString()
        {
            return string.Format("{0}:{1}",this.CategoryName,this.ParameterName);
        }
    }

    /// <summary>
    /// 描述采集参数的模型类。
    /// </summary>
    [DataContract]
    public class CategoryDetail : BaseModel<CategoryDetailKey>
    {
        /// <summary>
        /// 主键。
        /// </summary>
        [DataMember]
        public override CategoryDetailKey Key
        {
            get;
            set;
        }
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
