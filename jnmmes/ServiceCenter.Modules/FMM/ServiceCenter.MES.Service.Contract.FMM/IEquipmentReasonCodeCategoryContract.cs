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
    /// 定义代码分组数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface IEquipmentReasonCodeCategoryContract
    {
         /// <summary>
         /// 添加代码分组数据。
         /// </summary>
         /// <param name="obj">代码分组数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(EquipmentReasonCodeCategory obj);
         /// <summary>
         /// 修改代码分组数据。
         /// </summary>
         /// <param name="obj">代码分组数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(EquipmentReasonCodeCategory obj);
         /// <summary>
         /// 删除代码分组数据。
         /// </summary>
         /// <param name="key">代码分组数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(string key);
         /// <summary>
         /// 获取代码分组数据。
         /// </summary>
         /// <param name="key">代码分组数据标识符.</param>
         /// <returns>MethodReturnResult&lt;ReasonCodeCategory&gt;，代码分组数据.</returns>
         [OperationContract]
         MethodReturnResult<EquipmentReasonCodeCategory> Get(string key);
         /// <summary>
         /// 获取代码分组数据集合。
         /// </summary>
         /// <param name="cfg">查询参数。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;ReasonCodeCategory&gt;&gt;，代码分组数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<EquipmentReasonCodeCategory>> Get(ref PagingConfig cfg);
    }
}
