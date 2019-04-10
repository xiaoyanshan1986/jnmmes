using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.DataAccess.Interface.FMM;
using ServiceCenter.MES.Model.FMM;
using NHibernate;
using NHibernate.Cfg;
using ServiceCenter.Common.DataAccess.NHibernate;


namespace ServiceCenter.MES.DataAccess.FMM
{
    /// <summary>
    /// 表示客户端配置数据访问类。
    /// </summary>
    public class ClientConfigDataEngine
        : DatabaseDataEngine<ClientConfig, string>
        ,IClientConfigDataEngine
    {
        public ClientConfigDataEngine(ISessionFactory sf): base(sf) { }
    }
}
