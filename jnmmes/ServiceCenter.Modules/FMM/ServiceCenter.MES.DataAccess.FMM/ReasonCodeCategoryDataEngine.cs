using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.DataAccess.Interface.FMM;
using ServiceCenter.MES.Model.FMM;
using NHibernate;
using ServiceCenter.Common.DataAccess.NHibernate;


namespace ServiceCenter.MES.DataAccess.FMM
{
    /// <summary>
    /// 表示原因代码分组数据访问类。
    /// </summary>
    public class ReasonCodeCategoryDataEngine
        : DatabaseDataEngine<ReasonCodeCategory, string>
        , IReasonCodeCategoryDataEngine
    {
        public ReasonCodeCategoryDataEngine(ISessionFactory sf):base(sf)
        {
        }

    }
}
