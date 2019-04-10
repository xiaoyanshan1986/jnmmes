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
    /// 批次出站方法的参数类。
    /// </summary>
    [DataContract]
    public class TrackOutParameter : TrackParameter
    {
        /// <summary>
        /// 等级。
        /// </summary>
        [DataMember]
        public string Grade { get; set; }

        /// <summary>
        /// 花色。
        /// </summary>
        [DataMember]
        public string Color { get; set; }

        /// <summary>
        /// 是否工序已结束
        /// </summary>
        [DataMember]
        public bool IsFinished { get; set; }

        /// <summary>
        /// 检验使用的条码集合。
        /// </summary>
        ///
        [DataMember]
        public IDictionary<string, IList<string>> CheckBarcodes { get; set; }

        /// <summary>
        /// 报废原因代码。
        /// </summary>
        [DataMember]
        public IDictionary<string, IList<ScrapReasonCodeParameter>> ScrapReasonCodes { get; set; }

        /// <summary>
        /// 不良原因代码。
        /// </summary>
        [DataMember]
        public IDictionary<string, IList<DefectReasonCodeParameter>> DefectReasonCodes { get; set; }
    }
    
    /// <summary>
    /// 批次暂停方法的参数类。
    /// </summary>
    [DataContract]
    public class LotIVDataParameter : MethodParameter
    {
        /// <summary>
        /// 原因组名称。
        /// </summary>
        [DataMember]
        public string LotNumber { get; set; }
    }

    /// <summary>
    /// 批次出站操作契约接口。
    /// </summary>
    [ServiceContract]
    public interface ILotTrackOutContract
    {
        /// <summary>
        /// 批次出站操作。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>方法执行结果。</returns>
        [OperationContract]
        MethodReturnResult TrackOut(TrackOutParameter p);
        
        /// <summary>
        /// 批次出站操作。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>方法执行结果。</returns>
        [OperationContract]
        MethodReturnResult ModifyIVDataForLot(LotIVDataParameter lotIVDataParameter);
    }

    /// <summary>
    /// 用于扩展批次出站检查的接口。
    /// </summary>
    public interface ILotTrackOutCheck
    {
        /// <summary>
        /// 进行批次出站前检查。
        /// </summary>
        /// <param name="p">出站参数。</param>
        /// <returns>
        /// 错误代码表示：0：成功，其他失败。
        /// </returns>
        MethodReturnResult Check(TrackOutParameter p);
    }

    /// <summary>
    /// 用于扩展批次出站执行的接口。
    /// </summary>
    public interface ILotTrackOut
    {
        /// <summary>
        /// 进行批次出站操作。
        /// </summary>
        /// <param name="p">出站参数。</param>
        /// <returns>
        /// 错误代码表示：0：成功，其他失败。
        /// </returns>
        MethodReturnResult Execute(TrackOutParameter p);
    }
}
