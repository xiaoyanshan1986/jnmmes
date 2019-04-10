using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.Model.BaseData;
using ServiceCenter.DataAccess;

namespace ServiceCenter.MES.DataAccess.Interface.BaseData
{
    /// <summary>
    /// 表示基础属性值数据访问接口。
    /// </summary>
    public interface IBaseAttributeValueDataEngine :
        IDatabaseDataEngine<BaseAttributeValue, BaseAttributeValueKey>
    {
        /// <summary>
        /// 获取基础属性值集合。
        /// </summary>
        /// <param name="categoryName">基础属性分类名称。</param>
        /// <returns>基础属性值集合。</returns>
        IList<BaseAttributeValue> GetList(string categoryName);
        /// <summary>
        /// 获取基础属性值集合。
        /// </summary>
        /// <param name="categoryName">基础属性分类名称</param>
        /// <param name="attributeName">基础属性名称</param>
        /// <returns>基础属性值集合。</returns>
        IList<BaseAttributeValue> GetList(string categoryName, string attributeName);

    }
}
