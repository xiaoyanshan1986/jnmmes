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
    /// <summary>
    /// 表示校准板线别数据访问类。
    /// </summary>
    public class CalibrationPlateLineDataEngine
            : DatabaseDataEngine<CalibrationPlateLine, CalibrationPlateLineKey>
            ,ICalibrationPlateLineDataEngine
    {
        public CalibrationPlateLineDataEngine(ISessionFactory sf) : base(sf) { }
    }
}
