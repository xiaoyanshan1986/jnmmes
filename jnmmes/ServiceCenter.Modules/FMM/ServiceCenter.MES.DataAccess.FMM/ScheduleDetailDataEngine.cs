using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.DataAccess.Interface.FMM;
using ServiceCenter.MES.Model.FMM;
using NHibernate;
using ServiceCenter.Common.DataAccess.NHibernate;


namespace ServiceCenter.MES.DataAccess.FMM
{
    /// <summary>
    /// 表示排班计划明细数据访问类。
    /// </summary>
    public class ScheduleDetailDataEngine
        : DatabaseDataEngine<ScheduleDetail, ScheduleDetailKey>
        ,IScheduleDetailDataEngine
    {
        public ScheduleDetailDataEngine(ISessionFactory sf) : base(sf) { }
    }
}
