using ServiceCenter.MES.Model.ERP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.DataAccess;


namespace ServiceCenter.MES.DataAccess.Interface.ERP
{  
    public interface IWOReportDataEngine : IDatabaseDataEngine<WOReport, string>
    {
    }

}
