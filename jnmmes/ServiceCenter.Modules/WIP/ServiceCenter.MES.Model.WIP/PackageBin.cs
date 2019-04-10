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
    public enum EnumBinPackaged
    {
        /// <summary>
        /// 批次未完成
        /// </summary>
        UnFinished = 0,
        /// <summary>
        /// 批次完成状态
        /// </summary>
        Finished = 1
    }

    /// <summary>
    /// 包装明细主键
    /// </summary>
    public struct PackageBinKey
    {
        /// <summary>
        /// 包装号
        /// </summary>
        [DataMember]
        public string PackageNo { get; set; }

        /// <summary>
        /// Bin号
        /// </summary>
        [DataMember]
        public string BinNo { get; set; }

        /// <summary>
        /// Bin线别代码
        /// </summary>
        [DataMember]
        public string PackageLine { get; set; }

        public override string ToString()
        {
            return string.Format("{0}：{1}", this.PackageNo, this.BinNo);
        }
    }

    /// <summary>
    /// 描述包装明细数据的模型类。
    /// </summary>
    [DataContract]
    public class PackageBin : BaseModel<PackageBinKey>
    {     
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
        public virtual EnumBinPackaged BinPackaged { get; set; }
                
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
        /// 
        /// </summary>
        [DataMember]
        public virtual int BinIndex { get; set; }
    }
}
