using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;
using System.ComponentModel.DataAnnotations;
using ServiceCenter.MES.Model.ZPVM.Resources;
using ServiceCenter.Common;

namespace ServiceCenter.MES.Model.ZPVM
{
    /// <summary>
    /// 柜动作枚举。
    /// </summary>
    public enum EnumChestActivity
    {
        /// <summary>
        /// 入柜。
        /// </summary>
        [Display(Name = "入柜")]
        InChest = 0,

        /// <summary>
        /// 出柜。
        /// </summary>
        [Display(Name = "出柜")]
        OutChest = 1,

        /// <summary>
        /// 投料。
        /// </summary>
        [Display(Name = "投料")]
        InFabStore = 2
    }

    /// <summary>
    /// 表示柜相关动作日志设置主键。
    /// </summary>
    public struct ChestLogKey
    {
        /// <summary>
        /// 柜号。
        /// </summary>
        public string ChestNo { get; set; }

        /// <summary>
        /// 托号。
        /// </summary>
        public string PackageNo { get; set; }

        /// <summary>
        /// 柜动作。
        /// </summary>
        public EnumChestActivity ChestActivity { get; set; }

        /// <summary>
        /// 创建时间。
        /// </summary>
        public DateTime CreateTime { get; set; }
        
        public override string ToString()
        {
            return string.Format("{0}:{1}:{2}", this.ChestNo, this.PackageNo,this.ChestActivity.GetDisplayName(),CreateTime.ToString());
        }
    }
    /// <summary>
    /// 描述柜相关动作日志的数据模型
    /// </summary>
    [DataContract]
    public class ChestLog : BaseModel<ChestLogKey>
    {
            
        /// <summary>
        /// 创建人。
        /// </summary>
        [DataMember]
        public virtual string Creator { get; set; }

        /// <summary>
        /// 入柜模式。
        /// </summary>
        [DataMember]
        public virtual int ModelType { get; set; }

    }
}
