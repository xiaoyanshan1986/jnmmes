using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;

namespace ServiceCenter.MES.Model.LSM
{
    /// <summary>
    /// 领料单明细数据主键。
    /// </summary>
    public struct MaterialReceiptDetailKey
    {
        /// <summary>
        /// 领料单号。
        /// </summary>
        public string ReceiptNo { get; set; }
        /// <summary>
        /// 项目号。
        /// </summary>
        public int ItemNo { get; set; }

        public override string ToString()
        {
            return string.Format("{0}({1})", this.ReceiptNo, this.ItemNo);
        }
    }

    /// <summary>
    /// 描述领料单明细数据的模型类。
    /// </summary>
    [DataContract]
    public class MaterialReceiptDetail : BaseModel<MaterialReceiptDetailKey>
    {
        public MaterialReceiptDetail()
        {
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;
        }
        /// <summary>
        /// 线边仓名称。
        /// </summary>
        [DataMember]
        public virtual string LineStoreName
        {
            get;
            set;
        }

        /// <summary>
        /// 物料代码。
        /// </summary>
        [DataMember]
        public virtual string MaterialCode
        {
            get;
            set;
        }
        /// <summary>
        /// 物料批号。
        /// </summary>
        [DataMember]
        public virtual string MaterialLot
        {
            get;
            set;
        }
        /// <summary>
        /// 数量。
        /// </summary>
        [DataMember]
        public virtual double Qty { get; set; }


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
        /// 供应商代码。
        /// </summary>
        [DataMember]
        public virtual string SupplierCode { get; set; }
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
