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
    public enum EnumCornerPackaged
    {
        /// <summary>
        /// 包装BIN未完成
        /// </summary>
        UnFinished = 0,
        /// <summary>
        /// 包装BIN完成状态
        /// </summary>
        Finished = 1
    }

   

    /// <summary>
    /// 描述包装护角的模型类。
    /// </summary>
    [DataContract]
    public class PackageCorner : BaseModel<string>
    {
        /// <summary>
        /// 主键（GUID）
        /// </summary>
        public override string Key
        {
            get
            {
                return base.Key;
            }
            set
            {
                base.Key = value;
            }
        }

        /// <summary>
        /// 线别
        /// </summary>
        [DataMember]
        public virtual string PackageLine { get; set; }

        /// <summary>
        /// Bin号
        /// </summary>
        [DataMember]
        public virtual string BinNo { get; set; }

        /// <summary>
        /// Bin累计数量
        /// </summary>
        [DataMember]
        public virtual int BinQty { get; set; }

        /// <summary>
        /// Bin最大数量
        /// </summary>
        [DataMember]
        public virtual int BinMaxQty { get; set; }
                
        /// <summary>
        /// Bin包装完成状态
        /// </summary>
        [DataMember]
        public virtual EnumCornerPackaged BinPackaged { get; set; }
                
        /// <summary>
        /// Bin状态
        /// </summary>
        [DataMember]
        public virtual int BinState { get; set; }
      
        /// <summary>
        /// 创建人
        /// </summary>
        [DataMember]
        public virtual string Creator { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMember]
        public virtual DateTime? CreateTime { get; set; }

        /// <summary>
        /// 编辑人
        /// </summary>
        [DataMember]
        public virtual string Editor { get; set; }

        /// <summary>
        /// 编辑时间
        /// </summary>
        [DataMember]
        public virtual DateTime? EditTime { get; set; }
         /// <summary>
        /// 托锁定状态
        /// </summary>
        [DataMember]
        public virtual int LockFlag { get; set; }
   
    }
}
