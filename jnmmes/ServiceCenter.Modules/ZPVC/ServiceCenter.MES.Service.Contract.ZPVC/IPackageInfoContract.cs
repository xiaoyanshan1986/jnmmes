using ServiceCenter.MES.Model.WIP;
using ServiceCenter.MES.Model.ZPVC;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.Contract.ZPVC
{
    /// <summary>
    /// 定义包装信息数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IPackageInfoContract
    {
         /// <summary>
         /// 添加包装信息数据。
         /// </summary>
         /// <param name="obj">包装信息数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(Package p, PackageInfo obj);
         /// <summary>
         /// 修改包装信息数据。
         /// </summary>
         /// <param name="obj">包装信息数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(PackageInfo obj);
         /// <summary>
         /// 删除包装信息数据。
         /// </summary>
         /// <param name="key">包装信息数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(string key);
         /// <summary>
         /// 获取包装信息数据。
         /// </summary>
         /// <param name="key">包装信息数据标识符.</param>
         /// <returns>MethodReturnResult&lt;PackageInfo&gt;，包装信息数据.</returns>
         [OperationContract]
         MethodReturnResult<PackageInfo> Get(string key);
         /// <summary>
         /// 获取包装信息数据集合。
         /// </summary>
         /// <param name="cfg">查询包装信息。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;PackageInfo&gt;&gt;，包装信息数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<PackageInfo>> Get(ref PagingConfig cfg);
    }
}
