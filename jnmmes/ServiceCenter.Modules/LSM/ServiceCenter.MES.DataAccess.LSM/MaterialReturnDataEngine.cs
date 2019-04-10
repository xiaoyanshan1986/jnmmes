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
    /// 表示退料数据访问类。
    /// </summary>
    public class MaterialReturnDataEngine
        : DatabaseDataEngine<MaterialReturn, string>
        ,IMaterialReturnDataEngine
    {
        public MaterialReturnDataEngine(ISessionFactory sf): base(sf) { }
    }
}
