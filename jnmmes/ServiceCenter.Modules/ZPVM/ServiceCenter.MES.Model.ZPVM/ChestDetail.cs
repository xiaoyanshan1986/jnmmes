using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;

namespace ServiceCenter.MES.Model.ZPVM
{
    /// <summary>
    /// 包装入柜对象类型枚举。
    /// </summary>
    public enum EnumChestObjectType
    {
        /// <summary>
        /// 托号。
        /// </summary>
        PackageNo=0
    }
    /// <summary>
    /// 包装入柜明细主键。
    /// </summary>
    public struct ChestDetailKey
    {
        /// <summary>
        /// 柜号。
        /// </summary>
        [DataMember]
        public string ChestNo { get; set; }
        /// <summary>
        /// 对象类型。
        /// </summary>
        [DataMember]
        public EnumChestObjectType ObjectType { get; set; }
        /// <summary>
        /// 对象标识号。
        /// </summary>
        [DataMember]
        public string ObjectNumber { get; set; }

        public override string ToString()
        {
            return string.Format("{0}：{1}", this.ChestNo, this.ObjectNumber);
        }
    }
    /// <summary>
    /// 描述包装入柜明细数据的模型类。
    /// </summary>
    [DataContract]
    public class ChestDetail : BaseModel<ChestDetailKey>
    {
        //[DataMember]
        ///// <summary>
        ///// 托对象
        ///// </summary>
        //public virtual Package Package { get; set; }

        /// <summary>
        /// 项目号。
        /// </summary>
        [DataMember]
        public virtual int ItemNo { get; set; }

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

        /// <summary>
        /// 工单号。
        /// </summary>
        [DataMember]
        public virtual string OrderNumber { get; set; } 
    }
}
