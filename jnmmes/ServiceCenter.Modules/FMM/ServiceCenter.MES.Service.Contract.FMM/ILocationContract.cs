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
    /// 定义区域数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface ILocationContract
    {
         /// <summary>
         /// 添加区域数据。
         /// </summary>
         /// <param name="obj">区域数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(Location obj);
         /// <summary>
         /// 修改区域数据。
         /// </summary>
         /// <param name="obj">区域数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(Location obj);
         /// <summary>
         /// 删除区域数据。
         /// </summary>
         /// <param name="key">区域数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(string key);
         /// <summary>
         /// 获取区域数据。
         /// </summary>
         /// <param name="key">区域数据标识符.</param>
         /// <returns>MethodReturnResult&lt;Location&gt;，区域数据.</returns>
         [OperationContract]
         MethodReturnResult<Location> Get(string key);
         /// <summary>
         /// 获取区域数据集合。
         /// </summary>
         /// <param name="cfg">查询区域。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;Location&gt;&gt;，区域数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<Location>> Get(ref PagingConfig cfg);
    }
}
