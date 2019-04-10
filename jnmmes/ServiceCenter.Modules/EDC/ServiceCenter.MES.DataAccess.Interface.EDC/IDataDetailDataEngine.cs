using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.Model.EDC;
using ServiceCenter.DataAccess;

namespace ServiceCenter.MES.DataAccess.Interface.EDC
{
    /// <summary>
    /// 表示采集数据明细的数据访问接口。
    /// </summary>
    public interface IDataDetailDataEngine : IDatabaseDataEngine<DataDetail, DataDetailKey>
    {
    }
}
