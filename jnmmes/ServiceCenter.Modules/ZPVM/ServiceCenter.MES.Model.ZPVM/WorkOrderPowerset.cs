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
    /// 表示工单分档主键。
    /// </summary>
    public struct WorkOrderPowersetKey
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
        /// 分档代码。
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 项目号。
        /// </summary>
        public int ItemNo { get; set; }

        public override string ToString()
        {
            return string.Format("{0}:{1}({2}-{3})", this.OrderNumber, this.MaterialCode, this.Code,this.ItemNo);
        }
    }
    /// <summary>
    /// 描述工单分档数据模型
    /// </summary>
    [DataContract]
    public class WorkOrderPowerset : BaseModel<WorkOrderPowersetKey>
    {
        /// <summary>
        /// 分档名称。
        /// </summary>
        [DataMember]
        public virtual string Name { get; set; }
        /// <summary>
        /// 最小值。
        /// </summary>
        [DataMember]
        public virtual double? MinValue { get; set; }
        /// <summary>
        /// 最大值。
        /// </summary>
        [DataMember]
        public virtual double? MaxValue { get; set; }
        /// <summary>
        /// 档位名称。
        /// </summary>
        [DataMember]
        public virtual string PowerName { get; set; }
        /// <summary>
        /// 标准功率。
        /// </summary>
        [DataMember]
        public virtual double? StandardPower { get; set; }
        /// <summary>
        /// 标准电流。
        /// </summary>
        [DataMember]
        public virtual double? StandardIsc { get; set; }
        /// <summary>
        /// 标准电压。
        /// </summary>
        [DataMember]
        public virtual double? StandardVoc { get; set; }
        /// <summary>
        /// 标准最大电流。
        /// </summary>
        [DataMember]
        public virtual double? StandardIPM { get; set; }
        /// <summary>
        /// 标准最大电压。
        /// </summary>
        [DataMember]
        public virtual double? StandardVPM { get; set; }
        /// <summary>
        /// 标准填充因子。
        /// </summary>
        [DataMember]
        public virtual double? StandardFuse { get; set; }
        /// <summary>
        /// 工单分档方式。
        /// </summary>
        [DataMember]
        public virtual string PowerDifference { get; set; }

        /// <summary>
        /// 是否允许混颜色
        /// </summary>
        [DataMember]
        public virtual bool MixColor { get; set; }

        /// <summary>
        /// 工单子分档方式。
        /// </summary>
        [DataMember]
        public virtual EnumPowersetSubWay SubWay { get; set; }
        /// <summary>
        /// ArticleNo。
        /// </summary>
        [DataMember]
        public virtual string ArticleNo { get; set; }
        /// <summary>
        /// 描述。
        /// </summary>
        [DataMember]
        public virtual string Description { get; set; }
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
