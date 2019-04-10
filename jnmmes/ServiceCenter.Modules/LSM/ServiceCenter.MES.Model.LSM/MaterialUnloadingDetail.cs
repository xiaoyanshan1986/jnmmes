using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;

namespace ServiceCenter.MES.Model.LSM
{
    /// <summary>
    /// 下料明细数据主键。
    /// </summary>
    public struct MaterialUnloadingDetailKey
    {
        /// <summary>
        /// 下料主键。
        /// </summary>
        public string UnloadingKey { get; set; }
        /// <summary>
        /// 项目号。
        /// </summary>
        public int ItemNo { get; set; }

        public override string ToString()
        {
            return string.Format("{0}({1})", this.UnloadingKey, this.ItemNo);
        }
    }

    /// <summary>
    /// 描述下料明细数据的模型类。
    /// </summary>
    [DataContract]
    public class MaterialUnloadingDetail : BaseModel<MaterialUnloadingDetailKey>
    {
        public MaterialUnloadingDetail()
        {
            this.CreateTime = DateTime.Now;
            this.EditTime = DateTime.Now;
        }

        /// <summary>
        /// 上料主键。
        /// </summary>
        [DataMember]
        public virtual string LoadingKey
        {
            get;
            set;
        }
        /// <summary>
        /// 上料项目号。
        /// </summary>
        [DataMember]
        public virtual int LoadingItemNo
        {
            get;
            set;
        }

        /// <summary>
        /// 线边仓名称。
        /// </summary>
        [DataMember]
        public virtual string LineStoreName
        {
            get;
            set;
        }
        /// <summary>
        /// 工单号。
        /// </summary>
        [DataMember]
        public virtual string OrderNumber
        {
            get;
            set;
        }
        /// <summary>
        /// 物料代码。
        /// </summary>
        [DataMember]
        public virtual string MaterialCode
        {
            get;
            set;
        }
        /// <summary>
        /// 物料批号。
        /// </summary>
        [DataMember]
        public virtual string MaterialLot
        {
            get;
            set;
        }
        /// <summary>
        /// 下料数量。
        /// </summary>
        [DataMember]
        public virtual double UnloadingQty { get; set; }
        
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
