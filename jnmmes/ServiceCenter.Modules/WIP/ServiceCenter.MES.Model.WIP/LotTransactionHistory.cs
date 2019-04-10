using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;

namespace ServiceCenter.MES.Model.WIP
{
    /// <summary>
    /// 描述批次事务前批次数据的模型类。
    /// </summary>
    [DataContract]
    public class LotTransactionHistory : BaseModel<string>
    {
        /// <summary>
        /// 构造函数。
        /// </summary>
        public LotTransactionHistory()
        {

        }
        /// <summary>
        /// 构造函数。
        /// </summary>
        public LotTransactionHistory(string transactionKey,Lot obj)
        {
            this.Key = transactionKey;
            this.LotNumber=obj.Key;
            this.ContainerNo=obj.ContainerNo;
            this.CreateTime=obj.CreateTime;
            this.Creator = obj.Creator;
            this.DeletedFlag = obj.DeletedFlag;
            this.Description = obj.Description;
            this.Editor = obj.Editor;
            this.EditTime = obj.EditTime;
            this.EquipmentCode = obj.EquipmentCode;
            this.Grade = obj.Grade;
            this.HoldFlag = obj.HoldFlag;
            this.InitialQuantity = obj.InitialQuantity;
            this.IsMainLot = obj.IsMainLot;
            this.LineCode = obj.LineCode;
            this.LocationName = obj.LocationName;
            this.LotType = obj.LotType;
            this.MaterialCode = obj.MaterialCode;
            this.OperateComputer = obj.OperateComputer;
            this.OrderNumber = obj.OrderNumber;
            this.OriginalOrderNumber = obj.OriginalOrderNumber;
            this.PackageFlag = obj.PackageFlag;
            this.PackageNo = obj.PackageNo;
            this.PreLineCode = obj.PreLineCode;
            this.Priority = obj.Priority;
            this.Quantity = obj.Quantity;
            this.RepairFlag = obj.RepairFlag;
            this.Reworker = obj.Reworker;
            this.ReworkFlag = obj.ReworkFlag;
            this.ReworkTime = obj.ReworkTime;
            this.RouteEnterpriseName = obj.RouteEnterpriseName;
            this.RouteName = obj.RouteName;
            this.RouteStepName = obj.RouteStepName;
            this.ShippedFlag = obj.ShippedFlag;
            this.SplitFlag = obj.SplitFlag;
            this.StartProcessTime = obj.StartProcessTime;
            this.StartWaitTime = obj.StartWaitTime;
            this.StateFlag = obj.StateFlag;
            this.Status = obj.Status;
            this.Color = obj.Color;
            this.Attr1 = obj.Attr1;
            this.Attr2 = obj.Attr2;
            this.Attr3 = obj.Attr3;
            this.Attr4 = obj.Attr4;
            this.Attr5 = obj.Attr5;
        }
        /// <summary>
        /// 批次号。
        /// </summary>
        [DataMember]
        public virtual string LotNumber { get; set; }
        /// <summary>
        /// 容器号。
        /// </summary>
        [DataMember]
        public virtual  string ContainerNo { get; set; }
        /// <summary>
        /// 批次类型。
        /// </summary>
        [DataMember]
        public virtual EnumLotType LotType { get; set; }
        /// <summary>
        /// 原始工单号。
        /// </summary>
        [DataMember]
        public virtual string OriginalOrderNumber { get; set; }
        /// <summary>
        /// 当前工单号。
        /// </summary>
        [DataMember]
        public virtual  string OrderNumber { get; set; }
        /// <summary>
        /// 物料编码。
        /// </summary>
        [DataMember]
        public virtual  string MaterialCode { get; set; }
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
        /// 优先级。
        /// </summary>
        [DataMember]
        public virtual EnumLotPriority Priority { get; set; }
        /// <summary>
        /// 初始数量。
        /// </summary>
        [DataMember]
        public virtual double InitialQuantity { get; set; }
        /// <summary>
        /// 当前数量。
        /// </summary>
        [DataMember]
        public virtual  double Quantity { get; set; }
        /// <summary>
        /// 工艺流程组名称。
        /// </summary>
        [DataMember]
        public virtual  string RouteEnterpriseName { get; set; }
        /// <summary>
        /// 工艺流程名称。
        /// </summary>
        [DataMember]
        public virtual  string RouteName { get; set; }
        /// <summary>
        /// 工步名称。
        /// </summary>
        [DataMember]
        public virtual  string RouteStepName { get; set; }
        /// <summary>
        /// 生产线代码。
        /// </summary>
        [DataMember]
        public virtual  string LineCode { get; set; }
        /// <summary>
        /// 设备代码。
        /// </summary>
        [DataMember]
        public virtual  string EquipmentCode { get; set; }
        /// <summary>
        /// 开始等待时间。
        /// </summary>
        [DataMember]
        public virtual  DateTime? StartWaitTime { get; set; }
        /// <summary>
        /// 开始处理时间。
        /// </summary>
        [DataMember]
        public virtual  DateTime? StartProcessTime { get; set; }
        /// <summary>
        /// 是否主批次。
        /// </summary>
        [DataMember]
        public virtual bool IsMainLot { get; set; }
        /// <summary>
        /// 拆分状态。
        /// </summary>
        [DataMember]
        public virtual bool SplitFlag { get; set; }
        /// <summary>
        /// 返修标志。 0:未返修 >0：返修次数
        /// </summary>
        [DataMember]
        public virtual int RepairFlag { get; set; }
        /// <summary>
        /// 返工标志。0:未返工 >0：返工次数
        /// </summary>
        [DataMember]
        public virtual int ReworkFlag { get; set; }
        /// <summary>
        /// 暂停状态。
        /// </summary>
        [DataMember]
        public virtual bool HoldFlag { get; set; }
        /// <summary>
        /// 出货标志。
        /// </summary>
        [DataMember]
        public virtual bool ShippedFlag { get; set; }
        /// <summary>
        /// 包装标记。
        /// </summary>
        [DataMember]
        public virtual bool PackageFlag { get; set; }
        /// <summary>
        /// 包装号。
        /// </summary>
        [DataMember]
        public virtual string PackageNo { get; set; }
        /// <summary>
        /// 结束删除标志。
        /// </summary>
        [DataMember]
        public virtual bool DeletedFlag { get; set; }
        /// <summary>
        /// 批次操作状态。
        /// </summary>
        [DataMember]
        public virtual EnumLotState StateFlag { get; set; }
        /// <summary>
        /// 状态。
        /// </summary>
        [DataMember]
        public virtual  EnumObjectStatus Status { get; set; }
        /// <summary>
        /// 操作计算机名称。
        /// </summary>
        [DataMember]
        public virtual  string OperateComputer { get; set; }
        /// <summary>
        /// 上一线别代码。
        /// </summary>
        [DataMember]
        public virtual  string PreLineCode { get; set; }
        /// <summary>
        /// 返工操作人。
        /// </summary>
        [DataMember]
        public virtual string Reworker { get; set; }
        /// <summary>
        /// 返工时间。
        /// </summary>
        [DataMember]
        public virtual DateTime? ReworkTime { get; set; }
        /// <summary>
        /// 描述。
        /// </summary>
        [DataMember]
        public virtual string Description { get; set; }
        /// <summary>
        /// 区域名称。
        /// </summary>
        [DataMember]
        public virtual  string LocationName{ get; set; }
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
        /// <summary>
        /// 创建人。
        /// </summary>
        [DataMember]
        public virtual  string Creator { get; set; }
        /// <summary>
        /// 创建时间。
        /// </summary>
        [DataMember]
        public virtual  DateTime? CreateTime { get; set; }
        /// <summary>
        /// 编辑人。
        /// </summary>
        [DataMember]
        public virtual  string Editor { get; set; }
        /// <summary>
        /// 编辑时间。
        /// </summary>
        [DataMember]
        public virtual  DateTime? EditTime { get; set; }
    }
}
