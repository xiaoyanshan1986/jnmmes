using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;
using System.ComponentModel.DataAnnotations;
using ServiceCenter.MES.Model.ZPVM.Resources;
using ServiceCenter.Common;

namespace ServiceCenter.MES.Model.ZPVM
{
    /// <summary>
    /// 表示工单产品控制参数主键。
    /// </summary>
    public struct ProductControlObjectKey
    {
        /// <summary>
        /// 产品编号。
        /// </summary>
        public string ProductCode { get; set; }
        /// <summary>
        /// 电池片效率。
        /// </summary>
        public string CellEff { get; set; }
        /// <summary>
        /// 供应商编码。
        /// </summary>
        public string SupplierCode { get; set; }
        /// <summary>
        /// 控制对象。
        /// </summary>
        public EnumPVMTestDataType Object { get; set; }
        /// <summary>
        /// 控制类型（运算符）。
        /// </summary>
        public string Type { get; set; }

        public override string ToString()
        {
            return string.Format("{0}:{1}{2}({3}{4})", this.ProductCode, this.CellEff,this.SupplierCode,this.Object.GetDisplayName(), this.Type);
        }
    }
    /// <summary>
    /// 描述工单产品控制参数对象的数据模型
    /// </summary>
    [DataContract]
    public class ProductControlObject : BaseModel<ProductControlObjectKey>
    {
        /// <summary>
        /// 产品名称。
        /// </summary>
        [DataMember]
        public virtual string ProductName { get; set; }
        /// <summary>
        /// 供应商名称。
        /// </summary>
        [DataMember]
        public virtual string SupplierName { get; set; }
        /// <summary>
        /// 控制值。
        /// </summary>
        [DataMember]
        public virtual double Value { get; set; }
        /// <summary>
        /// 是否可用。
        /// </summary>
        [DataMember]
        public virtual bool IsUsed { get; set; }
        /// <summary>
        /// 创建人。
        /// </summary>
        [DataMember]
        public virtual  string Creator { get; set; }
        /// <summary>
        /// 创建时间。
        /// </summary>
        [DataMember]
        public virtual  DateTime CreateTime { get; set; }
        /// <summary>
        /// 编辑人。
        /// </summary>
        [DataMember]
        public virtual  string Editor { get; set; }
        /// <summary>
        /// 编辑时间。
        /// </summary>
        [DataMember]
        public virtual  DateTime EditTime { get; set; }

    }
}
