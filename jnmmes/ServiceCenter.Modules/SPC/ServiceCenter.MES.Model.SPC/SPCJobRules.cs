using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using ServiceCenter.MES.Model.SPC.Resources;

namespace ServiceCenter.MES.Model.SPC
{
    public struct SPCJobRulesKey
    {
        /// <summary>
        /// ID
        /// </summary>
        [DataMember]
        public  string JobId { set; get; }
        /// <summary>
        /// 参数名称
        /// </summary>
        [DataMember]
        public  string ParamName { set; get; }
        /// <summary>
        /// 参数名称
        /// </summary>
        [DataMember]
        public  string RuleName { set; get; }
    
    }

    [DataContract]
    public class SPCJobRules : BaseModel<SPCJobRulesKey>
    {
        /// <summary>
        /// 是否有效
        /// </summary>
        [DataMember]
        public virtual int? IsvalID { set; get; }

        /// <summary>
        /// 创建人
        /// </summary>
        [DataMember]
        public virtual string Creator { set; get; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMember]
        public virtual DateTime? CreateTime { set; get; }
        /// <summary>
        /// 编辑人
        /// </summary>
        [DataMember]
        public virtual string Editor { set; get; }
        /// <summary>
        /// 编辑时间
        /// </summary>
        [DataMember]
        public virtual DateTime? EditTime { set; get; }
    }
}
