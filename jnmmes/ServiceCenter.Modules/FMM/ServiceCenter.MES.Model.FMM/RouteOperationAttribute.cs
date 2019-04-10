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
    /// 表示工序属性数据主键。
    /// </summary>
    public struct RouteOperationAttributeKey
    {
        /// <summary>
        /// 工序名称。
        /// </summary>
        public string RouteOperationName{get;set;}
        /// <summary>
        /// 属性名称。
        /// </summary>
        public string AttributeName { get; set; }

        public override string ToString()
        {
            return string.Format("{0}:{1}", this.RouteOperationName, this.AttributeName);
        }
    }
    /// <summary>
    /// 表示工序属性数据。
    /// </summary>
    [DataContract]
    public class RouteOperationAttribute : BaseModel<RouteOperationAttributeKey>
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
