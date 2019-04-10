using ServiceCenter.Common.Model;
using ServiceCenter.MES.Model.LSM;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.Contract.LSM
{

    /// <summary>
    /// 物料批号拆批的参数类。
    /// </summary>
    [DataContract]
    public class SplitMaterialLotParameter : BaseMethodParameter
    {
        /// <summary>
        /// 父批次号。
        /// </summary>
        [DataMember]
        public string ParentMaterialLotNumber { get; set; }
        /// <summary>
        /// 子批次号及批次数量
        /// </summary>
        [DataMember]
        public IList<ChildMaterialLotParameter> ChildMaterialLot { get; set; }
        /// <summary>
        /// 拆批数量
        /// </summary>
        [DataMember]
        public int count { get; set; }

        /// <summary>
        /// 工单号。
        /// </summary>
        [DataMember]
        public string OrderNumber { get; set; }
    }


    [DataContract]
    public class ChildMaterialLotParameter
    {
        /// <summary>
        /// 物料批号。
        /// </summary>
        [DataMember]
        public string MaterialLotNumber { get; set; }


        /// <summary>
        /// 领取数量。
        /// </summary>
        [DataMember]
        public double Quantity { get; set; }

    }
    /// <summary>
    /// 定义线边仓物料数据服务契约。
    /// </summary>
    [ServiceContract]
    public interface ILineStoreMaterialContract
    {
        /// <summary>
        /// 获取线边仓物料数据。
        /// </summary>
        /// <param name="key">线边仓物料数据标识符.</param>
        /// <returns>MethodReturnResult&lt;LineStoreMaterial&gt;，线边仓物料数据.</returns>
        [OperationContract]
        MethodReturnResult<LineStoreMaterial> Get(LineStoreMaterialKey key);
        /// <summary>
        /// 获取线边仓物料数据集合。
        /// </summary>
        /// <param name="cfg">查询线边仓物料。</param>
        /// <returns>MethodReturnResult&lt;IList&lt;LineStoreMaterial&gt;&gt;，线边仓物料数据集合。</returns>
        [OperationContract(Name = "GetList")]
        MethodReturnResult<IList<LineStoreMaterial>> Get(ref PagingConfig cfg);
        /// <summary>
        /// 获取线边仓物料明细数据。
        /// </summary>
        /// <param name="key">线边仓物料明细数据标识符.</param>
        /// <returns>MethodReturnResult&lt;LineStoreMaterialDetail&gt;，线边仓物料明细数据.</returns>
        [OperationContract]
        MethodReturnResult<LineStoreMaterialDetail> GetDetail(LineStoreMaterialDetailKey key);
        /// <summary>
        /// 获取线边仓物料明细数据集合。
        /// </summary>
        /// <param name="cfg">查询线边仓物料明细。</param>
        /// <returns>MethodReturnResult&lt;IList&lt;LineStoreMaterialDetail&gt;&gt;，线边仓物料明细数据集合。</returns>
        [OperationContract(Name = "GetDetailList")]
        MethodReturnResult<IList<LineStoreMaterialDetail>> GetDetail(ref PagingConfig cfg);
        /// <summary>
        /// 物料拆批
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [OperationContract]
        MethodReturnResult SplitMaterialLot(SplitMaterialLotParameter p);

    }
}
