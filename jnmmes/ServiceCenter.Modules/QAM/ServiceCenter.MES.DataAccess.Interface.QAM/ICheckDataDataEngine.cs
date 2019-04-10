using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.Model.QAM;
using ServiceCenter.DataAccess;

namespace ServiceCenter.MES.DataAccess.Interface.QAM
{
    /// <summary>
    /// 表示检验数据的数据访问接口。
    /// </summary>
    public interface ICheckDataDataEngine 
        : IDatabaseDataEngine<CheckData, string>
    {
    }
}
