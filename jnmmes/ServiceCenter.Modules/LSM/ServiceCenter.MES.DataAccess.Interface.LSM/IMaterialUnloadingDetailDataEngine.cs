using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.Model.LSM;
using ServiceCenter.DataAccess;

namespace ServiceCenter.MES.DataAccess.Interface.LSM
{
    /// <summary>
    /// 定义下料明细数据访问接口。
    /// </summary>
    public interface IMaterialUnloadingDetailDataEngine
        : IDatabaseDataEngine<MaterialUnloadingDetail, MaterialUnloadingDetailKey>
    {
    }
}
