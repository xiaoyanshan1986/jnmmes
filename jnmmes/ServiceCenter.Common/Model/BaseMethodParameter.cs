using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.Common.Model
{

    [DataContract]
    public class BaseMethodParameter
    {
        public BaseMethodParameter()
        {
            
        }
       
        /// <summary>
        /// 操作人。
        /// </summary>
        [DataMember]
        public string Operator { get; set; }
        /// <summary>
        /// 操作客户端。
        /// </summary>
        [DataMember]
        public string OperateComputer { get; set; }
        /// <summary>
        /// 创建人。
        /// </summary>
        [DataMember]
        public string Creator { get; set; }
        /// <summary>
        /// 班别名称。
        /// </summary>
        [DataMember]
        public string ShiftName { get; set; }
        /// <summary>
        /// 备注。
        /// </summary>
        [DataMember]
        public string Remark { get; set; }


        /// <summary>
        /// 备注。
        /// </summary>
        [DataMember]
        public DateTime? LastUpdatedDateTime { get; set; }
    }
}
