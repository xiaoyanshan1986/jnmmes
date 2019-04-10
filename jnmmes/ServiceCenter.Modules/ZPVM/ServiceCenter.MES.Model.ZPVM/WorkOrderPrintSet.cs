using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;
using System.ComponentModel.DataAnnotations;
using ServiceCenter.MES.Model.ZPVM.Resources;

namespace ServiceCenter.MES.Model.ZPVM
{
    /// <summary>
    /// 表示工单标签打印设置主键。
    /// </summary>
    public struct WorkOrderPrintSetKey
    {
        /// <summary>
        /// 工单号。
        /// </summary>
        public string OrderNumber { get; set; }
        /// <summary>
        /// 产品料号。
        /// </summary>
        public string MaterialCode { get; set; }
        /// <summary>
        /// 标签代码。
        /// </summary>
        public string LabelCode { get; set; }

        public override string ToString()
        {
            return string.Format("{0}:{1}({2})", this.OrderNumber, this.MaterialCode, this.LabelCode);
        }
    }
    /// <summary>
    /// 描述工单标签打印设置的数据模型
    /// </summary>
    [DataContract]
    public class WorkOrderPrintSet : BaseModel<WorkOrderPrintSetKey>
    {
        /// <summary>
        /// 项目号。
        /// </summary>
        [DataMember]
        public virtual int ItemNo { get; set; }
        /// <summary>
        /// 打印数量。
        /// </summary>
         [DataMember]
        public virtual int Qty { get; set; }
        /// <summary>
        /// 是否可用。
        /// </summary>
        [DataMember]
        public virtual bool IsUsed { get; set; }

        /// <summary>
        /// 是否所有功率。
        /// </summary>
        [DataMember]
        public virtual bool IsAllPower { get; set; }

        /// <summary>
        /// 分档代码。
        /// </summary>
        [DataMember]
        public virtual string PowerCode { get; set; }

        /// <summary>
        /// 分档项目号。
        /// </summary>
        [DataMember]
        public virtual int PowerItemNo { get; set; }

        /// <summary>
        /// 功率档。
        /// </summary>
        [DataMember]
        public virtual string PowerName { get; set; }

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
