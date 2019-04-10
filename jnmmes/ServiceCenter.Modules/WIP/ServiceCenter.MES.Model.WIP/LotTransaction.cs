using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;
using System.ComponentModel.DataAnnotations;
using ServiceCenter.MES.Model.WIP.Resources;
using ServiceCenter.Common.Model;

namespace ServiceCenter.MES.Model.WIP
{

    public class LotProcessingParameter : BaseMethodParameter
    {
        /// <summary>
        /// 批次列表
        /// </summary>
        [DataMember]
        public string Lotlist { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>

        [DataMember]
        public string ErrorMsg { get; set; }
        /// <summary>
        /// 页号。
        /// </summary>
        [DataMember]
        public int PageNo { get; set; }
        /// <summary>
        /// 每页大小。
        /// </summary>
        [DataMember]
        public int PageSize { get; set; }
        /// <summary>
        /// 总记录数。
        /// </summary>
        [DataMember]
        public int TotalRecords { get; set; }

    }
    /// <summary>
    /// 批次操作枚举。
    /// </summary>
    public enum EnumLotActivity
    {
        /// <summary>
        /// -1:撤销。
        /// </summary>
        [Display(Name = "EnumLotActivity_Undo", ResourceType = typeof(StringResource))]
        Undo=-1,

        /// <summary>
        /// 0:创建。
        /// </summary>
        [Display(Name = "EnumLotActivity_Create", ResourceType = typeof(StringResource))]
        Create=0,

        /// <summary>
        /// 1:进站。
        /// </summary>
        [Display(Name = "EnumLotActivity_TrackIn", ResourceType = typeof(StringResource))]
        TrackIn=1,

        /// <summary>
        /// 2:出站。
        /// </summary>
        [Display(Name = "EnumLotActivity_TrackOut", ResourceType = typeof(StringResource))]
        TrackOut = 2,

        /// <summary>
        /// 3:暂停。
        /// </summary>
        [Display(Name = "EnumLotActivity_Hold", ResourceType = typeof(StringResource))]
        Hold = 3,

        /// <summary>
        /// 4:释放。
        /// </summary>
        [Display(Name = "EnumLotActivity_Release", ResourceType = typeof(StringResource))]
        Release = 4,

        /// <summary>
        /// 5:报废。
        /// </summary>
        [Display(Name = "EnumLotActivity_Scrap", ResourceType = typeof(StringResource))]
        Scrap = 5,

        /// <summary>
        /// 6:不良。
        /// </summary>
        [Display(Name = "EnumLotActivity_Defect", ResourceType = typeof(StringResource))]
        Defect = 6,

        /// <summary>
        /// 7:补料。
        /// </summary>
        [Display(Name = "EnumLotActivity_Patch", ResourceType = typeof(StringResource))]
        Patch = 7,

        /// <summary>
        /// 8:返修。
        /// </summary>
        [Display(Name = "EnumLotActivity_Repair", ResourceType = typeof(StringResource))]
        Repair = 8,

        /// <summary>
        /// 9:包装。
        /// </summary>
        [Display(Name = "EnumLotActivity_Package", ResourceType = typeof(StringResource))]
        Package = 9,

        /// <summary>
        /// 10:拆批。
        /// </summary>
        [Display(Name = "EnumLotActivity_Splited", ResourceType = typeof(StringResource))]
        Splited = 10,

        /// <summary>
        /// 11:拆创批。
        /// </summary>
        [Display(Name = "EnumLotActivity_SplitCreate", ResourceType = typeof(StringResource))]
        SplitCreate = 11,

        /// <summary>
        /// 12:合批。
        /// </summary>
        [Display(Name = "EnumLotActivity_Merge", ResourceType = typeof(StringResource))]
        Merge = 12,

        /// <summary>
        /// 13:被合批。
        /// </summary>
        [Display(Name = "EnumLotActivity_Merged", ResourceType = typeof(StringResource))]
        Merged = 13,

        /// <summary>
        /// 14:返工。
        /// </summary>
        [Display(Name = "EnumLotActivity_Rework", ResourceType = typeof(StringResource))]
        Rework = 14,

        /// <summary>
        /// 15:结束。
        /// </summary>
        [Display(Name = "EnumLotActivity_Terminal", ResourceType = typeof(StringResource))]
        Terminal=15,

