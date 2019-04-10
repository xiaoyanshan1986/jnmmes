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
    /// 客户端配置属性主键。
    /// </summary>
    public struct ClientConfigAttributeKey
    {
        /// <summary>
        /// 客户端名称。
        /// </summary>
        public string ClientName {get;set;}
        /// <summary>
        /// 属性名称。
        /// </summary>
        public string AttributeName { get; set; }

        /// <summary>
        /// 属性值。
        /// </summary>
        [DataMember]
        public  int ItemNo { get; set; }

        public override string ToString()
        {
            return string.Format("{0}：{1}：{2}", this.ClientName, this.AttributeName, this.ItemNo);
        }
    }
    /// <summary>
    /// 客户端配置属性数据模型类。
    /// </summary>
    [DataContract]
    public class ClientConfigAttribute : BaseModel<ClientConfigAttributeKey>
    {
        /// <summary>
        /// 属性值。
        /// </summary>
        [DataMember]
        public virtual string Value { get; set; }
        
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
