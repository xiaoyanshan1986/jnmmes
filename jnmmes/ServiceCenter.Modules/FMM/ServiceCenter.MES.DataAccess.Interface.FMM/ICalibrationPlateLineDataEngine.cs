using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.DataAccess;
using ServiceCenter.Model;

namespace ServiceCenter.MES.DataAccess.Interface.FMM
{
    /// <summary>
    /// 表示校准板线别数据访问接口。
    /// </summary>
    public interface ICalibrationPlateLineDataEngine
        : IDatabaseDataEngine<CalibrationPlateLine, CalibrationPlateLineKey>
    {

    }
}
