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
    /// 表示替换规则主键。
    /// </summary>
    public struct MaterialReplaceKey
    {
        /// <summary>
        /// 产品编码。
        /// </summary>
        public string ProductCode { get; set; }
        /// <summary>
        /// 工单号。
        /// </summary>
        public string OrderNumber { get; set; }
        /// <summary>
        /// 原物料编码。
        /// </summary>
        public string OldMaterialCode { get; set; }
        /// <summary>
        /// 原物料供应商。
        /// </summary>
        public string OldMaterialSupplier { get; set; }

        public override string ToString()
        {
            return string.Format("{0}:{1}:{2}:{3}", this.ProductCode, this.OrderNumber, this.OldMaterialCode, this.OldMaterialSupplier);
        }
    }
    /// <summary>
    /// 描述分档数据模型
    /// </summary>
    [DataContract]
    public class MaterialReplace : BaseModel<MaterialReplaceKey>
    {
              
        /// <summary>
        /// 现物料编码。
        /// </summary>
        [DataMember]
        public virtual string NewMaterialCode { get; set; }
        /// <summary>
        /// 现物料供应商。
        /// </summary>
        [DataMember]
        public virtual string NewMaterialSupplier { get; set; }

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

        /// <summary>
        /// 描述。
        /// </summary>
        [DataMember]
        public virtual string Description { get; set; }
    }
}
