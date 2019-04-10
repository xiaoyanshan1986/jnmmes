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
    /// 线边仓类型。
    /// </summary>
    public enum EnumLineStoreType
    {
        /// <summary>
        /// 原料仓。
        /// </summary>
        [Display(Name = "EnumLineStoreType_Material", ResourceType = typeof(StringResource))]
        Material = 0,
        /// <summary>
        /// 产品仓。
        /// </summary>
        [Display(Name = "EnumLineStoreType_Product", ResourceType = typeof(StringResource))]
        Product = 1,
        /// <summary>
        /// 不良仓。
        /// </summary>
        [Display(Name = "EnumLineStoreType_Defect", ResourceType = typeof(StringResource))]
        Defect = 2,
        /// <summary>
        /// 报废仓。
        /// </summary>
        [Display(Name = "EnumLineStoreType_Scrap", ResourceType = typeof(StringResource))]
        Scrap = 3
    }
    /// <summary>
    /// 线边仓数据。
    /// </summary>
    [DataContract]
    public class LineStore : BaseModel<string>
    {
        public LineStore()
        {
            this.RequestFlag = false;
            this.Status = EnumObjectStatus.Available;
        }
        /// <summary>
        /// 主键（线边仓名称）。
        /// </summary>
        [DataMember]
        public override string Key
        {
            get;
            set;
        }
        /// <summary>
        /// 线边仓类型。
        /// </summary>
        [DataMember]
        public virtual EnumLineStoreType Type { get; set; }
        /// <summary>
        /// 线边仓是否需要申请过账 0：否 1：是
        /// </summary>
        [DataMember]
        public virtual bool RequestFlag { get; set; }
        /// <summary>
        ///状态 1可用 0不可用
        /// </summary>
        [DataMember]
        public virtual EnumObjectStatus Status { get; set; }
        /// <summary>
        /// 车间名称。
        /// </summary>
        [DataMember]
        public virtual string LocationName { get; set; }
        /// <summary>
        /// 工序名称。
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
