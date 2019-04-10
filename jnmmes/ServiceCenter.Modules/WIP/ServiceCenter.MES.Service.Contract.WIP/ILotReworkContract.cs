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
    /// 批次返工参数类。
    /// </summary>
    [DataContract]
    public class ReworkParameter : MethodParameter
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
        /// <summary>
        /// c车间名称
        /// </summary>
        [DataMember]
        public string LocationName { get; set; }

        /// <summary>
        /// 是否是包的返工
        /// </summary>
        [DataMember]
        public bool IsPackageRework { get; set; }
    }

    /// <summary>
    /// 按托返工参数类。
    /// </summary>
    [DataContract]
    public class PackageReworkParameter : MethodParameter
    {
        /// <summary>
        /// 托号
        /// </summary>
        [DataMember]
        public string PackageNo { get; set; }

        /// <summary>
        /// 批次号
        /// </summary>
        [DataMember]
        public string LotNumber { get; set; }

        /// <summary>
        /// 车间
        /// </summary>
        [DataMember]
        public string LocationName { get; set; }

        /// <summary>
        /// 线别
        /// </summary>
        [DataMember]
        public string LineCode { get; set; }

        /// <summary>
        /// 是否保留托号
        /// </summary>
        [DataMember]
        public bool RetainPackageNo { get; set; }

        /// <summary>
        /// 是否按批次号投料
        /// </summary>
        [DataMember]
        public bool IsLot { get; set; }
    }

    /// <summary>
    /// 批次返工单操作契约接口。
    /// </summary>
    [ServiceContract]
    public interface ILotReworkContract
    {
        /// <summary>
        /// 记录批次返工单数据的操作。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>方法执行结果。</returns>
        [OperationContract]
        MethodReturnResult Rework(PackageReworkParameter p);

        //[OperationContract]
        //MethodReturnResult<DataSet> LotUpdate();
    }

    /// <summary>
    /// 用于扩展批次返工单操作检查的接口。
    /// </summary>
    public interface ILotReworkCheck
    {
        /// <summary>
        /// 进行批次返工单操作前检查。
        /// </summary>
        /// <param name="p">返工单操作参数。</param>
        /// <returns>
        /// 错误代码表示：0：成功，其他失败。
        /// </returns>
        MethodReturnResult Check(PackageReworkParameter p);
    }

    /// <summary>
    /// 用于扩展批次返工单操作执行的接口。
    /// </summary>
    public interface ILotRework
    {
        /// <summary>
        /// 进行批次返工单操作。
        /// </summary>
        /// <param name="p">返工单操作参数。</param>
        /// <returns>
        /// 错误代码表示：0：成功，其他失败。
        /// </returns>
        MethodReturnResult Execute(PackageReworkParameter p);
    }
}
