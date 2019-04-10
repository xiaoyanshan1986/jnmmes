using ServiceCenter.MES.Model.BaseData;
using ServiceCenter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCenter.MES.Service.Contract.BaseData
{
    /// <summary>
    /// 定义基础属性数据管理契约接口。
    /// </summary>
    [ServiceContract]
    public interface IBaseAttributeContract
    {
        /// <summary>
        /// 添加基础属性。
        /// </summary>
        /// <param name="obj">基础属性数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        [OperationContract]
        MethodReturnResult Add(BaseAttribute obj);
        /// <summary>
        /// 修改基础属性。
        /// </summary>
        /// <param name="obj">基础属性数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        [OperationContract]
        MethodReturnResult Modify(BaseAttribute obj);
        /// <summary>
        /// 删除基础属性。
        /// </summary>
        /// <param name="name">基础属性名。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        [OperationContract]
        MethodReturnResult Delete(BaseAttributeKey name);
        /// <summary>
        /// 获取基础属性数据。
        /// </summary>
        /// <param name="name">基础属性名.</param>
        /// <returns><see cref="MethodReturnResult&lt;BaseAttribute&gt;" />,基础属性数据.</returns>
        [OperationContract]
        MethodReturnResult<BaseAttribute> Get(BaseAttributeKey name);
        /// <summary>
        /// 获取基础属性数据集合。
        /// </summary>
        /// <param name="cfg">查询参数.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;BaseAttribute&gt;&gt;，基础属性数据集合.</returns>
        [OperationContract(Name = "GetList")]
        MethodReturnResult<IList<BaseAttribute>> Get(ref PagingConfig cfg);
    }
}
