using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;

namespace ServiceCenter.MES.Model.EDC
{
    /// <summary>
    /// 采集数据明细主键。
    /// </summary>
    public struct DataDetailKey
    {
        /// <summary>
        /// 数据主键。
        /// </summary>
        public string DataKey { get; set; }
        /// <summary>
        /// 参数名称。
        /// </summary>
        public string ParameterName { get; set; }
        /// <summary>
        /// 顺序号。
        /// </summary>
        public int SequenceNo { get; set; }

        public override string ToString()
        {
            return string.Format("{0}:{1}({2})", this.DataKey, this.ParameterName,this.SequenceNo);
        }
    }

    /// <summary>
    /// 描述采集数据明细的模型类。
    /// </summary>
    [DataContract]
    public class DataDetail : BaseModel<DataDetailKey>
    {
        /// <summary>
        /// 主键。
        /// </summary>
        [DataMember]
        public override DataDetailKey Key
        {
            get;
            set;
        }
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
