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
    /// 表示产品标签数据主键
    /// </summary>
    public struct MaterialPrintSetKey
    {
        /// <summary>
        /// 产品代码
        /// </summary>
        public string MaterialCode { get; set; }

        /// <summary>
        /// 标签代码
        /// </summary>
        public string LabelCode { get; set; }

        public override string ToString()
        {
            return string.Format("{0}-{1}"
                                , this.MaterialCode
                                , this.LabelCode);
        }
    }

    /// <summary>
    /// 表示产品标签设置类
    /// </summary>
    [DataContract]
    public class MaterialPrintSet : BaseModel<MaterialPrintSetKey>
    {
        /// <summary>
        /// 打印数量。
        /// </summary>
        [DataMember]
        public virtual int Qty { get; set; }

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
