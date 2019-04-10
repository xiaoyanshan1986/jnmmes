using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;

namespace ServiceCenter.MES.Model.FMM
{
    /// <summary>
    /// 物料数据。
    /// </summary>
    [DataContract]
    public class Material : BaseModel<string>
    {
        /// <summary>
        /// 主键(物料编码）。
        /// </summary>
        [DataMember]
        public override string Key
        {
            get;
            set;
        }

        /// <summary>
        /// 物料名称
        /// </summary>
        [DataMember]
        public virtual string Name { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        [DataMember]
        public virtual string ModelName { get; set; }

        /// <summary>
        /// 规格。
        /// </summary>
        [DataMember]
        public virtual string Spec { get; set; }
        /// <summary>
        /// 单位。
        /// </summary>
        [DataMember]
        public virtual string Unit { get; set; }
        /// <summary>
        /// 类型。
        /// </summary>
        [DataMember]
        public virtual string Type { get; set; }
        /// <summary>
        /// 物料分类。
        /// </summary>
        [DataMember]
        public virtual string Class { get; set; }
        /// <summary>
        /// 条码。
        /// </summary>
        [DataMember]
        public virtual string BarCode { get; set; }
        /// <summary>
        /// 每批主材料数量。
        /// </summary>
        [DataMember]
        public virtual double MainRawQtyPerLot { get; set; }
        /// <summary>
        /// 每批产品数量。
        /// </summary>
        [DataMember]
        public virtual double MainProductQtyPerLot { get; set; }
        /// <summary>
        /// 是否是原材料。
        /// </summary>
        [DataMember]
        public virtual bool IsRaw{get;set;}
        /// <summary>
        /// 是否是产品。
        /// </summary>
        [DataMember]
        public virtual bool IsProduct { get; set; }

        /// <summary>
        /// 描述。
        /// </summary>
        [DataMember]
        public virtual string Description { get; set; }
        /// <summary>
        /// 状态。
        /// </summary>
        [DataMember]
        public virtual EnumObjectStatus Status { get; set; }
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
