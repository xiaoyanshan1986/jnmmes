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
    /// 查询批次物料清单数据的参数类。
    /// </summary>
    [DataContract]
    public class LotMaterialListQueryParameter
    {
        public LotMaterialListQueryParameter()
        {
            this.PageNo = 0;
            this.PageSize = 20;
        }
        [DataMember]
        public string LocationName { get; set; }
        [DataMember]
        public string OrderNumber { get; set; }
        [DataMember]
        public DateTime? StartCreateTime { get; set; }
        [DataMember]
        public DateTime? EndCreateTime { get; set; }
        /// <summary>
        /// 批次号。
        /// </summary>
        [DataMember]
        public string LotNumber { get; set; }
        /// <summary>
        /// 批次号1。
        /// </summary>
        [DataMember]
        public string LotNumber1 { get; set; }

        [DataMember]
        public int PageSize { get; set; }

        [DataMember]
        public int PageNo { get; set; }

        [DataMember]
        public int TotalRecords { get; set; }
    }


    /// <summary>
    /// 查询物料编码等的MES信息及出货信息等
    /// </summary>
    public class MaterialDataParameter
    {
        public MaterialDataParameter()
        {
            this.PageNo = 0;
            this.PageSize = 20;
        }

        ///// <summary>
        ///// 生产工单
        ///// </summary>
        //[DataMember]
        //public string OrderNumber { get; set; }

        /// <summary>
        /// 产品编码
        /// </summary>
        [DataMember]
        public string ProductMaterialCode { get; set; }

        /// <summary>
        /// 物料编码
        /// </summary>
        [DataMember]
        public string BomMaterialCode { get; set; }

        /// <summary>
        /// 物料名称
        /// </summary>
        [DataMember]
        public string BomMaterialName { get; set; }

        /// <summary>
        /// 出库包装号
        /// </summary>
        [DataMember]
        public string OutPackageNo { get; set; }

        /// <summary>
        /// 出库开始日期
        /// </summary>
        [DataMember]
        public DateTime? OutStartTime { get; set; }

        /// <summary>
        /// 出库结束日期
        /// </summary>
        [DataMember]
        public DateTime? OutEndTime { get; set; }

        /// <summary>
        /// 每页记录数
        /// </summary>
        [DataMember]
        public int PageSize { get; set; }

        /// <summary>
        /// 页号
        /// </summary>
        [DataMember]
        public int PageNo { get; set; }

        /// <summary>
        /// 总记录数
        /// </summary>
        [DataMember]
        public int Records { get; set; }


        /// <summary>
        /// 总数据
        /// </summary>
        [DataMember]
        public DataTable AllData { get; set; }

    }


    /// <summary>
    /// 查询批次物料清单契约接口。
    /// </summary>
    [ServiceContract]
    public interface ILotMaterialListContract
    {
        /// <summary>
        /// 查询批次物料清单数据。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>方法执行结果。</returns>
        [OperationContract]
        MethodReturnResult<DataSet> Get(ref LotMaterialListQueryParameter p);

        /// <summary>
        /// 查询物料消耗数据。
        /// </summary>
        /// <param name="p">参数</param>
        /// <returns>返回结果集</returns>
        [OperationContract]
        MethodReturnResult<DataSet> GetMaterialConsume(RPTDailyDataGetParameter p);


        /// <summary>
        /// 查询批次物料出库数据。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>方法执行结果。</returns>
        [OperationContract]
        MethodReturnResult<DataSet> GetRPTMaterialData(ref MaterialDataParameter p); 
    }
}
