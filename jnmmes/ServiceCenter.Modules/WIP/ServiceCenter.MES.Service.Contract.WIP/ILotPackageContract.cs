using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.Model;


namespace ServiceCenter.MES.Service.Contract.WIP
{
     /// <summary>
    /// 批次包装方法的参数类。
    /// </summary>
    [DataContract]
    public class PackageParameter : TrackParameter
    {
        /// <summary>
        /// 设备代码。
        /// </summary>
        //[DataMember]
        //public string EquipmentCode { get; set; }
        /// <summary>
        /// 包装号。
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
    public interface ILotPackageContract
    {
        /// <summary>
        /// 批次包装操作。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>方法执行结果。</returns>
        [OperationContract]
        MethodReturnResult Package(PackageParameter p);

        /// <summary>
        /// 批次包装操作。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>方法执行结果。</returns>
        [OperationContract]
        MethodReturnResult FinishPackage(PackageParameter p);

        /// <summary>
        /// 批次包装操作。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>方法执行结果。</returns>
        [OperationContract]
        MethodReturnResult TrackOutPackage(PackageParameter p);


        /// <summary>
        /// 拆包操作。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>方法执行结果。</returns>
        [OperationContract]
        MethodReturnResult UnPackage(PackageParameter p);


        /// <summary>
        /// 根据批次号生成包装号。
        /// </summary>
        /// <param name="lotNumber">批次号。</param>
        /// <param name="isLastestPackage">是否尾包。</param>
        /// <returns>
        /// 方法执行结果。
        /// 代码表示：0：成功，其他失败。
        /// </returns>
        [OperationContract]
        MethodReturnResult<string> Generate(string lotNumber, bool isLastestPackage);
    }

    /// <summary>
    /// 用于扩展批次包装检查的接口。
    /// </summary>
    public interface ILotPackageCheck
    {
        /// <summary>
        /// 进行批次包装前检查。
        /// </summary>
        /// <param name="p">包装参数。</param>
        /// <returns>
        /// 错误代码表示：0：成功，其他失败。
        /// </returns>
        MethodReturnResult Check(PackageParameter p);
    }

    /// <summary>
    /// 用于扩展批次包装执行的接口。
    /// </summary>
    public interface ILotPackage
    {
        /// <summary>
        /// 进行批次包装操作。
        /// </summary>
        /// <param name="p">包装参数。</param>
        /// <returns>
        /// 错误代码表示：0：成功，其他失败。
        /// </returns>
        MethodReturnResult Execute(PackageParameter p);

    }
    
    /// <summary>
    ///  包装号生成接口。
    /// </summary>
    public interface IPackageNoGenerate
    {
        /// <summary>
        /// 根据批次号生成包装号。
        /// </summary>
        /// <param name="lotNumber">批次号。</param>
        /// <param name="isLastestPackage">是否尾包。</param>
        /// <returns>包装号。</returns>
        //string Generate(string lotNumber, bool isLastestPackage);     

        /// <summary>
        /// 根据批次号生成托号
        /// </summary>
        /// <param name="lotNumber">批次号</param>
        /// <returns>托号</returns>

        MethodReturnResult<string> CreatePackageNo(string lotNumber);

        MethodReturnResult<string> CreatePackageNo(OemData oemData, WorkOrder oemWorkOrder);
    }
}
