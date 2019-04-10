using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;
using System.ComponentModel.DataAnnotations;
using ServiceCenter.MES.Model.FMM.Resources;
using ServiceCenter.Common;

namespace ServiceCenter.MES.Model.FMM
{
    /// <summary>
    /// 客户端类型。
    /// </summary>
    public enum EnumClientType
    {
        /// <summary>
        /// PC客户端。
        /// </summary>
        [Display(Name = "EnumClientType_PC", ResourceType = typeof(StringResource))]
        PC=0,
        /// <summary>
        /// 网络打印机。
        /// </summary>
        [Display(Name = "EnumClientType_NetworkPrinter", ResourceType = typeof(StringResource))]
        NetworkPrinter=1,
        /// <summary>
        /// 本地打印机。
        /// </summary>
        [Display(Name = "EnumClientType_RawPrinter", ResourceType = typeof(StringResource))]
        RawPrinter = 2,
        /// <summary>
        /// 条码扫描器。
        /// </summary>
        [Display(Name = "EnumClientType_Reader", ResourceType = typeof(StringResource))]
        Reader=3,
        /// <summary>
        /// 其他。
        /// </summary>
        [Display(Name = "EnumClientType_Other", ResourceType = typeof(StringResource))]
        Other=4
    }
    /// <summary>
    /// 客户端配置数据模型类。
    /// </summary>
    [DataContract]
    public class ClientConfig : BaseModel<string>
    {
        /// <summary>
        /// 主键（客户端名称）。
        /// </summary>
        [DataMember]
        public override string Key
        {
            get;
            set;
        }
        /// <summary>
        /// 客户端类型。
        /// </summary>
        [DataMember]
        public virtual EnumClientType ClientType { get; set; }
        /// <summary>
        /// IP地址。
        /// </summary>
        [DataMember]
        public virtual string IPAddress { get; set; }
        /// <summary>
        /// 车间名称。
        /// </summary>
        [DataMember]
        public virtual string LocationName { get; set; }
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
