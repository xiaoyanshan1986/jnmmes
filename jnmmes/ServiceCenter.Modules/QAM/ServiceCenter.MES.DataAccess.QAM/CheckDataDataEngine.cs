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
    /// 表示检验数据的数据访问类。
    /// </summary>
    public class CheckDataDataEngine
        : DatabaseDataEngine<CheckData, string>
        , ICheckDataDataEngine
    {
        public CheckDataDataEngine(ISessionFactory sf)
            : base(sf)
        {
        }

    }
}
