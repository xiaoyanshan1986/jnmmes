using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;
using System.ComponentModel.DataAnnotations;
using ServiceCenter.MES.Model.FMM.Resources;
using ServiceCenter.Common;
using ServiceCenter.MES.Model.ZPVM;


namespace ServiceCenter.MES.Model.FMM
{
    /// <summary>
    /// 表示规则-控制参数主键。
    /// </summary>
    public struct EquipmentControlObjectKey
    {
        /// <summary>
        /// 规则代码。
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 控制对象。
        /// </summary>
        public EnumPVMTestDataType Object { get; set; }
        /// <summary>
        /// 控制类型（运算符）。
        /// </summary>
        public string Type { get; set; }

        public override string ToString()
        {
            return string.Format("{0}:{1}:{2}", this.Code, this.Object.GetDisplayName(), this.Type);
        }
    }
    /// <summary>
    /// 描述规则-控制参数对象的数据模型
    /// </summary>
    [DataContract]
    public class EquipmentControlObject : BaseModel<EquipmentControlObjectKey>
    {
        /// <summary>
        /// 控制值。
        /// </summary>
        [DataMember]
        public virtual double Value { get; set; }
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
