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
    /// 表示检验数据明细的数据访问类。
    /// </summary>
    public class CheckDataDetailDataEngine
        : DatabaseDataEngine<CheckDataDetail, CheckDataDetailKey>
        , ICheckDataDetailDataEngine
    {
        public CheckDataDetailDataEngine(ISessionFactory sf)
            : base(sf)
        {
        }

    }
}
