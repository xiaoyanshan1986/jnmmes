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
    public class DataAcquisitionDataDataEngine
        : DatabaseDataEngine<DataAcquisitionData, DataAcquisitionDataKey>
        , IDataAcquisitionDataDataEngine
    {
        public DataAcquisitionDataDataEngine(ISessionFactory sf)
            : base(sf)
        {
        }
    }
}
