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
    /// 描述批次包装操作事务数据的模型类。
    /// </summary>
    [DataContract]
    public class LotTransactionPackage : BaseModel<string>
    {
        /// <summary>
        /// 包装号。
        /// </summary>
        [DataMember]
        public virtual string PackageNo { get; set; }
        /// <summary>
        /// 编辑人。
        /// </summary>
        [DataMember]
        public virtual string Editor { get; set; }
        /// <summary>
        /// 编辑时间。
        /// </summary>
        [DataMember]
        public virtual DateTime? EditTime { get; set; }
    }
}
