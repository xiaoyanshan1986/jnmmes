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
    /// 领料单明细类替换后对象
    /// </summary>
    [DataContract]
    public class MaterialReceiptReplace : BaseModel<int>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public MaterialReceiptReplace()
        {

        }

        /// <summary>
        /// 主键（项目号）
        /// </summary>
        public override int Key
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
        public virtual string OrderNumber { get; set; }

        /// <summary>
        /// 现物料编码。
        /// </summary>
        [DataMember]
        public virtual string MaterialCode { get; set; }

        /// <summary>
        /// 现物料批号。
        /// </summary>
        [DataMember]
        public virtual string MaterialLot { get; set; }

        /// <summary>
        /// 数量。
        /// </summary>
        [DataMember]
        public virtual double Qty { get; set; }

        /// <summary>
        /// 电池片效率
        /// </summary>
        [DataMember]
        public virtual string CellPower { get; set; }

        /// <summary>
        /// 现电池片颜色
        /// </summary>
        [DataMember]
        public virtual string CellColor { get; set; }

        /// <summary>
        /// 电池片等级
        /// </summary>
        [DataMember]
        public virtual string CellGrade { get; set; }

        /// <summary>
        /// 原供应商代码
        /// </summary>
        [DataMember]
        public virtual string OldSupplierCode { get; set; }

        /// <summary>
        /// 原供应商名称
        /// </summary>
        [DataMember]
        public virtual string OldSupplierName { get; set; }

        /// <summary>
        /// 原物料编码
        /// </summary>
        [DataMember]
        public virtual string OldMaterialCode { get; set; }

        /// <summary>
        /// 原电池片颜色
        /// </summary>
        [DataMember]
        public virtual string OldCellColor { get; set; }

        /// <summary>
        /// 原物料批号。
        /// </summary>
        [DataMember]
        public virtual string OldMaterialLot { get; set; }

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
        /// 生产厂商代码。
        /// </summary>
        [DataMember]
        public virtual string ManufacturerCode { get; set; }

        /// <summary>
        /// 生产厂商名称。
        /// </summary>
        [DataMember]
        public virtual string ManufacturerName { get; set; }

        /// <summary>
        /// 原生产厂商代码。
        /// </summary>
        [DataMember]
        public virtual string OldManufacturerCode { get; set; }

        /// <summary>
        /// 原生产厂商名称。
        /// </summary>
        [DataMember]
        public virtual string OldManufacturerName { get; set; }

        /// <summary>
        /// 供应商物料批号。
        /// </summary>
        [DataMember]
        public virtual string SupplierMaterialLot { get; set; }

        /// <summary>
        /// 描述。
        /// </summary>
        [DataMember]
        public virtual string Description { get; set; }
        
    }
}
