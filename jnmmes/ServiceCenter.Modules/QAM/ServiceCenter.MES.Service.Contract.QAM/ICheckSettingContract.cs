using ServiceCenter.MES.Model.QAM;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.Contract.QAM
{
    /// <summary>
    /// 定义检验设置数据服务契约。
    /// </summary>
     [ServiceContract]
    public interface ICheckSettingContract
    {
         /// <summary>
         /// 添加检验设置数据。
         /// </summary>
         /// <param name="obj">检验设置数据。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(CheckSetting obj);
         /// <summary>
         /// 修改检验设置数据。
         /// </summary>
         /// <param name="obj">检验设置数据。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(CheckSetting obj);
         /// <summary>
         /// 删除检验设置数据。
         /// </summary>
         /// <param name="key">检验设置数据标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(string key);
         /// <summary>
         /// 获取检验设置数据。
         /// </summary>
         /// <param name="key">检验设置数据标识符.</param>
         /// <returns>MethodReturnResult&lt;CheckSetting&gt;，检验设置数据.</returns>
         [OperationContract]
         MethodReturnResult<CheckSetting> Get(string key);
         /// <summary>
         /// 获取检验设置数据集合。
         /// </summary>
         /// <param name="cfg">查询检验设置。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;CheckSetting&gt;&gt;，检验设置数据集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<CheckSetting>> Get(ref PagingConfig cfg);
    }
}
