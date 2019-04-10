using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;

namespace ServiceCenter.MES.Model.WIP
{

    /// <summary>
    /// 描述批次检验操作事务数据的模型类。
    /// </summary>
    [DataContract]
    public class LotTransactionCheck : BaseModel<string>
    {
        /// <summary>
        /// 校对条码1。
        /// </summary>
        [DataMember]
        public virtual string Barcode1 { get; set; }
        /// <summary>
        /// 校对条码2。
        /// </summary>
        [DataMember]
        public virtual string Barcode2 { get; set; }
        /// <summary>
        /// 校对条码3。
        /// </summary>
        [DataMember]
        public virtual string Barcode3 { get; set; }
        /// <summary>
        /// 校对条码4。
        /// </summary>
        [DataMember]
        public virtual string Barcode4 { get; set; }
        /// <summary>
        /// 校对条码5。
        /// </summary>
        [DataMember]
        public virtual string Barcode5 { get; set; }

        /// <summary>
        /// 等级。
        /// </summary>
        [DataMember]
        public virtual string Grade { get; set; }

        /// <summary>
        /// 花色。
        /// </summary>
        [DataMember]
        public virtual string Color { get; set; }

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
