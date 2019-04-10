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
    /// 定义效率档配置数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IEfficiencyConfigurationContract
    {
         /// <summary>
         /// 添加效率档配置数据。
         /// </summary>
         /// <param name="obj">效率档配置数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(EfficiencyConfiguration obj);
         /// <summary>
         /// 修改效率档配置数据。
         /// </summary>
         /// <param name="obj">效率档配置数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(EfficiencyConfiguration obj);
         /// <summary>
         /// 删除效率档配置数据。
         /// </summary>
         /// <param name="key">效率档配置数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(EfficiencyConfigurationKey key);
         /// <summary>
         /// 获取效率档配置数据。
         /// </summary>
         /// <param name="key">效率档配置数据标识符.</param>
         /// <returns>MethodReturnResult&lt;EfficiencyConfiguration&gt;，效率档配置数据.</returns>
         [OperationContract]
         MethodReturnResult<EfficiencyConfiguration> Get(EfficiencyConfigurationKey key);
         /// <summary>
         /// 获取效率档配置数据集合。
         /// </summary>
         /// <param name="cfg">查询效率档配置。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;EfficiencyConfiguration&gt;&gt;，效率档配置数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<EfficiencyConfiguration>> Get(ref PagingConfig cfg);
    }
}
