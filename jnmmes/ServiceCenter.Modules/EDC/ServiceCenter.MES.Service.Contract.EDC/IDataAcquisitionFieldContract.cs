using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using ServiceCenter.MES.Model.EDC;
using ServiceCenter.Model;

namespace ServiceCenter.MES.Service.Contract.EDC
{
    /// <summary>
    /// 定义采集项目字段数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IDataAcquisitionFieldContract
    {
        /// <summary>
        /// 添加采集字段。
        /// </summary>
        /// <param name="obj">采集字段数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        [OperationContract]
        MethodReturnResult Add(DataAcquisitionField obj);

        /// <summary>
        /// 修改采集字段。
        /// </summary>
        /// <param name="obj">采集字段数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        [OperationContract]
        MethodReturnResult Modify(DataAcquisitionField obj);

        /// <summary>
        /// 删除采集字段。
        /// </summary>
        /// <param name="name">采集字段名。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        [OperationContract]
        MethodReturnResult Delete(DataAcquisitionFieldKey name);

        /// <summary>
        /// 获取采集字段数据。
        /// </summary>
        /// <param name="name">采集字段名.</param>
        /// <returns><see cref="MethodReturnResult&lt;DataAcquisitionField&gt;" />,采集字段数据.</returns>
        [OperationContract]
        MethodReturnResult<DataAcquisitionField> Get(DataAcquisitionFieldKey name);

        /// <summary>
        /// 获取采集字段数据集合。
        /// </summary>
        /// <param name="cfg">查询参数.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;DataAcquisitionField&gt;&gt;，采集字段数据集合.</returns>
        [OperationContract(Name = "GetList")]
        MethodReturnResult<IList<DataAcquisitionField>> Get(ref PagingConfig cfg);
    }
}
