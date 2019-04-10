using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;

namespace Astronergy.ServiceCenter.Model.WIP
{
    /// <summary>
    /// 批次过站记录数据。
    /// </summary>
    [DataContract]
    public class WIP_LOT:BaseModel<string>
    {
        /// <summary>
        /// 操作主键。
        /// </summary>
        public override string Key
        {
            get { return this.TransactionKey; }
        }
        /// <summary>
        /// 批次记录主键。
        /// </summary>
        [DataMember]
        public string TransactionKey { get; set; }

        /// <summary>
        /// 批次主键。
        /// </summary>
        [DataMember]
        public string LotKey { get; set; }

        /// <summary>
        /// 批次号。
        /// </summary>
        [DataMember]
        public string LotNumber { get; set; }

        /// <summary>
        /// 工单主键。
        /// </summary>
        [DataMember]
        public string WorkOrderKey { get; set; }

        /// <summary>
        /// 工单号。
        /// </summary>
        [DataMember]
        public string WorkOrderNo { get; set; }

        /// <summary>
        /// 在工单中的索引号。
        /// </summary>
        [DataMember]
        public string WorkOrderSeq { get; set; }

        /// <summary>
        /// 成品主键。
        /// </summary>
        [DataMember]
        public string PartVerKey { get; set; }

        /// <summary>
        /// 成品编码。
        /// </summary>
        [DataMember]
        public string PartNumber { get; set; }

        /// <summary>
        /// 产品ID号。
        /// </summary>
        [DataMember]
        public string ProId { get; set; }

        /// <summary>
        /// 优先级，最高 1 -> 10 最低，默认 5。
        /// </summary>
        [DataMember]
        public double Priority { get; set; }

        /// <summary>
        /// 产品初始化数量。
        /// </summary>
        [DataMember]
        public double QualityInitial { get; set; }

        /// <summary>
        /// 批次当前数量。
        /// </summary>
        [DataMember]
        public double Quality { get; set; }

        /// <summary>
        /// 工艺流程组主键。
        /// </summary>
        [DataMember]
        public string RouteEnterpriseVerKey { get; set; }

        /// <summary>
        /// 当前工艺流程主键。
        /// </summary>
        [DataMember]
        public string CurRouteVerKey { get; set; }

        /// <summary>
        /// 当前工步主键。
        /// </summary>
        [DataMember]
        public string CurStepVerKey { get; set; }

        /// <summary>
        /// 当前生产线主键。
        /// </summary>
        [DataMember]
        public string CurProductionLineKey { get; set; }

        /// <summary>
        /// 批次当前所在线别名称。
        /// </summary>
        [DataMember]
        public string LineName { get; set; }

        /// <summary>
        /// 开始等待时间。
        /// </summary>
        [DataMember]
        public DateTime StartWaitTime { get; set; }

        /// <summary>
        /// 开始处理时间。
        /// </summary>
        [DataMember]
        public DateTime StratProcessTime { get; set; }

        /// <summary>
        /// 批次采集主键。
        /// </summary>
        [DataMember]
        public string EdcInsKey { get; set; }

        /// <summary>
        /// 批次当前数量。
        /// </summary>
        [DataMember]
        public double StateFlag { get; set; }

        /// <summary>
        /// 批次当前数量。
        /// </summary>
        [DataMember]
        public double IsMainLot { get; set; }

        /// <summary>
        /// 批次当前数量。
        /// </summary>
        [DataMember]
        public double SplitFlag { get; set; }

        /// <summary>
        /// 批次当前数量。
        /// </summary>
        [DataMember]
        public double LotSeq { get; set; }

        /// <summary>
        /// 批次当前数量。
        /// </summary>
        [DataMember]
        public double ReworkFlag { get; set; }

        /// <summary>
        /// 批次当前数量。
        /// </summary>
        [DataMember]
        public double HoldFlag { get; set; }

        /// <summary>
        /// 批次当前数量。
        /// </summary>
        [DataMember]
        public double ShippedFlag { get; set; }

        /// <summary>
        /// 批次当前数量。
        /// </summary>
        [DataMember]
        public double DeletedTermFlag { get; set; }

