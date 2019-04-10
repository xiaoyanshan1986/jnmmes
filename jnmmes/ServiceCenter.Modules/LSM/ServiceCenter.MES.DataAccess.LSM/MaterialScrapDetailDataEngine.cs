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
    /// 表示报废单明细数据访问类。
    /// </summary>
    public class MaterialScrapDetailDataEngine
        : DatabaseDataEngine<MaterialScrapDetail, MaterialScrapDetailKey>
        ,IMaterialScrapDetailDataEngine
    {
        public MaterialScrapDetailDataEngine(ISessionFactory sf): base(sf) { }
    }
}
