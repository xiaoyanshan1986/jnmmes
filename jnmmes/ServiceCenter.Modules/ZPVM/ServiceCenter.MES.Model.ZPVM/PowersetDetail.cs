using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;
using System.ComponentModel.DataAnnotations;
using ServiceCenter.MES.Model.ZPVM.Resources;

namespace ServiceCenter.MES.Model.ZPVM
{
    /// <summary>
    /// 表示子分档主键。
    /// </summary>
    public struct PowersetDetailKey
    {
        /// <summary>
        /// 分档代码。
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 项目号。
        /// </summary>
        public int ItemNo { get; set; }
        /// <summary>
        /// 子分档代码。
        /// </summary>
        public string SubCode { get; set; }

        public override string ToString()
        {
            return string.Format("{0}:{1}:{2}", this.Code, this.ItemNo, this.SubCode);
        }
    }
    /// <summary>
    /// 描述子分档数据模型
    /// </summary>
    [DataContract]
    public class PowersetDetail : BaseModel<PowersetDetailKey>
    {
        /// <summary>
        /// 子分档名称。
        /// </summary>
        [DataMember]
        public virtual string SubName { get; set; }
        /// <summary>
        /// 最小值。
        /// </summary>
        [DataMember]
        public virtual double? MinValue { get; set; }
        /// <summary>
        /// 最大值。
        /// </summary>
        [DataMember]
        public virtual double? MaxValue { get; set; }
        /// <summary>
        /// 图片。
        /// </summary>
        [DataMember]
        public virtual byte[] Picture { get; set; }
        /// <summary>
        /// 是否可用。
        /// </summary>
        [DataMember]
        public virtual bool IsUsed { get; set; }
        /// <summary>
        /// 创建人。
        /// </summary>
        [DataMember]
        public virtual  string Creator { get; set; }
        /// <summary>
        /// 创建时间。
        /// </summary>
        [DataMember]
        public virtual  DateTime CreateTime { get; set; }
        /// <summary>
        /// 编辑人。
        /// </summary>
        [DataMember]
        public virtual  string Editor { get; set; }
        /// <summary>
        /// 编辑时间。
        /// </summary>
        [DataMember]
        public virtual  DateTime EditTime { get; set; }

    }
}
