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
    /// 批次返修记录方法的参数类。
    /// </summary>
    [DataContract]
    public class RepairParameter : MethodParameter
    {
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
    /// 批次返修操作契约接口。
    /// </summary>
    [ServiceContract]
    public interface ILotRepairContract
    {
        /// <summary>
        /// 记录批次返修数据的操作。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>方法执行结果。</returns>
        [OperationContract]
        MethodReturnResult Repair(RepairParameter p);
    }

    /// <summary>
    /// 用于扩展批次返修操作检查的接口。
    /// </summary>
    public interface ILotRepairCheck
    {
        /// <summary>
        /// 进行批次返修操作前检查。
        /// </summary>
        /// <param name="p">返修操作参数。</param>
        /// <returns>
        /// 错误代码表示：0：成功，其他失败。
        /// </returns>
        MethodReturnResult Check(RepairParameter p);
    }

    /// <summary>
    /// 用于扩展批次返修操作执行的接口。
    /// </summary>
    public interface ILotRepair
    {
        /// <summary>
        /// 进行批次返修操作。
        /// </summary>
        /// <param name="p">返修操作参数。</param>
        /// <returns>
        /// 错误代码表示：0：成功，其他失败。
        /// </returns>
        MethodReturnResult Execute(RepairParameter p);
    }
}
