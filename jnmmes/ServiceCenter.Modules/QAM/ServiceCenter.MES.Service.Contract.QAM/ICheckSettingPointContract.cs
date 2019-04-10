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
    /// 定义检验设置点服务契约。
    /// </summary>
     [ServiceContract]
    public interface ICheckSettingPointContract
    {
         /// <summary>
         /// 添加检验设置点。
         /// </summary>
         /// <param name="obj">检验设置点。</param>
         /// <returns><see cref="MethodReturnResult"/></returns>
         [OperationContract]
         MethodReturnResult Add(CheckSettingPoint obj);
         /// <summary>
         /// 修改检验设置点。
         /// </summary>
         /// <param name="obj">检验设置点。</param>
         /// <returns><see cref="MethodReturnResult" /></returns>
         [OperationContract]
         MethodReturnResult Modify(CheckSettingPoint obj);
         /// <summary>
         /// 删除检验设置点。
         /// </summary>
         /// <param name="key">检验设置点标识符.</param>
         /// <returns>MethodReturnResult.</returns>
         [OperationContract]
         MethodReturnResult Delete(CheckSettingPointKey key);
         /// <summary>
         /// 获取检验设置点。
         /// </summary>
         /// <param name="key">检验设置点标识符.</param>
         /// <returns>MethodReturnResult&lt;CheckSettingPoint&gt;，检验设置点.</returns>
         [OperationContract]
         MethodReturnResult<CheckSettingPoint> Get(CheckSettingPointKey key);
         /// <summary>
         /// 获取检验设置点集合。
         /// </summary>
         /// <param name="cfg">查询检验设置点。</param>
         /// <returns>MethodReturnResult&lt;IList&lt;CheckSettingPoint&gt;&gt;，检验设置点集合。</returns>
         [OperationContract(Name="GetList")]
         MethodReturnResult<IList<CheckSettingPoint>> Get(ref PagingConfig cfg);
    }
}
