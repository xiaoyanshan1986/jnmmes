using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.Model;
using System.Data;

namespace ServiceCenter.MES.Service.Contract.WIP
{
    /// <summary>
    /// 表示不良原因代码参数。
    /// </summary>
    [DataContract]
    public class DefectReasonCodeParameter : ReasonCodeParameter
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

        [DataMember]
        public IList<DefectPOSParameter> ListDefectPOSParameter { get; set; }
    }
    /// <summary>
    /// 批次不良记录方法的参数类。
    /// </summary>
    [DataContract]
    public class DefectParameter : MethodParameter
    {
        [DataMember]
        public string Grade { get; set; }
        /// <summary>
        /// 不良原因代码。
        /// </summary>
        [DataMember]
        public IDictionary<string,IList<DefectReasonCodeParameter>> ReasonCodes { get; set; }
    }
    /// <summary>
    /// 批次不良操作契约接口。
    /// </summary>
    [ServiceContract]
    public interface ILotDefectContract
    {
        /// <summary>
        /// 记录批次不良数据的操作。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>方法执行结果。</returns>
        [OperationContract]
        MethodReturnResult Defect(DefectParameter p);

        [OperationContract]
        //获取不良位置的信息

        MethodReturnResult<DataSet> GetXY(string key);
    }

    /// <summary>
    /// 用于扩展批次不良操作检查的接口。
    /// </summary>
    public interface ILotDefectCheck
    {
        /// <summary>
        /// 进行批次不良操作前检查。
        /// </summary>
        /// <param name="p">不良操作参数。</param>
        /// <returns>
        /// 错误代码表示：0：成功，其他失败。
        /// </returns>
        MethodReturnResult Check(DefectParameter p);
    }

    /// <summary>
    /// 用于扩展批次不良操作执行的接口。
    /// </summary>
    public interface ILotDefect
    {
        /// <summary>
        /// 进行批次不良操作。
        /// </summary>
        /// <param name="p">不良操作参数。</param>
        /// <returns>
        /// 错误代码表示：0：成功，其他失败。
        /// </returns>
        MethodReturnResult Execute(DefectParameter p);
    }
}
