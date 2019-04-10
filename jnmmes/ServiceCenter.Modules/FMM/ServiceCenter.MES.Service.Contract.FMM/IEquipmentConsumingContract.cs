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
    /// 设备异常类型耗时管理服务契约。
    /// </summary>
    [ServiceContract]
    public interface IEquipmentConsumingContract
    {
        /// <summary>
        /// 添加设备异常类型耗时管理。
        /// </summary>
        /// <param name="obj">设备异常类型耗时管理。</param>
        /// <returns><see cref="MethodReturnResult"/></returns>
        [OperationContract]
        MethodReturnResult Add(EquipmentConsuming obj);

        /// <summary>
        /// 修改设备异常类型耗时管理。
        /// </summary>
        /// <param name="obj">设备异常类型耗时管理。</param>
        /// <returns><see cref="MethodReturnResult" /></returns>
        [OperationContract]
        MethodReturnResult Modify(EquipmentConsuming obj);

        /// <summary>
        /// 修改设备异常类型耗时管理。
        /// </summary>
        /// <param name="lst">设备异常类型耗时管理集合。</param>
        /// <returns><see cref="MethodReturnResult" /></returns>
        [OperationContract(Name = "ModifyList")]
        MethodReturnResult Modify(IList<EquipmentConsuming> lst);

        /// <summary>
        /// 删除设备异常类型耗时管理。
        /// </summary>
        /// <param name="key">设备异常类型耗时管理标识符.</param>
        /// <returns>MethodReturnResult.</returns>
        [OperationContract]
        MethodReturnResult Delete(EquipmentConsumingKey key);

        /// <summary>
        /// 获取设备异常类型耗时管理。
        /// </summary>
        /// <param name="key">设备异常类型耗时管理标识符.</param>
        /// <returns>MethodReturnResult&lt;EquipmentConsuming&gt;，设备异常类型耗时管理数据.</returns>
        [OperationContract]
        MethodReturnResult<EquipmentConsuming> Get(EquipmentConsumingKey key);

        /// <summary>
        /// 获取设备异常类型耗时管理集合。
        /// </summary>
        /// <param name="cfg">查询参数。</param>
        /// <returns>MethodReturnResult&lt;IList&lt;EquipmentConsuming&gt;&gt;，设备异常类型耗时管理集合。</returns>
        [OperationContract(Name = "GetList")]
        MethodReturnResult<IList<EquipmentConsuming>> Get(ref PagingConfig cfg);
    }
}
