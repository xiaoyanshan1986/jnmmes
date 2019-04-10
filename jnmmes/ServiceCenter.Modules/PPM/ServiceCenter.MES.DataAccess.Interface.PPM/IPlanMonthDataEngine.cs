using ServiceCenter.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceCenter.MES.Model.PPM;
using System.Threading.Tasks;

namespace ServiceCenter.MES.DataAccess.Interface.PPM
{
    //生产计划管理接口
    public interface IPlanMonthDataEngine : IDatabaseDataEngine<PlanMonth, PlanMonthKey>
    {
    }
}
