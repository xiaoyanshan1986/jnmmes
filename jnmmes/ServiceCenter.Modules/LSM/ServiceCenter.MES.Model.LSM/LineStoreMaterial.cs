using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;

namespace ServiceCenter.MES.Model.LSM
{
    /// <summary>
    /// 线边仓物料数据主键。
    /// </summary>
    public struct LineStoreMaterialKey
    {
        /// <summary>
        /// 线边仓名称。
        /// </summary>
        public string LineStoreName { get; set; }
        /// <summary>
        /// 物料编码。
        /// </summary>
        public string MaterialCode { get; set; }

        public override string ToString()
        {
            return string.Format("{0}:{1}", this.LineStoreName, this.MaterialCode);
        }

    }
    /// <summary>
    /// 描述线边仓物料数据的模型类。
    /// </summary>
    [DataContract]
    public class LineStoreMaterial : BaseModel<LineStoreMaterialKey>
    {
        public LineStoreMaterial()
        {
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;
        }
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
