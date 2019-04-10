using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using ServiceCenter.MES.Model.EDC;
using ServiceCenter.Model;
using System.Runtime.Serialization;
using System.Data;

namespace ServiceCenter.MES.Service.Contract.EDC
{
    /// <summary>
    /// 组件日运营数据获取参数类。
    /// </summary>
    [DataContract]
    public class DataAcquisitionDataGetParameter
    {
        /// <summary>
        /// 项目代码
        /// </summary>
        [DataMember]
        public string ItemCode { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>
        [DataMember]
        public string StartDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        [DataMember]
        public string EndDate { get; set; }

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
        /// 设备代码
        /// </summary>
        [DataMember]
        public string EquipmentCode { get; set; }

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
        public int Records { get; set; }
    }

    /// <summary>
    /// 定义采集数据数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IDataAcquisitionDataContract
    {

        /// <summary>
        /// 添加采集数据。
        /// </summary>
        /// <param name="obj">采集数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        [OperationContract]
        MethodReturnResult Add(DataAcquisitionData obj);

        /// <summary>
        /// 添加采集项目值。
        /// </summary>
        /// <param name="lst">基础属性数据值数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        [OperationContract(Name = "AddList")]
        MethodReturnResult Add(IList<DataAcquisitionData> lst);

        /// <summary>
        /// 修改采集项目值
        /// </summary>
        /// <param name="obj">采集数据数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        [OperationContract]
        MethodReturnResult Modify(DataAcquisitionData obj);

        /// <summary>
        /// 添加采集项目值。
        /// </summary>
        /// <param name="lst">基础属性数据值数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        [OperationContract(Name = "ModifyList")]
        MethodReturnResult Modify(IList<DataAcquisitionData> lst);

        /// <summary>
        /// 删除采集数据。
        /// </summary>
        /// <param name="name">采集数据主键</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        [OperationContract]
        MethodReturnResult Delete(DataAcquisitionDataKey name);

        /// <summary>
        /// 删除采集数据（条件）
        /// </summary>
        /// <param name="eDCTime">采集时间</param>
        /// <param name="itemCode">项目代码</param>
        /// <param name="lineCode">线别</param>
        /// <param name="equipmentCode">设备代码</param>
        /// <param name="locationName">车间</param>
        /// <returns></returns>
        [OperationContract(Name = "DeleteList")]
        MethodReturnResult Delete(DateTime eDCTime, string itemCode, string lineCode, string equipmentCode, string locationName);

        /// <summary>
        /// 获取采集数据数据。
        /// </summary>
        /// <param name="name">采集数据主键</param>
        /// <returns><see cref="MethodReturnResult&lt;DataAcquisitionData&gt;" />,采集数据数据.</returns>
        [OperationContract]
        MethodReturnResult<DataAcquisitionData> Get(DataAcquisitionDataKey name);

        /// <summary>
        /// 获取采集数据数据集合。
        /// </summary>
        /// <param name="cfg">查询参数.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;DataAcquisitionData&gt;&gt;，采集数据数据集合.</returns>
        [OperationContract(Name = "GetList")]
        MethodReturnResult<IList<DataAcquisitionData>> Get(ref PagingConfig cfg);

        /// <summary>
        /// 采集数据获取操作。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>方法执行结果。</returns>
        [OperationContract]
        MethodReturnResult<DataSet> GetData(ref DataAcquisitionDataGetParameter p);
    }
}
