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
    /// 定义基础属性分类数据管理契约接口。
    /// </summary>
    [ServiceContract]
    public interface IBaseAttributeCategoryContract
    {
        /// <summary>
        /// 添加基础属性分类。
        /// </summary>
        /// <param name="obj">基础属性分类数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        [OperationContract]
        MethodReturnResult Add(BaseAttributeCategory obj);
        /// <summary>
        /// 修改基础属性分类。
        /// </summary>
        /// <param name="obj">基础属性分类数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        [OperationContract]
        MethodReturnResult Modify(BaseAttributeCategory obj);
        /// <summary>
        /// 删除基础属性分类。
        /// </summary>
        /// <param name="name">基础属性分类名。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        [OperationContract]
        MethodReturnResult Delete(string name);
        /// <summary>
        /// 获取基础属性分类数据。
        /// </summary>
        /// <param name="name">基础属性分类名.</param>
        /// <returns><see cref="MethodReturnResult&lt;BaseAttributeCategory&gt;" />,基础属性分类数据.</returns>
        [OperationContract]
        MethodReturnResult<BaseAttributeCategory> Get(string name);
        /// <summary>
        /// 获取基础属性分类数据集合。
        /// </summary>
        /// <param name="cfg">查询参数.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;BaseAttributeCategory&gt;&gt;，基础属性分类数据集合.</returns>
        [OperationContract(Name = "GetList")]
        MethodReturnResult<IList<BaseAttributeCategory>> Get(ref PagingConfig cfg);
    }
}
