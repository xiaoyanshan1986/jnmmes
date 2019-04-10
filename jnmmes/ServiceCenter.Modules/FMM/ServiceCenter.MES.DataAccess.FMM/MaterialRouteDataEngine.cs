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
    /// 表示物料类型加工工艺流程租数据访问类。
    /// </summary>
    public class MaterialRouteDataEngine
        : DatabaseDataEngine<MaterialRoute, MaterialRouteKey>
        , IMaterialRouteDataEngine
    {
        public MaterialRouteDataEngine(ISessionFactory sf) : base(sf) { }
    }
}
