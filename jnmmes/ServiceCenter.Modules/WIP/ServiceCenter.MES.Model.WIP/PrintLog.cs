using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;
using System.ComponentModel.DataAnnotations;
using ServiceCenter.MES.Model.WIP.Resources;

namespace ServiceCenter.MES.Model.WIP
{    
    /// <summary>
    /// 描述打印操作日志数据的模型类。
    /// </summary>
    [DataContract]
    public class PrintLog : BaseModel<string>
    {
        /// <summary>
        /// 主键（事物代码）
        /// </summary>
        [DataMember]
        public override string Key
        {
            get;
            set;
        }

        public PrintLog()
        {
            Key = Guid.NewGuid().ToString();
        }
                
        /// <summary>
        /// 批次号
        /// </summary>
        [DataMember]
        public virtual string LotNumber { get; set; }

        /// <summary>
        /// 客户端
        /// </summary>
        [DataMember]
        public virtual string ClientName { get; set; }

        /// <summary>
        /// 打印数量
        /// </summary>
        [DataMember]
        public virtual int PrintQty { get; set; }

        /// <summary>
        /// 打印标签代码
        /// </summary>
        [DataMember]
        public virtual string PrintLabelCode { get; set; }

        /// <summary>
        /// 打印机名称
        /// </summary>
        [DataMember]
        public virtual string PrinterName { get; set; }

        /// <summary>
        /// 打印机类型
        /// </summary>
        [DataMember]
        public virtual string PrintType { get; set; }

        /// <summary>
        /// 打印是否成功
        /// </summary>
        [DataMember]
        public virtual bool IsSucceed { get; set; }

        /// <summary>
        /// 打印内容
        /// </summary>
        [DataMember]
        public virtual string PrintData { get; set; }
        
        /// <summary>
        /// 创建人
        /// </summary>
        [DataMember]
        public virtual string Creator { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMember]
        public virtual DateTime? CreateTime { get; set; }
        
        /// <summary>
        /// 完成时间
        /// </summary>
        [DataMember]
        public virtual DateTime? FinishTime { get; set; }

    }
}
