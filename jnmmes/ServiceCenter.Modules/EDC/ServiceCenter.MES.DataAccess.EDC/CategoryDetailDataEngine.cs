using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.DataAccess.Interface.EDC;
using ServiceCenter.MES.Model.EDC;
using NHibernate;
using ServiceCenter.Common.DataAccess.NHibernate;


namespace ServiceCenter.MES.DataAccess.EDC
{
    /// <summary>
    /// 表示采集参数数据访问类。
    /// </summary>
    public class CategoryDetailDataEngine
        : DatabaseDataEngine<CategoryDetail, CategoryDetailKey>
        , ICategoryDetailDataEngine
    {
        public CategoryDetailDataEngine(ISessionFactory sf)
            : base(sf)
        {
        }

    }
}
