using ServiceCenter.MES.Model.FMM;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.Contract.FMM
{
    /// <summary>
    /// 定义设备布局明细数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IEquipmentLayoutDetailContract
    {
         /// <summary>
         /// 添加设备布局明细数据。
         /// </summary>
         /// <param name="obj">设备布局明细数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(EquipmentLayoutDetail obj);
         /// <summary>
         /// 修改设备布局明细数据。
         /// </summary>
         /// <param name="obj">设备布局明细数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(EquipmentLayoutDetail obj);
         /// <summary>
         /// 删除设备布局明细数据。
         /// </summary>
         /// <param name="key">设备布局明细数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(EquipmentLayoutDetailKey key);
         /// <summary>
         /// 获取设备布局明细数据。
         /// </summary>
         /// <param name="key">设备布局明细数据标识符.</param>
         /// <returns>MethodReturnResult&lt;EquipmentLayoutDetail&gt;，设备布局明细数据.</returns>
         [OperationContract]
         MethodReturnResult<EquipmentLayoutDetail> Get(EquipmentLayoutDetailKey key);
         /// <summary>
         /// 获取设备布局明细数据集合。
         /// </summary>
         /// <param name="cfg">查询设备布局明细。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;EquipmentLayoutDetail&gt;&gt;，设备布局明细数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<EquipmentLayoutDetail>> Get(ref PagingConfig cfg);


         [OperationContract]
         MethodReturnResult<DataTable> GetEQPInfo(string LayoutName);

         [OperationContract]
         MethodReturnResult<DataTable> GetParameByEqpCode(string EqpCode);

         /// <summary>
         /// 获取设备状态及最后一次设备状态变更EVENT的信息
         /// </summary>
         /// <param name="EqpCode"></param>
         /// <returns></returns>
         [OperationContract]
         MethodReturnResult<DataTable> GetEquipmentInfo(string EqpCode);
    }
}
