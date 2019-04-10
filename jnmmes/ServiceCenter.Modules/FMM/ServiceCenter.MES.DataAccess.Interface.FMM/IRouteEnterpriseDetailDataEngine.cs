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
    /// 表示工艺流程组明细数据的数据访问接口。
    /// </summary>
    public interface IRouteEnterpriseDetailDataEngine : IDatabaseDataEngine<RouteEnterpriseDetail, RouteEnterpriseDetailKey>
    {
    }
}
