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
    /// 检验设置点主键。
    /// </summary>
    public struct CheckSettingPointKey
    {
        /// <summary>
        /// 检验设置主键。
        /// </summary>
        public string CheckSettingKey { get; set; }
        /// <summary>
        /// 项目号。
        /// </summary>
        public int ItemNo { get; set; }

        public override string ToString()
        {
            return string.Format("{0}", this.ItemNo);
        }
    }
    /// <summary>
    /// 描述检验设置点的模型类。
    /// </summary>
    [DataContract]
    public class CheckSettingPoint : BaseModel<CheckSettingPointKey>
    {
        /// <summary>
        /// 检验参数组名称。
        /// </summary>
        [DataMember]
        public virtual string CategoryName { get; set; }
        /// <summary>
        /// 检验计划名称。
        /// </summary>
        [DataMember]
        public virtual string CheckPlanName { get; set; }
        /// <summary>
        /// 状态 1：可用 0：不可用。
        /// </summary>
        [DataMember]
        public virtual EnumObjectStatus Status { get; set; }
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
