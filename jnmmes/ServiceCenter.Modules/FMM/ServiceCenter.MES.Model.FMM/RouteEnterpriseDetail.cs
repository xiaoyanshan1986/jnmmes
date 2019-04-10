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
    /// 表示工艺流程组明细主键。
    /// </summary>
    public struct RouteEnterpriseDetailKey
    {
        /// <summary>
        /// 工艺流程组名称。
        /// </summary>
        public string RouteEnterpriseName { get; set; }
        /// <summary>
        /// 工艺流程名称。
        /// </summary>
        public string RouteName { get; set; }

        public override string ToString()
        {
            return string.Format("{0}:{1}", this.RouteEnterpriseName, this.RouteName);
        }
    }
    /// <summary>
    /// 表示工艺流程组明细数据。
    /// </summary>
    [DataContract]
    public class RouteEnterpriseDetail : BaseModel<RouteEnterpriseDetailKey>
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
