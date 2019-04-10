using ServiceCenter.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceCenter.MES.Model.FMM;
using System.Threading.Tasks;

namespace ServiceCenter.MES.DataAccess.Interface.FMM
{
    ////设备异常类型耗时管理接口
    public interface IEquipmentConsumingDataEngine : IDatabaseDataEngine<EquipmentConsuming, EquipmentConsumingKey>
    {
    }
}
