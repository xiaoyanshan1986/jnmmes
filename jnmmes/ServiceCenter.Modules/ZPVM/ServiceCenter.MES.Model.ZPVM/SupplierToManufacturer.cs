using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;
using System.ComponentModel.DataAnnotations;
using ServiceCenter.MES.Model.ZPVM.Resources;

namespace ServiceCenter.MES.Model.ZPVM
{    
    /// <summary>
    /// 表示转换规则主键。
    /// </summary>
    public struct SupplierToManufacturerKey
    {
        /// <summary>
        /// 物料编码。
        /// </summary>
        public string MaterialCode { get; set; }

        /// <summary>
        /// 生产工单。
        /// </summary>
        public string OrderNumber { get; set; }

        /// <summary>
        /// 供应商代码。
        /// </summary>
        public string SupplierCode { get; set; }        

        public override string ToString()
        {
            return string.Format("{0}:{1}:{2}", this.MaterialCode, this.OrderNumber, this.SupplierCode);
        }
    }
    /// <summary>
    /// 描述数据模型
    /// </summary>
    [DataContract]
    public class SupplierToManufacturer : BaseModel<SupplierToManufacturerKey>
    {
              
        /// <summary>
        /// 生产厂商代码。
        /// </summary>
        [DataMember]
        public virtual string ManufacturerCode { get; set; }

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
              
    }
}
