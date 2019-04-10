using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;
using System.ComponentModel.DataAnnotations;
using ServiceCenter.MES.Model.WIP.Resources;

namespace ServiceCenter.MES.Model.WIP
{
    /// <summary>
    /// 包装状态枚举。
    /// </summary>
    public enum EnumPackageState
    {
        /// <summary>
        /// 包装中
        /// </summary>
        [Display(Name = "EnumPackageState_Packaging", ResourceType = typeof(StringResource))]
        Packaging = 0,

        /// <summary>
        /// 已包装
        /// </summary>
        [Display(Name = "EnumPackageState_Packaged", ResourceType = typeof(StringResource))]
        Packaged = 1,

        /// <summary>
        /// 已检验
        /// </summary>
        [Display(Name = "EnumPackageState_Checked", ResourceType = typeof(StringResource))]
        Checked = 2,

        /// <summary>
        /// 入库单中未申请
        /// </summary>
        [Display(Name = "EnumPackageState_InStoragelist", ResourceType = typeof(StringResource))]
        InStoragelist = 3,

        /// <summary>
        /// 入库申请
        /// </summary>
        [Display(Name = "EnumPackageState_Apply", ResourceType = typeof(StringResource))]
        Apply = 4,

        /// <summary>
        /// 已入库
        /// </summary>
        [Display(Name = "EnumPackageState_ToWarehouse", ResourceType = typeof(StringResource))]
        ToWarehouse = 5,

        /// <summary>
        /// 已出货
        /// </summary>
        [Display(Name = "EnumPackageState_Shipped", ResourceType = typeof(StringResource))]
        Shipped = 6,

        /// <summary>
        /// 线边仓待投料
        /// </summary>
        [Display(Name = "EnumPackageState_InFabStore", ResourceType = typeof(StringResource))]
        InFabStore = 7
    }
    /// <summary>
    /// 包装类型枚举。
    /// </summary>
    public enum EnumPackageType
    {
        /// <summary>
        /// 按包。
        /// </summary>
        Packet=0,
        /// <summary>
        /// 按箱。
        /// </summary>
        Box=1
    }

    /// <summary>
    /// 包装类型枚举。
    /// </summary>
    public enum EnumPackageMixedType
    {
        /// <summary>
        /// 按包。
        /// </summary>
        UnMixedType = 0,
        /// <summary>
        /// 按箱。
        /// </summary>
        MixedType = 1
    }
    /// <summary>
    /// 描述包装数据的模型类。
    /// </summary>
    [DataContract]
    public class Package : BaseModel<string>
    {
        /// <summary>
        /// 主键（包装号）
        /// </summary>
        public override string Key
        {
            get
            {
                return base.Key;
            }
            set
            {
                base.Key = value;
            }
        }
        /// <summary>
        /// 容器号。
        /// </summary>
        [DataMember]
        public virtual string ContainerNo { get; set; }
        
        ///// <summary>
        ///// 柜明细对象。
        ///// </summary>
        //[DataMember]
        //public virtual ChestDetail ChestDetail { get; set; }

        /// <summary>
        /// 工单号。
        /// </summary>
        [DataMember]
        public virtual string OrderNumber { get; set; }
        /// <summary>
        /// 包装状态。0:包装中 1：已包装 2：已检验 3：已入库 4：已出货
        /// </summary>
        [DataMember]
        public virtual EnumPackageState PackageState { get; set; }

        /// <summary>
        /// 是否在入库单
        /// </summary>
        [DataMember]
        public virtual int InOrder { get; set; }

        /// <summary>
        /// 是否在库
        /// </summary>
        [DataMember]
        public virtual int InStg { get; set; }
        /// <summary>
        /// 物料编码。
        /// </summary>
        [DataMember]
        public virtual string MaterialCode { get; set; }
        /// <summary>
        /// 是否尾包。
        /// </summary>
        [DataMember]
        public virtual bool IsLastPackage { get; set; }
        /// <summary>
        /// 包装数量。
        /// </summary>
        [DataMember]
        public virtual double Quantity { get; set; }
        /// <summary>
        /// 类型。
        /// </summary>
        [DataMember]
        public virtual EnumPackageType PackageType { get; set; }
        /// <summary>
        /// 描述。
        /// </summary>
        [DataMember]
        public virtual string Description { get; set; }
        /// <summary>
        /// 检验人。
        /// </summary>
        [DataMember]
        public virtual string Checker { get; set; }
        /// <summary>
        /// 检验时间。
        /// </summary>
        [DataMember]
        public virtual DateTime? CheckTime { get; set; }
        /// <summary>
        /// 入库人。
        /// </summary>
        [DataMember]
        public virtual string ToWarehousePerson { get; set; }
        /// <summary>
        /// 入库时间。
        /// </summary>
        [DataMember]
        public virtual DateTime? ToWarehouseTime { get; set; }
        /// <summary>
        /// 出货人。
        /// </summary>
        [DataMember]
        public virtual string ShipmentPerson { get; set; }
        /// <summary>
        /// 出货时间。
        /// </summary>
        [DataMember]
        public virtual DateTime? ShipmentTime { get; set; }
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
        /// 供应商编号
        /// </summary>
        [DataMember]
        public virtual string SupplierCode { get; set; }
        /// <summary>
        /// 包装类型
        /// </summary>
        [DataMember]
        public virtual EnumPackageMixedType PackageMixedType { get; set; }

        /// <summary>
        /// 线别
        /// </summary>
        [DataMember]
        public virtual string LineCode { get; set; }
        /// <summary>
        /// 等级
        /// </summary>
        [DataMember]
        public virtual string Grade { get; set; }
        /// <summary>
        /// 花色
        /// </summary>
        [DataMember]
        public virtual string Color { get; set; }
        /// <summary>
        /// 分档名称
        /// </summary>
        [DataMember]
        public virtual string PowerName { get; set; }
        /// <summary>
        /// 子分档代码
        /// </summary>
        [DataMember]
        public virtual string PowerSubCode { get; set; }

        /// <summary>
        /// 车间
        /// </summary>
        [DataMember]
        public virtual string Location { get; set; }

    }
}
