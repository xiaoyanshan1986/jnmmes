using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;
using System.ComponentModel.DataAnnotations;
using ServiceCenter.MES.Model.FMM.Resources;

namespace ServiceCenter.MES.Model.FMM
{
    /// <summary>
    /// 区域等级
    /// </summary>
    public enum LocationLevel
    {
        /// <summary>
        /// 工厂
        /// </summary>
        [Display(Name = "LocationLevel_Factory", ResourceType = typeof(StringResource))]
        Factory = 1,
        /// <summary>
        /// 车间
        /// </summary>
        [Display(Name = "LocationLevel_Room", ResourceType = typeof(StringResource))]
        Room = 2,
        /// <summary>
        /// 区域
        /// </summary>
        [Display(Name = "LocationLevel_Area", ResourceType = typeof(StringResource))]
        Area = 4
    }
    /// <summary>
    /// 区域数据模型
    /// </summary>
    [DataContract]
    public class Location:BaseModel<string>
    {
        /// <summary>
        /// 主键（区域名称）。
        /// </summary>
        [DataMember]
        public override string Key
        {
            get;
            set;
        }

        /// <summary>
        /// 描述。
        /// </summary>
        [DataMember]
        public virtual  string Description { get; set; }

        /// <summary>
        /// 区域等级。1：工厂 2：车间 4：区域
        /// </summary>
        [DataMember]
        public virtual LocationLevel Level { get; set; }

        /// <summary>
        /// 父级区域名称。
        /// </summary>
        [DataMember]
        public virtual string ParentLocationName { get; set; }

        /// <summary>
        /// ERP部门代码
        /// </summary>
        [DataMember]
        public virtual string ERPDeptCode { get; set; }

        /// 创建人。
        /// </summary>
        [DataMember]
        public virtual  string Creator { get; set; }

        /// <summary>
        /// 创建时间。
        /// </summary>
        [DataMember]
        public virtual  DateTime CreateTime { get; set; }

        /// <summary>
        /// 编辑人。
        /// </summary>
        [DataMember]
        public virtual  string Editor { get; set; }

        /// <summary>
        /// 编辑时间。
        /// </summary>
        [DataMember]
        public virtual  DateTime EditTime { get; set; }

    }
}
