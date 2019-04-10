using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Model.ZPVC;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.Contract.ZPVC
{
    /// <summary>
    /// 装箱参数类。
    /// </summary>
    [DataContract]
    public class BoxParameter
    {
        /// <summary>
        /// 箱号。
        /// </summary>
        [DataMember]
        public string BoxNo { get; set; }
        /// <summary>
        /// 包号。
        /// </summary>
        [DataMember]
        public string PackageNo { get; set; }
        [DataMember]
        public string Creator { get; set; }
    }
    /// <summary>
    /// 拆箱参数类。
    /// </summary>
    [DataContract]
    public class UnboxParameter : BoxParameter
    {

    }
    /// <summary>
    /// 定义装箱服务契约。
    /// </summary>
     [ServiceContract]
    public interface IBoxContract
    {
         /// <summary>
         /// 装箱。
         /// </summary>
         /// <param name="p">装箱参数类。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Box(BoxParameter p);
         /// <summary>
         /// 拆箱。
         /// </summary>
         /// <param name="p">拆箱参数类。</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Unbox(UnboxParameter p);
    }
}
