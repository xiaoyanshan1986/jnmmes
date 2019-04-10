using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.Model;

namespace ServiceCenter.MES.Service.Contract.WIP
{
    /// <summary>
    /// 批次暂停方法的参数类。
    /// </summary>
    [DataContract]
    public class HoldParameter:MethodParameter
    {
        /// <summary>
        /// 原因组名称。
        /// </summary>
        [DataMember]
        public string ReasonCodeCategoryName { get; set; }
        /// <summary>
        /// 原因代码名称。
        /// </summary>
        [DataMember]
        public string ReasonCodeName { get; set; }
        /// <summary>
        /// 暂停描述。
        /// </summary>
        [DataMember]
        public string HoldDescription { get; set; }
        /// <summary>
        /// 暂停密码。
        /// </summary>
        [DataMember]
        public string HoldPassword { get; set; }
    }

    /// <summary>
    /// 批次暂停操作契约接口。
    /// </summary>
    [ServiceContract]
    public interface ILotHoldContract
    {
        /// <summary>
        /// 批次暂停操作。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>
        /// 方法执行结果。
        /// 代码表示：0：成功，其他失败。
        /// </returns>
        [OperationContract]
        MethodReturnResult Hold(HoldParameter p);
    }


    /// <summary>
    /// 用于扩展批次暂停前检查的接口。
    /// </summary>
    public interface ILotHoldCheck
    {
        /// <summary>
        /// 进行批次暂停前检查。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>
        /// 错误代码表示：0：成功，其他失败。
        /// </returns>
        MethodReturnResult Check(HoldParameter p);
    }
    /// <summary>
    /// 用于扩展批次暂停的接口。
    /// </summary>
    public interface ILotHold
    {
        /// <summary>
        /// 进行批次暂停操作。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>
        /// 错误代码表示：0：成功，其他失败。
        /// </returns>
        MethodReturnResult Execute(HoldParameter p);
    }

}
