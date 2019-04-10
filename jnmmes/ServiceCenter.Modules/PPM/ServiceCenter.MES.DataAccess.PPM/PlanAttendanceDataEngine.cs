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
    //排班计划管理
    public class PlanAttendanceDataEngine
        : DatabaseDataEngine<PlanAttendance, PlanAttendanceKey>
        , IPlanAttendanceDataEngine
    {
        public PlanAttendanceDataEngine(ISessionFactory sf) : base(sf) { }
    }
}
