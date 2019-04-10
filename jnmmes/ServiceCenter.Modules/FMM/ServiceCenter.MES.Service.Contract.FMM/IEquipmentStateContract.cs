using ServiceCenter.MES.Model.FMM;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.Contract.FMM
{
    /// <summary>
    /// 定义设备状态数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IEquipmentStateContract
    {
         /// <summary>
         /// 添加设备状态数据。
         /// </summary>
         /// <param name="obj">设备状态数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(EquipmentState obj);
         /// <summary>
         /// 修改设备状态数据。
         /// </summary>
         /// <param name="obj">设备状态数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(EquipmentState obj);
         /// <summary>
         /// 删除设备状态数据。
         /// </summary>
         /// <param name="key">设备状态数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(string key);
         /// <summary>
         /// 获取设备状态数据。
         /// </summary>
         /// <param name="key">设备状态数据标识符.</param>
         /// <returns>MethodReturnResult&lt;EquipmentState&gt;，设备状态数据.</returns>
         [OperationContract]
         MethodReturnResult<EquipmentState> Get(string key);
         /// <summary>
         /// 获取设备状态数据集合。
         /// </summary>
         /// <param name="cfg">查询设备状态。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;EquipmentState&gt;&gt;，设备状态数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<EquipmentState>> Get(ref PagingConfig cfg);
    }
}
