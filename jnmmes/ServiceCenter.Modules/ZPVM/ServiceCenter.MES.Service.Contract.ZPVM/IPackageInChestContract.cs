using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.Model.ZPVM;
using ServiceCenter.Model;
using System.Data;
using ServiceCenter.MES.Service.Contract.ERP;

namespace ServiceCenter.MES.Service.Contract.ZPVM
{
    #region 注释Generate

    ///// <summary>
    //    /// 根据包装号生成柜号。
    //    /// </summary>
    //    /// <param name="ChestNo">柜号。</param>
    //    /// <param name="isLast">是否尾柜。</param>
    //    /// <returns>
    //    /// 方法执行结果。
    //    /// 代码表示：0：成功，其他失败。
    //    /// </returns>
    //    [OperationContract]
    //    MethodReturnResult<string> Generate(string chestNo,string PackageNo);

    #endregion

    /// <summary>
    /// 包装入柜方法的参数类。
    /// </summary>
    [DataContract]
    public class ChestParameter
    {
        [DataMember]
        public string ErrorMsg { get; set; }

        /// <summary>
        /// 页号。
        /// </summary>
        [DataMember]
        public int PageNo { get; set; }

        /// <summary>
        /// 每页大小。
        /// </summary>
        [DataMember]
        public int PageSize { get; set; }

        /// <summary>
        /// 总记录数。
        /// </summary>
        [DataMember]
        public int TotalRecords { get; set; }

        /// <summary>
        /// 柜号。
        /// </summary>
        [DataMember]
        public string ChestNo { get; set; }

        /// <summary>
        /// 工单号。
        /// </summary>
        [DataMember]
        public string OrderNumber { get; set; }

        [DataMember]
        /// <summary>
        /// 批次号
        /// </summary>
        public string LotNumber { get; set; }

        /// <summary>
        /// 入柜开始时间。
        /// </summary>
        [DataMember]
        public string ChestDateStart { get; set; }

        /// <summary>
        /// 入柜结束时间。
        /// </summary>
        [DataMember]
        public string ChestDateEnd { get; set; }

        /// <summary>
        /// 包装号。
        /// </summary>
        [DataMember]
        public string PackageNo { get; set; }

        /// <summary>
        /// 物料编码
        /// </summary>
        [DataMember]
        public string MaterialCode { get; set; }

        /// <summary>
        /// 编辑人。
        /// </summary>
        [DataMember]
        public string Editor { get; set; }

        /// <summary>
        /// 满柜数量。
        /// </summary>
        [DataMember]
        public double ChestFullQty { get; set; }

        /// <summary>
        /// 是否完成入柜。
        /// </summary>
        [DataMember]
        public bool IsFinishPackageInChest { get; set; }

        /// <summary>
        /// 是否尾柜？
        /// </summary>
        [DataMember]
        public bool IsLastestPackageInChest { get; set; }

        /// <summary>
        /// 库位
        /// </summary>
        [DataMember]
        public string StoreLocation { get; set; }

        /// <summary>
        /// 是否手动模式
        /// </summary>
        [DataMember]
        public bool isManual { get; set; }

        /// <summary>
        /// 入柜模式：【0--人为手动，1--系统自动】
        /// </summary>
        public int ModelType { get; set; }
    }

    /// <summary>
    /// 包装入柜操作契约接口。
    /// </summary>
    [ServiceContract]
    public interface IPackageInChestContract
    {
        /// <summary>
        /// 入柜操作。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>方法执行结果。</returns>
        [OperationContract]
        MethodReturnResult Chest(ChestParameter p);

        /// <summary>
        /// 完成入柜。
        /// </summary>
        [OperationContract]
        MethodReturnResult ChangeChest(string chestNo,string userName);

        /// <summary>
        /// 获取柜明细列表。
        /// </summary>
        [OperationContract(Name = "GetDetailList")]
        MethodReturnResult<IList<ChestDetail>> GetDetail(ref PagingConfig cfg);

        /// <summary>
        /// 获取柜明细。
        /// </summary>
        [OperationContract]
        MethodReturnResult<ChestDetail> GetDetail(ChestDetailKey key);

        /// <summary>
        /// 获取柜数据列表。
        /// </summary>
        [OperationContract(Name = "GetList")]
        MethodReturnResult<IList<Chest>> Get(ref PagingConfig cfg);

        /// <summary>
        /// 获取柜数据。
        /// </summary>
        [OperationContract]
        MethodReturnResult<Chest> Get(string key);

        /// <summary>
        /// 手动完成入柜操作。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>方法执行结果。</returns>
        [OperationContract]
        MethodReturnResult FinishChest(ChestParameter p);       

        /// <summary>
        /// 出柜操作。
        /// </summary>
        /// <param name="p">参数。</param>
        /// <returns>方法执行结果。</returns>
        [OperationContract]
        MethodReturnResult UnPackageInChest(ChestParameter p);

        /// <summary>
        /// 协鑫查询获取柜明细
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [OperationContract]
        MethodReturnResult<DataSet> GetChestDetail(ref ChestParameter p);

        /// <summary>
        /// 查询获取柜明细
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [OperationContract]
        MethodReturnResult<DataSet> GetChestDetailByDB(ref ChestParameter p);

        /// <summary>
        /// 刷新柜号明细数据（存储过程）
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [OperationContract]
        MethodReturnResult<DataSet> GetRefreshChestDetailByDB(ref ChestParameter p);

        /// <summary>
        /// 提取托号相关数据
        /// </summary>
        /// <param name="p">
        /// 1.提取（WIP_PACKAGE）表到当前库{p.ReType = 1,p.IsDelete = 0}
        /// 2.提取其他归档表数据到当前库，并删除从归档库{p.ReType = 2,p.IsDelete = 1}
        /// </param>
        /// <returns></returns>
        [OperationContract]
        MethodReturnResult GetREbackdata(REbackdataParameter p);

        /// <summary>
        /// 查询检验后柜号明细数据（存储过程）
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [OperationContract]
        MethodReturnResult<DataSet> GetCheckedChestDetailByDB(ref ChestParameter p); 

        /// <summary>
        /// 根据包装号查找符合条件的柜号。
        /// </summary>
        /// <param name="ChestNo">柜号。</param>
        /// <param name="isLast">是否尾柜。</param>
        /// <returns>
        /// 方法执行结果。
        /// 代码表示：0：成功，其他失败。
        /// </returns>
        [OperationContract]
        MethodReturnResult<string> GetChestNo(string PackageNo, string chestNo, bool isLastChest, bool isManual);

        /// <summary>
        /// 执行柜明细检验
        /// </summary>
        /// <param name="PackageNo"></param>
        /// <param name="chestNo"></param>
        /// <returns></returns>
        [OperationContract]
        MethodReturnResult CheckPackageInChest(string PackageNo, string chestNo, string userName);

        /// <summary>
        /// 执行柜明细取消检验
        /// </summary>
        /// <param name="PackageNo"></param>
        /// <param name="chestNo"></param>
        /// <returns></returns>
        [OperationContract]
        MethodReturnResult UnCheckPackageInChest(string PackageNo, string chestNo, string userName);
    }

    /// <summary>
    /// 用于扩展包装入柜检查的接口。
    /// </summary>
    public interface IPackageInChestCheck
    {
        /// <summary>
        /// 进行包装入柜前检查。
        /// </summary>
        /// <param name="p">入柜参数。</param>
        /// <returns>
        /// 错误代码表示：0：成功，其他失败。
        /// </returns>
        MethodReturnResult Check(ChestParameter p);
    }
}
