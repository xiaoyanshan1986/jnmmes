using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;

namespace ServiceCenter.MES.Model.QAM
{
    /// <summary>
    /// 检验数据明细主键。
    /// </summary>
    public struct CheckDataDetailKey
    {
        /// <summary>
        /// 检验数据主键。
        /// </summary>
        public string CheckDataKey { get; set; }
        /// <summary>
        /// 参数名称。
        /// </summary>
        public string ParameterName { get; set; }
        /// <summary>
        /// 顺序号。
        /// </summary>
        public int SequenceNo { get; set; }
    }

    /// <summary>
    /// 描述检验数据明细的模型类。
    /// </summary>
    [DataContract]
    public class CheckDataDetail : BaseModel<CheckDataDetailKey>
    {
        /// <summary>
        /// 项目号。
        /// </summary>
        [DataMember]
        public virtual int ItemNo { get; set; }
        /// <summary>
        /// 参数值。
        /// </summary>
        [DataMember]
        public virtual string ParameterValue { get; set; }
        /// <summary>
        /// 删除标识。
        /// </summary>
        [DataMember]
        public virtual bool DeletedFlag { get; set; }
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
