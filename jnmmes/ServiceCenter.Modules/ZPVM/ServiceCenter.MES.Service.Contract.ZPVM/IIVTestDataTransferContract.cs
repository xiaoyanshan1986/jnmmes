using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.Model;
using ServiceCenter.Common;
using ServiceCenter.MES.Model.ZPVM;

namespace ServiceCenter.MES.Service.Contract.ZPVM
{
    /// <summary>
    /// IV测试数据移转的参数类。
    /// </summary>
    [DataContract]
    public class IVTestDataTransferParameter
    {
        /// <summary>
        /// IV测试数据集合。
        /// </summary>
        [DataMember]
        public IList<IVTestData> List { get; set; }
    }
    /// <summary>
    /// IV测试数据移转契约接口。
    /// </summary>
    [ServiceContract]
    public interface IIVTestDataTransferContract
    {
        /// <summary>
        /// IV测试数据移转操作。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>方法执行结果。</returns>
        [OperationContract]
        MethodReturnResult Transfer(IVTestDataTransferParameter p);
    }
    /// <summary>
    /// 用于扩展IV测试数据移转检查的接口。
    /// </summary>
    public interface IIVTestDataTransferCheck
    {
        /// <summary>
        /// 进行IV测试数据移转前检查。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>
        /// 错误代码表示：0：成功，其他失败。
        /// </returns>
        MethodReturnResult Check(IVTestDataTransferParameter p);
    }

    /// <summary>
    /// 用于扩展IV测试数据移转执行的接口。
    /// </summary>
    public interface IIVTestDataTransfer
    {
        /// <summary>
        /// 进行IV测试数据移转操作。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>
        /// 错误代码表示：0：成功，其他失败。
        /// </returns>
        MethodReturnResult Execute(IVTestDataTransferParameter p);
    }
}
