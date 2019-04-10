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
    /// 批次过站方法的参数类。
    /// </summary>
    [DataContract]
    public class TrackParameter : MethodParameter
    {
        /// <summary>
        /// 线别代码
        /// </summary>
        [DataMember]
        public string LineCode { get; set; }

        /// <summary>
        /// 工艺流程
        /// </summary>
        [DataMember]
        public string RouteName { get; set; }

        /// <summary>
        /// 工序名称
        /// </summary>
        [DataMember]
        public string RouteOperationName { get; set; }

        /// <summary>
        /// 设备代码
        /// </summary>
        [DataMember]
        public string EquipmentCode { get; set; }

        /// <summary>
        /// 自动扣料标识
        /// </summary>
        [DataMember]
        public bool AutoDeductMaterial { get; set; }
        
        /// <summary>
        /// 事务结束时间
        /// </summary>
        [DataMember]
        public DateTime TransENDTime { get; set; }

        /// <summary>
        /// 事务类型
        /// </summary>
        [DataMember]
        public EnumLotActivity Activity { get; set; }
    }

     /// <summary>
    /// 批次进站方法的参数类。
    /// </summary>
    [DataContract]
    public class TrackInParameter : TrackParameter
    {
        
    }

    /// <summary>
    /// 批次进站操作契约接口。
    /// </summary>
    [ServiceContract]
    public interface ILotTrackInContract
    {
        /// <summary>
        /// 批次进站操作。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>方法执行结果。</returns>
        [OperationContract]
        MethodReturnResult TrackIn(TrackInParameter p);
    }

    /// <summary>
    /// 用于扩展批次进站检查的接口。
    /// </summary>
    public interface ILotTrackInCheck
    {
        /// <summary>
        /// 进行批次进站前检查。
        /// </summary>
        /// <param name="p">进站参数。</param>
        /// <returns>
        /// 错误代码表示：0：成功，其他失败。
        /// </returns>
        MethodReturnResult Check(TrackInParameter p);
    }

    /// <summary>
    /// 用于扩展批次进站执行的接口。
    /// </summary>
    public interface ILotTrackIn
    {
        /// <summary>
        /// 进行批次进站操作。
        /// </summary>
        /// <param name="p">进站参数。</param>
        /// <returns>
        /// 错误代码表示：0：成功，其他失败。
        /// </returns>
        MethodReturnResult Execute(TrackInParameter p);
    }
}
