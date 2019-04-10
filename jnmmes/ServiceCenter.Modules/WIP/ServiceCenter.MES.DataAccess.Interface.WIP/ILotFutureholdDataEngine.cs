using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.Model.WIP;
using ServiceCenter.DataAccess;
using ServiceCenter.Model;

namespace ServiceCenter.MES.DataAccess.Interface.WIP
{
    /// <summary>
    /// 批次预设暂停数据访问接口。
    /// </summary>
    public interface ILotFutureholdDataEngine
        : IDatabaseDataEngine<LotFuturehold, string>
    {
        /// <summary>
        /// 获取批次预设暂停数据。
        /// </summary>
        /// <param name="lotNumber">批次号。</param>
        /// <param name="stepName">工步名称。</param>
        /// <param name="action">触发暂停的动作。</param>
        /// <returns>批次预设暂停数据。</returns>
        LotFuturehold Get(string lotNumber, string stepName, string action);
    }
}
