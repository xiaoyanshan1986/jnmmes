using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ServiceCenter.MES.Model.ERP
{
    /// <summary>
    /// 入库单据内托号核对状态枚举。
    /// </summary>
    public enum EnumPackageCheckState
    {
        /// <summary>
        /// 未核对
        /// </summary>
        [Display(Name = "未核对")]
        NoCheck = 0,

        /// <summary>
        /// 已核对
        /// </summary>
        [Display(Name = "已核对")]
        Checked = 1
    }

    /// <summary>
    /// 入库单明细主键
    /// </summary>
    public struct WOReportDetailKey
    {
        /// <summary>
        /// MES入库工单号
        /// </summary>
        public string BillCode { get; set; }

        /// <summary>
        /// 项目号
        /// </summary>
        public int ItemNo { get; set; }
        
        /// <summary>
        /// 入库单明细主键序列化
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("({0}:{1})", this.BillCode, this.ItemNo.ToString());
        }
    }

    /// <summary>
    /// 入库单明细类对象
    /// </summary>
    [DataContract]
    public class WOReportDetail : BaseModel<WOReportDetailKey>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public WOReportDetail()
        {

        }
        
        /// <summary>
        /// 托号
        /// </summary>
        [DataMember]
        public virtual string ObjectNumber { get; set; }

        /// <summary>
        /// 入库单据内托号核对状态
        /// </summary>
        [DataMember]
        public virtual EnumPackageCheckState PackageCheckState { get; set; }

        /// <summary>
        /// 工单号
        /// </summary>
        [DataMember]
        public virtual string OrderNumber { get; set; }

        /// <summary>
        /// 功率档
        /// </summary>
        [DataMember]
        public virtual string EffiName { get; set; }

        /// <summary>
        /// 子档位 
        /// </summary>
        [DataMember]
        public virtual string PsSubcode { get; set; }

        /// <summary>
        /// 产品编码
        /// </summary>
        [DataMember]
        public virtual string MaterialCode { get; set; }
        
        /// <summary>
        /// 实际效率合计
        /// </summary>
        [DataMember]
        public virtual decimal SumCoefPMax { get; set; }

        /// <summary>
        /// 等级
        /// </summary>
        [DataMember]
        public virtual string Grade { get; set; }

        /// <summary>
        /// 分档编号
        /// </summary>
        [DataMember]
        public virtual string EffiCode { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        [DataMember]
        public virtual decimal Qty { get; set; }

        /// <summary>
        /// ERP入库单号
        /// </summary>
        [DataMember]
        public virtual string ERPStockInCode { get; set; }

        /// <summary>
        /// ERP入库单主键
        /// </summary>
        [DataMember]
        public virtual string ERPStockInKey { get; set; }

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
