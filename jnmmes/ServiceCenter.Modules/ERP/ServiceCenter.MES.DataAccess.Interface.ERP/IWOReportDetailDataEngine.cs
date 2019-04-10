using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.DataAccess;
using ServiceCenter.MES.Model.ERP;

namespace ServiceCenter.MES.DataAccess.Interface.ERP
{
    public interface IWOReportDetailDataEngine : IDatabaseDataEngine<WOReportDetail, WOReportDetailKey>
    {
    }
   
}
