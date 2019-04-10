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
    /// 表示基础属性数据访问接口。
    /// </summary>
    public interface IBaseAttributeDataEngine 
        : IDatabaseDataEngine<BaseAttribute, BaseAttributeKey>
    {
    }
}
