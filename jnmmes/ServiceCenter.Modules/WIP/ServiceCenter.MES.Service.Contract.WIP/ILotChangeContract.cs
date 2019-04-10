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
    /// 批次转工单的参数类。
    /// </summary>
    [DataContract]
    public class ChangeParameter : MethodParameter
    {
        /// <summary>
        /// 新工单。
        /// </summary>
        [DataMember]
        public string OrderNumber { get; set; }
        /// <summary>
        /// 新产品料号。
        /// </summary>
        [DataMember]
        public string MaterialCode { get; set; } 
        /// <summary>
        /// 工艺流程组名称。
        /// </summary>
        [DataMember]
        public string RouteEnterpriseName { get; set; }
        /// <summary>
        /// 工艺流程名称。
        /// </summary>
        [DataMember]
        public string RouteName { get; set; }
        /// <summary>
        /// 工艺工步名称。
        /// </summary>
        [DataMember]
        public string RouteStepName { get; set; }
    }

    /// <summary>
    /// 批次转工单操作契约接口。
    /// </summary>
    [ServiceContract]
    public interface ILotChangeContract
    {
        /// <summary>
        /// 记录批次转工单数据的操作。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>方法执行结果。</returns>
        [OperationContract]
        MethodReturnResult Change(ChangeParameter p);
    }

    /// <summary>
    /// 用于扩展批次转工单操作检查的接口。
    /// </summary>
    public interface ILotChangeCheck
    {
        /// <summary>
        /// 进行批次转工单操作前检查。
        /// </summary>
        /// <param name="p">转工单操作参数。</param>
        /// <returns>
        /// 错误代码表示：0：成功，其他失败。
        /// </returns>
        MethodReturnResult Check(ChangeParameter p);
    }

    /// <summary>
    /// 用于扩展批次转工单操作执行的接口。
    /// </summary>
    public interface ILotChange
    {
        /// <summary>
        /// 进行批次转工单操作。
        /// </summary>
        /// <param name="p">转工单操作参数。</param>
        /// <returns>
        /// 错误代码表示：0：成功，其他失败。
        /// </returns>
        MethodReturnResult Execute(ChangeParameter p);
    }
}
