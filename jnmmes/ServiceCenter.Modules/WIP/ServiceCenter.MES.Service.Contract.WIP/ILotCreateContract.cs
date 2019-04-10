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
    /// 表示批次创建方法的参数类。
    /// </summary>
    [DataContract]
    public class CreateParameter : MethodParameter
    {
        /// <summary>
        /// 工单号。
        /// </summary>
        [DataMember]
        public string OrderNumber { get; set; }
        /// <summary>
        /// 批次类型。
        /// </summary>
         [DataMember]
        public EnumLotType LotType { get; set; }
         /// <summary>
         /// 线别。
         /// </summary>
         [DataMember]
         public string LineCode { get; set; }
        /// <summary>
        /// 每批次产品数量。
        /// </summary>
         [DataMember]
        public double LotQuantity { get; set; }
        /// <summary>
        /// 工艺流程组名称。
        /// </summary>
         [DataMember]
        public string RouteEnterpriseName { get; set; }
        /// <summary>
        /// 工艺流程名称。
        /// </summary>
         [DataMember]
        public string RouteName { get; set; }
        /// <summary>
        /// 工艺工步名称。
        /// </summary>
         [DataMember]
        public string RouteStepName { get; set; }
        /// <summary>
        /// 线边仓名称。
        /// </summary>
         [DataMember]
        public string LineStoreName { get; set; }
        /// <summary>
        /// 原材料编码。
        /// </summary>
         [DataMember]
        public string RawMaterialCode { get; set; }
        /// <summary>
        /// 原材料批号。
        /// </summary>
         [DataMember]
        public string RawMaterialLot { get; set; }
        /// <summary>
        /// 每批次原材料数量。
        /// </summary>
         [DataMember]
        public double RawQuantity { get; set; }
        /// <summary>
        /// 录入的自定义属性集合。
        /// </summary>
        //[DataMember]
        //public IDictionary<string, IList<TransactionParameter>> Attributes { get; set; }

    }

    /// <summary>
    /// 批次创建操作契约接口。
    /// </summary>
    [ServiceContract]
    public interface ILotCreateContract
    {
        /// <summary>
        /// 批次创建操作。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>
        /// 方法执行结果。
        /// 代码表示：0：成功，其他失败。
        /// </returns>
        [OperationContract]
        MethodReturnResult Create(CreateParameter p);
        /// <summary>
        /// 根据工单号生成批次号。
        /// </summary>
        /// <param name="lotType">批次类型。</param>
        /// <param name="orderNumber">工单号。</param>
        /// <param name="count">批次个数。</param>
        /// <returns>
        /// 方法执行结果。
        /// 代码表示：0：成功，其他失败。
        /// </returns>
        [OperationContract]
        MethodReturnResult<IList<string>> Generate(EnumLotType lotType, string orderNumber, int count, string prefix);

        /// <summary>
        /// 批次匹配优化器
        /// </summary>
        /// <param name="lot"></param>
        /// <returns></returns>
        [OperationContract]
        MethodReturnResult UpdateLotSEModules(Lot lot);
    }


    /// <summary>
    /// 用于扩展批次创建前检查的接口。
    /// </summary>
    public interface ILotCreateCheck
    {
        /// <summary>
        /// 进行批次创建前检查。
        /// </summary>
        /// <param name="p">创建参数。</param>
        /// <returns>
        /// 错误代码表示：0：成功，其他失败。
        /// </returns>
        MethodReturnResult Check(CreateParameter p);
    }

    /// <summary>
    /// 用于扩展批次创建执行的接口。
    /// </summary>
    public interface ILotCreate
    {
        /// <summary>
        /// 进行批次创建操作。
        /// </summary>
        /// <param name="p">创建参数。</param>
        /// <returns>
        /// 错误代码表示：0：成功，其他失败。
        /// </returns>
        MethodReturnResult Execute(CreateParameter p);
    }

    /// <summary>
    ///  批次号生成接口。
    /// </summary>
    public interface ILotNumberGenerate
    {
        /// <summary>
        /// 根据批次个数生成批次号。
        /// </summary>
        /// <param name="lotType">批次类型。</param>
        /// <param name="orderNumber">工单号。</param>
        /// <param name="count">批次个数。</param>
        /// <returns>批次号集合。</returns>
        IList<string> Generate(EnumLotType lotType, string orderNumber, int count, string other);
    }
}
