using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.Model.EMS;
using ServiceCenter.DataAccess;

namespace ServiceCenter.MES.DataAccess.Interface.EMS
{
    /// <summary>
    /// 表示设备状态事件数据访问接口。
    /// </summary>
    public interface IEquipmentStateEventDataEngine 
        : IDatabaseDataEngine<EquipmentStateEvent, string>
    {
    }
}
