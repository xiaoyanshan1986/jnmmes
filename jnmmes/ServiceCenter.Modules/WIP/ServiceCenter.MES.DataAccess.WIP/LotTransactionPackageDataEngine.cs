using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.Model.WIP;
using NHibernate;

using ServiceCenter.Model;
using ServiceCenter.MES.DataAccess.Interface.WIP;
using ServiceCenter.Common.DataAccess.NHibernate;

namespace ServiceCenter.MES.DataAccess.WIP
{
    /// <summary>
    /// 在制品批次包装数据访问类。
    /// </summary>
    public class LotTransactionPackageDataEngine
        : DatabaseDataEngine<LotTransactionPackage, string>
        , ILotTransactionPackageDataEngine
    {

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="sf"></param>
        public LotTransactionPackageDataEngine(ISessionFactory sf):base(sf)
        {
        }
    }
}
