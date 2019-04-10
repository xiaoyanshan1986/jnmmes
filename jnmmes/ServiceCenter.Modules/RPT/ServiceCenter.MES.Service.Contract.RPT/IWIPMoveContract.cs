using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;
using System.Data;

namespace ServiceCenter.MES.Service.Contract.RPT
{
    /// <summary>
    /// 在制品MOVE数据获取参数类。
    /// </summary>
    [DataContract]
    public class WIPMoveGetParameter
    {
        /// <summary>
        /// 车间。
        /// </summary>
        [DataMember]
        public string LocationName { get; set; }
        /// <summary>
        /// 工单。
        /// </summary>
        [DataMember]
        public string OrderNumber { get; set; }
        /// <summary>
        /// 产品料号。
        /// </summary>
        [DataMember]
        public string MaterialCode { get; set; }
        /// <summary>
        /// 开始时间。
        /// </summary>
        [DataMember]
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// 结束时间。
        /// </summary>
        [DataMember]
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// 班别名称。
        /// </summary>
        [DataMember]
        public string ShiftName { get; set; } 
                /// <summary>
        /// 工序名称。
        /// </summary>
        [DataMember]
        public string StepName { get; set; }

        [DataMember]
        public string StartTime { get; set; }
        /// <summary>
        /// 结束时间。
        /// </summary>
        [DataMember]
        public string EndTime { get; set; }



        
    }
     [DataContract]
    public class WIPMoveDetailGetParameter:WIPMoveGetParameter
    {
        /// <summary>
        /// 工序名称。
        /// </summary>
        [DataMember]
        public string RouteOperationName { get; set; }
        /// <summary>
        /// 车间。
        /// </summary>
        //[DataMember]
        //public string LocationName { get; set; }
        /// <summary>
        /// 动作。
        /// </summary>
        [DataMember]
        public int Activity { get; set; }
        /// <summary>
        /// 页号。
        /// </summary>
        [DataMember]
        public int PageNo { get; set; }
        /// <summary>
        /// 每页大小。
        /// </summary>
        [DataMember]
        public int PageSize { get; set; }
        /// <summary>
        /// 总记录数。
        /// </summary>
        [DataMember]
        public int TotalRecords { get; set; }
    }
    /// <summary>
    /// 在制品MOVE报表契约接口。
    /// </summary>
    [ServiceContract]
    public interface IWIPMoveContract
    {
        /// <summary>
        /// 在制品MOVE数据获取操作。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>方法执行结果。</returns>
        [OperationContract]
        MethodReturnResult<DataSet> Get(WIPMoveGetParameter p);
        /// <summary>
        /// 在制品MOVE数据获取操作。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>方法执行结果。</returns>
        [OperationContract]
        MethodReturnResult<DataSet> GetWipMoveForStep(WIPMoveGetParameter p);
        /// <summary>
        /// 在制品MOVE明细数据获取操作。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>方法执行结果。</returns>
        [OperationContract]
        MethodReturnResult<DataSet> GetDetail(ref WIPMoveDetailGetParameter p);
        /// <summary>
        /// 批次号获取简单信息
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>方法执行结果。</returns>
        [OperationContract]
        MethodReturnResult<DataSet> GetLotInformation(string lot);
        [OperationContract]
        MethodReturnResult<DataSet> GetDailyQuantityOfWIP(QMSemiProductionGetParameter p);
    }
}
