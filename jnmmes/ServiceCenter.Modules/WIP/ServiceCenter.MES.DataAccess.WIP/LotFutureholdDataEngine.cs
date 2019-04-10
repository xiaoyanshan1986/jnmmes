using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.Model.WIP;

using ServiceCenter.Model;
using ServiceCenter.MES.DataAccess.Interface.WIP;
using NHibernate;
using ServiceCenter.Common.DataAccess.NHibernate;

namespace ServiceCenter.MES.DataAccess.WIP
{
    /// <summary>
    /// 批次预设暂停数据访问类。
    /// </summary>
    public class LotFutureholdDataEngine
        : DatabaseDataEngine<LotFuturehold, string>, ILotFutureholdDataEngine
    {
        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="sf"></param>
        public LotFutureholdDataEngine(ISessionFactory sf):base(sf)
        {
        }

        public LotFuturehold Get(string lotNumber, string stepName, string action)
        {
            throw new NotImplementedException();
        }
    }
}