        /// <summary>
        /// 批次当前数量。
        /// </summary>
        [DataMember]
        public double IsPrint { get; set; }

        /// <summary>
        /// 批次采集主键。
        /// </summary>
        [DataMember]
        public string LotType { get; set; }

        /// <summary>
        /// 批次采集主键。
        /// </summary>
        [DataMember]
        public string CreatType { get; set; }

        /// <summary>
        /// 批次采集主键。
        /// </summary>
        [DataMember]
        public string Color { get; set; }

        /// <summary>
        /// 批次采集主键。
        /// </summary>
        [DataMember]
        public string Operator { get; set; }

        /// <summary>
        /// 批次采集主键。
        /// </summary>
        [DataMember]
        public string OprLine { get; set; }

        /// <summary>
        /// 批次采集主键。
        /// </summary>
        [DataMember]
        public string OprComputer { get; set; }

        /// <summary>
        /// 批次采集主键。
        /// </summary>
        [DataMember]
        public string OprLinePre { get; set; }

        /// <summary>
        /// 批次采集主键。
        /// </summary>
        [DataMember]
        public string ChildLine { get; set; }

        /// <summary>
        /// 批次采集主键。
        /// </summary>
        [DataMember]
        public string MaterialCode { get; set; }

        /// <summary>
        /// 批次采集主键。
        /// </summary>
        [DataMember]
        public string MaterialLot { get; set; }

        /// <summary>
        /// 批次采集主键。
        /// </summary>
        [DataMember]
        public string SupplierName { get; set; }

        /// <summary>
        /// 批次采集主键。
        /// </summary>
        [DataMember]
        public string SiLot { get; set; }

        /// <summary>
        /// 批次采集主键。
        /// </summary>
        [DataMember]
        public string Efficiency { get; set; }

        /// <summary>
        /// 批次采集主键。
        /// </summary>
        [DataMember]
        public string FactoryRoomKey { get; set; }

        /// <summary>
        /// 批次采集主键。
        /// </summary>
        [DataMember]
        public string FactoryRoomName { get; set; }

        /// <summary>
        /// 批次采集主键。
        /// </summary>
        [DataMember]
        public string CreateOperationName { get; set; }

        /// <summary>
        /// 批次采集主键。
        /// </summary>
        [DataMember]
        public string ShiftName { get; set; }

        /// <summary>
        /// 批次采集主键。
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// 批次采集主键。
        /// </summary>
        [DataMember]
        public string Creator { get; set; }

        /// <summary>
        /// 批次采集主键。
        /// </summary>
        [DataMember]
        public string CreateTime { get; set; }

        /// <summary>
        /// 批次采集主键。
        /// </summary>
        [DataMember]
        public string CreateTimeZoneKey { get; set; }

        /// <summary>
        /// 批次采集主键。
        /// </summary>
        [DataMember]
        public string Editor { get; set; }

        /// <summary>
        /// 批次采集主键。
        /// </summary>
        [DataMember]
        public DateTime EditTime { get; set; }

        /// <summary>
        /// 批次采集主键。
        /// </summary>
        [DataMember]
        public string EditTimeZone { get; set; }

        /// <summary>
        /// 批次采集主键。
        /// </summary>
        [DataMember]
        public string ProLevel { get; set; }

        /// <summary>
        /// 批次采集主键。
        /// </summary>
        [DataMember]
        public string PalletNo { get; set; }

        /// <summary>
        /// 批次采集主键。
        /// </summary>
        [DataMember]
        public DateTime PalletTime { get; set; }

        /// <summary>
        /// 批次采集主键。
        /// </summary>
        [DataMember]
        public string LotSideCode { get; set; }

        /// <summary>
        /// 批次采集主键。
        /// </summary>
        [DataMember]
        public string LotCustomerCode { get; set; }

        /// <summary>
        /// 批次采集主键。
        /// </summary>
        [DataMember]
        public string EtlFlag { get; set; }

        /// <summary>
        /// 批次采集主键。
        /// </summary>
        [DataMember]
        public string SmallPackNumber { get; set; }         

    }
}
