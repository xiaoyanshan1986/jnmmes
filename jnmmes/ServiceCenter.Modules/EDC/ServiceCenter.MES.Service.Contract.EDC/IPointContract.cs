using ServiceCenter.MES.Model.EDC;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.Contract.EDC
{
    /// <summary>
    /// 定义采集点设置数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IPointContract
    {
         /// <summary>
         /// 添加采集点设置数据。
         /// </summary>
         /// <param name="obj">采集点设置数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(Point obj);
         /// <summary>
         /// 修改采集点设置数据。
         /// </summary>
         /// <param name="obj">采集点设置数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(Point obj);
         /// <summary>
         /// 删除采集点设置数据。
         /// </summary>
         /// <param name="key">采集点设置数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(string key);
         /// <summary>
         /// 获取采集点设置数据。
         /// </summary>
         /// <param name="key">采集点设置数据标识符.</param>
         /// <returns>MethodReturnResult&lt;Point&gt;，采集点设置数据.</returns>
         [OperationContract]
         MethodReturnResult<Point> Get(string key);
         /// <summary>
         /// 获取采集点设置数据集合。
         /// </summary>
         /// <param name="cfg">查询采集点设置。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;Point&gt;&gt;，采集点设置数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<Point>> Get(ref PagingConfig cfg);
    }
}
