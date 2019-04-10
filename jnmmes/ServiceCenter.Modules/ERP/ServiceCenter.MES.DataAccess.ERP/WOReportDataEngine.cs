using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using ServiceCenter.Common.DataAccess.NHibernate;
using ServiceCenter.MES.DataAccess.Interface.ERP;
using ServiceCenter.MES.Model.ERP;

namespace ServiceCenter.MES.DataAccess.ERP
{
    public class WOReportDataEngine
    : DatabaseDataEngine<WOReport, string>
        , IWOReportDataEngine
    {
        public WOReportDataEngine(ISessionFactory sf)
            : base(sf)
        {
        }

    }
}
