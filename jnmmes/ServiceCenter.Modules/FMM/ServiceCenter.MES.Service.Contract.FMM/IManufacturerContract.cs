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
    /// 定义生产厂商管理服务契约。
    /// </summary>
     [ServiceContract]
    public interface IManufacturerContract
    {
         /// <summary>
         /// 添加生产厂商。
         /// </summary>
         /// <param name="obj">生产厂商数据。</param>
         /// <returns><see cref="MethodReturnResult"/>.</returns>
         [OperationContract]
         MethodReturnResult Add(Manufacturer obj);
         /// <summary>
         /// 修改生产厂商。
         /// </summary>
         /// <param name="obj">生产厂商数据。</param>
         /// <returns><see cref="MethodReturnResult"/>.</returns>
         [OperationContract]
         MethodReturnResult Modify(Manufacturer obj);
         /// <summary>
         /// 删除生产厂商。
         /// </summary>
         /// <param name="key">生产厂商标识符。</param>
         /// <returns><see cref="MethodReturnResult"/>.</returns>
         [OperationContract]
         MethodReturnResult Delete(string key);
         /// <summary>
         /// 获取生产厂商数据。
         /// </summary>
         /// <param name="key">生产厂商标识符.</param>
         /// <returns><see cref="MethodReturnResult&lt;Manufacturer&gt;"/>,生产厂商数据.</returns>
         [OperationContract]
         MethodReturnResult<Manufacturer> Get(string key);

         ///// <summary>
         /////  获取生产厂商数据集合。
         ///// </summary>
         ///// <param name="cfg">查询参数.</param>
         ///// <returns>MethodReturnResult&lt;IList&lt;Manufacturer&gt;&gt;，生产厂商数据集合.</returns>
         [OperationContract(Name = "GetList")]
         MethodReturnResult<IList<Manufacturer>> Get(ref PagingConfig cfg);
    }
}
