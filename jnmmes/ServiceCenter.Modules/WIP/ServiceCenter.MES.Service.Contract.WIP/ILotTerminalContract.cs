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
    /// 表示批次结束方法的参数类。
    /// </summary>
    [DataContract]
    public class TerminalParameter : MethodParameter
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
    }
    /// <summary>
    /// 批次结束操作契约接口。
    /// </summary>
    [ServiceContract]
    public interface ILotTerminalContract
    {
        /// <summary>
        /// 批次结束操作。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>方法执行结果。</returns>
        [OperationContract]
        MethodReturnResult Terminal(TerminalParameter p);
    }


    /// <summary>
    /// 用于扩展批次结束前检查的接口。
    /// </summary>
    public interface ILotTerminalCheck
    {
        /// <summary>
        /// 进行批次结束前检查。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>
        /// 错误代码表示：0：成功，其他失败。
        /// </returns>
        MethodReturnResult Check(TerminalParameter p);
    }
    /// <summary>
    /// 用于扩展批次结束的接口。
    /// </summary>
    public interface ILotTerminal
    {
        /// <summary>
        /// 进行批次结束操作。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>
        /// 错误代码表示：0：成功，其他失败。
        /// </returns>
        MethodReturnResult Execute(TerminalParameter p);
    }

}
