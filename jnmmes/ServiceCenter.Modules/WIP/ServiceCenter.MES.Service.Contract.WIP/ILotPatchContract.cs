using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.Model;

namespace ServiceCenter.MES.Service.Contract.WIP
{
    /// <summary>
    /// 表示补料原因代码参数。
    /// </summary>
    [DataContract]
    public class PatchReasonCodeParameter : ReasonCodeParameter
    {
        /// <summary>
        /// 责任工序名称。
        /// </summary>
        [DataMember]
        public string RouteOperationName { get; set; }
        /// <summary>
        /// 责任人。
        /// </summary>
        [DataMember]
        public string ResponsiblePerson { get; set; }
    }
    /// <summary>
    /// 批次补料记录方法的参数类。
    /// </summary>
    [DataContract]
    public class PatchParameter : MethodParameter
    {
        /// <summary>
        /// 线边仓名称。
        /// </summary>
        [DataMember]
        public string LineStoreName { get; set; }
        /// <summary>
        /// 线别名称。
        /// </summary>
        [DataMember]
        public string LineCode { get; set; }
        /// <summary>
        /// 原材料编码。
        /// </summary>
        [DataMember]
        public string RawMaterialCode { get; set; }
        /// <summary>
        /// 原材料批号。
        /// </summary>
        [DataMember]
        public string RawMaterialLot { get; set; }

        /// <summary>
        /// 补料原因代码。
        /// </summary>
        [DataMember]
        public IDictionary<string,IList<PatchReasonCodeParameter>> ReasonCodes { get; set; }

    }
    /// <summary>
    /// 批次补料操作契约接口。
    /// </summary>
    [ServiceContract]
    public interface ILotPatchContract
    {
        /// <summary>
        /// 记录批次补料数据的操作。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>方法执行结果。</returns>
        [OperationContract]
        MethodReturnResult Patch(PatchParameter p);
    }

    /// <summary>
    /// 用于扩展批次补料操作检查的接口。
    /// </summary>
    public interface ILotPatchCheck
    {
        /// <summary>
        /// 进行批次补料操作前检查。
        /// </summary>
        /// <param name="p">补料操作参数。</param>
        /// <returns>
        /// 错误代码表示：0：成功，其他失败。
        /// </returns>
        MethodReturnResult Check(PatchParameter p);
    }

    /// <summary>
    /// 用于扩展批次补料操作执行的接口。
    /// </summary>
    public interface ILotPatch
    {
        /// <summary>
        /// 进行批次补料操作。
        /// </summary>
        /// <param name="p">补料操作参数。</param>
        /// <returns>
        /// 错误代码表示：0：成功，其他失败。
        /// </returns>
        MethodReturnResult Execute(PatchParameter p);
    }
}
