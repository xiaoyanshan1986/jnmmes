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
    /// 包装对象类型枚举。
    /// </summary>
    public enum EnumPackageOemObjectType
    {
    //    /// <summary>
    //    /// 批次。
    //    /// </summary>
    //    Lot = 0,
    //    /// <summary>
    //    /// 小包。
    //    /// </summary>
    //    Packet = 1
    }
    /// <summary>
    /// 包装明细主键。
    /// </summary>
    public struct PackageOemDetailKey
    {

        /// <summary>
        /// 包装号。
        /// </summary>
        [DataMember]
        public  string SN { get; set; }

    }
    /// <summary>
    /// 描述包装明细数据的模型类。
    /// </summary>
    [DataContract]
    public class PackageOemDetail : BaseModel<PackageOemDetailKey>
    {

        ///// <summary>
        ///// 序号。
        ///// </summary>
        //[DataMember]
        //public virtual int No { get; set; }
        /// <summary>
        /// 对象类型。
        /// </summary>
        [DataMember]
        public virtual string Type { get; set; }
        /// <summary>
        /// 批次号。
        /// </summary>
        [DataMember]
        public virtual string PackageNo { get; set; }    
        /// <summary>
        /// 实测功率。
        /// </summary>
        [DataMember]
        public virtual string PMP { get; set; }
        /// 短路电流。
        /// </summary>
        [DataMember]
        public virtual string ISC { get; set; }
        /// <summary>
        /// 开路电压。
        /// </summary>
        [DataMember]
        public virtual string VOC { get; set; }

        /// <summary>
        /// 工作电流。
        /// </summary>
        [DataMember]
        public virtual string IMP { get; set; }

        /// <summary>
        /// 工作电压。
        /// </summary>
        [DataMember]
        public virtual string VMP { get; set; }
        /// <summary>
        /// 填充因子。
        /// </summary>
        [DataMember]
        public virtual string FF { get; set; }
        /// <summary>
        /// 功率档。
        /// </summary>
        [DataMember]
        public virtual string PNOM { get; set; }
        /// <summary>
        /// 电流档。
        /// </summary>
        [DataMember]
        public virtual string DL{ get; set; }
        /// <summary>
        /// 时间。
        /// </summary>
        [DataMember]
        public virtual DateTime? Time { get; set; } 
    }
}
