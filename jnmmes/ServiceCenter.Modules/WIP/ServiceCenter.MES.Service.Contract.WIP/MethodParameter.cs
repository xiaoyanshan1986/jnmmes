using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.Contract.WIP
{
    /// <summary> 原因代码参数 </summary>
    [DataContract]
    public class ReasonCodeParameter
    {
        /// <summary>
        /// 原因组名称。
        /// </summary>
        [DataMember]
        public string ReasonCodeCategoryName { get; set; }
        /// <summary>
        /// 原因代码名称。
        /// </summary>
        [DataMember]
        public string ReasonCodeName { get; set; }
        /// <summary>
        /// 数量。
        /// </summary>
        [DataMember]
        public double Quantity { get; set; }
        /// <summary>
        /// 原因描述。
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        
    }

    /// <summary> 附加参数数据 </summary>
    [DataContract]
    public class TransactionParameter
    {
        /// <summary>
        /// 参数名称。
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// 参数值。
        /// </summary>
        [DataMember]
        public string Value { get; set; }
        /// <summary>
        /// 参数索引号。
        /// </summary>
        [DataMember]
        public int Index { get; set; }
    }

    /// <summary> 附加的参数X,Y数据 </summary>
    [DataContract]
    public class DefectPOSParameter
    {
        /// <summary>
        /// 参数名称。
        /// </summary>
        [DataMember]
        public string POS_X { get; set; }
        /// <summary>
        /// 参数值。
        /// </summary>
        [DataMember]
        public string POS_Y { get; set; }
    }

    /// <summary> 批次操作方法参数类 </summary>
    [DataContract]
    public class MethodParameter
    {
        public MethodParameter()
        {
            this.TransactionKeys = new Dictionary<string, string>();
        }
        /// <summary>
        /// 用于设置批次操作的唯一标识值。在执行操作时设置。
        /// </summary>
        public IDictionary<string,string> TransactionKeys { get; set; }

        /// <summary>
        /// 批次号。
        /// </summary>
        [DataMember]
        public IList<string> LotNumbers { get; set; }

        /// <summary>
        /// 包装号列表。
        /// </summary>
        [DataMember]
        public IList<string> PackageNos { get; set; }

        /// <summary>
        /// 录入的附加参数集合。
        /// </summary>
        [DataMember]
        public IDictionary<string, IList<TransactionParameter>> Paramters { get; set; }
        
        /// <summary>
        /// 录入的附加参数集合。
        /// </summary>
        [DataMember]
        public IDictionary<string, IList<TransactionParameter>> Attributes { get; set; }

        /// <summary>
        /// 扣料集合
        /// </summary>
        [DataMember]
        public IDictionary<string, IList<MaterialConsumptionParameter>> MaterialParamters { get; set; }

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
    }

    /// <summary> 扣料参数数据类 </summary>
    [DataContract]
    public class MaterialConsumptionParameter
    {
        /// <summary>
        /// 物料代码
        /// </summary>
        [DataMember]
        public string MaterialCode { get; set; }

        /// <summary>
        /// 物料批次号
        /// </summary>
        [DataMember]
        public string MaterialLot { get; set; }

        /// <summary>
        /// 物料消耗量
        /// </summary>
        [DataMember]
        public decimal LoadingQty { get; set; }
    }
}
