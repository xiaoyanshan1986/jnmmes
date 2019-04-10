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
    /// 定义设备组数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IEquipmentGroupContract
    {
         /// <summary>
         /// 添加设备组数据。
         /// </summary>
         /// <param name="obj">设备组数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(EquipmentGroup obj);
         /// <summary>
         /// 修改设备组数据。
         /// </summary>
         /// <param name="obj">设备组数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(EquipmentGroup obj);
         /// <summary>
         /// 删除设备组数据。
         /// </summary>
         /// <param name="key">设备组数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(string key);
         /// <summary>
         /// 获取设备组数据。
         /// </summary>
         /// <param name="key">设备组数据标识符.</param>
         /// <returns>MethodReturnResult&lt;EquipmentGroup&gt;，设备组数据.</returns>
         [OperationContract]
         MethodReturnResult<EquipmentGroup> Get(string key);
         /// <summary>
         /// 获取设备组数据集合。
         /// </summary>
         /// <param name="cfg">查询设备组。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;EquipmentGroup&gt;&gt;，设备组数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<EquipmentGroup>> Get(ref PagingConfig cfg);
    }
}
