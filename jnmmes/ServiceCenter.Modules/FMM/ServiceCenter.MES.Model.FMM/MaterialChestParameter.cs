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
    /// 产品成柜参数数据。
    /// </summary>
    [DataContract]
    public class MaterialChestParameter : BaseModel<string>
    {
        /// <summary>
        /// 主键（产品编码）。
        /// </summary>
        [DataMember]
        public override string Key
        {
            get;
            set;
        }

        /// <summary>
        /// 颜色是否不可混。
        /// </summary>
        [DataMember]
        public virtual bool ColorLimit { get; set; }
        /// <summary>
        /// 等级是否不可混。
        /// </summary>
        [DataMember]
        public virtual bool GradeLimit { get; set; }

        /// <summary>
        /// 功率是否不可混。
        /// </summary>
        [DataMember]
        public virtual bool PowerLimit { get; set; }
        /// <summary>
        /// 电流档是否不可混。
        /// </summary>
        [DataMember]
        public virtual bool IscLimit { get; set; }

        /// <summary>
        /// 包装能否入柜。
        /// </summary>
        [DataMember]
        public virtual bool IsPackagedChest { get; set; }
        /// <summary>
        /// 工单是否不可混。
        /// </summary>
        [DataMember]
        public virtual bool OrderNumberLimit { get; set; }

        /// <summary>
        /// 尾柜产品编码是否不可混。
        /// </summary>
        [DataMember]
        public virtual bool LastChestMaterialLimit { get; set; }

        /// <summary>
        /// 满柜数量。
        /// </summary>
        [DataMember]
        public virtual int FullChestQty { get; set; }

        /// <summary>
        /// 柜最大满包数量。
        /// </summary>
        [DataMember]
        public virtual int InChestFullPackageQty { get; set; } 

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
