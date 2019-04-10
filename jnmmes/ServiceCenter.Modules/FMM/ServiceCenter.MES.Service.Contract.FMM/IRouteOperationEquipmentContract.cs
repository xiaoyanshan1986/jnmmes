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
    /// 定义工序设备数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IRouteOperationEquipmentContract
    {
         /// <summary>
         /// 添加工序设备数据。
         /// </summary>
         /// <param name="obj">工序设备数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(RouteOperationEquipment obj);
         /// <summary>
         /// 修改工序设备数据。
         /// </summary>
         /// <param name="obj">工序设备数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(RouteOperationEquipment obj);
         /// <summary>
         /// 删除工序设备数据。
         /// </summary>
         /// <param name="key">工序设备数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(RouteOperationEquipmentKey key);
         /// <summary>
         /// 获取工序设备数据。
         /// </summary>
         /// <param name="key">工序设备数据标识符.</param>
         /// <returns>MethodReturnResult&lt;RouteOperationEquipment&gt;，工序设备数据.</returns>
         [OperationContract]
         MethodReturnResult<RouteOperationEquipment> Get(RouteOperationEquipmentKey key);
         /// <summary>
         /// 获取工序设备数据集合。
         /// </summary>
         /// <param name="cfg">查询参数。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;RouteOperationEquipment&gt;&gt;，工序设备数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<RouteOperationEquipment>> Get(ref PagingConfig cfg);
    }
}
