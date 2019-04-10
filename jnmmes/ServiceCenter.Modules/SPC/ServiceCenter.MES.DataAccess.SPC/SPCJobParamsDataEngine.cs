using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.DataAccess.Interface.SPC;
using ServiceCenter.MES.Model.SPC;

using NHibernate;
using ServiceCenter.Common.DataAccess.NHibernate;

namespace ServiceCenter.MES.DataAccess.SPC
{
    public class SPCJobParamsDataEngine
        : DatabaseDataEngine<SPCJobParam, SPCJobParamKey>
        , ISPCJobParamsDataEngine
    {
        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="sf">The sf.</param>

        public SPCJobParamsDataEngine(ISessionFactory sf)
            : base(sf)
        {

        }
    }
}
