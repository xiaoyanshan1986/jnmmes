using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.DataAccess.Interface.PPM;
using ServiceCenter.MES.Model.PPM;
using NHibernate;
using ServiceCenter.Common.DataAccess.NHibernate;


namespace ServiceCenter.MES.DataAccess.PPM
{
    /// <summary>
    /// 表示工单BOM的数据访问类。
    /// </summary>
    public class WorkOrderBOMDataEngine
        : DatabaseDataEngine<WorkOrderBOM, WorkOrderBOMKey>
        , IWorkOrderBOMDataEngine
    {
        public WorkOrderBOMDataEngine(ISessionFactory sf)
            : base(sf)
        {
        }

    }
}
