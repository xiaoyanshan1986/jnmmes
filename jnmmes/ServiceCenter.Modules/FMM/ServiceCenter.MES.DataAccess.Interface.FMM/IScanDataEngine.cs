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
    /// 表示班别数据访问接口。
    /// </summary>
    public interface IScanDataEngine
        : IDatabaseDataEngine<Scan, string>
    {

    }
}
