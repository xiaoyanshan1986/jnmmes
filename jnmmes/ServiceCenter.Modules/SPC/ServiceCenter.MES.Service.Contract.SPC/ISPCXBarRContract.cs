using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.Contract.SPC
{

    [DataContract]
    public class SPCXBarRDataGetParameter
    {
        [DataMember]
        public string RptType { get; set; }

        [DataMember]
        public string JobId { get; set; }

        [DataMember]
        public string RouteStepName { get; set; }
        [DataMember]
        public string EquipmentCode { get; set; }

        [DataMember]
        public string SLotCode { get; set; }

        public string DAttr1 { get; set; }
        /// <summary>
        /// 生产线。
        /// </summary>
        [DataMember]
        public string ProductionLineCode { get; set; }
        [DataMember]
        public string PointGroupName { get; set; }
        [DataMember]
        public string ParamterName { get; set; }
        /// <summary>
        /// 生产线。
        /// </summary>
        [DataMember]
        public string ProductionSeqNo { get; set; }
        /// <summary>
        /// 开始日期从。
        /// </summary>
        [DataMember]
        public string StartTime { get; set; }
        /// <summary>
        /// 开始日期到。
        /// </summary>
        [DataMember]
        public string EndTime { get; set; }
    }


    [DataContract]
    public class SPCChartMonitorQuery
    {
        [DataMember]
        public string RouteStepName { get; set; }

        [DataMember]
        public string EquipmentCode { get; set; }

        [DataMember]
        public string ParamterName { get; set; }
    }

    [ServiceContract]
    public interface ISPCXBarRContract
    {
        [OperationContract]
        MethodReturnResult<DataSet> Get(SPCXBarRDataGetParameter p);

        [OperationContract]
        MethodReturnResult<DataSet> GetEquipment(string stepname);


        [OperationContract]
        MethodReturnResult<DataSet> GetJobDataCode(string codeType, string jobId, string stepName);

        [OperationContract]
        MethodReturnResult<DataSet> GetXBarData(SPCXBarRDataGetParameter p);

        [OperationContract]
        MethodReturnResult<DataSet> GetChartMonitorList(SPCChartMonitorQuery p);

        [OperationContract]
        MethodReturnResult<DataSet> GetOriginalDataForExport(string testtime, string linecode, string eqpcode, string ParamterName);

        [OperationContract]
        int UpdateDealNote(string testtime, string linecode, string eqpcode, string ParamterName, string Note);
    }
}