        /// <summary>
        /// 16:转工单。
        /// </summary>
        [Display(Name = "EnumLotActivity_Change", ResourceType = typeof(StringResource))]
        Change = 16,

        /// <summary>
        /// 17:拆托。
        /// </summary>
        [Display(Name = "EnumLotActivity_UnPackage", ResourceType = typeof(StringResource))]
        UnPackage = 17,

        /// <summary>
        /// 18:修正IV数据。
        /// </summary>
        [Display(Name = "修正IV数据")]
        ModifyIVData = 18,

        /// <summary>
        /// 19:新增到入库单。
        /// </summary>
        [Display(Name = "入库新增")]
        AddInErp = 19,

        /// <summary>
        /// 20:入库申请。
        /// </summary>
        [Display(Name = "入库申请")]
        Apply = 20,

        /// <summary>
        /// 21:入库接收。
        /// </summary>
        [Display(Name = "入库接收")]
        Receieved = 21,

        /// <summary>
        /// 22:从入库单删除。
        /// </summary>
        [Display(Name = "入库删除")]
        Delete = 22
    }
    /// <summary>
    /// 描述批次操作事务数据的模型类。
    /// </summary>
    [DataContract]
    public class LotTransaction : BaseModel<string>
    {
        public LotTransaction()
        {
            this.UndoFlag = false;
        }
        /// <summary>
        /// 批次操作。
        /// </summary>
        [DataMember]
        public virtual EnumLotActivity Activity { get; set; }

        /// <summary>
        /// 批次号。
        /// </summary>
        [DataMember]
        public virtual string LotNumber { get; set; }
        /// <summary>
        /// 工单号。
        /// </summary>
        [DataMember]
        public virtual string OrderNumber { get; set; }
        /// <summary>
        /// 车间名称。
        /// </summary>
        [DataMember]
        public virtual string LocationName { get; set; }
        /// <summary>
        /// 线别。
        /// </summary>
        [DataMember]
        public virtual string LineCode { get; set; }
        /// <summary>
        /// 操作前数量。
        /// </summary>
        [DataMember]
        public virtual double InQuantity { get; set; }
        /// <summary>
        /// 操作后数量。
        /// </summary>
        [DataMember]
        public virtual double OutQuantity { get; set; }
        /// <summary>
        /// 工艺流程组名称。
        /// </summary>
        [DataMember]
        public virtual string RouteEnterpriseName { get; set; }
        /// <summary>
        /// 工艺流程名称。
        /// </summary>
        [DataMember]
        public virtual string RouteName { get; set; }
        /// <summary>
        /// 工艺流程工步名称。
        /// </summary>
        [DataMember]
        public virtual string RouteStepName { get; set; }
        /// <summary>
        /// 班别名称。
        /// </summary>
        [DataMember]
        public virtual string ShiftName { get; set; }
        /// <summary>
        /// 操作客户端名称。
        /// </summary>
        [DataMember]
        public virtual string OperateComputer { get; set; }
        /// <summary>
        /// 描述。
        /// </summary>
        [DataMember]
        public virtual string Description { get; set; }
        /// <summary>
        /// 撤销标记。0:否;1:是。
        /// </summary>
        [DataMember]
        public virtual bool UndoFlag { get; set; }
        /// <summary>
        /// 撤销操作主键。
        /// </summary>
        [DataMember]
        public virtual string UndoTransactionKey { get; set; }
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
        /// 创建时间。
        /// </summary>
        [DataMember]
        public virtual DateTime TimeStamp { get; set; }
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
        /// 等级。
        /// </summary>
        [DataMember]
        public virtual string Grade { get; set; }
        /// <summary>
        /// 颜色。
        /// </summary>
        [DataMember]
        public virtual string Color { get; set; }

        [DataMember]
        public virtual string Attr1 { get; set; }

        [DataMember]
        public virtual string Attr2 { get; set; }

        [DataMember]
        public virtual string Attr3 { get; set; }

        [DataMember]
        public virtual string Attr4 { get; set; }

        [DataMember]
        public virtual string Attr5 { get; set; }

        [DataMember]
        public virtual string OriginalOrderNumber { get; set; }
    }
}
