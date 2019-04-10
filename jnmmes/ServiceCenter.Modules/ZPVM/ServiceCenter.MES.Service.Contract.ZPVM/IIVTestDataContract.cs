using ServiceCenter.Common.Model;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.Contract.ZPVM
{
    [DataContract]
    public class LotIVdataParameter : BaseMethodParameter
    {
        /// <summary>
        /// 批次列表
        /// </summary>
        [DataMember]
        public string Lotlist { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [DataMember]
        public string StratTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>

        [DataMember]
        public string EndTime { get; set; }
        /// <summary>
        /// 是否有效
        /// </summary>

        [DataMember]
        public bool? IsDefault { get; set; }

        /// <summary>
        /// 是否打印
        /// </summary>

        [DataMember]
        public bool? IsPrint { get; set; }

        /// <summary>
        /// 设备代码
        /// </summary>

        [DataMember]
        public string EquipmentCode { get; set; }

        /// <summary>
        /// 设备代码
        /// </summary>

        [DataMember]
        public string LineCode { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>

        [DataMember]
        public string ErrorMsg { get; set; }
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
    /// 定义IV测试数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IIVTestDataContract
    {         
         /// <summary>
         /// 添加IV测试数据。
         /// </summary>
         /// <param name="obj">IV测试数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(IVTestData obj);
         /// <summary>
         /// 修改IV测试数据。
         /// </summary>
         /// <param name="obj">IV测试数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(IVTestData obj);
         /// <summary>
         /// 删除IV测试数据。
         /// </summary>
         /// <param name="key">IV测试数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(IVTestDataKey key);
         /// <summary>
         /// 获取IV测试数据。
         /// </summary>
         /// <param name="key">IV测试数据标识符.</param>
         /// <returns>MethodReturnResult&lt;IVTestData&gt;，IV测试数据.</returns>
         [OperationContract]
         MethodReturnResult<IVTestData> Get(IVTestDataKey key);
         /// <summary>
         /// 获取IV测试数据集合。
         /// </summary>
         /// <param name="cfg">查询IV测试数据。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;IVTestData&gt;&gt;，IV测试数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<IVTestData>> Get(ref PagingConfig cfg);

         [OperationContract]
         MethodReturnResult<DataSet> GetIVdata(ref LotIVdataParameter p);

         [OperationContract]
         MinPaiData GetMinPaiData(string LotNumber, string lineCode);

         [OperationContract]
         string CallBack(string lotNumber, string ErrorMessage, string lineCode);
    }
}
