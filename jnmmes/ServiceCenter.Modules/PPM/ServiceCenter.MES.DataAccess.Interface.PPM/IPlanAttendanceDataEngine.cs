using ServiceCenter.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceCenter.MES.Model.PPM;
using System.Threading.Tasks;

namespace ServiceCenter.MES.DataAccess.Interface.PPM
{
    //排班计划管理接口
    public interface IPlanAttendanceDataEngine : IDatabaseDataEngine<PlanAttendance, PlanAttendanceKey>
    {
    }
}
