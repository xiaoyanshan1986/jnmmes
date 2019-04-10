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
    /// 批次包装方法的参数类。
    /// </summary>
    [DataContract]
    public class InBinParameter : TrackParameter
    {
        /// <summary>
        /// 设备代码。
        /// </summary>
        [DataMember]
        public string ScanNo { get; set; }

        /// <summary>
        /// 设备IP地址。
        /// </summary>
        [DataMember]
        public string ScanIP { get; set; }

        /// <summary>
        /// 组件序列号。
        /// </summary>
        [DataMember]
        public string ScanLotNumber { get; set; }

        /// <summary>
        /// 线别
        /// </summary>
        [DataMember]
        public string PackageLine { get; set; }

        /// <summary>
        /// Bin号
        /// </summary>
        [DataMember]
        public string BinNo { get; set; }

        /// <summary>
        /// 包装号
        /// </summary>
        [DataMember]
        public string PackageNo { get; set; }   
    
        /// <summary>
        /// 是否完成包装。
        /// </summary>
        [DataMember]
        public bool IsFinishPackage { get; set; }

        /// <summary>
        /// 是否尾包？
        /// </summary>
        [DataMember]
        public bool IsLastestPackage { get; set; }

    }

    /// <summary>
    /// 批次包装操作契约接口。
    /// </summary>
    [ServiceContract]
    public interface ILotBinContract
    {
        /// <summary>
        /// 批次包装操作。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>方法执行结果。</returns>
        [OperationContract]
        MethodReturnResult InBin(InBinParameter p);


        /// <summary>
        /// 批次包装操作。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>方法执行结果。</returns>
        [OperationContract]
        MethodReturnResult ChkBin(InBinParameter p);

        /// <summary>
        /// 批次包装操作。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>方法执行结果。</returns>
        [OperationContract]
        MethodReturnResult PathCheck(InBinParameter p);
        ///// <summary>
        ///// 批次包装操作。
        ///// </summary>
        ///// <param name="p">参数。</param>
        ///// <returns>方法执行结果。</returns>
        //[OperationContract]
        //MethodReturnResult UnInBin(InBinParameter p);


        ///// <summary>
        ///// 拆包操作。
        ///// </summary>
        ///// <param name="p">参数。</param>
        ///// <returns>方法执行结果。</returns>
        //[OperationContract]
        //MethodReturnResult FinishBin(InBinParameter p);



        /// <summary>
        /// 拆包操作。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>方法执行结果。</returns>
        [OperationContract]
        MethodReturnResult<IList<PackageBin>> QueryBinListFromPackageLine(string packageLine);

    }

    /// <summary>
    /// 用于扩展批次包装检查的接口。
    /// </summary>
    public interface ILotBinCheck
    {
        /// <summary>
        /// 进行批次包装前检查。
        /// </summary>
        /// <param name="p">包装参数。</param>
        /// <returns>
        /// 错误代码表示：0：成功，其他失败。
        /// </returns>
        MethodReturnResult Check(InBinParameter p);
    }
   
    /// <summary>
    /// 用于扩展批次包装执行的接口。
    /// </summary>
    public interface ILotBin
    {
        /// <summary>
        /// 进行批次包装操作。
        /// </summary>
        /// <param name="p">包装参数。</param>
        /// <returns>
        /// 错误代码表示：0：成功，其他失败。
        /// </returns>
        MethodReturnResult Execute(InBinParameter p);

    }
}
