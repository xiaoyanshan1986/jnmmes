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
    /// 物料BOM主键。
    /// </summary>
    public struct MaterialBOMKey
    {
        /// <summary>
        /// 物料号。
        /// </summary>
        public string MaterialCode { get; set; }
        /// <summary>
        /// 项目号。
        /// </summary>
        public int ItemNo { get; set; }

        public override string ToString()
        {
            return string.Format("{0}:{1}", this.MaterialCode, this.ItemNo);
        }
    }

    /// <summary>
    /// 描述物料BOM的模型类。
    /// </summary>
    [DataContract]
    public class MaterialBOM : BaseModel<MaterialBOMKey>
    {
        /// <summary>
        /// 原材料编码。
        /// </summary>
        [DataMember]
        public virtual string RawMaterialCode { get; set; }

        /// <summary>
        /// 数量。
        /// </summary>
        [DataMember]
        public virtual double Qty { get; set; }

        /// <summary>
        /// 单位。
        /// </summary>
        [DataMember]
        public virtual string MaterialUnit { get; set; }

        /// <summary>
        /// 最小单元
        /// </summary>
        [DataMember]
        public virtual float MinUnit { get; set; }

        /// <summary>
        /// 可替代物料
        /// </summary>
        [DataMember]
        public virtual string ReplaceMaterial { get; set; }

        /// <summary>
        /// 工作中心。
        /// </summary>
        [DataMember]
        public virtual string WorkCenter { get; set; }

        /// <summary>
        /// 存储位置。
        /// </summary>
        [DataMember]
        public virtual string StoreLocation { get; set; }

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
