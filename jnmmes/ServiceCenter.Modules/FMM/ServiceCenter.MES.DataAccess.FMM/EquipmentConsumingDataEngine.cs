
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.DataAccess.Interface.FMM;
using ServiceCenter.MES.Model.FMM;
using NHibernate;
using ServiceCenter.Common.DataAccess.NHibernate;

namespace ServiceCenter.MES.DataAccess.FMM
{
    //设备异常类型耗时管理
    public class EquipmentConsumingDataEngine
        : DatabaseDataEngine<EquipmentConsuming, EquipmentConsumingKey>
        , IEquipmentConsumingDataEngine
    {
        public EquipmentConsumingDataEngine(ISessionFactory sf) : base(sf) { }
    }
}