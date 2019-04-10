using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceCenter.MES.Model.PPM;
using ServiceCenter.DataAccess;

namespace ServiceCenter.MES.DataAccess.Interface.PPM
{
    /// <summary>
    /// 表示工单工艺的数据访问接口。
    /// </summary>
    public interface IWorkOrderRouteDataEngine
        : IDatabaseDataEngine<WorkOrderRoute, WorkOrderRouteKey>
    {
    }
}
