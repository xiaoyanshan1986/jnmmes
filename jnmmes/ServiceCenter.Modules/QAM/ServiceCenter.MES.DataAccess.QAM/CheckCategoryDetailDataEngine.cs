using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.DataAccess.Interface.QAM;
using ServiceCenter.MES.Model.QAM;
using NHibernate;
using ServiceCenter.Common.DataAccess.NHibernate;


namespace ServiceCenter.MES.DataAccess.QAM
{
    /// <summary>
    /// 表示检验参数数据访问类。
    /// </summary>
    public class CheckCategoryDetailDataEngine
        : DatabaseDataEngine<CheckCategoryDetail, CheckCategoryDetailKey>
        , ICheckCategoryDetailDataEngine
    {
        public CheckCategoryDetailDataEngine(ISessionFactory sf)
            : base(sf)
        {
        }

    }
}
