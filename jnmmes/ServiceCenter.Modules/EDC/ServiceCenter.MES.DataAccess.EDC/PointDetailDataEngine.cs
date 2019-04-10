using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.DataAccess.Interface.EDC;
using ServiceCenter.MES.Model.EDC;
using NHibernate;
using ServiceCenter.Common.DataAccess.NHibernate;


namespace ServiceCenter.MES.DataAccess.EDC
{
    /// <summary>
    /// 表示采集点设置明细数据访问类。
    /// </summary>
    public class PointDetailDataEngine
        : DatabaseDataEngine<PointDetail, PointDetailKey>
        , IPointDetailDataEngine
    {
        public PointDetailDataEngine(ISessionFactory sf)
            : base(sf)
        {
        }

    }
}
