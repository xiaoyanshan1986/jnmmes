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
    /// 线边仓物料明细数据主键。
    /// </summary>
    public struct LineStoreMaterialDetailKey
    {
        /// <summary>
        /// 线边仓名称。
        /// </summary>
        public string LineStoreName { get; set; }
        /// <summary>
        /// 工单号。
        /// </summary>
        public string OrderNumber { get; set; }
        /// <summary>
        /// 物料编码。
        /// </summary>
        public string MaterialCode { get; set; }
        /// <summary>
        /// 物料批号。
        /// </summary>
        public string MaterialLot { get; set; }

        public override string ToString()
        {
            return string.Format("{2}({0}:{1})", this.LineStoreName, this.MaterialCode, this.MaterialLot);
        }
    }
    /// <summary>
    /// 描述线边仓物料明细数据的模型类。
    /// </summary>
    [DataContract]
    public class LineStoreMaterialDetail : BaseModel<LineStoreMaterialDetailKey>
    {
        public LineStoreMaterialDetail()
        {
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;
        }
        /// <summary>
        /// 接收数量。
        /// </summary>
        [DataMember]
        public virtual double ReceiveQty { get; set; }
        /// <summary>
        /// 退回数量。
        /// </summary>
        [DataMember]
        public virtual double ReturnQty { get; set; }
        /// <summary>
        /// 报废数量。
        /// </summary>
        [DataMember]
        public virtual double ScrapQty { get; set; }
        /// <summary>
        /// 上料数量。
        /// </summary>
        [DataMember]
        public virtual double LoadingQty { get; set; }
        /// <summary>
        /// 下料数量。
        /// </summary>
        [DataMember]
        public virtual double UnloadingQty { get; set; }
        /// <summary>
        /// 当前数量。
        /// </summary>
        [DataMember]
        public virtual double CurrentQty { get; set; }
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
        /// /描述
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
