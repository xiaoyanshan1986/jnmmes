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
    /// 定义设备布局数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IEquipmentLayoutContract
    {
         /// <summary>
         /// 添加设备布局数据。
         /// </summary>
         /// <param name="obj">设备布局数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(EquipmentLayout obj);
         /// <summary>
         /// 修改设备布局数据。
         /// </summary>
         /// <param name="obj">设备布局数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(EquipmentLayout obj);
         /// <summary>
         /// 删除设备布局数据。
         /// </summary>
         /// <param name="key">设备布局数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(string key);
         /// <summary>
         /// 获取设备布局数据。
         /// </summary>
         /// <param name="key">设备布局数据标识符.</param>
         /// <returns>MethodReturnResult&lt;EquipmentLayout&gt;，设备布局数据.</returns>
         [OperationContract]
         MethodReturnResult<EquipmentLayout> Get(string key);
         /// <summary>
         /// 获取设备布局数据集合。
         /// </summary>
         /// <param name="cfg">查询设备布局。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;EquipmentLayout&gt;&gt;，设备布局数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<EquipmentLayout>> Get(ref PagingConfig cfg);
    }
}
