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
    /// 工步属性主键。
    /// </summary>
    public struct RouteStepAttributeKey {
        /// <summary>
        /// 工艺流程名称。
        /// </summary>
        public string RouteName { get; set; }
        /// <summary>
        /// 工步名称。
        /// </summary>
        public string RouteStepName { get; set; }
        /// <summary>
        /// 属性名称。
        /// </summary>
        public string AttributeName{ get; set; }

        public override string ToString()
        {
            return string.Format("{0}:{1}:{2}", this.RouteName, this.RouteStepName, this.AttributeName);
        }
    }
    /// <summary>
    /// 工艺流程中工步属性数据模型。
    /// </summary>
    [DataContract]
    public class RouteStepAttribute : BaseModel<RouteStepAttributeKey>
    {
        /// <summary>
        /// 属性值。
        /// </summary>
        [DataMember]
        public virtual string Value { get; set; }
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
