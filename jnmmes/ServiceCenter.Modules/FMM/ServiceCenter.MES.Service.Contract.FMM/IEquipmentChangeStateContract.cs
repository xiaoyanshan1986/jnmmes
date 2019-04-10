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
    /// 定义设备可切换状态数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IEquipmentChangeStateContract
    {
         /// <summary>
         /// 添加设备可切换状态数据。
         /// </summary>
         /// <param name="obj">设备可切换状态数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(EquipmentChangeState obj);
         /// <summary>
         /// 修改设备可切换状态数据。
         /// </summary>
         /// <param name="obj">设备可切换状态数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(EquipmentChangeState obj);
         /// <summary>
         /// 删除设备可切换状态数据。
         /// </summary>
         /// <param name="key">设备可切换状态数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(string key);
         /// <summary>
         /// 获取设备可切换状态数据。
         /// </summary>
         /// <param name="key">设备可切换状态数据标识符.</param>
         /// <returns>MethodReturnResult&lt;EquipmentChangeState&gt;，设备可切换状态数据.</returns>
         [OperationContract]
         MethodReturnResult<EquipmentChangeState> Get(string key);
         /// <summary>
         /// 获取设备可切换状态数据集合。
         /// </summary>
         /// <param name="cfg">查询设备可切换状态。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;EquipmentChangeState&gt;&gt;，设备可切换状态数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<EquipmentChangeState>> Get(ref PagingConfig cfg);
    }
}
