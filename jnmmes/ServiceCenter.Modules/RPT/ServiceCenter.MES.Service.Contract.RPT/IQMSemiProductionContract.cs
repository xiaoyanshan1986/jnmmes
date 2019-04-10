using ServiceCenter.Model;
using System.Data;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace ServiceCenter.MES.Service.Contract.RPT
{
    /// <summary>
    /// 在制品MOVE数据获取参数类。
    /// </summary>
    [DataContract]
    public class QMSemiProductionGetParameter
    {  /// <summary>
        ///批次。
        /// </summary>
        [DataMember]
        public string LocationName { get; set; }
        [DataMember]
        public string Grade { get; set; } 
        /// <summary>
        /// 设备。
        /// </summary>
        [DataMember]
        public string IsProdReport { get; set; } 
        /// <summary>
        /// 开始时间。
        /// </summary>
        [DataMember]
        public string StartDate { get; set; }
        /// <summary>
        /// 结束时间。
        /// </summary>
        [DataMember]
        public string EndDate { get; set; }

       
    }

    [DataContract]
    public class DefectPOSGetParameter
    {
        [DataMember]
        public string IsProdReport { get; set; } 
        [DataMember]
        public string StepName { get; set; }   
        [DataMember]
        public string LineCode { get; set; }   
        /// <summary>
        ///批次。
        /// </summary>
        [DataMember]
        public string LotNumber { get; set; }
        [DataMember]
        public string PosX { get; set; }
        /// <summary>
        /// y。
        /// </summary>
        [DataMember]
        public string PosY { get; set; }
        /// <summary>
        /// 开始时间。
        /// </summary>
        [DataMember]
        public string StartDate { get; set; }
        /// <summary>
        /// 结束时间。
        /// </summary>
        [DataMember]
        public string EndDate { get; set; }
    }
    [ServiceContract]
    public interface IQMSemiProductionContract
    {
        [OperationContract]
        MethodReturnResult<DataSet> GetBaseDataForIVTest(string type);
        [OperationContract]
        MethodReturnResult<DataSet> GetSemiProdQtyForLine(QMSemiProductionGetParameter p);

        [OperationContract]
        MethodReturnResult<DataSet> GetSemiProdQtyForLocation(QMSemiProductionGetParameter p);

        [OperationContract]
        MethodReturnResult<DataSet> GetQtyForDefective(QMSemiProductionGetParameter p);

        [OperationContract]
        MethodReturnResult<DataSet> GetQtyForDefectPOS(DefectPOSGetParameter p);

        [OperationContract]
        MethodReturnResult<DataSet> GetQtyForDefectReason(DefectPOSGetParameter p);

        [OperationContract]
        MethodReturnResult<DataSet> GetEquipmentDailyMoveForOEE(string EquipmentNo, string curDate);
    }
}
