using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.DataAccess.Interface.EMS;
using ServiceCenter.MES.Model.EMS;
using NHibernate;
using ServiceCenter.Common.DataAccess.NHibernate;


namespace ServiceCenter.MES.DataAccess.EMS
{
    /// <summary>
    /// 表示设备数据访问类。
    /// </summary>
    public class EquipmentStateEventDataEngine
        : DatabaseDataEngine<EquipmentStateEvent, string>
        , IEquipmentStateEventDataEngine
    {
        public EquipmentStateEventDataEngine(ISessionFactory sf)
            : base(sf)
        {
        }

    }
}
