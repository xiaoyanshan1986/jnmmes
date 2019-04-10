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
    /// 表示工步参数数据主键。
    /// </summary>
    public struct RouteStepParameterKey
    { 
        /// <summary>
        /// 工艺流程名称。
        /// </summary>
        public string RouteName { get; set; }
        /// <summary>
        /// 工步名称。
        /// </summary>
        public string RouteStepName{ get; set; }
        /// <summary>
        /// 参数名称。
        /// </summary>
        public string ParameterName { get; set; }

        public override string ToString()
        {
            return string.Format("{0}:{1}:{2}", this.RouteName, this.RouteStepName, this.ParameterName);
        }
    }
    /// <summary>
    /// 表示工艺流程中工步采集参数数据。
    /// </summary>
    [DataContract]
    public class RouteStepParameter : BaseModel<RouteStepParameterKey>
    {
        /// <summary>
        /// 构造函数。
        /// </summary>
        public RouteStepParameter()
        {
            this.IsMustInput = true;
        }
        /// <summary>
        /// 索引序号。
        /// </summary>
        [DataMember]
        public virtual int ParamIndex { get; set; }
        /// <summary>
        /// 数据类型 1：整型 2：日期 3：日期时间 4：布尔 5：字符串 6：浮点型 7：设置
        /// </summary>
        [DataMember]
        public virtual EnumDataType DataType { get; set; }
        /// <summary>
        /// 是否必须输入 0：否 1：是,默认值为 1
        /// </summary>
        [DataMember]
        public virtual bool IsMustInput { get; set; }
        /// <summary>
        /// 数据来源 0：手工输入 1：设备接口 2：上料功能
        /// </summary>
        [DataMember]
        public virtual EnumDataFrom DataFrom { get; set; }
        /// <summary>
        /// 是否只读  0：否 1：是
        /// </summary>
        [DataMember]
        public virtual bool IsReadOnly { get; set; }
        /// <summary>
        /// 物料类型
        /// </summary>
        [DataMember]
        public virtual string MaterialType { get; set; }
        /// <summary>
        /// 采集时刻.
        /// </summary>
        [DataMember]
        public virtual EnumDataCollectionAction DCType { get; set; }
        /// <summary>
        /// 验证规则.
        /// </summary>
        [DataMember]
        public virtual EnumValidateRule ValidateRule { get; set; }
        /// <summary>
        /// 验证失败规则.
        /// </summary>
        [DataMember]
        public virtual EnumValidateFailedRule ValidateFailedRule { get; set; }
        /// <summary>
        /// 验证失败消息内容。
        /// </summary>
        [DataMember]
        public virtual string ValidateFailedMessage { get; set; }
       
        /// <summary>
        /// 是否删除标记 0：已删除 1：未删除
        /// </summary>
        [DataMember]
        public virtual bool IsDeleted { get; set; }
        /// <summary>
        /// 是否自动填充前一次的值 0 否 1是
        /// </summary>
        [DataMember]
        public virtual bool IsUsePreValue { get; set; }
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
