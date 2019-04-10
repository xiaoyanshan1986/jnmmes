using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.DataAccess;
using NHibernate;

namespace ServiceCenter.MES.DataAccess.Interface.FMM
{
    /// <summary>
    /// 表示设备切换状态数据访问接口。
    /// </summary>
    public interface IEquipmentChangeStateDataEngine 
        : IDatabaseDataEngine<EquipmentChangeState, string>
    {
        /// <summary>
        /// 根据源状态和目的状态获取设备切换状态数据。
        /// </summary>
        /// <param name="from">源状态。</param>
        /// <param name="to">目的状态。</param>
        /// <returns>设备切换状态数据。</returns>
        EquipmentChangeState Get(string from, string to);

        EquipmentChangeState Get(string from, string to, ISession session);
    }
}
