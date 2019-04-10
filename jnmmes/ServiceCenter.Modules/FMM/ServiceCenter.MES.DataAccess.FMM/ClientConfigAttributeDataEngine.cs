using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.DataAccess.Interface.FMM;
using ServiceCenter.MES.Model.FMM;

using NHibernate;
using NHibernate.Cfg;
using ServiceCenter.DataAccess;
using ServiceCenter.Model;
using ServiceCenter.Common.DataAccess.NHibernate;


namespace ServiceCenter.MES.DataAccess.FMM
{
    /// <summary>
    /// 表示客户端配置属性数据访问类。
    /// </summary>
    public class ClientConfigAttributeDataEngine
        : DatabaseDataEngine<ClientConfigAttribute, ClientConfigAttributeKey>
        , IClientConfigAttributeDataEngine
    {
        public ClientConfigAttributeDataEngine(ISessionFactory sf):base(sf)
        {
        }
    }
}
