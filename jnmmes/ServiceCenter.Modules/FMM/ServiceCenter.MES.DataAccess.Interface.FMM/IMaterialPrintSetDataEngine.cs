using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.Model.FMM;
using ServiceCenter.DataAccess;

namespace ServiceCenter.MES.DataAccess.Interface.FMM
{
    /// <summary>
    /// 表示物料产品标签设置数据访问接口。
    /// </summary>
    public interface IMaterialPrintSetDataEngine
        : IDatabaseDataEngine<MaterialPrintSet, MaterialPrintSetKey>
    {
    }
}
