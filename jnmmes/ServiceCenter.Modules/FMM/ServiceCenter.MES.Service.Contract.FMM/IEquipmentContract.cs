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
    /// 定义设备数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IEquipmentContract
    {
         /// <summary>
         /// 添加设备数据。
         /// </summary>
         /// <param name="obj">设备数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(Equipment obj);
         /// <summary>
         /// 修改设备数据。
         /// </summary>
         /// <param name="obj">设备数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(Equipment obj);
         /// <summary>
         /// 删除设备数据。
         /// </summary>
         /// <param name="key">设备数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(string key);
         /// <summary>
         /// 获取设备数据。
         /// </summary>
         /// <param name="key">设备数据标识符.</param>
         /// <returns>MethodReturnResult&lt;Equipment&gt;，设备数据.</returns>
         [OperationContract]
         MethodReturnResult<Equipment> Get(string key);
         /// <summary>
         /// 获取设备数据集合。
         /// </summary>
         /// <param name="cfg">查询设备。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;Equipment&gt;&gt;，设备数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<Equipment>> Get(ref PagingConfig cfg);
    }
}
