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
    public class DataAcquisitionTransDataEngine
        : DatabaseDataEngine<DataAcquisitionTrans, DataAcquisitionTransKey>
        , IDataAcquisitionTransDataEngine
    {
        public DataAcquisitionTransDataEngine(ISessionFactory sf)
            : base(sf)
        {
        }
    }
}
