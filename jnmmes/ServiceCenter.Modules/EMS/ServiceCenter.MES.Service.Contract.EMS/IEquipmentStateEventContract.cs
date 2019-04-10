using ServiceCenter.MES.Model.EMS;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.Contract.EMS
{
    /// <summary>
    /// 定义设备状态事件数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IEquipmentStateEventContract
    {
         /// <summary>
         /// 添加设备状态事件数据。
         /// </summary>
         /// <param name="obj">设备状态事件数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(EquipmentStateEvent obj);
         /// <summary>
         /// 修改设备状态事件数据。
         /// </summary>
         /// <param name="obj">设备状态事件数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(EquipmentStateEvent obj);
         /// <summary>
         /// 删除设备状态事件数据。
         /// </summary>
         /// <param name="key">设备状态事件数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(string key);
         /// <summary>
         /// 获取设备状态事件数据。
         /// </summary>
         /// <param name="key">设备状态事件数据标识符.</param>
         /// <returns>MethodReturnResult&lt;EquipmentStateEvent&gt;，设备状态事件数据.</returns>
         [OperationContract]
         MethodReturnResult<EquipmentStateEvent> Get(string key);
         /// <summary>
         /// 获取设备状态事件数据集合。
         /// </summary>
         /// <param name="cfg">查询设备状态事件。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;EquipmentStateEvent&gt;&gt;，设备状态事件数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<EquipmentStateEvent>> Get(ref PagingConfig cfg);
    }
}
