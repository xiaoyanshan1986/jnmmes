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
    //目标参数管理
    public class TargetParameterDataEngine
        : DatabaseDataEngine<TargetParameter, TargetParameterKey>
        , ITargetParameterDataEngine
    {
        public TargetParameterDataEngine(ISessionFactory sf) : base(sf) { }
    }
}
