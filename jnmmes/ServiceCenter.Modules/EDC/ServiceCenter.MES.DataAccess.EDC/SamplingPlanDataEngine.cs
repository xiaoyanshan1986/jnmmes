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
    /// <summary>
    /// 表示采集计划数据访问类。
    /// </summary>
    public class SamplingPlanDataEngine
        : DatabaseDataEngine<SamplingPlan, string>
        , ISamplingPlanDataEngine
    {
        public SamplingPlanDataEngine(ISessionFactory sf)
            : base(sf)
        {
        }

    }
}
