using ServiceCenter.MES.Model.WIP;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.Contract.WIP
{
    /// <summary>
    /// 定义OEM批次数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IOemDataContract
    {
         /// <summary>
         /// 添加OEM批次数据。
         /// </summary>
         /// <param name="obj">OEM批次数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(OemData obj);
         /// <summary>
         /// 修改OEM批次数据。
         /// </summary>
         /// <param name="obj">OEM批次数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(OemData obj);
         /// <summary>
         /// 删除OEM批次数据。
         /// </summary>
         /// <param name="key">OEM批次数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(string key);
         /// <summary>
         /// 获取OEM批次数据。
         /// </summary>
         /// <param name="key">OEM批次数据标识符.</param>
         /// <returns>MethodReturnResult&lt;OemData&gt;，OEM批次数据.</returns>
         [OperationContract]
         MethodReturnResult<OemData> Get(string key);
         /// <summary>
         /// 获取OEM批次数据集合。
         /// </summary>
         /// <param name="cfg">查询OEM批次。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;OemData&gt;&gt;，OEM批次数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<OemData>> Get(ref PagingConfig cfg);
    }
}
