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
    /// 定义检验设置点明细服务契约。
    /// </summary>
     [ServiceContract]
    public interface ICheckSettingPointDetailContract
    {
         /// <summary>
         /// 添加检验设置点明细。
         /// </summary>
         /// <param name="obj">检验设置点明细。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(CheckSettingPointDetail obj);
         /// <summary>
         /// 修改检验设置点明细。
         /// </summary>
         /// <param name="obj">检验设置点明细。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(CheckSettingPointDetail obj);
         /// <summary>
         /// 删除检验设置点明细。
         /// </summary>
         /// <param name="key">检验设置点明细标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(CheckSettingPointDetailKey key);
         /// <summary>
         /// 获取检验设置点明细。
         /// </summary>
         /// <param name="key">检验设置点明细标识符.</param>
         /// <returns>MethodReturnResult&lt;CheckSettingPointDetail&gt;，检验设置点明细.</returns>
         [OperationContract]
         MethodReturnResult<CheckSettingPointDetail> Get(CheckSettingPointDetailKey key);
         /// <summary>
         /// 获取检验设置点明细集合。
         /// </summary>
         /// <param name="cfg">查询检验设置点明细。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;CheckSettingPointDetail&gt;&gt;，检验设置点明细集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<CheckSettingPointDetail>> Get(ref PagingConfig cfg);
    }
}
