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
    /// 表示采集参数数据访问类。
    /// </summary>
    public class ParameterDataEngine
        : DatabaseDataEngine<Parameter, string>
        , IParameterDataEngine
    {
        public ParameterDataEngine(ISessionFactory sf) : base(sf) { }
    }
}
