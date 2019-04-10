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
    /// 表示产品工艺流程数据主键
    /// </summary>
    public struct MaterialRouteKey
    {
        /// <summary>
        /// 产品代码
        /// </summary>
        public string MaterialCode { get; set; }

        /// <summary>
        /// 车间名称
        /// </summary>
        public string LocationName { get; set; }

        /// <summary>
        /// 工艺流程组名称
        /// </summary>
        public string RouteEnterpriseName { get; set; }

        public override string ToString()
        {
            return string.Format("{0}-{2}-{1}"
                                ,this.MaterialCode
                                ,this.RouteEnterpriseName
                                ,this.LocationName);
        }
    }

    /// <summary>
    /// 表示产品工艺流程
    /// </summary>
    [DataContract]
    public class MaterialRoute : BaseModel<MaterialRouteKey>
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
