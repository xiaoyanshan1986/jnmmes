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
    /// 定义产品编码成柜参数数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IMaterialChestParameterContract
    {
         /// <summary>
         /// 添加产品编码成柜参数数据。
         /// </summary>
         /// <param name="obj">产品编码成柜参数数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(MaterialChestParameter obj);
         /// <summary>
         /// 修改产品编码成柜参数数据。
         /// </summary>
         /// <param name="obj">产品编码成柜参数数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(MaterialChestParameter obj);
         /// <summary>
         /// 删除产品编码成柜参数数据。
         /// </summary>
         /// <param name="key">产品编码成柜参数数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(string key);
         /// <summary>
         /// 获取产品编码成柜参数数据。
         /// </summary>
         /// <param name="key">产品编码成柜参数数据标识符.</param>
         /// <returns>MethodReturnResult&lt;MaterialChestParameter&gt;，产品编码成柜参数数据.</returns>
         [OperationContract]
         MethodReturnResult<MaterialChestParameter> Get(string key);
         /// <summary>
         /// 获取产品编码成柜参数数据集合。
         /// </summary>
         /// <param name="cfg">查询产品编码成柜参数。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;MaterialChestParameter&gt;&gt;，产品编码成柜参数数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<MaterialChestParameter>> Get(ref PagingConfig cfg);
    }
}
