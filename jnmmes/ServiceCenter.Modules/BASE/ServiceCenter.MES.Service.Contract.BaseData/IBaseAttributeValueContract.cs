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
    /// 定义基础属性数据值数据管理契约接口。
    /// </summary>
    [ServiceContract]
    public interface IBaseAttributeValueContract
    {
        /// <summary>
        /// 添加基础属性数据值。
        /// </summary>
        /// <param name="obj">基础属性数据值数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        [OperationContract]
        MethodReturnResult Add(BaseAttributeValue obj);

        /// <summary>
        /// 添加基础属性数据值。
        /// </summary>
        /// <param name="lst">基础属性数据值数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        [OperationContract(Name="AddList")]
        MethodReturnResult Add(IList<BaseAttributeValue> lst);

        /// <summary>
        /// 修改基础属性数据值。
        /// </summary>
        /// <param name="obj">基础属性数据值数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        [OperationContract]
        MethodReturnResult Modify(BaseAttributeValue obj);
        /// <summary>
        /// 修改基础属性数据值。
        /// </summary>
        /// <param name="lst">基础属性数据值数据。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        [OperationContract(Name = "ModifyList")]
        MethodReturnResult Modify(IList<BaseAttributeValue> lst);
        /// <summary>
        /// 删除基础属性数据值。
        /// </summary>
        /// <param name="key">基础属性数据值主键。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        [OperationContract]
        MethodReturnResult Delete(BaseAttributeValueKey key);
        /// <summary>
        /// 删除基础属性数据值。
        /// </summary>
        /// <param name="categoryName">基础数据分类名。</param>
        /// <param name="itemOrder">基础数据行号。</param>
        /// <returns><see cref="MethodReturnResult" />.</returns>
        [OperationContract(Name="DeleteByCategoryName")]
        MethodReturnResult Delete(string categoryName,int itemOrder);
        /// <summary>
        /// 获取基础属性数据值数据。
        /// </summary>
        /// <param name="name">基础属性数据值名.</param>
        /// <returns><see cref="MethodReturnResult&lt;BaseAttributeValue&gt;" />,基础属性数据值数据.</returns>
        [OperationContract]
        MethodReturnResult<BaseAttributeValue> Get(BaseAttributeValueKey key);
        /// <summary>
        /// 获取基础属性数据值数据集合。
        /// </summary>
        /// <param name="cfg">查询参数.</param>
        /// <returns>MethodReturnResult&lt;IList&lt;BaseAttributeValue&gt;&gt;，基础属性数据值数据集合.</returns>
        [OperationContract(Name = "GetList")]
        MethodReturnResult<IList<BaseAttributeValue>> Get(ref PagingConfig cfg);
    }
}
