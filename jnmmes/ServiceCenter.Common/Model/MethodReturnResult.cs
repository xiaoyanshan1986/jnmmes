using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.Model
{
    /// <summary>
    /// 表示方法执行后的返回结果。
    /// </summary>
    [DataContract]
    public class MethodReturnResult
    {
        /// <summary>
        /// 构造函数。
        /// </summary>
        public MethodReturnResult()
        {
            this.Code = 0;
            this.Message = string.Empty;
            this.HelpLink = string.Empty;
        }
        /// <summary>
        /// 0:表示执行成功的代码。如无特别含义，使用0代表执行成功,如果需要特殊代码标识，请使用小于0的自定义操作。
        /// >0:表示执行发生错误的代码。
        /// </summary>
        [DataMember]
        public int Code { get; set; }
        /// <summary>
        /// 方法返回的结果信息。
        /// </summary>
        [DataMember]
        public string Message { get; set; }
        /// <summary>
        /// 明细。
        /// </summary>
        [DataMember]
        public string Detail { get; set; }
        /// <summary>
        /// 提供对应帮助文件的对应超级链接信息。
        /// </summary>
        [DataMember]
        public string HelpLink { get; set; }

        /// <summary>
        /// 系统产生的Object的编号，一般用在AddObject的时候
        /// </summary>
        [DataMember]
        public string ObjectNo { get; set; }
    }
    /// <summary>
    /// 表示方法执行后的返回结果。
    /// </summary>
    [DataContract]
    public class MethodReturnResult<T> : MethodReturnResult
    {
        /// <summary>
        /// 方法返回的数据对象。
        /// </summary>
        [DataMember]
        public T Data { get; set; }
    }
}
