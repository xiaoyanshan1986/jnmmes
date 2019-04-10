using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;

namespace ServiceCenter.MES.Model.FMM
{
    /// <summary>
    /// 表示参数推导主键。
    /// </summary>
    public struct ParameterDerivationKey
    {
        /// <summary>
        /// 被推导参数名称。
        /// </summary>
        public string DerivedParameterName { get; set; }
        /// <summary>
        /// 推导参数名称。
        /// </summary>
        public string ParameterName { get; set; }

        public override string ToString()
        {
            return string.Format("{0}:{1}", this.DerivedParameterName, this.ParameterName);
        }
    }

    /// <summary>
    /// 描述参与采集参数推导的采集参数数据模型类。
    /// </summary>
    [DataContract]
    public class ParameterDerivation : BaseModel<ParameterDerivationKey>
    {
        /// <summary>
        /// 项目号。
        /// </summary>
        [DataMember]
        public virtual int ItemNo { get; set; }
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
