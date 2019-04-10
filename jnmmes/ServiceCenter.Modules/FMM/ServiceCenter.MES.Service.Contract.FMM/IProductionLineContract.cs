using ServiceCenter.MES.Model.FMM;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.Contract.FMM
{
    /// <summary>
    /// 定义生产线数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IProductionLineContract
    {
         /// <summary>
         /// 添加生产线数据。
         /// </summary>
         /// <param name="obj">生产线数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(ProductionLine obj);
         /// <summary>
         /// 修改生产线数据。
         /// </summary>
         /// <param name="obj">生产线数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(ProductionLine obj);
         /// <summary>
         /// 删除生产线数据。
         /// </summary>
         /// <param name="key">生产线数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(string key);
         /// <summary>
         /// 获取生产线数据。
         /// </summary>
         /// <param name="key">生产线数据标识符.</param>
         /// <returns>MethodReturnResult&lt;ProductionLine&gt;，生产线数据.</returns>
         [OperationContract]
         MethodReturnResult<ProductionLine> Get(string key);
         /// <summary>
         /// 获取生产线数据集合。
         /// </summary>
         /// <param name="cfg">查询参数。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;ProductionLine&gt;&gt;，生产线数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<ProductionLine>> Get(ref PagingConfig cfg);
    }
}
