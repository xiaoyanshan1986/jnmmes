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
    /// 在制品分布数据获取参数类。
    /// </summary>
    [DataContract]
    public class WIPDisplayGetParameter
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
        /// 在线时长（分钟数）。
        /// </summary>
        [DataMember]
        public double OnlineTime { get; set; } 
    }

    /// <summary>
    /// 在制品分布报表契约接口。
    /// </summary>
    [ServiceContract]
    public interface IWIPDisplayContract
    {
        /// <summary>
        /// 在制品分布数据获取操作。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>方法执行结果。</returns>
        [OperationContract]
        MethodReturnResult<DataSet> Get(WIPDisplayGetParameter p);
    }
}
