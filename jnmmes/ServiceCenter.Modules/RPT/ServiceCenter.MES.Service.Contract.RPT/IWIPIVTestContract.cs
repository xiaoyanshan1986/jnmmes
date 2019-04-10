using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.Contract.RPT
{
    /// <summary>
    /// 在制品MOVE数据获取参数类。
    /// </summary>
    [DataContract]
    public class WIPIVTestGetParameter
    {  /// <summary>
        ///批次。
        /// </summary>
        [DataMember]
        public string Lot_Number { get; set; }
        /// <summary>
        ///线别。
        /// </summary>
        [DataMember]
        public string LineCode { get; set; }
        /// <summary>
        ///校准板编号。
        /// </summary>
        [DataMember]
        public string CalibrationId { get; set; }   
        /// <summary>
        /// 设备。
        /// </summary>
        [DataMember]
        public string EquipmentCode { get; set; }
        /// <summary>
        ///分档规则。
        /// </summary>
        [DataMember]
        public string Attr_1 { get; set; }    
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
    public interface IWIPIVTestContract
    {
        [OperationContract]
        MethodReturnResult<DataSet> GetBaseDataForIVTest(string type);
        [OperationContract]
        MethodReturnResult<DataSet> Get(WIPIVTestGetParameter p);

        [OperationContract]
        MethodReturnResult<DataTable> GetIVDataForJZ(WIPIVTestGetParameter p);
        [OperationContract]
        MethodReturnResult<DataSet> GetIVDataForCTM(WIPIVTestGetParameter p);
    }
}
