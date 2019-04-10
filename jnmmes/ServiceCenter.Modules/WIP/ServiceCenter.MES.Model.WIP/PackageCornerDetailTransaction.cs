using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;

namespace ServiceCenter.MES.Model.WIP
{
    /// 描述包装明细数据的模型类。
    /// </summary>
    [DataContract]
    public class PackageCornerDetailTransaction : BaseModel<PackageCornerDetailKey>
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
