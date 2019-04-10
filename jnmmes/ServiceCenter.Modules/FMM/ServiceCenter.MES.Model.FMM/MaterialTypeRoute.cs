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
    /// 表示物料类型工艺流程数据主键。
    /// </summary>
    public struct MaterialTypeRouteKey
    {
        /// <summary>
        /// 物料类型。
        /// </summary>
        public string MaterialType { get; set; }
        /// <summary>
        /// 车间名称。
        /// </summary>
        public string LocationName { get; set; }
        /// <summary>
        /// 工艺流程组名称。
        /// </summary>
        public string RouteEnterpriseName { get; set; }
        /// <summary>
        /// 是否返工/返修工艺流程。
        /// </summary>
        public bool IsRework { get; set; }

        public override string ToString()
        {
            return string.Format("{0}-{2}-{1}({3})", this.MaterialType
                                ,this.RouteEnterpriseName
                                ,this.LocationName
                                ,this.IsRework);
        }
    }

    /// <summary>
    /// 表示物料类型工艺流程数据。
    /// </summary>
    [DataContract]
    public class MaterialTypeRoute : BaseModel<MaterialTypeRouteKey>
    {

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
