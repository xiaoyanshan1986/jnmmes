using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;
using System.ComponentModel.DataAnnotations;

namespace ServiceCenter.MES.Model.ZPVM
{
    /// <summary>
    /// 入柜状态枚举。
    /// </summary>
    public enum EnumChestState
    {
        /// <summary>
        /// 入柜中
        /// </summary>
        [Display(Name = "入柜中")]
        Packaging = 0,

        /// <summary>
        /// 入柜完成
        /// </summary>
        [Display(Name = "入柜完成")]
        Packaged = 1,

        /// <summary>
        /// 检验中
        /// </summary>
        [Display(Name = "检验中")]
        Checking = 2,

        /// <summary>
        /// 检验完成
        /// </summary>
        [Display(Name = "检验完成")]
        Checked = 3,

        /// <summary>
        /// 出货中
        /// </summary>
        [Display(Name = "出货中")]
        Shipping = 4,

        /// <summary>
        /// 已出货
        /// </summary>
        [Display(Name = "已出货")]
        Shipped = 5,
        
        /// <summary>
        /// 返工投料
        /// </summary>
        [Display(Name = "返工投料")]
        InFabStore = 6
    }
    
    /// <summary>
    /// 描述入柜数据的模型类。
    /// </summary>
    [DataContract]
    public class Chest : BaseModel<string>
    {
        /// <summary>
        /// 主键（柜号）
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
        /// 入柜状态。0:入柜中 1：已入柜 2：线边仓待投料
        /// </summary>
        [DataMember]
        public virtual EnumChestState ChestState { get; set; }

        /// <summary>
        /// 物料编码。
        /// </summary>
        [DataMember]
        public virtual string MaterialCode { get; set; }
        /// <summary>
        /// 是否尾柜。
        /// </summary>
        [DataMember]
        public virtual bool IsLastPackage { get; set; }
        /// <summary>
        /// 入柜数量。
        /// </summary>
        [DataMember]
        public virtual double Quantity { get; set; }
        
        /// <summary>
        /// 描述。
        /// </summary>
        [DataMember]
        public virtual string Description { get; set; }
               
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
        /// 工单号。
        /// </summary>
        [DataMember]
        public virtual string OrderNumber { get; set; }

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
        /// 库位
        /// </summary>
        [DataMember]
        public virtual string StoreLocation { get; set; }

    }
}
