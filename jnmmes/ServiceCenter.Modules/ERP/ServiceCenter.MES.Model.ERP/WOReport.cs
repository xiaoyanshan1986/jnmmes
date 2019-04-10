using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using ServiceCenter.MES.Model.ERP.Resources;

namespace ServiceCenter.MES.Model.ERP
{
    /// <summary>
    /// 入库接收单据核对状态枚举。
    /// </summary>
    public enum EnumBillCheckState
    {
        /// <summary>
        /// 未核对
        /// </summary>
        [Display(Name = "未核对")]
        NoCheck = 0,

        /// <summary>
        /// 核对中    
        /// </summary>
        [Display(Name = "核对中")]
        Checking = 1,

        /// <summary>
        /// 已核对   
        /// </summary>
        [Display(Name = "已核对")]
        Checked = 2
    }

    public enum EnumMixType
    {
        /// <summary>
        /// 否
        /// </summary>
        [Display(Name = "EnumMixType_False", ResourceType = typeof(StringResource))]
        False = 0,

        /// <summary>
        /// 是
        /// </summary>
        [Display(Name = "EnumMixType_True", ResourceType = typeof(StringResource))]
        True = 1
       
    }

    /// <summary>
    /// 是否报废
    /// </summary>
    public enum EnumScrapType
    {
        /// <summary>
        /// 否
        /// </summary>
        [Display(Name = "EnumScrapType_False", ResourceType = typeof(StringResource))]
        False = 0,

        /// <summary>
        /// 是
        /// </summary>
        [Display(Name = "EnumScrapType_True", ResourceType = typeof(StringResource))]
        True = 1

    }

    /// <summary>
    /// 入库单状态
    /// </summary>
    public enum EnumBillState
    {
        /// <summary>
        /// 新建
        /// </summary>
        [Display(Name = "EnumBillState_Create", ResourceType = typeof(StringResource))]
        Create = 0,

        /// <summary>
        /// 入库申报
        /// </summary>
        [Display(Name = "EnumBillState_Apply", ResourceType = typeof(StringResource))]
        Apply = 1,

        /// <summary>
        /// 接收
        /// </summary>
        [Display(Name = "EnumBillState_Receive", ResourceType = typeof(StringResource))]
        Receive = 2

    }

    /// <summary>
    /// 入库单类型
    /// </summary>
    public enum EnumStockInType
    {
        /// <summary>
        /// 产成品入库
        /// </summary>
        [Display(Name = "EnumStockInType_Finish", ResourceType = typeof(StringResource))]
        Finish = 0,

        /// <summary>
        /// 报废品入库
        /// </summary>
        [Display(Name = "EnumStockInType_Scrap", ResourceType = typeof(StringResource))]
        Scrap = 1,

        /// <summary>
        /// 在制品入库
        /// </summary>
        [Display(Name = "EnumStockInType_Wip", ResourceType = typeof(StringResource))]
        Wip = 2

    }

    /// <summary>
    /// 入库单类对象
    /// </summary>
    [DataContract]
    public class WOReport : BaseModel<string>
    {
        public WOReport()
        {
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;
            this.BillMakedDate = DateTime.Now;
            this.BillState = EnumBillState.Create;   //创建初始化状态 【0 - 新建单据】 【1 - 入库申报】 【2 - 入库接收】
        }

        /// <summary>
        /// 入库单主键
        /// </summary>
        [DataMember]
        public override string Key
        {
            get;
            set;
        }

        /// <summary>
        /// 入库单类型
        /// </summary>
        [DataMember]
        public virtual EnumStockInType BillType { get; set; }

        /// <summary>
        /// 入库单状态
        /// </summary>
        [DataMember]
        public virtual EnumBillState BillState { get; set; }

        /// <summary>
        /// 入库单接收核对状态
        /// </summary>
        [DataMember]
        public virtual EnumBillCheckState BillCheckState { get; set; }

        /// <summary>
        /// 入库日期
        /// </summary>
        [DataMember]
        public virtual DateTime? BillDate { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        public virtual string Note { get; set; }

        /// <summary>
        /// 制单人
        /// </summary>
        [DataMember]
        public virtual string BillMaker { get; set; }

        /// <summary>
        /// 制单日期
        /// </summary>
        [DataMember]
        public virtual DateTime? BillMakedDate { get; set; }

        /// <summary>
        /// 是否混装
        /// </summary>
        [DataMember]
        public virtual EnumMixType MixType { get; set; }

        /// <summary>
        /// 是否报废
        /// </summary>
        [DataMember]
        public virtual EnumScrapType ScrapType { get; set; }

        /// <summary>
        /// 入库仓库
        /// </summary>
        [DataMember]
        public virtual string Store { get; set; }

        /// <summary>
        /// 工单（弃用）
        /// </summary>
        [DataMember]
        public virtual string OrderNumber { get; set; }

        /// <summary>
        /// 产品编码（弃用）
        /// </summary>
        [DataMember]
        public virtual string MaterialCode { get; set; }
      
        /// <summary>
        /// 【非报废-入库单内托号总数量】 【报废-入库单内批次号总数量】
        /// </summary>
        [DataMember]
        public virtual decimal TotalQty { get; set; }

        /// <summary>
        /// ERP入库单号（弃用）
        /// </summary>
        [DataMember]
        public virtual string INCode { get; set; }     

        /// <summary>
        /// ERP报工单号
        /// </summary>
        [DataMember]
        public virtual string WRCode { get; set; }

        /// <summary>
        /// ERP报工单主键
        /// </summary>
        [DataMember]
        public virtual string ERPWRKey { get; set; }

        /// <summary>
        /// 编辑人
        /// </summary>
        [DataMember]
        public virtual string Editor { get; set; }

        /// <summary>
        /// 编辑时间
        /// </summary>
        [DataMember]
        public virtual DateTime? EditTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [DataMember]
        public virtual string Creator { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMember]
        public virtual DateTime CreateTime { get; set; }

        /// <summary>
        /// 客户端
        /// </summary>
        public virtual string OperateComputer { get; set; }
   
    }
   
}
