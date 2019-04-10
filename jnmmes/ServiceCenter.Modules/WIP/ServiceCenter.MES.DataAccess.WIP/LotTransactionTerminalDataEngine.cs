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
    /// 批次终止操作数据访问类。
    /// </summary>
    public class LotTransactionTerminalDataEngine
        : DatabaseDataEngine<LotTransactionTerminal, string>, ILotTransactionTerminalDataEngine
    {
        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="sf"></param>
        public LotTransactionTerminalDataEngine(ISessionFactory sf)
            : base(sf)
        {
        }
    }
}
