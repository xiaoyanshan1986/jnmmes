using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.DataAccess;
using ServiceCenter.Model;
using ServiceCenter.MES.Model.SPC;

namespace ServiceCenter.MES.DataAccess.Interface.SPC
{
    public interface ISPCJobDataEngine : IDatabaseDataEngine<SPCJob, string>
    {
    }
}
