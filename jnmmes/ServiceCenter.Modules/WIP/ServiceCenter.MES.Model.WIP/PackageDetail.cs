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
    public enum EnumPackageObjectType
    {
        /// <summary>
        /// 批次。
        /// </summary>
        Lot=0,
        /// <summary>
        /// 小包。
        /// </summary>
        Packet=1
    }
    /// <summary>
    /// 包装明细主键。
    /// </summary>
    public struct PackageDetailKey
    {
        /// <summary>
        /// 包装号。
        /// </summary>
        [DataMember]
        public string PackageNo { get; set; }
        /// <summary>
        /// 对象类型。
        /// </summary>
        [DataMember]
        public EnumPackageObjectType ObjectType { get; set; }
        /// <summary>
        /// 对象标识号。
        /// </summary>
        [DataMember]
        public string ObjectNumber { get; set; }

        public override string ToString()
        {
            return string.Format("{0}：{1}", this.PackageNo, this.ObjectNumber);
        }
    }
    /// <summary>
    /// 描述包装明细数据的模型类。
    /// </summary>
    [DataContract]
    public class PackageDetail : BaseModel<PackageDetailKey>
    {
        /// <summary>
        /// 项目号。
        /// </summary>
        [DataMember]
        public virtual int ItemNo { get; set; }
        /// 工单号。
        /// </summary>
        [DataMember]
        public virtual string OrderNumber { get; set; }
        /// <summary>
        /// 物料编码。
        /// </summary>
        [DataMember]
        public virtual string MaterialCode { get; set; }
      
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
