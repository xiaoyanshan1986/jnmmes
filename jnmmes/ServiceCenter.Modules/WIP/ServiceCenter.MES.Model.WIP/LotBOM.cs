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
    /// 批次物料来源枚举。
    /// </summary>
    public enum EnumMaterialFrom
    {
        /// <summary>
        /// 线边仓数据。
        /// </summary>
        LineStore=0,
        /// <summary>
        /// 上料数据。
        /// </summary>
        Loading=1
    }
    /// <summary>
    /// 批次BOM主键。
    /// </summary>
    public struct LotBOMKey
    {
        /// <summary>
        /// 批次号。
        /// </summary>
        public string LotNumber { get; set; }
        /// <summary>
        /// 物料批次号。
        /// </summary>
        public string MaterialLot { get; set; }
        /// <summary>
        /// 项目号。
        /// </summary>
        public int ItemNo { get; set; }
    }

    /// <summary>
    /// 描述批次BOM的模型类。
    /// </summary>
    [DataContract]
    public class LotBOM : BaseModel<LotBOMKey>
    {
        /// <summary>
        /// 事务操作主键。
        /// </summary>
        [DataMember]
        public virtual string TransactionKey { get; set; }
        /// <summary>
        /// 线边仓名称。
        /// </summary>
        [DataMember]
        public virtual string LineStoreName { get; set; }
        /// <summary>
        /// 物料编码。
        /// </summary>
        [DataMember]
        public virtual string MaterialCode { get; set; }
        /// <summary>
        /// 物料名称。
        /// </summary>
        [DataMember]
        public virtual string MaterialName { get; set; }
        /// <summary>
        /// 数量。
        /// </summary>
        [DataMember]
        public virtual double Qty { get; set; }

        /// <summary>
        /// 供应商代码。
        /// </summary>
        [DataMember]
        public virtual string SupplierCode { get; set; }
        /// <summary>
        /// 供应商名称。
        /// </summary>
        [DataMember]
        public virtual string SupplierName { get; set; }
        /// <summary>
        /// 工艺流程组名称。
        /// </summary>
        [DataMember]
        public virtual string RouteEnterpriseName { get; set; }
        /// <summary>
        ///工艺流程名称。
        /// </summary>
        [DataMember]
        public virtual string RouteName { get; set; }
        /// <summary>
        /// 工步名称。
        /// </summary>
        [DataMember]
        public virtual string RouteStepName { get; set; }
        /// <summary>
        /// 线别代码。
        /// </summary>
        [DataMember]
        public virtual string LineCode { get; set; }
        /// <summary>
        /// 设备代码。
        /// </summary>
        [DataMember]
        public virtual string EquipmentCode { get; set; }
        /// <summary>
        /// 物料来源。
        /// </summary>
        [DataMember]
        public virtual EnumMaterialFrom MaterialFrom { get; set; }
        /// <summary>
        /// 上料主键。
        /// </summary>
        [DataMember]
        public virtual string LoadingKey { get; set; }
        /// <summary>
        /// 上料项目号。
        /// </summary>
        [DataMember]
        public virtual int? LoadingItemNo { get; set; }
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
