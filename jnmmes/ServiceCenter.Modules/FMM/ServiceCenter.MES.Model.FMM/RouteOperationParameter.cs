using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;
using ServiceCenter.MES.Model.FMM.Resources;
using System.ComponentModel.DataAnnotations;

namespace ServiceCenter.MES.Model.FMM
{
    /// <summary>
    /// 数据来源枚举。
    /// </summary>
    public enum EnumDataFrom
    {
        /// <summary>
        /// 手工。
        /// </summary>
        [Display(Name = "EnumDataFrom_Manual", ResourceType = typeof(StringResource))]
        Manual=0,
        /// <summary>
        /// 设备。
        /// </summary>
        [Display(Name = "EnumDataFrom_Equipment", ResourceType = typeof(StringResource))]
        Equipment=1,
        /// <summary>
        /// 上料。
        /// </summary>
        [Display(Name = "EnumDataFrom_UpMaterial", ResourceType = typeof(StringResource))]
        UpMaterial=2
    }
    /// <summary>
    /// 触发采集的动作类型。
    /// </summary>
    public enum EnumDataCollectionAction
    {
        ///// <summary>
        ///// 创建时。
        ///// </summary>
        //[Display(Name = "EnumDataCollectionAction_Create", ResourceType = typeof(StringResource))]
        //Create=0,
        /// <summary>
        /// 进站时。
        /// </summary>
        [Display(Name = "EnumDataCollectionAction_TrackIn", ResourceType = typeof(StringResource))]
        TrackIn=1,
        /// <summary>
        /// 出站时。
        /// </summary>
        [Display(Name = "EnumDataCollectionAction_TrackOut", ResourceType = typeof(StringResource))]
        TrackOut=2
    }
    /// <summary>
    /// 验证规则。
    /// </summary>
    public enum EnumValidateRule
    {
        /// <summary>
        /// 0：不验证 
        /// </summary>
        [Display(Name = "EnumValidateRule_None", ResourceType = typeof(StringResource))]
        None=0,
        /// <summary>
        /// 匹配工单可用物料批号（根据领料记录）。
        /// </summary>
        [Display(Name = "EnumValidateRule_FullyWorkOrderMaterialLot", ResourceType = typeof(StringResource))]  
        FullyWorkOrderMaterialLot=1,
        /// <summary>
        /// 前缀匹配工单BOM物料。
        /// </summary>
        [Display(Name = "EnumValidateRule_PrefixWorkorderBOM", ResourceType = typeof(StringResource))]        
        PrefixWorkorderBOM=2,
        /// <summary>
        /// 后缀匹配工单BOM物料。
        /// </summary>
        [Display(Name = "EnumValidateRule_SuffixWorkorderBOM", ResourceType = typeof(StringResource))]        
        SuffixWorkorderBOM=3,
        /// <summary>
        /// 内容包含工单BOM物料。
        /// </summary>
        [Display(Name = "EnumValidateRule_IncludeWorkorderBOM", ResourceType = typeof(StringResource))]
        IncludeWorkorderBOM=4,
        /// <summary>
        /// 完全匹配工单BOM物料。
        /// </summary>
        [Display(Name = "EnumValidateRule_FullyWorkorderBOM", ResourceType = typeof(StringResource))]
        FullyWorkorderBOM=5,
        /// <summary>
        /// 匹配设备上料批号（根据上料记录）。
        /// </summary>
        [Display(Name = "EnumValidateRule_FullyLoadingMaterialLot", ResourceType = typeof(StringResource))]
        FullyLoadingMaterialLot = 6,
    }
    /// <summary>
    /// 验证失败规则。
    /// </summary>
    public enum EnumValidateFailedRule
    {
        /// <summary>
        /// 提示验证失败
        /// </summary>
        [Display(Name = "EnumValidateFailedRule_Alert", ResourceType = typeof(StringResource))]
        Alert=0
    }
    /// <summary>
    /// 工序参数数据主键。
    /// </summary>
    public struct RouteOperationParameterKey
    {
        /// <summary>
        /// 工序名称。
        /// </summary>
        public string RouteOperationName { get; set; }
        /// <summary>
        /// 参数名称。
        /// </summary>
        public string ParameterName { get; set; }

        public override string ToString()
        {
            return string.Format("{0}:{1}", this.RouteOperationName, this.ParameterName);
        }
    }

    /// <summary>
    /// 工序采集参数数据模型。
    /// </summary>
    [DataContract]
    public class RouteOperationParameter : BaseModel<RouteOperationParameterKey>
    {
        /// <summary>
        /// 构造函数。
        /// </summary>
        public RouteOperationParameter()
        {
            this.IsMustInput = true;
        }
        /// <summary>
        /// 索引序号。
        /// </summary>
        [DataMember]
        public virtual int ParamIndex { get; set; }
        /// <summary>
        /// 数据类型。
        /// </summary>
        [DataMember]
        public virtual EnumDataType DataType { get; set; }
        /// <summary>
        /// 是否必须输入。
        /// </summary>
        [DataMember]
        public virtual bool IsMustInput { get; set; }
        /// <summary>
        /// 数据来源 0：手工输入 1：设备接口 2：上料功能
        /// </summary>
        [DataMember]
        public virtual EnumDataFrom DataFrom { get; set; }
        /// <summary>
        /// 是否只读。
        /// </summary>
        [DataMember]
        public virtual bool IsReadOnly { get; set; }
        /// <summary>
        /// 物料类型
        /// </summary>
        [DataMember]
        public virtual string MaterialType { get; set; }
        /// <summary>
        /// 触发数据采集的动作。
        /// </summary>
        [DataMember]
        public virtual EnumDataCollectionAction DCType { get; set; }
        /// <summary>
        /// 验证规则。
        /// </summary>
        [DataMember]
        public virtual EnumValidateRule ValidateRule { get; set; }
        /// <summary>
        /// 验证失败规则。
        /// </summary>
        [DataMember]
        public virtual EnumValidateFailedRule ValidateFailedRule { get; set; }
        /// <summary>
        /// 验证失败消息内容。
        /// </summary>
        [DataMember]
        public virtual string ValidateFailedMessage { get; set; }

        /// <summary>
        /// 是否删除标记。
        /// </summary>
        [DataMember]
        public virtual bool IsDeleted { get; set; }
        /// <summary>
        /// 是否自动填充前一次的值。
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
