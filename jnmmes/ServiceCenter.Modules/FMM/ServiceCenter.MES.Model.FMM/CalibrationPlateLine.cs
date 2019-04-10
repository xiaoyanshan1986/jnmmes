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
    /// 描述校准板线别模型类。
    /// </summary>
    /// 
    public struct CalibrationPlateLineKey
    {
        /// <summary>
        /// 校准板编号。
        /// </summary>
        [DataMember]
        public string CalibrationPlateID { get; set; }
        /// <summary>
        /// 车间。
        /// </summary>
        [DataMember]
        public  string LocationName { get; set; }
        /// <summary>
        /// 线别。
        /// </summary>
        [DataMember]
        public string LineCode { get; set; }

        

        public override string ToString()
        {
            return string.Format("{0}:{1}:{2}", this.CalibrationPlateID, this.LocationName, this.LineCode);
        }
    }
    [DataContract]
    public class CalibrationPlateLine : BaseModel<CalibrationPlateLineKey>
    {
        /// <summary>
        /// 描述。
        /// </summary>
        [DataMember]
        public virtual string Explain { get; set; }
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
