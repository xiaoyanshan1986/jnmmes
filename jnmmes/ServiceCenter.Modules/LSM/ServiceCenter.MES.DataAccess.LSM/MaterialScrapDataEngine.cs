using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.DataAccess.Interface.LSM;
using ServiceCenter.MES.Model.LSM;
using NHibernate;
using NHibernate.Cfg;
using ServiceCenter.Common.DataAccess.NHibernate;


namespace ServiceCenter.MES.DataAccess.LSM
{
    /// <summary>
    /// 表示报废单数据访问类。
    /// </summary>
    public class MaterialScrapDataEngine
        : DatabaseDataEngine<MaterialScrap, string>
        ,IMaterialScrapDataEngine
    {
        public MaterialScrapDataEngine(ISessionFactory sf): base(sf) { }
    }
}
