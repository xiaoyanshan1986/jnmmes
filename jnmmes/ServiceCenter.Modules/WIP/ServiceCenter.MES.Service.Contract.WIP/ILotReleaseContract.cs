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
    /// 表示批次释放方法的参数类。
    /// </summary>
    [DataContract]
    public class ReleaseParameter : MethodParameter
    {
        /// <summary>
        /// 释放描述。
        /// </summary>
        [DataMember]
        public string ReleaseDescription { get; set; }
        /// <summary>
        /// 释放密码。
        /// </summary>
        [DataMember]
        public string ReleasePassword { get; set; }
    }
    /// <summary>
    /// 批次释放操作契约接口。
    /// </summary>
    [ServiceContract]
    public interface ILotReleaseContract
    {
        /// <summary>
        /// 批次释放操作。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>方法执行结果。</returns>
        [OperationContract]
        MethodReturnResult Release(ReleaseParameter p);
    }


    /// <summary>
    /// 用于扩展批次暂停释放前检查的接口。
    /// </summary>
    public interface ILotReleaseCheck
    {
        /// <summary>
        /// 进行批次暂停释放前检查。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>
        /// 错误代码表示：0：成功，其他失败。
        /// </returns>
        MethodReturnResult Check(ReleaseParameter p);
    }
    /// <summary>
    /// 用于扩展批次暂停释放的接口。
    /// </summary>
    public interface ILotRelease
    {
        /// <summary>
        /// 进行批次暂停释放操作。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>
        /// 错误代码表示：0：成功，其他失败。
        /// </returns>
        MethodReturnResult Execute(ReleaseParameter p);
    }

}
