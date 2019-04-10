using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.Model
{
    /// <summary>
    /// 数据分页查询的配置类。
    /// </summary>
    [DataContract]
    public class PagingConfig
    {
        public PagingConfig()
        {
            this.PageNo = 0;
            this.PageSize = 20;
            this.IsPaging = true;
        }
        /// <summary>
        /// 分页标志。
        /// </summary>
        [DataMember]
        public bool IsPaging { get; set; }
        /// <summary>
        /// 获取或设置每页大小。
        /// </summary>
        [DataMember]
        public int PageSize { get; set; }
        /// <summary>
        /// 获取或设置当前页号。
        /// </summary>
        [DataMember]
        public int PageNo { get; set; }
        /// <summary>
        /// 获取总页数。输出。
        /// </summary>
        public int Pages
        {
            get
            {
                if (this.Records == 0 || this.PageSize==0)
                {
                    return 0;
                }
                return (this.Records / this.PageSize) + ((this.Records % this.PageSize>0)?1:0);
            }
        }
        /// <summary>
        /// 获取或设置总的记录数。输出。
        /// </summary>
        [DataMember]
        public int Records { get; set; }
        /// <summary>
        /// 获取或设置排序列名，多个列名使用逗号(,)分隔。
        /// </summary>
        [DataMember]
        public string OrderBy
        {
            get;
            set;
        }
        /// <summary>
        /// 获取或设置筛选条件，多个条件使用连接运算符（AND,OR,LIKE）组合。
        /// </summary>
        [DataMember]
        public string Where
        {
            get;
            set;
        }
    }
}
