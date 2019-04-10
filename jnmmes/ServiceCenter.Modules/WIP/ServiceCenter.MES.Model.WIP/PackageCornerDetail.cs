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
    public enum EnumCornerDetailPackaged
    {
        /// <summary>
        /// 不包装护角
        /// </summary>
        UnPackaged = 0,
        /// <summary>
        /// 包装护角
        /// </summary>
        Packaged = 1
    }


    /// <summary>
    /// 包装明细主键。
    /// </summary>
    public struct PackageCornerDetailKey
    {
        /// <summary>
        /// 包装号。
        /// </summary>
        [DataMember]
        public string PackageKey { get; set; }
       
        /// <summary>
        /// 对象标识号。
        /// </summary>
        [DataMember]
        public string LotNumber { get; set; }

        public override string ToString()
        {
            return string.Format("{0}：{1}", this.PackageKey, this.LotNumber);
        }
    }
    /// <summary>
    /// 描述包装明细数据的模型类。
    /// </summary>
    [DataContract]
    public class PackageCornerDetail : BaseModel<PackageCornerDetailKey>
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
        public virtual DateTime CreateTime { get; set; }

        /// <summary>
        ///包护角标志
        /// </summary>
        [DataMember]
        public virtual int PackageFlag { get; set; }

        /// <summary>
        ///线别
        /// </summary>
        [DataMember]
        public virtual string PackageLine { get; set; }
    }
}
