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
    /// 表示规则-打印设置主键。
    /// </summary>
    public struct RulePrintSetKey
    {
        /// <summary>
        /// 规则代码。
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 标签代码。
        /// </summary>
        public string LabelCode { get; set; }

        public override string ToString()
        {
            return string.Format("{0}:{1}", this.Code, this.LabelCode);
        }
    }
    /// <summary>
    /// 描述规则-打印设置的数据模型
    /// </summary>
    [DataContract]
    public class RulePrintSet : BaseModel<RulePrintSetKey>
    {
        /// <summary>
        /// 项目号。
        /// </summary>
        [DataMember]
        public virtual int ItemNo { get; set; }
        /// <summary>
        /// 打印数量。
        /// </summary>
         [DataMember]
        public virtual int Qty { get; set; }
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
