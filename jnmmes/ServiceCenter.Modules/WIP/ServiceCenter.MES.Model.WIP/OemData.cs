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
    /// OEM组件状态枚举。
    /// </summary>
    public enum EnumOemStatus
    {
        /// <summary>
        /// 导入
        /// </summary>
        [Display(Name = "导入")]
        Import = 0,

        /// <summary>
        /// 已包装
        /// </summary>
        [Display(Name = "已包装")]
        Packaged = 1,

        /// <summary>
        /// 入库申请
        /// </summary>
        [Display(Name = "入库申请")]
        Apply = 2,

        /// <summary>
        /// 已入库
        /// </summary>
        [Display(Name = "已入库")]
        ToWarehouse = 3,

        /// <summary>
        /// 入库单中未申请
        /// </summary>
        [Display(Name = "入库单中未申请")]
        InStoragelist = 5

        ///// <summary>
        ///// 线边仓待投料
        ///// </summary>
        //[Display(Name = "线边仓待投料")]
        //InFabStore = 4
    }

    /// <summary>
    /// 描述批次数据的模型类。
    /// </summary>
    [DataContract]
    public class OemData : BaseModel<string>
    {

        public OemData(){

        }        

        /// <summary>
        /// 主键（批次号）
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
        /// 工单号。
        /// </summary>
        [DataMember]
        public virtual  string OrderNumber { get; set; }

        /// <summary>
        /// 产品类型。
        /// </summary>
        [DataMember]
        public virtual  string ProductType { get; set; }

        /// <summary>
        /// 功率档位。
        /// </summary>
        [DataMember]
        public virtual string PnName { get; set; }

        /// <summary>
        /// 电流档。
        /// </summary>
        [DataMember]
        public virtual string PsSubCode { get; set; }

        /// <summary>
        /// 功率值。
        /// </summary>
        [DataMember]
        public virtual double PMAX { get; set; }

        /// <summary>
        /// 短路电流值。
        /// </summary>
        [DataMember]
        public virtual double ISC { get; set; }

        /// <summary>
        /// 最大电流值。
        /// </summary>
        [DataMember]
        public virtual double IPM { get; set; }

        /// <summary>
        /// 开路电压。
        /// </summary>
        [DataMember]
        public virtual double VOC { get; set; }

        /// <summary>
        /// 最大电压。
        /// </summary>
        [DataMember]
        public virtual double VPM { get; set; }

        /// <summary>
        /// 填充因子。
        /// </summary>
        [DataMember]
        public virtual double FF { get; set; }

        /// <summary>
        /// 等级。
        /// </summary>
        [DataMember]
        public virtual  string Grade { get; set; }
        /// <summary>
        /// 颜色。
        /// </summary>
        [DataMember]
        public virtual string Color { get; set; }

        /// <summary>
        /// 箱号。
        /// </summary>
        [DataMember]
        public virtual string PackageNo { get; set; }

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
        /// 状态。0:导入 1：已包装 2：入库申请 3：已入库 4：线边仓待投料
        /// </summary>
        [DataMember]
        public virtual EnumOemStatus Status { get; set; }
        
    }
}
