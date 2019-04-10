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
    /// 表示工单产品的数据访问类。
    /// </summary>
    public class WorkOrderProductDataEngine
        : DatabaseDataEngine<WorkOrderProduct, WorkOrderProductKey>
        , IWorkOrderProductDataEngine
    {
        public WorkOrderProductDataEngine(ISessionFactory sf)
            : base(sf)
        {
        }

    }
}
