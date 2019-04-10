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
    /// 表示规则-等级设置主键。
    /// </summary>
    public struct RuleGradeKey
    {
        /// <summary>
        /// 规则代码。
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 等级。
        /// </summary>
        public string Grade { get; set; }

        public override string ToString()
        {
            return string.Format("{0}:{1}", this.Code, this.Grade);
        }
    }
    /// <summary>
    /// 描述规则-等级设置的数据模型
    /// </summary>
    [DataContract]
    public class RuleGrade : BaseModel<RuleGradeKey>
    {
        /// <summary>
        /// 项目号。
        /// </summary>
        [DataMember]
        public virtual int ItemNo { get; set; }
        /// <summary>
        /// 是否混档位包装。
        /// </summary>
         [DataMember]
        public virtual bool MixPowerset { get; set; }
         /// <summary>
         /// 是否混子档位包装。
         /// </summary>
         [DataMember]
         public virtual bool MixSubPowerset { get; set; }
         /// <summary>
         /// 是否混花色包装。
         /// </summary>
         [DataMember]
         public virtual bool MixColor { get; set; }
         /// <summary>
         /// 包装组名称。
         /// </summary>
         [DataMember]
         public virtual string PackageGroup { get; set; }
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
