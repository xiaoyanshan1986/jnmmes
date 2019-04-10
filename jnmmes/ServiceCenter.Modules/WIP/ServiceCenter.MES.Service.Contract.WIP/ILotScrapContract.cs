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
    /// 表示报废原因代码参数。
    /// </summary>
    [DataContract]
    public class ScrapReasonCodeParameter : ReasonCodeParameter
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
    /// 批次报废记录方法的参数类。
    /// </summary>
    [DataContract]
    public class ScrapParameter : MethodParameter
    {
        /// <summary>
        /// 报废原因代码。
        /// </summary>
        [DataMember]
        public IDictionary<string, IList<ScrapReasonCodeParameter>> ReasonCodes { get; set; }
    }
    /// <summary>
    /// 批次报废操作契约接口。
    /// </summary>
    [ServiceContract]
    public interface ILotScrapContract
    {
        /// <summary>
        /// 记录批次报废数据的操作。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>方法执行结果。</returns>
        [OperationContract]
        MethodReturnResult Scrap(ScrapParameter p);
    }

    /// <summary>
    /// 用于扩展批次报废操作检查的接口。
    /// </summary>
    public interface ILotScrapCheck
    {
        /// <summary>
        /// 进行批次报废操作前检查。
        /// </summary>
        /// <param name="p">报废操作参数。</param>
        /// <returns>
        /// 错误代码表示：0：成功，其他失败。
        /// </returns>
        MethodReturnResult Check(ScrapParameter p);
    }

    /// <summary>
    /// 用于扩展批次报废操作执行的接口。
    /// </summary>
    public interface ILotScrap
    {
        /// <summary>
        /// 进行批次报废操作。
        /// </summary>
        /// <param name="p">报废操作参数。</param>
        /// <returns>
        /// 错误代码表示：0：成功，其他失败。
        /// </returns>
        MethodReturnResult Execute(ScrapParameter p);
    }
}
